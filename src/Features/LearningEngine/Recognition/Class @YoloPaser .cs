using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Reflection;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;
using Microsoft.ML.TensorFlow;
using Microsoft.Data.Analysis;

namespace DxMLEngine.Features.Recognition
{
    public class BoxDimensions : Dimensions { }
    public class CellDimensions : Dimensions { }

    public class YoloParser
    {
        private const int ROW_COUNT = 13;
        private const int COL_COUNT = 13;
        private const int CHANNEL_COUNT = 125;
        private const int BOXES_PER_CELL = 5;
        private const int BOX_INFO_FEATURE_COUNT = 5;
        private const int CLASS_COUNT = 20;
        private const float CELL_WIDTH = 32;
        private const float CELL_HEIGHT = 32;

        private const int ChannelStride = ROW_COUNT * COL_COUNT;

        private static float[] Anchors = new float[]
        {
            1.08F, 1.19F, 3.42F, 4.41F, 6.63F, 11.38F, 9.42F, 5.11F, 16.62F, 10.52F
        };

        private static string[] Labels = new string[]
        {
            "aeroplane", "bicycle", "bird", "boat", "bottle",
            "bus", "car", "cat", "chair", "cow", "diningtable",
            "dog", "horse", "motorbike", "person", "pottedplant",
            "sheep", "sofa", "train", "tvmonitor"
        };

        private static Color[] LabelColors = new Color[]
        {
            Color.Khaki,
            Color.Fuchsia,
            Color.Silver,
            Color.RoyalBlue,
            Color.Green,
            Color.DarkOrange,
            Color.Purple,
            Color.Gold,
            Color.Red,
            Color.Aquamarine,
            Color.Lime,
            Color.AliceBlue,
            Color.Sienna,
            Color.Orchid,
            Color.Tan,
            Color.LightPink,
            Color.Yellow,
            Color.HotPink,
            Color.OliveDrab,
            Color.SandyBrown,
            Color.DarkTurquoise
        };

        #region PRE-PARSING

        private static float Sigmoid(float value)
        {
            var k = (float)Math.Exp(value);
            return k / (1.0f + k);
        }

        private static float[] Softmax(float[] values)
        {
            var maxVal = values.Max();
            var exp = values.Select(val => Math.Exp(val - maxVal));
            var sumExp = exp.Sum();

            return exp.Select(v => (float)(v / sumExp)).ToArray();
        }

        private static int GetOffset(int x, int y, int channel)
        {
            return (channel * ChannelStride) + (y * COL_COUNT) + x;
        }

        private static BoxDimensions ExtractBoundingBoxDimensions(float[] modelOutput, int x, int y, int channel)
        {
            return new BoxDimensions
            {
                X = modelOutput[GetOffset(x, y, channel)],
                Y = modelOutput[GetOffset(x, y, channel + 1)],
                Width = modelOutput[GetOffset(x, y, channel + 2)],
                Height = modelOutput[GetOffset(x, y, channel + 3)]
            };
        }

        private static float GetConfidence(float[] modelOutput, int x, int y, int channel)
        {
            return Sigmoid(modelOutput[GetOffset(x, y, channel + 4)]);
        }

        private static CellDimensions MapBoundingBoxToCell(int x, int y, int box, BoxDimensions boxDimensions)
        {
            return new CellDimensions
            {
                X = ((float)x + Sigmoid(boxDimensions.X)) * CELL_WIDTH,
                Y = ((float)y + Sigmoid(boxDimensions.Y)) * CELL_HEIGHT,
                Width = (float)Math.Exp(boxDimensions.Width) * CELL_WIDTH * Anchors[box * 2],
                Height = (float)Math.Exp(boxDimensions.Height) * CELL_HEIGHT * Anchors[box * 2 + 1],
            };
        }

        public static float[] ExtractLabels(float[] modelOutputs, int x, int y, int channel)
        {
            var predictedLabels = new float[CLASS_COUNT];
            int predictedLabelOffset = channel + BOX_INFO_FEATURE_COUNT;
            for (int predictedLabel = 0; predictedLabel < CLASS_COUNT; predictedLabel++)
            {
                predictedLabels[predictedLabel] = modelOutputs[GetOffset(x, y, predictedLabel + predictedLabelOffset)];
            }
            return Softmax(predictedLabels);
        }

        private static ValueTuple<int, float> GetTopResult(float[] predictedLabels)
        {
            return predictedLabels
                .Select((predictedLabel, index) => (Index: index, Value: predictedLabel))
                .OrderByDescending(result => result.Value)
                .First();
        }

        private static float CalculateFilteringRatio(RectangleF boundingBoxA, RectangleF boundingBoxB)
        {
            var areaA = boundingBoxA.Width * boundingBoxA.Height;

            if (areaA <= 0)
                return 0;

            var areaB = boundingBoxB.Width * boundingBoxB.Height;

            if (areaB <= 0)
                return 0;

            var minX = Math.Max(boundingBoxA.Left, boundingBoxB.Left);
            var minY = Math.Max(boundingBoxA.Top, boundingBoxB.Top);
            var maxX = Math.Min(boundingBoxA.Right, boundingBoxB.Right);
            var maxY = Math.Min(boundingBoxA.Bottom, boundingBoxB.Bottom);

            var intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0);

            return intersectionArea / (areaA + areaB - intersectionArea);
        }

        #endregion PRE-PARSING

        public static YoloBoundingBox[] ParseOutputs(float[] yoloOutputs, float threshold = 0.3F)
        {
            var boxes = new List<YoloBoundingBox>();
            for (int row = 0; row < ROW_COUNT; row++)
            {
                for (int column = 0; column < COL_COUNT; column++)
                {
                    for (int box = 0; box < BOXES_PER_CELL; box++)
                    {
                        var channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT));
                        var boxDimensions = ExtractBoundingBoxDimensions(yoloOutputs, row, column, channel);
                        var confidence = GetConfidence(yoloOutputs, row, column, channel);
                        var mappedBox = MapBoundingBoxToCell(row, column, box, boxDimensions);                        
                        if (confidence < threshold) continue;
                        
                        var predictedLabels = ExtractLabels(yoloOutputs, row, column, channel);
                        var (topResultIndex, topResultScore) = GetTopResult(predictedLabels);
                        var topScore = topResultScore * confidence;
                        if (topScore < threshold)  continue;

                        boxes.Add(new YoloBoundingBox()
                        {
                            Dimensions = new BoxDimensions
                            {
                                X = (mappedBox.X - mappedBox.Width / 2),
                                Y = (mappedBox.Y - mappedBox.Height / 2),
                                Width = mappedBox.Width,
                                Height = mappedBox.Height,
                            },
                            Confidence = topScore,
                            Label = Labels[topResultIndex],
                            Color = LabelColors[topResultIndex]
                        });
                    }
                }
            }
            return boxes.ToArray();
        }

        public static YoloBoundingBox[] FilterOverlappingBoxes(YoloBoundingBox[] boxes, int limit, float threshold)
        {
            var activeCount = boxes.Length;
            var isActiveBoxes = new bool[boxes.Length];

            for (int i = 0; i < isActiveBoxes.Length; i++)
                isActiveBoxes[i] = true;

            var sortedBoxes = (
                from box in boxes
                orderby box.Confidence
                select box).ToArray();

            var filteredBoxes = new List<YoloBoundingBox>();
            for (int i = 0; i < sortedBoxes.Length; i++)
            {
                if (isActiveBoxes[i])
                {
                    var thisBox = sortedBoxes[i];
                    filteredBoxes.Add(thisBox);
                    if (filteredBoxes.Count >= limit) break;
                    else
                    {
                        for (var j = i + 1; j < sortedBoxes.Length; j++)
                        {
                            if (isActiveBoxes[j])
                            {
                                var thatBox = sortedBoxes[j];
                                filteredBoxes.Add(thatBox);

                                var thisRect = (RectangleF)Convert.ChangeType(thisBox.Rectangle, typeof(RectangleF))!;
                                var thatRect = (RectangleF)Convert.ChangeType(thisBox.Rectangle, typeof(RectangleF))!;

                                if (CalculateFilteringRatio(thisRect, thatRect) > threshold)
                                {
                                    isActiveBoxes[j] = false;
                                    activeCount--;

                                    if (activeCount <= 0) break;
                                }
                            }
                        }
                        if (activeCount <= 0) break;
                    }
                }
            }
            return filteredBoxes.ToArray(); 
        }
    }
}

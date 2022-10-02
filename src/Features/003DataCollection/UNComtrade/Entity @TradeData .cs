using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.UNComtrade
{
    public class TradeData
    {
        public Validation? validation { get; set; }
        public Dataset[]? dataset { get; set; }

        public class Validation
        {
            public Status? status { get; set; }
            public object? message { get; set; }
            public Count? count { get; set; }
            public Datasettimer? datasetTimer { get; set; }
        }

        public class Status
        {
            public string? name { get; set; }
            public int value { get; set; }
            public int category { get; set; }
            public string? description { get; set; }
            public string? helpUrl { get; set; }
        }

        public class Count
        {
            public int value { get; set; }
            public DateTime started { get; set; }
            public DateTime finished { get; set; }
            public float durationSeconds { get; set; }
        }

        public class Datasettimer
        {
            public DateTime started { get; set; }
            public DateTime finished { get; set; }
            public float durationSeconds { get; set; }
        }

        public class Dataset
        {
            public string? pfCode { get; set; }
            public int yr { get; set; }
            public int period { get; set; }
            public string? periodDesc { get; set; }
            public int aggrLevel { get; set; }
            public int IsLeaf { get; set; }
            public int rgCode { get; set; }
            public string? rgDesc { get; set; }
            public int rtCode { get; set; }
            public string? rtTitle { get; set; }
            public string? rt3ISO { get; set; }
            public int ptCode { get; set; }
            public string? ptTitle { get; set; }
            public string? pt3ISO { get; set; }
            public object? ptCode2 { get; set; }
            public string? ptTitle2 { get; set; }
            public string? pt3ISO2 { get; set; }
            public string? cstCode { get; set; }
            public string? cstDesc { get; set; }
            public string? motCode { get; set; }
            public string? motDesc { get; set; }
            public string? cmdCode { get; set; }
            public string? cmdDescE { get; set; }
            public int qtCode { get; set; }
            public string? qtDesc { get; set; }
            public object? qtAltCode { get; set; }
            public string? qtAltDesc { get; set; }
            public object? TradeQuantity { get; set; }
            public object? AltQuantity { get; set; }
            public object? NetWeight { get; set; }
            public object? GrossWeight { get; set; }
            public long TradeValue { get; set; }
            public object? CIFValue { get; set; }
            public object? FOBValue { get; set; }
            public int estCode { get; set; }
        }
    }
}

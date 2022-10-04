using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.Amazon
{
    internal class Review
    {
        internal string? Asin { set; get; }
        internal string? ProductName { set; get; }
        internal string? SellerName { set; get; }
        internal string? OverallScore { set; get; }
        internal float? RatingScore { set; get; }
        internal int? NumberOfRating { set; get; }
        internal string? FiveStar { set; get; }
        internal string? FourStar { set; get; }
        internal string? ThreeStar { set; get; }
        internal string? TwoStar { set; get; }
        internal string? OneStar { set; get; }
        internal Comment[]? Comments { set; get; }
        internal string? Url { set; get; }

        internal class Comment
        {
            internal string? Id { set; get; }
            internal string? DateLocation { set; get; }
            internal string? ReviewerName { set; get; }
            internal string? ReviewRating { set; get; }
            internal string? ReviewTitle { set; get; }
            internal string? ReviewContent { set; get; }
            internal string? VerifiedPurchase { set; get; }
        }
    }
}

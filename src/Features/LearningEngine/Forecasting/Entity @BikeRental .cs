using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.Data;

namespace DxMLEngine.Features.Forecasting
{
    public class BikeRental
    {
        public DateTime RentalDate { set; get; }
        public float Year { set; get; }
        public float TotalRentals { set; get; }
    }
}

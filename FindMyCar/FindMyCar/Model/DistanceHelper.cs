using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMyCar
{
    public class DistanceHelper
    {
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static double CalculateDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double circumference = 40000.0; // Earth's circumference at the equator in km
            double distance = 0.0;

            //Calculate radians
            double latitude1Rad = DegreesToRadians(latitude1);
            double longitude1Rad = DegreesToRadians(longitude1);
            double latititude2Rad = DegreesToRadians(latitude2);
            double longitude2Rad = DegreesToRadians(longitude2);

            double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

            if (logitudeDiff > Math.PI)
            {
                logitudeDiff = 2.0 * Math.PI - logitudeDiff;
            }

            double angleCalculation =
                Math.Acos(
                  Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
                  Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

            distance = circumference * angleCalculation / (2.0 * Math.PI);

            return distance;
        }
    }
}

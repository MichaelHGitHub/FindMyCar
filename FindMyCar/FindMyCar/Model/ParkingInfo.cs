using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace FindMyCar
{
    public enum ParkingStatus
    {
        Unknown,
        Parking,
        Parked,
        Found
    }

    public class ParkingInfo
    {
        public long TicksAtParking;
//        public Geoposition Pos = null;
        public double Latitude = 0;
        public double Longitude = 0;
        public ParkingStatus Status;

        public ParkingInfo()
        {
            Latitude = 0;
            Longitude = 0;
            Status = ParkingStatus.Unknown;
            TicksAtParking = DateTime.MinValue.Ticks;
        }
    }
}

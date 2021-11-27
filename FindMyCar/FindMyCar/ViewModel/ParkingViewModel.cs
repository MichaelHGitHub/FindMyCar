using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Windows;
using Windows.UI;
using Windows.UI.Popups;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using FindMyCar.Resources;

namespace FindMyCar
{
    class ParkingViewModel : INotifyPropertyChanged
    {
        public ParkingInfo parkingInfo = null;
        private Geoposition currentPos = null;
        private Geolocator locator = null;
        private double lastDistance = 0;
        private bool knownDistance = false;


        public ParkingViewModel()
        {
            locator = new Geolocator();
            LoadParkingData();

            if (parkingInfo == null)
            {
                parkingInfo = new ParkingInfo();
            }

        }


        private void LoadParkingData()
        {
            try
            {
                string parkingInfoStr = IsolatedStorageSettings.ApplicationSettings["LastParkingInfo"] as string;

                if (!string.IsNullOrEmpty(parkingInfoStr))
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    XmlSerializer serializer = new XmlSerializer(Type.GetType("FindMyCar.ParkingInfo"));
                    MemoryStream stream = new MemoryStream(encoding.GetBytes(parkingInfoStr));
                    //StreamReader reader = new StreamReader(stream);
                    stream.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    parkingInfo = serializer.Deserialize(stream) as ParkingInfo;
                    stream.Dispose();

                }
            }catch(Exception e)
            {
                string s = e.Message;
                
                // do nothing. 
            }

        }

        public void Refresh()
        {
            if (parkingInfo != null)
            {
                NotifyPropertyChanged("TimeSinceParking");
                NotifyPropertyChanged("Range");
                NotifyPropertyChanged("Status");
            }
        }

        public string TimeSinceParking
        {
            get
            {
                string durationStr = string.Empty;
                TimeSpan duration = TimeSpan.MinValue;

                if (parkingInfo.Status == ParkingStatus.Parked)
                {
                    duration = (DateTime.Now - new DateTime(parkingInfo.TicksAtParking));

                    if (duration.Days > 0)
                    {
                        durationStr += duration.Days + " " + AppResources.Day;
                    }

                    if (duration.Hours > 0)
                    {
                        durationStr += duration.Hours + " " + AppResources.Hour;
                    }

                    if (duration.Minutes > 0)
                    {
                        durationStr += duration.Minutes + " " + AppResources.Minute;
                    }

                    if (!string.IsNullOrEmpty(durationStr))
                    {
                        durationStr += " ";
                        durationStr += AppResources.Ago;
                    }

                    if (duration.Days <= 0 && duration.Hours <= 0 && duration.Minutes <= 0 && duration.Seconds >= 0)
                    {
                        durationStr = AppResources.LessThan1Min;
                    }
                }

                return durationStr;
            }
        }

        public string Range
        {
            get
            {
                string range = "";
                double distance = 0;

                if (parkingInfo.Status == ParkingStatus.Parked && currentPos != null)
                {
                    if (currentPos.Coordinate.Latitude == parkingInfo.Latitude && currentPos.Coordinate.Longitude == parkingInfo.Longitude)
                    {
                        distance = 0;
                    }
                    else
                    {
                        distance = DistanceHelper.CalculateDistance(currentPos.Coordinate.Latitude, currentPos.Coordinate.Longitude, parkingInfo.Latitude, parkingInfo.Longitude);
                    }

                    if (distance >= 0)
                    {
                        lastDistance = distance;
                        knownDistance = true;
                    }

                    if (lastDistance > 1)
                    {
                        range = string.Format("{0,-10:N1} {1}", lastDistance, AppResources.Kilometer);
                    }
                    else
                    {
                        range = string.Format("{0,-10:N1} {1}", (lastDistance * 1000), AppResources.Meter);
                    }
             
                }

                if (!knownDistance)
                {
                    return AppResources.Unknown;
                }
                else
                {
                    return range;
                }

            }
        }

        public string Status
        {
            get
            {
                string statusString = AppResources.Unknown;

                if (parkingInfo != null)
                {
                    switch (parkingInfo.Status)
                    {
                        case ParkingStatus.Parking:
                            statusString = AppResources.Parking;
                            break;

                        case ParkingStatus.Parked:
                            statusString = AppResources.Parked;
                            break;

                        case ParkingStatus.Found:
                            statusString = AppResources.Found;
                            break;

                        default:
                            break;
                    }
                }

                return statusString;
            }

        }

        public async void ParkCar()
        {
            parkingInfo.Status = ParkingStatus.Parking;
            NotifyPropertyChanged("Status");

            currentPos = await locator.GetGeopositionAsync();
            parkingInfo.Latitude = currentPos.Coordinate.Latitude;
            parkingInfo.Longitude = currentPos.Coordinate.Longitude;
            parkingInfo.Status = ParkingStatus.Parked;
            parkingInfo.TicksAtParking = DateTime.Now.Ticks;

            Refresh();

            SaveLocation();

        }

        private void SaveLocation()
        {
            XmlSerializer serializer = new XmlSerializer(parkingInfo.GetType());
            MemoryStream stream = new MemoryStream();

            serializer.Serialize(stream, parkingInfo);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(stream);

            string infoStr = reader.ReadToEnd();

            reader.Dispose();
            stream.Dispose();

            IsolatedStorageSettings.ApplicationSettings["LastParkingInfo"] = infoStr;

        }

        public async void RefreshCurrentPos()
        {
            currentPos = await locator.GetGeopositionAsync();

            Refresh();
        }

        public async void FindRoute()
        {
            //Lunch Here map
            //uri = "explore-maps://v1.0/?latlon=56.615495,12.1865081&zoom=5";

            if (parkingInfo.Status == ParkingStatus.Parked)
            {
                string uri = "bingmaps:?";
                //uri += "rtp=";
                //uri += "pos.";
                //uri += currentPos.Coordinate.Latitude.ToString();
                //uri += "_";
                //uri += currentPos.Coordinate.Longitude.ToString();
                //uri += "~";
                //uri += "pos.";
                //uri += parkingInfo.Latitude.ToString();
                //uri += "_";
                //uri += parkingInfo.Longitude.ToString();
                //uri += "&mode=W";

                uri += "where=";
                uri += parkingInfo.Latitude.ToString();
                uri += ",";
                uri += parkingInfo.Longitude.ToString();
                uri += "&lvl=18";

                await Launcher.LaunchUriAsync(new Uri(uri));

            }
            else
            {
                MessageBox.Show(AppResources.NoParkingData, AppResources.ApplicationTitle, MessageBoxButton.OK);

            }
         }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}

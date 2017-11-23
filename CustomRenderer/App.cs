using Xamarin.Forms;
using Plugin.Connectivity;
using Android.Locations;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Plugin.Geolocator;
using System;

namespace CustomRenderer
{
    public class App : Application
    {
        public static double ScreenHeight;
        public static double ScreenWidth;
        decimal lat;
        decimal lon;
        string slat;
        string slon;
        string deviceIdentifier;
        string ciudad;

        public App ()
		{
            if (CheckConnectivity())
            {
                MainPage = new NavigationPage(new MapPage(null));
            }
            else
            {
                MainPage = new NavigationPage(new NoConexion());
            }                      
        }

        protected override void OnResume ()
		{
            if (CheckConnectivity())
            {
                MainPage = new NavigationPage(new MapPage(null));
            }
            else
            {
                MainPage = new NavigationPage(new NoConexion());
            }            
        }        
        protected override async void OnStart()
        {
            if (IsLocationAvailable() && CheckConnectivity() && IsLocationEnabled())
            {
                await findMeAsync();
                IDevice device = DependencyService.Get<IDevice>();
                deviceIdentifier = device.GetIdentifier();
                string location = obtener_Ciudad();
                JObject jlocation = JObject.Parse(location);
                ciudad = jlocation["results"][0]["locations"][0]["adminArea5"].ToString();
                insertar_acceso();
            }
            else
            {
                new NavigationPage(new NoConexion());
            }
        }

        string insertar_acceso()
        {
            if (CheckConnectivity())
            {
                string respuesta;

                string url1 = "http://tesis2017.xyz/user_insert.php?uID=" + deviceIdentifier + "&ciudad=" + ciudad;

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = client.GetAsync(url1).Result)
                        {
                            using (HttpContent content = response.Content)
                            {
                                respuesta = content.ReadAsStringAsync().Result;
                                return respuesta;
                            }
                        }
                    }
                }
                catch
                {
                    return "error en la conexion";
                }
            }
            else
            {
                new NavigationPage(new NoConexion());
                return "0";
            }
            
           
            
        }
        string obtener_Ciudad()
        {
            if (CheckConnectivity())
            {
                string datosUbicacion;

                string url2 = "https://www.mapquestapi.com/geocoding/v1/reverse?key=OA8B0RAywUeRc5EZnDNBArVuzAqXbjNT&location=" + slat + "%2C" + slon + "&outFormat=json&thumbMaps=false";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = client.GetAsync(url2).Result)
                        {
                            using (HttpContent content = response.Content)
                            {
                                datosUbicacion = content.ReadAsStringAsync().Result;
                                return datosUbicacion;
                            }
                        }
                    }
                }
                catch
                {
                    return "error en la conexion";
                }
            }
            else
            {
                new NavigationPage(new NoConexion());
                return "error en la conexion";                
            }            
        }

        public bool IsLocationAvailable()
        {
            return CrossGeolocator.Current.IsGeolocationAvailable;
        }
        public bool IsLocationEnabled()
        {
            return CrossGeolocator.Current.IsGeolocationEnabled;
        }

        bool CheckConnectivity()
        {
            var isConnected = CrossConnectivity.Current.IsConnected;

            if (!isConnected)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async System.Threading.Tasks.Task findMeAsync()
        {
            try
            {
                TimeSpan timeout = new TimeSpan(0, 0, 0, 10);

                var locator = CrossGeolocator.Current;

                locator.DesiredAccuracy = 50;
                if (IsLocationAvailable() && IsLocationEnabled())
                {
                    var position = await locator.GetPositionAsync(timeout, includeHeading: false);
                    lat = (decimal)position.Latitude;
                    lon = (decimal)position.Longitude;
                    slat = lat.ToString().Replace(',', '.');
                    slon = lon.ToString().Replace(',', '.');                    
                }               
            }
            catch (OperationCanceledException ex)
            {
                
            }
        }
    }
}


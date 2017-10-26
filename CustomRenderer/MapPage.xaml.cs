using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using System.Diagnostics;
using System.Net;

namespace CustomRenderer
{
    public partial class MapPage : ContentPage
    {
        public MapPage(MapSpan ms)
        {
            string datosOcupacion;
            string estadoletra;
            bool estadoBool;
            int counter = 0;
            Debug.WriteLine("***********************************************1***************************************");
            var customMap = new CustomMap
            {                
                MapType = MapType.Street,
                WidthRequest = App.ScreenWidth,
                HeightRequest = App.ScreenHeight,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                IsShowingUser = true                
            };

            Debug.WriteLine("***********************************************2***************************************");
            customMap.CustomPins = new List<CustomPin>();

            Debug.WriteLine("***********************************************3***************************************");
            var btnActualizar = new Button()
            {
                WidthRequest = App.ScreenWidth,
                HeightRequest = App.ScreenHeight / 10,
                Text = "ACTUALIZAR",

                BackgroundColor = Xamarin.Forms.Color.FromHex("#FFEBAF"),
                Image = "update.png",
                BorderRadius = 5,
                BorderColor = Xamarin.Forms.Color.FromHex("#E8D49C"),
                BorderWidth = 2,
                HorizontalOptions = LayoutOptions.Center,
            };

            Debug.WriteLine("***********************************************4***************************************");
            btnActualizar.Clicked += (sender, e) =>
            {
                int numExistingPages = Navigation.NavigationStack.Count;

                if (numExistingPages == 2)
                {
                    Navigation.RemovePage(this);
                }
                MapSpan mapsector = customMap.VisibleRegion;
                Navigation.PushAsync(new MapPage(mapsector));
            };

            Debug.WriteLine("***********************************************5***************************************");
            InitializeComponent();

            Debug.WriteLine("***********************************************6***************************************");
            //requestGPSAsync();
            datosOcupacion = obtenerDatosOcupacion();
            var datosResultadoOcupacion = JArray.Parse(datosOcupacion);
            findMeAsync();

            foreach (var v in datosResultadoOcupacion)
            {
                if ((int)datosResultadoOcupacion[counter]["estado"] == 1)
                {
                    estadoBool = false;
                    estadoletra = "Ocupado";
                }
                else
                {
                    estadoBool = true;
                    estadoletra = "Disponible";
                }
                Uri parkingurl = new Uri(string.Format("geo:0,0?q="
                    +datosResultadoOcupacion[counter]["coordenadas_lat"]
                    +","+ datosResultadoOcupacion[counter]["coordenadas_lon"]
                    +"("
                    +"Estacionamiento "+estadoletra+")"));

                var pin = new CustomPin()
                {             
                Pin = new Pin()
                    {
                        Type = PinType.Place,
                        Position = new Position((double)datosResultadoOcupacion[counter]["coordenadas_lat"], (double)datosResultadoOcupacion[counter]["coordenadas_lon"]),
                        Label = "Estacionamiento "+estadoletra,
                        Address = "Calle " + datosResultadoOcupacion[counter]["calle"].ToString() + " con " + datosResultadoOcupacion[counter]["interseccion1"].ToString()
                    },
                    Id = datosResultadoOcupacion[counter]["id_estacionamiento"].ToString(),
                    Url = parkingurl.ToString(),
                    Estado = estadoBool
                };                
                customMap.CustomPins.Add(pin);
                counter++;
            }

            Debug.WriteLine("***********************************************7***************************************");
            foreach (var pin in customMap.CustomPins)
            {
                customMap.Pins.Add(pin.Pin);
            }

            Debug.WriteLine("***********************************************8***************************************");
            // dont delete below code ,they will save you if timer doesnt work .

            if (ms == null)
            {
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(customMap.CustomPins[0].Pin.Position, Distance.FromMiles(0.32)));
            }
            else
            {
                customMap.MoveToRegion(ms);
            }

            Debug.WriteLine("***********************************************9***************************************");
            Content =
            new AbsoluteLayout
            {
            Children =
                {
                customMap,
                    new StackLayout
                    {
                        VerticalOptions = LayoutOptions.End,
                        Children =
                        {
                        btnActualizar
                        }
                    }
                }
            };
        }

        string obtenerDatosOcupacion()
        {

            Debug.WriteLine("***********************************************10***************************************");
            string datosOcupacion;
            string url = "http://tesis2017.000webhostapp.com/webservice.php";

            Debug.WriteLine("***********************************************11***************************************");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = response.Content)
                        {
                            datosOcupacion = content.ReadAsStringAsync().Result;
                            return datosOcupacion;
                        }
                    }
                }
            }
            catch
            {
                return "error en la conexion";
            }
        }

        private async System.Threading.Tasks.Task findMeAsync()
        {

            Debug.WriteLine("***********************************************GEOLOCATOR1***************************************");
            TimeSpan timeout = new TimeSpan(0, 0, 0, 10);

            Debug.WriteLine("**********************************************GEOLOCATOR2***************************************");
            var locator = CrossGeolocator.Current;

            Debug.WriteLine("***********************************************GEOLOCATOR3***************************************");
            locator.DesiredAccuracy = 50;

            Debug.WriteLine("***********************************************GEOLOCATOR4***************************************");
            var position = await locator.GetPositionAsync(timeout, includeHeading: false); ;

            Debug.WriteLine("***********************************************GEOLOCATOR5***************************************");
        }        
    }
}


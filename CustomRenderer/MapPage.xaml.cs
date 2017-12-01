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
using Plugin.Connectivity;

namespace CustomRenderer
{
    public partial class MapPage : ContentPage
    {        
        public MapPage(MapSpan ms)
        {
            findMeAsync();
            string datosOcupacion;
            string estadoletra;
            bool estadoBool;
            int counter = 0;
            var customMap = new CustomMap();
            customMap.MapType = MapType.Street;
            customMap.WidthRequest = App.ScreenWidth;
            customMap.HeightRequest = App.ScreenHeight;
            customMap.VerticalOptions = LayoutOptions.FillAndExpand;
            customMap.HorizontalOptions = LayoutOptions.FillAndExpand;
            if (IsLocationAvailable() && IsLocationEnabled())
            {
                customMap.IsShowingUser = true;
            }
            
            
           
            customMap.CustomPins = new List<CustomPin>();
            
            var btnActualizar = new Button()
            {
                WidthRequest = App.ScreenWidth/5,
                HeightRequest = App.ScreenHeight / 10,
                BackgroundColor = Xamarin.Forms.Color.FromHex("#FFEBAF"),
                Image = "update.png",
                BorderRadius = 15,
                BorderColor = Xamarin.Forms.Color.FromHex("#E8D49C"),
                BorderWidth = 2,
                HorizontalOptions = LayoutOptions.Start,
            };

            var btnLista = new Button()
            {
                WidthRequest = App.ScreenWidth / 5,
                HeightRequest = App.ScreenHeight / 10,
                BorderRadius = 15,
                Image = "list48.png",
                BackgroundColor = Xamarin.Forms.Color.FromHex("#FFEBAF"),
                BorderColor = Xamarin.Forms.Color.FromHex("#E8D49C"),
                BorderWidth = 2,
                HorizontalOptions = LayoutOptions.Center,
            };

            btnLista.Clicked += async (sender, e) =>
            {
                btnLista.IsEnabled = false;
                if (CheckConnectivity())
                {
                    await Navigation.PushModalAsync(new PLista());
                }
                else
                {
                    await Navigation.PushModalAsync(new NoConexion());
                }
                
            };

            
            btnActualizar.Clicked += async (sender, e) =>
            {
                if (CheckConnectivity())
                {
                    btnActualizar.IsEnabled = false;
                    int numExistingPages = Navigation.NavigationStack.Count;
                    Debug.WriteLine("**********CANTIDAD DE PAGINAS EN EL NAVIGATIONSTACK********: " + numExistingPages.ToString());
                    if (numExistingPages >= 2)
                    {
                        Navigation.RemovePage(this);
                    }
                    MapSpan mapsector = customMap.VisibleRegion;
                    await Navigation.PushModalAsync(new MapPage(mapsector));
                }
                else
                {
                    await Navigation.PushAsync(new NoConexion());
                }
            };
            datosOcupacion = obtenerDatosOcupacion();
            if (datosOcupacion == "error en la conexion")
            {
                DisplayAlert("Atención", "No hay conexión", "OK");
            }
            var datosResultadoOcupacion = new JArray();
            try
            {
                datosResultadoOcupacion = JArray.Parse(datosOcupacion);
            }
            catch
            {
                DisplayAlert("Atención", "Servidor inalcanzable, intente mas tarde.", "OK");
            }
            
           
            InitializeComponent();
            foreach (var v in datosResultadoOcupacion)
            {
                string calle = "";
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
                if (datosResultadoOcupacion[counter]["calle"].ToString().Length != 0)
                {
                    calle = "Calle " + datosResultadoOcupacion[counter]["calle"].ToString() + " con " + datosResultadoOcupacion[counter]["interseccion1"].ToString();
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
                        Address = calle
                    },
                    Id = datosResultadoOcupacion[counter]["id_estacionamiento"].ToString(),
                    Url = parkingurl.ToString(),
                    Estado = estadoBool
                };                
                customMap.CustomPins.Add(pin);
                counter++;
            }

            foreach (var pin in customMap.CustomPins)
            {
                customMap.Pins.Add(pin.Pin);
            }

            if (ms == null)
            {
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-38.738580, -72.598367), Distance.FromMiles(3)));
            }
            else
            {
                customMap.MoveToRegion(ms);
            }
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
                        btnActualizar,
                        btnLista
                        }
                    }
                }
            };
            
        }

        public bool IsLocationEnabled()
        {
            return CrossGeolocator.Current.IsGeolocationEnabled;
        }

        public bool IsLocationAvailable()
        {
            return CrossGeolocator.Current.IsGeolocationAvailable;
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

        string obtenerDatosOcupacion()
        {
            if (CheckConnectivity())
            {
                string datosOcupacion;
                string url = "http://tesis2017.xyz/webservice.php";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
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
            else
            {
                new NavigationPage(new NoConexion());
                return "error en la conexion";
            }
                
        }

        private async System.Threading.Tasks.Task findMeAsync()
        {
            try
            {
                TimeSpan timeout = new TimeSpan(9, 9, 9, 10);
                
                var locator = CrossGeolocator.Current;
                
                locator.DesiredAccuracy = 50;
                
                var position = await locator.GetPositionAsync(timeout, includeHeading: false);
            }
            catch(OperationCanceledException ex)
            {
                await DisplayAlert("Atención", "Por favor active el GPS de su celular. "+ex, "OK");
            }
            
        }        
    }
}


using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Newtonsoft.Json.Linq;
using System.Net.Http;

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

            var customMap = new CustomMap
            {
                MapType = MapType.Street,
                WidthRequest = App.ScreenWidth,
                HeightRequest = App.ScreenHeight,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                IsShowingUser = true,
            };

            customMap.CustomPins = new List<CustomPin>();

            var btnActualizar = new Button()
            {
                WidthRequest = App.ScreenWidth,
                HeightRequest = App.ScreenHeight / 10,
                Text = "ACTUALIZAR",

                BackgroundColor = Xamarin.Forms.Color.Red,
                Image = "update.png",
                BorderRadius = 15,
                BorderColor = Xamarin.Forms.Color.Black,
                BorderWidth = 3,
                HorizontalOptions = LayoutOptions.Center,
            };

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

            InitializeComponent();
            
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
                    estadoletra = "Desocupado";
                }
                var pin = new CustomPin()
                {
                    Pin = new Pin()
                    {
                        Type = PinType.Place,
                        Position = new Position((double)datosResultadoOcupacion[counter]["coordenadas_lat"], (double)datosResultadoOcupacion[counter]["coordenadas_lon"]),
                        Label = estadoletra,
                        Address = "Calle " + datosResultadoOcupacion[counter]["calle"].ToString() + " con " + datosResultadoOcupacion[counter]["interseccion1"].ToString()
                    },
                    Id = datosResultadoOcupacion[counter]["id_estacionamiento"].ToString(),
                    Url = "",
                    Estado = estadoBool
                };
                customMap.CustomPins.Add(pin);
                counter++;
            }
            foreach (var pin in customMap.CustomPins)
            {
                customMap.Pins.Add(pin.Pin);
            }
            // dont delete below code ,they will save you if timer doesnt work .

            if (ms == null)
            {
                customMap.MoveToRegion(MapSpan.FromCenterAndRadius(customMap.CustomPins[0].Pin.Position, Distance.FromMiles(0.6)));
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
                        btnActualizar
                        }
                    }
                }
            };
        }

        string obtenerDatosOcupacion()
        {
            string datosOcupacion;
            string url = "http://tesis2017.000webhostapp.com/webservice.php";
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
            
            var locator = CrossGeolocator.Current;
            Plugin.Geolocator.Abstractions.Position position = new Plugin.Geolocator.Abstractions.Position();

            position = await locator.GetPositionAsync();
            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMiles(1)));
            
        }

        private async void requestGPSAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need location", "Gunna need that location", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }
                if (status == PermissionStatus.Granted)
                {
                    var results = await CrossGeolocator.Current.GetPositionAsync();
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch
            {

            }
        }
    }
}


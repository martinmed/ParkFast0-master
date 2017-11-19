using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Xamarin.Forms.Maps;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System.Diagnostics;
using Plugin.Connectivity;

namespace CustomRenderer
{
    public class PLista : ContentPage
    {
        private CLista objetoEstacionamiento;
        string estadoletra;
        public ObservableCollection<CLista> claselista { get; set; }
        public PLista()
        {
            bool estadoBool;
            int counter = 0;
            string datosOcupacion;
            claselista = new ObservableCollection<CLista>();
            ListView lstView = new ListView();

            lstView.ItemTapped += async (sender, e) =>
            {
                var datosEstacionamientoSeleccionado = JsonConvert.SerializeObject(e.Item);
                objetoEstacionamiento = JsonConvert.DeserializeObject<CLista>(datosEstacionamientoSeleccionado);
                float estacionamiento_Lat = objetoEstacionamiento.coord_lat;
                float estacionamiento_Lon = objetoEstacionamiento.coord_lon;
                Position estacionamiento_Position = new Position(estacionamiento_Lat, estacionamiento_Lon);
                MapSpan mapspan = new MapSpan(estacionamiento_Position, 0.00272, 0.00272);

                await Navigation.PushModalAsync(new MapPage(mapspan));
            };
            Label lbl_Titulo = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                Text = "ESTACIONAMIENTOS",
                FontSize = 32,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.White,
                TextColor = Color.SlateGray,
                WidthRequest = App.ScreenWidth,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };

            lstView.Header = lbl_Titulo;


            lstView.RowHeight = 60;
            lstView.WidthRequest = App.ScreenWidth;
            lstView.BackgroundColor = Color.WhiteSmoke;
            lstView.SeparatorColor = Color.Black;
            lstView.SeparatorVisibility = SeparatorVisibility.Default;
            lstView.ItemTemplate = new DataTemplate(typeof(CustomCell));
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
                CLista clista = new CLista();
                if (datosResultadoOcupacion[counter]["calle"].ToString() == "")
                {
                    clista.Calle = datosResultadoOcupacion[counter]["descripcion"].ToString();
                }
                else
                {
                    clista.Calle = datosResultadoOcupacion[counter]["calle"].ToString() + " con " + datosResultadoOcupacion[counter]["interseccion1"].ToString();
                }

                clista.Estado = estadoletra;

                float estacionamiento_lat = (float)datosResultadoOcupacion[counter]["coordenadas_lat"];
                float estacionamiento_lon = (float)datosResultadoOcupacion[counter]["coordenadas_lon"];

                clista.coord_lat = estacionamiento_lat;
                clista.coord_lon = estacionamiento_lon;

                TimeSpan timeout = new TimeSpan(0, 0, 0, 10);

                var locator = CrossGeolocator.Current;
                string a = locator.GetPositionAsync().ToString();
                var position_a = new Position(estacionamiento_lon, estacionamiento_lon);

                var position_b = locator.GetPositionAsync(timeout, includeHeading: false);

                //////////////////////////while (position_b==null)
                //////////////////////////{
                //////////////////////////    Debug.WriteLine("ESPERANDO");
                //////////////////////////}

                if (estadoletra == "Ocupado")
                {
                    clista.Image = "pin2.png";

                }
                else
                {
                    clista.Image = "pin.png";
                }
                string b = clista.Estado.ToString();

                claselista.Add(clista);

                counter++;
            }
            Button backButton = new Button()
            {
                Image = "back.png",
                Text="Volver"
            };
            backButton.Clicked += (sender, e) =>
            {
                Navigation.PushModalAsync(new MapPage(null));
            };
            lstView.ItemsSource = claselista;
            Content =
                new StackLayout
                {
                    Children =
                        {
                        lstView,
                            new StackLayout
                            {
                            Children =
                                {
                                backButton                        
                                }
                            }
                        }
                };
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

        //public static double DistanceBetween(Position a, Position b)
        //{
        //    double d = Math.Acos(
        //       (Math.Sin(a.Latitude) * Math.Sin(b.Latitude)) +
        //       (Math.Cos(a.Latitude) * Math.Cos(b.Latitude))
        //       * Math.Cos(b.Longitude - a.Longitude));

        //    return 6378137 * d;
        //}

        string obtenerDatosOcupacion()
        {
            if (CheckConnectivity())
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
            else
            {
                new NavigationPage(new NoConexion());
                return "error en la conexion";
            }           
        }

        public class CustomCell : ViewCell
        {
            public CustomCell()
            {                
                //instantiate each of our views
                var image = new Image();
                var calleLabel = new Label();
                calleLabel.Margin = new Thickness(10,0,0,0);
                calleLabel.TextColor = Color.Black;
                var estadoLabel = new Label();
                estadoLabel.TextColor = Color.Black;
                estadoLabel.Margin = new Thickness(10,0,0,0);
                var verticaLayout = new StackLayout();
                var coord_latLabel = new Label();
                var coord_lonLabel = new Label();
                coord_latLabel.IsVisible = false;
                coord_lonLabel.IsVisible = false;

                var horizontalLayout = new StackLayout()
                {
                    BackgroundColor = Color.WhiteSmoke
                };
                horizontalLayout.WidthRequest = App.ScreenWidth;

                //set bindings

                calleLabel.SetBinding(Label.TextProperty, new Binding("Calle"));
                estadoLabel.SetBinding(Label.TextProperty, new Binding("Estado"));
                image.SetBinding(Image.SourceProperty, new Binding("Image"));
                coord_latLabel.SetBinding(Label.TextProperty, new Binding("coord_lat"));
                coord_lonLabel.SetBinding(Label.TextProperty, new Binding("coord_lon"));

                //Set properties for desired design
                horizontalLayout.Orientation = StackOrientation.Horizontal;
                horizontalLayout.HorizontalOptions = LayoutOptions.EndAndExpand;
                image.HorizontalOptions = LayoutOptions.EndAndExpand;
                calleLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
                calleLabel.FontSize = 20;

                //add views to the view hierarchy
                verticaLayout.Children.Add(calleLabel);
                verticaLayout.Children.Add(estadoLabel);
                horizontalLayout.Children.Add(verticaLayout);
                horizontalLayout.Children.Add(image);

                // add to parent view
                View = horizontalLayout;
            }
        }
    }
}



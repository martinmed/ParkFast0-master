using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Xamarin.Forms;

namespace CustomRenderer
{
    public class HomePage : ContentPage
    {
        Button btn_respuesta = new Button();
        Label results = new Label();
        JArray datosResultadoOcupacion = new JArray();
        Label info = new Label();        
        public HomePage()
        {
            info.Text = "Si pertenece a alguna de las siguientes entidades, por favor seleccionela";
            btn_respuesta.Text = "Respuesta";
            btn_respuesta.Clicked += (sender, e) =>
             {
                 OnClick(sender,e);
             };
            

            string entidades = obtener_entidades();
            if (entidades == "error en la conexion")
            {
                DisplayAlert("Atención", "No hay conexión", "OK");
            }

            try
            {
                datosResultadoOcupacion = JArray.Parse(entidades);
               
            }
            catch
            {
                DisplayAlert("Atención", "Servidor inalcanzable, intente mas tarde.", "OK");
            }
            Content =
            new StackLayout
            {
                Children =
                {
                info,    
                btn_respuesta,
                results
                }
            };
        }


        SelectMultipleBasePage<CheckItem> multiPage;
        async void OnClick(object sender, EventArgs ea)
        {
            int counter = 0;
            var items = new List<CheckItem>();
            foreach (var v in datosResultadoOcupacion)
            {
                items.Add(new CheckItem { Name = datosResultadoOcupacion[counter]["nombre_entidad"].ToString() });
                counter++;
            }
            if (multiPage == null)
                multiPage = new SelectMultipleBasePage<CheckItem>(items) { Title = "Check all that apply" };

            await Navigation.PushAsync(multiPage);
        }

        string obtener_entidades()
        {
            string datosOcupacion;
            string url = "http://tesis2017.000webhostapp.com/get_entidades.php";

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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (multiPage != null)
            {
                results.Text = "";
                var answers = multiPage.GetSelection();
                foreach (var a in answers)
                {
                    results.Text += a.Name + ", ";
                }
            }
            else
            {
                results.Text = "(none)";
            }
        }

    }
}
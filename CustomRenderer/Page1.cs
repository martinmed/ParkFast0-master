using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace CustomRenderer
{
    public class Page1 : ContentPage
    {
        public Page1()
        {
            Image noconexion = new Image();
            noconexion.Source = "no_conexion.png";

            Label lblNoConexion = new Label();
            lblNoConexion.Text= "Por favor revise su conexión a Internet";

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions=LayoutOptions.Start,
                Children = {
                    noconexion,
                    lblNoConexion
                }
            };
        }
    }
}
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace CustomRenderer
{
    
    public class NoConexion : ContentPage
    {
        
        public NoConexion()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetBackButtonTitle(this, "titulo de la pagina");
            Image imgNoConexion = new Image();
            imgNoConexion.Source = "no_conexion.png";
            imgNoConexion.Opacity = 0.2;

            Label lblNoConexion = new Label();
            lblNoConexion.Text= "Por favor revise su conexión a Internet";
            lblNoConexion.FontAttributes = FontAttributes.Bold;
            

            Button btnReintentar = new Button();
            btnReintentar.BackgroundColor = Xamarin.Forms.Color.FromHex("#2bcd05");
            btnReintentar.BorderRadius = 10;
            btnReintentar.Margin = Padding = new Thickness(0, 100, 0, 0);
            btnReintentar.Text = "Reintentar";

            

            btnReintentar.Clicked += (sender, e) =>
            {
               
                if (CheckConnectivity())
                {
                    if (Navigation.NavigationStack.Count == 2)
                    {
                        Navigation.RemovePage(this);
                    }
                    Navigation.PushAsync(new MapPage(null));
                }
                else
                {

                }
            };

            

            bool CheckConnectivity()
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions=LayoutOptions.Center,
                Children = {
                    imgNoConexion,
                    lblNoConexion,
                    btnReintentar
                }
            };
        }
    }
}
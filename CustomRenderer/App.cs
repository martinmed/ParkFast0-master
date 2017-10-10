using Xamarin.Forms;
using Plugin.Connectivity;

namespace CustomRenderer
{
	public class App : Application
	{
		public static double ScreenHeight;
		public static double ScreenWidth;

		public App ()
		{
            //#if __ANDROID__
            //            var cantidadCuenta = AccountStore.Create(Forms.Context).FindAccountsForService(Application.Current.ToString()).Count();

            //            if (cantidadCuenta == 0)
            //            {
            //                MainPage = new AppUnap.PLogin();
            //            }
            //            else
            //            {
            //                MainPage = new AppUnap.PPrincipal();
            //            }
            //#endif
            bool conectividad = CheckConnectivity();
            if (conectividad)
            {
                MainPage = new NavigationPage(new MapPage(null));
            }
            else
            {
                MainPage = new NavigationPage(new NoConexion());
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

            
        }
        

		protected override void OnStart ()
		{
            
        }

		protected override void OnSleep ()
		{
            bool conectividad = CheckConnectivity();
            if (conectividad)
            {
                MainPage = new NavigationPage(new MapPage(null));
            }
            else
            {
                MainPage = new NavigationPage(new NoConexion());
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
        }

		protected override void OnResume ()
		{
            bool conectividad = CheckConnectivity();
            if (conectividad)
            {
                MainPage = new NavigationPage(new MapPage(null));
            }
            else
            {
                MainPage = new NavigationPage(new NoConexion());
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
        }
	}
}


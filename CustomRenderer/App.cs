using Xamarin.Forms;

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
            MainPage = new MapPage ();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}


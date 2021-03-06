﻿using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Plugin.Permissions;
using System.Diagnostics;

namespace CustomRenderer.Droid
{
	[Activity (Label = "ParkFast", Icon = "@drawable/pin", ConfigurationChanges =
        ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            {
                try
                {
                    if ((int)grantResults[0] == 0)
                    {
                        LoadApplication(new App());
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    System.Diagnostics.Debug.WriteLine("*****************"+e.ToString());
                }
                
            }           
        }
        public override void OnBackPressed()
        {           
            FinishAndRemoveTask();
        }

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
			Xamarin.FormsMaps.Init (this, bundle);

			var width = Resources.DisplayMetrics.WidthPixels;
			var height = Resources.DisplayMetrics.HeightPixels;
			var density = Resources.DisplayMetrics.Density;

			App.ScreenWidth = (width) / density;
			App.ScreenHeight = (height) / density;

			LoadApplication (new App ());
		}
	}
}

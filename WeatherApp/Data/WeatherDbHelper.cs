using System;
using Android.Content;
using System.IO;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

namespace WeatherApp
{
	public class WeatherDbHelper: SQLiteConnection
	{
		const int DATABASE_VERSION = 2;

		const String DATABASE_NAME = "weather.db";
		static string personalFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		static readonly string DATABASE_PATH = Path.Combine (personalFolder, DATABASE_NAME);



		public WeatherDbHelper () : base (new SQLitePlatformAndroid (), DATABASE_PATH)
		{
			
		}

		public void Create ()
		{
			CreateTable<LocationEntry> ();
			CreateTable<WeatherEntry> ();
		}

		public void Update ()
		{
			DropTable<LocationEntry> ();
			DropTable<WeatherEntry> ();

			Create ();
		}

		public void DropAll ()
		{
			DropTable<WeatherEntry> ();
			DropTable<LocationEntry> ();
		}
	}
}


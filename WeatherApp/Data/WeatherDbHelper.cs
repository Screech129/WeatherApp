using System;
using Android.Content;
using System.IO;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

namespace WeatherApp
{
	public class WeatherDbHelper: SQLiteConnection
	{
		const int DatabaseVersion = 2;

		const String DatabaseName = "weather.db";
		static string _personalFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		static readonly string DATABASE_PATH = Path.Combine (_personalFolder, DatabaseName);



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


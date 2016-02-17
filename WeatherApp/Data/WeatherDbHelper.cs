using System;
using Android.Content;
using System.IO;
using SQLite;

namespace WeatherApp
{
	public class WeatherDbHelper: SQLiteConnection
	{
		const int DATABASE_VERSION = 2;

		const String DATABASE_NAME = "weather.db";
		static string personalFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
		static readonly string DATABASE_PATH = Path.Combine (personalFolder, DATABASE_NAME);


		public WeatherDbHelper () : base (DATABASE_PATH)
		{
			
		}

		public void Create ()
		{
			CreateTable<WeatherEntry> ();
			CreateTable<LocationEntry> ();
		}

		public void Update ()
		{
			DropTable<WeatherEntry> ();
			DropTable<LocationEntry> ();

			Create ();
		}

		public void DropAll ()
		{
			DropTable<LocationEntry> ();
			DropTable<WeatherEntry> ();
		}
	}
}


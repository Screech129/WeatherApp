using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Text;
using Android.Util;
using Android.Content;
using Android.Preferences;
using Android.App;
using Org.Json;
using Android.Content.Res;
using System.Collections;
using Android.Database;

namespace WeatherApp
{
	public class FetchWeatherTask
	{
		

		Context _context;

		public FetchWeatherTask (Context context)
		{
			_context = context;
		}

		public async Task FetchWeatherTaskFromZip (string zipCode)
		{


		}


	}
}


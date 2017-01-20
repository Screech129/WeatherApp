using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WeatherApp.Helpers
{
    public static class Constants
    {
        public static Context Context;
        public static readonly string ContentAuthority = Context.GetString(Resource.String.content_authority);
    }
}
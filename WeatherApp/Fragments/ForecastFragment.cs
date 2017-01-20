using System;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fragment = Android.App.Fragment;
using LoaderManager = Android.App.LoaderManager;

namespace WeatherApp.Fragments
{
    public class ForecastFragment : Fragment, LoaderManager.ILoaderCallbacks,
        ISharedPreferencesOnSharedPreferenceChangeListener
    {

        ForecastAdapter forecastAdapter;
        private const string LogTag = "ForecastAdapter";
        private RecyclerView recyclerView;
        private int position = RecyclerView.NoPosition;
        private bool useTodayLayout, autoSelectView;
        private static readonly string SelectedKey = "selected_position";
        private const int UrlLoader = 0;
        private bool holdForTransition;
        private int choiceMode;

        private readonly string[] forecastColumns =
        {
            // In this case the id needs to be fully qualified with a table name, since
            // the content provider joins the location & weather tables in the background
            // (both have an _id column)
            // On the one hand, that's annoying.  On the other, you can search the weather table
            // using the location set by the user, which is only in the Location table.
            // So the convenience is worth it.
            WeatherContractOpen.WeatherEntryOpen.TableName + "." + WeatherContractOpen.WeatherEntryOpen.Id,
            WeatherContractOpen.WeatherEntryOpen.ColumnDate,
            WeatherContractOpen.WeatherEntryOpen.ColumnShortDesc,
            WeatherContractOpen.WeatherEntryOpen.ColumnMaxTemp,
            WeatherContractOpen.WeatherEntryOpen.ColumnMinTemp,
            WeatherContractOpen.LocationEntryOpen.ColumnLocationSetting,
            WeatherContractOpen.WeatherEntryOpen.ColumnWeatherId,
            WeatherContractOpen.LocationEntryOpen.ColumnCoordLat,
            WeatherContractOpen.LocationEntryOpen.ColumnCoordLong
        };

        // These indices are tied to FORECAST_COLUMNS.  If FORECAST_COLUMNS changes, these
        // must change.
        public const int ColWeatherId = 0;
        public const int ColWeatherDate = 1;
        public const int ColWeatherDesc = 2;
        public const int ColWeatherMaxTemp = 3;
        public const int ColWeatherMinTemp = 4;
        public const int ColLocationSetting = 5;
        public const int ColWeatherConditionId = 6;
        public const int ColCoordLat = 7;
        public const int ColCoordLong = 8;


        public interface ICallback
        {
            void OnItemSelected (Android.Net.Uri dateUri, ForecastAdapter.ForecastAdapterViewHolder vh);
        }

        public ForecastFragment ()
        {

        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);
        }

        public override void OnResume ()
        {
            PreferenceManager.GetDefaultSharedPreferences(Activity)
                .RegisterOnSharedPreferenceChangeListener(this);
            base.OnResume();
        }

        public override void OnPause ()
        {
            PreferenceManager.GetDefaultSharedPreferences(Activity)
                .UnregisterOnSharedPreferenceChangeListener(this);
            base.OnPause();
        }

        public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.forecastfragment, menu);
        }

        public override bool OnOptionsItemSelected (IMenuItem item)
        {
            var id = item.ItemId;
            if (id == Resource.Id.action_viewLocation)
            {
                OpenPreferredLocationInMap();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnInflate (Context context, IAttributeSet attrs, Bundle savedInstanceState)
        {
            base.OnInflate(context, attrs, savedInstanceState);
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.ForecastFragment,
                0, 0);
            choiceMode = a.GetInt(Resource.Styleable.ForecastFragment_android_choiceMode, (int)ChoiceMode.None);
            autoSelectView = a.GetBoolean(Resource.Styleable.ForecastFragment_autoSelectView, false);
            holdForTransition = a.GetBoolean(Resource.Styleable.ForecastFragment_sharedElementTransitions, false);
            a.Recycle();
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_main, container, false);
            System.Diagnostics.Debug.WriteLine("On CreateView called Forecast Fragment");
            try
            {

                recyclerView = (RecyclerView)rootView.FindViewById(Resource.Id.recyclerview_forecast);
                var parallaxView = rootView.FindViewById(Resource.Id.parallax_bar);
                var emptyView = rootView.FindViewById(Resource.Id.recyclerview_forecast_empty);
                recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                recyclerView.HasFixedSize = true;
                forecastAdapter = new ForecastAdapter(Activity, emptyView, choiceMode);
                forecastAdapter.ItemClick += OnClick;
                recyclerView.SetAdapter(forecastAdapter);
                if (parallaxView != null)
                {
                    recyclerView.AddOnScrollListener(new OnScroll(parallaxView));
                }
                var appbarView = (AppBarLayout)rootView.FindViewById(Resource.Id.appbar);
                if (appbarView != null)
                {
                    ViewCompat.SetElevation(appbarView, 0);
                    recyclerView.AddOnScrollListener(new OnScroll(appbarView));
                }
                if (savedInstanceState != null)
                {
                    if (savedInstanceState.ContainsKey(SelectedKey))
                    {
                        position = savedInstanceState.GetInt(SelectedKey);
                    }
                    forecastAdapter.OnRestoreInstanceState(savedInstanceState);
                }

                forecastAdapter.SetUseTodayLayout(useTodayLayout);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Log.WriteLine(LogPriority.Error, LogTag, ex.ToString());
                throw;
            }

            return rootView;
        }




        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            // We hold for transition here just in-case the activity
            // needs to be re-created. In a standard return transition,
            // this doesn't actually make a difference.
            if (holdForTransition)
            {
                Activity.PostponeEnterTransition();
            }
            LoaderManager.InitLoader(UrlLoader, null, this);
            base.OnActivityCreated(savedInstanceState);
        }

        public void OnLocationChanged ()
        {
            LoaderManager.RestartLoader(UrlLoader, null, this);
        }

        private void OpenPreferredLocationInMap ()
        {
            var c = forecastAdapter.GetCursor();
            if (null != c)
            {
                c.MoveToPosition(0);
                var posLat = c.GetString(ColCoordLat);
                var posLong = c.GetString(ColCoordLong);
                var geoLocation = Android.Net.Uri.Parse("geo:" + posLat + "," + posLong);

                var mapIntent = new Intent(Intent.ActionView, geoLocation);
                if (mapIntent.ResolveActivity(Activity.PackageManager) != null)
                {
                    StartActivity(mapIntent);
                }
                else
                {
                    Toast.MakeText(Activity, "No Map App found", ToastLength.Long).Show();
                    Log.Debug("Main Activity", "Couldn't call " + geoLocation.ToString() + ", No Maps App");
                }

            }

            //Alternative way of calling the activity but not a fully implicint intent
            //			var geoLocation = Android.Net.Uri.Parse("http://maps.google.com/maps/place/"+zipCode);
            //
            //			var mapIntent = new Intent(Intent.ActionView,geoLocation);
            //			mapIntent.SetClassName ("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            //			try
            //			{
            //				StartActivity (mapIntent);
            //
            //			}
            //			catch(ActivityNotFoundException ex)
            //			{
            //				try
            //				{
            //					Intent unrestrictedIntent = new Intent(Intent.ActionView, geoLocation);
            //					StartActivity(unrestrictedIntent);
            //				}
            //				catch(ActivityNotFoundException innerEx)
            //				{
            //					Toast.MakeText(this, "Please install a maps application", ToastLength.Long).Show();
            //				}
            //			}
        }

        public override void OnSaveInstanceState (Bundle outState)
        {
            if (position != RecyclerView.NoPosition)
            {
                outState.PutInt(SelectedKey, position);
            }
            forecastAdapter.OnSaveInstanceState(outState);
            base.OnSaveInstanceState(outState);
        }

        public Loader OnCreateLoader (int id, Bundle args)
        {
            var locationSetting = Utility.GetPreferredLocation(Activity);

            var sortOrder = WeatherContractOpen.WeatherEntryOpen.ColumnDate + " ASC";
            var weatherForLocationUri = WeatherContractOpen.WeatherEntryOpen.BuildWeatherLocationWithStartDate(
                                                        locationSetting, DateTime.Now.Date.Ticks);

            return new CursorLoader(Activity, weatherForLocationUri, forecastColumns, null, null, sortOrder);
        }

        public void OnLoadFinished (Loader loader, Java.Lang.Object data)
        {
            var cursor = (ICursor)data;
            forecastAdapter.SwapCursor(cursor);
            if (position != RecyclerView.NoPosition)
                recyclerView.SmoothScrollToPosition(position);

            UpdateEmptyView();

            if (cursor.Count == 0)
            {
                Activity.StartPostponedEnterTransition();
            }
            else
            {
                recyclerView.ViewTreeObserver.PreDraw += (sender, args) =>
                    {
                        // Since we know we're going to get items, we keep the listener around until
                        // we see Children.
                        if (recyclerView.ChildCount > 0)
                        {
                            var itemPosition = forecastAdapter.GetSelectedItemPosition();
                            if (RecyclerView.NoPosition == itemPosition) itemPosition = 0;
                            var vh = recyclerView.FindViewHolderForAdapterPosition(itemPosition);
                            if (null != vh && autoSelectView)
                            {
                                forecastAdapter.SelectView(vh);
                            }
                            if (holdForTransition)
                            {
                                Activity.StartPostponedEnterTransition();
                            }


                        }
                    };
            }
        }

        public void OnLoaderReset (Loader loader)
        {
            forecastAdapter.SwapCursor(null);
        }

        public void SetUseTodayLayout (bool useTodayLayout)
        {
            this.useTodayLayout = useTodayLayout;
            forecastAdapter?.SetUseTodayLayout(this.useTodayLayout);
        }

        private void UpdateEmptyView ()
        {
            if (forecastAdapter.ItemCount == 0)
            {

                var tv = (TextView)View.FindViewById(Resource.Id.recyclerview_forecast_empty);

                if (tv != null)
                {
                    var locStatus = Utility.GetLocationStatus(Activity);

                    switch (locStatus)
                    {
                        case (int)Helpers.LocationStatus.LocationStatusServerDown:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_server_down);
                            break;
                        case (int)Helpers.LocationStatus.LocationStatusServerInvalid:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_server_error);
                            break;
                        case (int)Helpers.LocationStatus.LocationStatusUnkown:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_server_unknown);
                            break;
                        case (int)Helpers.LocationStatus.LocationStatusInvalid:
                            tv.Text = Activity.GetString(Resource.String.empty_forecast_list_invalid_location);
                            break;
                        default:
                            if (!Utility.CheckNetworkStatus(Activity))
                            {
                                tv.Text = "Weather information not available. No network connection.";
                            }
                            break;
                    }
                }

            }
        }

        public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(GetString(Resource.String.pref_location_status_key)))
            {
                UpdateEmptyView();
            }
        }

        public void OnClick (object sender, long date)
        {
            var vh = (ForecastAdapter.ForecastAdapterViewHolder)sender;
            var locationSetting = Utility.GetPreferredLocation(Activity);
            ((ICallback)Activity)
                    .OnItemSelected(WeatherContractOpen.WeatherEntryOpen.BuildWeatherLocationWithDate(
                                    locationSetting, date), vh);
            position = vh.AdapterPosition;
        }

        public override void OnDestroy ()
        {
            base.OnDestroy();
            recyclerView?.ClearOnScrollListeners();
        }
    }

    public class OnScroll : RecyclerView.OnScrollListener
    {
        private readonly View parallaxView;
        private readonly AppBarLayout appBarView;
        public OnScroll (View parallaxView)
        {
            this.parallaxView = parallaxView;
        }
        public OnScroll (AppBarLayout appBarView)
        {
            this.appBarView = appBarView;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            if (appBarView == null)
            {
                base.OnScrolled(recyclerView, dx, dy);
                var max = parallaxView.Height;
                if (dy > 0)
                {
                    parallaxView.TranslationY = Math.Max(-max, parallaxView.TranslationY - dy / 2);
                }
                else
                {
                    parallaxView.TranslationY = Math.Min(0, parallaxView.TranslationY - dy / 2);
                }
            }
            else
            {
                if (0 == recyclerView.ComputeVerticalScrollOffset())
                {
                    appBarView.Elevation = 0;
                }
                else
                {
                    appBarView.Elevation = 5;
                }
            }
           
        }

    }

}


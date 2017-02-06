
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Android.Preferences;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Gms.Location.Places;
using Android.Gms.Location.Places.UI;
using Android.Gms.Maps.Model;
using Android.Support.Design.Widget;
using Android.Text;
using WeatherApp.Sync;

namespace WeatherApp
{
    public class SettingsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private const int PlacePickerRequest = 9090;
        private ImageView attribution;

        public SettingsFragment ()
        {

        }

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            AddPreferencesFromResource(Resource.Xml.pref_general);
            foreach (var key in PreferenceManager.GetDefaultSharedPreferences(Activity).All.Keys)
            {

                OnSharedPreferenceChanged(PreferenceManager.GetDefaultSharedPreferences(Activity), key);
            }

            attribution = new ImageView(Activity);
            attribution.SetImageResource(Resource.Drawable.powered_by_google_light);

            if (!Utility.IsLocationLatLonAvailable(Activity))
            {
                attribution.Visibility = ViewStates.Gone;
            }

           
        }

        public override void OnResume ()
        {
            base.OnResume();
            PreferenceManager.GetDefaultSharedPreferences(Activity)
            .RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnPause ()
        {
            base.OnPause();
            PreferenceManager.GetDefaultSharedPreferences(Activity)
                .UnregisterOnSharedPreferenceChangeListener(this);
        }


        public void OnSharedPreferenceChanged (ISharedPreferences sharedPreferences, string key)
        {
            if (key.Equals(GetString(Resource.String.pref_location_key)))
            {
                // we've changed the location
                // first clear locationStatus
                var editor = sharedPreferences.Edit();
                editor.Remove(GetString(Resource.String.pref_location_latitude));
                editor.Remove(GetString(Resource.String.pref_location_longitude));
                editor.Commit();

                if (attribution != null)
                {
                    attribution.Visibility = ViewStates.Gone;
                }

                Utility.ResetLocationStatus(Activity);
                SunshineSyncAdapter.SyncImmediately(Activity);
            }
            else if (key.Equals(GetString(Resource.String.pref_temp_key)))
            {
                // units have changed. update lists of weather entries accordingly
                Activity.ContentResolver.NotifyChange(WeatherContractOpen.WeatherEntryOpen.ContentUri, null);
            }
            else if (key.Equals(GetString(Resource.String.pref_location_status_key)))
            {
                // our location status has changed.  Update the summary accordingly
                var locationPreference = FindPreference(GetString(Resource.String.pref_location_key));
                BindPreferenceSummaryToValue(locationPreference);
            }
            else if (key.Equals(GetString(Resource.String.pref_art_pack_key)))
            {
                // art pack have changed. update lists of weather entries accordingly
                Activity.ContentResolver.NotifyChange(WeatherContractOpen.WeatherEntryOpen.ContentUri, null);
            }
        }

        public void SetPreferenceSummary (Preference pref, object value)
        {
            var stringValue = value.ToString();
            var key = pref.Key;

            if (pref == null)
            {
                return;
            }
            if (pref.GetType() == typeof(ListPreference))
            {
                var listPref = (ListPreference)pref;
               
                var prefIndex = listPref.FindIndexOfValue(stringValue);
                if (prefIndex > 0)
                    pref.Summary = listPref.Value;
            }

            else if (key.Equals(GetString(Resource.String.pref_location_key)))
            {
                var status = Utility.GetLocationStatus(Activity);
                switch (status)
                {
                    case (int)Helpers.LocationStatus.LocationStatusOk:
                        pref.Summary = stringValue;
                        break;
                    case (int)Helpers.LocationStatus.LocationStatusUnkown:
                        pref.Summary = string.Format(Activity.GetString(Resource.String.pref_location_unknown_description),
                            stringValue);
                        break;
                    case (int)Helpers.LocationStatus.LocationStatusInvalid:
                        pref.Summary = string.Format(Activity.GetString(Resource.String.pref_location_error_description),
                            stringValue);
                        break;
                    default:
                        pref.Summary = stringValue;
                        break;
                }

            }
            else
            {
                pref.Summary = stringValue;
            }
        }

        private void BindPreferenceSummaryToValue (Preference preference)
        {
            // Set the preference summaries
            SetPreferenceSummary(preference,
                    PreferenceManager
                            .GetDefaultSharedPreferences(preference.Context)
                            .GetString(preference.Key, ""));
        }

        public override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            // Check to see if the result is from our Place Picker intent
            if (requestCode == PlacePickerRequest)
            {
                // Make sure the request was successful
                if (resultCode == Result.Ok)
                {
                    var place = PlacePicker.GetPlace(Activity, data);
                    var address = place.AddressFormatted.ToString();


                    var latLong = place.LatLng;
                    var sharedPreferences =
                            PreferenceManager.GetDefaultSharedPreferences(Activity);
                    var editor = sharedPreferences.Edit();
                    editor.PutString(GetString(Resource.String.pref_location_key), address);
                    editor.PutFloat(GetString(Resource.String.pref_location_longitude), float.Parse(latLong.Longitude.ToString(CultureInfo.InvariantCulture)));
                    editor.PutFloat(GetString(Resource.String.pref_location_latitude), float.Parse(latLong.Latitude.ToString(CultureInfo.InvariantCulture)));
                    editor.Commit();

                    // If the provided place doesn't have an address, we'll form a display-friendly
                    // string from the latlng values.
                    if (TextUtils.IsEmpty(address))
                    {
                        address = string.Format($"({latLong.Latitude}.2f, {latLong.Longitude}.2f)");
                    }
                    var locationPreference = FindPreference(GetString(Resource.String.pref_location_key));
                    SetPreferenceSummary(locationPreference, address);
                  
                    // Add attributions for our new PlacePicker location.
                    if (attribution != null)
                    {
                        attribution.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        // For pre-Honeycomb devices, we cannot add a footer, so we will use a snackbar
                        var rootView = View.FindViewById(Android.Resource.Id.Content);
                        Snackbar.Make(rootView, GetString(Resource.String.attribution_text),
                                Snackbar.LengthLong).Show();
                    }

                    Utility.ResetLocationStatus(Activity);
                    SunshineSyncAdapter.SyncImmediately(Activity);
                }


            }
            else
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }
        }


    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Common;
using Android.Gms.Location.Places.UI;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WeatherApp.CustomViews
{
    class LocationEditTextPreference : EditTextPreference
    {
        private readonly int minLength;
        private const int PlacePickerRequest = 9090;

        public LocationEditTextPreference (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LocationEditTextPreference (Context context) : base(context)
        {
        }

        public LocationEditTextPreference (Context context, IAttributeSet attrs) : base(context, attrs)
        {
            var a = context.Theme.ObtainStyledAttributes(attrs,
                Resource.Styleable.LocationEditTextPreference, 0, 0);

            try
            {
                minLength = a.GetInteger(Resource.Styleable.LocationEditTextPreference_MinLength, 2);
            }
            finally
            {
                a.Recycle();
            }

            var ApiAvailability = GoogleApiAvailability.Instance;
            var resultCode = ApiAvailability.IsGooglePlayServicesAvailable(context);
            if (resultCode == ConnectionResult.Success)
            {
                WidgetLayoutResource = Resource.Layout.pref_current_location;
            }

        }

        public LocationEditTextPreference (Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public LocationEditTextPreference (Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected override void ShowDialog (Bundle state)
        {
            base.ShowDialog(state);
            var text = this.EditText;

            text.AfterTextChanged += (s, e) =>
            {
                var d = this.Dialog;
                if (d == null || d.GetType() != typeof(AlertDialog)) return;
                var dialog = (AlertDialog)d;
                var posBtn = dialog.GetButton((int)DialogButtonType.Positive);
                posBtn.Enabled = text.Text.Length >= minLength;
            };
        }

        protected override View OnCreateView (ViewGroup parent)
        {
            var view = base.OnCreateView(parent);
            var currentLocation = view.FindViewById(Resource.Id.current_location);
            if (currentLocation != null)
            {
                currentLocation.Click += (sender, args) =>
                {
                    var context = Context;

                    // Launch the Place Picker so that the user can specify their location, and then
                    // return the result to SettingsActivity.
                    // TODO(student): Create a PlacePicker.IntentBuilder object here.
                    var intent = new PlacePicker.IntentBuilder();
                    // We are in a view right now, not an activity. So we need to get ourselves
                    // an activity that we can use to start our Place Picker intent. By using
                    // SettingsActivity in this way, we can ensure the result of the Place Picker
                    // intent comes to the right place for us to process it.
                    var settingsActivity = (SettingsActivity)context;
                    try
                    {
                        // TODO(student): Launch the intent using your settingsActivity object to access
                        // startActivityForResult(). You'll need to build your builder object and use
                        // the request code we declared in SettingsActivity.
                        settingsActivity.StartActivityForResult(intent.Build(settingsActivity),PlacePickerRequest);
                    }
                    catch (Exception ex) when (ex is GooglePlayServicesNotAvailableException
                        || ex is GooglePlayServicesRepairableException)
                    {
                        // What did you do?? This is why we check Google Play services in onResume!!!
                        // The difference in these exception types is the difference between pausing
                        // for a moment to prompt the user to update/install/enable Play services vs
                        // complete and utter failure.
                        // If you prefer to manage Google Play services dynamically, then you can do so
                        // by responding to these exceptions in the right moment. But I prefer a cleaner
                        // user experience, which is why you check all of this when the app resumes,
                        // and then disable/enable features based on that availability.
                    }

                };

            }
            return view;

        }
    }
}
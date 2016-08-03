using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
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
        private int minLength;
        public LocationEditTextPreference (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LocationEditTextPreference (Context context) : base(context)
        {
        }

        public LocationEditTextPreference (Context context, IAttributeSet attrs) : base(context, attrs)
        {
            var a = Context.Theme.ObtainStyledAttributes(attrs,
                Resource.Styleable.LocationEditTextPreference, 0, 0);

            try
            {
                minLength = a.GetInteger(Resource.Styleable.LocationEditTextPreference_MinLength, 2);
            }
            finally
            {
                a.Recycle();
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
    }
}
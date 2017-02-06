package md5a0fa4cb098b8b089bb605cb563250f96;


public class LocationEditTextPreference
	extends android.preference.EditTextPreference
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_showDialog:(Landroid/os/Bundle;)V:GetShowDialog_Landroid_os_Bundle_Handler\n" +
			"n_onCreateView:(Landroid/view/ViewGroup;)Landroid/view/View;:GetOnCreateView_Landroid_view_ViewGroup_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.CustomViews.LocationEditTextPreference, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", LocationEditTextPreference.class, __md_methods);
	}


	public LocationEditTextPreference (android.content.Context p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == LocationEditTextPreference.class)
			mono.android.TypeManager.Activate ("WeatherApp.CustomViews.LocationEditTextPreference, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public LocationEditTextPreference (android.content.Context p0, android.util.AttributeSet p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == LocationEditTextPreference.class)
			mono.android.TypeManager.Activate ("WeatherApp.CustomViews.LocationEditTextPreference, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public LocationEditTextPreference (android.content.Context p0, android.util.AttributeSet p1, int p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == LocationEditTextPreference.class)
			mono.android.TypeManager.Activate ("WeatherApp.CustomViews.LocationEditTextPreference, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public LocationEditTextPreference (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3) throws java.lang.Throwable
	{
		super (p0, p1, p2, p3);
		if (getClass () == LocationEditTextPreference.class)
			mono.android.TypeManager.Activate ("WeatherApp.CustomViews.LocationEditTextPreference, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public void showDialog (android.os.Bundle p0)
	{
		n_showDialog (p0);
	}

	private native void n_showDialog (android.os.Bundle p0);


	public android.view.View onCreateView (android.view.ViewGroup p0)
	{
		return n_onCreateView (p0);
	}

	private native android.view.View n_onCreateView (android.view.ViewGroup p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

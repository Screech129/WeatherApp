package md5fbd53c09b56bd1d9ce119b1b7718dab5;


public class OnScroll
	extends android.support.v7.widget.RecyclerView.OnScrollListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onScrolled:(Landroid/support/v7/widget/RecyclerView;II)V:GetOnScrolled_Landroid_support_v7_widget_RecyclerView_IIHandler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Fragments.OnScroll, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnScroll.class, __md_methods);
	}


	public OnScroll () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnScroll.class)
			mono.android.TypeManager.Activate ("WeatherApp.Fragments.OnScroll, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public OnScroll (android.view.View p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnScroll.class)
			mono.android.TypeManager.Activate ("WeatherApp.Fragments.OnScroll, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

	public OnScroll (android.support.design.widget.AppBarLayout p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnScroll.class)
			mono.android.TypeManager.Activate ("WeatherApp.Fragments.OnScroll, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.Design.Widget.AppBarLayout, Xamarin.Android.Support.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onScrolled (android.support.v7.widget.RecyclerView p0, int p1, int p2)
	{
		n_onScrolled (p0, p1, p2);
	}

	private native void n_onScrolled (android.support.v7.widget.RecyclerView p0, int p1, int p2);

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

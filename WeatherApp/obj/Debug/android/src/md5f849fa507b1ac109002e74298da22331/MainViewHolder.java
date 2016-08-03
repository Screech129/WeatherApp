package md5f849fa507b1ac109002e74298da22331;


public class MainViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WeatherApp.MainViewHolder, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MainViewHolder.class, __md_methods);
	}


	public MainViewHolder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MainViewHolder.class)
			mono.android.TypeManager.Activate ("WeatherApp.MainViewHolder, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public MainViewHolder (android.view.View p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == MainViewHolder.class)
			mono.android.TypeManager.Activate ("WeatherApp.MainViewHolder, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

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

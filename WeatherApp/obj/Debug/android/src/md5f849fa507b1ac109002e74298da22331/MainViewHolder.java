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

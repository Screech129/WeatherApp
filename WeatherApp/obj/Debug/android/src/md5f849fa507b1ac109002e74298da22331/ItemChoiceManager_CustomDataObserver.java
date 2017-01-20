package md5f849fa507b1ac109002e74298da22331;


public class ItemChoiceManager_CustomDataObserver
	extends android.support.v7.widget.RecyclerView.AdapterDataObserver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onChanged:()V:GetOnChangedHandler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.ItemChoiceManager+CustomDataObserver, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ItemChoiceManager_CustomDataObserver.class, __md_methods);
	}


	public ItemChoiceManager_CustomDataObserver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ItemChoiceManager_CustomDataObserver.class)
			mono.android.TypeManager.Activate ("WeatherApp.ItemChoiceManager+CustomDataObserver, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public ItemChoiceManager_CustomDataObserver (android.support.v7.widget.RecyclerView.Adapter p0, md5f849fa507b1ac109002e74298da22331.ItemChoiceManager p1) throws java.lang.Throwable
	{
		super ();
		if (getClass () == ItemChoiceManager_CustomDataObserver.class)
			mono.android.TypeManager.Activate ("WeatherApp.ItemChoiceManager+CustomDataObserver, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V7.Widget.RecyclerView+Adapter, Xamarin.Android.Support.v7.RecyclerView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null:WeatherApp.ItemChoiceManager, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0, p1 });
	}


	public void onChanged ()
	{
		n_onChanged ();
	}

	private native void n_onChanged ();

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

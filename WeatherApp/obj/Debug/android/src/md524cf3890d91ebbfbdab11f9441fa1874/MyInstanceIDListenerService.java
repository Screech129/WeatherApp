package md524cf3890d91ebbfbdab11f9441fa1874;


public class MyInstanceIdListenerService
	extends com.google.android.gms.iid.InstanceIDListenerService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTokenRefresh:()V:GetOnTokenRefreshHandler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Services.MyInstanceIdListenerService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MyInstanceIdListenerService.class, __md_methods);
	}


	public MyInstanceIdListenerService () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MyInstanceIdListenerService.class)
			mono.android.TypeManager.Activate ("WeatherApp.Services.MyInstanceIdListenerService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTokenRefresh ()
	{
		n_onTokenRefresh ();
	}

	private native void n_onTokenRefresh ();

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

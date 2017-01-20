package md524cf3890d91ebbfbdab11f9441fa1874;


public class MyGcmListenerService
	extends com.google.android.gms.gcm.GcmListenerService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMessageReceived:(Ljava/lang/String;Landroid/os/Bundle;)V:GetOnMessageReceived_Ljava_lang_String_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Services.MyGcmListenerService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MyGcmListenerService.class, __md_methods);
	}


	public MyGcmListenerService () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MyGcmListenerService.class)
			mono.android.TypeManager.Activate ("WeatherApp.Services.MyGcmListenerService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onMessageReceived (java.lang.String p0, android.os.Bundle p1)
	{
		n_onMessageReceived (p0, p1);
	}

	private native void n_onMessageReceived (java.lang.String p0, android.os.Bundle p1);

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

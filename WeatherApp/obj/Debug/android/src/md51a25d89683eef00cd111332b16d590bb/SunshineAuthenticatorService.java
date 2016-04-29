package md51a25d89683eef00cd111332b16d590bb;


public class SunshineAuthenticatorService
	extends android.app.Service
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:()V:GetOnCreateHandler\n" +
			"n_onBind:(Landroid/content/Intent;)Landroid/os/IBinder;:GetOnBind_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Sync.SunshineAuthenticatorService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SunshineAuthenticatorService.class, __md_methods);
	}


	public SunshineAuthenticatorService () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SunshineAuthenticatorService.class)
			mono.android.TypeManager.Activate ("WeatherApp.Sync.SunshineAuthenticatorService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate ()
	{
		n_onCreate ();
	}

	private native void n_onCreate ();


	public android.os.IBinder onBind (android.content.Intent p0)
	{
		return n_onBind (p0);
	}

	private native android.os.IBinder n_onBind (android.content.Intent p0);

	java.util.ArrayList refList;
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

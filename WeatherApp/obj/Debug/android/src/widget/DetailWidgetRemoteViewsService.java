package widget;


public class DetailWidgetRemoteViewsService
	extends android.widget.RemoteViewsService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onGetViewFactory:(Landroid/content/Intent;)Landroid/widget/RemoteViewsService$RemoteViewsFactory;:GetOnGetViewFactory_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Widget.DetailWidgetRemoteViewsService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DetailWidgetRemoteViewsService.class, __md_methods);
	}


	public DetailWidgetRemoteViewsService () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DetailWidgetRemoteViewsService.class)
			mono.android.TypeManager.Activate ("WeatherApp.Widget.DetailWidgetRemoteViewsService, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public android.widget.RemoteViewsService.RemoteViewsFactory onGetViewFactory (android.content.Intent p0)
	{
		return n_onGetViewFactory (p0);
	}

	private native android.widget.RemoteViewsService.RemoteViewsFactory n_onGetViewFactory (android.content.Intent p0);

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

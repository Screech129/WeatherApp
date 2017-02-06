package android.appwidget;


public class provider
	extends android.appwidget.AppWidgetProvider
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onUpdate:(Landroid/content/Context;Landroid/appwidget/AppWidgetManager;[I)V:GetOnUpdate_Landroid_content_Context_Landroid_appwidget_AppWidgetManager_arrayIHandler\n" +
			"n_onAppWidgetOptionsChanged:(Landroid/content/Context;Landroid/appwidget/AppWidgetManager;ILandroid/os/Bundle;)V:GetOnAppWidgetOptionsChanged_Landroid_content_Context_Landroid_appwidget_AppWidgetManager_ILandroid_os_Bundle_Handler\n" +
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Widget.TodayWidgetProvider, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", provider.class, __md_methods);
	}


	public provider () throws java.lang.Throwable
	{
		super ();
		if (getClass () == provider.class)
			mono.android.TypeManager.Activate ("WeatherApp.Widget.TodayWidgetProvider, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onUpdate (android.content.Context p0, android.appwidget.AppWidgetManager p1, int[] p2)
	{
		n_onUpdate (p0, p1, p2);
	}

	private native void n_onUpdate (android.content.Context p0, android.appwidget.AppWidgetManager p1, int[] p2);


	public void onAppWidgetOptionsChanged (android.content.Context p0, android.appwidget.AppWidgetManager p1, int p2, android.os.Bundle p3)
	{
		n_onAppWidgetOptionsChanged (p0, p1, p2, p3);
	}

	private native void n_onAppWidgetOptionsChanged (android.content.Context p0, android.appwidget.AppWidgetManager p1, int p2, android.os.Bundle p3);


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

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

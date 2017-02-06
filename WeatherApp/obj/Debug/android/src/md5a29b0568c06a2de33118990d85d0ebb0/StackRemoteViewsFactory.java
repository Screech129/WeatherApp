package md5a29b0568c06a2de33118990d85d0ebb0;


public class StackRemoteViewsFactory
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.widget.RemoteViewsService.RemoteViewsFactory
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getCount:()I:GetGetCountHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_hasStableIds:()Z:GetHasStableIdsHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_getLoadingView:()Landroid/widget/RemoteViews;:GetGetLoadingViewHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_getViewTypeCount:()I:GetGetViewTypeCountHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_getItemId:(I)J:GetGetItemId_IHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_getViewAt:(I)Landroid/widget/RemoteViews;:GetGetViewAt_IHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onCreate:()V:GetOnCreateHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onDataSetChanged:()V:GetOnDataSetChangedHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onDestroy:()V:GetOnDestroyHandler:Android.Widget.RemoteViewsService/IRemoteViewsFactoryInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Widget.StackRemoteViewsFactory, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", StackRemoteViewsFactory.class, __md_methods);
	}


	public StackRemoteViewsFactory () throws java.lang.Throwable
	{
		super ();
		if (getClass () == StackRemoteViewsFactory.class)
			mono.android.TypeManager.Activate ("WeatherApp.Widget.StackRemoteViewsFactory, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public StackRemoteViewsFactory (android.content.Context p0, android.content.ContentResolver p1, java.lang.String p2, android.content.Intent p3) throws java.lang.Throwable
	{
		super ();
		if (getClass () == StackRemoteViewsFactory.class)
			mono.android.TypeManager.Activate ("WeatherApp.Widget.StackRemoteViewsFactory, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Content.ContentResolver, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:Android.Content.Intent, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public int getCount ()
	{
		return n_getCount ();
	}

	private native int n_getCount ();


	public boolean hasStableIds ()
	{
		return n_hasStableIds ();
	}

	private native boolean n_hasStableIds ();


	public android.widget.RemoteViews getLoadingView ()
	{
		return n_getLoadingView ();
	}

	private native android.widget.RemoteViews n_getLoadingView ();


	public int getViewTypeCount ()
	{
		return n_getViewTypeCount ();
	}

	private native int n_getViewTypeCount ();


	public long getItemId (int p0)
	{
		return n_getItemId (p0);
	}

	private native long n_getItemId (int p0);


	public android.widget.RemoteViews getViewAt (int p0)
	{
		return n_getViewAt (p0);
	}

	private native android.widget.RemoteViews n_getViewAt (int p0);


	public void onCreate ()
	{
		n_onCreate ();
	}

	private native void n_onCreate ();


	public void onDataSetChanged ()
	{
		n_onDataSetChanged ();
	}

	private native void n_onDataSetChanged ();


	public void onDestroy ()
	{
		n_onDestroy ();
	}

	private native void n_onDestroy ();

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

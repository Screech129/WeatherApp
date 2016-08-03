package md5f849fa507b1ac109002e74298da22331;


public class ForecastAdapter
	extends android.widget.CursorAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getItemViewType:(I)I:GetGetItemViewType_IHandler\n" +
			"n_getViewTypeCount:()I:GetGetViewTypeCountHandler\n" +
			"n_newView:(Landroid/content/Context;Landroid/database/Cursor;Landroid/view/ViewGroup;)Landroid/view/View;:GetNewView_Landroid_content_Context_Landroid_database_Cursor_Landroid_view_ViewGroup_Handler\n" +
			"n_bindView:(Landroid/view/View;Landroid/content/Context;Landroid/database/Cursor;)V:GetBindView_Landroid_view_View_Landroid_content_Context_Landroid_database_Cursor_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.ForecastAdapter, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ForecastAdapter.class, __md_methods);
	}


	public ForecastAdapter (android.content.Context p0, android.database.Cursor p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == ForecastAdapter.class)
			mono.android.TypeManager.Activate ("WeatherApp.ForecastAdapter, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Database.ICursor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public ForecastAdapter (android.content.Context p0, android.database.Cursor p1, boolean p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == ForecastAdapter.class)
			mono.android.TypeManager.Activate ("WeatherApp.ForecastAdapter, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Database.ICursor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ForecastAdapter (android.content.Context p0, android.database.Cursor p1, int p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == ForecastAdapter.class)
			mono.android.TypeManager.Activate ("WeatherApp.ForecastAdapter, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Database.ICursor, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Widget.CursorAdapterFlags, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public int getItemViewType (int p0)
	{
		return n_getItemViewType (p0);
	}

	private native int n_getItemViewType (int p0);


	public int getViewTypeCount ()
	{
		return n_getViewTypeCount ();
	}

	private native int n_getViewTypeCount ();


	public android.view.View newView (android.content.Context p0, android.database.Cursor p1, android.view.ViewGroup p2)
	{
		return n_newView (p0, p1, p2);
	}

	private native android.view.View n_newView (android.content.Context p0, android.database.Cursor p1, android.view.ViewGroup p2);


	public void bindView (android.view.View p0, android.content.Context p1, android.database.Cursor p2)
	{
		n_bindView (p0, p1, p2);
	}

	private native void n_bindView (android.view.View p0, android.content.Context p1, android.database.Cursor p2);

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

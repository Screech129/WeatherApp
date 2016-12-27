package md5f849fa507b1ac109002e74298da22331;


public class DetailFragment
	extends android.support.v4.app.Fragment
	implements
		mono.android.IGCUserPeer,
		android.support.v4.app.LoaderManager.LoaderCallbacks
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"n_onCreateOptionsMenu:(Landroid/view/Menu;Landroid/view/MenuInflater;)V:GetOnCreateOptionsMenu_Landroid_view_Menu_Landroid_view_MenuInflater_Handler\n" +
			"n_onActivityCreated:(Landroid/os/Bundle;)V:GetOnActivityCreated_Landroid_os_Bundle_Handler\n" +
			"n_onOptionsItemSelected:(Landroid/view/MenuItem;)Z:GetOnOptionsItemSelected_Landroid_view_MenuItem_Handler\n" +
			"n_onCreateLoader:(ILandroid/os/Bundle;)Landroid/support/v4/content/Loader;:GetOnCreateLoader_ILandroid_os_Bundle_Handler:Android.Support.V4.App.LoaderManager/ILoaderCallbacksInvoker, Xamarin.Android.Support.Fragment\n" +
			"n_onLoadFinished:(Landroid/support/v4/content/Loader;Ljava/lang/Object;)V:GetOnLoadFinished_Landroid_support_v4_content_Loader_Ljava_lang_Object_Handler:Android.Support.V4.App.LoaderManager/ILoaderCallbacksInvoker, Xamarin.Android.Support.Fragment\n" +
			"n_onLoaderReset:(Landroid/support/v4/content/Loader;)V:GetOnLoaderReset_Landroid_support_v4_content_Loader_Handler:Android.Support.V4.App.LoaderManager/ILoaderCallbacksInvoker, Xamarin.Android.Support.Fragment\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.DetailFragment, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DetailFragment.class, __md_methods);
	}


	public DetailFragment () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DetailFragment.class)
			mono.android.TypeManager.Activate ("WeatherApp.DetailFragment, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);


	public void onCreateOptionsMenu (android.view.Menu p0, android.view.MenuInflater p1)
	{
		n_onCreateOptionsMenu (p0, p1);
	}

	private native void n_onCreateOptionsMenu (android.view.Menu p0, android.view.MenuInflater p1);


	public void onActivityCreated (android.os.Bundle p0)
	{
		n_onActivityCreated (p0);
	}

	private native void n_onActivityCreated (android.os.Bundle p0);


	public boolean onOptionsItemSelected (android.view.MenuItem p0)
	{
		return n_onOptionsItemSelected (p0);
	}

	private native boolean n_onOptionsItemSelected (android.view.MenuItem p0);


	public android.support.v4.content.Loader onCreateLoader (int p0, android.os.Bundle p1)
	{
		return n_onCreateLoader (p0, p1);
	}

	private native android.support.v4.content.Loader n_onCreateLoader (int p0, android.os.Bundle p1);


	public void onLoadFinished (android.support.v4.content.Loader p0, java.lang.Object p1)
	{
		n_onLoadFinished (p0, p1);
	}

	private native void n_onLoadFinished (android.support.v4.content.Loader p0, java.lang.Object p1);


	public void onLoaderReset (android.support.v4.content.Loader p0)
	{
		n_onLoaderReset (p0);
	}

	private native void n_onLoaderReset (android.support.v4.content.Loader p0);

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

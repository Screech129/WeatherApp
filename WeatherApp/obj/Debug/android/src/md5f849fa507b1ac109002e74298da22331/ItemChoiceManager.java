package md5f849fa507b1ac109002e74298da22331;


public class ItemChoiceManager
	extends android.widget.AbsListView
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_isItemChecked:(I)Z:GetIsItemChecked_IHandler\n" +
			"n_setSelection:(I)V:GetSetSelection_IHandler\n" +
			"n_getAdapter:()Landroid/widget/ListAdapter;:GetGetAdapterHandler\n" +
			"n_setAdapter:(Landroid/widget/ListAdapter;)V:GetSetAdapter_Landroid_widget_ListAdapter_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.ItemChoiceManager, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ItemChoiceManager.class, __md_methods);
	}


	public ItemChoiceManager (android.content.Context p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == ItemChoiceManager.class)
			mono.android.TypeManager.Activate ("WeatherApp.ItemChoiceManager, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public ItemChoiceManager (android.content.Context p0, android.util.AttributeSet p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == ItemChoiceManager.class)
			mono.android.TypeManager.Activate ("WeatherApp.ItemChoiceManager, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public ItemChoiceManager (android.content.Context p0, android.util.AttributeSet p1, int p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == ItemChoiceManager.class)
			mono.android.TypeManager.Activate ("WeatherApp.ItemChoiceManager, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ItemChoiceManager (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3) throws java.lang.Throwable
	{
		super (p0, p1, p2, p3);
		if (getClass () == ItemChoiceManager.class)
			mono.android.TypeManager.Activate ("WeatherApp.ItemChoiceManager, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public boolean isItemChecked (int p0)
	{
		return n_isItemChecked (p0);
	}

	private native boolean n_isItemChecked (int p0);


	public void setSelection (int p0)
	{
		n_setSelection (p0);
	}

	private native void n_setSelection (int p0);


	public android.widget.ListAdapter getAdapter ()
	{
		return n_getAdapter ();
	}

	private native android.widget.ListAdapter n_getAdapter ();


	public void setAdapter (android.widget.ListAdapter p0)
	{
		n_setAdapter (p0);
	}

	private native void n_setAdapter (android.widget.ListAdapter p0);

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

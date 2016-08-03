package md51a25d89683eef00cd111332b16d590bb;


public class SunshineSyncAdapter
	extends android.content.AbstractThreadedSyncAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPerformSync:(Landroid/accounts/Account;Landroid/os/Bundle;Ljava/lang/String;Landroid/content/ContentProviderClient;Landroid/content/SyncResult;)V:GetOnPerformSync_Landroid_accounts_Account_Landroid_os_Bundle_Ljava_lang_String_Landroid_content_ContentProviderClient_Landroid_content_SyncResult_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherApp.Sync.SunshineSyncAdapter, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SunshineSyncAdapter.class, __md_methods);
	}


	public SunshineSyncAdapter (android.content.Context p0, boolean p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == SunshineSyncAdapter.class)
			mono.android.TypeManager.Activate ("WeatherApp.Sync.SunshineSyncAdapter, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public SunshineSyncAdapter (android.content.Context p0, boolean p1, boolean p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == SunshineSyncAdapter.class)
			mono.android.TypeManager.Activate ("WeatherApp.Sync.SunshineSyncAdapter, WeatherApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void onPerformSync (android.accounts.Account p0, android.os.Bundle p1, java.lang.String p2, android.content.ContentProviderClient p3, android.content.SyncResult p4)
	{
		n_onPerformSync (p0, p1, p2, p3, p4);
	}

	private native void n_onPerformSync (android.accounts.Account p0, android.os.Bundle p1, java.lang.String p2, android.content.ContentProviderClient p3, android.content.SyncResult p4);

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

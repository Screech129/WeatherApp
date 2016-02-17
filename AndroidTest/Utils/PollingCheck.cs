using System;
using NUnit.Framework;
using System.Threading;
using Android.Runtime;

namespace AndroidTest
{
	public class PollingCheck
	{
		private const int TIME_SLICE = 50;
		private long _timeout = 3000;

		public PollingCheck ()
		{
		}

		public PollingCheck (long timeout)
		{
			_timeout = timeout;
		}

		protected virtual bool check ()
		{
			return false;
		}

		public void run ()
		{
			if (check ()) {
				return;
			}
		

			long timeout = _timeout;

			while (timeout > 0) {
				try {
					Thread.Sleep (TIME_SLICE);
				} catch (ThreadInterruptedException ex) {
					Assert.Fail ("unexpected ThreadInterruptedException");
				}

				if (check ()) {
					return;
				}

				timeout -= TIME_SLICE;
			}

			Assert.Fail ("unexpected timout");
		}

		public static void check (string message, long timeout, Func<bool> condition)
		{
			while (timeout > 0) {
				if (condition ()) {
					return;
				}

				Thread.Sleep (TIME_SLICE);
				timeout -= TIME_SLICE;
			}
			Assert.Fail (message);
		}
	}
}


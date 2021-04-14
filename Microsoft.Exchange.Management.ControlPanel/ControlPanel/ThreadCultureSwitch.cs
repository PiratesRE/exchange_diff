using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ThreadCultureSwitch : IDisposable
	{
		public ThreadCultureSwitch() : this(CultureInfo.InstalledUICulture)
		{
		}

		public ThreadCultureSwitch(CultureInfo newCulture)
		{
			this.thread = Thread.CurrentThread;
			this.previousCulture = this.thread.CurrentCulture;
			this.previousUICulture = this.thread.CurrentUICulture;
			this.thread.CurrentCulture = newCulture;
			this.thread.CurrentUICulture = newCulture;
		}

		public void Dispose()
		{
			this.thread.CurrentCulture = this.previousCulture;
			this.thread.CurrentUICulture = this.previousUICulture;
		}

		private Thread thread;

		private CultureInfo previousCulture;

		private CultureInfo previousUICulture;
	}
}

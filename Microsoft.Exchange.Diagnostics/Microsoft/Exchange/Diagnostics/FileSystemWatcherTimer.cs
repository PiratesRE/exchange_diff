using System;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics
{
	internal class FileSystemWatcherTimer : IDisposable
	{
		internal FileSystemWatcherTimer(string filePath, Action notifyHandler)
		{
			if (notifyHandler == null)
			{
				throw new ArgumentNullException("notifyHandler");
			}
			this.filePath = filePath;
			this.notifyHandler = notifyHandler;
			SharedTimer.Instance.RegisterCallback(new TimerCallback(this.Callback));
		}

		public void Dispose()
		{
			SharedTimer.Instance.UnRegisterCallback(new TimerCallback(this.Callback));
		}

		private void Callback(object arg)
		{
			ConfigFileFingerPrint configFileFingerPrint = new ConfigFileFingerPrint(this.filePath);
			if (!configFileFingerPrint.Equals(this.lastFingerPrint))
			{
				InternalBypassTrace.TracingConfigurationTracer.TraceDebug(0, (long)this.GetHashCode(), "File changed, old attributes=\"{0}\", new attributes=\"{1}\"", new object[]
				{
					this.lastFingerPrint,
					configFileFingerPrint
				});
				this.lastFingerPrint = configFileFingerPrint;
				this.notifyHandler();
			}
		}

		private string filePath;

		private Action notifyHandler;

		private ConfigFileFingerPrint lastFingerPrint;
	}
}

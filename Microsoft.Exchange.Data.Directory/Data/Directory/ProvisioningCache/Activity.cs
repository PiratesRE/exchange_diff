using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal abstract class Activity : DisposeTrackableBase
	{
		private protected ProvisioningCache ProvisioningCache { protected get; private set; }

		internal bool GotStopSignalFromTestCode { get; private set; }

		protected Activity(ProvisioningCache cache)
		{
			if (cache == null)
			{
				throw new ArgumentNullException("cache");
			}
			this.ProvisioningCache = cache;
			this.GotStopSignalFromTestCode = false;
		}

		public abstract string Name { get; }

		private protected Thread AsyncThread { protected get; private set; }

		public Thread ExecuteAsync(Action<Activity, Exception> callback)
		{
			Thread thread = new Thread(new ParameterizedThreadStart(this.ExecuteAsyncEntryPoint));
			thread.IsBackground = true;
			thread.Start(callback);
			this.AsyncThread = thread;
			return thread;
		}

		internal virtual void StopExecute()
		{
			this.GotStopSignalFromTestCode = true;
		}

		protected void Execute()
		{
			try
			{
				this.InternalExecute();
			}
			catch (ThreadAbortException ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_PCActivityExit, this.Name.ToString(), new object[]
				{
					this.Name.ToString(),
					ex.ToString()
				});
			}
		}

		protected abstract void InternalExecute();

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Activity>(this);
		}

		private void ExecuteAsyncEntryPoint(object parameter)
		{
			Exception arg = null;
			try
			{
				ExWatson.SendReportOnUnhandledException(new ExWatson.MethodDelegate(this.Execute), (object ex) => !(ex is ThreadAbortException));
			}
			catch (Exception ex)
			{
				Exception ex2;
				arg = ex2;
			}
			finally
			{
				Action<Activity, Exception> action = (Action<Activity, Exception>)parameter;
				if (action != null)
				{
					action(this, arg);
				}
			}
		}
	}
}

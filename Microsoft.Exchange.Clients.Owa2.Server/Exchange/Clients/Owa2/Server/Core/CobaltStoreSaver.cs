using System;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CobaltStoreSaver : DisposeTrackableBase
	{
		public void Initialize(string smtpAddress, string exchangeSessionId, TimeSpan interval, Func<bool> autoSaveCallback, Action<Exception> permanentFailureCallback)
		{
			this.smtpAddress = smtpAddress;
			this.exchangeSessionId = exchangeSessionId;
			this.autoSaveInterval = interval;
			this.autoSave = autoSaveCallback;
			this.permanentFailure = permanentFailureCallback;
			this.timer = new Timer(new TimerCallback(this.TimerCallback), null, this.autoSaveInterval, this.autoSaveInterval);
		}

		public void SaveAndThrowExceptions()
		{
			lock (this.syncObject)
			{
				if (this.timer != null)
				{
					Exception ex = null;
					try
					{
						try
						{
							if (!this.autoSave())
							{
								this.Stop();
							}
						}
						catch (SaveConflictException)
						{
							if (!this.autoSave())
							{
								this.Stop();
							}
						}
					}
					catch (StoragePermanentException ex2)
					{
						ex = ex2;
					}
					catch (InvalidStoreIdException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						this.Stop();
						this.permanentFailure(ex);
						throw new StoragePermanentException(new LocalizedString(ex.Message), ex);
					}
				}
			}
		}

		private void Stop()
		{
			if (!Monitor.IsEntered(this.syncObject))
			{
				throw new InvalidOperationException("CobaltStoreSaver.Stop must only be invoked while holding lock.");
			}
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		public void SaveAndLogExceptions(RequestDetailsLogger logger)
		{
			Exception ex2;
			try
			{
				this.SaveAndThrowExceptions();
				return;
			}
			catch (StoragePermanentException ex)
			{
				ex2 = ex;
			}
			catch (StorageTransientException ex3)
			{
				ex2 = ex3;
			}
			logger.ActivityScope.SetProperty(ServiceCommonMetadata.GenericErrors, ex2.ToString());
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				lock (this.syncObject)
				{
					this.Stop();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CobaltStoreSaver>(this);
		}

		private void TimerCallback(object unused)
		{
			SimulatedWebRequestContext.ExecuteWithoutUserContext("WAC.AutoSave", delegate(RequestDetailsLogger logger)
			{
				WacUtilities.SetEventId(logger, "WAC.AutoSave");
				logger.ActivityScope.SetProperty(OwaServerLogger.LoggerData.PrimarySmtpAddress, this.smtpAddress);
				logger.ActivityScope.SetProperty(WacRequestHandlerMetadata.ExchangeSessionId, this.exchangeSessionId);
				this.SaveAndLogExceptions(logger);
			});
		}

		private string smtpAddress;

		private string exchangeSessionId;

		private Timer timer;

		private TimeSpan autoSaveInterval;

		private Func<bool> autoSave;

		private Action<Exception> permanentFailure;

		private object syncObject = new object();
	}
}

using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class GenericADNotificationHandler<T> : ADNotificationHandler where T : ADConfigurationObject, new()
	{
		internal GenericADNotificationHandler()
		{
			this.Register();
		}

		private IConfigurationSession ConfigSession
		{
			get
			{
				return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 58, "ConfigSession", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcore\\GenericADNotificationHandler.cs");
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					ADNotificationHandler.DebugTrace("{0}.Dispose() called", new object[]
					{
						base.GetType().Name
					});
					lock (this)
					{
						this.EnsureTimerDisposed();
						if (this.notifRequest != null)
						{
							ADNotificationHandler.DebugTrace("{0}.Dispose: UnregisterChangeNotification", new object[]
							{
								base.GetType().Name
							});
							try
							{
								ADNotificationAdapter.UnregisterChangeNotification(this.notifRequest);
							}
							catch (ADTransientException ex)
							{
								ADNotificationHandler.ErrorTrace("{0}.Dispose: {1}", new object[]
								{
									base.GetType().Name,
									ex
								});
							}
							this.notifRequest = null;
						}
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected void Register()
		{
			this.EnsureTimerDisposed();
			try
			{
				ADNotificationHandler.DebugTrace("GenericADNotificationHandler.Register()", new object[0]);
				this.notifRequest = ADNotificationAdapter.RegisterChangeNotification<T>(this.ConfigSession.GetOrgContainerId(), new ADNotificationCallback(this.InternalConfigChanged), this);
				base.FireConfigChangedEvent(null);
			}
			catch (ADTransientException ex)
			{
				TimeSpan adnotificationsRetryTime = Constants.ADNotificationsRetryTime;
				this.LogRegistrationError(adnotificationsRetryTime, ex);
				ADNotificationHandler.ErrorTrace("GenericADNotificationHandler.Register: {0}", new object[]
				{
					ex
				});
				this.registrationTimer = new Timer(new TimerCallback(this.RegistrationTimerExpired), this, adnotificationsRetryTime, TimeSpan.Zero);
			}
		}

		private void InternalConfigChanged(ADNotificationEventArgs args)
		{
			lock (this)
			{
				Exception ex = null;
				try
				{
					ADNotificationHandler.DebugTrace("GenericADNotificationHandler.InternalConfigChanged: {0}, id={1}, type={2}", new object[]
					{
						args.ChangeType,
						args.Id,
						args.Type
					});
					if (this.notifRequest != null)
					{
						base.FireConfigChangedEvent(args);
					}
				}
				catch (ADTransientException ex2)
				{
					ex = ex2;
				}
				catch (ADOperationException ex3)
				{
					ex = ex3;
				}
				catch (DataValidationException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					ADNotificationHandler.ErrorTrace("GenericADNotificationHandler.InternalConfigChanged: {1}", new object[]
					{
						base.GetType().Name,
						ex
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADNotificationProcessingError, null, new object[]
					{
						ex.Message
					});
				}
			}
		}

		private void RegistrationTimerExpired(object context)
		{
			lock (this)
			{
				if (this.registrationTimer != null)
				{
					this.Register();
				}
			}
		}

		private void EnsureTimerDisposed()
		{
			if (this.registrationTimer != null)
			{
				ADNotificationHandler.DebugTrace("{0}: disposing retry timer", new object[]
				{
					base.GetType().Name
				});
				this.registrationTimer.Dispose();
				this.registrationTimer = null;
			}
		}

		private Timer registrationTimer;

		private ADNotificationRequestCookie notifRequest;
	}
}

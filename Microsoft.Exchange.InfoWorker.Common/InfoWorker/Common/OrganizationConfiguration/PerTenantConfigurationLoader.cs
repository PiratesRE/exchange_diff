using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal abstract class PerTenantConfigurationLoader<T>
	{
		protected virtual bool RefreshOnChange
		{
			get
			{
				return false;
			}
		}

		public PerTenantConfigurationLoader(OrganizationId organizationId)
		{
			this.organizationId = organizationId;
		}

		public PerTenantConfigurationLoader(OrganizationId organizationId, TimeSpan timeoutInterval)
		{
			this.organizationId = organizationId;
			this.timeoutInterval = timeoutInterval;
		}

		public abstract void Initialize();

		protected T Data
		{
			get
			{
				if (this.HasExpired())
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), this.organizationId, null, false), 107, "Data", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\OrganizationConfiguration\\PerTenantConfigurationLoader.cs");
					this.InternalRead(tenantOrTopologyConfigurationSession, true);
					this.startTime = ExDateTime.Now;
				}
				return this.data;
			}
		}

		protected void Initialize(object notificationLock)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), this.organizationId, null, false), 132, "Initialize", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\OrganizationConfiguration\\PerTenantConfigurationLoader.cs");
			if (this.data == null || this.notificationCookie == null)
			{
				this.InternalRead(tenantOrTopologyConfigurationSession, false);
				this.startTime = ExDateTime.Now;
			}
			if (this.organizationId != OrganizationId.ForestWideOrgId || VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				return;
			}
			if (this.notificationCookie == null)
			{
				lock (notificationLock)
				{
					if (this.notificationCookie == null)
					{
						this.RegisterChangeNotification(tenantOrTopologyConfigurationSession);
					}
				}
			}
		}

		protected abstract T Read(IConfigurationSession session);

		protected abstract ADNotificationRequestCookie Register(IConfigurationSession session);

		protected void ChangeCallback(ADNotificationEventArgs args)
		{
			IConfigurationSession session = (IConfigurationSession)args.Context;
			this.adChangeCoalescer.Value.OnEvent(delegate
			{
				try
				{
					this.InternalRead(session, this.RefreshOnChange);
				}
				catch (DataSourceTransientException)
				{
				}
				catch (DataSourceOperationException)
				{
				}
				catch (DataValidationException)
				{
				}
			});
		}

		private void RegisterChangeNotification(IConfigurationSession session)
		{
			int retryCount = 3;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				this.notificationCookie = this.Register(session);
			}, retryCount);
			if (adoperationResult.Exception != null)
			{
				CachedOrganizationConfiguration.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_UnableToRegisterChangeNotification, adoperationResult.Exception.GetType().FullName, new object[]
				{
					typeof(T).Name,
					adoperationResult.Exception.GetType().FullName,
					adoperationResult.Exception.Message
				});
				if (!(adoperationResult.Exception is DataSourceTransientException))
				{
					throw adoperationResult.Exception;
				}
			}
		}

		protected void InternalRead(IConfigurationSession session, bool force = false)
		{
			T newData = default(T);
			OrganizationProperties organizationProperties;
			if (!OrganizationPropertyCache.TryGetOrganizationProperties(this.organizationId, out organizationProperties) || force || !organizationProperties.TryGetValue<T>(out newData))
			{
				int retryCount = 3;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					newData = this.Read(session);
				}, retryCount);
				if (adoperationResult.Exception != null)
				{
					CachedOrganizationConfiguration.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_UnableToReadConfigurationFromAD, adoperationResult.Exception.GetType().FullName, new object[]
					{
						typeof(T).Name,
						adoperationResult.Exception.GetType().FullName,
						adoperationResult.Exception.Message
					});
					throw adoperationResult.Exception;
				}
				if (adoperationResult.Succeeded && organizationProperties != null)
				{
					organizationProperties.SetValue<T>(newData);
				}
			}
			this.data = newData;
		}

		private bool HasExpired()
		{
			return this.timeoutInterval != TimeSpan.Zero && ExDateTime.Now - this.startTime > this.timeoutInterval;
		}

		private ADNotificationRequestCookie notificationCookie;

		private readonly TimeSpan timeoutInterval = TimeSpan.Zero;

		private ExDateTime startTime;

		protected OrganizationId organizationId;

		protected T data;

		private Lazy<PerTenantConfigurationLoader<T>.Coalescer> adChangeCoalescer = new Lazy<PerTenantConfigurationLoader<T>.Coalescer>(() => new PerTenantConfigurationLoader<T>.Coalescer(PerTenantConfigurationLoader<T>.adChangeCoalescingTime), LazyThreadSafetyMode.PublicationOnly);

		private static readonly TimeSpan adChangeCoalescingTime = new TimeSpan(0, 0, 5);

		private class Coalescer
		{
			public Coalescer(TimeSpan delay)
			{
				this.delay = delay;
			}

			private void TimerCallback(Action callback)
			{
				lock (this)
				{
					this.timer.Dispose();
					this.timer = null;
				}
				callback();
			}

			public void OnEvent(Action callback)
			{
				lock (this)
				{
					if (this.timer == null)
					{
						this.timer = new Timer(delegate(object o)
						{
							this.TimerCallback(callback);
						}, null, this.delay, new TimeSpan(-1L));
					}
					else
					{
						this.timer.Change(this.delay, new TimeSpan(-1L));
					}
				}
			}

			private Timer timer;

			private readonly TimeSpan delay;
		}
	}
}

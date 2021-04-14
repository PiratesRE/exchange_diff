using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal abstract class TenantConfigurationCacheableItem<TConfig> : TenantConfigurationCacheableItemBase where TConfig : ADConfigurationObject, new()
	{
		protected TenantConfigurationCacheableItem()
		{
		}

		protected TenantConfigurationCacheableItem(bool initialized)
		{
			this.initialized = initialized;
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		internal static void HandleChangeNotification(ADNotificationEventArgs args)
		{
			CacheNotificationArgs cacheNotificationArgs = (CacheNotificationArgs)args.Context;
			cacheNotificationArgs.CacheNotificationHandler(cacheNotificationArgs.OrganizationId);
		}

		public abstract void ReadData(IConfigurationSession session);

		public virtual void ReadData(IConfigurationSession session, object state)
		{
			throw new NotImplementedException();
		}

		public override bool InitializeWithoutRegistration(IConfigurationSession adSession, bool allowExceptions)
		{
			this.organizationId = adSession.SessionSettings.CurrentOrganizationId;
			return this.initialized = this.InternalRead(adSession, allowExceptions, null);
		}

		public override bool TryInitialize(OrganizationId organizationId, CacheNotificationHandler cacheNotificationHandler, object state)
		{
			IConfigurationSession configurationSession = this.Initialize(organizationId, false);
			if (configurationSession == null)
			{
				return false;
			}
			if (this.InternalRead(configurationSession, false, state) && this.RegisterChangeNotification(configurationSession, cacheNotificationHandler, false))
			{
				this.initialized = true;
				return true;
			}
			return false;
		}

		public override bool Initialize(OrganizationId organizationId, CacheNotificationHandler cacheNotificationHandler, object state)
		{
			IConfigurationSession configurationSession = this.Initialize(organizationId, true);
			if (configurationSession == null)
			{
				return false;
			}
			if (this.InternalRead(configurationSession, true, state) && this.RegisterChangeNotification(configurationSession, cacheNotificationHandler, true))
			{
				this.initialized = true;
				return true;
			}
			return false;
		}

		public override void UnregisterChangeNotification()
		{
			if (this.notificationCookie != null)
			{
				this.TryRunADOperation(delegate
				{
					ADNotificationAdapter.UnregisterChangeNotification(this.notificationCookie);
				}, false);
			}
		}

		protected void ThrowIfNotInitialized(object source)
		{
			if (!this.initialized)
			{
				throw new InvalidOperationException(source.ToString() + " is not initialized and cannot be used");
			}
		}

		protected virtual IConfigurationSession CreateSession(bool throwExceptions)
		{
			TConfig tconfig = Activator.CreateInstance<TConfig>();
			if (tconfig.IsShareable)
			{
				IConfigurationSession sharedConfigSession = null;
				this.TryRunADOperation(delegate
				{
					sharedConfigSession = SharedConfiguration.CreateScopedToSharedConfigADSession(this.organizationId);
				}, throwExceptions);
				return sharedConfigSession;
			}
			IConfigurationSession session = null;
			this.TryRunADOperation(delegate
			{
				session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 214, "CreateSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\TenantConfigurationCacheableItem.cs");
			}, throwExceptions);
			return session;
		}

		private IConfigurationSession Initialize(OrganizationId organizationId, bool throwExceptions)
		{
			this.organizationId = organizationId;
			return this.CreateSession(throwExceptions);
		}

		private bool RegisterChangeNotification(IConfigurationSession session, CacheNotificationHandler cacheNotificationHandler, bool throwExceptions)
		{
			if (!OrganizationId.ForestWideOrgId.Equals(this.organizationId))
			{
				return true;
			}
			try
			{
				this.notificationCookie = ADNotificationAdapter.RegisterChangeNotification<TConfig>(this.organizationId.ConfigurationUnit ?? session.GetOrgContainerId(), new ADNotificationCallback(TenantConfigurationCacheableItem<TConfig>.HandleChangeNotification), new CacheNotificationArgs(cacheNotificationHandler, this.organizationId));
			}
			catch (TransientException)
			{
				if (!throwExceptions)
				{
					return false;
				}
				throw;
			}
			catch (DataSourceOperationException)
			{
				if (!throwExceptions)
				{
					return false;
				}
				throw;
			}
			return true;
		}

		private bool InternalRead(IConfigurationSession session, bool throwExceptions, object state)
		{
			if (state != null)
			{
				return this.TryRunADOperation(delegate
				{
					this.ReadData(session, state);
				}, throwExceptions);
			}
			return this.TryRunADOperation(delegate
			{
				this.ReadData(session);
			}, throwExceptions);
		}

		private bool TryRunADOperation(ADOperation operation, bool throwExceptions)
		{
			int retryCount = 3;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				operation();
			}, retryCount);
			if (adoperationResult.Exception == null)
			{
				return adoperationResult.Succeeded;
			}
			if (!throwExceptions)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<Exception>((long)this.GetHashCode(), "Failed to run AD operation: {0}", adoperationResult.Exception);
				return false;
			}
			throw adoperationResult.Exception;
		}

		private OrganizationId organizationId;

		private ADNotificationRequestCookie notificationCookie;

		private bool initialized;
	}
}

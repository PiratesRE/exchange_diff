using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal class ConfigurationLoader<TCache, TBuilder> : ITransportComponent where TCache : class where TBuilder : ConfigurationLoader<TCache, TBuilder>.Builder, new()
	{
		public ConfigurationLoader(TimeSpan reloadInterval) : this(null, reloadInterval)
		{
		}

		public ConfigurationLoader(ConfigurationLoader<TCache, TBuilder>.ExternalConfigurationSetter externalConfigurationSetter, TimeSpan reloadInterval)
		{
			this.session = this.CreateSession();
			this.externalConfigurationSetter = externalConfigurationSetter;
			this.cookies = new List<ADNotificationRequestCookie>();
			if (reloadInterval > TimeSpan.Zero)
			{
				this.reloadInterval = reloadInterval;
				this.reloadTimer = new GuardedTimer(new TimerCallback(this.TimedReload), null, -1, this.reloadInterval.Milliseconds);
			}
		}

		protected virtual ITopologyConfigurationSession CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 152, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\ConfigurationLoader.cs");
		}

		public event ConfigurationUpdateHandler<TCache> Changed;

		public bool IsInitialized
		{
			get
			{
				return this.cache != null && this.cookies.Count > 0;
			}
		}

		public TCache Cache
		{
			get
			{
				return this.cache;
			}
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			if (this.cache is IDiagnosable)
			{
				return ((IDiagnosable)((object)this.cache)).GetDiagnosticInfo(parameters);
			}
			return null;
		}

		public void Load()
		{
			lock (this)
			{
				if (!this.IsInitialized)
				{
					TBuilder builder = Activator.CreateInstance<TBuilder>();
					if (this.externalConfigurationSetter != null)
					{
						this.externalConfigurationSetter(builder);
					}
					builder.SetLoader(this);
					builder.Register();
					this.Reload(null);
					if (this.reloadTimer != null)
					{
						this.reloadTimer.Change(this.reloadInterval, this.reloadInterval);
					}
				}
				if (!this.IsInitialized)
				{
					throw new TransportComponentLoadFailedException(Strings.ConfigurationLoaderFailed(typeof(TCache).Name), null);
				}
			}
		}

		public void Unload()
		{
			foreach (ADNotificationRequestCookie requestCookie in this.cookies)
			{
				ADNotificationAdapter.UnregisterChangeNotification(requestCookie, true);
			}
			if (this.reloadTimer != null)
			{
				this.reloadTimer.Dispose(true);
				this.reloadTimer = null;
			}
			this.cookies.Clear();
			this.cache = default(TCache);
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Reload(object sender, EventArgs e)
		{
			this.Reload(null);
		}

		private void TimedReload(object state)
		{
			this.Reload(true);
		}

		protected void Reload(ADNotificationEventArgs args)
		{
			this.Reload(false);
		}

		private void Reload(bool timerReload)
		{
			lock (this)
			{
				if (!timerReload || !(DateTime.UtcNow.Subtract(this.lastNotificationUpdate) < this.reloadInterval))
				{
					TBuilder tbuilder = this.LoadFromAD();
					if (tbuilder != null)
					{
						TCache tcache = tbuilder.BuildCache();
						if (tcache != null)
						{
							this.cache = tcache;
							ConfigurationUpdateHandler<TCache> changed = this.Changed;
							if (changed != null)
							{
								changed(tcache);
							}
							if (timerReload)
							{
								ConfigurationLoader<TCache, TBuilder>.Log.LogEvent(TransportEventLogConstants.Tuple_ConfigurationLoaderSuccessfulForcedUpdate, null, new object[]
								{
									typeof(TCache),
									this.lastDescription,
									tbuilder.Description
								});
							}
							else
							{
								this.lastNotificationUpdate = DateTime.UtcNow;
								this.lastDescription = tbuilder.Description;
								ConfigurationLoader<TCache, TBuilder>.Log.LogEvent(TransportEventLogConstants.Tuple_ConfigurationLoaderSuccessfulUpdate, null, new object[]
								{
									typeof(TCache)
								});
							}
						}
						else
						{
							ConfigurationLoader<TCache, TBuilder>.Log.LogEvent(TransportEventLogConstants.Tuple_ConfigurationLoaderExternalError, this.ToString(), new object[]
							{
								typeof(TCache),
								tbuilder.FailureMessage
							});
						}
					}
				}
			}
		}

		private TBuilder LoadFromAD()
		{
			TBuilder builder = Activator.CreateInstance<TBuilder>();
			if (this.externalConfigurationSetter != null)
			{
				this.externalConfigurationSetter(builder);
			}
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				builder.LoadData(this.session, QueryScope.SubTree);
			});
			if (!adoperationResult.Succeeded)
			{
				ConfigurationLoader<TCache, TBuilder>.Log.LogEvent(TransportEventLogConstants.Tuple_ConfigurationLoaderException, this.ToString(), new object[]
				{
					typeof(TCache),
					adoperationResult.Exception
				});
				return default(TBuilder);
			}
			return builder;
		}

		public const int DefaultRegisterNotificationRetryCount = 3;

		private static readonly ExEventLog Log = new ExEventLog(ExTraceGlobals.ConfigurationTracer.Category, TransportEventLog.GetEventSource());

		private readonly ITopologyConfigurationSession session;

		private TCache cache;

		private List<ADNotificationRequestCookie> cookies;

		private ConfigurationLoader<TCache, TBuilder>.ExternalConfigurationSetter externalConfigurationSetter;

		private GuardedTimer reloadTimer;

		private TimeSpan reloadInterval;

		private DateTime lastNotificationUpdate = DateTime.UtcNow;

		private string lastDescription = string.Empty;

		public delegate void ExternalConfigurationSetter(TBuilder builder);

		public abstract class Builder
		{
			public string Description
			{
				get
				{
					return this.description;
				}
				set
				{
					this.description = value;
				}
			}

			public LocalizedString FailureMessage
			{
				get
				{
					return this.failureMessage;
				}
				set
				{
					this.failureMessage = value;
				}
			}

			public void SetLoader(ConfigurationLoader<TCache, TBuilder> loader)
			{
				this.loader = loader;
			}

			public abstract void Register();

			public abstract void LoadData(ITopologyConfigurationSession session, QueryScope scope);

			public abstract TCache BuildCache();

			protected static ADObjectId GetFirstOrgContainerId()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 457, "GetFirstOrgContainerId", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Configuration\\ConfigurationLoader.cs");
				return tenantOrTopologyConfigurationSession.GetOrgContainerId();
			}

			protected void Register<T>() where T : ADConfigurationObject, new()
			{
				this.Register<T>(null);
			}

			protected void Register<T>(Func<ADObjectId> rootIdGetter) where T : ADConfigurationObject, new()
			{
				this.RegisterWithAD<T>(rootIdGetter);
			}

			private void RegisterWithAD<T>(Func<ADObjectId> rootIdGetter) where T : ADConfigurationObject, new()
			{
				ADNotificationRequestCookie item;
				ADOperationResult adoperationResult = this.TryRegisterChangeNotification<T>(rootIdGetter, out item);
				if (adoperationResult != ADOperationResult.Success)
				{
					ConfigurationLoader<TCache, TBuilder>.Log.LogEvent(TransportEventLogConstants.Tuple_ConfigurationLoaderException, this.ToString(), new object[]
					{
						typeof(TCache),
						adoperationResult.Exception
					});
					throw new TransportComponentLoadFailedException(Strings.ConfigurationLoaderFailed(typeof(TCache).Name), null);
				}
				this.loader.cookies.Add(item);
			}

			protected virtual ADOperationResult TryRegisterChangeNotification<TConfigObject>(Func<ADObjectId> rootIdGetter, out ADNotificationRequestCookie cookie) where TConfigObject : ADConfigurationObject, new()
			{
				cookie = null;
				return ADNotificationAdapter.TryRegisterChangeNotification<TConfigObject>(rootIdGetter, new ADNotificationCallback(this.Reload), null, 3, out cookie);
			}

			protected void Reload(ADNotificationEventArgs args)
			{
				this.loader.Reload(false);
			}

			private ConfigurationLoader<TCache, TBuilder> loader;

			private LocalizedString failureMessage = LocalizedString.Empty;

			private string description = string.Empty;
		}

		public abstract class SimpleBuilder<TConfigObject> : ConfigurationLoader<TCache, TBuilder>.Builder where TConfigObject : ADConfigurationObject, new()
		{
			public override void Register()
			{
				base.Register<TConfigObject>(() => this.RootId);
			}

			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope = QueryScope.SubTree)
			{
				this.configObjects = new List<TConfigObject>(session.FindPaged<TConfigObject>(this.RootId, scope, null, null, 0));
			}

			public override TCache BuildCache()
			{
				return this.BuildCache(this.configObjects);
			}

			protected ADObjectId RootId
			{
				get
				{
					return this.rootId;
				}
				set
				{
					this.rootId = value;
				}
			}

			protected abstract TCache BuildCache(List<TConfigObject> configObjects);

			private List<TConfigObject> configObjects;

			private ADObjectId rootId;
		}
	}
}

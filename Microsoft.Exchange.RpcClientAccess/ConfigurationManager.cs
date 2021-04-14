using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConfigurationManager : BaseObject
	{
		public ConfigurationManager()
		{
			ConfigurationManager.SyncInstance[] array = new ConfigurationManager.SyncInstance[2];
			array[0] = new ConfigurationManager.SyncInstance(this, ConfigurationSchema<ServiceConfiguration.Schema>.Instance, (ConfigurationPropertyBag propertyBag) => Configuration.ServiceConfiguration = new ServiceConfiguration(propertyBag));
			array[1] = new ConfigurationManager.SyncInstance(this, ConfigurationSchema<ProtocolLogConfiguration.Schema>.Instance, (ConfigurationPropertyBag propertyBag) => Configuration.ProtocolLogConfiguration = new ProtocolLogConfiguration(propertyBag));
			this.allInstances = array;
		}

		public event Action<Exception> ConfigurationLoadFailed;

		public event Action<Exception> AsyncUnhandledException;

		public bool HasConfigurationsThatFailToUpdate
		{
			get
			{
				foreach (ConfigurationManager.SyncInstance syncInstance in this.allInstances)
				{
					if (!syncInstance.LastUpdateSucceeded)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal static ConfigurationManager CreateForTest(out Dictionary<ConfigurationSchema, ConfigurationManager.ITestHook> testHooks)
		{
			ConfigurationManager configurationManager = new ConfigurationManager();
			testHooks = configurationManager.allInstances.ToDictionary((ConfigurationManager.SyncInstance syncInstance) => syncInstance.Schema, (ConfigurationManager.SyncInstance syncInstance) => syncInstance);
			return configurationManager;
		}

		public void LoadAndRegisterForNotifications()
		{
			if (!this.isInitialized)
			{
				this.isInitialized = true;
				this.ForEach(delegate(ConfigurationManager.SyncInstance x)
				{
					x.RegisterForNotifications();
				});
				this.ForceReload();
				return;
			}
			throw new InvalidOperationException("ConfigurationManager.LoadAndRegisterForNotifications() can only be called once");
		}

		internal void ForceReload()
		{
			this.ForEach(delegate(ConfigurationManager.SyncInstance x)
			{
				x.Load();
			});
		}

		protected override void InternalDispose()
		{
			foreach (Action action in this.cleanupActions)
			{
				action();
			}
			this.cleanupActions.Clear();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ConfigurationManager>(this);
		}

		private void ForEach(Action<ConfigurationManager.SyncInstance> action)
		{
			foreach (ConfigurationManager.SyncInstance obj in this.allInstances)
			{
				action(obj);
			}
		}

		private void OnAsyncUnhandledException(Exception ex)
		{
			Action<Exception> asyncUnhandledException = this.AsyncUnhandledException;
			if (asyncUnhandledException != null)
			{
				asyncUnhandledException(ex);
			}
		}

		private bool TryOnConfigurationLoadFailed(Exception ex)
		{
			Action<Exception> configurationLoadFailed = this.ConfigurationLoadFailed;
			if (configurationLoadFailed != null)
			{
				configurationLoadFailed(ex);
				return true;
			}
			return false;
		}

		private readonly ConfigurationManager.SyncInstance[] allInstances;

		private readonly List<Action> cleanupActions = new List<Action>();

		private bool isInitialized;

		internal interface ITestHook
		{
			Action SetOverride<TValue>(ConfigurationSchema.Property<TValue> property, TValue valueOverride);
		}

		private sealed class SyncInstance : ConfigurationManager.ITestHook
		{
			public SyncInstance(ConfigurationManager configurationManager, ConfigurationSchema schema, Func<ConfigurationPropertyBag, object> setter)
			{
				this.configurationManager = configurationManager;
				this.Schema = schema;
				this.setter = setter;
			}

			public bool LastUpdateSucceeded { get; private set; }

			public void Load()
			{
				this.Load(this.Schema.DataSources);
			}

			public void Load(IEnumerable<ConfigurationSchema.DataSource> dataSources)
			{
				this.ExecuteDataSourceOperation(dataSources, delegate(ConfigurationSchema.DataSource dataSource)
				{
					dataSource.Load(new ConfigurationSchema.ConfigurationUpdater(this.UpdateConfiguration), Configuration.EventLogger);
				});
			}

			public void RegisterForNotifications()
			{
				this.ExecuteDataSourceOperation(this.Schema.DataSources, delegate(ConfigurationSchema.DataSource dataSource)
				{
					Action action = dataSource.RegisterNotifications(this.GetNotificationCallback(dataSource));
					if (action != null)
					{
						this.configurationManager.cleanupActions.Add(action);
					}
				});
			}

			Action ConfigurationManager.ITestHook.SetOverride<TValue>(ConfigurationSchema.Property<TValue> property, TValue valueOverride)
			{
				object previousValueOverrideObject;
				bool wasOldOverrideReplaced;
				lock (this.updateLock)
				{
					wasOldOverrideReplaced = this.overrides.TryGetValue(property, out previousValueOverrideObject);
					this.overrides[property] = valueOverride;
				}
				this.Load();
				return delegate()
				{
					lock (this.updateLock)
					{
						if (wasOldOverrideReplaced)
						{
							this.overrides[property] = previousValueOverrideObject;
						}
						else
						{
							this.overrides.Remove(property);
						}
					}
					this.Load();
				};
			}

			private void ExecuteDataSourceOperation(IEnumerable<ConfigurationSchema.DataSource> dataSources, Action<ConfigurationSchema.DataSource> operationDelegate)
			{
				foreach (ConfigurationSchema.DataSource obj in dataSources)
				{
					try
					{
						operationDelegate(obj);
					}
					catch (ConfigurationSchema.LoadException ex)
					{
						if (!this.configurationManager.TryOnConfigurationLoadFailed(ex.InnerException))
						{
							throw;
						}
					}
				}
			}

			private Action GetNotificationCallback(ConfigurationSchema.DataSource dataSource)
			{
				return delegate()
				{
					ExWatson.SendReportOnUnhandledException(delegate()
					{
						this.Load(new ConfigurationSchema.DataSource[]
						{
							dataSource
						});
					}, delegate(object ex)
					{
						this.configurationManager.OnAsyncUnhandledException(ex as Exception);
						return true;
					});
				};
			}

			private void UpdateConfiguration(Action<ConfigurationPropertyBag> loadActionDelegate)
			{
				object obj = null;
				this.LastUpdateSucceeded = false;
				lock (this.updateLock)
				{
					ConfigurationPropertyBag configurationPropertyBag = new ConfigurationPropertyBag(this.propertyBag, this.overrides);
					loadActionDelegate(configurationPropertyBag);
					if (configurationPropertyBag.IsValid)
					{
						this.propertyBag = configurationPropertyBag;
						obj = this.setter(configurationPropertyBag);
					}
				}
				if (obj != null)
				{
					this.LastUpdateSucceeded = true;
					Configuration.InternalFireOnChanged(obj);
					return;
				}
				this.configurationManager.TryOnConfigurationLoadFailed(null);
			}

			public readonly ConfigurationSchema Schema;

			private readonly ConfigurationManager configurationManager;

			private readonly Func<ConfigurationPropertyBag, object> setter;

			private readonly Dictionary<ConfigurationSchema.Property, object> overrides = new Dictionary<ConfigurationSchema.Property, object>();

			private readonly object updateLock = new object();

			private ConfigurationPropertyBag propertyBag;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.RpcClientAccess.Messages;
using Microsoft.Win32;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ConfigurationSchema
	{
		public static TimeSpan RegistryNotificationPollPeriod
		{
			get
			{
				return ConfigurationSchema.registryNotificationPollPeriod;
			}
			internal set
			{
				ConfigurationSchema.registryNotificationPollPeriod = value;
			}
		}

		public abstract IEnumerable<ConfigurationSchema.DataSource> DataSources { get; }

		public abstract void LoadAll(ConfigurationSchema.ConfigurationUpdater configurationUpdater, ConfigurationSchema.EventLogger eventLogger);

		private static TimeSpan registryNotificationPollPeriod = TimeSpan.FromSeconds(3.0);

		internal delegate void ConfigurationUpdater(Action<ConfigurationPropertyBag> refreshDelegate);

		internal delegate bool TryConvert<TInput, TOutput>(TInput input, out TOutput output);

		internal delegate void EventLogger(ExEventLog.EventTuple tuple, params object[] args);

		public abstract class DataSource
		{
			protected DataSource(ICollection<ConfigurationSchema.DataSource> dataSourceRegistry)
			{
				dataSourceRegistry.Add(this);
			}

			protected IEnumerable<ConfigurationSchema.Property> Properties
			{
				get
				{
					return this.dataSourceProperties;
				}
			}

			internal void AddProperty(ConfigurationSchema.Property property)
			{
				this.dataSourceProperties.Add(property);
			}

			internal bool CanQueryData(object context)
			{
				return context != null;
			}

			internal abstract void Load(ConfigurationSchema.ConfigurationUpdater configurationUpdater, ConfigurationSchema.EventLogger eventLogger);

			internal virtual Action RegisterNotifications(Action notificationDelegate)
			{
				return null;
			}

			private readonly List<ConfigurationSchema.Property> dataSourceProperties = new List<ConfigurationSchema.Property>();
		}

		public abstract class Property
		{
			public static bool TryCast<TInput, TOutput>(TInput input, out TOutput output)
			{
				if (input is TOutput)
				{
					output = (TOutput)((object)input);
					return true;
				}
				output = default(TOutput);
				return false;
			}

			public static ConfigurationSchema.TryConvert<TInput, TOutput> TryConvertIf<TInput, TOutput>(Predicate<TInput> validationDelegate, Converter<TInput, TOutput> conversionDelegate)
			{
				return delegate(TInput input, out TOutput output)
				{
					if (validationDelegate(input))
					{
						output = conversionDelegate(input);
						return true;
					}
					output = default(TOutput);
					return false;
				};
			}

			public static ConfigurationSchema.TryConvert<TInput, TOutput> TryCastIf<TInput, TOutput>(Predicate<TInput> validationDelegate)
			{
				return delegate(TInput input, out TOutput output)
				{
					if (validationDelegate(input))
					{
						return ConfigurationSchema.Property.TryCast<TInput, TOutput>(input, out output);
					}
					output = default(TOutput);
					return false;
				};
			}

			public static bool Identical<T>(T input, out T output)
			{
				output = input;
				return true;
			}

			internal abstract void Load(ConfigurationPropertyBag configurationPropertyBag, object context, ConfigurationSchema.EventLogger eventLogger);
		}

		public abstract class Property<TValue> : ConfigurationSchema.Property
		{
			protected Property(ConfigurationSchema.DataSource dataSource, TValue defaultValue)
			{
				this.defaultValue = defaultValue;
				dataSource.AddProperty(this);
			}

			internal TValue DefaultValue
			{
				get
				{
					return this.defaultValue;
				}
			}

			internal static ConfigurationSchema.Property<TValue> Declare(ConfigurationSchema.DataSource constantDataSource, Func<TValue> valueDelegate)
			{
				if (!(constantDataSource is ConfigurationSchema.ConstantDataSource))
				{
					throw new ArgumentException("Should be Schema.ConstantDataSource", "constantDataSource");
				}
				return new ConfigurationSchema.DelegateProperty<TValue, ConfigurationSchema.ConstantDataSource>((ConfigurationSchema.ConstantDataSource)constantDataSource, (ConfigurationSchema.ConstantDataSource dataSource, object context) => valueDelegate(), valueDelegate());
			}

			internal static ConfigurationSchema.Property<TValue> Declare<TDataSource>(TDataSource dataSource, Func<TDataSource, object, TValue> valueDelegate, TValue defaultValue) where TDataSource : ConfigurationSchema.DataSource
			{
				return new ConfigurationSchema.DelegateProperty<TValue, TDataSource>(dataSource, valueDelegate, defaultValue);
			}

			internal static ConfigurationSchema.Property<TValue> Declare<TKey, TRawData>(ConfigurationSchema.DataSource<TKey, TRawData> dataSource, TKey dataKey, TValue defaultValue)
			{
				return ConfigurationSchema.Property<TValue>.Declare<TKey, TRawData, TValue>(dataSource, dataKey, new ConfigurationSchema.TryConvert<TRawData, TValue>(ConfigurationSchema.Property.TryCast<TRawData, TValue>), new ConfigurationSchema.TryConvert<TValue, TValue>(ConfigurationSchema.Property.Identical<TValue>), defaultValue);
			}

			internal static ConfigurationSchema.Property<TValue> Declare<TKey, TRawData, TIntermediate>(ConfigurationSchema.DataSource<TKey, TRawData> dataSource, TKey dataKey, ConfigurationSchema.TryConvert<TIntermediate, TValue> tryCovertDelegate, TValue defaultValue)
			{
				return ConfigurationSchema.Property<TValue>.Declare<TKey, TRawData, TIntermediate>(dataSource, dataKey, new ConfigurationSchema.TryConvert<TRawData, TIntermediate>(ConfigurationSchema.Property.TryCast<TRawData, TIntermediate>), tryCovertDelegate, defaultValue);
			}

			internal static ConfigurationSchema.Property<TValue> Declare<TKey, TRawData, TIntermediate>(ConfigurationSchema.DataSource<TKey, TRawData> dataSource, TKey dataKey, Predicate<TIntermediate> validationDelegate, Converter<TIntermediate, TValue> conversionDelegate, TValue defaultValue)
			{
				return ConfigurationSchema.Property<TValue>.Declare<TKey, TRawData, TIntermediate>(dataSource, dataKey, new ConfigurationSchema.TryConvert<TRawData, TIntermediate>(ConfigurationSchema.Property.TryCast<TRawData, TIntermediate>), ConfigurationSchema.Property.TryConvertIf<TIntermediate, TValue>(validationDelegate, conversionDelegate), defaultValue);
			}

			internal static ConfigurationSchema.Property<TValue> Declare<TKey, TRawData, TIntermediate>(ConfigurationSchema.DataSource<TKey, TRawData> dataSource, TKey dataKey, ConfigurationSchema.TryConvert<TRawData, TIntermediate> tryParseDelegate, ConfigurationSchema.TryConvert<TIntermediate, TValue> tryCovertDelegate, TValue defaultValue)
			{
				Util.ThrowOnNullArgument(dataSource, "dataSource");
				return new ConfigurationSchema.DataSourceProperty<TValue, TKey, TRawData, TIntermediate>(dataSource, dataKey, tryParseDelegate, tryCovertDelegate, defaultValue);
			}

			private readonly TValue defaultValue;
		}

		public class LoadException : Exception
		{
			public LoadException(Exception innerException) : base("Could not load configuration information.", innerException)
			{
			}

			public override string Message
			{
				get
				{
					return base.InnerException.Message;
				}
			}
		}

		internal abstract class DataSource<TKey, TRawData> : ConfigurationSchema.DataSource
		{
			protected DataSource(ICollection<ConfigurationSchema.DataSource> dataSourceRegistry) : base(dataSourceRegistry)
			{
			}

			internal abstract string GetDatumAdminDisplayName(TKey key);

			internal virtual string GetTypeDisplayName(Type rawValueType)
			{
				return rawValueType.Name;
			}

			internal abstract bool TryGetRawData(TKey key, object context, out TRawData rawData);

			protected void Load(ConfigurationSchema.ConfigurationUpdater configurationUpdater, object context, ConfigurationSchema.EventLogger eventLogger)
			{
				configurationUpdater(delegate(ConfigurationPropertyBag newConfiguration)
				{
					newConfiguration.Delete(this.Properties);
					foreach (ConfigurationSchema.Property property in this.Properties)
					{
						property.Load(newConfiguration, context, eventLogger);
					}
				});
			}
		}

		protected sealed class RegistryDataSource : ConfigurationSchema.DataSource<string, object>
		{
			internal RegistryDataSource(ICollection<ConfigurationSchema.DataSource> dataSourceRegistry, string registryKeyName) : base(dataSourceRegistry)
			{
				this.registryKeyName = registryKeyName;
			}

			internal override string GetDatumAdminDisplayName(string key)
			{
				return string.Format("{0}\\{1}\\@{2}", Registry.LocalMachine.Name, this.registryKeyName, key);
			}

			internal override bool TryGetRawData(string key, object context, out object rawData)
			{
				rawData = this.ExecuteRegistryOperation<object>(() => ((RegistryKey)context).GetValue(key, null));
				return rawData != null;
			}

			internal override string GetTypeDisplayName(Type rawValueType)
			{
				return ConfigurationSchema.RegistryDataSource.typeToRegistryType[rawValueType];
			}

			internal override void Load(ConfigurationSchema.ConfigurationUpdater configurationUpdater, ConfigurationSchema.EventLogger eventLogger)
			{
				using (RegistryKey registryKey = this.ExecuteRegistryOperation<RegistryKey>(() => Registry.LocalMachine.OpenSubKey(this.registryKeyName, false)))
				{
					base.Load(configurationUpdater, registryKey, eventLogger);
				}
			}

			internal override Action RegisterNotifications(Action notificationDelegate)
			{
				RegistryWatcher watcher = new RegistryWatcher(this.registryKeyName, false);
				lock (ConfigurationSchema.RegistryDataSource.watchers)
				{
					ConfigurationSchema.RegistryDataSource.watchers.Add(watcher, notificationDelegate);
				}
				ConfigurationSchema.RegistryDataSource.EnableDisableNotificationTimer();
				return delegate()
				{
					ConfigurationSchema.RegistryDataSource.RemoveWatcher(watcher);
				};
			}

			private static void EnableDisableNotificationTimer()
			{
				lock (ConfigurationSchema.RegistryDataSource.watchers)
				{
					ConfigurationSchema.RegistryDataSource.notificationTimer.Change((ConfigurationSchema.RegistryDataSource.watchers.Count > 0) ? ((long)ConfigurationSchema.RegistryNotificationPollPeriod.TotalMilliseconds) : -1L, -1L);
				}
			}

			private static void NotificationTimerCallback(object state)
			{
				try
				{
					KeyValuePair<RegistryWatcher, Action>[] array;
					lock (ConfigurationSchema.RegistryDataSource.watchers)
					{
						array = ConfigurationSchema.RegistryDataSource.watchers.ToArray<KeyValuePair<RegistryWatcher, Action>>();
					}
					foreach (KeyValuePair<RegistryWatcher, Action> keyValuePair in array)
					{
						if (keyValuePair.Key.IsChanged())
						{
							keyValuePair.Value();
						}
					}
				}
				finally
				{
					ConfigurationSchema.RegistryDataSource.EnableDisableNotificationTimer();
				}
			}

			private static void RemoveWatcher(RegistryWatcher watcher)
			{
				lock (ConfigurationSchema.RegistryDataSource.watchers)
				{
					ConfigurationSchema.RegistryDataSource.watchers.Remove(watcher);
				}
				ConfigurationSchema.RegistryDataSource.EnableDisableNotificationTimer();
			}

			private TResult ExecuteRegistryOperation<TResult>(Func<TResult> registryOperation)
			{
				TResult result;
				try
				{
					result = registryOperation();
				}
				catch (SecurityException innerException)
				{
					throw new ConfigurationSchema.LoadException(innerException);
				}
				catch (IOException innerException2)
				{
					throw new ConfigurationSchema.LoadException(innerException2);
				}
				catch (UnauthorizedAccessException innerException3)
				{
					throw new ConfigurationSchema.LoadException(innerException3);
				}
				return result;
			}

			private static readonly Dictionary<Type, string> typeToRegistryType = new Dictionary<Type, string>().AddPair(typeof(int), "REG_DWORD").AddPair(typeof(long), "REG_QWORD").AddPair(typeof(string), "REG_SZ").AddPair(typeof(string[]), "REG_MULTI_SZ").AddPair(typeof(byte[]), "REG_BINARY");

			private static readonly Dictionary<RegistryWatcher, Action> watchers = new Dictionary<RegistryWatcher, Action>();

			private static readonly Timer notificationTimer = new Timer(new TimerCallback(ConfigurationSchema.RegistryDataSource.NotificationTimerCallback), null, -1, -1);

			private readonly string registryKeyName;
		}

		protected sealed class DirectoryDataSource<TDirectoryObject> : ConfigurationSchema.DataSource<ADPropertyDefinition, object> where TDirectoryObject : ADConfigurationObject, new()
		{
			internal DirectoryDataSource(ICollection<ConfigurationSchema.DataSource> dataSourceRegistry, Func<ITopologyConfigurationSession, TDirectoryObject> adObjectReadDelegate, Func<ITopologyConfigurationSession, ADObjectId> notificationScopeDelegate) : base(dataSourceRegistry)
			{
				this.adObjectReadDelegate = adObjectReadDelegate;
				this.notificationScopeDelegate = notificationScopeDelegate;
			}

			internal ConfigurationSchema.Property<TValue> DeclareProperty<TValue>(ADPropertyDefinition dataKey)
			{
				return ConfigurationSchema.Property<TValue>.Declare<ADPropertyDefinition, object>(this, dataKey, (TValue)((object)dataKey.DefaultValue));
			}

			internal override string GetDatumAdminDisplayName(ADPropertyDefinition key)
			{
				if (this.adObjectId != null)
				{
					return string.Format("({0}), {1}", this.adObjectId.DistinguishedName, key.Name);
				}
				string format = "@(...)[objectCategory={0}], {1}";
				TDirectoryObject tdirectoryObject = Activator.CreateInstance<TDirectoryObject>();
				return string.Format(format, tdirectoryObject.MostDerivedObjectClass, key.Name);
			}

			internal override bool TryGetRawData(ADPropertyDefinition key, object context, out object rawData)
			{
				TDirectoryObject tdirectoryObject = (TDirectoryObject)((object)context);
				rawData = tdirectoryObject[key];
				return true;
			}

			internal override Action RegisterNotifications(Action notificationDelegate)
			{
				this.notificationDelegate = notificationDelegate;
				if (this.notificationScope == null)
				{
					this.notificationScope = this.GetNotificationScope();
				}
				if (this.notificationScope == null)
				{
					this.notificationScopeRetry = new Timer(delegate(object unused)
					{
						this.RetryNotificationScope();
					}, null, 0, ConfigurationSchema.DirectoryDataSource<TDirectoryObject>.NotificationScopeRetryPeriod.Milliseconds);
				}
				else
				{
					this.notificationCookie = ADNotificationAdapter.RegisterChangeNotification<TDirectoryObject>(this.notificationScope, delegate(ADNotificationEventArgs unusedNotificationInfo)
					{
						this.InternalOnNotify();
					});
				}
				return delegate()
				{
					this.UnregisterChangeNotificationInternal();
				};
			}

			private void InternalOnNotify()
			{
				try
				{
					if (Monitor.TryEnter(this.changeNotificationLock))
					{
						this.notificationDelegate();
					}
				}
				finally
				{
					if (Monitor.IsEntered(this.changeNotificationLock))
					{
						Monitor.Exit(this.changeNotificationLock);
					}
				}
			}

			private void RetryNotificationScope()
			{
				try
				{
					if (Monitor.TryEnter(this.notificationScopeLock))
					{
						if (this.notificationScopeRetry != null)
						{
							this.notificationScope = this.GetNotificationScope();
							if (this.notificationScope != null)
							{
								this.notificationCookie = ADNotificationAdapter.RegisterChangeNotification<TDirectoryObject>(this.notificationScope, delegate(ADNotificationEventArgs unusedNotificationInfo)
								{
									this.InternalOnNotify();
								});
								this.InternalOnNotify();
								this.notificationScopeRetry.Dispose();
								this.notificationScopeRetry = null;
							}
						}
					}
				}
				catch (ADTransientException)
				{
				}
				finally
				{
					if (Monitor.IsEntered(this.notificationScopeLock))
					{
						Monitor.Exit(this.notificationScopeLock);
					}
				}
			}

			private void UnregisterChangeNotificationInternal()
			{
				lock (this.notificationScopeLock)
				{
					if (this.notificationScopeRetry != null)
					{
						this.notificationScopeRetry.Dispose();
						this.notificationScopeRetry = null;
					}
					if (this.notificationCookie != null)
					{
						ADNotificationAdapter.UnregisterChangeNotification(this.notificationCookie);
					}
				}
			}

			private ADObjectId GetNotificationScope()
			{
				ADObjectId adobjectId = this.ExecuteADOperation<ADObjectId>(this.notificationScopeDelegate);
				if (adobjectId != null && adobjectId.ObjectGuid != Guid.Empty)
				{
					return adobjectId;
				}
				return null;
			}

			internal override void Load(ConfigurationSchema.ConfigurationUpdater configurationUpdater, ConfigurationSchema.EventLogger eventLogger)
			{
				base.Load(configurationUpdater, this.LoadDirectoryObject(), eventLogger);
			}

			private TDirectoryObject LoadDirectoryObject()
			{
				TDirectoryObject tdirectoryObject = this.ExecuteADOperation<TDirectoryObject>(this.adObjectReadDelegate);
				if (tdirectoryObject != null)
				{
					this.adObjectId = tdirectoryObject.Id;
				}
				return tdirectoryObject;
			}

			private TResult ExecuteADOperation<TResult>(Func<ITopologyConfigurationSession, TResult> adOperationDelegate)
			{
				TResult result = default(TResult);
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					ITopologyConfigurationSession arg = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 679, "ExecuteADOperation", "f:\\15.00.1497\\sources\\dev\\mapimt\\src\\Common\\ConfigurationSchema.cs");
					result = adOperationDelegate(arg);
				}, 1);
				if (adoperationResult != ADOperationResult.Success)
				{
					throw new ConfigurationSchema.LoadException(adoperationResult.Exception);
				}
				return result;
			}

			private static readonly TimeSpan NotificationScopeRetryPeriod = TimeSpan.FromMinutes(1.0);

			private readonly Func<ITopologyConfigurationSession, TDirectoryObject> adObjectReadDelegate;

			private readonly Func<ITopologyConfigurationSession, ADObjectId> notificationScopeDelegate;

			private ADObjectId adObjectId;

			private ADObjectId notificationScope;

			private Action notificationDelegate;

			private Timer notificationScopeRetry;

			private ADNotificationRequestCookie notificationCookie;

			private object notificationScopeLock = new object();

			private object changeNotificationLock = new object();
		}

		protected sealed class AppSettingsDataSource : ConfigurationSchema.DataSource<string, string>
		{
			public AppSettingsDataSource(ICollection<ConfigurationSchema.DataSource> dataSourceRegistry) : base(dataSourceRegistry)
			{
			}

			internal override bool TryGetRawData(string key, object context, out string rawData)
			{
				rawData = ((NameValueCollection)context)[key];
				return rawData != null;
			}

			internal override string GetDatumAdminDisplayName(string key)
			{
				return string.Format("app.config://configuration/{0}/add[@key='{1}']/@value", "appSettings", key);
			}

			internal override Action RegisterNotifications(Action notificationDelegate)
			{
				string appConfigFileName = Configuration.AppConfigFileName;
				FileSystemWatcherTimer watcher = new FileSystemWatcherTimer(appConfigFileName, notificationDelegate);
				return delegate()
				{
					watcher.Dispose();
				};
			}

			internal override void Load(ConfigurationSchema.ConfigurationUpdater configurationUpdater, ConfigurationSchema.EventLogger eventLogger)
			{
				NameValueCollection appSettings;
				try
				{
					ConfigurationManager.RefreshSection("appSettings");
					appSettings = ConfigurationManager.AppSettings;
				}
				catch (ConfigurationErrorsException innerException)
				{
					throw new ConfigurationSchema.LoadException(innerException);
				}
				base.Load(configurationUpdater, appSettings, eventLogger);
			}

			private const string SectionName = "appSettings";
		}

		protected sealed class ConstantDataSource : ConfigurationSchema.DataSource<ConfigurationSchema.ConstantDataSource, ConfigurationSchema.ConstantDataSource>
		{
			internal ConstantDataSource(ICollection<ConfigurationSchema.DataSource> dataSourceRegistry) : base(dataSourceRegistry)
			{
			}

			internal ConfigurationSchema.Property<TValue> Declare<TValue>(Func<TValue> valueDelegate)
			{
				return ConfigurationSchema.Property<TValue>.Declare(this, valueDelegate);
			}

			internal override string GetDatumAdminDisplayName(ConfigurationSchema.ConstantDataSource key)
			{
				return string.Format("constant:{0}", key);
			}

			internal override void Load(ConfigurationSchema.ConfigurationUpdater configurationUpdater, ConfigurationSchema.EventLogger eventLogger)
			{
				base.Load(configurationUpdater, null, eventLogger);
			}

			internal override bool TryGetRawData(ConfigurationSchema.ConstantDataSource key, object context, out ConfigurationSchema.ConstantDataSource rawData)
			{
				rawData = null;
				return true;
			}
		}

		protected class DelegateProperty<TValue, TDataSource> : ConfigurationSchema.Property<TValue> where TDataSource : ConfigurationSchema.DataSource
		{
			internal DelegateProperty(TDataSource dataSource, Func<TDataSource, object, TValue> valueDelegate, TValue defaultValue) : base(dataSource, defaultValue)
			{
				this.dataSource = dataSource;
				this.valueDelegate = valueDelegate;
			}

			internal override void Load(ConfigurationPropertyBag configurationPropertyBag, object context, ConfigurationSchema.EventLogger eventLogger)
			{
				configurationPropertyBag.Set<TValue>(this, this.valueDelegate(this.dataSource, context));
			}

			private readonly TDataSource dataSource;

			private readonly Func<TDataSource, object, TValue> valueDelegate;
		}

		protected class DataSourceProperty<TValue, TKey, TRawData, TIntermediate> : ConfigurationSchema.Property<TValue>
		{
			internal DataSourceProperty(ConfigurationSchema.DataSource<TKey, TRawData> dataSource, TKey dataKey, ConfigurationSchema.TryConvert<TRawData, TIntermediate> tryParseDelegate, ConfigurationSchema.TryConvert<TIntermediate, TValue> tryConvertDelegate, TValue defaultValue) : base(dataSource, defaultValue)
			{
				this.dataKey = dataKey;
				this.dataSource = dataSource;
				this.tryParseDelegate = tryParseDelegate;
				this.tryCovertDelegate = tryConvertDelegate;
			}

			internal override void Load(ConfigurationPropertyBag configurationPropertyBag, object context, ConfigurationSchema.EventLogger eventLogger)
			{
				TRawData trawData;
				if (!this.dataSource.CanQueryData(context) || !this.dataSource.TryGetRawData(this.dataKey, context, out trawData))
				{
					return;
				}
				TIntermediate input;
				if (!this.tryParseDelegate(trawData, out input))
				{
					eventLogger(RpcClientAccessServiceEventLogConstants.Tuple_ConfigurationInvalidValueType, new object[]
					{
						this.dataSource.GetDatumAdminDisplayName(this.dataKey),
						trawData,
						this.dataSource.GetTypeDisplayName(typeof(TIntermediate))
					});
					configurationPropertyBag.MarkInvalid();
					return;
				}
				TValue value;
				if (this.tryCovertDelegate(input, out value))
				{
					configurationPropertyBag.Set<TValue>(this, value);
					return;
				}
				eventLogger(RpcClientAccessServiceEventLogConstants.Tuple_ConfigurationInvalidValue, new object[]
				{
					this.dataSource.GetDatumAdminDisplayName(this.dataKey),
					trawData
				});
				configurationPropertyBag.MarkInvalid();
			}

			[Conditional("DEBUG")]
			private void DebugCheckConversions(TKey key)
			{
				this.dataSource.GetDatumAdminDisplayName(key);
			}

			[Conditional("DEBUG")]
			private void DebugCheckConversions(Type rawDataType)
			{
				this.dataSource.GetTypeDisplayName(rawDataType);
			}

			private readonly TKey dataKey;

			private readonly ConfigurationSchema.DataSource<TKey, TRawData> dataSource;

			private readonly ConfigurationSchema.TryConvert<TRawData, TIntermediate> tryParseDelegate;

			private readonly ConfigurationSchema.TryConvert<TIntermediate, TValue> tryCovertDelegate;
		}
	}
}

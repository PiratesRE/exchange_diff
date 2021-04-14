using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal sealed class TransportAppConfig : ITransportAppConfig
	{
		private TransportAppConfig()
		{
		}

		public TransportAppConfig.ResourceManagerConfig ResourceManager
		{
			get
			{
				return this.resourceManagerConfig;
			}
		}

		public TransportAppConfig.JetDatabaseConfig JetDatabase
		{
			get
			{
				return this.jetDatabaseConfig;
			}
		}

		public TransportAppConfig.DumpsterConfig Dumpster
		{
			get
			{
				return this.dumpsterConfig;
			}
		}

		public TransportAppConfig.ShadowRedundancyConfig ShadowRedundancy
		{
			get
			{
				return this.shadowRedundancyConfig;
			}
		}

		public TransportAppConfig.RemoteDeliveryConfig RemoteDelivery
		{
			get
			{
				return this.remoteDeliveryConfig;
			}
		}

		public TransportAppConfig.MapiSubmissionConfig MapiSubmission
		{
			get
			{
				return this.mapiSubmissionConfig;
			}
		}

		public TransportAppConfig.ResolverConfig Resolver
		{
			get
			{
				return this.resolverConfig;
			}
		}

		public TransportAppConfig.RoutingConfig Routing
		{
			get
			{
				return this.routingConfig;
			}
		}

		public TransportAppConfig.ContentConversionConfig ContentConversion
		{
			get
			{
				return this.contentConversionConfig;
			}
		}

		public TransportAppConfig.IPFilteringDatabaseConfig IPFilteringDatabase
		{
			get
			{
				return this.ipFilteringDatabaseConfig;
			}
		}

		public TransportAppConfig.IMessageResubmissionConfig MessageResubmission
		{
			get
			{
				return this.messageResubmissionConfig;
			}
		}

		public TransportAppConfig.QueueDatabaseConfig QueueDatabase
		{
			get
			{
				return this.queueDatabaseConfig;
			}
		}

		public TransportAppConfig.WorkerProcessConfig WorkerProcess
		{
			get
			{
				return this.workerProcessConfig;
			}
		}

		public TransportAppConfig.LatencyTrackerConfig LatencyTracker
		{
			get
			{
				return this.latencyTrackerConfig;
			}
		}

		public TransportAppConfig.RecipientValidatorConfig RecipientValidtor
		{
			get
			{
				return this.recipientValidatorConfig;
			}
		}

		public TransportAppConfig.PerTenantCacheConfig PerTenantCache
		{
			get
			{
				return this.perTenantCacheConfig;
			}
		}

		public TransportAppConfig.MessageThrottlingConfiguration MessageThrottlingConfig
		{
			get
			{
				return this.messageThrottlingConfig;
			}
		}

		public TransportAppConfig.SMTPOutConnectionCacheConfig ConnectionCacheConfig
		{
			get
			{
				return this.connectionCacheConfig;
			}
		}

		public TransportAppConfig.IsMemberOfResolverConfiguration TransportIsMemberOfResolverConfig
		{
			get
			{
				return this.transportIsMemberOfResolverConfig;
			}
		}

		public TransportAppConfig.IsMemberOfResolverConfiguration MailboxRulesIsMemberOfResolverConfig
		{
			get
			{
				return this.mailboxRulesIsMemberOfResolverConfig;
			}
		}

		public TransportAppConfig.SmtpAvailabilityConfig SmtpAvailabilityConfiguration
		{
			get
			{
				return this.smtpAvailabilityConfig;
			}
		}

		public TransportAppConfig.SmtpDataConfig SmtpDataConfiguration
		{
			get
			{
				return this.smtpDataConfig;
			}
		}

		public TransportAppConfig.SmtpMailCommandConfig SmtpMailCommandConfiguration
		{
			get
			{
				return this.smtpMailCommandConfig;
			}
		}

		public TransportAppConfig.MessageContextBlobConfig MessageContextBlobConfiguration
		{
			get
			{
				return this.messageContextBlobConfig;
			}
		}

		public TransportAppConfig.SmtpReceiveConfig SmtpReceiveConfiguration
		{
			get
			{
				return this.smtpReceiveConfig;
			}
		}

		public TransportAppConfig.SmtpSendConfig SmtpSendConfiguration
		{
			get
			{
				return this.smtpSendConfig;
			}
		}

		public TransportAppConfig.SmtpProxyConfig SmtpProxyConfiguration
		{
			get
			{
				return this.smtpProxyConfig;
			}
		}

		public TransportAppConfig.SmtpInboundProxyConfig SmtpInboundProxyConfiguration
		{
			get
			{
				return this.smtpInboundProxyConfig;
			}
		}

		public TransportAppConfig.SmtpOutboundProxyConfig SmtpOutboundProxyConfiguration
		{
			get
			{
				return this.smtpOutboundProxyConfig;
			}
		}

		public TransportAppConfig.DeliveryQueuePrioritizationConfig DeliveryQueuePrioritizationConfiguration
		{
			get
			{
				return this.deliveryQueuePrioritizationConfig;
			}
		}

		public TransportAppConfig.QueueConfig QueueConfiguration
		{
			get
			{
				return this.queueConfig;
			}
		}

		public TransportAppConfig.DeliveryFailureConfig DeliveryFailureConfiguration
		{
			get
			{
				return this.deliveryFailureConfig;
			}
		}

		public TransportAppConfig.SecureMailConfig SecureMail
		{
			get
			{
				return this.secureMailConfig;
			}
		}

		public TransportAppConfig.LoggingConfig Logging
		{
			get
			{
				return this.loggingConfig;
			}
		}

		public TransportAppConfig.FlowControlLogConfig FlowControlLog
		{
			get
			{
				return this.flowControlLogConfig;
			}
		}

		public TransportAppConfig.ConditionalThrottlingConfig ThrottlingConfig
		{
			get
			{
				return this.throttlingConfig;
			}
		}

		public TransportAppConfig.TransportRulesConfig TransportRuleConfig
		{
			get
			{
				return this.transportRulesConfig;
			}
		}

		public TransportAppConfig.PoisonMessageConfig PoisonMessage
		{
			get
			{
				return this.poisonMessageConfig;
			}
		}

		public TransportAppConfig.SmtpMessageThrottlingAgentConfig SmtpMessageThrottlingConfig
		{
			get
			{
				return this.smtpMessageThrottlingAgentConfig;
			}
		}

		public TransportAppConfig.StateManagementConfig StateManagement
		{
			get
			{
				return this.stateManagementConfig;
			}
		}

		public TransportAppConfig.BootLoaderConfig BootLoader
		{
			get
			{
				return this.bootLoaderConfig;
			}
		}

		public TransportAppConfig.ProcessingQuotaConfig ProcessingQuota
		{
			get
			{
				return this.processingQuotaConfig;
			}
		}

		public TransportAppConfig.ADPollingConfig ADPolling
		{
			get
			{
				return this.adPollingConfig;
			}
		}

		public static TransportAppConfig Load()
		{
			try
			{
				if (TransportAppConfig.instance == null)
				{
					lock (TransportAppConfig.initializationLock)
					{
						if (TransportAppConfig.instance == null)
						{
							TransportAppConfig transportAppConfig = new TransportAppConfig();
							transportAppConfig.resourceManagerConfig = TransportAppConfig.ResourceManagerConfig.Load();
							transportAppConfig.jetDatabaseConfig = TransportAppConfig.JetDatabaseConfig.Load();
							transportAppConfig.dumpsterConfig = TransportAppConfig.DumpsterConfig.Load();
							transportAppConfig.messageResubmissionConfig = TransportAppConfig.MessageResubmissionConfig.Load();
							transportAppConfig.shadowRedundancyConfig = TransportAppConfig.ShadowRedundancyConfig.Load();
							transportAppConfig.remoteDeliveryConfig = TransportAppConfig.RemoteDeliveryConfig.Load();
							transportAppConfig.mapiSubmissionConfig = TransportAppConfig.MapiSubmissionConfig.Load();
							transportAppConfig.resolverConfig = TransportAppConfig.ResolverConfig.Load();
							transportAppConfig.routingConfig = TransportAppConfig.RoutingConfig.Load();
							transportAppConfig.contentConversionConfig = TransportAppConfig.ContentConversionConfig.Load();
							transportAppConfig.ipFilteringDatabaseConfig = TransportAppConfig.IPFilteringDatabaseConfig.Load();
							transportAppConfig.queueDatabaseConfig = TransportAppConfig.QueueDatabaseConfig.Load();
							transportAppConfig.workerProcessConfig = TransportAppConfig.WorkerProcessConfig.Load();
							transportAppConfig.latencyTrackerConfig = TransportAppConfig.LatencyTrackerConfig.Load();
							transportAppConfig.recipientValidatorConfig = TransportAppConfig.RecipientValidatorConfig.Load();
							transportAppConfig.perTenantCacheConfig = TransportAppConfig.PerTenantCacheConfig.Load();
							transportAppConfig.messageThrottlingConfig = TransportAppConfig.MessageThrottlingConfiguration.Load();
							transportAppConfig.connectionCacheConfig = TransportAppConfig.SMTPOutConnectionCacheConfig.Load();
							transportAppConfig.transportIsMemberOfResolverConfig = TransportAppConfig.IsMemberOfResolverConfiguration.Load("Transport");
							transportAppConfig.mailboxRulesIsMemberOfResolverConfig = TransportAppConfig.IsMemberOfResolverConfiguration.Load("MailboxRules");
							transportAppConfig.smtpAvailabilityConfig = TransportAppConfig.SmtpAvailabilityConfig.Load();
							transportAppConfig.smtpDataConfig = TransportAppConfig.SmtpDataConfig.Load();
							transportAppConfig.smtpMailCommandConfig = TransportAppConfig.SmtpMailCommandConfig.Load();
							transportAppConfig.messageContextBlobConfig = TransportAppConfig.MessageContextBlobConfig.Load();
							transportAppConfig.smtpReceiveConfig = TransportAppConfig.SmtpReceiveConfig.Load();
							transportAppConfig.smtpSendConfig = TransportAppConfig.SmtpSendConfig.Load();
							transportAppConfig.smtpProxyConfig = TransportAppConfig.SmtpProxyConfig.Load();
							transportAppConfig.smtpInboundProxyConfig = TransportAppConfig.SmtpInboundProxyConfig.Load();
							transportAppConfig.smtpOutboundProxyConfig = TransportAppConfig.SmtpOutboundProxyConfig.Load();
							transportAppConfig.deliveryQueuePrioritizationConfig = TransportAppConfig.DeliveryQueuePrioritizationConfig.Load();
							transportAppConfig.queueConfig = TransportAppConfig.QueueConfig.Load();
							transportAppConfig.deliveryFailureConfig = TransportAppConfig.DeliveryFailureConfig.Load();
							transportAppConfig.secureMailConfig = TransportAppConfig.SecureMailConfig.Load();
							transportAppConfig.loggingConfig = TransportAppConfig.LoggingConfig.Load();
							transportAppConfig.flowControlLogConfig = TransportAppConfig.FlowControlLogConfig.Load();
							transportAppConfig.transportRulesConfig = TransportAppConfig.TransportRulesConfig.Load();
							transportAppConfig.throttlingConfig = TransportAppConfig.ConditionalThrottlingConfig.Load();
							transportAppConfig.poisonMessageConfig = TransportAppConfig.PoisonMessageConfig.Load();
							transportAppConfig.smtpMessageThrottlingAgentConfig = TransportAppConfig.SmtpMessageThrottlingAgentConfig.Load();
							transportAppConfig.stateManagementConfig = TransportAppConfig.StateManagementConfig.Load();
							transportAppConfig.bootLoaderConfig = TransportAppConfig.BootLoaderConfig.Load();
							transportAppConfig.processingQuotaConfig = TransportAppConfig.ProcessingQuotaConfig.Load();
							transportAppConfig.adPollingConfig = TransportAppConfig.ADPollingConfig.Load();
							Thread.MemoryBarrier();
							TransportAppConfig.instance = transportAppConfig;
						}
					}
				}
			}
			catch (ConfigurationErrorsException ex)
			{
				Process currentProcess = Process.GetCurrentProcess();
				TransportAppConfig.Log.LogEvent(TransportEventLogConstants.Tuple_AppConfigLoadFailed, null, new object[]
				{
					currentProcess.ProcessName,
					currentProcess.Id,
					ex.ToString()
				});
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, ex.ToString(), ResultSeverityLevel.Warning, false);
				throw;
			}
			return TransportAppConfig.instance;
		}

		public static XElement GetDiagnosticInfoForType(object config)
		{
			if (config == null)
			{
				return null;
			}
			Type type = config.GetType();
			return new XElement(type.Name, (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
			select new XElement(property.Name, property.GetValue(config, null))).ToArray<XElement>());
		}

		public static T GetConfigValue<T>(string label, T min, T max, T defaultValue, TransportAppConfig.TryParse<T> tryParse) where T : IComparable<T>
		{
			string value = ConfigurationManager.AppSettings[label];
			T result;
			TransportAppConfig.TryParseConfigValue<T>(label, value, min, max, defaultValue, tryParse, out result);
			return result;
		}

		public static T GetConfigValue<T>(string label, T defaultValue, TransportAppConfig.TryParse<T> tryParse)
		{
			string value = ConfigurationManager.AppSettings[label];
			T result;
			TransportAppConfig.TryParseConfigValue<T>(value, defaultValue, tryParse, out result);
			return result;
		}

		public static List<T> GetConfigList<T>(string label, char separator, TransportAppConfig.TryParse<T> tryParse)
		{
			string configValuesString = ConfigurationManager.AppSettings[label];
			return TransportAppConfig.GetConfigListFromValue<T>(configValuesString, separator, tryParse);
		}

		public static List<T> GetConfigListFromValue<T>(string configValuesString, char separator, TransportAppConfig.TryParse<T> tryParse)
		{
			List<T> list = new List<T>();
			if (!string.IsNullOrEmpty(configValuesString))
			{
				string[] array = configValuesString.Split(new char[]
				{
					separator
				});
				foreach (string value in array)
				{
					T item;
					if (TransportAppConfig.TryParseConfigValue<T>(value, default(T), tryParse, out item))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public static ByteQuantifiedSize GetConfigByteQuantifiedSize(string label, ByteQuantifiedSize min, ByteQuantifiedSize max, ByteQuantifiedSize defaultValue)
		{
			return TransportAppConfig.GetConfigValue<ByteQuantifiedSize>(label, min, max, defaultValue, new TransportAppConfig.TryParse<ByteQuantifiedSize>(ByteQuantifiedSize.TryParse));
		}

		public static int GetConfigInt(string label, int min, int max, int defaultValue)
		{
			return TransportAppConfig.GetConfigValue<int>(label, min, max, defaultValue, new TransportAppConfig.TryParse<int>(int.TryParse));
		}

		public static List<int> GetConfigIntList(string label, int min, int max, int defaultValue, char separator)
		{
			List<int> configList = TransportAppConfig.GetConfigList<int>(label, separator, new TransportAppConfig.TryParse<int>(int.TryParse));
			for (int i = 0; i < configList.Count; i++)
			{
				if (configList[i] < min || configList[i] > max)
				{
					configList[i] = defaultValue;
				}
			}
			return configList;
		}

		public static double GetConfigDouble(string label, double min, double max, double defaultValue)
		{
			return TransportAppConfig.GetConfigValue<double>(label, min, max, defaultValue, new TransportAppConfig.TryParse<double>(double.TryParse));
		}

		public static TimeSpan GetConfigTimeSpan(string label, TimeSpan min, TimeSpan max, TimeSpan defaultValue)
		{
			return TransportAppConfig.GetConfigValue<TimeSpan>(label, min, max, defaultValue, new TransportAppConfig.TryParse<TimeSpan>(TimeSpan.TryParse));
		}

		public static bool GetConfigBool(string label, bool defaultValue)
		{
			return TransportAppConfig.GetConfigValue<bool>(label, defaultValue, new TransportAppConfig.TryParse<bool>(bool.TryParse));
		}

		public static bool? GetConfigNullableBool(string label)
		{
			return TransportAppConfig.GetConfigValue<bool?>(label, null, delegate(string s, out bool? parsed)
			{
				bool value = false;
				if (!string.IsNullOrEmpty(s) && bool.TryParse(s, out value))
				{
					parsed = new bool?(value);
					return true;
				}
				parsed = null;
				return false;
			});
		}

		public static TimeSpan? GetConfigNullableTimeSpan(string label, TimeSpan min, TimeSpan max)
		{
			return TransportAppConfig.GetConfigValue<TimeSpan?>(label, null, delegate(string s, out TimeSpan? parsed)
			{
				TimeSpan timeSpan;
				if (!string.IsNullOrEmpty(s) && TimeSpan.TryParse(s, out timeSpan) && timeSpan <= max && timeSpan >= min)
				{
					parsed = new TimeSpan?(timeSpan);
					return true;
				}
				parsed = null;
				return false;
			});
		}

		public static T GetConfigEnum<T>(string label, T defaultValue) where T : struct
		{
			return TransportAppConfig.GetConfigEnum<T>(label, defaultValue, EnumParseOptions.IgnoreCase);
		}

		public static T GetConfigEnum<T>(string label, T defaultValue, EnumParseOptions options) where T : struct
		{
			return TransportAppConfig.GetConfigValue<T>(label, defaultValue, delegate(string s, out T parsed)
			{
				return EnumValidator.TryParse<T>(s, options, out parsed);
			});
		}

		internal static string GetConfigString(string label, string defaultValue)
		{
			return ConfigurationManager.AppSettings[label] ?? defaultValue;
		}

		private static List<T> GetConfigList<T>(TransportAppConfig.ConfigurationList configurationList, TransportAppConfig.TryParse<T> tryParse)
		{
			List<T> list = new List<T>(configurationList.Count);
			for (int i = 0; i < configurationList.Count; i++)
			{
				T item;
				if (tryParse(configurationList[i].Value, out item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("TransportAppConfig");
			foreach (PropertyInfo propertyInfo in base.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty))
			{
				XElement xelement2 = new XElement(propertyInfo.Name);
				xelement.Add(xelement2);
				object value = propertyInfo.GetValue(this, null);
				if (value != null)
				{
					foreach (PropertyInfo propertyInfo2 in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty))
					{
						xelement2.SetAttributeValue(propertyInfo2.Name, propertyInfo2.GetValue(value, null));
					}
				}
			}
			return xelement;
		}

		private static bool TryParseConfigValue<T>(string label, string value, T min, T max, T defaultValue, TransportAppConfig.TryParse<T> tryParse, out T configValue) where T : IComparable<T>
		{
			if (min != null && max != null && min.CompareTo(max) > 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Minimum must be smaller than or equal to Maximum (Config='{0}', Min='{1}', Max='{2}', Default='{3}').", new object[]
				{
					label,
					min,
					max,
					defaultValue
				}));
			}
			if (TransportAppConfig.TryParseConfigValue<T>(value, defaultValue, tryParse, out configValue) && (min == null || configValue.CompareTo(min) >= 0) && (max == null || configValue.CompareTo(max) <= 0))
			{
				return true;
			}
			configValue = defaultValue;
			return false;
		}

		private static bool TryParseConfigValue<T>(string value, T defaultValue, TransportAppConfig.TryParse<T> tryParse, out T configValue)
		{
			if (!string.IsNullOrEmpty(value) && tryParse(value, out configValue))
			{
				return true;
			}
			configValue = defaultValue;
			return false;
		}

		private static readonly ExEventLog Log = new ExEventLog(ExTraceGlobals.ConfigurationTracer.Category, TransportEventLog.GetEventSource());

		private static object initializationLock = new object();

		private static TransportAppConfig instance;

		private TransportAppConfig.ResourceManagerConfig resourceManagerConfig;

		private TransportAppConfig.JetDatabaseConfig jetDatabaseConfig;

		private TransportAppConfig.DumpsterConfig dumpsterConfig;

		private TransportAppConfig.ShadowRedundancyConfig shadowRedundancyConfig;

		private TransportAppConfig.RemoteDeliveryConfig remoteDeliveryConfig;

		private TransportAppConfig.MapiSubmissionConfig mapiSubmissionConfig;

		private TransportAppConfig.ResolverConfig resolverConfig;

		private TransportAppConfig.RoutingConfig routingConfig;

		private TransportAppConfig.ContentConversionConfig contentConversionConfig;

		private TransportAppConfig.IPFilteringDatabaseConfig ipFilteringDatabaseConfig;

		private TransportAppConfig.IMessageResubmissionConfig messageResubmissionConfig;

		private TransportAppConfig.QueueDatabaseConfig queueDatabaseConfig;

		private TransportAppConfig.WorkerProcessConfig workerProcessConfig;

		private TransportAppConfig.LatencyTrackerConfig latencyTrackerConfig;

		private TransportAppConfig.RecipientValidatorConfig recipientValidatorConfig;

		private TransportAppConfig.PerTenantCacheConfig perTenantCacheConfig;

		private TransportAppConfig.MessageThrottlingConfiguration messageThrottlingConfig;

		private TransportAppConfig.SMTPOutConnectionCacheConfig connectionCacheConfig;

		private TransportAppConfig.IsMemberOfResolverConfiguration transportIsMemberOfResolverConfig;

		private TransportAppConfig.IsMemberOfResolverConfiguration mailboxRulesIsMemberOfResolverConfig;

		private TransportAppConfig.SmtpAvailabilityConfig smtpAvailabilityConfig;

		private TransportAppConfig.SmtpDataConfig smtpDataConfig;

		private TransportAppConfig.SmtpMailCommandConfig smtpMailCommandConfig;

		private TransportAppConfig.MessageContextBlobConfig messageContextBlobConfig;

		private TransportAppConfig.SmtpReceiveConfig smtpReceiveConfig;

		private TransportAppConfig.SmtpSendConfig smtpSendConfig;

		private TransportAppConfig.SmtpProxyConfig smtpProxyConfig;

		private TransportAppConfig.SmtpInboundProxyConfig smtpInboundProxyConfig;

		private TransportAppConfig.SmtpOutboundProxyConfig smtpOutboundProxyConfig;

		private TransportAppConfig.DeliveryFailureConfig deliveryFailureConfig;

		private TransportAppConfig.DeliveryQueuePrioritizationConfig deliveryQueuePrioritizationConfig;

		private TransportAppConfig.SecureMailConfig secureMailConfig;

		private TransportAppConfig.QueueConfig queueConfig;

		private TransportAppConfig.LoggingConfig loggingConfig;

		private TransportAppConfig.FlowControlLogConfig flowControlLogConfig;

		private TransportAppConfig.ConditionalThrottlingConfig throttlingConfig;

		private TransportAppConfig.TransportRulesConfig transportRulesConfig;

		private TransportAppConfig.PoisonMessageConfig poisonMessageConfig;

		private TransportAppConfig.SmtpMessageThrottlingAgentConfig smtpMessageThrottlingAgentConfig;

		private TransportAppConfig.StateManagementConfig stateManagementConfig;

		private TransportAppConfig.BootLoaderConfig bootLoaderConfig;

		private TransportAppConfig.ProcessingQuotaConfig processingQuotaConfig;

		private TransportAppConfig.ADPollingConfig adPollingConfig;

		public delegate bool TryParse<T>(string config, out T parsedConfig);

		public interface IMessageResubmissionConfig
		{
			bool MessageResubmissionEnabled { get; }

			TimeSpan ResubmissionInterval { get; }

			TimeSpan ResubmissionInitialDelay { get; }

			int ResubmissionPageSize { get; }

			int MaxOutstandingResubmissionMessages { get; }

			int MaxResubmissionRequests { get; }

			int MaxRecentResubmissionRequests { get; }

			TimeSpan RecentResubmitRequestPeriod { get; }

			TimeSpan ResubmitRequestExpiryPeriod { get; }

			TimeSpan TestResubmitRequestExpiryPeriod { get; }
		}

		public class ConfigurationListItem : ConfigurationElement
		{
			[ConfigurationProperty("value", IsKey = true, IsRequired = true)]
			public string Value
			{
				get
				{
					return (string)base["value"];
				}
				internal set
				{
					base["value"] = value;
				}
			}
		}

		public class ConfigurationList : ConfigurationElementCollection
		{
			public ConfigurationList()
			{
			}

			internal ConfigurationList(List<string> elements)
			{
				foreach (string value in elements)
				{
					this.BaseAdd(new TransportAppConfig.ConfigurationListItem
					{
						Value = value
					});
				}
			}

			public TransportAppConfig.ConfigurationListItem this[int index]
			{
				get
				{
					return (TransportAppConfig.ConfigurationListItem)base.BaseGet(index);
				}
			}

			protected override ConfigurationElement CreateNewElement()
			{
				return new TransportAppConfig.ConfigurationListItem();
			}

			protected override object GetElementKey(ConfigurationElement element)
			{
				return ((TransportAppConfig.ConfigurationListItem)element).Value;
			}
		}

		public class ConfigurationListsSection : ConfigurationSection
		{
			internal ConfigurationListsSection()
			{
			}

			[ConfigurationProperty("downgradedResponses")]
			public TransportAppConfig.ConfigurationList DowngradedResponses
			{
				get
				{
					return (TransportAppConfig.ConfigurationList)base["downgradedResponses"];
				}
				internal set
				{
					base["downgradedResponses"] = value;
				}
			}

			[ConfigurationProperty("transportRulesScanVelocities")]
			public TransportAppConfig.ConfigurationList TransportRulesScanVelocities
			{
				get
				{
					return (TransportAppConfig.ConfigurationList)base["transportRulesScanVelocities"];
				}
				internal set
				{
					base["transportRulesScanVelocities"] = value;
				}
			}

			[ConfigurationProperty("upgradedResponses")]
			public TransportAppConfig.ConfigurationList UpgradedResponses
			{
				get
				{
					return (TransportAppConfig.ConfigurationList)base["upgradedResponses"];
				}
				internal set
				{
					base["upgradedResponses"] = value;
				}
			}
		}

		public sealed class ResourceManagerConfig
		{
			private ResourceManagerConfig()
			{
			}

			public bool EnableResourceMonitoring
			{
				get
				{
					return this.enableResourceMonitoring;
				}
			}

			public TimeSpan ResourceMonitoringInterval
			{
				get
				{
					return this.resourceMonitoringInterval;
				}
			}

			public int PercentagePrivateBytesHighThreshold
			{
				get
				{
					return this.percentagePrivateBytesHighThreshold;
				}
			}

			public int PercentagePrivateBytesMediumThreshold
			{
				get
				{
					return this.percentagePrivateBytesMediumThreshold;
				}
			}

			public int PercentagePrivateBytesNormalThreshold
			{
				get
				{
					return this.percentagePrivateBytesNormalThreshold;
				}
			}

			public int PercentageDatabaseDiskSpaceHighThreshold
			{
				get
				{
					return this.percentageDatabaseDiskSpaceHighThreshold;
				}
			}

			public int PercentageDatabaseDiskSpaceMediumThreshold
			{
				get
				{
					return this.percentageDatabaseDiskSpaceMediumThreshold;
				}
			}

			public int PercentageDatabaseDiskSpaceNormalThreshold
			{
				get
				{
					return this.percentageDatabaseDiskSpaceNormalThreshold;
				}
			}

			public int PercentageDatabaseLoggingDiskSpaceHighThreshold
			{
				get
				{
					return this.percentageLoggingDiskSpaceHighThreshold;
				}
			}

			public int PercentageDatabaseLoggingDiskSpaceMediumThreshold
			{
				get
				{
					return this.percentageLoggingDiskSpaceMediumThreshold;
				}
			}

			public int PercentageDatabaseLoggingDiskSpaceNormalThreshold
			{
				get
				{
					return this.percentageLoggingDiskSpaceNormalThreshold;
				}
			}

			public ByteQuantifiedSize TempDiskSpaceRequired
			{
				get
				{
					return this.tempDiskSpaceRequired;
				}
			}

			public int VersionBucketsHighThreshold
			{
				get
				{
					return this.versionBucketsHighThreshold;
				}
			}

			public int VersionBucketsMediumThreshold
			{
				get
				{
					return this.versionBucketsMediumThreshold;
				}
			}

			public int VersionBucketsNormalThreshold
			{
				get
				{
					return this.versionBucketsNormalThreshold;
				}
			}

			public int VersionBucketsHistoryDepth
			{
				get
				{
					return this.versionBucketsHistoryDepth;
				}
			}

			public int PrivateBytesHistoryDepth
			{
				get
				{
					return this.privateBytesHistoryDepth;
				}
			}

			public int PercentagePhysicalMemoryUsedLimit
			{
				get
				{
					return this.percentagePhysicalMemoryUsedLimit;
				}
			}

			public bool DehydrateMessagesUnderMemoryPressure
			{
				get
				{
					return this.dehydrateMessagesUnderMemoryPressure;
				}
			}

			public int SubmissionQueueNormalThreshold
			{
				get
				{
					return this.submissionQueueNormalThreshold;
				}
			}

			public int SubmissionQueueMediumThreshold
			{
				get
				{
					return this.submissionQueueMediumThreshold;
				}
			}

			public int SubmissionQueueHighThreshold
			{
				get
				{
					return this.submissionQueueHighThreshold;
				}
			}

			public int SubmissionQueueHistoryDepth
			{
				get
				{
					return this.submissionQueueHistoryDepth;
				}
			}

			public TimeSpan BaseThrottlingDelayInterval
			{
				get
				{
					return this.baseThrottlingDelayInterval;
				}
			}

			public TimeSpan MaxThrottlingDelayInterval
			{
				get
				{
					return this.maxThrottlingDelayInterval;
				}
			}

			public TimeSpan StepThrottlingDelayInterval
			{
				get
				{
					return this.stepThrottlingDelayInterval;
				}
			}

			public TimeSpan StartThrottlingDelayInterval
			{
				get
				{
					return this.startThrottlingDelayInterval;
				}
			}

			public static TransportAppConfig.ResourceManagerConfig Load()
			{
				TransportAppConfig.ResourceManagerConfig resourceManagerConfig = new TransportAppConfig.ResourceManagerConfig();
				resourceManagerConfig.enableResourceMonitoring = TransportAppConfig.GetConfigBool("EnableResourceMonitoring", true);
				if (resourceManagerConfig.enableResourceMonitoring)
				{
					resourceManagerConfig.resourceMonitoringInterval = TransportAppConfig.GetConfigTimeSpan("ResourceMonitoringInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(2.0));
					resourceManagerConfig.percentageDatabaseDiskSpaceHighThreshold = TransportAppConfig.GetConfigInt("PercentageDatabaseDiskSpaceUsedHighThreshold", 10, 100, 100);
					if (resourceManagerConfig.percentageDatabaseDiskSpaceHighThreshold != 0)
					{
						resourceManagerConfig.percentageDatabaseDiskSpaceMediumThreshold = TransportAppConfig.GetConfigInt("PercentageDatabaseDiskSpaceUsedMediumThreshold", 5, resourceManagerConfig.percentageDatabaseDiskSpaceHighThreshold - 1, resourceManagerConfig.percentageDatabaseDiskSpaceHighThreshold - 2);
						resourceManagerConfig.percentageDatabaseDiskSpaceNormalThreshold = TransportAppConfig.GetConfigInt("PercentageDatabaseDiskSpaceUsedNormalThreshold", 3, resourceManagerConfig.percentageDatabaseDiskSpaceMediumThreshold - 1, resourceManagerConfig.percentageDatabaseDiskSpaceMediumThreshold - 2);
					}
					resourceManagerConfig.percentageLoggingDiskSpaceHighThreshold = TransportAppConfig.GetConfigInt("PercentageDatabaseLoggingDiskSpaceUsedHighThreshold", 10, 100, 100);
					if (resourceManagerConfig.percentageLoggingDiskSpaceHighThreshold != 0)
					{
						resourceManagerConfig.percentageLoggingDiskSpaceMediumThreshold = TransportAppConfig.GetConfigInt("PercentageDatabaseLoggingDiskSpaceUsedMediumThreshold", 5, resourceManagerConfig.percentageLoggingDiskSpaceHighThreshold - 1, resourceManagerConfig.percentageLoggingDiskSpaceHighThreshold - 2);
						resourceManagerConfig.percentageLoggingDiskSpaceNormalThreshold = TransportAppConfig.GetConfigInt("PercentageDatabaseLoggingDiskSpaceUsedNormalThreshold", 3, resourceManagerConfig.percentageLoggingDiskSpaceMediumThreshold - 1, resourceManagerConfig.percentageLoggingDiskSpaceMediumThreshold - 2);
					}
					resourceManagerConfig.tempDiskSpaceRequired = TransportAppConfig.GetConfigByteQuantifiedSize("TempDiskSpaceRequired", ByteQuantifiedSize.FromKB(100UL), ByteQuantifiedSize.FromBytes(ulong.MaxValue), ByteQuantifiedSize.FromMB(200UL));
					resourceManagerConfig.percentagePrivateBytesHighThreshold = TransportAppConfig.GetConfigInt("PercentagePrivateBytesUsedHighThreshold", 10, 100, 0);
					if (resourceManagerConfig.percentagePrivateBytesHighThreshold != 0)
					{
						resourceManagerConfig.percentagePrivateBytesMediumThreshold = TransportAppConfig.GetConfigInt("PercentagePrivateBytesUsedMediumThreshold", 8, resourceManagerConfig.percentagePrivateBytesHighThreshold - 1, resourceManagerConfig.percentagePrivateBytesHighThreshold - 2);
						resourceManagerConfig.percentagePrivateBytesNormalThreshold = TransportAppConfig.GetConfigInt("PercentagePrivateBytesUsedNormalThreshold", 6, resourceManagerConfig.percentagePrivateBytesMediumThreshold - 1, resourceManagerConfig.percentagePrivateBytesMediumThreshold - 2);
					}
					resourceManagerConfig.versionBucketsHighThreshold = TransportAppConfig.GetConfigInt("VersionBucketsHighThreshold", 1, 8000, 2500);
					resourceManagerConfig.versionBucketsMediumThreshold = TransportAppConfig.GetConfigInt("VersionBucketsMediumThreshold", 1, resourceManagerConfig.versionBucketsHighThreshold, 2000);
					resourceManagerConfig.versionBucketsNormalThreshold = TransportAppConfig.GetConfigInt("VersionBucketsNormalThreshold", 1, resourceManagerConfig.versionBucketsMediumThreshold, 1750);
					resourceManagerConfig.percentagePhysicalMemoryUsedLimit = TransportAppConfig.GetConfigInt("PercentagePhysicalMemoryUsedLimit", 20, 100, 94);
					resourceManagerConfig.dehydrateMessagesUnderMemoryPressure = TransportAppConfig.GetConfigBool("DehydrateMessagesUnderMemoryPressure", true);
					resourceManagerConfig.versionBucketsHistoryDepth = TransportAppConfig.GetConfigInt("VersionBucketsHistoryDepth", 1, 1000, 10);
					resourceManagerConfig.privateBytesHistoryDepth = TransportAppConfig.GetConfigInt("PrivateBytesHistoryDepth", 1, 1000, 30);
					resourceManagerConfig.submissionQueueHighThreshold = TransportAppConfig.GetConfigInt("SubmissionQueueHighThreshold", 1, 50000, 15000);
					resourceManagerConfig.submissionQueueMediumThreshold = TransportAppConfig.GetConfigInt("SubmissionQueueMediumThreshold", 1, resourceManagerConfig.submissionQueueHighThreshold, 10000);
					resourceManagerConfig.submissionQueueNormalThreshold = TransportAppConfig.GetConfigInt("SubmissionQueueNormalThreshold", 1, resourceManagerConfig.submissionQueueMediumThreshold, 2000);
					resourceManagerConfig.submissionQueueHistoryDepth = TransportAppConfig.GetConfigInt("SubmissionQueueHistoryDepth", 1, 1000, 300);
					resourceManagerConfig.maxThrottlingDelayInterval = TransportAppConfig.GetConfigTimeSpan("SmtpMaxThrottlingDelayInterval", TimeSpan.Zero, TimeSpan.FromMinutes(10.0), TimeSpan.FromSeconds(55.0));
					resourceManagerConfig.baseThrottlingDelayInterval = TransportAppConfig.GetConfigTimeSpan("SmtpBaseThrottlingDelayInterval", TimeSpan.Zero, resourceManagerConfig.maxThrottlingDelayInterval, TimeSpan.Zero);
					resourceManagerConfig.stepThrottlingDelayInterval = TransportAppConfig.GetConfigTimeSpan("SmtpStepThrottlingDelayInterval", TimeSpan.Zero, resourceManagerConfig.maxThrottlingDelayInterval, TimeSpan.FromSeconds(1.0));
					resourceManagerConfig.startThrottlingDelayInterval = TransportAppConfig.GetConfigTimeSpan("SmtpStartThrottlingDelayInterval", TimeSpan.Zero, resourceManagerConfig.maxThrottlingDelayInterval, TimeSpan.FromSeconds(1.0));
				}
				return resourceManagerConfig;
			}

			private const int MaxHistoryDepth = 1000;

			private bool enableResourceMonitoring;

			private TimeSpan resourceMonitoringInterval;

			private int percentagePrivateBytesHighThreshold;

			private int percentagePrivateBytesMediumThreshold;

			private int percentagePrivateBytesNormalThreshold;

			private int percentageDatabaseDiskSpaceHighThreshold;

			private int percentageDatabaseDiskSpaceMediumThreshold;

			private int percentageDatabaseDiskSpaceNormalThreshold;

			private int percentageLoggingDiskSpaceHighThreshold;

			private int percentageLoggingDiskSpaceMediumThreshold;

			private int percentageLoggingDiskSpaceNormalThreshold;

			private ByteQuantifiedSize tempDiskSpaceRequired;

			private int versionBucketsHighThreshold;

			private int versionBucketsMediumThreshold;

			private int versionBucketsNormalThreshold;

			private int versionBucketsHistoryDepth;

			private int privateBytesHistoryDepth;

			private int percentagePhysicalMemoryUsedLimit;

			private bool dehydrateMessagesUnderMemoryPressure;

			private int submissionQueueHighThreshold;

			private int submissionQueueMediumThreshold;

			private int submissionQueueNormalThreshold;

			private int submissionQueueHistoryDepth;

			private TimeSpan baseThrottlingDelayInterval;

			private TimeSpan maxThrottlingDelayInterval;

			private TimeSpan stepThrottlingDelayInterval;

			private TimeSpan startThrottlingDelayInterval;
		}

		public class DumpsterConfig
		{
			private DumpsterConfig()
			{
			}

			public bool AllowDuplicateDelivery
			{
				get
				{
					return this.allowDuplicateDelivery;
				}
			}

			public static TransportAppConfig.DumpsterConfig Load()
			{
				TransportAppConfig.DumpsterConfig dumpsterConfig = new TransportAppConfig.DumpsterConfig();
				string value = ConfigurationManager.AppSettings["DumpsterAllowDuplicateDelivery"];
				if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out dumpsterConfig.allowDuplicateDelivery))
				{
					dumpsterConfig.allowDuplicateDelivery = false;
				}
				return dumpsterConfig;
			}

			private const string AllowDuplicateDeliveryString = "DumpsterAllowDuplicateDelivery";

			private bool allowDuplicateDelivery;
		}

		public sealed class ShadowRedundancyConfig
		{
			private ShadowRedundancyConfig()
			{
			}

			public bool ShadowRedundancyLocalDisabled
			{
				get
				{
					return this.shadowRedundancyLocalDisabled;
				}
			}

			public bool DelayedAckSkippingEnabled
			{
				get
				{
					return this.delayedAckSkippingEnabled;
				}
			}

			public int DelayedAckSkippingQueueLength
			{
				get
				{
					return this.delayedAckSkippingQueueLength;
				}
			}

			public bool ShadowRedundancyPromotionEnabled
			{
				get
				{
					return this.shadowRedundancyPromotionEnabled;
				}
			}

			public ShadowRedundancyCompatibilityVersion CompatibilityVersion
			{
				get
				{
					return this.compatibilityVersion;
				}
			}

			public List<RoutingHost> ShadowHubList
			{
				get
				{
					return this.shadowHubList;
				}
			}

			public int MaxDiscardIdsPerSmtpCommand
			{
				get
				{
					return this.maxDiscardIdsPerSmtpCommand;
				}
			}

			public TimeSpan MaxPendingHeartbeatInterval
			{
				get
				{
					return this.maxPendingHeartbeatInterval;
				}
			}

			public static TransportAppConfig.ShadowRedundancyConfig Load()
			{
				return new TransportAppConfig.ShadowRedundancyConfig
				{
					shadowRedundancyLocalDisabled = TransportAppConfig.GetConfigBool("ShadowRedundancyLocalDisabled", false),
					delayedAckSkippingEnabled = TransportAppConfig.GetConfigBool("DelayedAckSkippingEnabled", true),
					delayedAckSkippingQueueLength = TransportAppConfig.GetConfigInt("DelayedAckSkippingQueueLength", 1, 250000, 100),
					shadowRedundancyPromotionEnabled = TransportAppConfig.GetConfigBool("ShadowRedundancyPromotionEnabled", false),
					compatibilityVersion = TransportAppConfig.GetConfigEnum<ShadowRedundancyCompatibilityVersion>("ShadowRedundancyCompatibilityVersion", ShadowRedundancyCompatibilityVersion.E15),
					shadowHubList = TransportAppConfig.GetConfigList<RoutingHost>("ShadowRedundancyShadowHubList", ',', new TransportAppConfig.TryParse<RoutingHost>(RoutingHost.TryParse)),
					maxDiscardIdsPerSmtpCommand = TransportAppConfig.GetConfigInt("ShadowRedundancyMaxDiscardIdsPerSmtpCommand", 50, 1000, 50),
					maxPendingHeartbeatInterval = TransportAppConfig.GetConfigTimeSpan("ShadowRedundancyMaxPendingHeartbeatInterval", TransportAppConfig.ShadowRedundancyConfig.MaxPendingHeartbeatIntervalMinValue, TransportAppConfig.ShadowRedundancyConfig.MaxPendingHeartbeatIntervalMaxValue, TransportAppConfig.ShadowRedundancyConfig.DefaultMaxPendingHeartbeatInterval)
				};
			}

			private const string ShadowRedundancyLocalDisabledLabel = "ShadowRedundancyLocalDisabled";

			private const bool ShadowRedundancyLocalDisabledDefault = false;

			private const string DelayedAckSkippingEnabledLabel = "DelayedAckSkippingEnabled";

			private const bool DelayedAckSkippingEnabledDefault = true;

			private const string DelayedAckSkippingQueueLengthLabel = "DelayedAckSkippingQueueLength";

			private const int DelayedAckSkippingQueueLengthMin = 1;

			private const int DelayedAckSkippingQueueLengthMax = 250000;

			private const int DelayedAckSkippingQueueLengthDefault = 100;

			private const string ShadowRedundancyPromotionEnabledLabel = "ShadowRedundancyPromotionEnabled";

			private const bool ShadowRedundancyPromotionEnabledDefault = false;

			private const string ShadowRedundancyCompatibilityVersionLabel = "ShadowRedundancyCompatibilityVersion";

			private const string ShadowRedundancyShadowHubListLabel = "ShadowRedundancyShadowHubList";

			private const string MaxDiscardIdsPerSmtpCommandLabel = "ShadowRedundancyMaxDiscardIdsPerSmtpCommand";

			private const int MaxDiscardIdsPerSmtpCommandMin = 50;

			private const int MaxDiscardIdsPerSmtpCommandMax = 1000;

			private const int MaxDiscardIdsPerSmtpCommandDefault = 50;

			private const char ShadowRedundancyShadowHubListSeparator = ',';

			private const ShadowRedundancyCompatibilityVersion DefaultCompatibilityVersion = ShadowRedundancyCompatibilityVersion.E15;

			private const string ShadowRedundancyMaxPendingHeartbeatIntervalLabel = "ShadowRedundancyMaxPendingHeartbeatInterval";

			private static readonly TimeSpan DefaultMaxPendingHeartbeatInterval = TimeSpan.FromMinutes(30.0);

			private static readonly TimeSpan MaxPendingHeartbeatIntervalMinValue = TimeSpan.FromMinutes(1.0);

			private static readonly TimeSpan MaxPendingHeartbeatIntervalMaxValue = TimeSpan.MaxValue;

			private bool shadowRedundancyLocalDisabled;

			private bool delayedAckSkippingEnabled;

			private int delayedAckSkippingQueueLength;

			private bool shadowRedundancyPromotionEnabled;

			private ShadowRedundancyCompatibilityVersion compatibilityVersion;

			private List<RoutingHost> shadowHubList;

			private int maxDiscardIdsPerSmtpCommand;

			private TimeSpan maxPendingHeartbeatInterval;
		}

		public sealed class JetDatabaseConfig
		{
			private JetDatabaseConfig()
			{
			}

			public ByteQuantifiedSize CheckpointDepthMax
			{
				get
				{
					return this.checkpointDepthMax;
				}
			}

			public ByteQuantifiedSize MaxCacheSize
			{
				get
				{
					return this.maxCacheSize;
				}
			}

			public ByteQuantifiedSize MinCacheSize
			{
				get
				{
					return this.minCacheSize;
				}
			}

			public uint StartFlushThreshold
			{
				get
				{
					return this.startFlushThresholdPercent;
				}
			}

			public uint StopFlushThreshold
			{
				get
				{
					return this.stopFlushThresholdPercent;
				}
			}

			public ByteQuantifiedSize BufferedStreamSize
			{
				get
				{
					return this.bufferedStreamSize;
				}
			}

			public uint AutomaticShrinkDatabaseFreeSpaceThreshold
			{
				get
				{
					return this.automaticShrinkDatabaseFreeSpaceThreshold;
				}
			}

			public static TransportAppConfig.JetDatabaseConfig Load()
			{
				TransportAppConfig.JetDatabaseConfig jetDatabaseConfig = new TransportAppConfig.JetDatabaseConfig();
				jetDatabaseConfig.checkpointDepthMax = TransportAppConfig.GetConfigByteQuantifiedSize("DatabaseCheckPointDepthMax", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromMB(384UL));
				jetDatabaseConfig.maxCacheSize = TransportAppConfig.GetConfigByteQuantifiedSize("DatabaseMaxCacheSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromMB(512UL));
				jetDatabaseConfig.minCacheSize = TransportAppConfig.GetConfigByteQuantifiedSize("DatabaseMinCacheSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromMB(32UL));
				if (jetDatabaseConfig.minCacheSize.ToBytes() > jetDatabaseConfig.maxCacheSize.ToBytes())
				{
					jetDatabaseConfig.minCacheSize = jetDatabaseConfig.maxCacheSize / 2;
				}
				string text = ConfigurationManager.AppSettings["DatabaseCacheFlushStart"];
				uint num;
				if (!string.IsNullOrEmpty(text) && uint.TryParse(text, out num) && num <= 100U)
				{
					jetDatabaseConfig.startFlushThresholdPercent = num;
				}
				string text2 = ConfigurationManager.AppSettings["DatabaseCacheFlushStop"];
				if (!string.IsNullOrEmpty(text2) && uint.TryParse(text2, out num) && num <= 100U)
				{
					jetDatabaseConfig.stopFlushThresholdPercent = num;
				}
				jetDatabaseConfig.bufferedStreamSize = TransportAppConfig.GetConfigByteQuantifiedSize("BufferedStreamSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromKB(128UL), ByteQuantifiedSize.FromKB(32UL));
				string text3 = ConfigurationManager.AppSettings["AutomaticShrinkDatabaseFreeSpaceThreshold"];
				if (!string.IsNullOrEmpty(text3) && uint.TryParse(text3, out num) && num >= 0U && num <= 100U)
				{
					jetDatabaseConfig.automaticShrinkDatabaseFreeSpaceThreshold = num;
				}
				return jetDatabaseConfig;
			}

			public static readonly ByteQuantifiedSize PageSize = ByteQuantifiedSize.FromKB(32UL);

			private ByteQuantifiedSize checkpointDepthMax;

			private ByteQuantifiedSize maxCacheSize;

			private ByteQuantifiedSize minCacheSize;

			private uint startFlushThresholdPercent = 3U;

			private uint stopFlushThresholdPercent = 5U;

			private ByteQuantifiedSize bufferedStreamSize;

			private uint automaticShrinkDatabaseFreeSpaceThreshold = 20U;
		}

		public class RemoteDeliveryConfig
		{
			private RemoteDeliveryConfig()
			{
				int num = Enum.GetNames(typeof(DeliveryPriority)).Length;
				this.maxPerDomainPriorityConnections = new int[num];
				this.maxPerDomainPriorityConnections[0] = 3;
				this.maxPerDomainPriorityConnections[1] = 15;
				this.maxPerDomainPriorityConnections[2] = 2;
				this.maxPerDomainPriorityConnections[3] = 2;
				this.messageExpirationTimeout = new TimeSpan[num];
				this.messageExpirationTimeout[0] = TransportAppConfig.RemoteDeliveryConfig.DefaultHighPriorityMessageExpirationTimeout;
				this.messageExpirationTimeout[1] = TransportAppConfig.RemoteDeliveryConfig.DefaultNormalPriorityMessageExpirationTimeout;
				this.messageExpirationTimeout[2] = TransportAppConfig.RemoteDeliveryConfig.DefaultLowPriorityMessageExpirationTimeout;
				this.messageExpirationTimeout[3] = TransportAppConfig.RemoteDeliveryConfig.DefaultLowPriorityMessageExpirationTimeout;
				this.delayNotificationTimeout = new TimeSpan[num];
				this.delayNotificationTimeout[0] = TransportAppConfig.RemoteDeliveryConfig.DefaultHighPriorityDelayNotificationTimeout;
				this.delayNotificationTimeout[1] = TransportAppConfig.RemoteDeliveryConfig.DefaultNormalPriorityDelayNotificationTimeout;
				this.delayNotificationTimeout[2] = TransportAppConfig.RemoteDeliveryConfig.DefaultLowPriorityDelayNotificationTimeout;
				this.delayNotificationTimeout[3] = TransportAppConfig.RemoteDeliveryConfig.DefaultLowPriorityDelayNotificationTimeout;
			}

			internal static TransportAppConfig.RemoteDeliveryConfig CreateForUnitTestsOnly(List<int> internalMessageRetryIntervalRangeList, List<int> externalMessageRetryIntervalRangeList)
			{
				TransportAppConfig.RemoteDeliveryConfig remoteDeliveryConfig = new TransportAppConfig.RemoteDeliveryConfig();
				TransportAppConfig.RemoteDeliveryConfig.ValidateMessageRetryIntervalRangeList(internalMessageRetryIntervalRangeList);
				TransportAppConfig.RemoteDeliveryConfig.ValidateMessageRetryIntervalRangeList(externalMessageRetryIntervalRangeList);
				remoteDeliveryConfig.internalMessageRetryIntervalRangeList = internalMessageRetryIntervalRangeList;
				remoteDeliveryConfig.externalMessageRetryIntervalRangeList = externalMessageRetryIntervalRangeList;
				return remoteDeliveryConfig;
			}

			public int MessageThresholdToUpdateHealthCounters
			{
				get
				{
					return this.messageThresholdToUpdateHealthCounters;
				}
			}

			public TimeSpan RefreshIntervalToUpdateHealth
			{
				get
				{
					return this.refreshIntervalToUpdateHealth;
				}
			}

			public TimeSpan MaxIdleTimeBeforeResubmit
			{
				get
				{
					return this.maxIdleTimeBeforeResubmission;
				}
			}

			public TimeSpan MailboxDeliveryQueueRetryInterval
			{
				get
				{
					return this.mailboxDeliveryQueueRetryInterval;
				}
			}

			public TimeSpan MailboxDeliveryFastQueueRetryInterval
			{
				get
				{
					return this.mailboxDeliveryFastQueueRetryInterval;
				}
			}

			public TimeSpan MailboxServerThreadLimitQueueRetryInterval
			{
				get
				{
					return this.mailboxServerThreadLimitQueueRetryInterval;
				}
			}

			public TimeSpan MailboxDatabaseThreadLimitQueueRetryInterval
			{
				get
				{
					return this.mailboxDatabaseThreadLimitQueueRetryInterval;
				}
			}

			public int MaxMailboxDeliveryPerMdbConnections
			{
				get
				{
					return this.maxMailboxDeliveryPerMdbConnections;
				}
			}

			public int MaxMailboxDeliveryPerMdbConnectionsHighHealthPercent
			{
				get
				{
					return this.maxMailboxDeliveryPerMdbConnectionsHighHealthPercent;
				}
			}

			public int MaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent
			{
				get
				{
					return this.maxMailboxDeliveryPerMdbConnectionsMediumHealthPercent;
				}
			}

			public int MaxMailboxDeliveryPerMdbConnectionsLowHealthPercent
			{
				get
				{
					return this.maxMailboxDeliveryPerMdbConnectionsLowHealthPercent;
				}
			}

			public int MaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent
			{
				get
				{
					return this.maxMailboxDeliveryPerMdbConnectionsLowestHealthPercent;
				}
			}

			public bool DynamicMailboxDatabaseThrottlingEnabled
			{
				get
				{
					return this.dynamicMailboxDatabaseThrottlingEnabled;
				}
			}

			public ByteQuantifiedSize MaxMailboxDeliveryConcurrentMessageSizeLimit
			{
				get
				{
					return this.maxMailboxDeliveryConcurrentMessageSizeLimit;
				}
			}

			public int MaxStoreDriverDeliveryExceptionCallstackHistoryPerBucket
			{
				get
				{
					return this.maxStoreDriverDeliveryExceptionCallstackHistoryPerBucket;
				}
			}

			public int MaxStoreDriverDeliveryExceptionOccurrenceHistoryPerException
			{
				get
				{
					return this.maxStoreDriverDeliveryExceptionOccurrenceHistoryPerException;
				}
			}

			public string StoreDriverExceptionCallstackToTrap
			{
				get
				{
					return this.storeDriverExceptionCallstackToTrap;
				}
			}

			public int MdbHealthMediumToHighThreshold
			{
				get
				{
					return this.mdbHealthMediumToHighThreshold;
				}
			}

			public int MdbHealthLowToMediumThreshold
			{
				get
				{
					return this.mdbHealthLowToMediumThreshold;
				}
			}

			public bool MailboxDeliveryThrottlingEnabled
			{
				get
				{
					return this.mailboxDeliveryThrottlingEnabled;
				}
			}

			public int MailboxDeliveryThrottlingLogBufferSize
			{
				get
				{
					return this.mailboxDeliveryThrottlingLogBufferSize;
				}
			}

			public TimeSpan MailboxDeliveryThrottlingLogFlushInterval
			{
				get
				{
					return this.mailboxDeliveryThrottlingLogFlushInterval;
				}
			}

			public TimeSpan MailboxDeliveryThrottlingLogAsyncLogInterval
			{
				get
				{
					return this.mailboxDeliveryThrottlingLogAsyncLogInterval;
				}
			}

			public TimeSpan MailboxDeliveryThrottlingLogSummaryLoggingInterval
			{
				get
				{
					return this.mailboxDeliveryThrottlingLogSummaryLoggingInterval;
				}
			}

			public bool MailboxTransportTableBasedExceptionHandlerEnabled
			{
				get
				{
					return this.mailboxTransportTableBasedExceptionHandlerEnabled;
				}
			}

			public TimeSpan QuarantinedMailboxRetryInterval
			{
				get
				{
					return this.quarantinedMailboxRetryInterval;
				}
			}

			public TimeSpan QueueGlitchRetryInterval
			{
				get
				{
					return this.queueGlitchRetryInterval;
				}
			}

			public int QueueGlitchRetryCount
			{
				get
				{
					return this.queueGlitchRetryCount;
				}
			}

			public int MaxQueryResultCount
			{
				get
				{
					return this.maxQueryResultCount;
				}
			}

			public DnsFaultTolerance DnsFaultTolerance
			{
				get
				{
					return this.dnsFaultTolerance;
				}
			}

			public bool LoadBalancingForServerFailoverEnabled
			{
				get
				{
					return this.loadBalancingForServerFailoverEnabled;
				}
			}

			public bool PriorityQueuingEnabled
			{
				get
				{
					return this.priorityQueuingEnabled;
				}
			}

			public bool LocalDeliveryPriorityQueuingEnabled
			{
				get
				{
					return this.localDeliveryPriorityQueuingEnabled;
				}
			}

			public bool RemoteDeliveryPriorityQueuingEnabled
			{
				get
				{
					return this.remoteDeliveryPriorityQueuingEnabled;
				}
			}

			public ByteQuantifiedSize MaxHighPriorityMessageSize
			{
				get
				{
					return this.maxHighPriorityMessageSize;
				}
			}

			public int MailboxServerThreadLimit
			{
				get
				{
					return this.mailboxServerThreadLimit;
				}
			}

			public int RecipientThreadLimit
			{
				get
				{
					return this.recipientThreadLimit;
				}
			}

			public int DeliverySourceThreadLimit
			{
				get
				{
					return this.deliverySourceThreadLimitPerCore * Environment.ProcessorCount;
				}
			}

			public int MailboxDeliveryMaxMessagesPerConnection
			{
				get
				{
					return this.mailboxDeliveryMaxMessagesPerConnection;
				}
			}

			public int[] DeliveryPriorityQuotas
			{
				get
				{
					return this.deliveryPriorityQuotas;
				}
			}

			public TimeSpan StoreDriverRecipientDeliveryHangThreshold
			{
				get
				{
					return this.storeDriverRecipientDeliveryHangThreshold;
				}
			}

			public TimeSpan StoreDriverDeliveryHangDetectionInterval
			{
				get
				{
					return this.storeDriverDeliveryHangDetectionInterval;
				}
			}

			public bool ExcludeDnsServersFromLoopbackAdapters
			{
				get
				{
					return this.excludeDnsServersFromLoopbackAdapters;
				}
			}

			public bool ExcludeIPv6SiteLocalDnsAddresses
			{
				get
				{
					return this.excludeIPv6SiteLocalDnsAddresses;
				}
			}

			public TimeSpan DnsRequestTimeout
			{
				get
				{
					return this.dnsRequestTimeout;
				}
			}

			public TimeSpan DnsQueryRetryInterval
			{
				get
				{
					return this.dnsQueryRetryInterval;
				}
			}

			public TimeSpan ConfigUpdateResubmitDeferInterval
			{
				get
				{
					return this.configUpdateResubmitDeferInterval;
				}
			}

			public bool DnsIpv6Enabled
			{
				get
				{
					return this.dnsIpv6Enabled;
				}
			}

			public int MailboxQueueMessageCountThresholdForConcurrentConnections
			{
				get
				{
					return this.mailboxQueueMessageCountThresholdForConcurrentConnections;
				}
			}

			public int SmtpConnectorQueueMessageCountThresholdForConcurrentConnections
			{
				get
				{
					return this.smtpConnectorQueueMessageCountThresholdForConcurrentConnections;
				}
			}

			public int IntraorgSmtpQueueMessageCountThresholdForConcurrentConnections
			{
				get
				{
					return this.intraorgSmtpQueueMessageCountThresholdForConcurrentConnections;
				}
			}

			public int OtherQueueMessageCountThresholdForConcurrentConnections
			{
				get
				{
					return this.otherQueueMessageCountThresholdForConcurrentConnections;
				}
			}

			public int DeprioritizeOnRecipientThreadLimitExceededCount
			{
				get
				{
					return this.deprioritizeOnRecipientThreadLimitExceededCount;
				}
			}

			public bool MessageRetryIntervalProgressiveBackoffEnabled
			{
				get
				{
					return this.messageRetryIntervalProgressiveBackoffEnabled;
				}
			}

			public TimeSpan ResubmitDueToOutboundConnectorChangeInterval
			{
				get
				{
					return this.resubmitDueToOutboundConnectorChangeInterval;
				}
			}

			public TimeSpan OutboundConnectorLookbackBufferInterval
			{
				get
				{
					return this.outboundConnectorLookbackBufferInterval;
				}
			}

			public static TransportAppConfig.RemoteDeliveryConfig Load()
			{
				TransportAppConfig.RemoteDeliveryConfig remoteDeliveryConfig = new TransportAppConfig.RemoteDeliveryConfig();
				remoteDeliveryConfig.maxIdleTimeBeforeResubmission = TransportAppConfig.GetConfigTimeSpan("MaxIdleTimeBeforeResubmit", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxIdleTimeBeforeResubmit);
				remoteDeliveryConfig.mailboxDeliveryQueueRetryInterval = TransportAppConfig.GetConfigTimeSpan("MailboxDeliveryQueueRetryInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDeliveryQueueRetryInterval);
				remoteDeliveryConfig.mailboxDeliveryFastQueueRetryInterval = TransportAppConfig.GetConfigTimeSpan("MailboxDeliveryFastQueueRetryInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDeliveryFastQueueRetryInterval);
				remoteDeliveryConfig.mailboxServerThreadLimitQueueRetryInterval = TransportAppConfig.GetConfigTimeSpan("MailboxServerThreadLimitQueueRetryInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxServerThreadLimitQueueRetryInterval);
				remoteDeliveryConfig.mailboxDatabaseThreadLimitQueueRetryInterval = TransportAppConfig.GetConfigTimeSpan("MailboxDatabaseThreadLimitQueueRetryInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDatabaseThreadLimitQueueRetryInterval);
				remoteDeliveryConfig.maxMailboxDeliveryPerMdbConnections = TransportAppConfig.GetConfigInt("MaxMailboxDeliveryPerMdbConnections", 1, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnections);
				remoteDeliveryConfig.maxMailboxDeliveryPerMdbConnectionsHighHealthPercent = TransportAppConfig.GetConfigInt("MaxMailboxDeliveryPerMdbConnectionsHighHealthPercent", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsHighHealthPercent);
				remoteDeliveryConfig.maxMailboxDeliveryPerMdbConnectionsMediumHealthPercent = TransportAppConfig.GetConfigInt("MaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent);
				remoteDeliveryConfig.maxMailboxDeliveryPerMdbConnectionsLowHealthPercent = TransportAppConfig.GetConfigInt("MaxMailboxDeliveryPerMdbConnectionsLowHealthPercent", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsLowHealthPercent);
				remoteDeliveryConfig.maxMailboxDeliveryPerMdbConnectionsLowestHealthPercent = TransportAppConfig.GetConfigInt("MaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent);
				remoteDeliveryConfig.dynamicMailboxDatabaseThrottlingEnabled = TransportAppConfig.GetConfigBool("DynamicMailboxDatabaseThrottlingEnabled", TransportAppConfig.RemoteDeliveryConfig.DefaultDynamicMailboxDatabaseThrottlingEnabled);
				remoteDeliveryConfig.maxMailboxDeliveryConcurrentMessageSizeLimit = TransportAppConfig.GetConfigByteQuantifiedSize("MaxMailboxDeliveryConcurrentMessageSizeLimit", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryConcurrentMessageSizeLimit);
				remoteDeliveryConfig.mdbHealthMediumToHighThreshold = TransportAppConfig.GetConfigInt("MdbHealthMediumToHighThreshold", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMdbHealthMediumToHighThreshold);
				remoteDeliveryConfig.mdbHealthLowToMediumThreshold = TransportAppConfig.GetConfigInt("MdbHealthLowToMediumThreshold", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMdbHealthLowToMediumThreshold);
				remoteDeliveryConfig.mailboxDeliveryThrottlingEnabled = TransportAppConfig.GetConfigBool("MailboxDeliveryThrottlingEnabled", TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDeliveryThrottlingEnabled);
				remoteDeliveryConfig.mailboxDeliveryThrottlingLogBufferSize = TransportAppConfig.GetConfigInt("MailboxDeliveryThrottlingLogBufferSize", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogBufferSize);
				remoteDeliveryConfig.mailboxDeliveryThrottlingLogFlushInterval = TransportAppConfig.GetConfigTimeSpan("MailboxDeliveryThrottlingLogFlushInterval", TimeSpan.MinValue, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogFlushInterval);
				remoteDeliveryConfig.mailboxDeliveryThrottlingLogAsyncLogInterval = TransportAppConfig.GetConfigTimeSpan("MailboxDeliveryThrottlingLogAsyncLogInterval", TimeSpan.MinValue, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogAsyncLogInterval);
				remoteDeliveryConfig.mailboxDeliveryThrottlingLogSummaryLoggingInterval = TransportAppConfig.GetConfigTimeSpan("MailboxDeliveryThrottlingLogSummaryLoggingInterval", TimeSpan.Zero, TimeSpan.FromHours(1.0), TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogSummaryLoggingInterval);
				remoteDeliveryConfig.mailboxTransportTableBasedExceptionHandlerEnabled = TransportAppConfig.GetConfigBool("MailboxTransportTableBasedExceptionHandlerEnabled", TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxTransportTableBasedExceptionHandlerEnabled);
				remoteDeliveryConfig.quarantinedMailboxRetryInterval = TransportAppConfig.GetConfigTimeSpan("QuarantinedMailboxRetryInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultQuarantinedMailboxRetryInterval);
				remoteDeliveryConfig.queueGlitchRetryInterval = TransportAppConfig.GetConfigTimeSpan("QueueGlitchRetryInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultQueueGlithRetryInterval);
				remoteDeliveryConfig.queueGlitchRetryCount = TransportAppConfig.GetConfigInt("QueueGlitchRetryCount", 0, 15, 4);
				remoteDeliveryConfig.maxQueryResultCount = TransportAppConfig.GetConfigInt("MaxQueueViewerQueryResultCount", 1, int.MaxValue, 50000);
				remoteDeliveryConfig.dnsFaultTolerance = TransportAppConfig.GetConfigEnum<DnsFaultTolerance>("DnsFaultTolerance", DnsFaultTolerance.Lenient);
				remoteDeliveryConfig.loadBalancingForServerFailoverEnabled = TransportAppConfig.GetConfigBool("LoadBalancingForServerFailoverEnabled", true);
				remoteDeliveryConfig.priorityQueuingEnabled = TransportAppConfig.GetConfigBool("PriorityQueuingEnabled", false);
				remoteDeliveryConfig.localDeliveryPriorityQueuingEnabled = TransportAppConfig.GetConfigBool("LocalDeliveryPriorityQueuingEnabled", true);
				remoteDeliveryConfig.remoteDeliveryPriorityQueuingEnabled = TransportAppConfig.GetConfigBool("RemoteDeliveryPriorityQueuingEnabled", true);
				remoteDeliveryConfig.maxPerDomainPriorityConnections[0] = TransportAppConfig.GetConfigInt("MaxPerDomainHighPriorityConnections", 1, int.MaxValue, 3);
				remoteDeliveryConfig.maxPerDomainPriorityConnections[1] = TransportAppConfig.GetConfigInt("MaxPerDomainNormalPriorityConnections", 1, int.MaxValue, 15);
				remoteDeliveryConfig.maxPerDomainPriorityConnections[2] = TransportAppConfig.GetConfigInt("MaxPerDomainLowPriorityConnections", 1, int.MaxValue, 2);
				remoteDeliveryConfig.messageExpirationTimeout[0] = TransportAppConfig.GetConfigTimeSpan("HighPriorityMessageExpirationTimeout", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultHighPriorityMessageExpirationTimeout);
				remoteDeliveryConfig.messageExpirationTimeout[1] = TransportAppConfig.GetConfigTimeSpan("NormalPriorityMessageExpirationTimeout", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultNormalPriorityMessageExpirationTimeout);
				remoteDeliveryConfig.messageExpirationTimeout[2] = TransportAppConfig.GetConfigTimeSpan("LowPriorityMessageExpirationTimeout", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultLowPriorityMessageExpirationTimeout);
				remoteDeliveryConfig.delayNotificationTimeout[0] = TransportAppConfig.GetConfigTimeSpan("HighPriorityDelayNotificationTimeout", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultHighPriorityDelayNotificationTimeout);
				remoteDeliveryConfig.delayNotificationTimeout[1] = TransportAppConfig.GetConfigTimeSpan("NormalPriorityDelayNotificationTimeout", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultNormalPriorityDelayNotificationTimeout);
				remoteDeliveryConfig.delayNotificationTimeout[2] = TransportAppConfig.GetConfigTimeSpan("LowPriorityDelayNotificationTimeout", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultLowPriorityDelayNotificationTimeout);
				remoteDeliveryConfig.maxHighPriorityMessageSize = TransportAppConfig.GetConfigByteQuantifiedSize("MaxHighPriorityMessageSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), TransportAppConfig.RemoteDeliveryConfig.DefaultMaxHighPriorityMessageSize);
				remoteDeliveryConfig.mailboxServerThreadLimit = TransportAppConfig.GetConfigInt("MailboxServerThreadLimit", 1, TransportAppConfig.WorkerProcessConfig.MaxWorkerThreadsMaximum, 120);
				remoteDeliveryConfig.recipientThreadLimit = TransportAppConfig.GetConfigInt("RecipientThreadLimit", 1, remoteDeliveryConfig.maxMailboxDeliveryPerMdbConnections, 3);
				remoteDeliveryConfig.deliverySourceThreadLimitPerCore = TransportAppConfig.GetConfigInt("DeliverySourceThreadLimitPerCore", 1, int.MaxValue, 5);
				remoteDeliveryConfig.mailboxDeliveryMaxMessagesPerConnection = TransportAppConfig.GetConfigInt("MailboxDeliveryMaxMessagesPerConnection", 1, int.MaxValue, 20);
				remoteDeliveryConfig.deliveryPriorityQuotas[0] = TransportAppConfig.GetConfigInt("HighPriorityDeliveryQueueQuota", 1, int.MaxValue, 40);
				remoteDeliveryConfig.deliveryPriorityQuotas[1] = TransportAppConfig.GetConfigInt("NormalPriorityDeliveryQueueQuota", 1, int.MaxValue, 25);
				remoteDeliveryConfig.deliveryPriorityQuotas[2] = TransportAppConfig.GetConfigInt("LowPriorityDeliveryQueueQuota", 1, int.MaxValue, 4);
				remoteDeliveryConfig.deliveryPriorityQuotas[3] = TransportAppConfig.GetConfigInt("NonePriorityDeliveryQueueQuota", 1, int.MaxValue, 1);
				remoteDeliveryConfig.maxStoreDriverDeliveryExceptionCallstackHistoryPerBucket = TransportAppConfig.GetConfigInt("MaxStoreDriverDeliveryExceptionCallstackHistoryPerBucket", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxStoreDriverDeliveryExceptionCallstackHistoryPerBucket);
				remoteDeliveryConfig.maxStoreDriverDeliveryExceptionOccurrenceHistoryPerException = TransportAppConfig.GetConfigInt("MaxStoreDriverDeliveryExceptionOccurrenceHistoryPerException", 0, int.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultMaxStoreDriverDeliveryExceptionOccurrenceHistoryPerException);
				remoteDeliveryConfig.storeDriverExceptionCallstackToTrap = TransportAppConfig.GetConfigString("StoreDriverExceptionCallstackToTrap", TransportAppConfig.RemoteDeliveryConfig.DefaultStoreDriverExceptionCallstackToTrap);
				remoteDeliveryConfig.storeDriverRecipientDeliveryHangThreshold = TransportAppConfig.GetConfigTimeSpan("StoreDriverRecipientDeliveryHangThreshold", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultStoreDriverRecipientDeliveryHangThreshold);
				remoteDeliveryConfig.storeDriverDeliveryHangDetectionInterval = TransportAppConfig.GetConfigTimeSpan("StoreDriverDeliveryHangDetectionInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultStoreDriverDeliveryHangDetectionInterval);
				remoteDeliveryConfig.excludeDnsServersFromLoopbackAdapters = TransportAppConfig.GetConfigBool("ExcludeDnsServersFromLoopbackAdapters", true);
				remoteDeliveryConfig.excludeIPv6SiteLocalDnsAddresses = TransportAppConfig.GetConfigBool("ExcludeIPv6SiteLocalDnsAddresses", true);
				remoteDeliveryConfig.dnsRequestTimeout = TransportAppConfig.GetConfigTimeSpan("DnsRequestTimeout", TransportAppConfig.RemoteDeliveryConfig.DefaultDnsRequestTimeout, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultDnsRequestTimeout);
				remoteDeliveryConfig.dnsQueryRetryInterval = TransportAppConfig.GetConfigTimeSpan("DnsQueryRetryInterval", TransportAppConfig.RemoteDeliveryConfig.DefaultDnsQueryRetryInterval, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultDnsQueryRetryInterval);
				remoteDeliveryConfig.configUpdateResubmitDeferInterval = TransportAppConfig.GetConfigTimeSpan("ConfigUpdateResubmitDeferInterval", TimeSpan.Zero, TimeSpan.FromHours(12.0), TimeSpan.FromMinutes(1.0));
				remoteDeliveryConfig.dnsIpv6Enabled = TransportAppConfig.GetConfigBool("DnsIpv6Enabled", true);
				remoteDeliveryConfig.mailboxQueueMessageCountThresholdForConcurrentConnections = TransportAppConfig.GetConfigInt("MailboxQueueMessageCountThresholdForConcurrentConnections", 1, int.MaxValue, 20);
				remoteDeliveryConfig.smtpConnectorQueueMessageCountThresholdForConcurrentConnections = TransportAppConfig.GetConfigInt("SmtpConnectorQueueMessageCountThresholdForConcurrentConnections", 1, int.MaxValue, 20);
				remoteDeliveryConfig.intraorgSmtpQueueMessageCountThresholdForConcurrentConnections = TransportAppConfig.GetConfigInt("IntraorgSmtpQueueMessageCountThresholdForConcurrentConnections", 1, int.MaxValue, 20);
				remoteDeliveryConfig.otherQueueMessageCountThresholdForConcurrentConnections = TransportAppConfig.GetConfigInt("OtherQueueMessageCountThresholdForConcurrentConnections", 1, int.MaxValue, 20);
				remoteDeliveryConfig.refreshIntervalToUpdateHealth = TransportAppConfig.GetConfigTimeSpan("RemoteDeliveryHealthCounterUpdateInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultRefreshIntervalForUpdatingHealth);
				remoteDeliveryConfig.resubmitDueToOutboundConnectorChangeInterval = TransportAppConfig.GetConfigTimeSpan("ResubmitDueToOutboundConnectorChangeInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultResubmitDueToOutboundConnectorChangeInterval);
				remoteDeliveryConfig.outboundConnectorLookbackBufferInterval = TransportAppConfig.GetConfigTimeSpan("OutboundConnectorLookbackBufferInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.RemoteDeliveryConfig.DefaultOutboundConnectorLookbackBufferInterval);
				remoteDeliveryConfig.messageThresholdToUpdateHealthCounters = TransportAppConfig.GetConfigInt("MinimumQueuedMessageCountForMonitoring", 0, int.MaxValue, 100);
				remoteDeliveryConfig.deprioritizeOnRecipientThreadLimitExceededCount = TransportAppConfig.GetConfigInt("DeprioritizeOnRecipientThreadLimitExceededCount", 1, 1000, 3);
				remoteDeliveryConfig.messageRetryIntervalProgressiveBackoffEnabled = TransportAppConfig.GetConfigBool("MessageRetryIntervalProgressiveBackoffEnabled", false);
				remoteDeliveryConfig.internalMessageRetryIntervalRangeList = TransportAppConfig.GetConfigIntList("InternalMessageRetryIntervalRangeList", 1, 43200, 60, ',');
				remoteDeliveryConfig.externalMessageRetryIntervalRangeList = TransportAppConfig.GetConfigIntList("ExternalMessageRetryIntervalRangeList", 1, 43200, 60, ',');
				if (remoteDeliveryConfig.internalMessageRetryIntervalRangeList.Count == 0)
				{
					remoteDeliveryConfig.internalMessageRetryIntervalRangeList = TransportAppConfig.RemoteDeliveryConfig.DefaultInternalMessageRetryIntervalRangeList;
				}
				if (remoteDeliveryConfig.externalMessageRetryIntervalRangeList.Count == 0)
				{
					remoteDeliveryConfig.externalMessageRetryIntervalRangeList = TransportAppConfig.RemoteDeliveryConfig.DefaultExternalMessageRetryIntervalRangeList;
				}
				TransportAppConfig.RemoteDeliveryConfig.ValidateMessageRetryIntervalRangeList(remoteDeliveryConfig.internalMessageRetryIntervalRangeList);
				TransportAppConfig.RemoteDeliveryConfig.ValidateMessageRetryIntervalRangeList(remoteDeliveryConfig.externalMessageRetryIntervalRangeList);
				return remoteDeliveryConfig;
			}

			public int MaxPerDomainPriorityConnections(DeliveryPriority priority)
			{
				return this.maxPerDomainPriorityConnections[(int)priority];
			}

			public TimeSpan MessageExpirationTimeout(DeliveryPriority priority)
			{
				return this.messageExpirationTimeout[(int)priority];
			}

			public TimeSpan DelayNotificationTimeout(DeliveryPriority priority)
			{
				return this.delayNotificationTimeout[(int)priority];
			}

			public TimeSpan GetMessageRetryInterval(int numConsecutiveRetries, bool isInternalDelivery)
			{
				return TransportAppConfig.RemoteDeliveryConfig.GetMessageRetryInterval(numConsecutiveRetries, isInternalDelivery ? this.internalMessageRetryIntervalRangeList : this.externalMessageRetryIntervalRangeList, this.random);
			}

			private static TimeSpan GetMessageRetryInterval(int numConsecutiveRetries, List<int> messageRetryIntervalRangeList, Random random)
			{
				if (numConsecutiveRetries < 1)
				{
					throw new ArgumentOutOfRangeException(string.Format("numConsecutiveRetries [{0}] should have a positive value", numConsecutiveRetries));
				}
				int num = Math.Min(messageRetryIntervalRangeList.Count - 1, 2 * numConsecutiveRetries - 1);
				int num2 = random.Next(messageRetryIntervalRangeList[num - 1], messageRetryIntervalRangeList[num]);
				return TimeSpan.FromSeconds((double)num2);
			}

			private static void ValidateMessageRetryIntervalRangeList(List<int> messageRetryIntervalRangeList)
			{
				if (messageRetryIntervalRangeList.Count < 2)
				{
					throw new ConfigurationErrorsException("retry interval range list should have at least 2 entries");
				}
				if (messageRetryIntervalRangeList.Count % 2 == 1)
				{
					throw new ConfigurationErrorsException("retry interval range list should have an even number of entries");
				}
				for (int i = 1; i < messageRetryIntervalRangeList.Count; i++)
				{
					if (messageRetryIntervalRangeList[i] <= messageRetryIntervalRangeList[i - 1])
					{
						throw new ConfigurationErrorsException("retry interval range list should have entries increasing in value");
					}
				}
			}

			private const bool DefaultDnsIpv6Enabled = true;

			private const int DefaultQueueGlitchRetryCount = 4;

			private const int DefaultMaxQueryResultCount = 50000;

			private const DnsFaultTolerance DefaultDnsFaultTolerance = DnsFaultTolerance.Lenient;

			private const bool DefaultLoadBalancingForServerFailoverEnabled = true;

			private const int DefaultMaxPerDomainHighPriorityConnections = 3;

			private const int DefaultMaxPerDomainNormalPriorityConnections = 15;

			private const int DefaultMaxPerDomainLowPriorityConnections = 2;

			private const int DefaultMailboxServerThreadLimit = 120;

			private const int DefaultRecipientThreadLimit = 3;

			private const int DefaultDeliverySourceThreadLimitPerCore = 5;

			private const int DefaultMailboxDeliveryMaxMessagesPerConnection = 20;

			private const int DefaultNonePriorityDeliveryQueueQuota = 1;

			private const int DefaultLowPriorityDeliveryQueueQuota = 4;

			private const int DefaultNormalPriorityDeliveryQueueQuota = 25;

			private const int DefaultHighPriorityDeliveryQueueQuota = 40;

			private const int DefaultMessageThresoldForUpdatingOutboundIPPoolCounters = 100;

			private static readonly TimeSpan DefaultRefreshIntervalForUpdatingHealth = TimeSpan.FromMinutes(5.0);

			private static readonly TimeSpan DefaultMaxIdleTimeBeforeResubmit = TimeSpan.FromHours(12.0);

			private static readonly TimeSpan DefaultMailboxDeliveryQueueRetryInterval = TimeSpan.FromMinutes(5.0);

			private static readonly TimeSpan DefaultMailboxDeliveryFastQueueRetryInterval = TimeSpan.FromSeconds(2.0);

			private static readonly TimeSpan DefaultMailboxServerThreadLimitQueueRetryInterval = TimeSpan.FromSeconds(5.0);

			private static readonly TimeSpan DefaultMailboxDatabaseThreadLimitQueueRetryInterval = TimeSpan.FromSeconds(5.0);

			private static readonly int DefaultMaxMailboxDeliveryPerMdbConnections = 8;

			private static readonly int DefaultMaxMailboxDeliveryPerMdbConnectionsHighHealthPercent = 75;

			private static readonly int DefaultMaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent = 50;

			private static readonly int DefaultMaxMailboxDeliveryPerMdbConnectionsLowHealthPercent = 25;

			private static readonly int DefaultMaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent = 1;

			private static readonly bool DefaultDynamicMailboxDatabaseThrottlingEnabled = true;

			private static readonly int DefaultThrottlingLogBufferSize = 65536;

			private static readonly TimeSpan DefaultThrottlingLogFlushInterval = TimeSpan.FromSeconds(60.0);

			private static readonly TimeSpan DefaultThrottlingLogAsyncLogInterval = TimeSpan.FromSeconds(15.0);

			private static readonly TimeSpan DefaultThrottlingLogSummaryLoggingInterval = TimeSpan.FromMinutes(15.0);

			private static readonly ByteQuantifiedSize DefaultMaxMailboxDeliveryConcurrentMessageSizeLimit = ByteQuantifiedSize.FromMB(512UL);

			private static readonly int DefaultMaxStoreDriverDeliveryExceptionCallstackHistoryPerBucket = 20;

			private static readonly int DefaultMaxStoreDriverDeliveryExceptionOccurrenceHistoryPerException = 10;

			private static readonly string DefaultStoreDriverExceptionCallstackToTrap = null;

			private static readonly int DefaultMdbHealthMediumToHighThreshold = 67;

			private static readonly int DefaultMdbHealthLowToMediumThreshold = 34;

			private static readonly bool DefaultMailboxDeliveryThrottlingEnabled = true;

			private static readonly bool DefaultMailboxTransportTableBasedExceptionHandlerEnabled = true;

			private static readonly TimeSpan DefaultQuarantinedMailboxRetryInterval = TimeSpan.FromMinutes(5.0);

			private static readonly TimeSpan DefaultQueueGlithRetryInterval = TimeSpan.FromMinutes(1.0);

			private static readonly TimeSpan DefaultHighPriorityMessageExpirationTimeout = TimeSpan.FromHours(8.0);

			private static readonly TimeSpan DefaultNormalPriorityMessageExpirationTimeout = TimeSpan.FromDays(2.0);

			private static readonly TimeSpan DefaultLowPriorityMessageExpirationTimeout = TimeSpan.FromDays(2.0);

			private static readonly TimeSpan DefaultHighPriorityDelayNotificationTimeout = TimeSpan.FromMinutes(30.0);

			private static readonly TimeSpan DefaultNormalPriorityDelayNotificationTimeout = TimeSpan.FromHours(4.0);

			private static readonly TimeSpan DefaultLowPriorityDelayNotificationTimeout = TimeSpan.FromHours(8.0);

			private static readonly ByteQuantifiedSize DefaultMaxHighPriorityMessageSize = ByteQuantifiedSize.FromKB(250UL);

			private static readonly TimeSpan DefaultStoreDriverRecipientDeliveryHangThreshold = TimeSpan.FromMinutes(20.0);

			private static readonly TimeSpan DefaultStoreDriverDeliveryHangDetectionInterval = TimeSpan.FromMinutes(5.0);

			private static readonly TimeSpan DefaultDnsRequestTimeout = TimeSpan.FromMinutes(1.0);

			private static readonly TimeSpan DefaultDnsQueryRetryInterval = TimeSpan.FromSeconds(5.0);

			private static readonly TimeSpan DefaultResubmitDueToOutboundConnectorChangeInterval = TimeSpan.FromMinutes(15.0);

			private static readonly TimeSpan DefaultOutboundConnectorLookbackBufferInterval = TimeSpan.FromMinutes(2.0);

			private static readonly List<int> DefaultInternalMessageRetryIntervalRangeList = new List<int>
			{
				5,
				15,
				25,
				35,
				295,
				305,
				800,
				1000
			};

			private static readonly List<int> DefaultExternalMessageRetryIntervalRangeList = new List<int>
			{
				840,
				960,
				1020,
				1140,
				3540,
				3660
			};

			private int[] deliveryPriorityQuotas = new int[4];

			private TimeSpan maxIdleTimeBeforeResubmission = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxIdleTimeBeforeResubmit;

			private TimeSpan mailboxDeliveryQueueRetryInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDeliveryQueueRetryInterval;

			private TimeSpan mailboxDeliveryFastQueueRetryInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDeliveryFastQueueRetryInterval;

			private TimeSpan mailboxServerThreadLimitQueueRetryInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxServerThreadLimitQueueRetryInterval;

			private TimeSpan mailboxDatabaseThreadLimitQueueRetryInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDatabaseThreadLimitQueueRetryInterval;

			private int maxMailboxDeliveryPerMdbConnections = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnections;

			private int maxMailboxDeliveryPerMdbConnectionsHighHealthPercent = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsHighHealthPercent;

			private int maxMailboxDeliveryPerMdbConnectionsMediumHealthPercent = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent;

			private int maxMailboxDeliveryPerMdbConnectionsLowHealthPercent = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsLowHealthPercent;

			private int maxMailboxDeliveryPerMdbConnectionsLowestHealthPercent = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent;

			private bool dynamicMailboxDatabaseThrottlingEnabled = TransportAppConfig.RemoteDeliveryConfig.DefaultDynamicMailboxDatabaseThrottlingEnabled;

			private int mailboxDeliveryThrottlingLogBufferSize = TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogBufferSize;

			private TimeSpan mailboxDeliveryThrottlingLogFlushInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogFlushInterval;

			private TimeSpan mailboxDeliveryThrottlingLogAsyncLogInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogAsyncLogInterval;

			private TimeSpan mailboxDeliveryThrottlingLogSummaryLoggingInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultThrottlingLogSummaryLoggingInterval;

			private ByteQuantifiedSize maxMailboxDeliveryConcurrentMessageSizeLimit = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxMailboxDeliveryConcurrentMessageSizeLimit;

			private int maxStoreDriverDeliveryExceptionCallstackHistoryPerBucket = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxStoreDriverDeliveryExceptionCallstackHistoryPerBucket;

			private int maxStoreDriverDeliveryExceptionOccurrenceHistoryPerException = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxStoreDriverDeliveryExceptionOccurrenceHistoryPerException;

			private string storeDriverExceptionCallstackToTrap = TransportAppConfig.RemoteDeliveryConfig.DefaultStoreDriverExceptionCallstackToTrap;

			private int mdbHealthMediumToHighThreshold = TransportAppConfig.RemoteDeliveryConfig.DefaultMdbHealthMediumToHighThreshold;

			private int mdbHealthLowToMediumThreshold = TransportAppConfig.RemoteDeliveryConfig.DefaultMdbHealthLowToMediumThreshold;

			private bool mailboxDeliveryThrottlingEnabled = TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxDeliveryThrottlingEnabled;

			private bool mailboxTransportTableBasedExceptionHandlerEnabled = TransportAppConfig.RemoteDeliveryConfig.DefaultMailboxTransportTableBasedExceptionHandlerEnabled;

			private TimeSpan quarantinedMailboxRetryInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultQuarantinedMailboxRetryInterval;

			private TimeSpan queueGlitchRetryInterval = TransportAppConfig.RemoteDeliveryConfig.DefaultQueueGlithRetryInterval;

			private int queueGlitchRetryCount = 4;

			private int maxQueryResultCount = 50000;

			private DnsFaultTolerance dnsFaultTolerance;

			private int messageThresholdToUpdateHealthCounters = 100;

			private TimeSpan refreshIntervalToUpdateHealth = TransportAppConfig.RemoteDeliveryConfig.DefaultRefreshIntervalForUpdatingHealth;

			private bool loadBalancingForServerFailoverEnabled;

			private bool priorityQueuingEnabled;

			private bool localDeliveryPriorityQueuingEnabled;

			private bool remoteDeliveryPriorityQueuingEnabled;

			private int[] maxPerDomainPriorityConnections;

			private TimeSpan[] messageExpirationTimeout;

			private TimeSpan[] delayNotificationTimeout;

			private ByteQuantifiedSize maxHighPriorityMessageSize = TransportAppConfig.RemoteDeliveryConfig.DefaultMaxHighPriorityMessageSize;

			private int mailboxServerThreadLimit = 120;

			private int recipientThreadLimit = 3;

			private int deliverySourceThreadLimitPerCore = 5;

			private int mailboxDeliveryMaxMessagesPerConnection = 20;

			private TimeSpan storeDriverRecipientDeliveryHangThreshold;

			private TimeSpan storeDriverDeliveryHangDetectionInterval;

			private bool excludeDnsServersFromLoopbackAdapters;

			private bool excludeIPv6SiteLocalDnsAddresses;

			private TimeSpan dnsRequestTimeout;

			private TimeSpan dnsQueryRetryInterval;

			private TimeSpan configUpdateResubmitDeferInterval;

			private bool dnsIpv6Enabled;

			private int mailboxQueueMessageCountThresholdForConcurrentConnections;

			private int smtpConnectorQueueMessageCountThresholdForConcurrentConnections;

			private int intraorgSmtpQueueMessageCountThresholdForConcurrentConnections;

			private int otherQueueMessageCountThresholdForConcurrentConnections;

			private int deprioritizeOnRecipientThreadLimitExceededCount;

			private bool messageRetryIntervalProgressiveBackoffEnabled;

			private List<int> internalMessageRetryIntervalRangeList;

			private List<int> externalMessageRetryIntervalRangeList;

			private TimeSpan resubmitDueToOutboundConnectorChangeInterval;

			private TimeSpan outboundConnectorLookbackBufferInterval;

			private Random random = new Random();
		}

		public class MapiSubmissionConfig
		{
			private MapiSubmissionConfig()
			{
			}

			public int MaxConcurrentSubmissionsPerMailboxServer
			{
				get
				{
					return this.maxConcurrentSubmissionsPerMailboxServer;
				}
			}

			public int MaxConcurrentSubmissionsPerMailboxDatabase
			{
				get
				{
					return this.maxConcurrentSubmissionsPerMailboxDatabase;
				}
			}

			public int MaxStoreDriverSubmissionExceptionOccurrenceHistoryPerException
			{
				get
				{
					return this.maxStoreDriverSubmissionExceptionOccurrenceHistoryPerException;
				}
			}

			public int MaxStoreDriverSubmissionExceptionCallstackHistoryPerBucket
			{
				get
				{
					return this.maxStoreDriverSubmissionExceptionCallstackHistoryPerBucket;
				}
			}

			public static TransportAppConfig.MapiSubmissionConfig Load()
			{
				return new TransportAppConfig.MapiSubmissionConfig
				{
					maxConcurrentSubmissionsPerMailboxServer = TransportAppConfig.GetConfigInt("MaxConcurrentSubmissionsPerMailboxServer", 0, int.MaxValue, 12),
					maxConcurrentSubmissionsPerMailboxDatabase = TransportAppConfig.GetConfigInt("MaxConcurrentSubmissionsPerMailboxDatabase", 0, int.MaxValue, 4),
					maxStoreDriverSubmissionExceptionOccurrenceHistoryPerException = TransportAppConfig.GetConfigInt("MaxStoreDriverSubmissionExceptionOccurrenceHistoryPerException", 0, int.MaxValue, 10),
					maxStoreDriverSubmissionExceptionCallstackHistoryPerBucket = TransportAppConfig.GetConfigInt("MaxStoreDriverSubmissionExceptionCallstackHistoryPerBucket", 0, int.MaxValue, 10)
				};
			}

			private const int DefaultMaxConcurrentSubmissionsPerMailboxServer = 12;

			private const int DefaultMaxConcurrentSubmissionsPerMailboxDatabase = 4;

			private const int DefaultMaxStoreDriverSubmissionExceptionOccurrenceHistoryPerException = 10;

			private const int DefaultMaxStoreDriverSubmissionExceptionCallstackHistoryPerBucket = 20;

			private int maxConcurrentSubmissionsPerMailboxServer;

			private int maxConcurrentSubmissionsPerMailboxDatabase;

			private int maxStoreDriverSubmissionExceptionOccurrenceHistoryPerException = 10;

			private int maxStoreDriverSubmissionExceptionCallstackHistoryPerBucket = 20;
		}

		public class ResolverConfig
		{
			private ResolverConfig()
			{
			}

			public bool IsResolverEnabled
			{
				get
				{
					return this.isResolverEnabled;
				}
			}

			public int ExpansionSizeLimit
			{
				get
				{
					return this.expansionSizeLimit;
				}
			}

			public ResolverLogLevel ResolverLogLevel
			{
				get
				{
					return this.resolverLogLevel;
				}
			}

			public int BatchLookupRecipientCount
			{
				get
				{
					return this.batchLookupRecipientCount;
				}
			}

			public double ResolverRetryInterval
			{
				get
				{
					return this.resolverRetryInterval;
				}
			}

			public double DeliverMoveMailboxRetryInterval
			{
				get
				{
					return this.deliverMoveMailboxRetryInterval;
				}
			}

			public int MaxExecutingJobs
			{
				get
				{
					return this.maxExecutingJobs;
				}
			}

			public int MaxJobThreads
			{
				get
				{
					return this.maxJobThreads;
				}
			}

			public int MaxJobsPerThread
			{
				get
				{
					return this.maxJobsPerThread;
				}
			}

			public TimeSpan JobHealthTimeThreshold
			{
				get
				{
					return this.jobHealthTimeThreshold;
				}
			}

			public TimeSpan JobHealthUpdateInterval
			{
				get
				{
					return this.jobHealthUpdateInterval;
				}
			}

			public bool EnableForwardingProhibitedFeature
			{
				get
				{
					return this.enableForwardingProhibitedFeature;
				}
			}

			public int MaxResolveRecipientCacheSize
			{
				get
				{
					return this.maxResolveRecipientCacheSize;
				}
			}

			public int MaxResolverMemberOfGroupCacheSize
			{
				get
				{
					return this.maxResolverMemberOfGroupCacheSize;
				}
			}

			public TimeSpan AcceptedDomainReloadInterval
			{
				get
				{
					return this.acceptedDomainReloadInterval;
				}
			}

			public int AcceptedDomainReloadLoggingThreshold
			{
				get
				{
					return this.acceptedDomainReloadLoggingThreshold;
				}
			}

			public bool NDRForAmbiguousRecipients
			{
				get
				{
					return this.ndrForAmbiguousRecipients;
				}
			}

			public TimeSpan DeferralTimeForAmbiguousRecipients
			{
				get
				{
					return this.deferralTimeForAmbiguousRecipients;
				}
			}

			public bool LargeDGLimitEnforcementEnabled
			{
				get
				{
					return this.largeDGLimitEnforcementEnabled;
				}
			}

			public ByteQuantifiedSize LargeDGMaxMessageSize
			{
				get
				{
					return this.largeDGMaxMessageSize;
				}
			}

			public int LargeDGGroupCount
			{
				get
				{
					return this.largeDGGroupCount;
				}
			}

			public int LargeDGGroupCountForUnRestrictedDG
			{
				get
				{
					return this.largeDGGroupCountForUnRestrictedDG;
				}
			}

			public bool ForceNdrForDlRestrictionError
			{
				get
				{
					return this.forceNdrForDlRestrictionError;
				}
			}

			public static TransportAppConfig.ResolverConfig Load()
			{
				TransportAppConfig.ResolverConfig resolverConfig = new TransportAppConfig.ResolverConfig();
				resolverConfig.resolverRetryInterval = TransportAppConfig.GetConfigDouble("ResolverRetryInterval", 0.0, double.MaxValue, 30.0);
				resolverConfig.batchLookupRecipientCount = TransportAppConfig.GetConfigInt("BatchLookupRecipientCount", 0, 50, 20);
				resolverConfig.deliverMoveMailboxRetryInterval = TransportAppConfig.GetConfigDouble("DeliverMoveMailboxRetryInterval", 0.0, double.MaxValue, 2.0);
				resolverConfig.resolverLogLevel = TransportAppConfig.ResolverConfig.GetResolverLogLevel();
				resolverConfig.expansionSizeLimit = TransportAppConfig.GetConfigInt("ExpansionSizeLimit", 1, int.MaxValue, 1000);
				resolverConfig.enableForwardingProhibitedFeature = TransportAppConfig.GetConfigBool("EnableForwardingProhibitedFeature", false);
				resolverConfig.isResolverEnabled = TransportAppConfig.GetConfigBool("isResolverEnabled", false);
				int processorCount = Environment.ProcessorCount;
				resolverConfig.maxExecutingJobs = TransportAppConfig.GetConfigInt("MaxExecutingJobs", 1, 50, 6) * processorCount;
				resolverConfig.maxJobThreads = TransportAppConfig.GetConfigInt("MaxJobThreads", 1, 25, 3) * processorCount;
				resolverConfig.maxJobsPerThread = TransportAppConfig.GetConfigInt("MaxJobsPerThread", 1, int.MaxValue, 1);
				resolverConfig.jobHealthTimeThreshold = TransportAppConfig.GetConfigTimeSpan("JobHealthTimeThreshold", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(15.0));
				resolverConfig.jobHealthUpdateInterval = TransportAppConfig.GetConfigTimeSpan("JobHealthUpdateInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(10.0), TimeSpan.FromSeconds(30.0));
				resolverConfig.maxResolveRecipientCacheSize = TransportAppConfig.GetConfigInt("MaxResolveRecipientCacheSize", 0, int.MaxValue, 100000);
				resolverConfig.maxResolverMemberOfGroupCacheSize = TransportAppConfig.GetConfigInt("MaxResolverMemberOfGroupCacheSize", 0, int.MaxValue, 100000);
				resolverConfig.acceptedDomainReloadInterval = TransportAppConfig.GetConfigTimeSpan("AcceptedDomainForcedReloadInterval", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromMinutes(2.0));
				resolverConfig.acceptedDomainReloadLoggingThreshold = TransportAppConfig.GetConfigInt("AcceptedDomainForcedReloadLoggingThreshold", 0, int.MaxValue, 0);
				resolverConfig.ndrForAmbiguousRecipients = TransportAppConfig.GetConfigBool("NDRForAmbiguousRecipients", false);
				resolverConfig.deferralTimeForAmbiguousRecipients = TransportAppConfig.GetConfigTimeSpan("DeferralTimeForAmbiguousRecipients", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(6.0), TimeSpan.FromMinutes(30.0));
				resolverConfig.largeDGLimitEnforcementEnabled = TransportAppConfig.GetConfigBool("LargeDGLimitEnforcementEnabled", false);
				resolverConfig.forceNdrForDlRestrictionError = TransportAppConfig.GetConfigBool("ForceNdrForDlRestrictionError", true);
				if (resolverConfig.largeDGLimitEnforcementEnabled)
				{
					resolverConfig.largeDGMaxMessageSize = TransportAppConfig.GetConfigByteQuantifiedSize("LargeDGMaxMessageSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromMB(2UL));
					resolverConfig.largeDGGroupCount = TransportAppConfig.GetConfigInt("LargeDGGroupCount", 0, int.MaxValue, 5000);
					resolverConfig.largeDGGroupCountForUnRestrictedDG = TransportAppConfig.GetConfigInt("LargeDGGroupCountForUnRestrictedDG", 0, int.MaxValue, 5000);
				}
				return resolverConfig;
			}

			private static ResolverLogLevel GetResolverLogLevel()
			{
				string value = ConfigurationManager.AppSettings["ResolverLogLevel"];
				if (string.IsNullOrEmpty(value))
				{
					return ResolverLogLevel.Disabled;
				}
				ResolverLogLevel result;
				try
				{
					result = (ResolverLogLevel)Enum.Parse(typeof(ResolverLogLevel), value, true);
				}
				catch (ArgumentException)
				{
					return ResolverLogLevel.Disabled;
				}
				return result;
			}

			private const double DefaultRetryInterval = 30.0;

			private const double DefaultMoveMailboxRetryInterval = 2.0;

			private const int DefaultMaxResolveRecipientCacheSize = 100000;

			private const int DefaultMaxResolverMemberOfGroupCacheSize = 100000;

			private bool isResolverEnabled;

			private int expansionSizeLimit;

			private ResolverLogLevel resolverLogLevel;

			private int batchLookupRecipientCount;

			private double resolverRetryInterval = 30.0;

			private double deliverMoveMailboxRetryInterval = 30.0;

			private int maxExecutingJobs;

			private int maxJobThreads;

			private int maxJobsPerThread;

			private TimeSpan jobHealthTimeThreshold;

			private TimeSpan jobHealthUpdateInterval;

			private bool enableForwardingProhibitedFeature;

			private int maxResolveRecipientCacheSize = 100000;

			private int maxResolverMemberOfGroupCacheSize = 100000;

			private TimeSpan acceptedDomainReloadInterval;

			private int acceptedDomainReloadLoggingThreshold;

			private bool ndrForAmbiguousRecipients;

			private TimeSpan deferralTimeForAmbiguousRecipients;

			private ByteQuantifiedSize largeDGMaxMessageSize;

			private int largeDGGroupCount;

			private int largeDGGroupCountForUnRestrictedDG;

			private bool largeDGLimitEnforcementEnabled;

			private bool forceNdrForDlRestrictionError;
		}

		public class RoutingConfig
		{
			protected RoutingConfig()
			{
				this.configReloadInterval = TransportAppConfig.RoutingConfig.GetConfigReloadInterval();
				this.databaseFullReloadInterval = TransportAppConfig.RoutingConfig.GetDatabaseFullReloadInterval();
				this.deferredReloadInterval = TransportAppConfig.RoutingConfig.GetDeferredReloadInterval();
				this.maxDeferredNotifications = TransportAppConfig.RoutingConfig.GetMaxDeferredNotifications();
				this.minConfigReloadInterval = TransportAppConfig.GetConfigTimeSpan("MinRoutingConfigReloadInterval", TimeSpan.Zero, TimeSpan.FromHours(12.0), TimeSpan.FromMinutes(5.0));
				this.pfReplicaAgeThreshold = TransportAppConfig.RoutingConfig.GetPfReplicaAgeThreshold();
				this.disableExchangeServerAuth = TransportAppConfig.RoutingConfig.GetDisableExchangeServerAuth();
				this.localLoopMessageDeferralIntervals = TransportAppConfig.RoutingConfig.GetLocalLoopMessageDeferralIntervals();
				this.localLoopSubdomainDepth = TransportAppConfig.RoutingConfig.GetLocalLoopSubdomainDepth();
				this.localLoopDetectionEnabled = TransportAppConfig.RoutingConfig.GetLocalLoopDetectionEnabled();
				this.loopDetectionNumberOfTransits = TransportAppConfig.RoutingConfig.GetLoopDetectionNumberOfTransits();
				this.localLoopDetectionSubDomainLeftToRightOffsetForPerfCounter = TransportAppConfig.RoutingConfig.GetLocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter();
				this.maxAllowedCategorizerResubmits = TransportAppConfig.GetConfigInt("MaxAllowedCategorizerResubmits", 2, int.MaxValue, 1000);
				this.destinationRoutingToRemoteSitesEnabled = TransportAppConfig.GetConfigBool("DestinationRoutingToRemoteSitesEnabled", true);
				this.dagRoutingEnabled = TransportAppConfig.GetConfigBool("DagRoutingEnabled", true);
				this.smtpDeliveryToMailboxEnabled = TransportAppConfig.GetConfigBool("SmtpDeliveryToMailboxEnabled", true);
				this.proxyRoutingAllowedTargetVersions = TransportAppConfig.GetConfigList<int>("ProxyRoutingAllowedTargetVersions", ',', new TransportAppConfig.TryParse<int>(int.TryParse));
				this.proxyRoutingMaxTotalHubCount = TransportAppConfig.GetConfigInt("ProxyRoutingMaxTotalHubCount", 1, int.MaxValue, 10);
				this.proxyRoutingMaxRemoteSiteHubCount = TransportAppConfig.GetConfigInt("ProxyRoutingMaxRemoteSiteHubCount", 0, int.MaxValue, 4);
				this.proxyRoutingServerSelectStrategy = TransportAppConfig.GetConfigEnum<RoutedServerSelectStrategy>("ProxyRoutingServerSelectStrategy", RoutedServerSelectStrategy.FavorCloserProximity);
				this.outboundFrontendServers = TransportAppConfig.GetConfigList<RoutingHost>("OutboundFrontendServers", ',', new TransportAppConfig.TryParse<RoutingHost>(RoutingHost.TryParse));
				this.externalOutboundFrontendProxyEnabled = TransportAppConfig.GetConfigBool("ExternalOutboundFrontendProxyEnabled", false);
				this.outboundProxyRoutingXVersionEnabled = TransportAppConfig.GetConfigBool("OutboundProxyRoutingXVersionEnabled", true);
				this.routingToNonActiveServersEnabled = TransportAppConfig.GetConfigBool("RoutingToNonActiveServersEnabled", false);
				this.randomLoadBalancingOffsetEnabled = TransportAppConfig.GetConfigBool("RandomLoadBalancingOffsetEnabled", true);
				this.maxDeferCountForRecipientHasNoMdb = TransportAppConfig.GetConfigInt("MaxDeferCountForRecipientHasNoMdb", 0, 1000, 2);
				this.routingTopologyCacheEnabled = TransportAppConfig.GetConfigBool("RoutingTopologyCacheEnabled", true);
				this.deferralTimeForNoMdb = TransportAppConfig.RoutingConfig.GetDeferralTimeForNoMdb();
				this.dagSelectorEnabled = TransportAppConfig.GetConfigBool("DagSelectorEnabled", false);
				this.dagSelectorIncrementMessageThresholdFactor = TransportAppConfig.GetConfigDouble("DagSelectorIncrementMessageThresholdFactor", 0.0, 1.0, 0.25);
				this.dagSelectorMessageThresholdPerServer = TransportAppConfig.GetConfigInt("DagSelectorMessageThresholdPerServer", 1, int.MaxValue, 125);
				this.dagSelectorActiveServersForDagToBeRoutable = TransportAppConfig.GetConfigInt("DagSelectorActiveServersForDagToBeRoutable", 1, 16, 2);
				this.dagSelectorMinimumSitesForDagToBeRoutable = TransportAppConfig.GetConfigInt("DagSelectorMinimumSitesForDagToBeRoutable", 1, 4, 2);
				this.tenantDagQuotaMessageThreshold = TransportAppConfig.GetConfigInt("TenantDagQuotaMessageThreshold", 1, int.MaxValue, 50);
				this.tenantDagQuotaDagsPerTenant = TransportAppConfig.GetConfigInt("TenantDagQuotaDagsPerTenant", 1, int.MaxValue, 3);
				this.tenantDagQuotaPastWeight = TransportAppConfig.GetConfigDouble("TenantDagQuotaPastWeight", 0.0, 1.0, 0.5);
				this.checkDagSelectorHeader = TransportAppConfig.GetConfigBool("CheckDagSelectorHeaderEnabled", false);
				this.logDagSelectorDiagnosticInfo = TransportAppConfig.GetConfigBool("DagSelectorLogDiagnosticInfo", true);
			}

			public bool DagSelectorEnabled
			{
				get
				{
					return this.dagSelectorEnabled && this.dagRoutingEnabled;
				}
			}

			public int DagSelectorMessageThresholdPerServer
			{
				get
				{
					return this.dagSelectorMessageThresholdPerServer;
				}
			}

			public int DagSelectorActiveServersForDagToBeRoutable
			{
				get
				{
					return this.dagSelectorActiveServersForDagToBeRoutable;
				}
			}

			public int DagSelectorMinimumSitesForDagToBeRoutable
			{
				get
				{
					return this.dagSelectorMinimumSitesForDagToBeRoutable;
				}
			}

			public double DagSelectorIncrementMessageThresholdFactor
			{
				get
				{
					return this.dagSelectorIncrementMessageThresholdFactor;
				}
			}

			public int TenantDagQuotaMessagesPerDag
			{
				get
				{
					return this.tenantDagQuotaMessageThreshold;
				}
			}

			public int TenantDagQuotaDagsPerTenant
			{
				get
				{
					return this.tenantDagQuotaDagsPerTenant;
				}
			}

			public double TenantDagQuotaPastWeight
			{
				get
				{
					return this.tenantDagQuotaPastWeight;
				}
			}

			public bool CheckDagSelectorHeader
			{
				get
				{
					return this.checkDagSelectorHeader;
				}
			}

			public bool DagSelectorLogDiagnosticInfo
			{
				get
				{
					return this.logDagSelectorDiagnosticInfo;
				}
			}

			public TimeSpan ConfigReloadInterval
			{
				get
				{
					return this.configReloadInterval;
				}
			}

			public TimeSpan DatabaseFullReloadInterval
			{
				get
				{
					return this.databaseFullReloadInterval;
				}
			}

			public TimeSpan DeferredReloadInterval
			{
				get
				{
					return this.deferredReloadInterval;
				}
			}

			public TimeSpan MinConfigReloadInterval
			{
				get
				{
					return this.minConfigReloadInterval;
				}
			}

			public int MaxDeferredNotifications
			{
				get
				{
					return this.maxDeferredNotifications;
				}
			}

			public int LoopDetectionNumberOfTransits
			{
				get
				{
					return this.loopDetectionNumberOfTransits;
				}
			}

			public int LocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter
			{
				get
				{
					return this.localLoopDetectionSubDomainLeftToRightOffsetForPerfCounter;
				}
			}

			public TimeSpan PfReplicaAgeThreshold
			{
				get
				{
					return this.pfReplicaAgeThreshold;
				}
			}

			public int[] LocalLoopMessageDeferralIntervals
			{
				get
				{
					return this.localLoopMessageDeferralIntervals;
				}
			}

			public int LocalLoopSubdomainDepth
			{
				get
				{
					return this.localLoopSubdomainDepth;
				}
			}

			public bool LocalLoopDetectionEnabled
			{
				get
				{
					return this.localLoopDetectionEnabled;
				}
			}

			public int MaxAllowedCategorizerResubmits
			{
				get
				{
					return this.maxAllowedCategorizerResubmits;
				}
			}

			public bool DisableExchangeServerAuth
			{
				get
				{
					return this.disableExchangeServerAuth;
				}
			}

			public int MaxDeferCountForRecipientHasNoMdb
			{
				get
				{
					return this.maxDeferCountForRecipientHasNoMdb;
				}
			}

			public TimeSpan DeferralTimeForNoMdb
			{
				get
				{
					return this.deferralTimeForNoMdb;
				}
			}

			public bool DestinationRoutingToRemoteSitesEnabled
			{
				get
				{
					return this.destinationRoutingToRemoteSitesEnabled;
				}
			}

			public bool DagRoutingEnabled
			{
				get
				{
					return this.dagRoutingEnabled;
				}
				protected set
				{
					this.dagRoutingEnabled = value;
				}
			}

			public bool SmtpDeliveryToMailboxEnabled
			{
				get
				{
					return this.smtpDeliveryToMailboxEnabled;
				}
			}

			public IList<int> ProxyRoutingAllowedTargetVersions
			{
				get
				{
					return this.proxyRoutingAllowedTargetVersions;
				}
				protected set
				{
					this.proxyRoutingAllowedTargetVersions = value;
				}
			}

			public int ProxyRoutingMaxTotalHubCount
			{
				get
				{
					return this.proxyRoutingMaxTotalHubCount;
				}
				protected set
				{
					this.proxyRoutingMaxTotalHubCount = value;
				}
			}

			public int ProxyRoutingMaxRemoteSiteHubCount
			{
				get
				{
					return this.proxyRoutingMaxRemoteSiteHubCount;
				}
				protected set
				{
					this.proxyRoutingMaxRemoteSiteHubCount = value;
				}
			}

			public RoutedServerSelectStrategy ProxyRoutingServerSelectStrategy
			{
				get
				{
					return this.proxyRoutingServerSelectStrategy;
				}
				protected set
				{
					this.proxyRoutingServerSelectStrategy = value;
				}
			}

			public IList<RoutingHost> OutboundFrontendServers
			{
				get
				{
					return this.outboundFrontendServers;
				}
				protected set
				{
					this.outboundFrontendServers = value;
				}
			}

			public bool ExternalOutboundFrontendProxyEnabled
			{
				get
				{
					return this.externalOutboundFrontendProxyEnabled;
				}
				protected set
				{
					this.externalOutboundFrontendProxyEnabled = value;
				}
			}

			public bool OutboundProxyRoutingXVersionEnabled
			{
				get
				{
					return this.outboundProxyRoutingXVersionEnabled;
				}
				protected set
				{
					this.outboundProxyRoutingXVersionEnabled = value;
				}
			}

			public bool RoutingToNonActiveServersEnabled
			{
				get
				{
					return this.routingToNonActiveServersEnabled;
				}
				protected set
				{
					this.routingToNonActiveServersEnabled = value;
				}
			}

			public bool RandomLoadBalancingOffsetEnabled
			{
				get
				{
					return this.randomLoadBalancingOffsetEnabled;
				}
				protected set
				{
					this.randomLoadBalancingOffsetEnabled = value;
				}
			}

			public bool RoutingTopologyCacheEnabled
			{
				get
				{
					return this.routingTopologyCacheEnabled;
				}
				internal set
				{
					this.routingTopologyCacheEnabled = value;
				}
			}

			public static TransportAppConfig.RoutingConfig Load()
			{
				return new TransportAppConfig.RoutingConfig();
			}

			private static TimeSpan GetConfigReloadInterval()
			{
				return TransportAppConfig.GetConfigTimeSpan("RoutingConfigReloadInterval", TimeSpan.FromSeconds(30.0), TimeSpan.MaxValue, TimeSpan.FromHours(12.0));
			}

			private static TimeSpan GetDatabaseFullReloadInterval()
			{
				return TransportAppConfig.GetConfigTimeSpan("RoutingConfigDatabaseFullReloadInterval", TimeSpan.FromSeconds(0.0), TimeSpan.MaxValue, TimeSpan.FromHours(4.0));
			}

			private static TimeSpan GetDeferredReloadInterval()
			{
				int configInt = TransportAppConfig.GetConfigInt("DeferredReloadTimeoutSeconds", 0, int.MaxValue, 5);
				if (configInt != 2147483647)
				{
					return TimeSpan.FromSeconds((double)configInt);
				}
				return TimeSpan.MaxValue;
			}

			private static int GetMaxDeferredNotifications()
			{
				string text = ConfigurationManager.AppSettings["MaxDeferredNotifications"];
				int num;
				if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num) || num <= 0)
				{
					return 20;
				}
				return num;
			}

			private static int GetLoopDetectionNumberOfTransits()
			{
				return TransportAppConfig.GetConfigInt("LoopDetectionNumberOfTransits", 1, int.MaxValue, 4);
			}

			private static int GetLocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter()
			{
				return TransportAppConfig.GetConfigInt("LocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter", 0, int.MaxValue, 0);
			}

			private static bool GetDisableExchangeServerAuth()
			{
				string value = ConfigurationManager.AppSettings["DisableExchangeServerAuth"];
				bool result = false;
				if (!string.IsNullOrEmpty(value))
				{
					bool.TryParse(value, out result);
				}
				return result;
			}

			private static TimeSpan GetPfReplicaAgeThreshold()
			{
				return TransportAppConfig.GetConfigTimeSpan("PFReplicaAgeThreshold", TimeSpan.FromSeconds(0.0), TimeSpan.MaxValue, TransportAppConfig.RoutingConfig.DefaultPfReplicaAgeThreshold);
			}

			private static int[] GetLocalLoopMessageDeferralIntervals()
			{
				int[] array = TransportAppConfig.GetConfigIntList("LocalLoopMessageDeferralIntervals", 0, int.MaxValue, 0, ',').ToArray();
				if (array == null || array.Length == 0)
				{
					array = TransportAppConfig.RoutingConfig.DefaultLocalLoopMessageDeferralIntervals;
				}
				return array;
			}

			private static int GetLocalLoopSubdomainDepth()
			{
				return TransportAppConfig.GetConfigInt("LocalLoopSubdomainDepth", 0, 10, 0);
			}

			private static bool GetLocalLoopDetectionEnabled()
			{
				return TransportAppConfig.GetConfigBool("LocalLoopDetectionEnabled", true);
			}

			private static TimeSpan GetDeferralTimeForNoMdb()
			{
				return TransportAppConfig.GetConfigTimeSpan("DeferralTimeForNoMdb", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(6.0), TimeSpan.FromMinutes(15.0));
			}

			public const string OutboundFrontendServersLabel = "OutboundFrontendServers";

			public const char OutboundFrontendServersDelimiter = ',';

			private const int DefaultMaxDeferredNotifications = 20;

			private const int DefaultLocalLoopSubdomainDepth = 0;

			private const int DefaultLoopDetectionNumberOfTransits = 4;

			private const int DefaultLocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter = 0;

			private const int MaxLocalLoopSubdomainDepth = 10;

			private const bool DefaultLocalLoopDetectionEnabled = true;

			private static readonly TimeSpan DefaultPfReplicaAgeThreshold = TimeSpan.FromHours(48.0);

			private static readonly int[] DefaultLocalLoopMessageDeferralIntervals = new int[]
			{
				0,
				0,
				30,
				30,
				900,
				900,
				900
			};

			private readonly TimeSpan configReloadInterval;

			private readonly TimeSpan databaseFullReloadInterval;

			private readonly TimeSpan deferredReloadInterval;

			private readonly int maxDeferredNotifications;

			private readonly int loopDetectionNumberOfTransits;

			private readonly int localLoopDetectionSubDomainLeftToRightOffsetForPerfCounter;

			private readonly TimeSpan minConfigReloadInterval;

			private readonly TimeSpan pfReplicaAgeThreshold = TransportAppConfig.RoutingConfig.DefaultPfReplicaAgeThreshold;

			private readonly int[] localLoopMessageDeferralIntervals = TransportAppConfig.RoutingConfig.DefaultLocalLoopMessageDeferralIntervals;

			private readonly int localLoopSubdomainDepth;

			private readonly bool localLoopDetectionEnabled = true;

			private readonly int maxAllowedCategorizerResubmits;

			private readonly bool disableExchangeServerAuth;

			private readonly int maxDeferCountForRecipientHasNoMdb;

			private readonly TimeSpan deferralTimeForNoMdb;

			private readonly bool destinationRoutingToRemoteSitesEnabled;

			private readonly bool smtpDeliveryToMailboxEnabled;

			private bool dagRoutingEnabled;

			private IList<int> proxyRoutingAllowedTargetVersions;

			private int proxyRoutingMaxTotalHubCount;

			private int proxyRoutingMaxRemoteSiteHubCount;

			private RoutedServerSelectStrategy proxyRoutingServerSelectStrategy;

			private IList<RoutingHost> outboundFrontendServers;

			private bool externalOutboundFrontendProxyEnabled;

			private bool outboundProxyRoutingXVersionEnabled;

			private bool routingToNonActiveServersEnabled;

			private bool randomLoadBalancingOffsetEnabled;

			private readonly bool dagSelectorEnabled;

			private readonly int dagSelectorMessageThresholdPerServer;

			private readonly double dagSelectorIncrementMessageThresholdFactor;

			private readonly int dagSelectorActiveServersForDagToBeRoutable;

			private readonly int dagSelectorMinimumSitesForDagToBeRoutable;

			private readonly int tenantDagQuotaMessageThreshold;

			private readonly int tenantDagQuotaDagsPerTenant;

			private readonly double tenantDagQuotaPastWeight;

			private readonly bool checkDagSelectorHeader;

			private readonly bool logDagSelectorDiagnosticInfo;

			private bool routingTopologyCacheEnabled;
		}

		public sealed class ContentConversionConfig
		{
			private ContentConversionConfig()
			{
			}

			public ByteEncoderTypeFor7BitCharsets ByteEncoderTypeFor7BitCharsets
			{
				get
				{
					return this.byteEncoderTypeFor7BitCharsets;
				}
			}

			public bool TreatInlineDispositionAsAttachment
			{
				get
				{
					return this.treatInlineDispositionAsAttachment;
				}
			}

			public bool QuoteDisplayNameBeforeRfc2047Encoding
			{
				get
				{
					return this.quoteDisplayNameBeforeRfc2047Encoding;
				}
			}

			public int PreferredInternetCodePageForShiftJis
			{
				get
				{
					return this.preferredInternetCodePageForShiftJis;
				}
			}

			public int RequiredCharsetCoverage
			{
				get
				{
					return this.requiredCharsetCoverage;
				}
			}

			public static TransportAppConfig.ContentConversionConfig Load()
			{
				TransportAppConfig.ContentConversionConfig contentConversionConfig = new TransportAppConfig.ContentConversionConfig();
				contentConversionConfig.byteEncoderTypeFor7BitCharsets = TransportAppConfig.ContentConversionConfig.GetByteEncoderTypeFor7BitCharsets();
				string text = ConfigurationManager.AppSettings["PreferredInternetCodePageForShiftJis"];
				int num;
				if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num))
				{
					contentConversionConfig.preferredInternetCodePageForShiftJis = num;
				}
				text = ConfigurationManager.AppSettings["RequiredCharsetCoverage"];
				if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num) && num >= 0 && num <= 100)
				{
					contentConversionConfig.requiredCharsetCoverage = num;
				}
				contentConversionConfig.treatInlineDispositionAsAttachment = TransportAppConfig.ContentConversionConfig.GetTreatInlineDispositionAsAttachment();
				contentConversionConfig.quoteDisplayNameBeforeRfc2047Encoding = TransportAppConfig.ContentConversionConfig.GetQuoteDisplayNameBeforeRfc2047Encoding();
				return contentConversionConfig;
			}

			private static ByteEncoderTypeFor7BitCharsets GetByteEncoderTypeFor7BitCharsets()
			{
				return TransportAppConfig.GetConfigEnum<ByteEncoderTypeFor7BitCharsets>("ByteEncoderTypeFor7BitCharsets", ByteEncoderTypeFor7BitCharsets.UseQP, EnumParseOptions.AllowNumericConstants | EnumParseOptions.IgnoreCase);
			}

			private static bool GetTreatInlineDispositionAsAttachment()
			{
				return TransportAppConfig.GetConfigBool("TreatInlineDispositionAsAttachment", false);
			}

			private static bool GetQuoteDisplayNameBeforeRfc2047Encoding()
			{
				return TransportAppConfig.GetConfigBool("QuoteDisplayNameBeforeRfc2047Encoding", false);
			}

			private const ByteEncoderTypeFor7BitCharsets DefaultByteEncoderTypeFor7BitCharsets = ByteEncoderTypeFor7BitCharsets.UseQP;

			private const bool DefaultTreatInlineDispositionAsAttachment = false;

			private const bool DefaultQuoteDisplayNameBeforeRfc2047Encoding = false;

			private ByteEncoderTypeFor7BitCharsets byteEncoderTypeFor7BitCharsets;

			private int preferredInternetCodePageForShiftJis = 50222;

			private bool quoteDisplayNameBeforeRfc2047Encoding;

			private int requiredCharsetCoverage = 100;

			private bool treatInlineDispositionAsAttachment;
		}

		public sealed class IPFilteringDatabaseConfig
		{
			private IPFilteringDatabaseConfig()
			{
			}

			public string DatabasePath
			{
				get
				{
					return this.databasePath;
				}
			}

			public string LogFilePath
			{
				get
				{
					return this.logFilePath;
				}
			}

			public uint LogFileSize
			{
				get
				{
					return (uint)this.logFileSize.ToBytes();
				}
			}

			public uint LogBufferSize
			{
				get
				{
					return (uint)this.logBufferSize.ToBytes();
				}
			}

			public static TransportAppConfig.IPFilteringDatabaseConfig Load()
			{
				TransportAppConfig.IPFilteringDatabaseConfig ipfilteringDatabaseConfig = new TransportAppConfig.IPFilteringDatabaseConfig();
				string value = ConfigurationManager.AppSettings["IPFilterDatabasePath"];
				if (!string.IsNullOrEmpty(value))
				{
					ipfilteringDatabaseConfig.databasePath = value;
				}
				string value2 = ConfigurationManager.AppSettings["IPFilterDatabaseLoggingPath"];
				if (!string.IsNullOrEmpty(value2))
				{
					ipfilteringDatabaseConfig.logFilePath = value2;
				}
				ipfilteringDatabaseConfig.logFileSize = TransportAppConfig.GetConfigByteQuantifiedSize("IPFilterDatabaseLoggingFileSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromKB(512UL));
				ipfilteringDatabaseConfig.logBufferSize = TransportAppConfig.GetConfigByteQuantifiedSize("IPFilterDatabaseLoggingBufferSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromKB(5UL));
				return ipfilteringDatabaseConfig;
			}

			private string databasePath;

			private string logFilePath;

			private ByteQuantifiedSize logFileSize;

			private ByteQuantifiedSize logBufferSize;
		}

		public sealed class MessageResubmissionConfig : TransportAppConfig.IMessageResubmissionConfig
		{
			private MessageResubmissionConfig()
			{
			}

			public bool MessageResubmissionEnabled { get; private set; }

			public TimeSpan ResubmissionInterval { get; private set; }

			public TimeSpan ResubmissionInitialDelay { get; private set; }

			public int ResubmissionPageSize { get; private set; }

			public int MaxOutstandingResubmissionMessages { get; private set; }

			public int MaxResubmissionRequests { get; private set; }

			public TimeSpan ResubmitRequestExpiryPeriod { get; private set; }

			public TimeSpan TestResubmitRequestExpiryPeriod { get; private set; }

			public int MaxRecentResubmissionRequests { get; private set; }

			public TimeSpan RecentResubmitRequestPeriod { get; private set; }

			public static TransportAppConfig.IMessageResubmissionConfig Load()
			{
				TransportAppConfig.MessageResubmissionConfig messageResubmissionConfig = new TransportAppConfig.MessageResubmissionConfig();
				messageResubmissionConfig.MessageResubmissionEnabled = TransportAppConfig.GetConfigBool("MessageResubmissionEnabled", true);
				messageResubmissionConfig.ResubmissionInterval = TransportAppConfig.GetConfigTimeSpan("MessageResubmissionInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(2.0));
				messageResubmissionConfig.ResubmissionInitialDelay = TransportAppConfig.GetConfigTimeSpan("MessageResubmissionInitialDelay", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromSeconds(30.0));
				messageResubmissionConfig.MaxOutstandingResubmissionMessages = TransportAppConfig.GetConfigInt("MessageResubmissionMaxOutstandingMessages", 1, int.MaxValue, 90);
				messageResubmissionConfig.ResubmissionPageSize = TransportAppConfig.GetConfigInt("MessageResubmissionPageSize", 1, messageResubmissionConfig.MaxOutstandingResubmissionMessages, 30);
				messageResubmissionConfig.MaxResubmissionRequests = TransportAppConfig.GetConfigInt("MaxResubmissionRequests", 0, int.MaxValue, 100);
				messageResubmissionConfig.MaxRecentResubmissionRequests = TransportAppConfig.GetConfigInt("MaxRecentResubmissionRequests", 0, int.MaxValue, messageResubmissionConfig.MaxResubmissionRequests);
				messageResubmissionConfig.RecentResubmitRequestPeriod = TransportAppConfig.GetConfigTimeSpan("RecentResubmitRequestPeriod", TimeSpan.FromMinutes(1.0), TimeSpan.MaxValue, TimeSpan.FromHours(1.0));
				messageResubmissionConfig.ResubmitRequestExpiryPeriod = TransportAppConfig.GetConfigTimeSpan("ResubmitRequestExpiryPeriod", TimeSpan.FromMinutes(1.0), TimeSpan.MaxValue, TimeSpan.FromDays(3.0));
				messageResubmissionConfig.TestResubmitRequestExpiryPeriod = TransportAppConfig.GetConfigTimeSpan("TestResubmitRequestExpiryPeriod", TimeSpan.FromMinutes(1.0), TimeSpan.MaxValue, TimeSpan.FromHours(3.0));
				return messageResubmissionConfig;
			}
		}

		public interface IProcessingQuotaConfig
		{
			bool EnforceProcessingQuota { get; }

			bool TestProcessingQuota { get; }

			TimeSpan UpdateThrottlingDataInterval { get; }

			int HighWatermarkCpuPercentage { get; }

			int LowWatermarkCpuPercentage { get; }

			string ThrottleDataFilePath { get; }
		}

		public class ProcessingQuotaConfig : TransportAppConfig.IProcessingQuotaConfig
		{
			public static TransportAppConfig.ProcessingQuotaConfig Load()
			{
				return new TransportAppConfig.ProcessingQuotaConfig
				{
					EnforceProcessingQuota = (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.EnforceProcessingQuota.Enabled && TransportAppConfig.GetConfigBool("EnforceProcessingQuota", false)),
					TestProcessingQuota = (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.TestProcessingQuota.Enabled && TransportAppConfig.GetConfigBool("TestProcessingQuota", false)),
					UpdateThrottlingDataInterval = TransportAppConfig.GetConfigTimeSpan("ProcessingQuotaDataUpdateInterval", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(10.0)),
					HighWatermarkCpuPercentage = TransportAppConfig.GetConfigInt("HighWatermarkProcessingQuotaTriggerPercentage", 0, 100, 60),
					LowWatermarkCpuPercentage = TransportAppConfig.GetConfigInt("LowWatermarkProcessingQuotaTriggerPercentage", 0, 100, 40),
					ThrottleDataFilePath = TransportAppConfig.GetConfigString("ProcessingQuotaDataFilePath", string.Empty)
				};
			}

			public bool EnforceProcessingQuota { get; private set; }

			public bool TestProcessingQuota { get; private set; }

			public TimeSpan UpdateThrottlingDataInterval { get; private set; }

			public int HighWatermarkCpuPercentage { get; private set; }

			public int LowWatermarkCpuPercentage { get; private set; }

			public string ThrottleDataFilePath { get; private set; }
		}

		public class QueueDatabaseConfig : IMessagingDatabaseConfig
		{
			private QueueDatabaseConfig()
			{
			}

			public string DatabasePath
			{
				get
				{
					return this.databasePath;
				}
			}

			public string LogFilePath
			{
				get
				{
					return this.logFilePath;
				}
			}

			public uint LogFileSize
			{
				get
				{
					return (uint)this.logFileSize.ToBytes();
				}
			}

			public uint LogBufferSize
			{
				get
				{
					return (uint)this.logBufferSize.ToBytes();
				}
			}

			public uint ExtensionSize
			{
				get
				{
					return (uint)this.extensionSize.ToBytes();
				}
			}

			public uint MaxBackgroundCleanupTasks
			{
				get
				{
					return this.maxBackgroundCleanupTasks;
				}
			}

			public int MaxConnections
			{
				get
				{
					return this.maxConnections;
				}
			}

			public DatabaseRecoveryAction DatabaseRecoveryAction
			{
				get
				{
					return this.databaseRecoveryAction;
				}
			}

			public TimeSpan MessagingGenerationCleanupAge
			{
				get
				{
					if (this.safetyNetHoldTime == null)
					{
						return this.transportConfiguration.TransportSettings.TransportSettings.SafetyNetHoldTime;
					}
					return this.safetyNetHoldTime.Value;
				}
			}

			public TimeSpan MessagingGenerationExpirationAge
			{
				get
				{
					if (this.transportConfiguration == null || this.transportConfiguration.LocalServer == null)
					{
						return TimeSpan.MaxValue;
					}
					return this.MessagingGenerationCleanupAge + this.transportConfiguration.LocalServer.TransportServer.MessageExpirationTimeout;
				}
			}

			public TimeSpan MessagingGenerationLength
			{
				get
				{
					return this.messagingGenerationLength;
				}
			}

			public TimeSpan DefaultAsyncCommitTimeout
			{
				get
				{
					return this.defaultAsyncCommitTimeout;
				}
			}

			public byte MaxMessageLoadTimePercentage
			{
				get
				{
					return this.maxMessageLoadTimePercentage;
				}
			}

			public TimeSpan StatisticsUpdateInterval
			{
				get
				{
					return this.statisticsUpdateInterval;
				}
			}

			public bool CloneInOriginalGeneration
			{
				get
				{
					return this.cloneInOriginalGeneration;
				}
			}

			private static string GetQueueDBPath(string appConfigKey, string defaultValue)
			{
				string text = Path.Combine("D:\\Queue", "mail.que");
				if (!string.IsNullOrEmpty(text) && File.Exists(text))
				{
					return "D:\\Queue";
				}
				string path = ConfigurationContext.Setup.IsUnpacked ? ConfigurationContext.Setup.InstallPath : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				string configString = TransportAppConfig.GetConfigString(appConfigKey, defaultValue);
				if (!Path.IsPathRooted(configString))
				{
					return Path.Combine(path, configString);
				}
				return configString;
			}

			public static TransportAppConfig.QueueDatabaseConfig Load()
			{
				TransportAppConfig.QueueDatabaseConfig queueDatabaseConfig = new TransportAppConfig.QueueDatabaseConfig();
				queueDatabaseConfig.databasePath = TransportAppConfig.QueueDatabaseConfig.GetQueueDBPath("QueueDatabasePath", "TransportRoles\\data\\Queue\\");
				queueDatabaseConfig.logFilePath = TransportAppConfig.QueueDatabaseConfig.GetQueueDBPath("QueueDatabaseLoggingPath", queueDatabaseConfig.databasePath);
				queueDatabaseConfig.maxConnections = TransportAppConfig.GetConfigInt("QueueDatabaseMaxConnections", 1, int.MaxValue, 4);
				queueDatabaseConfig.logFileSize = TransportAppConfig.GetConfigByteQuantifiedSize("QueueDatabaseLoggingFileSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromMB(5UL));
				queueDatabaseConfig.logBufferSize = TransportAppConfig.GetConfigByteQuantifiedSize("QueueDatabaseLoggingBufferSize", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromMB(5UL));
				queueDatabaseConfig.extensionSize = TransportAppConfig.GetConfigByteQuantifiedSize("QueueDatabaseExtensionSize", ByteQuantifiedSize.FromMB(1UL), ByteQuantifiedSize.FromBytes((ulong)-1), ByteQuantifiedSize.FromMB(256UL));
				queueDatabaseConfig.maxBackgroundCleanupTasks = (uint)TransportAppConfig.GetConfigInt("QueueDatabaseMaxBackgroundCleanupTasks", 0, int.MaxValue, 32);
				queueDatabaseConfig.databaseRecoveryAction = TransportAppConfig.GetConfigEnum<DatabaseRecoveryAction>("QueueDatabaseRecoveryAction", DatabaseRecoveryAction.Move);
				queueDatabaseConfig.messagingGenerationLength = TransportAppConfig.GetConfigTimeSpan("QueueDatabaseGenerationLength", TimeSpan.FromSeconds(10.0), TimeSpan.FromDays(10.0), TimeSpan.FromHours(1.0));
				queueDatabaseConfig.safetyNetHoldTime = TransportAppConfig.GetConfigNullableTimeSpan("SafetyNetHoldTimeInterval", TimeSpan.FromSeconds(15.0), TimeSpan.FromSeconds(2147483647.0));
				queueDatabaseConfig.defaultAsyncCommitTimeout = TransportAppConfig.GetConfigTimeSpan("QueueDatabaseAsyncCommitTimeout", TimeSpan.FromMilliseconds(25.0), TimeSpan.FromSeconds(60.0), TimeSpan.FromMilliseconds(100.0));
				queueDatabaseConfig.maxMessageLoadTimePercentage = (byte)TransportAppConfig.GetConfigInt("MaxMessageLoadTimePercentage", 1, 100, 75);
				queueDatabaseConfig.recentGenerationDepth = TransportAppConfig.GetConfigInt("RecentGenerationDepth", 0, 24, 1);
				queueDatabaseConfig.statisticsUpdateInterval = TransportAppConfig.GetConfigTimeSpan("StatisticsUpdateInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(20.0), TimeSpan.FromMinutes(5.0));
				queueDatabaseConfig.cloneInOriginalGeneration = TransportAppConfig.GetConfigBool("CloneInOriginalGeneration", true);
				return queueDatabaseConfig;
			}

			public void SetLoadTimeDependencies(ITransportConfiguration transportConfiguration)
			{
				this.transportConfiguration = transportConfiguration;
			}

			public int RecentGenerationDepth
			{
				get
				{
					return this.recentGenerationDepth;
				}
			}

			private const string DefaultDatabaseFileName = "mail.que";

			private const string DefaultDatacenterDatabasePath = "D:\\Queue";

			private const DatabaseRecoveryAction DefaultDatabaseRecoveryAction = DatabaseRecoveryAction.Move;

			private string databasePath;

			private string logFilePath;

			private ByteQuantifiedSize logFileSize;

			private ByteQuantifiedSize logBufferSize;

			private uint maxBackgroundCleanupTasks;

			private int maxConnections;

			private DatabaseRecoveryAction databaseRecoveryAction;

			private TimeSpan? safetyNetHoldTime;

			private TimeSpan messagingGenerationLength;

			private TimeSpan defaultAsyncCommitTimeout;

			private byte maxMessageLoadTimePercentage;

			private ITransportConfiguration transportConfiguration;

			private ByteQuantifiedSize extensionSize;

			private int recentGenerationDepth;

			private TimeSpan statisticsUpdateInterval;

			private bool cloneInOriginalGeneration;
		}

		public class WorkerProcessConfig
		{
			public int MinIOThreads
			{
				get
				{
					return this.minIOThreads;
				}
			}

			public int MinWorkerThreads
			{
				get
				{
					return this.minWorkerThreads;
				}
			}

			public int MaxIOThreads
			{
				get
				{
					return this.maxIOThreads;
				}
			}

			public int MaxWorkerThreads
			{
				get
				{
					return this.maxWorkerThreads;
				}
			}

			public bool HandleLeakedException
			{
				get
				{
					return this.handleLeakedException;
				}
			}

			public string TemporaryStoragePath
			{
				get
				{
					return this.temporaryStoragePath;
				}
			}

			public bool CrashOnStopTimeout
			{
				get
				{
					return this.crashOnStopTimeout;
				}
			}

			public TimeSpan BackgroundProcessingThreadHangDetectionToleranceInterval { get; private set; }

			public int FreeMemoryRequiredToStartInMbytes
			{
				get
				{
					return this.freeMemoryRequiredToStartInMBytes;
				}
			}

			public static TransportAppConfig.WorkerProcessConfig Load()
			{
				TransportAppConfig.WorkerProcessConfig workerProcessConfig = new TransportAppConfig.WorkerProcessConfig();
				TransportAppConfig.WorkerProcessConfig.GetThreadLimits(Environment.ProcessorCount, out workerProcessConfig.minIOThreads, out workerProcessConfig.maxIOThreads, out workerProcessConfig.minWorkerThreads, out workerProcessConfig.maxWorkerThreads);
				workerProcessConfig.handleLeakedException = TransportAppConfig.GetConfigBool("HandleLeakedException", true);
				workerProcessConfig.crashOnStopTimeout = TransportAppConfig.GetConfigBool("CrashOnStopTimeout", false);
				workerProcessConfig.temporaryStoragePath = TransportAppConfig.GetConfigString("TemporaryStoragePath", Path.GetTempPath());
				workerProcessConfig.BackgroundProcessingThreadHangDetectionToleranceInterval = TransportAppConfig.GetConfigTimeSpan("BackgroundProcessingThreadHangDetectionToleranceInterval", TimeSpan.FromMinutes(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(15.0));
				workerProcessConfig.freeMemoryRequiredToStartInMBytes = TransportAppConfig.GetConfigInt("FreeMemoryRequiredToStartServiceInMbytes", 0, int.MaxValue, 0);
				return workerProcessConfig;
			}

			private static void GetThreadLimits(int numProcs, out int minIoThreads, out int maxIoThreads, out int minWorkerThreads, out int maxWorkerThreads)
			{
				minIoThreads = TransportAppConfig.GetConfigInt("MinIOThreads", numProcs, numProcs * 100, Math.Min(60 + numProcs * 10, 120));
				minWorkerThreads = TransportAppConfig.GetConfigInt("MinWorkerThreads", numProcs, numProcs * 200, Math.Min(120 + numProcs * 20, 400));
				maxIoThreads = TransportAppConfig.GetConfigInt("MaxIOThreads", numProcs, numProcs * 100, numProcs * 25);
				maxWorkerThreads = TransportAppConfig.GetConfigInt("MaxWorkerThreads", numProcs, TransportAppConfig.WorkerProcessConfig.MaxWorkerThreadsMaximum, numProcs * 65);
				if (maxIoThreads < minIoThreads)
				{
					maxIoThreads = minIoThreads;
				}
				if (maxWorkerThreads < minWorkerThreads)
				{
					maxWorkerThreads = minWorkerThreads;
				}
			}

			private const string MinIOThreadsKey = "MinIOThreads";

			private const string MinWorkerThreadsKey = "MinWorkerThreads";

			private const string MaxIOThreadsKey = "MaxIOThreads";

			private const string MaxWorkerThreadsKey = "MaxWorkerThreads";

			private const string HandleLeakedExceptionKey = "HandleLeakedException";

			private const string TemporaryStoragePathKey = "TemporaryStoragePath";

			private const string CrashOnStopTimeoutKey = "CrashOnStopTimeout";

			private const string BackgroundProcessingThreadHangDetectionToleranceIntervalKey = "BackgroundProcessingThreadHangDetectionToleranceInterval";

			private const string FreeMemoryRequiredToStartInMbytesKey = "FreeMemoryRequiredToStartServiceInMbytes";

			internal static readonly int MaxWorkerThreadsMaximum = Environment.ProcessorCount * 1000;

			private int minIOThreads;

			private int minWorkerThreads;

			private int maxIOThreads;

			private int maxWorkerThreads;

			private bool handleLeakedException = true;

			private string temporaryStoragePath;

			private bool crashOnStopTimeout;

			private int freeMemoryRequiredToStartInMBytes;
		}

		public class LatencyTrackerConfig
		{
			private LatencyTrackerConfig()
			{
			}

			public TimeSpan ServerLatencyThreshold
			{
				get
				{
					return this.serverLatencyThreshold;
				}
			}

			public TimeSpan PercentileLatencyExpiryInterval
			{
				get
				{
					return this.percentileLatencyExpiryInterval;
				}
			}

			public ushort PercentileLatencyInfinitySeconds
			{
				get
				{
					return this.percentileLatencyInfinitySeconds;
				}
			}

			public bool TrustExternalPickupReceivedHeaders
			{
				get
				{
					return this.trustExternalPickupReceivedHeaders;
				}
				internal set
				{
					this.trustExternalPickupReceivedHeaders = value;
				}
			}

			public bool MessageLatencyEnabled
			{
				get
				{
					return this.messageLatencyEnabled;
				}
			}

			public bool TreeLatencyTrackingEnabled
			{
				get
				{
					return this.treeLatencyTrackingEnabled;
				}
			}

			public TimeSpan ComponentThresholdInterval
			{
				get
				{
					return this.componentThresholdInterval;
				}
			}

			public TimeSpan MinInterestingUnknownInterval
			{
				get
				{
					return this.minInterestingUnknownInterval;
				}
			}

			public static TransportAppConfig.LatencyTrackerConfig Load()
			{
				return new TransportAppConfig.LatencyTrackerConfig
				{
					serverLatencyThreshold = TransportAppConfig.GetConfigTimeSpan("MinTotalServerLatencyToLog", TimeSpan.Zero, TransportAppConfig.LatencyTrackerConfig.MaxLatency, TransportAppConfig.LatencyTrackerConfig.DefaultServerLatencyThreshold),
					componentThresholdInterval = TransportAppConfig.GetConfigTimeSpan("ComponentThresholdInterval", TimeSpan.Zero, TransportAppConfig.LatencyTrackerConfig.MaxLatency, TransportAppConfig.LatencyTrackerConfig.DefaultComponentThresholdInterval),
					minInterestingUnknownInterval = TransportAppConfig.GetConfigTimeSpan("MinInterestingUnknownInterval", TimeSpan.Zero, TransportAppConfig.LatencyTrackerConfig.MaxLatency, TransportAppConfig.LatencyTrackerConfig.DefaultMinInterestingUnknownInterval),
					percentileLatencyExpiryInterval = TransportAppConfig.GetConfigTimeSpan("PercentileLatencyExpiryInterval", TransportAppConfig.LatencyTrackerConfig.MinPercentileLatencyExpiryInterval, TransportAppConfig.LatencyTrackerConfig.MaxPercentileLatencyExpiryInterval, TransportAppConfig.LatencyTrackerConfig.DefaultPercentileLatencyExpiryInterval),
					percentileLatencyInfinitySeconds = TransportAppConfig.LatencyTrackerConfig.GetPercentileLatencyInfinitySeconds(),
					trustExternalPickupReceivedHeaders = TransportAppConfig.GetConfigBool("TrustExternalPickupReceivedHeaders", false),
					messageLatencyEnabled = TransportAppConfig.GetConfigBool("MessageLatencyEnabled", true),
					treeLatencyTrackingEnabled = TransportAppConfig.GetConfigBool("TreeLatencyTrackingEnabled", false)
				};
			}

			private static ushort GetPercentileLatencyInfinitySeconds()
			{
				return (ushort)(TransportAppConfig.GetConfigTimeSpan("PercentileLatencyInfinityInterval", TransportAppConfig.LatencyTrackerConfig.MinPercentileLatencyInfinityInterval, TransportAppConfig.LatencyTrackerConfig.MaxPercentileLatencyInfinityInterval, TransportAppConfig.LatencyTrackerConfig.DefaultPercentileLatencyInfinityInterval).Ticks / 10000000L);
			}

			public const bool DefaultTrustExternalPickupReceivedHeaders = false;

			public const string TrustExternalPickupReceivedHeadersLabel = "TrustExternalPickupReceivedHeaders";

			public const string ServerLatencyThresholdLabel = "MinTotalServerLatencyToLog";

			public const string PercentileLatencyExpiryIntervalLabel = "PercentileLatencyExpiryInterval";

			public const string PercentileLatencyInfinityIntervalLabel = "PercentileLatencyInfinityInterval";

			public const string MessageLatencyEnabledLabel = "MessageLatencyEnabled";

			public const string TreeLatencyTrackingEnabledLabel = "TreeLatencyTrackingEnabled";

			public const string ComponentThresholdIntervalLabel = "ComponentThresholdInterval";

			public const string MinInterestingUnknownIntervalLabel = "MinInterestingUnknownInterval";

			public const bool DefaultMessageLatencyEnabled = true;

			public const bool DefaultTreeLatencyTrackingEnabled = false;

			public static readonly TimeSpan MaxLatency = TimeSpan.FromSeconds(43200.0);

			public static readonly TimeSpan DefaultServerLatencyThreshold = TimeSpan.Zero;

			public static readonly TimeSpan DefaultPercentileLatencyExpiryInterval = TimeSpan.FromMinutes(5.0);

			public static readonly TimeSpan MinPercentileLatencyExpiryInterval = TimeSpan.FromSeconds(15.0);

			public static readonly TimeSpan MaxPercentileLatencyExpiryInterval = TimeSpan.FromHours(1.0);

			public static readonly TimeSpan DefaultPercentileLatencyInfinityInterval = TimeSpan.FromMinutes(15.0);

			public static readonly TimeSpan MinPercentileLatencyInfinityInterval = TimeSpan.FromSeconds(2.0);

			public static readonly TimeSpan MaxPercentileLatencyInfinityInterval = TimeSpan.FromHours(1.0);

			public static readonly TimeSpan DefaultComponentThresholdInterval = TimeSpan.FromMilliseconds(1.0);

			public static readonly TimeSpan DefaultMinInterestingUnknownInterval = TimeSpan.FromMilliseconds(1.0);

			private TimeSpan serverLatencyThreshold;

			private TimeSpan percentileLatencyExpiryInterval;

			private ushort percentileLatencyInfinitySeconds;

			private bool trustExternalPickupReceivedHeaders;

			private bool messageLatencyEnabled;

			private bool treeLatencyTrackingEnabled;

			private TimeSpan componentThresholdInterval;

			private TimeSpan minInterestingUnknownInterval;
		}

		public class RecipientValidatorConfig
		{
			private RecipientValidatorConfig()
			{
			}

			public TimeSpan RefreshInterval
			{
				get
				{
					return this.refreshInterval;
				}
			}

			public TimeSpan ReloadInterval
			{
				get
				{
					return this.reloadInterval;
				}
			}

			public static TransportAppConfig.RecipientValidatorConfig Load()
			{
				return new TransportAppConfig.RecipientValidatorConfig
				{
					refreshInterval = TransportAppConfig.GetConfigTimeSpan("RefreshInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TransportAppConfig.RecipientValidatorConfig.DefaultRefreshInterval),
					reloadInterval = TransportAppConfig.GetConfigTimeSpan("ReloadInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TransportAppConfig.RecipientValidatorConfig.DefaultReloadInterval)
				};
			}

			private static readonly TimeSpan DefaultRefreshInterval = new TimeSpan(0, 5, 0);

			private static readonly TimeSpan DefaultReloadInterval = new TimeSpan(4, 0, 0);

			private TimeSpan refreshInterval;

			private TimeSpan reloadInterval;
		}

		public sealed class PerTenantCacheConfig
		{
			private PerTenantCacheConfig()
			{
			}

			public ByteQuantifiedSize TransportSettingsCacheMaxSize
			{
				get
				{
					return this.transportSettingsCacheMaxSize;
				}
			}

			public TimeSpan TransportSettingsCacheExpiryInterval
			{
				get
				{
					return this.transportSettingsCacheExpiryInterval;
				}
			}

			public TimeSpan TransportSettingsCacheCleanupInterval
			{
				get
				{
					return this.transportSettingsCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize PerimeterSettingsCacheMaxSize
			{
				get
				{
					return this.perimeterSettingsCacheMaxSize;
				}
			}

			public TimeSpan PerimeterSettingsCacheExpiryInterval
			{
				get
				{
					return this.perimeterSettingsCacheExpiryInterval;
				}
			}

			public TimeSpan PerimeterSettingsCacheCleanupInterval
			{
				get
				{
					return this.perimeterSettingsCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize OrganizationMailboxDatabaseCacheMaxSize
			{
				get
				{
					return this.organizationMailboxDatabaseCacheMaxSize;
				}
			}

			public TimeSpan OrganizationMailboxDatabaseCacheExpiryInterval
			{
				get
				{
					return this.organizationMailboxDatabaseCacheExpiryInterval;
				}
			}

			public TimeSpan OrganizationMailboxDatabaseCacheCleanupInterval
			{
				get
				{
					return this.organizationMailboxDatabaseCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize JournalingRulesCacheMaxSize
			{
				get
				{
					return this.journalingRulesCacheMaxSize;
				}
			}

			public ByteQuantifiedSize ReconciliationCacheConfigMaxSize
			{
				get
				{
					return this.reconciliationCacheConfigMaxSize;
				}
			}

			public TimeSpan JournalingCacheExpiryInterval
			{
				get
				{
					return this.journalingCacheExpiryInterval;
				}
			}

			public TimeSpan JournalingCacheCleanupInterval
			{
				get
				{
					return this.journalingCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize MicrosoftExchangeRecipientCacheMaxSize
			{
				get
				{
					return this.microsoftExchangeRecipientCacheMaxSize;
				}
			}

			public TimeSpan MicrosoftExchangeRecipientCacheExpiryInterval
			{
				get
				{
					return this.microsoftExchangeRecipientCacheExpiryInterval;
				}
			}

			public TimeSpan MicrosoftExchangeRecipientCacheCleanupInterval
			{
				get
				{
					return this.microsoftExchangeRecipientCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize RemoteDomainsCacheMaxSize
			{
				get
				{
					return this.remoteDomainsCacheMaxSize;
				}
			}

			public TimeSpan RemoteDomainsCacheExpiryInterval
			{
				get
				{
					return this.remoteDomainsCacheExpiryInterval;
				}
			}

			public TimeSpan RemoteDomainsCacheCleanupInterval
			{
				get
				{
					return this.remoteDomainsCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize AcceptedDomainsCacheMaxSize
			{
				get
				{
					return this.acceptedDomainsCacheMaxSize;
				}
			}

			public TimeSpan AcceptedDomainsCacheExpiryInterval
			{
				get
				{
					return this.acceptedDomainsCacheExpiryInterval;
				}
			}

			public TimeSpan AcceptedDomainsCacheCleanupInterval
			{
				get
				{
					return this.acceptedDomainsCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize TransportRulesCacheMaxSize
			{
				get
				{
					return this.transportRulesCacheMaxSize;
				}
			}

			public TimeSpan TransportRulesCacheExpiryInterval
			{
				get
				{
					return this.transportRulesCacheExpiryInterval;
				}
			}

			public TimeSpan TransportRulesCacheCleanupInterval
			{
				get
				{
					return this.transportRulesCacheCleanupInterval;
				}
			}

			public ByteQuantifiedSize OutboundConnectorsCacheSize
			{
				get
				{
					return this.outboundConnectorsCacheMaxSize;
				}
			}

			public TimeSpan OutboundConnectorsCacheExpirationInterval
			{
				get
				{
					return this.outboundConnectorsCacheExpiryInterval;
				}
			}

			public TimeSpan OutboundConnectorsCacheCleanupInterval
			{
				get
				{
					return this.outboundConnectorsCacheCleanupInterval;
				}
			}

			public static TransportAppConfig.PerTenantCacheConfig Load()
			{
				return new TransportAppConfig.PerTenantCacheConfig
				{
					transportSettingsCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("TransportSettingsCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					perimeterSettingsCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("PerimeterSettingsCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					organizationMailboxDatabaseCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("OrganizationMailboxDatabaseCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					journalingRulesCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("JournalingRulesCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					reconciliationCacheConfigMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("ReconciliationConfigCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					microsoftExchangeRecipientCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("MicrosoftExchangeRecipientCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					remoteDomainsCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("RemoteDomainsCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					acceptedDomainsCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("AcceptedDomainsCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					transportRulesCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("TransportRulesCacheMaxSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.TransportRulesCacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.TransportRulesCacheSizeMaxValue),
					outboundConnectorsCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("OutboundConnectorsCacheSize", TransportAppConfig.PerTenantCacheConfig.CacheSizeMinValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeMaxValue, TransportAppConfig.PerTenantCacheConfig.CacheSizeDefaultValue),
					transportSettingsCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("TransportSettingsCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					perimeterSettingsCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("PerimeterSettingsCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					organizationMailboxDatabaseCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("OrganizationMailboxDatabaseCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					journalingCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("JournalingRulesCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					microsoftExchangeRecipientCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("MicrosoftExchangeRecipientCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					remoteDomainsCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("RemoteDomainsCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					acceptedDomainsCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("AcceptedDomainsCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					transportRulesCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("TransportRulesCacheExpiryInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					outboundConnectorsCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("OutboundConnectorsCacheExpirationInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheExpiryIntervalDefault),
					transportSettingsCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("TransportSettingsCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					perimeterSettingsCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("PerimeterSettingsCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					organizationMailboxDatabaseCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("OrganizationMailboxDatabaseCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					journalingCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("JournalingRulesCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					microsoftExchangeRecipientCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("MicrosoftExchangeRecipientCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					remoteDomainsCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("RemoteDomainsCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					acceptedDomainsCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("AcceptedDomainsCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					transportRulesCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("TransportRulesCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault),
					outboundConnectorsCacheCleanupInterval = TransportAppConfig.GetConfigTimeSpan("OutboundConnectorsCacheCleanupInterval", TransportAppConfig.PerTenantCacheConfig.CacheIntervalMin, TransportAppConfig.PerTenantCacheConfig.CacheIntervalMax, TransportAppConfig.PerTenantCacheConfig.CacheCleanupIntervalDefault)
				};
			}

			private const string TransportSettingsCacheMaxSizeString = "TransportSettingsCacheMaxSize";

			private const string PerimeterSettingsCacheMaxSizeString = "PerimeterSettingsCacheMaxSize";

			private const string OrganizationMailboxDatabaseCacheMaxSizeString = "OrganizationMailboxDatabaseCacheMaxSize";

			private const string JournalingRulesCacheMaxSizeString = "JournalingRulesCacheMaxSize";

			private const string ReconciliationConfigCacheMaxSizeString = "ReconciliationConfigCacheMaxSize";

			private const string MicrosoftExchangeRecipientCacheMaxSizeString = "MicrosoftExchangeRecipientCacheMaxSize";

			private const string RemoteDomainsCacheMaxSizeString = "RemoteDomainsCacheMaxSize";

			private const string AcceptedDomainsCacheMaxSizeString = "AcceptedDomainsCacheMaxSize";

			private const string TransportRulesCacheMaxSizeString = "TransportRulesCacheMaxSize";

			private const string TransportSettingsCacheExpiryIntervalString = "TransportSettingsCacheExpiryInterval";

			private const string PerimeterSettingsCacheExpiryIntervalString = "PerimeterSettingsCacheExpiryInterval";

			private const string OrganizationMailboxDatabaseCacheExpiryIntervalString = "OrganizationMailboxDatabaseCacheExpiryInterval";

			private const string JournalingRulesCacheExpiryIntervalString = "JournalingRulesCacheExpiryInterval";

			private const string MicrosoftExchangeRecipientCacheExpiryIntervalString = "MicrosoftExchangeRecipientCacheExpiryInterval";

			private const string RemoteDomainsCacheExpiryIntervalString = "RemoteDomainsCacheExpiryInterval";

			private const string AcceptedDomainsCacheExpiryIntervalString = "AcceptedDomainsCacheExpiryInterval";

			private const string TransportRulesCacheExpiryIntervalString = "TransportRulesCacheExpiryInterval";

			private const string TransportSettingsCacheCleanupIntervalString = "TransportSettingsCacheCleanupInterval";

			private const string PerimeterSettingsCacheCleanupIntervalString = "PerimeterSettingsCacheCleanupInterval";

			private const string OrganizationMailboxDatabaseCacheCleanupIntervalString = "OrganizationMailboxDatabaseCacheCleanupInterval";

			private const string JournalingRulesCacheCleanupIntervalString = "JournalingRulesCacheCleanupInterval";

			private const string MicrosoftExchangeRecipientCacheCleanupIntervalString = "MicrosoftExchangeRecipientCacheCleanupInterval";

			private const string RemoteDomainsCacheCleanupIntervalString = "RemoteDomainsCacheCleanupInterval";

			private const string AcceptedDomainsCacheCleanupIntervalString = "AcceptedDomainsCacheCleanupInterval";

			private const string TransportRulesCacheCleanupIntervalString = "TransportRulesCacheCleanupInterval";

			public const string OutboundConnectorsCacheSizeString = "OutboundConnectorsCacheSize";

			public const string OutboundConnectorsCacheExpirationIntervalString = "OutboundConnectorsCacheExpirationInterval";

			public const string OutboundConnectorsCacheCleanupIntervalString = "OutboundConnectorsCacheCleanupInterval";

			private static readonly ByteQuantifiedSize CacheSizeMinValue = ByteQuantifiedSize.FromMB(0UL);

			private static readonly ByteQuantifiedSize CacheSizeMaxValue = ByteQuantifiedSize.FromMB(100UL);

			private static readonly ByteQuantifiedSize TransportRulesCacheSizeMaxValue = ByteQuantifiedSize.FromMB(1000UL);

			private static readonly ByteQuantifiedSize CacheSizeDefaultValue = ByteQuantifiedSize.FromMB(20UL);

			private static readonly TimeSpan CacheIntervalMin = TimeSpan.MinValue;

			private static readonly TimeSpan CacheIntervalMax = TimeSpan.FromDays(1.0);

			private static readonly TimeSpan CacheExpiryIntervalDefault = TimeSpan.FromMinutes(15.0);

			private static readonly TimeSpan CacheCleanupIntervalDefault = TimeSpan.FromHours(2.0);

			private ByteQuantifiedSize perimeterSettingsCacheMaxSize;

			private TimeSpan perimeterSettingsCacheExpiryInterval;

			private TimeSpan perimeterSettingsCacheCleanupInterval;

			private ByteQuantifiedSize organizationMailboxDatabaseCacheMaxSize;

			private TimeSpan organizationMailboxDatabaseCacheExpiryInterval;

			private TimeSpan organizationMailboxDatabaseCacheCleanupInterval;

			private ByteQuantifiedSize transportSettingsCacheMaxSize;

			private TimeSpan transportSettingsCacheExpiryInterval;

			private TimeSpan transportSettingsCacheCleanupInterval;

			private ByteQuantifiedSize journalingRulesCacheMaxSize;

			private ByteQuantifiedSize reconciliationCacheConfigMaxSize;

			private TimeSpan journalingCacheExpiryInterval;

			private TimeSpan journalingCacheCleanupInterval;

			private ByteQuantifiedSize microsoftExchangeRecipientCacheMaxSize;

			private TimeSpan microsoftExchangeRecipientCacheExpiryInterval;

			private TimeSpan microsoftExchangeRecipientCacheCleanupInterval;

			private ByteQuantifiedSize remoteDomainsCacheMaxSize;

			private TimeSpan remoteDomainsCacheExpiryInterval;

			private TimeSpan remoteDomainsCacheCleanupInterval;

			private ByteQuantifiedSize acceptedDomainsCacheMaxSize;

			private TimeSpan acceptedDomainsCacheExpiryInterval;

			private TimeSpan acceptedDomainsCacheCleanupInterval;

			private ByteQuantifiedSize transportRulesCacheMaxSize;

			private TimeSpan transportRulesCacheExpiryInterval;

			private TimeSpan transportRulesCacheCleanupInterval;

			private ByteQuantifiedSize outboundConnectorsCacheMaxSize;

			private TimeSpan outboundConnectorsCacheExpiryInterval;

			private TimeSpan outboundConnectorsCacheCleanupInterval;
		}

		public sealed class MessageThrottlingConfiguration
		{
			private MessageThrottlingConfiguration()
			{
			}

			public bool Enabled
			{
				get
				{
					return this.enabled;
				}
			}

			public static TransportAppConfig.MessageThrottlingConfiguration Load()
			{
				return new TransportAppConfig.MessageThrottlingConfiguration
				{
					enabled = TransportAppConfig.GetConfigBool("MessageThrottlingEnabled", true)
				};
			}

			private const bool MessageThrottlingEnabledDefault = true;

			private const string EnabledPropertyString = "MessageThrottlingEnabled";

			private bool enabled;
		}

		public sealed class SMTPOutConnectionCacheConfig
		{
			private SMTPOutConnectionCacheConfig()
			{
			}

			public bool EnableConnectionCache
			{
				get
				{
					return this.enableConnectionCache;
				}
			}

			public bool EnableShadowConnectionCache
			{
				get
				{
					return this.enableShadowConnectionCache;
				}
			}

			public int ConnectionCacheMaxNumberOfEntriesForOutboundProxy
			{
				get
				{
					return this.connectionCacheMaxNumberOfEntriesForOutboundProxy;
				}
			}

			public int ConnectionCacheMaxNumberOfEntriesForNonOutboundProxy
			{
				get
				{
					return this.connectionCacheMaxNumberOfEntriesForNonOutboundProxy;
				}
			}

			public TimeSpan ConnectionTimeoutForOutboundProxy
			{
				get
				{
					return this.connectionTimeoutForOutboundProxy;
				}
			}

			public TimeSpan ConnectionTimeoutForNonOutboundProxy
			{
				get
				{
					return this.connectionTimeoutForNonOutboundProxy;
				}
			}

			public TimeSpan ConnectionInactivityTimeout
			{
				get
				{
					return this.connectionInactivityTimeoutForOutboundProxy;
				}
			}

			public TimeSpan ConnectionInactivityTimeoutForNonOutboundProxy
			{
				get
				{
					return this.connectionInactivityTimeoutForNonOutboundProxy;
				}
			}

			public static TransportAppConfig.SMTPOutConnectionCacheConfig Load()
			{
				return new TransportAppConfig.SMTPOutConnectionCacheConfig
				{
					enableConnectionCache = TransportAppConfig.GetConfigBool("EnableConnectionCache", TransportAppConfig.SMTPOutConnectionCacheConfig.EnableConnectionCacheDefaultValue),
					enableShadowConnectionCache = TransportAppConfig.GetConfigBool("EnableShadowConnectionCache", false),
					connectionCacheMaxNumberOfEntriesForOutboundProxy = TransportAppConfig.GetConfigInt("ConnectionCacheMaxNumberOfEntriesForOutboundProxy", TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheNumberOfEntryMinValue, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheNumberOfEntryMaxValue, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheNumberOfEntryDefaultValue),
					connectionCacheMaxNumberOfEntriesForNonOutboundProxy = TransportAppConfig.GetConfigInt("ConnectionCacheMaxNumberOfEntriesForNonOutboundProxy", TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheNumberOfEntryMinValue, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheNumberOfEntryMaxValue, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheNumberOfEntryDefaultValue),
					connectionTimeoutForOutboundProxy = TransportAppConfig.GetConfigTimeSpan("ConnectionTimeoutForOutboundProxy", TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMin, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMax, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheConnectionTimeOutDefault),
					connectionTimeoutForNonOutboundProxy = TransportAppConfig.GetConfigTimeSpan("ConnectionTimeoutForNonOutboundProxy", TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMin, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMax, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheConnectionTimeOutDefault),
					connectionInactivityTimeoutForOutboundProxy = TransportAppConfig.GetConfigTimeSpan("ConnectionInactivityTimeoutForOutboundProxy", TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMin, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMax, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheConnectionInactivityTimeOutDefault),
					connectionInactivityTimeoutForNonOutboundProxy = TransportAppConfig.GetConfigTimeSpan("ConnectionInactivityTimeoutForNonOutboundProxy", TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMin, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionTimeOutMax, TransportAppConfig.SMTPOutConnectionCacheConfig.ConnectionCacheConnectionInactivityTimeOutDefault)
				};
			}

			private const string EnableConnectionCacheString = "EnableConnectionCache";

			private const string ConnectionCacheMaxNumberOfEntriesForOutboundProxyString = "ConnectionCacheMaxNumberOfEntriesForOutboundProxy";

			private const string ConnectionCacheMaxNumberOfEntriesForNonOutboundProxyString = "ConnectionCacheMaxNumberOfEntriesForNonOutboundProxy";

			private const string ConnectionCacheConnectionTimeOutForOutboundProxyString = "ConnectionTimeoutForOutboundProxy";

			private const string ConnectionCacheConnectionTimeOutForNonOutboundProxyString = "ConnectionTimeoutForNonOutboundProxy";

			private const string ConnectionCacheConnectionInactivityTimeOutForOutboundProxyString = "ConnectionInactivityTimeoutForOutboundProxy";

			private const string ConnectionCacheConnectionInactivityTimeOutForNonOutboundProxyString = "ConnectionInactivityTimeoutForNonOutboundProxy";

			private static readonly bool EnableConnectionCacheDefaultValue = false;

			private static readonly int ConnectionCacheNumberOfEntryMinValue = 0;

			private static readonly int ConnectionCacheNumberOfEntryMaxValue = 100;

			private static readonly int ConnectionCacheNumberOfEntryDefaultValue = 50;

			private static readonly TimeSpan ConnectionTimeOutMin = TimeSpan.FromSeconds(5.0);

			private static readonly TimeSpan ConnectionTimeOutMax = TimeSpan.FromMinutes(60.0);

			private static readonly TimeSpan ConnectionCacheConnectionTimeOutDefault = TimeSpan.FromMinutes(10.0);

			private static readonly TimeSpan ConnectionCacheConnectionInactivityTimeOutDefault = TimeSpan.FromMinutes(5.0);

			private bool enableConnectionCache;

			private bool enableShadowConnectionCache;

			private int connectionCacheMaxNumberOfEntriesForOutboundProxy;

			private int connectionCacheMaxNumberOfEntriesForNonOutboundProxy;

			private TimeSpan connectionTimeoutForOutboundProxy;

			private TimeSpan connectionTimeoutForNonOutboundProxy;

			private TimeSpan connectionInactivityTimeoutForOutboundProxy;

			private TimeSpan connectionInactivityTimeoutForNonOutboundProxy;
		}

		public sealed class IsMemberOfResolverConfiguration
		{
			public IsMemberOfResolverConfiguration(bool enabled, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration resolvedGroupsCacheConfiguration, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration expandedGroupsCacheConfiguration, bool disableDynamicGroups)
			{
				this.enabled = enabled;
				this.resolvedGroupsCacheConfiguration = resolvedGroupsCacheConfiguration;
				this.expandedGroupsCacheConfiguration = expandedGroupsCacheConfiguration;
				this.disableDynamicGroups = disableDynamicGroups;
			}

			private IsMemberOfResolverConfiguration()
			{
			}

			public bool Enabled
			{
				get
				{
					return this.enabled;
				}
			}

			public TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration ResolvedGroupsCacheConfiguration
			{
				get
				{
					return this.resolvedGroupsCacheConfiguration;
				}
			}

			public TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration ExpandedGroupsCacheConfiguration
			{
				get
				{
					return this.expandedGroupsCacheConfiguration;
				}
			}

			public bool DisableDynamicGroups
			{
				get
				{
					return this.disableDynamicGroups;
				}
			}

			public static TransportAppConfig.IsMemberOfResolverConfiguration Load(string componentName)
			{
				TransportAppConfig.IsMemberOfResolverConfiguration isMemberOfResolverConfiguration = new TransportAppConfig.IsMemberOfResolverConfiguration();
				string label = string.Format("{0}_IsMemberOfResolver_Enabled", componentName);
				isMemberOfResolverConfiguration.enabled = TransportAppConfig.GetConfigBool(label, true);
				isMemberOfResolverConfiguration.resolvedGroupsCacheConfiguration = TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.Load(componentName, "ResolvedGroupsCache", TransportAppConfig.IsMemberOfResolverConfiguration.ResolvedGroupsMaxCacheSize, TransportAppConfig.IsMemberOfResolverConfiguration.ResolvedGroupsDefaultCacheSize);
				isMemberOfResolverConfiguration.expandedGroupsCacheConfiguration = TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.Load(componentName, "ExpandedGroupsCache", TransportAppConfig.IsMemberOfResolverConfiguration.ExpandedGroupsMaxCacheSize, TransportAppConfig.IsMemberOfResolverConfiguration.ExpandedGroupsDefaultCacheSize);
				string label2 = string.Format("{0}_IsMemberOfResolver_DisableDynamicGroups", componentName);
				isMemberOfResolverConfiguration.disableDynamicGroups = TransportAppConfig.GetConfigBool(label2, false);
				return isMemberOfResolverConfiguration;
			}

			private const string ResolvedGroupsCacheName = "ResolvedGroupsCache";

			private const string ExpandedGroupsCacheName = "ExpandedGroupsCache";

			private const string EnabledTemplate = "{0}_IsMemberOfResolver_Enabled";

			private const string DisableDynamicGroupsTemplate = "{0}_IsMemberOfResolver_DisableDynamicGroups";

			private static readonly ByteQuantifiedSize ResolvedGroupsMaxCacheSize = ByteQuantifiedSize.FromGB(1UL);

			private static readonly ByteQuantifiedSize ResolvedGroupsDefaultCacheSize = ByteQuantifiedSize.FromMB(32UL);

			private static readonly ByteQuantifiedSize ExpandedGroupsMaxCacheSize = ByteQuantifiedSize.FromGB(64UL);

			private static readonly ByteQuantifiedSize ExpandedGroupsDefaultCacheSize = ByteQuantifiedSize.FromMB(512UL);

			private bool enabled;

			private TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration resolvedGroupsCacheConfiguration;

			private TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration expandedGroupsCacheConfiguration;

			private bool disableDynamicGroups;

			public sealed class CacheConfiguration
			{
				public CacheConfiguration(long maxSizeInBytes, TimeSpan expirationInterval, TimeSpan cleanupInterval, TimeSpan purgeInterval, TimeSpan refreshInterval)
				{
					TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.ThrowIfInvalidSize(maxSizeInBytes, "maxSizeInBytes");
					TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.ThrowIfInvalidTimeSpan(expirationInterval, "expirationInterval");
					TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.ThrowIfInvalidTimeSpan(cleanupInterval, "cleanupInterval");
					TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.ThrowIfInvalidTimeSpan(refreshInterval, "refreshInterval");
					this.maxSize = ByteQuantifiedSize.FromBytes((ulong)maxSizeInBytes);
					this.expirationInterval = expirationInterval;
					this.cleanupInterval = cleanupInterval;
					this.purgeInterval = purgeInterval;
					this.refreshInterval = refreshInterval;
				}

				private CacheConfiguration()
				{
				}

				public ByteQuantifiedSize MaxSize
				{
					get
					{
						return this.maxSize;
					}
				}

				public TimeSpan ExpirationInterval
				{
					get
					{
						return this.expirationInterval;
					}
				}

				public TimeSpan CleanupInterval
				{
					get
					{
						return this.cleanupInterval;
					}
				}

				public TimeSpan PurgeInterval
				{
					get
					{
						return this.purgeInterval;
					}
				}

				public TimeSpan RefreshInterval
				{
					get
					{
						return this.refreshInterval;
					}
				}

				public static TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration Load(string componentName, string cacheName, ByteQuantifiedSize maxCacheSize, ByteQuantifiedSize defaultCacheSize)
				{
					TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration cacheConfiguration = new TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration();
					string label = string.Format("{0}_IsMemberOfResolver_{1}_MaxSize", componentName, cacheName);
					cacheConfiguration.maxSize = TransportAppConfig.GetConfigByteQuantifiedSize(label, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MinCacheSize, maxCacheSize, defaultCacheSize);
					string label2 = string.Format("{0}_IsMemberOfResolver_{1}_ExpirationInterval", componentName, cacheName);
					cacheConfiguration.expirationInterval = TransportAppConfig.GetConfigTimeSpan(label2, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MinCacheExpirationInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MaxCacheExpirationInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.DefaultCacheExpirationInterval);
					string label3 = string.Format("{0}_IsMemberOfResolver_{1}_CleanupInterval", componentName, cacheName);
					cacheConfiguration.cleanupInterval = TransportAppConfig.GetConfigTimeSpan(label3, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MinCacheCleanupInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MaxCacheCleanupInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.DefaultCacheCleanupInterval);
					string label4 = string.Format("{0}_IsMemberOfResolver_{1}_PurgeInterval", componentName, cacheName);
					cacheConfiguration.purgeInterval = TransportAppConfig.GetConfigTimeSpan(label4, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MinCachePurgeInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MaxCachePurgeInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.DefaultCachePurgeInterval);
					string label5 = string.Format("{0}_IsMemberOfResolver_{1}_RefreshInterval", componentName, cacheName);
					cacheConfiguration.refreshInterval = TransportAppConfig.GetConfigTimeSpan(label5, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MinCacheRefreshInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.MaxCacheRefreshInterval, TransportAppConfig.IsMemberOfResolverConfiguration.CacheConfiguration.DefaultCacheRefreshInterval);
					return cacheConfiguration;
				}

				private static void ThrowIfInvalidSize(long size, string name)
				{
					if (size < 0L)
					{
						throw new ArgumentOutOfRangeException(name, size, name + " must be greater than or equal to 0");
					}
				}

				private static void ThrowIfInvalidTimeSpan(TimeSpan timeSpan, string name)
				{
					if (timeSpan.TotalSeconds < 0.0)
					{
						throw new ArgumentOutOfRangeException(name, timeSpan, name + " must be greater than or equal to 0");
					}
				}

				private const string ExpirationIntervalTemplate = "{0}_IsMemberOfResolver_{1}_ExpirationInterval";

				private const string CleanupIntervalTemplate = "{0}_IsMemberOfResolver_{1}_CleanupInterval";

				private const string PurgeIntervalTemplate = "{0}_IsMemberOfResolver_{1}_PurgeInterval";

				private const string RefreshIntervalTemplate = "{0}_IsMemberOfResolver_{1}_RefreshInterval";

				private const string MaxSizeTemplate = "{0}_IsMemberOfResolver_{1}_MaxSize";

				private static readonly ByteQuantifiedSize MinCacheSize = ByteQuantifiedSize.FromMB(0UL);

				private static readonly TimeSpan MinCacheExpirationInterval = TimeSpan.FromSeconds(5.0);

				private static readonly TimeSpan MaxCacheExpirationInterval = TimeSpan.FromDays(1.0);

				private static readonly TimeSpan DefaultCacheExpirationInterval = TimeSpan.FromHours(3.0);

				private static readonly TimeSpan MinCacheCleanupInterval = TimeSpan.FromSeconds(5.0);

				private static readonly TimeSpan MaxCacheCleanupInterval = TimeSpan.FromHours(4.0);

				private static readonly TimeSpan DefaultCacheCleanupInterval = TimeSpan.FromHours(1.0);

				private static readonly TimeSpan MinCachePurgeInterval = TimeSpan.FromSeconds(5.0);

				private static readonly TimeSpan MaxCachePurgeInterval = TimeSpan.FromHours(4.0);

				private static readonly TimeSpan DefaultCachePurgeInterval = TimeSpan.FromMinutes(5.0);

				private static readonly TimeSpan MinCacheRefreshInterval = TimeSpan.FromSeconds(5.0);

				private static readonly TimeSpan MaxCacheRefreshInterval = TimeSpan.FromHours(4.0);

				private static readonly TimeSpan DefaultCacheRefreshInterval = TimeSpan.FromMinutes(10.0);

				private ByteQuantifiedSize maxSize;

				private TimeSpan expirationInterval;

				private TimeSpan cleanupInterval;

				private TimeSpan purgeInterval;

				private TimeSpan refreshInterval;
			}
		}

		public sealed class SmtpAvailabilityConfig
		{
			public int SmtpAvailabilityMinConnectionsToMonitor
			{
				get
				{
					return this.smtpAvailabilityMinConnectionsToMonitor;
				}
			}

			public TimeSpan KerberosTicketCacheFlushMinInterval
			{
				get
				{
					return this.kerberosTicketCacheFlushMinInterval;
				}
			}

			public static TransportAppConfig.SmtpAvailabilityConfig Load()
			{
				return new TransportAppConfig.SmtpAvailabilityConfig
				{
					smtpAvailabilityMinConnectionsToMonitor = TransportAppConfig.GetConfigInt("SmtpAvailabilityMinConnectionsToMonitor", 0, int.MaxValue, 20),
					kerberosTicketCacheFlushMinInterval = TransportAppConfig.GetConfigTimeSpan("KerberosTicketCacheFlushMinInterval", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0))
				};
			}

			private const int DefaultSmtpAvailabilityMinConnectionsToMonitor = 20;

			private int smtpAvailabilityMinConnectionsToMonitor;

			private TimeSpan kerberosTicketCacheFlushMinInterval;
		}

		public interface ISmtpMailCommandConfig
		{
			bool TransferAdditionalTenantDataThroughXAttr { get; }

			bool UseAdditionalTenantDataFromXAttr { get; }
		}

		public sealed class SmtpMailCommandConfig : TransportAppConfig.ISmtpMailCommandConfig
		{
			public bool TransferAdditionalTenantDataThroughXAttr { get; private set; }

			public bool UseAdditionalTenantDataFromXAttr { get; private set; }

			public static TransportAppConfig.SmtpMailCommandConfig Load()
			{
				return new TransportAppConfig.SmtpMailCommandConfig
				{
					TransferAdditionalTenantDataThroughXAttr = TransportAppConfig.GetConfigBool("TransferAdditionalTenantDataThroughXATTR", false),
					UseAdditionalTenantDataFromXAttr = TransportAppConfig.GetConfigBool("UseAdditionalTenantDataFromXATTR", false)
				};
			}
		}

		public sealed class SmtpDataConfig
		{
			public string SmtpDataResponse
			{
				get
				{
					return this.smtpDataResponse;
				}
			}

			public bool InboundApplyMissingEncoding
			{
				get
				{
					return this.inboundApplyMissingEncoding;
				}
			}

			public bool OutboundRejectBareLinefeeds
			{
				get
				{
					return this.outboundRejectBareLinefeeds;
				}
			}

			public bool AcceptAndFixSmtpAddressWithInvalidLocalPart
			{
				get
				{
					return this.acceptAndFixSmtpAddressWithInvalidLocalPart;
				}
			}

			public static TransportAppConfig.SmtpDataConfig Load()
			{
				return new TransportAppConfig.SmtpDataConfig
				{
					smtpDataResponse = TransportAppConfig.GetConfigString("SmtpCustomEndOfDataResponseString", "Queued mail for delivery"),
					inboundApplyMissingEncoding = TransportAppConfig.GetConfigBool("SmtpDataCommandInboundApplyMissingEncoding", true),
					outboundRejectBareLinefeeds = TransportAppConfig.GetConfigBool("SmtpDataCommandOutboundRejectBareLinefeeds", true),
					acceptAndFixSmtpAddressWithInvalidLocalPart = TransportAppConfig.GetConfigBool("AcceptAndFixSmtpAddressWithInvalidLocalPart", false)
				};
			}

			private const string DefaultSmtpDataResponse = "Queued mail for delivery";

			private string smtpDataResponse;

			private bool inboundApplyMissingEncoding;

			private bool outboundRejectBareLinefeeds;

			private bool acceptAndFixSmtpAddressWithInvalidLocalPart;
		}

		public sealed class MessageContextBlobConfig
		{
			public bool AdvertiseExtendedProperties
			{
				get
				{
					if (this.advertiseExtendedProperties == null)
					{
						return Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery || Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub;
					}
					return this.advertiseExtendedProperties.Value;
				}
			}

			public bool AdvertiseADRecipientCache
			{
				get
				{
					if (this.advertiseADRecipientCache == null)
					{
						return Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub || Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery;
					}
					return this.advertiseADRecipientCache.Value;
				}
			}

			public bool AdvertiseFastIndex
			{
				get
				{
					return this.advertiseFastIndex;
				}
			}

			public bool TransferADRecipientCache
			{
				get
				{
					if (this.transferADRecipientCache == null)
					{
						return Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub || Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission;
					}
					return this.transferADRecipientCache.Value;
				}
			}

			public bool TransferExtendedProperties
			{
				get
				{
					bool result = Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub || Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery || Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxSubmission;
					if (this.transferExtendedProperties == null)
					{
						return result;
					}
					return this.transferExtendedProperties.Value;
				}
			}

			public bool TransferFastIndex
			{
				get
				{
					return this.transferFastIndex;
				}
			}

			public ByteQuantifiedSize ExtendedPropertiesMaxBlobSize
			{
				get
				{
					return this.extendedPropertiesMaxBlobSize;
				}
			}

			public ByteQuantifiedSize AdrcCacheMaxBlobSize
			{
				get
				{
					return this.adrcCacheMaxBlobSize;
				}
			}

			public ByteQuantifiedSize FastIndexMaxBlobSize
			{
				get
				{
					return this.fastIndexMaxBlobSize;
				}
			}

			public bool WatsonReportOnFailureEnabled
			{
				get
				{
					return this.watsonReportOnFailureEnabled;
				}
			}

			public static TransportAppConfig.MessageContextBlobConfig Load()
			{
				return new TransportAppConfig.MessageContextBlobConfig
				{
					advertiseADRecipientCache = TransportAppConfig.GetConfigNullableBool("SMTPReceiveAdvertiseADRecipientCache"),
					advertiseExtendedProperties = TransportAppConfig.GetConfigNullableBool("SMTPReceiveAdvertiseExtendedProperties"),
					advertiseFastIndex = TransportAppConfig.GetConfigBool("SMTPReceiveAdvertiseFastIndex", false),
					transferADRecipientCache = TransportAppConfig.GetConfigNullableBool("SMTPSendTransferADRecipientCache"),
					transferExtendedProperties = TransportAppConfig.GetConfigNullableBool("SMTPSendTransferExtendedProperties"),
					transferFastIndex = TransportAppConfig.GetConfigBool("SMTPSendTransferFastIndex", false),
					watsonReportOnFailureEnabled = TransportAppConfig.GetConfigBool("WatsonReportOnFailureEnabled", true),
					extendedPropertiesMaxBlobSize = TransportAppConfig.GetConfigByteQuantifiedSize("ExtendedPropertiesMaxSmtpBlobSize", ByteQuantifiedSize.MinValue, ByteQuantifiedSize.FromMB(500UL), ByteQuantifiedSize.FromMB(200UL)),
					adrcCacheMaxBlobSize = TransportAppConfig.GetConfigByteQuantifiedSize("AdrcCacheMaxBlobSize", ByteQuantifiedSize.MinValue, ByteQuantifiedSize.FromMB(500UL), ByteQuantifiedSize.FromMB(200UL)),
					fastIndexMaxBlobSize = TransportAppConfig.GetConfigByteQuantifiedSize("FastIndexMaxBlobSize", ByteQuantifiedSize.MinValue, ByteQuantifiedSize.FromMB(500UL), ByteQuantifiedSize.FromMB(200UL))
				};
			}

			private bool? advertiseExtendedProperties;

			private bool? advertiseADRecipientCache;

			private bool advertiseFastIndex;

			private bool? transferADRecipientCache;

			private bool? transferExtendedProperties;

			private bool transferFastIndex;

			private ByteQuantifiedSize extendedPropertiesMaxBlobSize;

			private ByteQuantifiedSize adrcCacheMaxBlobSize;

			private ByteQuantifiedSize fastIndexMaxBlobSize;

			private bool watsonReportOnFailureEnabled;
		}

		public sealed class SmtpReceiveConfig
		{
			public bool TarpitMuaSubmission
			{
				get
				{
					return this.tarpitMuaSubmission;
				}
			}

			public bool SMTPAcceptAnyRecipient
			{
				get
				{
					return this.smtpAcceptAnyRecipient;
				}
			}

			public bool MailboxDeliveryAcceptAnonymousUsers
			{
				get
				{
					return this.mailboxDeliveryAcceptAnonymousUsers;
				}
			}

			public int MaxProxyHopCount
			{
				get
				{
					return this.maxProxyHopCount;
				}
			}

			public bool RejectUnscopedMessages
			{
				get
				{
					return this.rejectUnscopedMessages;
				}
			}

			public bool BlockedSessionLoggingEnabled
			{
				get
				{
					return this.blockedSessionLoggingEnabled;
				}
			}

			public bool OneLevelWildcardMatchForCertSelection
			{
				get
				{
					return this.oneLevelWildcardMatchForCertSelection;
				}
			}

			public bool WaitForSmtpSessionsAtShutdown
			{
				get
				{
					return this.waitForSmtpSessionsAtShutdown;
				}
			}

			public bool GrantExchangeServerPermissions
			{
				get
				{
					return this.grantExchangeServerPermissions;
				}
			}

			public bool ExclusiveAddressUse
			{
				get
				{
					return this.exclusiveAddressUse;
				}
			}

			public bool Ipv6ReceiveConnectionThrottlingEnabled
			{
				get
				{
					return this.ipv6ReceiveConnectionThrottlingEnabled;
				}
			}

			public bool ReceiveTlsThrottlingEnabled
			{
				get
				{
					return this.receiveTlsThrottlingEnabled;
				}
			}

			public bool DisableHandleInheritance
			{
				get
				{
					return this.disableHandleInheritance;
				}
			}

			public int NetworkConnectionReceiveBufferSize
			{
				get
				{
					return this.networkConnectionReceiveBufferSize;
				}
			}

			public static TransportAppConfig.SmtpReceiveConfig Load()
			{
				return new TransportAppConfig.SmtpReceiveConfig
				{
					tarpitMuaSubmission = TransportAppConfig.GetConfigBool("TarpitMuaSubmission", false),
					smtpAcceptAnyRecipient = (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.SmtpAcceptAnyRecipient.Enabled && TransportAppConfig.GetConfigBool("SMTPAcceptAnyRecipientOverride", false)),
					mailboxDeliveryAcceptAnonymousUsers = TransportAppConfig.GetConfigBool("MailboxDeliveryAcceptAnonymousUsers", false),
					maxProxyHopCount = TransportAppConfig.GetConfigInt("MaxProxyHopCount", 1, 10, 3),
					rejectUnscopedMessages = TransportAppConfig.GetConfigBool("RejectUnscopedMessages", true),
					blockedSessionLoggingEnabled = TransportAppConfig.GetConfigBool("BlockedSessionLoggingEnabled", false),
					oneLevelWildcardMatchForCertSelection = TransportAppConfig.GetConfigBool("SmtpReceiveOneLevelWildcardMatchForCertSelection", false),
					waitForSmtpSessionsAtShutdown = TransportAppConfig.GetConfigBool("WaitForSmtpSessionsAtShutdown", false),
					grantExchangeServerPermissions = TransportAppConfig.GetConfigBool("SmtpReceiveGrantExchangeServerPermissions", false),
					exclusiveAddressUse = TransportAppConfig.GetConfigBool("SmtpReceiveExclusiveAddressUse", true),
					ipv6ReceiveConnectionThrottlingEnabled = TransportAppConfig.GetConfigBool("Ipv6ReceiveConnectionThrottlingEnabled", false),
					receiveTlsThrottlingEnabled = TransportAppConfig.GetConfigBool("ReceiveTlsThrottlingEnabled", false),
					disableHandleInheritance = TransportAppConfig.GetConfigBool("SmtpReceiveDisableHandleInheritance", true),
					networkConnectionReceiveBufferSize = TransportAppConfig.GetConfigInt("NetworkConnectionReceiveBufferSize", 4096, int.MaxValue, 4096)
				};
			}

			private bool tarpitMuaSubmission;

			private bool smtpAcceptAnyRecipient;

			private bool mailboxDeliveryAcceptAnonymousUsers;

			private int maxProxyHopCount;

			private bool rejectUnscopedMessages;

			private bool blockedSessionLoggingEnabled;

			private bool oneLevelWildcardMatchForCertSelection;

			private bool waitForSmtpSessionsAtShutdown;

			private bool grantExchangeServerPermissions;

			private bool exclusiveAddressUse;

			private bool ipv6ReceiveConnectionThrottlingEnabled;

			private bool receiveTlsThrottlingEnabled;

			private bool disableHandleInheritance;

			private int networkConnectionReceiveBufferSize;
		}

		public sealed class SmtpSendConfig
		{
			public List<SmtpResponse> DowngradedResponses
			{
				get
				{
					return this.downgradedResponses;
				}
			}

			public List<SmtpResponse> UpgradedResponses
			{
				get
				{
					return this.upgradedResponses;
				}
			}

			public bool RetryMessageOnRcptTransientError
			{
				get
				{
					return this.retryMessageOnRcptTransientError;
				}
			}

			public TimeSpan SuspiciousDisconnectRetryInterval
			{
				get
				{
					return this.suspiciousDisconnectRetryInterval;
				}
			}

			public int PoisonForRemoteThreshold
			{
				get
				{
					return this.poisonForRemoteThreshold;
				}
			}

			public bool OneLevelWildcardMatchForCertSelection
			{
				get
				{
					return this.oneLevelWildcardMatchForCertSelection;
				}
			}

			public bool SendFewerMessagesToSlowerServerEnabled
			{
				get
				{
					return this.sendFewerMessagesToSlowerServerEnabled;
				}
			}

			public bool CacheOnlyUrlRetrievalForRemoteCertChain
			{
				get
				{
					return this.cacheOnlyUrlRetrievalForRemoteCertChain;
				}
			}

			public bool TreatTransientErrorsAsPermanentErrors
			{
				get
				{
					return this.treatTransientErrorsAsPermanentErrors;
				}
			}

			public static TransportAppConfig.SmtpSendConfig Load()
			{
				TransportAppConfig.SmtpSendConfig smtpSendConfig = new TransportAppConfig.SmtpSendConfig();
				TransportAppConfig.ConfigurationListsSection configurationListsSection = ConfigurationManager.GetSection("customLists") as TransportAppConfig.ConfigurationListsSection;
				if (configurationListsSection != null)
				{
					smtpSendConfig.downgradedResponses = TransportAppConfig.GetConfigList<SmtpResponse>(configurationListsSection.DowngradedResponses, new TransportAppConfig.TryParse<SmtpResponse>(SmtpResponse.TryParse));
					smtpSendConfig.upgradedResponses = TransportAppConfig.GetConfigList<SmtpResponse>(configurationListsSection.UpgradedResponses, new TransportAppConfig.TryParse<SmtpResponse>(SmtpResponse.TryParse));
				}
				smtpSendConfig.retryMessageOnRcptTransientError = TransportAppConfig.GetConfigBool("SMTPSendRetryMessageOnRcptTransientError", true);
				smtpSendConfig.poisonForRemoteThreshold = TransportAppConfig.GetConfigInt("SMTPSendPoisonForRemoteThreshold", 0, 100, 1000);
				smtpSendConfig.oneLevelWildcardMatchForCertSelection = TransportAppConfig.GetConfigBool("SmtpSendOneLevelWildcardMatchForCertSelection", false);
				smtpSendConfig.suspiciousDisconnectRetryInterval = TransportAppConfig.GetConfigTimeSpan("SMTPSendSuspiciousDisconnectRetryInterval", TimeSpan.Zero, TimeSpan.FromHours(12.0), TimeSpan.FromMinutes(10.0));
				smtpSendConfig.sendFewerMessagesToSlowerServerEnabled = TransportAppConfig.GetConfigBool("SendFewerMessagesToSlowerServer", true);
				smtpSendConfig.cacheOnlyUrlRetrievalForRemoteCertChain = TransportAppConfig.GetConfigBool("SmtpSendCacheOnlyUrlRetrievalForRemoteCertChain", true);
				smtpSendConfig.treatTransientErrorsAsPermanentErrors = TransportAppConfig.GetConfigBool("SmtpSendTreatTransientErrorsAsPermanentErrors", true);
				return smtpSendConfig;
			}

			private List<SmtpResponse> downgradedResponses = new List<SmtpResponse>();

			private List<SmtpResponse> upgradedResponses = new List<SmtpResponse>();

			private bool retryMessageOnRcptTransientError;

			private TimeSpan suspiciousDisconnectRetryInterval;

			private int poisonForRemoteThreshold;

			private bool oneLevelWildcardMatchForCertSelection;

			private bool sendFewerMessagesToSlowerServerEnabled;

			private bool cacheOnlyUrlRetrievalForRemoteCertChain;

			private bool treatTransientErrorsAsPermanentErrors;
		}

		public sealed class SmtpProxyConfig
		{
			public int MaxProxySetupAttempts
			{
				get
				{
					return this.maxProxySetupAttempts;
				}
			}

			public bool ValidateProxyTargetCertificate
			{
				get
				{
					return this.validateProxyTargetCertificate;
				}
			}

			public bool RequireXProxyExtension
			{
				get
				{
					return this.requireXProxyExtension;
				}
			}

			public SmtpDomainWithSubdomains ProxyCertificateFqdn
			{
				get
				{
					return this.proxyCertificateFqdn;
				}
			}

			public int ProxyPort
			{
				get
				{
					return this.proxyPort;
				}
			}

			public bool SimulateUserNotInAdAuthError
			{
				get
				{
					return this.simulateUserNotInAdAuthError;
				}
			}

			public string PodRedirectTemplate
			{
				get
				{
					return this.podRedirectTemplate;
				}
			}

			public int PodSiteStartRange
			{
				get
				{
					return this.podSiteStartRange;
				}
			}

			public int PodSiteEndRange
			{
				get
				{
					return this.podSiteEndRange;
				}
			}

			public bool ReplayAuthLogin
			{
				get
				{
					return this.replayAuthLogin;
				}
			}

			public bool PreferMailboxMountedServer
			{
				get
				{
					return this.preferMailboxMountedServer;
				}
			}

			public static TransportAppConfig.SmtpProxyConfig Load()
			{
				return new TransportAppConfig.SmtpProxyConfig
				{
					maxProxySetupAttempts = TransportAppConfig.GetConfigInt("MaxProxySetupAttempts", 1, 100, 6),
					proxyPort = TransportAppConfig.GetConfigInt("ProxyPort", 0, 65535, 465),
					simulateUserNotInAdAuthError = TransportAppConfig.GetConfigBool("SimulateUserNotInAdAuthError", false),
					validateProxyTargetCertificate = TransportAppConfig.GetConfigBool("ValidateProxyTargetCertificate", false),
					requireXProxyExtension = TransportAppConfig.GetConfigBool("RequireXProxyExtension", true),
					proxyCertificateFqdn = TransportAppConfig.GetConfigValue<SmtpDomainWithSubdomains>("ProxyCertificateFqdn", new SmtpDomainWithSubdomains("smtp.outlook.com"), new TransportAppConfig.TryParse<SmtpDomainWithSubdomains>(SmtpDomainWithSubdomains.TryParse)),
					podRedirectTemplate = TransportAppConfig.GetConfigString("PodRedirectTemplate", "pod{0}.proxy.outlook.com"),
					podSiteStartRange = TransportAppConfig.GetConfigInt("PodSiteStartRange", 0, int.MaxValue, 50000),
					podSiteEndRange = TransportAppConfig.GetConfigInt("PodSiteEndRange", 0, int.MaxValue, 59999),
					replayAuthLogin = TransportAppConfig.GetConfigBool("ClientProxyReplayAuthLogin", false),
					preferMailboxMountedServer = TransportAppConfig.GetConfigBool("ClientProxyPreferMailboxMountedServer", true)
				};
			}

			private int maxProxySetupAttempts;

			private bool validateProxyTargetCertificate;

			private bool requireXProxyExtension;

			private SmtpDomainWithSubdomains proxyCertificateFqdn;

			private int proxyPort;

			private bool simulateUserNotInAdAuthError;

			private string podRedirectTemplate;

			private int podSiteStartRange;

			private int podSiteEndRange;

			private bool replayAuthLogin;

			private bool preferMailboxMountedServer;
		}

		public interface ISmtpInboundProxyConfig
		{
			bool InboundProxyDestinationTrackingEnabled { get; }

			bool InboundProxyAccountForestTrackingEnabled { get; }

			bool RejectBasedOnInboundProxyDestinationTrackingEnabled { get; }

			bool RejectBasedOnInboundProxyAccountForestTrackingEnabled { get; }

			bool TrackInboundProxyDestinationsInRcpt { get; }

			int PerDestinationConnectionPercentageThreshold { get; }

			int PerAccountForestConnectionPercentageThreshold { get; }

			TimeSpan InboundProxyDestinationTrackerLogInterval { get; }

			bool TryGetDestinationConnectionThreshold(string destination, out int threshold);

			bool TryGetAccountForestConnectionThreshold(string destination, out int threshold);
		}

		public sealed class SmtpInboundProxyConfig : TransportAppConfig.ISmtpInboundProxyConfig
		{
			public bool RequireXProxyFromExtension
			{
				get
				{
					return this.requireXProxyFromExtension;
				}
			}

			public bool RequireTls
			{
				get
				{
					return this.requireTls;
				}
			}

			public bool UseExternalDnsServers
			{
				get
				{
					return this.useExternalDnsServers;
				}
			}

			public bool IsInternalDestination
			{
				get
				{
					return this.isInternalDestination;
				}
			}

			public List<IPAddress> ProxyDestinations
			{
				get
				{
					return this.proxyDestinations;
				}
			}

			public bool TreatProxyDestinationAsExternal
			{
				get
				{
					return this.treatProxyDestinationAsExternal;
				}
			}

			public SmtpDomainWithSubdomains TlsDomain
			{
				get
				{
					return this.tlsDomain;
				}
			}

			public TlsAuthLevel TlsAuthLevel
			{
				get
				{
					return this.tlsAuthLevel;
				}
			}

			public string ExternalCertificateSubject
			{
				get
				{
					return this.externalCertificateSubject;
				}
			}

			public ByteQuantifiedSize AccumulatedMessageSizeThreshold
			{
				get
				{
					return this.accumulatedMessageSizeThreshold;
				}
			}

			public bool PreserveTargetResponse
			{
				get
				{
					return this.preserveTargetResponse;
				}
			}

			public bool SendNewXProxyFromArguments
			{
				get
				{
					return this.sendNewXProxyFromArguments;
				}
			}

			public int PerHostConnectionAttempts
			{
				get
				{
					return this.perHostConnectionAttempts;
				}
			}

			public int PerDestinationConnectionPercentageThreshold
			{
				get
				{
					return this.perDestinationConnectionPercentageThreshold;
				}
			}

			public int PerAccountForestConnectionPercentageThreshold
			{
				get
				{
					return this.perAccountForestConnectionPercentageThreshold;
				}
			}

			public bool InboundProxyDestinationTrackingEnabled
			{
				get
				{
					return this.inboundProxyDestinationTrackingEnabled;
				}
			}

			public bool InboundProxyAccountForestTrackingEnabled
			{
				get
				{
					return this.inboundProxyAccountForestTrackingEnabled;
				}
			}

			public bool RejectBasedOnInboundProxyDestinationTrackingEnabled
			{
				get
				{
					return this.rejectBasedOnInboundProxyDestinationTrackingEnabled;
				}
			}

			public bool RejectBasedOnInboundProxyAccountForestTrackingEnabled
			{
				get
				{
					return this.rejectBasedOnInboundProxyAccountForestTrackingEnabled;
				}
			}

			public bool TrackInboundProxyDestinationsInRcpt
			{
				get
				{
					return this.trackInboundProxyDestinationsInRcpt;
				}
			}

			public TimeSpan InboundProxyDestinationTrackerLogInterval
			{
				get
				{
					return this.inboundProxyDestinationTrackerLogInterval;
				}
			}

			private bool TryGetDestinationConnectionThreshold(Dictionary<string, int> dictionary, string destination, out int threshold)
			{
				if (!dictionary.TryGetValue(destination, out threshold))
				{
					threshold = 0;
					return false;
				}
				return true;
			}

			public bool TryGetDestinationConnectionThreshold(string destination, out int threshold)
			{
				return this.TryGetDestinationConnectionThreshold(this.perDestinationConnectionThresholdDictionary, destination, out threshold);
			}

			public bool TryGetAccountForestConnectionThreshold(string destination, out int threshold)
			{
				return this.TryGetDestinationConnectionThreshold(this.perAccountForestConnectionThresholdDictionary, destination, out threshold);
			}

			public static TransportAppConfig.SmtpInboundProxyConfig Load()
			{
				TransportAppConfig.SmtpInboundProxyConfig smtpInboundProxyConfig = new TransportAppConfig.SmtpInboundProxyConfig();
				smtpInboundProxyConfig.requireXProxyFromExtension = TransportAppConfig.GetConfigBool("InboundProxyRequireXProxyFromExtension", true);
				smtpInboundProxyConfig.requireTls = TransportAppConfig.GetConfigBool("InboundProxyRequireTls", true);
				smtpInboundProxyConfig.useExternalDnsServers = TransportAppConfig.GetConfigBool("InboundProxyUseExternalDnsServers", false);
				smtpInboundProxyConfig.isInternalDestination = TransportAppConfig.GetConfigBool("InboundProxyIsInternalDestination", false);
				smtpInboundProxyConfig.proxyDestinations = TransportAppConfig.GetConfigList<IPAddress>("InboundProxyDestinations", ',', new TransportAppConfig.TryParse<IPAddress>(IPAddress.TryParse));
				smtpInboundProxyConfig.treatProxyDestinationAsExternal = TransportAppConfig.GetConfigBool("InboundProxyTreatProxyDestinationAsExternal", false);
				smtpInboundProxyConfig.tlsAuthLevel = TransportAppConfig.GetConfigEnum<TlsAuthLevel>("InboundProxyTlsAuthLevel", TransportAppConfig.SmtpInboundProxyConfig.DefaultTlsAuthLevel);
				smtpInboundProxyConfig.tlsDomain = TransportAppConfig.GetConfigValue<SmtpDomainWithSubdomains>("InboundProxyTlsDomain", (smtpInboundProxyConfig.tlsAuthLevel == TlsAuthLevel.DomainValidation) ? new SmtpDomainWithSubdomains("mail.messaging.microsoft.com") : null, new TransportAppConfig.TryParse<SmtpDomainWithSubdomains>(SmtpDomainWithSubdomains.TryParse));
				smtpInboundProxyConfig.externalCertificateSubject = TransportAppConfig.GetConfigString("InboundProxyExternalCertificateSubject", null);
				smtpInboundProxyConfig.accumulatedMessageSizeThreshold = TransportAppConfig.GetConfigByteQuantifiedSize("InboundProxyAccumulatedMessageSizeThreshold", ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL), TransportAppConfig.SmtpInboundProxyConfig.DefaultAccumulatedMessageSizeThreshold);
				smtpInboundProxyConfig.preserveTargetResponse = TransportAppConfig.GetConfigBool("InboundProxyPreserveTargetResponse", false);
				smtpInboundProxyConfig.sendNewXProxyFromArguments = TransportAppConfig.GetConfigBool("InboundProxySendNewXProxyFromArguments", true);
				smtpInboundProxyConfig.perHostConnectionAttempts = TransportAppConfig.GetConfigInt("InboundProxyPerHostConnectionAttempts", 1, 10, 1);
				smtpInboundProxyConfig.inboundProxyDestinationTrackingEnabled = TransportAppConfig.GetConfigBool("InboundProxyDestinationTrackerEnabled", false);
				smtpInboundProxyConfig.inboundProxyAccountForestTrackingEnabled = TransportAppConfig.GetConfigBool("InboundProxyAccountForestTrackerEnabled", false);
				smtpInboundProxyConfig.rejectBasedOnInboundProxyDestinationTrackingEnabled = TransportAppConfig.GetConfigBool("RejectBasedOnInboundProxyDestinationTrackerEnabled", false);
				smtpInboundProxyConfig.rejectBasedOnInboundProxyAccountForestTrackingEnabled = TransportAppConfig.GetConfigBool("RejectBasedOnInboundProxyAccountForestTrackerEnabled", false);
				smtpInboundProxyConfig.trackInboundProxyDestinationsInRcpt = TransportAppConfig.GetConfigBool("TrackInboundProxyDestinationsInRcpt", false);
				smtpInboundProxyConfig.perDestinationConnectionPercentageThreshold = TransportAppConfig.GetConfigInt("InboundProxyPerDestinationConnectionPercentage", 0, 100, 20);
				smtpInboundProxyConfig.perAccountForestConnectionPercentageThreshold = TransportAppConfig.GetConfigInt("InboundProxyPerAccountForestConnectionPercentage", 0, 100, 10);
				smtpInboundProxyConfig.inboundProxyDestinationTrackerLogInterval = TransportAppConfig.GetConfigTimeSpan("InboundProxyDestinationTrackerLogInterval", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0));
				smtpInboundProxyConfig.perDestinationConnectionThresholdDictionary = TransportAppConfig.SmtpInboundProxyConfig.GetPerDestinationConnectionThresholdDictionary(TransportAppConfig.GetConfigString("InboundProxyPerDestinationConnectionThresholds", null));
				smtpInboundProxyConfig.perAccountForestConnectionThresholdDictionary = TransportAppConfig.SmtpInboundProxyConfig.GetPerDestinationConnectionThresholdDictionary(TransportAppConfig.GetConfigString("InboundProxyPerAccountForestConnectionThresholds", null));
				return smtpInboundProxyConfig;
			}

			private static Dictionary<string, int> GetPerDestinationConnectionThresholdDictionary(string configString)
			{
				if (string.IsNullOrEmpty(configString))
				{
					return new Dictionary<string, int>();
				}
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				string[] array = configString.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						'='
					}, StringSplitOptions.RemoveEmptyEntries);
					int value;
					if (array3.Length == 2 && int.TryParse(array3[1], out value))
					{
						dictionary.Add(array3[0], value);
					}
				}
				return dictionary;
			}

			private static readonly ByteQuantifiedSize DefaultAccumulatedMessageSizeThreshold = ByteQuantifiedSize.FromMB(1UL);

			private static readonly TlsAuthLevel DefaultTlsAuthLevel = TlsAuthLevel.EncryptionOnly;

			private bool requireXProxyFromExtension;

			private bool requireTls;

			private bool useExternalDnsServers;

			private bool isInternalDestination;

			private List<IPAddress> proxyDestinations;

			private bool treatProxyDestinationAsExternal;

			private SmtpDomainWithSubdomains tlsDomain;

			private TlsAuthLevel tlsAuthLevel;

			private string externalCertificateSubject;

			private ByteQuantifiedSize accumulatedMessageSizeThreshold;

			private bool preserveTargetResponse;

			private bool sendNewXProxyFromArguments;

			private int perHostConnectionAttempts;

			private bool inboundProxyDestinationTrackingEnabled;

			private bool inboundProxyAccountForestTrackingEnabled;

			private bool rejectBasedOnInboundProxyDestinationTrackingEnabled;

			private bool trackInboundProxyDestinationsInRcpt;

			private int perDestinationConnectionPercentageThreshold;

			private TimeSpan inboundProxyDestinationTrackerLogInterval;

			private Dictionary<string, int> perDestinationConnectionThresholdDictionary;

			private Dictionary<string, int> perAccountForestConnectionThresholdDictionary;

			private bool rejectBasedOnInboundProxyAccountForestTrackingEnabled;

			private int perAccountForestConnectionPercentageThreshold;
		}

		public sealed class SmtpOutboundProxyConfig
		{
			public bool RequireTls
			{
				get
				{
					return this.requireTls;
				}
			}

			public bool UseExternalDnsServers
			{
				get
				{
					return this.useExternalDnsServers;
				}
			}

			public bool TreatProxyHopAsExternal
			{
				get
				{
					return this.treatProxyHopAsExternal;
				}
			}

			public SmtpDomainWithSubdomains TlsDomain
			{
				get
				{
					return this.tlsDomain;
				}
			}

			public TlsAuthLevel TlsAuthLevel
			{
				get
				{
					return this.tlsAuthLevel;
				}
			}

			public string ExternalCertificateSubject
			{
				get
				{
					return this.externalCertificateSubject;
				}
			}

			public int BulkRiskPoolPort
			{
				get
				{
					return this.bulkRiskPoolPort;
				}
			}

			public int HighRiskPoolPort
			{
				get
				{
					return this.highRiskPoolPort;
				}
			}

			public int LowRiskPoolPort
			{
				get
				{
					return this.lowRiskPoolPort;
				}
			}

			public Fqdn SendConnectorFqdn
			{
				get
				{
					return this.sendConnectorFqdn;
				}
			}

			public string ResourceForestMatchingDomains
			{
				get
				{
					return this.resourceForestMatchingDomains;
				}
			}

			public static TransportAppConfig.SmtpOutboundProxyConfig Load()
			{
				TransportAppConfig.SmtpOutboundProxyConfig smtpOutboundProxyConfig = new TransportAppConfig.SmtpOutboundProxyConfig();
				smtpOutboundProxyConfig.requireTls = TransportAppConfig.GetConfigBool("OutboundProxyRequireTls", true);
				smtpOutboundProxyConfig.useExternalDnsServers = TransportAppConfig.GetConfigBool("OutboundProxyUseExternalDnsServers", false);
				smtpOutboundProxyConfig.treatProxyHopAsExternal = TransportAppConfig.GetConfigBool("OutboundProxyTreatProxyHopAsExternal", false);
				smtpOutboundProxyConfig.tlsAuthLevel = TransportAppConfig.GetConfigEnum<TlsAuthLevel>("OutboundProxyTlsAuthLevel", TransportAppConfig.SmtpOutboundProxyConfig.DefaultTlsAuthLevel);
				smtpOutboundProxyConfig.tlsDomain = TransportAppConfig.GetConfigValue<SmtpDomainWithSubdomains>("OutboundProxyTlsDomain", (smtpOutboundProxyConfig.tlsAuthLevel == TlsAuthLevel.DomainValidation) ? new SmtpDomainWithSubdomains("mail.messaging.microsoft.com") : null, new TransportAppConfig.TryParse<SmtpDomainWithSubdomains>(SmtpDomainWithSubdomains.TryParse));
				smtpOutboundProxyConfig.externalCertificateSubject = TransportAppConfig.GetConfigString("OutboundProxyExternalCertificateSubject", null);
				smtpOutboundProxyConfig.bulkRiskPoolPort = TransportAppConfig.GetConfigInt("BulkRiskPoolPort", 0, 65535, 1028);
				smtpOutboundProxyConfig.highRiskPoolPort = TransportAppConfig.GetConfigInt("HighRiskPoolPort", 0, 65535, 1031);
				smtpOutboundProxyConfig.lowRiskPoolPort = TransportAppConfig.GetConfigInt("LowRiskPoolPort", 0, 65535, 1701);
				smtpOutboundProxyConfig.sendConnectorFqdn = TransportAppConfig.GetConfigValue<Fqdn>("OutboundProxySendConnectorFqdn", null, new TransportAppConfig.TryParse<Fqdn>(Fqdn.TryParse));
				smtpOutboundProxyConfig.resourceForestMatchingDomains = TransportAppConfig.GetConfigString("ResourceForestMatchingDomains", TransportAppConfig.SmtpOutboundProxyConfig.DefaultResourceForestMatchingDomains);
				return smtpOutboundProxyConfig;
			}

			private static readonly TlsAuthLevel DefaultTlsAuthLevel = TlsAuthLevel.EncryptionOnly;

			private static readonly string DefaultResourceForestMatchingDomains = "prod.outlook.com|prod.exchangelabs.com|protection.gbl|sdf.exchangelabs.com|ffo.gbl|prod.partner.outlook.cn|protectioncn.gbl|mgd.msft.net";

			private bool requireTls;

			private bool useExternalDnsServers;

			private SmtpDomainWithSubdomains tlsDomain;

			private TlsAuthLevel tlsAuthLevel;

			private bool treatProxyHopAsExternal;

			private string externalCertificateSubject;

			private int bulkRiskPoolPort;

			private int highRiskPoolPort;

			private int lowRiskPoolPort;

			private Fqdn sendConnectorFqdn;

			private string resourceForestMatchingDomains;
		}

		public sealed class DeliveryQueuePrioritizationConfig
		{
			public ByteQuantifiedSize AccumulatedMessageSizeThreshold
			{
				get
				{
					return this.accumulatedMessageSizeThreshold;
				}
			}

			public ByteQuantifiedSize AnonymousAccumulatedMessageSizeThreshold
			{
				get
				{
					return this.anonymousAccumulatedMessageSizeThreshold;
				}
			}

			public int AccumulatedRecipientCostLevel1Threshold
			{
				get
				{
					return this.accumulatedRecipientCostLevel1Threshold;
				}
			}

			public int AccumulatedRecipientCostLevel2Threshold
			{
				get
				{
					return this.accumulatedRecipientCostLevel2Threshold;
				}
			}

			public int AnonymousAccumulatedRecipientCostLevel1Threshold
			{
				get
				{
					return this.anonymousAccumulatedRecipientCostLevel1Threshold;
				}
			}

			public int AnonymousAccumulatedRecipientCostLevel2Threshold
			{
				get
				{
					return this.anonymousAccumulatedRecipientCostLevel2Threshold;
				}
			}

			public bool PrioritizationEnabled
			{
				get
				{
					return this.prioritizationEnabled;
				}
			}

			public bool PriorityHeaderPromotionEnabled
			{
				get
				{
					return this.priorityHeaderPromotionEnabled;
				}
			}

			public long AccumulatedRecipientCountLevel1Threshold { get; private set; }

			public long AccumulatedRecipientCountLevel2Threshold { get; private set; }

			public static TransportAppConfig.DeliveryQueuePrioritizationConfig Load()
			{
				return new TransportAppConfig.DeliveryQueuePrioritizationConfig
				{
					accumulatedMessageSizeThreshold = TransportAppConfig.GetConfigByteQuantifiedSize("DeliveryQueuePrioritizationAccumulatedMessageSizeThreshold", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL), TransportAppConfig.DeliveryQueuePrioritizationConfig.DefaultAccumulatedMessageSizeThreshold),
					anonymousAccumulatedMessageSizeThreshold = TransportAppConfig.GetConfigByteQuantifiedSize("DeliveryQueuePrioritizationAnonymousAccumulatedMessageSizeThreshold", ByteQuantifiedSize.FromBytes(0UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL), TransportAppConfig.DeliveryQueuePrioritizationConfig.DefaultAnonymousAccumulatedMessageSizeThreshold),
					accumulatedRecipientCostLevel1Threshold = TransportAppConfig.GetConfigInt("DeliveryQueuePrioritizationAccumulatedRecipientCostLevel1Threshold", 0, int.MaxValue, 500),
					accumulatedRecipientCostLevel2Threshold = TransportAppConfig.GetConfigInt("DeliveryQueuePrioritizationAccumulatedRecipientCostLevel2Threshold", 0, int.MaxValue, 2000),
					anonymousAccumulatedRecipientCostLevel1Threshold = TransportAppConfig.GetConfigInt("DeliveryQueuePrioritizationAnonymousAccumulatedRecipientCostLevel1Threshold", 0, int.MaxValue, 50),
					anonymousAccumulatedRecipientCostLevel2Threshold = TransportAppConfig.GetConfigInt("DeliveryQueuePrioritizationAnonymousAccumulatedRecipientCostLevel2Threshold", 0, int.MaxValue, 200),
					prioritizationEnabled = TransportAppConfig.GetConfigBool("DeliveryQueuePrioritizationEnabled", true),
					priorityHeaderPromotionEnabled = TransportAppConfig.GetConfigBool("PriorityHeaderPromotionEnabled", true),
					AccumulatedRecipientCountLevel1Threshold = TransportAppConfig.GetConfigValue<long>("DeliveryQueuePrioritizationAccumulatedRecipientCountLevel1Threshold", 1L, long.MaxValue, 10L, new TransportAppConfig.TryParse<long>(long.TryParse)),
					AccumulatedRecipientCountLevel2Threshold = TransportAppConfig.GetConfigValue<long>("DeliveryQueuePrioritizationAccumulatedRecipientCountLevel2Threshold", 1L, long.MaxValue, 40L, new TransportAppConfig.TryParse<long>(long.TryParse))
				};
			}

			private const int DefaultAccumulatedRecipientCostLevel1Threshold = 500;

			private const int DefaultAccumulatedRecipientCostLevel2Threshold = 2000;

			private const int DefaultAnonymousAccumulatedRecipientCostLevel1Threshold = 50;

			private const int DefaultAnonymousAccumulatedRecipientCostLevel2Threshold = 200;

			private const bool DefaultPrioritizationEnabled = true;

			private const long DefaultAccumulatedRecipientCountLevel1Threshold = 10L;

			private const long DefaultAccumulatedRecipientCountLevel2Threshold = 40L;

			private static readonly ByteQuantifiedSize DefaultAccumulatedMessageSizeThreshold = ByteQuantifiedSize.FromMB(1UL);

			private static readonly ByteQuantifiedSize DefaultAnonymousAccumulatedMessageSizeThreshold = ByteQuantifiedSize.FromMB(1UL);

			private ByteQuantifiedSize accumulatedMessageSizeThreshold;

			private ByteQuantifiedSize anonymousAccumulatedMessageSizeThreshold;

			private int accumulatedRecipientCostLevel1Threshold;

			private int accumulatedRecipientCostLevel2Threshold;

			private int anonymousAccumulatedRecipientCostLevel1Threshold;

			private int anonymousAccumulatedRecipientCostLevel2Threshold;

			private bool prioritizationEnabled;

			private bool priorityHeaderPromotionEnabled;
		}

		public interface ILegacyQueueConfig
		{
			TimeSpan MinLargeQueueDeferEventInterval { get; }

			TimeSpan MinQueueRetryOrSuspendDeferEventInterval { get; }

			TimeSpan MessageDeferEventCheckInterval { get; }

			TimeSpan QueueLoggingInterval { get; }

			int QueueLoggingThreshold { get; }

			bool QueueLoggingEnabled { get; }

			bool QueuedRecipientsByAgeTrackingEnabled { get; }

			TimeSpan MaxUpdateQueueBlockedInterval { get; }

			TimeSpan RecentPerfCounterTrackingInterval { get; }

			TimeSpan RecentPerfCounterTrackingBucketSize { get; }

			bool AsynchronousRetryQueue { get; }
		}

		public sealed class QueueConfig : TransportAppConfig.ILegacyQueueConfig
		{
			public TimeSpan MinLargeQueueDeferEventInterval
			{
				get
				{
					return this.minLargeQueueDeferEventInterval;
				}
			}

			public TimeSpan MinQueueRetryOrSuspendDeferEventInterval
			{
				get
				{
					return this.minQueueRetryOrSuspendDeferEventInterval;
				}
			}

			public TimeSpan MessageDeferEventCheckInterval
			{
				get
				{
					return this.messageDeferEventCheckInterval;
				}
			}

			public TimeSpan QueueLoggingInterval
			{
				get
				{
					return this.queueLoggingInterval;
				}
			}

			public int QueueLoggingThreshold
			{
				get
				{
					return this.queueLoggingThreshold;
				}
			}

			public bool QueueLoggingEnabled
			{
				get
				{
					return this.queueLoggingEnabled;
				}
			}

			public bool QueuedRecipientsByAgeTrackingEnabled
			{
				get
				{
					return this.queuedRecipientsByAgeTrackingEnabled;
				}
			}

			public TimeSpan MaxUpdateQueueBlockedInterval
			{
				get
				{
					return this.maxQueueUpdateBlockedInterval;
				}
			}

			public TimeSpan RecentPerfCounterTrackingInterval
			{
				get
				{
					return this.recentPerfCounterTrackingInterval;
				}
			}

			public TimeSpan RecentPerfCounterTrackingBucketSize
			{
				get
				{
					return this.recentPerfCounterTrackingBucketSize;
				}
			}

			public bool AsynchronousRetryQueue
			{
				get
				{
					return this.asynchronousRetryQueue;
				}
			}

			public TimeSpan QueueResubmitRetryTimeout
			{
				get
				{
					return this.queueResubmitRetryTimeout;
				}
			}

			public TimeSpan QueueResubmitRetryInterval
			{
				get
				{
					return this.queueResubmitRetryInterval;
				}
			}

			public TimeSpan SynchronousRetryQueueTimeout
			{
				get
				{
					return this.synchronousRetryQueueTimeout;
				}
			}

			public static TransportAppConfig.QueueConfig Load()
			{
				TransportAppConfig.QueueConfig queueConfig = new TransportAppConfig.QueueConfig();
				queueConfig.minLargeQueueDeferEventInterval = TransportAppConfig.GetConfigTimeSpan("MinLargeQueueDeferEventInterval", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TimeSpan.Zero);
				queueConfig.minQueueRetryOrSuspendDeferEventInterval = TransportAppConfig.GetConfigTimeSpan("MinQueueRetryOrSuspendDeferEventInterval", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TimeSpan.Zero);
				queueConfig.messageDeferEventCheckInterval = TransportAppConfig.GetConfigTimeSpan("MessageDeferEventCheckInterval", TimeSpan.FromSeconds(5.0), TimeSpan.FromHours(1.0), TimeSpan.FromSeconds(120.0));
				queueConfig.queueLoggingInterval = TransportAppConfig.GetConfigTimeSpan("QueueLoggingInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(1.0));
				queueConfig.queueLoggingThreshold = TransportAppConfig.GetConfigInt("QueueLoggingThreshold", 0, int.MaxValue, 10);
				queueConfig.maxQueueUpdateBlockedInterval = TransportAppConfig.GetConfigTimeSpan("MaxQueueUpdateBlockedInterval", TimeSpan.FromMinutes(5.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(15.0));
				queueConfig.queueLoggingEnabled = TransportAppConfig.GetConfigBool("QueueLogginEnabled", true);
				queueConfig.queuedRecipientsByAgeTrackingEnabled = TransportAppConfig.GetConfigBool("QueuedRecipientsByAgeTrackingEnabled", true);
				queueConfig.recentPerfCounterTrackingInterval = TransportAppConfig.GetConfigTimeSpan("RecentPerfCounterTrackingInterval", TimeSpan.Zero, TimeSpan.FromHours(6.0), TimeSpan.FromMinutes(5.0));
				queueConfig.recentPerfCounterTrackingBucketSize = TransportAppConfig.GetConfigTimeSpan("RecentPerfCounterTrackingBucketSize", TimeSpan.Zero, queueConfig.recentPerfCounterTrackingInterval, TimeSpan.FromSeconds(15.0));
				queueConfig.asynchronousRetryQueue = TransportAppConfig.GetConfigBool("AsynchronousRetryQueue", false);
				queueConfig.queueResubmitRetryTimeout = TransportAppConfig.GetConfigTimeSpan("QueueResubmitRetryTimeout", TimeSpan.Zero, TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(1.0));
				queueConfig.queueResubmitRetryInterval = TransportAppConfig.GetConfigTimeSpan("QueueResubmitRetryInterval", TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMinutes(1.0), TimeSpan.FromSeconds(5.0));
				queueConfig.synchronousRetryQueueTimeout = TransportAppConfig.GetConfigTimeSpan("SynchronousRetryQueueTimeout", TimeSpan.FromMinutes(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(10.0));
				return queueConfig;
			}

			private const string MinLargeQueueDeferEventIntervalString = "MinLargeQueueDeferEventInterval";

			private const string MinQueueRetryOrSuspendDeferEventIntervalString = "MinQueueRetryOrSuspendDeferEventInterval";

			private const string MessageDeferEventCheckIntervalString = "MessageDeferEventCheckInterval";

			private const string QueueLoggingIntervalString = "QueueLoggingInterval";

			private const string QueueLoggingThresholdString = "QueueLoggingThreshold";

			private const string QueueLogginEnabledString = "QueueLogginEnabled";

			private const string QueuedRecipientsByAgeTrackingEnabledString = "QueuedRecipientsByAgeTrackingEnabled";

			private const string MaxQueueUpdateBlockedIntervalString = "MaxQueueUpdateBlockedInterval";

			private TimeSpan minLargeQueueDeferEventInterval;

			private TimeSpan minQueueRetryOrSuspendDeferEventInterval;

			private TimeSpan messageDeferEventCheckInterval;

			private TimeSpan queueLoggingInterval;

			private bool queueLoggingEnabled;

			private bool queuedRecipientsByAgeTrackingEnabled;

			private int queueLoggingThreshold;

			private TimeSpan maxQueueUpdateBlockedInterval;

			private TimeSpan recentPerfCounterTrackingInterval;

			private TimeSpan recentPerfCounterTrackingBucketSize;

			private bool asynchronousRetryQueue;

			private TimeSpan queueResubmitRetryTimeout;

			private TimeSpan queueResubmitRetryInterval;

			private TimeSpan synchronousRetryQueueTimeout;
		}

		public sealed class DeliveryFailureConfig
		{
			public int DeliveryFailureMinSampleRouting5_4_4
			{
				get
				{
					return this.deliveryFailureMinSampleRouting544;
				}
			}

			public int DeliveryFailureMinSampleResolver5_1_4
			{
				get
				{
					return this.deliveryFailureMinSampleResolver514;
				}
			}

			public int DeliveryFailureMinSampleResolver5_2_0
			{
				get
				{
					return this.deliveryFailureMinSampleResolver520;
				}
			}

			public int DeliveryFailureMinSampleResolver5_2_4
			{
				get
				{
					return this.deliveryFailureMinSampleResolver524;
				}
			}

			public int DeliveryFailureMinSampleResolver5_4_6
			{
				get
				{
					return this.deliveryFailureMinSampleResolver546;
				}
			}

			public int DeliveryFailureMinSampleDeliverySMTP5_6_0
			{
				get
				{
					return this.deliveryFailureMinSampleDeliverySMTP560;
				}
			}

			public int DeliveryFailureMinSampleStoreDriver5_2_0
			{
				get
				{
					return this.deliveryFailureMinSampleStoreDriver520;
				}
			}

			public int DeliveryFailureMinSampleStoreDriver5_6_0
			{
				get
				{
					return this.deliveryFailureMinSampleStoreDriver560;
				}
			}

			public int DeliveryFailureMinSampleDeliveryAgent
			{
				get
				{
					return this.deliveryFailureMinSampleDeliveryAgent;
				}
			}

			public int DeliveryFailureMinSampleForeignConnector
			{
				get
				{
					return this.deliveryFailureMinSampleForeignConnector;
				}
			}

			public string DSNServerConnectorFqdn
			{
				get
				{
					return this.dsnServerConnectorFqdn;
				}
			}

			public static TransportAppConfig.DeliveryFailureConfig Load()
			{
				TransportAppConfig.DeliveryFailureConfig deliveryFailureConfig = new TransportAppConfig.DeliveryFailureConfig();
				deliveryFailureConfig.deliveryFailureMinSampleRouting544 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleRouting5_4_4", 0, int.MaxValue, 200);
				deliveryFailureConfig.deliveryFailureMinSampleResolver514 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleResolver5_1_4", 0, int.MaxValue, 0);
				deliveryFailureConfig.deliveryFailureMinSampleResolver520 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleResolver5_2_0", 0, int.MaxValue, 1000);
				deliveryFailureConfig.deliveryFailureMinSampleResolver524 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleResolver5_2_4", 0, int.MaxValue, 200);
				deliveryFailureConfig.deliveryFailureMinSampleResolver546 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleResolver5_4_6", 0, int.MaxValue, 0);
				deliveryFailureConfig.deliveryFailureMinSampleDeliverySMTP560 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleDeliverySMTP5_6_0", 0, int.MaxValue, 200);
				deliveryFailureConfig.deliveryFailureMinSampleStoreDriver520 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleStoreDriver5_2_0", 0, int.MaxValue, 500);
				deliveryFailureConfig.deliveryFailureMinSampleStoreDriver560 = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleStoreDriver5_6_0", 0, int.MaxValue, 100);
				deliveryFailureConfig.deliveryFailureMinSampleDeliveryAgent = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleDeliveryAgent", 0, int.MaxValue, 1000);
				deliveryFailureConfig.deliveryFailureMinSampleForeignConnector = TransportAppConfig.GetConfigInt("DeliveryFailureMinSampleForeignConnector", 0, int.MaxValue, 1000);
				deliveryFailureConfig.dsnServerConnectorFqdn = TransportAppConfig.GetConfigString("DSNServerConnectorFqdn", null);
				if (!string.IsNullOrEmpty(deliveryFailureConfig.dsnServerConnectorFqdn) && !DatacenterRegistry.IsForefrontForOffice())
				{
					throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, "Invalid Configuration: The DSNServerConnectorFqdn AppConfig parameter '{0}' is only valid on a FFO Hub role.", new object[]
					{
						deliveryFailureConfig.dsnServerConnectorFqdn
					}));
				}
				return deliveryFailureConfig;
			}

			private const int DefaultDeliveryFailureMinSampleRouting544 = 200;

			private const int DefaultDeliveryFailureMinSampleResolver514 = 0;

			private const int DefaultDeliveryFailureMinSampleResolver520 = 1000;

			private const int DefaultDeliveryFailureMinSampleResolver524 = 200;

			private const int DefaultDeliveryFailureMinSampleResolver546 = 0;

			private const int DefaultDeliveryFailureMinSampleDeliverySMTP560 = 200;

			private const int DefaultDeliveryFailureMinSampleStoreDriver520 = 500;

			private const int DefaultDeliveryFailureMinSampleStoreDriver560 = 100;

			private const int DefaultDeliveryFailureMinSampleDeliveryAgent = 1000;

			private const int DefaultDeliveryFailureMinSampleForeignConnector = 1000;

			private int deliveryFailureMinSampleRouting544;

			private int deliveryFailureMinSampleResolver514;

			private int deliveryFailureMinSampleResolver520;

			private int deliveryFailureMinSampleResolver524;

			private int deliveryFailureMinSampleResolver546;

			private int deliveryFailureMinSampleDeliverySMTP560;

			private int deliveryFailureMinSampleStoreDriver520;

			private int deliveryFailureMinSampleStoreDriver560;

			private int deliveryFailureMinSampleDeliveryAgent;

			private int deliveryFailureMinSampleForeignConnector;

			private string dsnServerConnectorFqdn;
		}

		public sealed class SecureMailConfig
		{
			public bool ClientCertificateChainValidationEnabled
			{
				get
				{
					return this.clientCertificateChainValidationEnabled;
				}
			}

			public bool TreatCRLTransientFailuresAsSuccessEnabled
			{
				get
				{
					return this.treatCRLTransientFailuresAsSuccessEnabled;
				}
			}

			public ByteQuantifiedSize CertificateValidationCacheMaxSize
			{
				get
				{
					return this.certificateValidationCacheMaxSize;
				}
			}

			public TimeSpan CertificateValidationCacheExpiryInterval
			{
				get
				{
					return this.certificateValidationCacheExpiryInterval;
				}
			}

			public TimeSpan CertificateValidationCacheTransientFailureExpiryInterval
			{
				get
				{
					return this.certificateValidationCacheTransientFailureExpiryInterval;
				}
			}

			public int SubjectAlternativeNameLimit
			{
				get
				{
					return this.subjectAlternativeNameLimit;
				}
			}

			public static TransportAppConfig.SecureMailConfig Load()
			{
				return new TransportAppConfig.SecureMailConfig
				{
					clientCertificateChainValidationEnabled = TransportAppConfig.GetConfigBool("ClientCertificateChainValidationEnabled", true),
					treatCRLTransientFailuresAsSuccessEnabled = TransportAppConfig.GetConfigBool("TreatCRLTransientFailuresAsSuccessEnabled", true),
					certificateValidationCacheMaxSize = TransportAppConfig.GetConfigByteQuantifiedSize("CertificateValidationCacheMaxSize", ByteQuantifiedSize.Zero, ByteQuantifiedSize.FromMB(100UL), ByteQuantifiedSize.FromMB(1UL)),
					certificateValidationCacheExpiryInterval = TransportAppConfig.GetConfigTimeSpan("CertificateValidationCacheExpiryInterval", TimeSpan.Zero, TimeSpan.FromHours(24.0), TimeSpan.FromHours(1.0)),
					certificateValidationCacheTransientFailureExpiryInterval = TransportAppConfig.GetConfigTimeSpan("CertificateValidationCacheTransientFailureExpiryInterval", TimeSpan.Zero, TimeSpan.FromHours(24.0), TimeSpan.FromMinutes(5.0)),
					subjectAlternativeNameLimit = TransportAppConfig.GetConfigInt("SubjectAlternativeNameLimit", 0, int.MaxValue, 256)
				};
			}

			private const bool DefaultClientCertificateChainValidationEnabled = true;

			private const bool DefaultTreatCRLTransientFailuresAsSuccessEnabled = true;

			private bool clientCertificateChainValidationEnabled;

			private bool treatCRLTransientFailuresAsSuccessEnabled;

			private ByteQuantifiedSize certificateValidationCacheMaxSize;

			private TimeSpan certificateValidationCacheExpiryInterval;

			private TimeSpan certificateValidationCacheTransientFailureExpiryInterval;

			private int subjectAlternativeNameLimit;
		}

		public sealed class LoggingConfig
		{
			public int SmtpSendLogBufferSize
			{
				get
				{
					return this.smtpSendLogBufferSize;
				}
			}

			public TimeSpan SmtpSendLogFlushInterval
			{
				get
				{
					return this.smtpSendLogFlushInterval;
				}
			}

			public TimeSpan SmtpSendLogAsyncInterval
			{
				get
				{
					return this.smtpSendLogAsyncInterval;
				}
			}

			public int SmtpRecvLogBufferSize
			{
				get
				{
					return this.smtpRecvLogBufferSize;
				}
			}

			public TimeSpan SmtpRecvLogFlushInterval
			{
				get
				{
					return this.smtpRecvLogFlushInterval;
				}
			}

			public TimeSpan SmtpRecvLogAsyncInterval
			{
				get
				{
					return this.smtpRecvLogAsyncInterval;
				}
			}

			public int ConnectivityLogBufferSize
			{
				get
				{
					return this.connectivityLogBufferSize;
				}
			}

			public TimeSpan ConnectivityLogFlushInterval
			{
				get
				{
					return this.connectivityLogFlushInterval;
				}
			}

			public TimeSpan ConnectivityLogAsyncInterval
			{
				get
				{
					return this.connectivityLogAsyncInterval;
				}
			}

			public int MsgTrkLogBufferSize
			{
				get
				{
					return this.msgTrkLogBufferSize;
				}
			}

			public TimeSpan MsgTrkLogFlushInterval
			{
				get
				{
					return this.msgTrkLogFlushInterval;
				}
			}

			public int MaxMsgTrkAgenInfoSize
			{
				get
				{
					return this.maxMsgTrkAgentInfoSize;
				}
			}

			public int TransportWlmLogBufferSize
			{
				get
				{
					return this.transportWlmLogBufferSize;
				}
			}

			public TimeSpan TransportWlmLogFlushInterval
			{
				get
				{
					return this.transportWlmLogFlushInterval;
				}
			}

			public static TransportAppConfig.LoggingConfig Load()
			{
				return new TransportAppConfig.LoggingConfig
				{
					smtpSendLogBufferSize = TransportAppConfig.GetConfigInt("SmtpSendLogBufferSize", 0, 10485760, 524288),
					smtpSendLogFlushInterval = TransportAppConfig.GetConfigTimeSpan("SmtpSendLogFlushInterval", TransportAppConfig.LoggingConfig.MinLogFlushInterval, TransportAppConfig.LoggingConfig.MaxLogFlushInterval, TransportAppConfig.LoggingConfig.DefaultSmtpSendLogFlushInterval),
					smtpSendLogAsyncInterval = TransportAppConfig.GetConfigTimeSpan("SmtpSendLogAsyncInterval", TransportAppConfig.LoggingConfig.MinLogAsyncInterval, TransportAppConfig.LoggingConfig.MaxLogAsyncInterval, TransportAppConfig.LoggingConfig.DefaultSmtpSendLogAsyncInterval),
					smtpRecvLogBufferSize = TransportAppConfig.GetConfigInt("SmtpRecvLogBufferSize", 0, 10485760, 524288),
					smtpRecvLogFlushInterval = TransportAppConfig.GetConfigTimeSpan("SmtpRecvLogFlushInterval", TransportAppConfig.LoggingConfig.MinLogFlushInterval, TransportAppConfig.LoggingConfig.MaxLogFlushInterval, TransportAppConfig.LoggingConfig.DefaultSmtpRecvLogFlushInterval),
					smtpRecvLogAsyncInterval = TransportAppConfig.GetConfigTimeSpan("SmtpRecvLogAsyncInterval", TransportAppConfig.LoggingConfig.MinLogAsyncInterval, TransportAppConfig.LoggingConfig.MaxLogAsyncInterval, TransportAppConfig.LoggingConfig.DefaultSmtpRecvLogAsyncInterval),
					connectivityLogBufferSize = TransportAppConfig.GetConfigInt("ConnectivityLogBufferSize", 0, 10485760, 524288),
					connectivityLogFlushInterval = TransportAppConfig.GetConfigTimeSpan("ConnectivityLogFlushInterval", TransportAppConfig.LoggingConfig.MinLogFlushInterval, TransportAppConfig.LoggingConfig.MaxLogFlushInterval, TransportAppConfig.LoggingConfig.DefaultConnectivityLogFlushInterval),
					connectivityLogAsyncInterval = TransportAppConfig.GetConfigTimeSpan("ConnectivityLogAsyncInterval", TransportAppConfig.LoggingConfig.MinLogAsyncInterval, TransportAppConfig.LoggingConfig.MaxLogAsyncInterval, TransportAppConfig.LoggingConfig.DefaultConnectivityLogAsyncInterval),
					msgTrkLogBufferSize = TransportAppConfig.GetConfigInt("MsgTrkLogBufferSize", 0, 10485760, 0),
					msgTrkLogFlushInterval = TransportAppConfig.GetConfigTimeSpan("MsgTrkLogFlushInterval", TransportAppConfig.LoggingConfig.MinLogFlushInterval, TransportAppConfig.LoggingConfig.MaxLogFlushInterval, TransportAppConfig.LoggingConfig.DefaultMsgTrkLogFlushInterval),
					maxMsgTrkAgentInfoSize = TransportAppConfig.GetConfigInt("MaxMsgTrkAgentInfoSize", 0, int.MaxValue, TransportAppConfig.LoggingConfig.DefaultMaxMsgTrkAgentInfoSize),
					transportWlmLogBufferSize = TransportAppConfig.GetConfigInt("TransportWlmLogBufferSize", 0, 10485760, 65536),
					transportWlmLogFlushInterval = TransportAppConfig.GetConfigTimeSpan("TransportWlmLogFlushInterval", TransportAppConfig.LoggingConfig.MinLogFlushInterval, TransportAppConfig.LoggingConfig.MaxLogFlushInterval, TransportAppConfig.LoggingConfig.DefaultTransportWlmLogFlushInterval)
				};
			}

			private const string SmtpSendLogBufferSizeKey = "SmtpSendLogBufferSize";

			private const string SmtpSendLogFlushIntervalKey = "SmtpSendLogFlushInterval";

			private const string SmtpSendLogAsyncIntervalKey = "SmtpSendLogAsyncInterval";

			private const string SmtpRecvLogBufferSizeKey = "SmtpRecvLogBufferSize";

			private const string SmtpRecvLogFlushIntervalKey = "SmtpRecvLogFlushInterval";

			private const string SmtpRecvLogAsyncIntervalKey = "SmtpRecvLogAsyncInterval";

			private const string ConnectivityLogBufferSizeKey = "ConnectivityLogBufferSize";

			private const string ConnectivityLogFlushIntervalKey = "ConnectivityLogFlushInterval";

			private const string ConnectivityLogAsyncIntervalKey = "ConnectivityLogAsyncInterval";

			private const string MsgTrkLogBufferSizeKey = "MsgTrkLogBufferSize";

			private const string MsgTrkLogFlushIntervalKey = "MsgTrkLogFlushInterval";

			private const string MaxMsgTrkAgentInfoSizeKey = "MaxMsgTrkAgentInfoSize";

			private const string TransportWlmLogBufferSizeKey = "TransportWlmLogBufferSize";

			private const string TransportWlmLogFlushIntervalKey = "TransportWlmLogFlushInterval";

			private const int DefaultSmtpSendLogBufferSize = 524288;

			private const int DefaultSmtpRecvLogBufferSize = 524288;

			private const int DefaultConnectivityLogBufferSize = 524288;

			private const int DefaultMsgTrkLogBufferSize = 0;

			private const int DefaultTransportWlmLogBufferSize = 65536;

			private const int MaxLogBufferSize = 10485760;

			private const int MinLogBufferSize = 0;

			private static readonly TimeSpan DefaultSmtpSendLogFlushInterval = TimeSpan.FromSeconds(30.0);

			private static readonly TimeSpan DefaultSmtpSendLogAsyncInterval = TimeSpan.FromSeconds(1.0);

			private static readonly TimeSpan DefaultSmtpRecvLogFlushInterval = TimeSpan.FromSeconds(30.0);

			private static readonly TimeSpan DefaultSmtpRecvLogAsyncInterval = TimeSpan.FromSeconds(1.0);

			private static readonly TimeSpan DefaultConnectivityLogFlushInterval = TimeSpan.FromSeconds(30.0);

			private static readonly TimeSpan DefaultConnectivityLogAsyncInterval = TimeSpan.FromSeconds(1.0);

			private static readonly TimeSpan DefaultMsgTrkLogFlushInterval = TimeSpan.MaxValue;

			private static readonly int DefaultMaxMsgTrkAgentInfoSize = 3072;

			private static readonly TimeSpan DefaultTransportWlmLogFlushInterval = TimeSpan.FromSeconds(60.0);

			private static readonly TimeSpan MaxLogFlushInterval = TimeSpan.MaxValue;

			private static readonly TimeSpan MinLogFlushInterval = TimeSpan.Zero;

			private static readonly TimeSpan MaxLogAsyncInterval = TimeSpan.FromSeconds(20.0);

			private static readonly TimeSpan MinLogAsyncInterval = TimeSpan.FromMilliseconds(100.0);

			private int smtpSendLogBufferSize;

			private TimeSpan smtpSendLogFlushInterval;

			private TimeSpan smtpSendLogAsyncInterval;

			private int smtpRecvLogBufferSize;

			private TimeSpan smtpRecvLogFlushInterval;

			private TimeSpan smtpRecvLogAsyncInterval;

			private int connectivityLogBufferSize;

			private TimeSpan connectivityLogFlushInterval;

			private TimeSpan connectivityLogAsyncInterval;

			private int msgTrkLogBufferSize;

			private TimeSpan msgTrkLogFlushInterval;

			private int maxMsgTrkAgentInfoSize;

			private int transportWlmLogBufferSize;

			private TimeSpan transportWlmLogFlushInterval;
		}

		public sealed class FlowControlLogConfig : IFlowControlLogConfig
		{
			public TimeSpan AsyncInterval
			{
				get
				{
					return this.asyncInterval;
				}
			}

			public int BufferSize
			{
				get
				{
					return this.bufferSize;
				}
			}

			public TimeSpan FlushInterval
			{
				get
				{
					return this.flushInterval;
				}
			}

			public TimeSpan SummaryLoggingInterval
			{
				get
				{
					return this.summaryLoggingInterval;
				}
			}

			public TimeSpan SummaryBucketLength
			{
				get
				{
					return this.summaryBucketLength;
				}
			}

			public int MaxSummaryLinesLogged
			{
				get
				{
					return this.maxSummaryLinesLogged;
				}
			}

			public static TransportAppConfig.FlowControlLogConfig Load()
			{
				return new TransportAppConfig.FlowControlLogConfig
				{
					bufferSize = TransportAppConfig.GetConfigInt("FlowControlLogBufferSize", 0, 10485760, 65536),
					flushInterval = TransportAppConfig.GetConfigTimeSpan("FlowControlLogFlushInterval", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromSeconds(60.0)),
					asyncInterval = TransportAppConfig.GetConfigTimeSpan("FlowControlLogAsyncInterval", TimeSpan.FromMilliseconds(100.0), TimeSpan.FromSeconds(20.0), TimeSpan.FromSeconds(15.0)),
					summaryLoggingInterval = TransportAppConfig.GetConfigTimeSpan("FlowControlLogSummaryInterval", TimeSpan.Zero, TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(15.0)),
					summaryBucketLength = TransportAppConfig.GetConfigTimeSpan("FlowControlLogSummaryBucketLength", TimeSpan.Zero, TimeSpan.FromHours(1.0), TimeSpan.FromSeconds(30.0)),
					maxSummaryLinesLogged = TransportAppConfig.GetConfigInt("FlowControlLogMaxSummaryLinesLogged", 0, int.MaxValue, 100)
				};
			}

			private TimeSpan asyncInterval;

			private int bufferSize;

			private TimeSpan flushInterval;

			private TimeSpan summaryLoggingInterval;

			private TimeSpan summaryBucketLength;

			private int maxSummaryLinesLogged;
		}

		public sealed class ConditionalThrottlingConfig : IWaitConditionManagerConfig
		{
			public ConditionalThrottlingConfig()
			{
			}

			public ConditionalThrottlingConfig(bool isCategorizer, TransportAppConfig.ConditionalThrottlingConfig clone)
			{
				this.isCategorizer = isCategorizer;
				this.aboveThresholdThrottlingBehaviorEnabled = clone.aboveThresholdThrottlingBehaviorEnabled;
				this.categorizerProcessingTimeThrottlingEnabled = clone.categorizerProcessingTimeThrottlingEnabled;
				this.categorizerTenantThrottlingEnabled = clone.categorizerTenantThrottlingEnabled;
				this.categorizerSenderThrottlingEnabled = clone.categorizerSenderThrottlingEnabled;
				this.categorizerThrottlingHistoryBucketSize = clone.categorizerThrottlingHistoryBucketSize;
				this.categorizerThrottlingHistoryInterval = clone.categorizerThrottlingHistoryInterval;
				this.deliverySenderThrottlingEnabled = clone.deliverySenderThrottlingEnabled;
				this.deliveryTenantThrottlingEnabled = clone.deliveryTenantThrottlingEnabled;
				this.deliveryThrottlingHistoryBucketSize = clone.deliveryThrottlingHistoryBucketSize;
				this.deliveryThrottlingHistoryInterval = clone.deliveryThrottlingHistoryInterval;
				this.throttlingProcessingMinThreshold = clone.throttlingProcessingMinThreshold;
				this.throttlingMemoryMaxThreshold = clone.throttlingMemoryMaxThreshold;
				this.throttlingMemoryMinThreshold = clone.throttlingMemoryMinThreshold;
				this.dehydrationThreshold = clone.dehydrationThreshold;
				this.emptyThrottlingCostRemovalInterval = clone.emptyThrottlingCostRemovalInterval;
				this.maxAllowedCapacityPercentage = clone.maxAllowedCapacityPercentage;
				this.categorizerThrottlingAsyncThreadPercentage = clone.categorizerThrottlingAsyncThreadPercentage;
			}

			public bool CategorizerTenantThrottlingEnabled
			{
				get
				{
					return this.categorizerTenantThrottlingEnabled;
				}
				internal set
				{
					this.categorizerTenantThrottlingEnabled = value;
				}
			}

			public bool CategorizerSenderThrottlingEnabled
			{
				get
				{
					return this.categorizerSenderThrottlingEnabled;
				}
				internal set
				{
					this.categorizerSenderThrottlingEnabled = value;
				}
			}

			public bool DeliveryTenantThrottlingEnabled
			{
				get
				{
					return this.deliveryTenantThrottlingEnabled;
				}
				internal set
				{
					this.deliveryTenantThrottlingEnabled = value;
				}
			}

			public bool DeliverySenderThrottlingEnabled
			{
				get
				{
					return this.deliverySenderThrottlingEnabled;
				}
				internal set
				{
					this.deliverySenderThrottlingEnabled = value;
				}
			}

			public bool QuotaOverrideEnabled
			{
				get
				{
					return this.isCategorizer && this.categorizerTenantThrottlingEnabled && Components.TransportAppConfig.ProcessingQuota.EnforceProcessingQuota;
				}
			}

			public bool TestQuotaOverrideEnabled
			{
				get
				{
					return this.isCategorizer && this.categorizerTenantThrottlingEnabled && Components.TransportAppConfig.ProcessingQuota.TestProcessingQuota;
				}
			}

			public bool CategorizerProcessingTimeThrottlingEnabled
			{
				get
				{
					return this.categorizerProcessingTimeThrottlingEnabled;
				}
				internal set
				{
					this.categorizerProcessingTimeThrottlingEnabled = value;
				}
			}

			public TimeSpan CategorizerThrottlingHistoryInterval
			{
				get
				{
					return this.categorizerThrottlingHistoryInterval;
				}
				internal set
				{
					this.categorizerThrottlingHistoryInterval = value;
				}
			}

			public TimeSpan CategorizerThrottlingHistoryBucketSize
			{
				get
				{
					return this.categorizerThrottlingHistoryBucketSize;
				}
				internal set
				{
					this.categorizerThrottlingHistoryBucketSize = value;
				}
			}

			public TimeSpan DeliveryThrottlingHistoryInterval
			{
				get
				{
					return this.deliveryThrottlingHistoryInterval;
				}
				internal set
				{
					this.deliveryThrottlingHistoryInterval = value;
				}
			}

			public TimeSpan DeliveryThrottlingHistoryBucketSize
			{
				get
				{
					return this.deliveryThrottlingHistoryBucketSize;
				}
				internal set
				{
					this.deliveryThrottlingHistoryBucketSize = value;
				}
			}

			public int CategorizerThrottlingAsyncThreadPercentage
			{
				get
				{
					return this.categorizerThrottlingAsyncThreadPercentage;
				}
				internal set
				{
					this.categorizerThrottlingAsyncThreadPercentage = value;
				}
			}

			public TimeSpan ThrottlingProcessingMinThreshold
			{
				get
				{
					return this.throttlingProcessingMinThreshold;
				}
				internal set
				{
					this.throttlingProcessingMinThreshold = value;
				}
			}

			public ByteQuantifiedSize ThrottlingMemoryMinThreshold
			{
				get
				{
					return this.throttlingMemoryMinThreshold;
				}
				internal set
				{
					this.throttlingMemoryMinThreshold = value;
				}
			}

			public ByteQuantifiedSize ThrottlingMemoryMaxThreshold
			{
				get
				{
					return this.throttlingMemoryMaxThreshold;
				}
			}

			public bool AboveThresholdThrottlingBehaviorEnabled
			{
				get
				{
					return this.aboveThresholdThrottlingBehaviorEnabled;
				}
				internal set
				{
					this.aboveThresholdThrottlingBehaviorEnabled = value;
				}
			}

			public int MaxAllowedCapacityPercentage
			{
				get
				{
					return this.maxAllowedCapacityPercentage;
				}
				internal set
				{
					this.maxAllowedCapacityPercentage = value;
				}
			}

			public TimeSpan EmptyThrottlingCostRemovalInterval
			{
				get
				{
					return this.emptyThrottlingCostRemovalInterval;
				}
				internal set
				{
					this.emptyThrottlingCostRemovalInterval = value;
				}
			}

			public int LockedMessageDehydrationThreshold
			{
				get
				{
					return this.dehydrationThreshold;
				}
				internal set
				{
					this.dehydrationThreshold = value;
				}
			}

			public bool TenantThrottlingEnabled
			{
				get
				{
					if (!this.isCategorizer)
					{
						return this.deliveryTenantThrottlingEnabled;
					}
					return this.categorizerTenantThrottlingEnabled;
				}
			}

			public bool SenderThrottlingEnabled
			{
				get
				{
					if (!this.isCategorizer)
					{
						return this.deliverySenderThrottlingEnabled;
					}
					return this.categorizerSenderThrottlingEnabled;
				}
			}

			public bool ProcessingTimeThrottlingEnabled
			{
				get
				{
					return this.isCategorizer && this.categorizerProcessingTimeThrottlingEnabled;
				}
			}

			public TimeSpan ThrottlingHistoryInterval
			{
				get
				{
					if (!this.isCategorizer)
					{
						return this.deliveryThrottlingHistoryInterval;
					}
					return this.categorizerThrottlingHistoryInterval;
				}
			}

			public TimeSpan ThrottlingHistoryBucketSize
			{
				get
				{
					if (!this.isCategorizer)
					{
						return this.deliveryThrottlingHistoryBucketSize;
					}
					return this.categorizerThrottlingHistoryBucketSize;
				}
			}

			public TimeSpan LockExpirationInterval
			{
				get
				{
					return this.lockExpirationInterval;
				}
			}

			public TimeSpan LockExpirationCheckPeriod
			{
				get
				{
					return this.lockExpirationCheckPeriod;
				}
			}

			public static TransportAppConfig.ConditionalThrottlingConfig Load()
			{
				TransportAppConfig.ConditionalThrottlingConfig conditionalThrottlingConfig = new TransportAppConfig.ConditionalThrottlingConfig();
				conditionalThrottlingConfig.categorizerTenantThrottlingEnabled = (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.TenantThrottling.Enabled && TransportAppConfig.GetConfigBool("CategorizerTenantThrottlingEnabled", VariantConfiguration.InvariantNoFlightingSnapshot.Transport.TenantThrottling.Enabled));
				conditionalThrottlingConfig.categorizerSenderThrottlingEnabled = TransportAppConfig.GetConfigBool("CategorizerSenderThrottlingEnabled", TransportAppConfig.ConditionalThrottlingConfig.DefaultSenderThrottlingEnabled);
				conditionalThrottlingConfig.deliveryTenantThrottlingEnabled = (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.TenantThrottling.Enabled && TransportAppConfig.GetConfigBool("DeliveryTenantThrottlingEnabled", VariantConfiguration.InvariantNoFlightingSnapshot.Transport.TenantThrottling.Enabled));
				conditionalThrottlingConfig.deliverySenderThrottlingEnabled = TransportAppConfig.GetConfigBool("DeliverySenderThrottlingEnabled", TransportAppConfig.ConditionalThrottlingConfig.DefaultSenderThrottlingEnabled);
				conditionalThrottlingConfig.categorizerProcessingTimeThrottlingEnabled = TransportAppConfig.GetConfigBool("CategorizerProcessingTimeThrottlingEnabled", TransportAppConfig.ConditionalThrottlingConfig.DefaultCategorizerProcessingTimeThrottlingEnabled);
				conditionalThrottlingConfig.categorizerThrottlingHistoryInterval = TransportAppConfig.GetConfigTimeSpan("CategorizerThrottlingHistoryInterval", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultCategorizerThrottlingHistoryInterval);
				conditionalThrottlingConfig.categorizerThrottlingHistoryBucketSize = TransportAppConfig.GetConfigTimeSpan("CategorizerThrottlingHistoryBucketSize", TimeSpan.FromSeconds(5.0), conditionalThrottlingConfig.categorizerThrottlingHistoryInterval, TransportAppConfig.ConditionalThrottlingConfig.DefaultCategorizerThrottlingHistoryBucketSize);
				conditionalThrottlingConfig.deliveryThrottlingHistoryInterval = TransportAppConfig.GetConfigTimeSpan("DeliveryThrottlingHistoryInterval", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultDeliveryThrottlingHistoryInterval);
				conditionalThrottlingConfig.deliveryThrottlingHistoryBucketSize = TransportAppConfig.GetConfigTimeSpan("DeliveryThrottlingHistoryBucketSize", TimeSpan.FromSeconds(5.0), conditionalThrottlingConfig.deliveryThrottlingHistoryInterval, TransportAppConfig.ConditionalThrottlingConfig.DefaultDeliveryThrottlingHistoryBucketSize);
				conditionalThrottlingConfig.throttlingProcessingMinThreshold = TransportAppConfig.GetConfigTimeSpan("ThrottlingProcessingTimeMinThreshold", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultThrottlingProcessingMinThreshold);
				conditionalThrottlingConfig.throttlingMemoryMinThreshold = TransportAppConfig.GetConfigByteQuantifiedSize("ThrottlingMemoryMinThreshold", ByteQuantifiedSize.Zero, ByteQuantifiedSize.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultThrottlingMemoryMinThreshold);
				conditionalThrottlingConfig.throttlingMemoryMaxThreshold = TransportAppConfig.GetConfigByteQuantifiedSize("ThrottlingMemoryMaxThreshold", ByteQuantifiedSize.Zero, ByteQuantifiedSize.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultThrottlingMemoryMaxThreshold);
				conditionalThrottlingConfig.aboveThresholdThrottlingBehaviorEnabled = TransportAppConfig.GetConfigBool("AboveThresholdThrottlingBehaviorEnabled", TransportAppConfig.ConditionalThrottlingConfig.DefaultAllowAboveThresholdThrottlingBehaviorEnabled);
				conditionalThrottlingConfig.maxAllowedCapacityPercentage = TransportAppConfig.GetConfigInt("MaxAllowedCapacityPercentageThrotlling", 0, 100, TransportAppConfig.ConditionalThrottlingConfig.DefaultMaxAllowedCapacityPercentage);
				conditionalThrottlingConfig.emptyThrottlingCostRemovalInterval = TransportAppConfig.GetConfigTimeSpan("EmptyThrotllingCostRemovalInterval", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultEmptyThrottlingCostRemovalInterval);
				conditionalThrottlingConfig.dehydrationThreshold = TransportAppConfig.GetConfigInt("ConditionalLockingDehydrationThreshold", 0, int.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultDehydrationThreshold);
				conditionalThrottlingConfig.categorizerThrottlingAsyncThreadPercentage = TransportAppConfig.GetConfigInt("CategorizerThrottlingAsyncThreadPercentage", 0, 100, TransportAppConfig.ConditionalThrottlingConfig.DefaultCategorizerThrottlingAsyncThreadPercentage);
				conditionalThrottlingConfig.lockExpirationInterval = TransportAppConfig.GetConfigTimeSpan("LockExpirationInterval", TimeSpan.Zero, TimeSpan.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultLockExpirationInterval);
				conditionalThrottlingConfig.lockExpirationCheckPeriod = TransportAppConfig.GetConfigTimeSpan("LockExpirationCheckPeriod", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TransportAppConfig.ConditionalThrottlingConfig.DefaultLockExpirationCheckPeriod);
				return conditionalThrottlingConfig;
			}

			internal IWaitConditionManagerConfig GetConfig(bool forCategorizer)
			{
				return new TransportAppConfig.ConditionalThrottlingConfig(forCategorizer, this);
			}

			private const string CategorizerTenantThrottlingEnabledString = "CategorizerTenantThrottlingEnabled";

			private const string CategorizerSenderThrottlingEnabledString = "CategorizerSenderThrottlingEnabled";

			private const string DeliveryTenantThrottlingEnabledString = "DeliveryTenantThrottlingEnabled";

			private const string DeliverySenderThrottlingEnabledString = "DeliverySenderThrottlingEnabled";

			private const string CategorizerProcessingTimeThrottlingEnabledString = "CategorizerProcessingTimeThrottlingEnabled";

			private const string CategorizerThrottlingHistoryIntervalString = "CategorizerThrottlingHistoryInterval";

			private const string CategorizerThrottlingHistoryBucketSizeString = "CategorizerThrottlingHistoryBucketSize";

			private const string DeliveryThrottlingHistoryIntervalString = "DeliveryThrottlingHistoryInterval";

			private const string DeliveryThrottlingHistoryBucketSizeString = "DeliveryThrottlingHistoryBucketSize";

			private const string ThrottlingProcessingTimeMinThresholdString = "ThrottlingProcessingTimeMinThreshold";

			private const string ThrottlingMemoryMinThresholdString = "ThrottlingMemoryMinThreshold";

			private const string ThrottlingMemoryMaxThresholdString = "ThrottlingMemoryMaxThreshold";

			private const string AboveThresholdThrottlingBehaviorEnabledString = "AboveThresholdThrottlingBehaviorEnabled";

			private const string MaxAllowedCapacityPercentageThrotllingString = "MaxAllowedCapacityPercentageThrotlling";

			private const string EmptyThrottlingCostRemovalIntervalString = "EmptyThrotllingCostRemovalInterval";

			private const string DehydrationThresholdString = "ConditionalLockingDehydrationThreshold";

			private const string CategorizerThrottlingAsyncThreadPercentageString = "CategorizerThrottlingAsyncThreadPercentage";

			private const string LockExpirationIntervalString = "LockExpirationInterval";

			private const string LockExpirationCheckPeriodString = "LockExpirationCheckPeriod";

			private static readonly bool DefaultSenderThrottlingEnabled = false;

			private static readonly bool DefaultCategorizerProcessingTimeThrottlingEnabled = true;

			private static readonly TimeSpan DefaultCategorizerThrottlingHistoryInterval = TimeSpan.FromMinutes(2.0);

			private static readonly TimeSpan DefaultCategorizerThrottlingHistoryBucketSize = TimeSpan.FromSeconds(5.0);

			private static readonly TimeSpan DefaultDeliveryThrottlingHistoryInterval = TimeSpan.FromMinutes(2.0);

			private static readonly TimeSpan DefaultDeliveryThrottlingHistoryBucketSize = TimeSpan.FromSeconds(5.0);

			private static readonly TimeSpan DefaultThrottlingProcessingMinThreshold = TimeSpan.FromSeconds(1.0);

			private static readonly ByteQuantifiedSize DefaultThrottlingMemoryMinThreshold = ByteQuantifiedSize.FromKB(50UL);

			private static readonly ByteQuantifiedSize DefaultThrottlingMemoryMaxThreshold = ByteQuantifiedSize.FromMB(500UL);

			private static readonly bool DefaultAllowAboveThresholdThrottlingBehaviorEnabled = true;

			private static readonly int DefaultMaxAllowedCapacityPercentage = 85;

			private static readonly TimeSpan DefaultEmptyThrottlingCostRemovalInterval = TimeSpan.FromSeconds(30.0);

			private static readonly int DefaultDehydrationThreshold = 2500;

			private static readonly int DefaultCategorizerThrottlingAsyncThreadPercentage = 50;

			private static readonly TimeSpan DefaultLockExpirationInterval = TimeSpan.FromMinutes(10.0);

			private static readonly TimeSpan DefaultLockExpirationCheckPeriod = TimeSpan.FromMinutes(1.0);

			private readonly bool isCategorizer;

			private bool categorizerTenantThrottlingEnabled;

			private bool categorizerSenderThrottlingEnabled;

			private bool deliveryTenantThrottlingEnabled;

			private bool deliverySenderThrottlingEnabled;

			private bool categorizerProcessingTimeThrottlingEnabled;

			private TimeSpan categorizerThrottlingHistoryInterval;

			private TimeSpan categorizerThrottlingHistoryBucketSize;

			private TimeSpan deliveryThrottlingHistoryInterval;

			private TimeSpan deliveryThrottlingHistoryBucketSize;

			private TimeSpan throttlingProcessingMinThreshold;

			private ByteQuantifiedSize throttlingMemoryMinThreshold;

			private ByteQuantifiedSize throttlingMemoryMaxThreshold;

			private bool aboveThresholdThrottlingBehaviorEnabled;

			private int maxAllowedCapacityPercentage;

			private TimeSpan emptyThrottlingCostRemovalInterval;

			private int dehydrationThreshold;

			private int categorizerThrottlingAsyncThreadPercentage;

			private TimeSpan lockExpirationInterval;

			private TimeSpan lockExpirationCheckPeriod;
		}

		public sealed class TransportRulesConfig
		{
			private TransportRulesConfig()
			{
			}

			public int TransportRuleLoadTimeReportingThresholdInMilliseconds { get; private set; }

			public int TransportRuleExecutionTimeReportingThresholdInBytesPerSecond { get; private set; }

			public int TransportRuleLoadTimeAlertingThresholdInMilliseconds { get; private set; }

			public int TransportRuleExecutionTimeAlertingThresholdInBytesPerSecond { get; private set; }

			public int TransportRuleMinFipsTimeoutInMilliseconds { get; private set; }

			public Dictionary<string, uint> ScanVelocities { get; private set; }

			public int TransportRuleMaxForkCount { get; private set; }

			public static TransportAppConfig.TransportRulesConfig Load()
			{
				TransportAppConfig.TransportRulesConfig transportRulesConfig = new TransportAppConfig.TransportRulesConfig();
				transportRulesConfig.TransportRuleLoadTimeReportingThresholdInMilliseconds = TransportAppConfig.GetConfigInt("TransportRuleLoadTimeReportingThresholdInMilliseconds", 0, int.MaxValue, 1000);
				transportRulesConfig.TransportRuleExecutionTimeReportingThresholdInBytesPerSecond = TransportAppConfig.GetConfigInt("TransportRuleExecutionTimeReportingThresholdInBytesPerSecond", 1, int.MaxValue, 307200);
				transportRulesConfig.TransportRuleLoadTimeAlertingThresholdInMilliseconds = TransportAppConfig.GetConfigInt("TransportRuleLoadTimeAlertingThresholdInMilliseconds", 0, int.MaxValue, 30000);
				transportRulesConfig.TransportRuleExecutionTimeAlertingThresholdInBytesPerSecond = TransportAppConfig.GetConfigInt("TransportRuleExecutionTimeAlertingThresholdInBytesPerSecond", 1, int.MaxValue, 1024);
				transportRulesConfig.TransportRuleMinFipsTimeoutInMilliseconds = TransportAppConfig.GetConfigInt("TransportRuleMinFipsTimeoutInMilliseconds", 0, int.MaxValue, 60000);
				transportRulesConfig.TransportRuleMaxForkCount = TransportAppConfig.GetConfigInt("TransportRuleMaxForkCount", 0, int.MaxValue, 200);
				transportRulesConfig.ScanVelocities = new Dictionary<string, uint>(TransportAppConfig.TransportRulesConfig.defaultScanVelocities);
				TransportAppConfig.ConfigurationListsSection configurationListsSection = ConfigurationManager.GetSection("customLists") as TransportAppConfig.ConfigurationListsSection;
				if (configurationListsSection != null)
				{
					List<KeyValuePair<string, uint>> configList = TransportAppConfig.GetConfigList<KeyValuePair<string, uint>>(configurationListsSection.TransportRulesScanVelocities, new TransportAppConfig.TryParse<KeyValuePair<string, uint>>(TransportAppConfig.TransportRulesConfig.TryParse));
					foreach (KeyValuePair<string, uint> keyValuePair in configList)
					{
						if (keyValuePair.Value != 0U)
						{
							transportRulesConfig.ScanVelocities[keyValuePair.Key] = keyValuePair.Value;
						}
					}
				}
				return transportRulesConfig;
			}

			private static bool TryParse(string config, out KeyValuePair<string, uint> parsedConfig)
			{
				parsedConfig = new KeyValuePair<string, uint>(string.Empty, 0U);
				if (string.IsNullOrWhiteSpace(config))
				{
					return false;
				}
				string[] array = config.Split(new char[]
				{
					','
				});
				if (array == null || array.Length != 2)
				{
					return false;
				}
				int value = 0;
				if (!int.TryParse(array[1], out value))
				{
					return false;
				}
				parsedConfig = new KeyValuePair<string, uint>(array[0], (uint)value);
				return true;
			}

			private static readonly Dictionary<string, uint> defaultScanVelocities = new Dictionary<string, uint>
			{
				{
					".",
					30U
				},
				{
					"doc",
					1292U
				},
				{
					"docx",
					92U
				},
				{
					"xls",
					166U
				},
				{
					"xlsx",
					30U
				},
				{
					"ppt",
					7000U
				},
				{
					"pptx",
					400U
				},
				{
					"htm",
					120U
				},
				{
					"html",
					120U
				},
				{
					"pdf",
					840U
				}
			};
		}

		public sealed class PoisonMessageConfig
		{
			public TimeSpan CrashDetectionWindow { get; private set; }

			public double AsyncMultiplier { get; private set; }

			public static TransportAppConfig.PoisonMessageConfig Load()
			{
				return new TransportAppConfig.PoisonMessageConfig
				{
					CrashDetectionWindow = TransportAppConfig.GetConfigTimeSpan("PoisonMessageDetectionWindow", TimeSpan.Zero, TransportAppConfig.PoisonMessageConfig.maxCrashDetectionWindow, TransportAppConfig.PoisonMessageConfig.defaultCrashDetectionWindow),
					AsyncMultiplier = TransportAppConfig.GetConfigDouble("PoisonThresholdAsyncMultiplier", TransportAppConfig.PoisonMessageConfig.minPoisonThresholdAsyncMultiplier, TransportAppConfig.PoisonMessageConfig.maxPoisonThresholdAsyncMultiplier, TransportAppConfig.PoisonMessageConfig.defaultPoisonThresholdAsyncMultiplier)
				};
			}

			private const string PoisonMessageDetectionWindow = "PoisonMessageDetectionWindow";

			private const string PoisonThresholdAsyncMultiplier = "PoisonThresholdAsyncMultiplier";

			private static TimeSpan defaultCrashDetectionWindow = TimeSpan.FromHours(8.0);

			private static TimeSpan maxCrashDetectionWindow = TimeSpan.FromDays(2.0);

			private static double minPoisonThresholdAsyncMultiplier = 0.0;

			private static double maxPoisonThresholdAsyncMultiplier = 1.0;

			private static double defaultPoisonThresholdAsyncMultiplier = 0.4;
		}

		public sealed class BootLoaderConfig : IBootLoaderConfig
		{
			public bool BootLoaderMessageTrackingEnabled { get; private set; }

			public TimeSpan MessageDropTimeout { get; private set; }

			public TimeSpan MessageExpirationGracePeriod { get; private set; }

			public TimeSpan PoisonMessageRetentionPeriod { get; private set; }

			public bool PoisonCountPublishingEnabled
			{
				get
				{
					return this.PoisonCountLookbackHours > 0;
				}
			}

			public int PoisonCountLookbackHours { get; private set; }

			public static TransportAppConfig.BootLoaderConfig Load()
			{
				return new TransportAppConfig.BootLoaderConfig
				{
					BootLoaderMessageTrackingEnabled = TransportAppConfig.GetConfigBool("BootLoaderMessageTrackingEnabled", TransportAppConfig.BootLoaderConfig.defaultBootLoaderMessageTrackingEnabled),
					MessageDropTimeout = TransportAppConfig.GetConfigTimeSpan("BootLoaderMessageSilentExpirationPeriod", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TransportAppConfig.BootLoaderConfig.defaultMessageDropTimeout),
					MessageExpirationGracePeriod = TransportAppConfig.GetConfigTimeSpan("BootLoaderMessageExpirationGracePeriod", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TransportAppConfig.BootLoaderConfig.defaultMessageExpirationGracePeriod),
					PoisonMessageRetentionPeriod = TransportAppConfig.GetConfigTimeSpan("PoisonMessageRetentionPeriod", TimeSpan.FromDays(1.0), TimeSpan.MaxValue, TransportAppConfig.BootLoaderConfig.defaultPoisonMessageRetentionPeriod),
					PoisonCountLookbackHours = TransportAppConfig.GetConfigInt("PoisonMessagePerfCounterLookbackHours", int.MinValue, int.MaxValue, TransportAppConfig.BootLoaderConfig.defaultPoisonCountLookbackHours)
				};
			}

			private const string BootLoaderMessageTrackingEnabledSetting = "BootLoaderMessageTrackingEnabled";

			private const string MessageDropTimeoutSetting = "BootLoaderMessageSilentExpirationPeriod";

			private const string MessageExpirationGracePeriodSetting = "BootLoaderMessageExpirationGracePeriod";

			private const string PoisonMessageRetentionPeriodSetting = "PoisonMessageRetentionPeriod";

			private const string PoisonMessageLookbackHoursSetting = "PoisonMessagePerfCounterLookbackHours";

			private static bool defaultBootLoaderMessageTrackingEnabled = false;

			private static TimeSpan defaultMessageDropTimeout = TimeSpan.FromDays(7.0);

			private static TimeSpan defaultMessageExpirationGracePeriod = TimeSpan.FromMinutes(30.0);

			private static TimeSpan defaultPoisonMessageRetentionPeriod = TimeSpan.FromDays(365.0);

			private static int defaultPoisonCountLookbackHours = 24;
		}

		internal interface ISmtpMessageThrottlingAgentConfig
		{
			bool Enabled { get; }

			bool LoggingModeEnabled { get; }

			bool SubjectThrottlingEnabled { get; }

			int InitialSenderRecipientRateTrackingThreshold { get; }

			TimeSpan InitialRateTrackingInterval { get; }

			TimeSpan SenderRateTrackingInterval { get; }

			bool WhitelistEnabled { get; }

			int EhaSenderToEhaRecipientThreshold { get; }

			int SenderRecipientThreshold { get; }

			int LowWatermarkThreshold { get; }

			TimeSpan ExpirationProcessingInterval { get; }

			int SenderRecipientSubjectThreshold { get; }

			int SubjectTrackingRemoveThreshold { get; }

			TimeSpan SubjectCounterSlidingWindow { get; }

			TimeSpan SubjectCounterSlidingWindowBucketLength { get; }

			int MaxSubjectCountPerSenderRecipient { get; }

			int SenderRecipientPartialSubjectThrottlingThreshold { get; }

			TimeSpan TrackSummaryLoggingInterval { get; }

			TimeSpan TrackSummaryBucketLength { get; }

			int MaxSummaryLinesLogged { get; }
		}

		internal class SmtpMessageThrottlingAgentConfig : TransportAppConfig.ISmtpMessageThrottlingAgentConfig
		{
			protected SmtpMessageThrottlingAgentConfig()
			{
			}

			public bool Enabled { get; private set; }

			public bool LoggingModeEnabled { get; private set; }

			public virtual bool SubjectThrottlingEnabled { get; private set; }

			public virtual int InitialSenderRecipientRateTrackingThreshold { get; private set; }

			public TimeSpan InitialRateTrackingInterval { get; private set; }

			public bool WhitelistEnabled { get; private set; }

			public virtual int EhaSenderToEhaRecipientThreshold { get; private set; }

			public TimeSpan SenderRateTrackingInterval { get; private set; }

			public int SenderRecipientThreshold
			{
				get
				{
					return this.senderRecipientThreshold;
				}
				set
				{
					this.senderRecipientThreshold = value;
				}
			}

			public int LowWatermarkThreshold { get; private set; }

			public TimeSpan ExpirationProcessingInterval { get; private set; }

			public virtual int SenderRecipientSubjectThreshold { get; private set; }

			public virtual int SubjectTrackingRemoveThreshold { get; private set; }

			public virtual TimeSpan SubjectCounterSlidingWindow { get; private set; }

			public virtual TimeSpan SubjectCounterSlidingWindowBucketLength { get; private set; }

			public virtual int MaxSubjectCountPerSenderRecipient { get; private set; }

			public virtual int SenderRecipientPartialSubjectThrottlingThreshold { get; private set; }

			public TimeSpan TrackSummaryLoggingInterval
			{
				get
				{
					return Components.TransportAppConfig.flowControlLogConfig.SummaryLoggingInterval;
				}
			}

			public TimeSpan TrackSummaryBucketLength
			{
				get
				{
					return Components.TransportAppConfig.flowControlLogConfig.SummaryBucketLength;
				}
			}

			public int MaxSummaryLinesLogged
			{
				get
				{
					return Components.TransportAppConfig.flowControlLogConfig.MaxSummaryLinesLogged;
				}
			}

			public static TransportAppConfig.SmtpMessageThrottlingAgentConfig Load()
			{
				TransportAppConfig.SmtpMessageThrottlingAgentConfig smtpMessageThrottlingAgentConfig = new TransportAppConfig.SmtpMessageThrottlingAgentConfig();
				smtpMessageThrottlingAgentConfig.Enabled = TransportAppConfig.GetConfigBool("SenderRecipientThrottlingEnabled", true);
				smtpMessageThrottlingAgentConfig.LoggingModeEnabled = TransportAppConfig.GetConfigBool("SenderRecipientThrottlingLoggingModeEnabled", false);
				smtpMessageThrottlingAgentConfig.InitialSenderRecipientRateTrackingThreshold = TransportAppConfig.GetConfigInt("SenderRecipientThrottlingInitialRateTrackingThreshold", 1, int.MaxValue, 200);
				smtpMessageThrottlingAgentConfig.WhitelistEnabled = TransportAppConfig.GetConfigBool("WhitelistThrottlingEnabled", true);
				smtpMessageThrottlingAgentConfig.EhaSenderToEhaRecipientThreshold = TransportAppConfig.GetConfigInt("SmtpThrottlingAgentEhaSenderToEhaRecipientRatePerHour", 0, int.MaxValue, 300000);
				smtpMessageThrottlingAgentConfig.SubjectThrottlingEnabled = TransportAppConfig.GetConfigBool("SenderRecipientSubjectThrottlingEnabled", false);
				smtpMessageThrottlingAgentConfig.SenderRecipientPartialSubjectThrottlingThreshold = TransportAppConfig.GetConfigInt("SmtpThrottlingAgentSenderRecipientPartialSubjectThrottlingThreshold", 0, int.MaxValue, 3000);
				smtpMessageThrottlingAgentConfig.SenderRecipientSubjectThreshold = TransportAppConfig.GetConfigInt("SmtpThrottlingAgentSenderRecipientSubjectThreshold", 0, int.MaxValue, 3000);
				smtpMessageThrottlingAgentConfig.SubjectTrackingRemoveThreshold = TransportAppConfig.GetConfigInt("SmtpThrottlingAgentSubjectTrackingRemoveThreshold", 0, int.MaxValue, 100);
				smtpMessageThrottlingAgentConfig.MaxSubjectCountPerSenderRecipient = TransportAppConfig.GetConfigInt("SmtpThrottlingAgentMaxSubjectCountPerSenderRecipient", 0, 5000, 5);
				smtpMessageThrottlingAgentConfig.SubjectCounterSlidingWindow = TransportAppConfig.GetConfigTimeSpan("SmtpThrottlingAgentSubjectCounterSlidingWindow", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(30.0));
				smtpMessageThrottlingAgentConfig.SubjectCounterSlidingWindowBucketLength = TransportAppConfig.GetConfigTimeSpan("SmtpThrottlingAgentSubjectCounterSlidingWindowBucketLength", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(3.0));
				smtpMessageThrottlingAgentConfig.InitialRateTrackingInterval = TransportAppConfig.GetConfigTimeSpan("SenderRecipientThrottlingInitialRateTrackingInterval", TimeSpan.FromSeconds(10.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(5.0));
				smtpMessageThrottlingAgentConfig.SenderRateTrackingInterval = TransportAppConfig.GetConfigTimeSpan("SenderRecipientThrottlingRateTrackingInterval", TimeSpan.FromSeconds(20.0), TimeSpan.FromMinutes(180.0), TimeSpan.FromMinutes(60.0));
				smtpMessageThrottlingAgentConfig.LowWatermarkThreshold = TransportAppConfig.GetConfigInt("SenderRecipientThrottlingLowWatermarkThreshold", 0, int.MaxValue, smtpMessageThrottlingAgentConfig.InitialSenderRecipientRateTrackingThreshold);
				smtpMessageThrottlingAgentConfig.ExpirationProcessingInterval = TransportAppConfig.GetConfigTimeSpan("SenderRecipientThrottlingExpirationProcessingInterval", TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(5.0));
				return smtpMessageThrottlingAgentConfig;
			}

			private int senderRecipientThreshold;
		}

		public sealed class StateManagementConfig
		{
			private StateManagementConfig()
			{
			}

			public bool StateChangeDetectionEnabled { get; private set; }

			public static TransportAppConfig.StateManagementConfig Load()
			{
				return new TransportAppConfig.StateManagementConfig
				{
					StateChangeDetectionEnabled = TransportAppConfig.GetConfigBool("StateManagementStateChangeDetectionEnabled", true)
				};
			}

			private const string StateChangeDetectionEnabledLabel = "StateManagementStateChangeDetectionEnabled";
		}

		public sealed class ADPollingConfig
		{
			private ADPollingConfig()
			{
			}

			public static TransportAppConfig.ADPollingConfig Load()
			{
				return new TransportAppConfig.ADPollingConfig
				{
					InterceptorRulesReloadInterval = TransportAppConfig.GetConfigTimeSpan("InterceptorRulesReloadInterval", TransportAppConfig.ADPollingConfig.minAllowedReloadInterval, TransportAppConfig.ADPollingConfig.maxAllowedReloadInterval, TimeSpan.FromMinutes(5.0)),
					TransportLogSearchServiceReloadInterval = TransportAppConfig.GetConfigTimeSpan("TransportLogSearchServiceReloadInterval", TransportAppConfig.ADPollingConfig.minAllowedReloadInterval, TransportAppConfig.ADPollingConfig.maxAllowedReloadInterval, TimeSpan.FromHours(6.0)),
					ConfigurationComponentReloadInterval = TransportAppConfig.GetConfigTimeSpan("ConfigurationComponentReloadInterval", TransportAppConfig.ADPollingConfig.minAllowedReloadInterval, TransportAppConfig.ADPollingConfig.maxAllowedReloadInterval, TimeSpan.FromMinutes(5.0)),
					DsnMessageCustomizationReloadInterval = TransportAppConfig.GetConfigTimeSpan("DsnMessageCustomizationReloadInterval", TransportAppConfig.ADPollingConfig.minAllowedReloadInterval, TransportAppConfig.ADPollingConfig.maxAllowedReloadInterval, TimeSpan.FromHours(6.0))
				};
			}

			public TimeSpan InterceptorRulesReloadInterval { get; private set; }

			public TimeSpan TransportLogSearchServiceReloadInterval { get; private set; }

			public TimeSpan ConfigurationComponentReloadInterval { get; private set; }

			public TimeSpan DsnMessageCustomizationReloadInterval { get; private set; }

			private static readonly TimeSpan minAllowedReloadInterval = TimeSpan.FromSeconds(5.0);

			private static readonly TimeSpan maxAllowedReloadInterval = TimeSpan.FromDays(1.0);
		}
	}
}

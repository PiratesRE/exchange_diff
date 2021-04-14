using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ServiceConfiguration
	{
		internal ServiceConfiguration(ConfigurationPropertyBag propertyBag)
		{
			propertyBag.Freeze();
			this.propertyBag = propertyBag;
			this.idleConnectionCheckPeriod = propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.IdleConnectionCheckPeriod);
			this.logConnectionLatencyCheckPeriod = propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.LogConnectionLatencyCheckPeriod);
			this.maintenanceJobTimerCheckPeriod = propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.MaintenanceJobTimerCheckPeriod);
			this.shareConnections = propertyBag.Get<bool>(ServiceConfiguration.Schema.ShareConnections);
			this.enableExMonTestMode = propertyBag.Get<bool>(ServiceConfiguration.Schema.EnableExMonTestMode);
			this.fastTransferMaxRequests = propertyBag.Get<int>(ServiceConfiguration.Schema.FastTransferMaxRequests);
			this.fastTransferBackoffInterval = propertyBag.Get<int>(ServiceConfiguration.Schema.FastTransferBackoffInterval);
			this.fastTransferBackoffRetryCount = propertyBag.Get<int>(ServiceConfiguration.Schema.FastTransferBackoffRetryCount);
			this.rpcPollsMax = propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.RpcPollsMax);
			this.rpcRetryCount = propertyBag.Get<int>(ServiceConfiguration.Schema.RpcRetryCount);
			this.rpcRetryDelay = propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.RpcRetryDelay);
			this.maxRandomAdditionalRpcRetryDelay = propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.MaxRandomAdditionalRpcRetryDelay);
		}

		public TimeSpan ADUserDataCacheTimeout
		{
			get
			{
				return this.propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.ADUserDataCacheTimeout);
			}
		}

		public bool CanServicePrivateLogons
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsRpcClientAccessObjectPresent) && this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsClientAccessRole);
			}
		}

		public bool CanServicePublicLogons
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsRpcClientAccessObjectPresent) && this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsClientAccessRole);
			}
		}

		public bool CanServiceRecoveryDatabaseLogons
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsMailboxRole);
			}
		}

		public bool IsClientVersionAllowed(MapiVersion version)
		{
			MapiVersionRanges mapiVersionRanges = this.propertyBag.Get<MapiVersionRanges>(ServiceConfiguration.Schema.BlockedClientVersions);
			return mapiVersionRanges == null || !mapiVersionRanges.IsIncluded(version);
		}

		public bool IsClientVersionAllowedInForest(MapiVersion requestedClientVersion)
		{
			ClientVersionCollection clientVersionCollection = this.propertyBag.Get<ClientVersionCollection>(ServiceConfiguration.Schema.RequiredVersionCollection);
			if (clientVersionCollection == null)
			{
				return true;
			}
			ushort[] array = requestedClientVersion.ToQuartet();
			Version version = new Version((int)array[0], (int)array[1], (int)array[2], (int)array[3]);
			return version < ServiceConfiguration.MinimumForcedUpgradeClientVersion || clientVersionCollection.IsClientVersionSufficient(version);
		}

		public bool IsEncryptionRequired
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsEncryptionRequired);
			}
		}

		public bool IsServiceEnabled
		{
			get
			{
				return this.CanServicePrivateLogons || this.CanServicePublicLogons;
			}
		}

		public bool IsDisabledOnMailboxRole
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsMailboxRole) && !this.propertyBag.Get<bool>(ServiceConfiguration.Schema.CanRunOnMailboxRole);
			}
		}

		public bool LogEveryConfigurationUpdate
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.LogEveryConfigurationUpdate);
			}
		}

		public int MaximumConnections
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.MaximumConnections);
			}
		}

		public TimeSpan IdleConnectionCheckPeriod
		{
			get
			{
				return this.idleConnectionCheckPeriod;
			}
		}

		public TimeSpan LogConnectionLatencyCheckPeriod
		{
			get
			{
				return this.logConnectionLatencyCheckPeriod;
			}
		}

		public TimeSpan MaintenanceJobTimerCheckPeriod
		{
			get
			{
				return this.maintenanceJobTimerCheckPeriod;
			}
		}

		public TimeSpan WaitBetweenTcpConnectToFindIfRpcServiceResponsive
		{
			get
			{
				return this.propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.WaitBetweenTcpConnectToFindIfRpcServiceResponsive);
			}
		}

		public Fqdn ThisServerFqdn
		{
			get
			{
				return this.propertyBag.Get<Fqdn>(ServiceConfiguration.Schema.ThisServerFqdn);
			}
		}

		public LegacyDN ThisServerLegacyDN
		{
			get
			{
				return this.propertyBag.Get<LegacyDN>(ServiceConfiguration.Schema.ThisServerLegacyDN);
			}
		}

		public bool ShareConnections
		{
			get
			{
				return this.shareConnections;
			}
		}

		public bool EnableExMonTestMode
		{
			get
			{
				return this.enableExMonTestMode;
			}
		}

		public bool EnablePreferredSiteEnforcement
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.EnablePreferredSiteEnforcement);
			}
		}

		public int FastTransferMaxRequests
		{
			get
			{
				return this.fastTransferMaxRequests;
			}
		}

		public int FastTransferBackoffInterval
		{
			get
			{
				return this.fastTransferBackoffInterval;
			}
		}

		public int FastTransferBackoffRetryCount
		{
			get
			{
				return this.fastTransferBackoffRetryCount;
			}
		}

		public TimeSpan RpcPollsMax
		{
			get
			{
				return this.rpcPollsMax;
			}
		}

		public int RpcRetryCount
		{
			get
			{
				return this.rpcRetryCount;
			}
		}

		public TimeSpan RpcRetryDelay
		{
			get
			{
				return this.rpcRetryDelay;
			}
		}

		public TimeSpan MaxRandomAdditionalRpcRetryDelay
		{
			get
			{
				return this.maxRandomAdditionalRpcRetryDelay;
			}
		}

		public bool EnableWebServicesEndpoint
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsXtcEnabled) || this.propertyBag.Get<bool>(ServiceConfiguration.Schema.EnableWebServicesEndpoint);
			}
		}

		public bool EnableSmartConnectionTearDown
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.EnableSmartConnectionTearDown);
			}
		}

		public bool EnableBlockInsufficientClientVersions
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.EnableBlockInsufficientClientVersions);
			}
		}

		public bool EnableWebServicesOrganizationRelationshipCheck
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.IsXtcEnabled);
			}
		}

		public bool AvailabilityServiceCallsDisabled
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.AvailabilityServiceCallsDisabled);
			}
		}

		public bool TMPublishEnabled
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.TMPublishEnabled);
			}
		}

		public bool TMOAuthEnabled
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.TMOAuthEnabled);
			}
		}

		public bool TMPublishHttpDebugEnabled
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.TMPublishHttpDebugEnabled);
			}
		}

		public TimeSpan TMPublishRequestTimeout
		{
			get
			{
				return this.propertyBag.Get<TimeSpan>(ServiceConfiguration.Schema.TMPublishRequestTimeout);
			}
		}

		public ICredentials TMPublishCredential
		{
			get
			{
				return this.propertyBag.Get<ICredentials>(ServiceConfiguration.Schema.TMPublishCredential);
			}
		}

		public bool TMUseMockSharePointOperation
		{
			get
			{
				return this.propertyBag.Get<bool>(ServiceConfiguration.Schema.TMUseMockSharePointOperation);
			}
		}

		public int TMPublishConcurrentOperationLimit
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.TMPublishConcurrentOperationLimit);
			}
		}

		public MapiVersion TMRequiredMAPIClientVersion
		{
			get
			{
				return this.propertyBag.Get<MapiVersion>(ServiceConfiguration.Schema.TMRequiredMAPIClientVersion);
			}
		}

		public int MaximumRpcTasks
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.MaximumRpcTasks);
			}
		}

		public int MaximumRpcThreads
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.MaximumRpcThreads);
			}
		}

		public int MinimumRpcThreads
		{
			get
			{
				return Math.Min(this.propertyBag.Get<int>(ServiceConfiguration.Schema.MinimumRpcThreads), this.MaximumRpcThreads);
			}
		}

		public int MaximumWebServiceTasks
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.MaximumWebServiceTasks);
			}
		}

		public int MaximumWebServiceThreads
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.MaximumWebServiceThreads);
			}
		}

		public int MinimumWebServiceThreads
		{
			get
			{
				return Math.Min(this.propertyBag.Get<int>(ServiceConfiguration.Schema.MinimumWebServiceThreads), this.MaximumWebServiceThreads);
			}
		}

		public int MaximumRpcHttpConnectionRegistrationTasks
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.MaximumRpcHttpConnectionRegistrationTasks);
			}
		}

		public int MaximumRpcHttpConnectionRegistrationThreads
		{
			get
			{
				return this.propertyBag.Get<int>(ServiceConfiguration.Schema.MaximumRpcHttpConnectionRegistrationThreads);
			}
		}

		public int MinimumRpcHttpConnectionRegistrationThreads
		{
			get
			{
				return Math.Min(this.propertyBag.Get<int>(ServiceConfiguration.Schema.MinimumRpcHttpConnectionRegistrationThreads), this.MaximumRpcHttpConnectionRegistrationThreads);
			}
		}

		public const string RpcClientAccessServiceName = "MSExchangeRPC";

		public const int DefaultMaximumRpcTasks = 5000;

		public const int DefaultMaximumRpcThreads = 250;

		public const int DefaultMaximumRpcThreadProcessorFactor = 3;

		public const int DefaultMinimumRpcThreadProcessorFactor = 1;

		public const int DefaultMinimumRpcThreads = 4;

		public const int DefaultMaximumWebServiceTasks = 1000;

		public const int DefaultMaximumWebServiceThreads = 100;

		public const int DefaultMaximumWebServiceThreadProcessorFactor = 3;

		public const int DefaultMinimumWebServiceThreads = 2;

		public const int DefaultMaximumRpcHttpConnectionRegistrationTasks = 5000;

		public const int DefaultMaximumRpcHttpConnectionRegistrationThreads = 32;

		public const int DefaultMaximumRpcHttpConnectionRegistrationThreadProcessorFactor = 2;

		public const int DefaultMinimumRpcHttpConnectionRegistrationThreadProcessorFactor = 1;

		public const string RegKeyPathExchangeServer = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		public const string RegKeyNameSIEngineURI = "SIEngineURI";

		public const string RegKeyNameSIUploaderQueueSize = "SIUploaderQueueSize";

		internal const int RpcRetryCountDefault = 6;

		public static Guid ComponentGuid = new Guid("53F12A79-F089-4312-9285-8CFDC77FB0A9");

		public static string Component = "RpcClientAccess";

		public static bool StreamInsightUploaderEnabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).RpcClientAccess.StreamInsightUploader.Enabled;

		public static string StreamInsightEngineURI = Convert.ToString(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15", "SIEngineURI", "http://xsi-exo-pre-momt.cloudapp.net:10200/momt/dcs/"));

		public static int StreamInsightUploaderQueueSize = Convert.ToInt32(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15", "SIUploaderQueueSize", 200));

		internal static readonly TimeSpan RpcPollsMaxDefault = TimeSpan.FromMinutes(1.0);

		internal static readonly TimeSpan RpcRetryDelayDefault = TimeSpan.FromSeconds(10.0);

		internal static readonly TimeSpan MaxRandomAdditionalRpcRetryDelayDefault = TimeSpan.FromSeconds(10.0);

		private static readonly Version MinimumForcedUpgradeClientVersion = new Version(15, 0, 4569, 0);

		private readonly ConfigurationPropertyBag propertyBag;

		private readonly TimeSpan idleConnectionCheckPeriod;

		private readonly TimeSpan logConnectionLatencyCheckPeriod;

		private readonly TimeSpan maintenanceJobTimerCheckPeriod;

		private readonly bool shareConnections;

		private readonly bool enableExMonTestMode;

		private readonly int fastTransferMaxRequests;

		private readonly int fastTransferBackoffInterval;

		private readonly int fastTransferBackoffRetryCount;

		private readonly TimeSpan rpcPollsMax;

		private readonly int rpcRetryCount;

		private readonly TimeSpan rpcRetryDelay;

		private readonly TimeSpan maxRandomAdditionalRpcRetryDelay;

		internal class Schema : ConfigurationSchema<ServiceConfiguration.Schema>
		{
			static Schema()
			{
				bool defaultValue = ServiceConfiguration.Schema.IsXtcEnabled.DefaultValue;
			}

			private const string ParametersRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeRPC\\ParametersSystem";

			private const string ADUserDataCacheTimeoutRegistryValueName = "ADUserDataCacheTimeout";

			private const string ExecutionFlagsRegistryValueName = "ExecutionFlags";

			private const string IdleConnectionCheckPeriodRegistryValueName = "IdleConnectionCheckPeriod";

			private const string LogConnectionLatencyCheckPeriodRegistryValueName = "LogConnectionLatencyCheckPeriod";

			private const string MaintenanceJobTimerCheckPeriodRegistryValueName = "MaintenanceJobTimerCheckPeriod";

			private const string WaitBetweenTcpConnectToFindIfRpcServiceResponsiveRegistryValueName = "WaitBetweenTcpConnectToFindIfRpcServiceResponsive";

			private const string LogEveryConfigurationUpdateValueName = "LogEveryConfigurationUpdate";

			private const string ShareConnectionsRegistryValueName = "ShareConnections";

			private const string EnableExMonTestModeRegistryValueName = "EnableExMonTestMode";

			private const string TMPublishEnabledRegistryValueName = "TMPublishEnabled";

			private const string TMOAuthEnabledRegistryValueName = "TMOAuthEnabled";

			private const string TMPublishHttpDebugEnabledRegistryValueName = "TMPublishHttpDebugEnabled";

			private const string TMPublishRequestTimeoutRegistryValueName = "TMPublishRequestTimeout";

			private const string TMPublishCredentialRegistryValueName = "TMPublishCredential";

			private const string TMUseMockSharePointOperationRegistryValue = "TMUseMockSharePointOperation";

			private const string TMPublishConcurrentOperationLimitRegistryValueName = "TMPublishConcurrentOperationLimit";

			private const string TMRequiredMAPIClientVersionRegistryValueName = "TMRequiredMAPIClientVersion";

			private const string EnablePreferredSiteEnforcementRegistryValueName = "EnablePreferredSiteEnforcement";

			private const string FastTransferBackoffIntervalRegistryValueName = "FXGetBuffer BackOff Constant";

			private const string FastTransferBackoffRetryCountRegistryValueName = "FXGetBuffer Retry Count";

			private const string FastTransferMaxRequestsRegistryValueName = "Max FXGetBuffer Users";

			private const string RpcPollsMaxRegistryValueName = "Maximum Polling Frequency";

			private const string RpcRetryCountRegistryValueName = "RPC Retry Count";

			private const string RpcRetryDelayRegistryValueName = "RPC Retry Delay";

			private const string MaxRandomAdditionalRpcRetryDelayRegistryValueName = "Maximum Random Additional RPC Retry Delay";

			private const string EnableSmartConnectionTearDownValueName = "EnableSmartConnectionTearDown";

			private const string DisableAvailabilityService = "DisableAvailabilityServiceCalls";

			private const string EnableWebServicesEndpointRegistryValueName = "EnableWebServicesEndpoint";

			private const string MaximumRpcTasksRegistryValueName = "MaximumRpcTasks";

			private const string MaximumRpcThreadsRegistryValueName = "MaximumRpcThreads";

			private const string MinimumRpcThreadsRegistryValueName = "MinimumRpcThreads";

			private const string MaximumWebServiceTasksRegistryValueName = "MaximumWebServiceTasks";

			private const string MaximumWebServiceThreadsRegistryValueName = "MaximumWebServiceThreads";

			private const string MinimumWebServiceThreadsRegistryValueName = "MinimumWebServiceThreads";

			private const string MaximumRpcHttpConnectionRegistrationTasksRegistryValueName = "MaximumRpcHttpConnectionRegistrationTasks";

			private const string MaximumRpcHttpConnectionRegistrationThreadsRegistryValueName = "MaximumRpcHttpConnectionRegistrationThreads";

			private const string MinimumRpcHttpConnectionRegistrationThreadsRegistryValueName = "MinimumRpcHttpConnectionRegistrationThreads";

			private const string EnableBlockInsufficientClientVersionsRegistryValueName = "EnableBlockInsufficientClientVersions";

			internal static ConfigurationSchema.Property<bool> IsXtcEnabled = ConfigurationSchema<ServiceConfiguration.Schema>.ConstantDataSource.Declare<bool>(delegate
			{
				VariantConfigurationSnapshot invariantNoFlightingSnapshot = VariantConfiguration.InvariantNoFlightingSnapshot;
				return invariantNoFlightingSnapshot.RpcClientAccess.XtcEndpoint.Enabled;
			});

			private static readonly ConfigurationSchema.RegistryDataSource ParametersRegistryKey = new ConfigurationSchema.RegistryDataSource(ConfigurationSchema<ServiceConfiguration.Schema>.AllDataSources, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeRPC\\ParametersSystem");

			internal static readonly ConfigurationSchema.Property<TimeSpan> ADUserDataCacheTimeout = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "ADUserDataCacheTimeout", (int input) => input >= 0, (int input) => TimeSpan.FromSeconds((double)input), TimeSpan.FromMinutes(15.0));

			internal static readonly ConfigurationSchema.Property<TimeSpan> WaitBetweenTcpConnectToFindIfRpcServiceResponsive = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "WaitBetweenTcpConnectToFindIfRpcServiceResponsive", (int input) => input >= 0, (int input) => TimeSpan.FromSeconds((double)input), TimeSpan.FromMinutes(1.0));

			internal static readonly ConfigurationSchema.Property<bool> CanRunOnMailboxRole = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "ExecutionFlags", delegate(int input, out bool output)
			{
				output = ((input & 2) == 2);
				return EnumValidator.IsValidValue<ServiceConfiguration.Schema.ExecutionFlags>((ServiceConfiguration.Schema.ExecutionFlags)input);
			}, true);

			internal static readonly ConfigurationSchema.Property<bool> LogEveryConfigurationUpdate = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "LogEveryConfigurationUpdate", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<TimeSpan> IdleConnectionCheckPeriod = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "IdleConnectionCheckPeriod", (int input) => input >= 0, (int input) => TimeSpan.FromSeconds((double)input), TimeSpan.FromMinutes(5.0));

			internal static readonly ConfigurationSchema.Property<TimeSpan> LogConnectionLatencyCheckPeriod = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "LogConnectionLatencyCheckPeriod", (int input) => input >= 0, (int input) => TimeSpan.FromMinutes((double)input), TimeSpan.FromMinutes(15.0));

			internal static readonly ConfigurationSchema.Property<TimeSpan> MaintenanceJobTimerCheckPeriod = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MaintenanceJobTimerCheckPeriod", (int input) => input >= 0, (int input) => TimeSpan.FromSeconds((double)input), TimeSpan.FromMinutes(1.0));

			internal static readonly ConfigurationSchema.Property<bool> ShareConnections = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "ShareConnections", (int value) => value == 0 || value == 1, (int value) => value == 1, true);

			internal static readonly ConfigurationSchema.Property<bool> EnableExMonTestMode = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "EnableExMonTestMode", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<bool> TMPublishEnabled = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMPublishEnabled", (int value) => value == 0 || value == 1, (int value) => value == 1, true);

			internal static readonly ConfigurationSchema.Property<bool> TMOAuthEnabled = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMOAuthEnabled", (int value) => value == 0 || value == 1, (int value) => value == 1, true);

			internal static readonly ConfigurationSchema.Property<bool> TMPublishHttpDebugEnabled = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMPublishHttpDebugEnabled", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<TimeSpan> TMPublishRequestTimeout = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMPublishRequestTimeout", (int value) => value >= 0, (int input) => TimeSpan.FromSeconds((double)input), TimeSpan.FromSeconds(120.0));

			internal static readonly ConfigurationSchema.Property<bool> TMUseMockSharePointOperation = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMUseMockSharePointOperation", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<int> TMPublishConcurrentOperationLimit = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMPublishConcurrentOperationLimit", (int input) => input >= 0, (int input) => input, 20);

			internal static readonly ConfigurationSchema.Property<ICredentials> TMPublishCredential = ConfigurationSchema.Property<ICredentials>.Declare<string, object, string>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMPublishCredential", delegate(string input, out ICredentials output)
			{
				output = null;
				if (string.IsNullOrEmpty(input))
				{
					return false;
				}
				string[] array = input.Split(new char[]
				{
					';'
				});
				if (array == null || array.Length != 3)
				{
					return false;
				}
				output = new NetworkCredential(array[0], array[1], array[2]);
				return true;
			}, CredentialCache.DefaultCredentials);

			internal static readonly ConfigurationSchema.Property<MapiVersion> TMRequiredMAPIClientVersion = ConfigurationSchema.Property<MapiVersion>.Declare<string, object, string>(ServiceConfiguration.Schema.ParametersRegistryKey, "TMRequiredMAPIClientVersion", delegate(string input, out MapiVersion output)
			{
				output = MapiVersion.Min;
				bool result;
				try
				{
					output = MapiVersion.Parse(input);
					result = true;
				}
				catch (FormatException)
				{
					result = false;
				}
				catch (ArgumentOutOfRangeException)
				{
					result = false;
				}
				return result;
			}, MapiVersion.Outlook15);

			internal static readonly ConfigurationSchema.Property<bool> EnablePreferredSiteEnforcement = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "EnablePreferredSiteEnforcement", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<int> FastTransferMaxRequests = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "Max FXGetBuffer Users", (int input) => input >= 0, (int input) => input, 15);

			internal static readonly ConfigurationSchema.Property<int> FastTransferBackoffInterval = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "FXGetBuffer BackOff Constant", (int input) => input >= 0 && input <= 10000, (int input) => input, 500);

			internal static readonly ConfigurationSchema.Property<int> FastTransferBackoffRetryCount = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "FXGetBuffer Retry Count", (int input) => input >= 0 && input <= 100, (int input) => input, 10);

			internal static readonly ConfigurationSchema.Property<TimeSpan> RpcPollsMax = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "Maximum Polling Frequency", (int input) => input >= 5000 && input <= 120000, (int input) => TimeSpan.FromMilliseconds((double)input), ServiceConfiguration.RpcPollsMaxDefault);

			internal static readonly ConfigurationSchema.Property<int> RpcRetryCount = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "RPC Retry Count", (int input) => input >= 0 && input <= 100, (int input) => input, 6);

			internal static readonly ConfigurationSchema.Property<TimeSpan> RpcRetryDelay = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "RPC Retry Delay", (int input) => input >= 100 && input <= 120000, (int input) => TimeSpan.FromMilliseconds((double)input), ServiceConfiguration.RpcRetryDelayDefault);

			internal static readonly ConfigurationSchema.Property<TimeSpan> MaxRandomAdditionalRpcRetryDelay = ConfigurationSchema.Property<TimeSpan>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "Maximum Random Additional RPC Retry Delay", (int input) => input >= 0 && input <= 120000, (int input) => TimeSpan.FromMilliseconds((double)input), ServiceConfiguration.MaxRandomAdditionalRpcRetryDelayDefault);

			internal static readonly ConfigurationSchema.Property<bool> EnableSmartConnectionTearDown = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "EnableSmartConnectionTearDown", (int value) => value == 0 || value == 1, (int value) => value == 1, true);

			internal static readonly ConfigurationSchema.Property<bool> EnableBlockInsufficientClientVersions = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "EnableBlockInsufficientClientVersions", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<bool> EnableWebServicesEndpoint = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "EnableWebServicesEndpoint", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<bool> AvailabilityServiceCallsDisabled = ConfigurationSchema.Property<bool>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "DisableAvailabilityServiceCalls", (int value) => value == 0 || value == 1, (int value) => value == 1, false);

			internal static readonly ConfigurationSchema.Property<int> MaximumRpcTasks = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MaximumRpcTasks", (int input) => input >= 100 && input <= 50000, (int input) => input, 5000);

			internal static readonly ConfigurationSchema.Property<int> MaximumRpcThreads = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MaximumRpcThreads", (int input) => input >= 1 && input <= 500, (int input) => input, Math.Min(Environment.ProcessorCount * 3, 250));

			internal static readonly ConfigurationSchema.Property<int> MinimumRpcThreads = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MinimumRpcThreads", (int input) => input >= 1 && input <= 500, (int input) => input, Environment.ProcessorCount);

			internal static readonly ConfigurationSchema.Property<int> MaximumWebServiceTasks = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MaximumWebServiceTasks", (int input) => input >= 100 && input <= 50000, (int input) => input, 1000);

			internal static readonly ConfigurationSchema.Property<int> MaximumWebServiceThreads = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MaximumWebServiceThreads", (int input) => input >= 1 && input <= 500, (int input) => input, Math.Min(Environment.ProcessorCount * 3, 100));

			internal static readonly ConfigurationSchema.Property<int> MinimumWebServiceThreads = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MinimumWebServiceThreads", (int input) => input >= 1 && input <= 500, (int input) => input, 2);

			internal static readonly ConfigurationSchema.Property<int> MaximumRpcHttpConnectionRegistrationTasks = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MaximumRpcHttpConnectionRegistrationTasks", (int input) => input >= 100 && input <= 50000, (int input) => input, 5000);

			internal static readonly ConfigurationSchema.Property<int> MaximumRpcHttpConnectionRegistrationThreads = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MaximumRpcHttpConnectionRegistrationThreads", (int input) => input >= 1 && input <= 500, (int input) => input, Math.Min(Environment.ProcessorCount * 2, 32));

			internal static readonly ConfigurationSchema.Property<int> MinimumRpcHttpConnectionRegistrationThreads = ConfigurationSchema.Property<int>.Declare<string, object, int>(ServiceConfiguration.Schema.ParametersRegistryKey, "MinimumRpcHttpConnectionRegistrationThreads", (int input) => input >= 1 && input <= 500, (int input) => input, Environment.ProcessorCount);

			private static readonly ConfigurationSchema.DirectoryDataSource<Server> LocalServerObject = new ConfigurationSchema.DirectoryDataSource<Server>(ConfigurationSchema<ServiceConfiguration.Schema>.AllDataSources, (ITopologyConfigurationSession session) => LocalServer.GetServer(), (ITopologyConfigurationSession session) => LocalServer.GetServer().Id);

			internal static readonly ConfigurationSchema.Property<bool> IsClientAccessRole = ServiceConfiguration.Schema.LocalServerObject.DeclareProperty<bool>(ServerSchema.IsClientAccessServer);

			internal static readonly ConfigurationSchema.Property<bool> IsMailboxRole = ConfigurationSchema.Property<bool>.Declare<ADPropertyDefinition, object>(ServiceConfiguration.Schema.LocalServerObject, ServerSchema.IsMailboxServer, true);

			internal static readonly ConfigurationSchema.Property<Fqdn> ThisServerFqdn = ConfigurationSchema.Property<Fqdn>.Declare<ADPropertyDefinition, object, string>(ServiceConfiguration.Schema.LocalServerObject, ServerSchema.Fqdn, new ConfigurationSchema.TryConvert<string, Fqdn>(Fqdn.TryParse), null);

			internal static readonly ConfigurationSchema.Property<LegacyDN> ThisServerLegacyDN = ConfigurationSchema.Property<LegacyDN>.Declare<ADPropertyDefinition, object, string>(ServiceConfiguration.Schema.LocalServerObject, ServerSchema.ExchangeLegacyDN, new ConfigurationSchema.TryConvert<string, LegacyDN>(LegacyDN.TryParse), null);

			private static readonly ConfigurationSchema.DirectoryDataSource<ExchangeRpcClientAccess> RpcClientAccessObject = new ConfigurationSchema.DirectoryDataSource<ExchangeRpcClientAccess>(ConfigurationSchema<ServiceConfiguration.Schema>.AllDataSources, (ITopologyConfigurationSession session) => session.Read<ExchangeRpcClientAccess>(ExchangeRpcClientAccess.FromServerId(LocalServer.GetServer().Id)), (ITopologyConfigurationSession session) => LocalServer.GetServer().Id);

			internal static readonly ConfigurationSchema.Property<bool> IsRpcClientAccessObjectPresent = ConfigurationSchema.Property<bool>.Declare<ConfigurationSchema.DirectoryDataSource<ExchangeRpcClientAccess>>(ServiceConfiguration.Schema.RpcClientAccessObject, (ConfigurationSchema.DirectoryDataSource<ExchangeRpcClientAccess> dataSource, object context) => dataSource.CanQueryData(context), false);

			internal static readonly ConfigurationSchema.Property<bool> IsEncryptionRequired = ServiceConfiguration.Schema.RpcClientAccessObject.DeclareProperty<bool>(ExchangeRpcClientAccessSchema.IsEncryptionRequired);

			internal static readonly ConfigurationSchema.Property<int> MaximumConnections = ServiceConfiguration.Schema.RpcClientAccessObject.DeclareProperty<int>(ExchangeRpcClientAccessSchema.MaximumConnections);

			internal static readonly ConfigurationSchema.Property<MapiVersionRanges> BlockedClientVersions = ConfigurationSchema.Property<MapiVersionRanges>.Declare<ADPropertyDefinition, object, string>(ServiceConfiguration.Schema.RpcClientAccessObject, ExchangeRpcClientAccessSchema.BlockedClientVersions, delegate(string blockedClientVersions, out MapiVersionRanges mapiVersionRanges)
			{
				bool result;
				try
				{
					mapiVersionRanges = new MapiVersionRanges(blockedClientVersions);
					result = true;
				}
				catch (FormatException innerException)
				{
					throw new ConfigurationSchema.LoadException(innerException);
				}
				catch (ArgumentException innerException2)
				{
					throw new ConfigurationSchema.LoadException(innerException2);
				}
				return result;
			}, new MapiVersionRanges(null));

			private static readonly ConfigurationSchema.DirectoryDataSource<OutlookProvider> OutlookProviderObject = new ConfigurationSchema.DirectoryDataSource<OutlookProvider>(ConfigurationSchema<ServiceConfiguration.Schema>.AllDataSources, (ITopologyConfigurationSession session) => session.Read<OutlookProvider>(OutlookProvider.GetParentContainer(session).GetChildId("EXCH")), delegate(ITopologyConfigurationSession session)
			{
				ADObjectId parentContainer = OutlookProvider.GetParentContainer(session);
				if (parentContainer != null)
				{
					OutlookProvider outlookProvider = session.Read<OutlookProvider>(parentContainer.GetChildId("EXCH"));
					if (outlookProvider != null)
					{
						return outlookProvider.Id;
					}
				}
				return null;
			});

			internal static readonly ConfigurationSchema.Property<ClientVersionCollection> RequiredVersionCollection = ConfigurationSchema.Property<ClientVersionCollection>.Declare<ADPropertyDefinition, object, ClientVersionCollection>(ServiceConfiguration.Schema.OutlookProviderObject, OutlookProviderSchema.RequiredClientVersions, delegate(object input, out ClientVersionCollection output)
			{
				output = (input as ClientVersionCollection);
				return true;
			}, new ConfigurationSchema.TryConvert<ClientVersionCollection, ClientVersionCollection>(ConfigurationSchema.Property.Identical<ClientVersionCollection>), null);

			[Flags]
			private enum ExecutionFlags
			{
				None = 0,
				Unused = 1,
				CanRunOnMailboxRole = 2
			}
		}
	}
}

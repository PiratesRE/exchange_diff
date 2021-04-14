using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal sealed class DataMart
	{
		static DataMart()
		{
			DataMart.ReportingCmdletKeyRoot = string.Format(CultureInfo.InvariantCulture, "SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\Cmdlet\\Reporting", new object[]
			{
				"v15"
			});
		}

		private DataMart()
		{
			this.dataMartKeyMapping = new Dictionary<DataMartType, string>
			{
				{
					DataMartType.Tenants,
					"DataMartTenants"
				},
				{
					DataMartType.TenantsScaled,
					"DataMartTenantsScaled"
				},
				{
					DataMartType.Transport,
					"DataMartTransport"
				},
				{
					DataMartType.C3,
					"DataMartC3"
				},
				{
					DataMartType.Manageability,
					"DataMartManageability"
				},
				{
					DataMartType.EngineeringFundamentals,
					"DataMartEngineeringFundamentals"
				},
				{
					DataMartType.Datacenter,
					"DataMartDatacenter"
				},
				{
					DataMartType.AM,
					"DataMartAM"
				},
				{
					DataMartType.OspExo,
					"DataMartOspExo"
				},
				{
					DataMartType.TenantSecurity,
					"DataMartTenantSecurity"
				},
				{
					DataMartType.ExoOutlook,
					"DataMartExoOutlook"
				}
			};
			this.dataMartServerMapping = new Dictionary<DataMartType, string>(this.dataMartKeyMapping.Count);
			this.dataMartDatabaseMapping = new Dictionary<DataMartType, string>(this.dataMartKeyMapping.Count);
		}

		public static DataMart Instance
		{
			get
			{
				if (DataMart.instance == null)
				{
					lock (DataMart.SyncRoot)
					{
						if (DataMart.instance == null)
						{
							DataMart.instance = new DataMart();
						}
					}
				}
				return DataMart.instance;
			}
		}

		public string GetConnectionString(DataMartType dataMartType, bool backup = false)
		{
			string result;
			try
			{
				string text = this.dataMartKeyMapping[dataMartType];
				string key = string.Format(CultureInfo.InvariantCulture, "{0}Server", new object[]
				{
					text
				});
				string text2 = DataMart.GetConfiguration(dataMartType, this.dataMartServerMapping, key);
				if (backup)
				{
					text2 = this.GetBackupServer(dataMartType);
					if (string.IsNullOrEmpty(text2))
					{
						return string.Empty;
					}
				}
				else
				{
					string text3 = Environment.MachineName.Substring(0, 3).ToUpper();
					if (text2.ToUpper().StartsWith("CDM-TENANTDS."))
					{
						text2 = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.exmgmt.local", new object[]
						{
							"CDM-TENANTDS",
							text3
						});
					}
					else if (text2.ToUpper().StartsWith("CDM-TENANTDS-SCALED."))
					{
						text2 = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.exmgmt.local", new object[]
						{
							"CDM-TENANTDS-SCALED",
							text3
						});
					}
				}
				string key2 = string.Format(CultureInfo.InvariantCulture, "{0}Database", new object[]
				{
					text
				});
				string configuration = DataMart.GetConfiguration(dataMartType, this.dataMartDatabaseMapping, key2);
				if (DataMart.connectionTimeout == -1)
				{
					DataMart.connectionTimeout = DataMart.LoadSettingFromRegistry<int>("SQLConnectionTimeout");
					DataMart.ValidateIntegerInRange("SQLConnectionTimeout", DataMart.connectionTimeout, 1, 180);
				}
				string text4 = string.Format(CultureInfo.InvariantCulture, "Server={0};Database={1};Integrated Security=SSPI;Connection Timeout={2}", new object[]
				{
					text2,
					configuration,
					DataMart.connectionTimeout
				});
				result = text4;
			}
			catch (DataMartConfigurationException ex)
			{
				ExTraceGlobals.LogTracer.TraceError<DataMartConfigurationException>(0L, "Load data mart configuration error: {0}", ex);
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_DataMartConfigurationError, new string[]
				{
					ex.Message
				});
				throw;
			}
			return result;
		}

		private string GetCFRBackupDataCenterNameByRegion(string regionKey, string serversKey, string localDataCenterName)
		{
			string text = DataMart.LoadSettingFromRegistry<string>(regionKey);
			if (!text.Contains(localDataCenterName))
			{
				return string.Empty;
			}
			string text2 = DataMart.LoadSettingFromRegistry<string>(serversKey);
			List<string> list = new List<string>(text2.Split(new char[]
			{
				','
			}));
			list.Remove(localDataCenterName);
			if (list.Count == 0)
			{
				return string.Empty;
			}
			Random random = new Random();
			return list[random.Next(list.Count)];
		}

		private string GetCFRBackupDataServerName(DataMartType dataMartType)
		{
			string localDataCenterName = Environment.MachineName.Substring(0, 3).ToUpper();
			string text = string.Empty;
			string result = string.Empty;
			try
			{
				text = this.GetCFRBackupDataCenterNameByRegion("DataMartTenantsRegionNAM", "DataMartTenantsServersNAM", localDataCenterName);
				if (string.IsNullOrEmpty(text))
				{
					text = this.GetCFRBackupDataCenterNameByRegion("DataMartTenantsRegionEUR", "DataMartTenantsServersEUR", localDataCenterName);
				}
				if (string.IsNullOrEmpty(text))
				{
					text = this.GetCFRBackupDataCenterNameByRegion("DataMartTenantsRegionAPC", "DataMartTenantsServersAPC", localDataCenterName);
				}
				if (string.IsNullOrEmpty(text))
				{
					result = DataMart.LoadSettingFromRegistry<string>("DataMartTenantsGlobalBackupServer");
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
			string text2 = string.Empty;
			switch (dataMartType)
			{
			case DataMartType.Tenants:
				text2 = "cdm-tenantds";
				break;
			case DataMartType.TenantsScaled:
				text2 = "cdm-tenantds-scaled";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.exmgmt.local", new object[]
				{
					text2,
					text
				});
			}
			return result;
		}

		private string GetBackupServer(DataMartType dataMartType)
		{
			switch (dataMartType)
			{
			case DataMartType.Tenants:
			case DataMartType.TenantsScaled:
				return this.GetCFRBackupDataServerName(dataMartType);
			default:
				return null;
			}
		}

		public int DefaultReportResultSize
		{
			get
			{
				DataMart.defaultReportResultSize = DataMart.LoadSettingFromRegistry<int>("DefaultReportResultSize");
				if (DataMart.defaultReportResultSize <= 0)
				{
					DataMart.defaultReportResultSize = 1000;
				}
				return DataMart.defaultReportResultSize;
			}
		}

		public bool IsTableFunctionQueryDisabled
		{
			get
			{
				DataMart.isTableFunctionQueryDisabled = DataMart.LoadSettingFromRegistry<int>("DisableTableFunctionQuery");
				return DataMart.isTableFunctionQueryDisabled == 1;
			}
		}

		private static string GetConfiguration(DataMartType dataMartType, Dictionary<DataMartType, string> mapping, string key)
		{
			if (!mapping.ContainsKey(dataMartType))
			{
				lock (DataMart.SyncRoot)
				{
					if (!mapping.ContainsKey(dataMartType))
					{
						string value = DataMart.LoadSettingFromRegistry<string>(key);
						DataMart.ValidateNotEmptyString(key, value);
						mapping[dataMartType] = value;
					}
				}
			}
			return mapping[dataMartType];
		}

		private static void ValidateNotEmptyString(string key, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new DataMartConfigurationException(Strings.EmptyStringConfiguration(key));
			}
		}

		private static void ValidateIntegerInRange(string key, int value, int min, int max)
		{
			if (value > max || value < min)
			{
				throw new DataMartConfigurationException(Strings.InvalidIntegerConfiguration(key, value, min, max));
			}
		}

		private static TValue LoadSettingFromRegistry<TValue>(string key)
		{
			TValue result = default(TValue);
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(DataMart.ReportingCmdletKeyRoot))
				{
					object obj = null;
					if (registryKey != null)
					{
						obj = registryKey.GetValue(key);
					}
					if (registryKey == null || obj == null)
					{
						throw new DataMartConfigurationException(Strings.RegistryKeyNotFound(key, DataMart.ReportingCmdletKeyRoot));
					}
					result = (TValue)((object)obj);
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (InvalidCastException ex3)
			{
				ex = ex3;
			}
			catch (FormatException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (UnauthorizedAccessException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				ExTraceGlobals.LogTracer.TraceError<Exception>(0L, "Error occurred when reading settings from registry. Exception: {0}", ex);
				throw new DataMartConfigurationException(Strings.FailedLoadRegistryKey(key, ex.Message), ex);
			}
			return result;
		}

		public const int MinConnectionTimeout = 1;

		public const int MaxConnectionTimeout = 180;

		private const string ReportingCmdletKeyRootFormat = "SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\Cmdlet\\Reporting";

		private const string ServerKeyFormat = "{0}Server";

		private const string DatabaseKeyFormat = "{0}Database";

		private const string ConnectionTimeoutKey = "SQLConnectionTimeout";

		private const string DefaultReportResultSizeKey = "DefaultReportResultSize";

		private const string DisableTableFunctionQueryKey = "DisableTableFunctionQuery";

		private const string DataMartTenantsRegionAPCKey = "DataMartTenantsRegionAPC";

		private const string DataMartTenantsRegionEURKey = "DataMartTenantsRegionEUR";

		private const string DataMartTenantsRegionNAMKey = "DataMartTenantsRegionNAM";

		private const string DataMartTenantsServersAPCKey = "DataMartTenantsServersAPC";

		private const string DataMartTenantsServersEURKey = "DataMartTenantsServersEUR";

		private const string DataMartTenantsServersNAMKey = "DataMartTenantsServersNAM";

		private const string DataMartTenantsServersGlobalBackupKey = "DataMartTenantsGlobalBackupServer";

		private const string ConnectionStringFormat = "Server={0};Database={1};Integrated Security=SSPI;Connection Timeout={2}";

		private const string CFRServerCNameFormat = "{0}.{1}.exmgmt.local";

		private static readonly string ReportingCmdletKeyRoot;

		private static readonly object SyncRoot = new object();

		private readonly Dictionary<DataMartType, string> dataMartKeyMapping;

		private readonly Dictionary<DataMartType, string> dataMartServerMapping;

		private readonly Dictionary<DataMartType, string> dataMartDatabaseMapping;

		private static int connectionTimeout = -1;

		private static int defaultReportResultSize = -1;

		private static int isTableFunctionQueryDisabled = 0;

		private static volatile DataMart instance;
	}
}

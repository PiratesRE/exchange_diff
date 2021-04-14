using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	public static class DatacenterRegistry
	{
		internal static bool IsMicrosoftHostedOnly()
		{
			if (DatacenterRegistry.isMicrosoftHostedOnly == null)
			{
				DatacenterRegistry.isMicrosoftHostedOnly = new bool?(DatacenterRegistry.CheckBooleanValue("DatacenterMode"));
			}
			return DatacenterRegistry.isMicrosoftHostedOnly.Value;
		}

		internal static bool TreatPreReqErrorsAsWarnings()
		{
			if (DatacenterRegistry.treatPreReqErrorsAsWarningsKey == null)
			{
				DatacenterRegistry.treatPreReqErrorsAsWarningsKey = new bool?(DatacenterRegistry.CheckBooleanValue("TreatPreReqErrorsAsWarnings"));
			}
			return DatacenterRegistry.treatPreReqErrorsAsWarningsKey.Value;
		}

		internal static bool IsForefrontForOffice()
		{
			if (DatacenterRegistry.isFfoDatacenter == null)
			{
				DatacenterRegistry.isFfoDatacenter = new bool?(DatacenterRegistry.CheckBooleanValue("ForefrontForOfficeMode"));
			}
			return DatacenterRegistry.isFfoDatacenter.Value;
		}

		internal static bool IsForefrontForOfficeDeployment()
		{
			if (DatacenterRegistry.isFfoDatacenterDeployment == null)
			{
				DatacenterRegistry.isFfoDatacenterDeployment = new bool?(DatacenterRegistry.CheckBooleanValue("FfoDeploymentMode"));
			}
			return DatacenterRegistry.isFfoDatacenterDeployment.Value;
		}

		internal static bool IsGallatinDatacenter()
		{
			if (DatacenterRegistry.isGallatinDatacenter == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "ServiceName");
				string a = (obj != null) ? obj.ToString() : string.Empty;
				DatacenterRegistry.isGallatinDatacenter = new bool?(string.Equals(a, "GALLATIN", StringComparison.InvariantCultureIgnoreCase));
			}
			return DatacenterRegistry.isGallatinDatacenter.Value;
		}

		internal static bool IsFFOGallatinDatacenter()
		{
			if (DatacenterRegistry.isFFOGallatinDatacenter == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "ServiceName");
				string a = (obj != null) ? obj.ToString() : string.Empty;
				DatacenterRegistry.isFFOGallatinDatacenter = new bool?(string.Equals(a, "FopePRODcn", StringComparison.InvariantCultureIgnoreCase));
			}
			return DatacenterRegistry.isFFOGallatinDatacenter.Value;
		}

		internal static string GetForefrontRegion()
		{
			if (DatacenterRegistry.ffoRegionValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "Region");
				DatacenterRegistry.ffoRegionValue = ((obj != null) ? obj.ToString() : string.Empty);
			}
			return DatacenterRegistry.ffoRegionValue;
		}

		internal static string GetForefrontRegionServiceInstance()
		{
			if (DatacenterRegistry.ffoRegionServiceInstanceValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "RegionServiceInstance");
				DatacenterRegistry.ffoRegionServiceInstanceValue = ((obj != null) ? obj.ToString() : string.Empty);
			}
			return DatacenterRegistry.ffoRegionServiceInstanceValue;
		}

		internal static string GetForefrontRegionTag()
		{
			if (DatacenterRegistry.ffoRegionTagValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "RegionTag");
				DatacenterRegistry.ffoRegionTagValue = ((obj != null) ? obj.ToString() : string.Empty);
			}
			return DatacenterRegistry.ffoRegionTagValue;
		}

		internal static string GetForefrontServiceTag()
		{
			if (DatacenterRegistry.ffoServiceTagValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "ServiceTag");
				DatacenterRegistry.ffoServiceTagValue = ((obj != null) ? obj.ToString() : string.Empty);
			}
			return DatacenterRegistry.ffoServiceTagValue;
		}

		internal static string GetForefrontDatacenter()
		{
			if (DatacenterRegistry.ffoDatacenterValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "Datacenter");
				if (obj != null)
				{
					DatacenterRegistry.ffoDatacenterValue = obj.ToString();
				}
				else
				{
					DatacenterRegistry.ffoDatacenterValue = string.Empty;
				}
			}
			return DatacenterRegistry.ffoDatacenterValue;
		}

		internal static string GetForefrontDomainDBSite()
		{
			if (DatacenterRegistry.ffoDomainDBSiteValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "DomainDBSite");
				if (obj != null)
				{
					DatacenterRegistry.ffoDomainDBSiteValue = obj.ToString();
				}
				else
				{
					DatacenterRegistry.ffoDomainDBSiteValue = string.Empty;
				}
			}
			return DatacenterRegistry.ffoDomainDBSiteValue;
		}

		internal static bool IsDatacenterDedicated()
		{
			if (DatacenterRegistry.isDatacenterDedicated == null)
			{
				DatacenterRegistry.isDatacenterDedicated = new bool?(DatacenterRegistry.CheckBooleanValue("DatacenterDedicated"));
			}
			return DatacenterRegistry.isDatacenterDedicated.Value;
		}

		internal static string GetForefrontFopeGlobalSite()
		{
			if (DatacenterRegistry.ffoFopeGlobalSiteValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "FopeGlobalSite");
				if (obj != null)
				{
					DatacenterRegistry.ffoFopeGlobalSiteValue = obj.ToString();
				}
				else
				{
					DatacenterRegistry.ffoFopeGlobalSiteValue = string.Empty;
				}
			}
			return DatacenterRegistry.ffoFopeGlobalSiteValue;
		}

		internal static string GetForefrontAlertEmail()
		{
			if (DatacenterRegistry.ffoAlertEmail == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "AlertEmail");
				if (obj != null)
				{
					DatacenterRegistry.ffoAlertEmail = obj.ToString();
				}
				else
				{
					DatacenterRegistry.ffoAlertEmail = string.Empty;
				}
			}
			return DatacenterRegistry.ffoAlertEmail;
		}

		internal static string GetForefrontArbitrationServiceUrl()
		{
			if (DatacenterRegistry.ffoArbitrationServiceUrlValue == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "ArbitrationServiceUrl");
				if (obj != null)
				{
					DatacenterRegistry.ffoArbitrationServiceUrlValue = obj.ToString();
				}
				else
				{
					DatacenterRegistry.ffoArbitrationServiceUrlValue = string.Empty;
				}
			}
			return DatacenterRegistry.ffoArbitrationServiceUrlValue;
		}

		internal static bool IsPartnerHostedOnly()
		{
			if (DatacenterRegistry.isPartnerHostedOnly == null)
			{
				object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "PartnerHostedMode");
				DatacenterRegistry.isPartnerHostedOnly = new bool?(obj is int && (int)obj == 1);
			}
			return DatacenterRegistry.isPartnerHostedOnly.Value;
		}

		internal static void CreatePartnerHostedRegistryKey()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15"))
			{
				registryKey.SetValue("PartnerHostedMode", 1, RegistryValueKind.DWord);
			}
		}

		internal static void RemovePartnerHostedRegistryKey()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15", true))
			{
				registryKey.DeleteValue("PartnerHostedMode", false);
			}
		}

		internal static bool IsDualWriteAllowed()
		{
			if (DatacenterRegistry.isFfoDualWriteAllowed == null)
			{
				if (ExEnvironment.IsTest)
				{
					DatacenterRegistry.isFfoDualWriteAllowed = new bool?(DatacenterRegistry.CheckBooleanValue("FfoDualWriteAllowed"));
				}
				else
				{
					DatacenterRegistry.isFfoDualWriteAllowed = new bool?(true);
				}
			}
			return DatacenterRegistry.isFfoDualWriteAllowed.Value;
		}

		private static bool CheckBooleanValue(string valueName)
		{
			object obj = DatacenterRegistry.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", valueName);
			if (obj == null)
			{
				return false;
			}
			if (obj is int)
			{
				bool result;
				switch ((int)obj)
				{
				case 0:
					result = false;
					break;
				case 1:
					result = true;
					break;
				default:
					throw new DatacenterInvalidRegistryException();
				}
				return result;
			}
			throw new DatacenterInvalidRegistryException();
		}

		private static object ReadRegistryKey(string keyPath, string valueName)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyPath))
			{
				if (registryKey != null)
				{
					return registryKey.GetValue(valueName, null);
				}
			}
			return null;
		}

		private const string PartnerHostedKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private const string PartnerHostedValueName = "PartnerHostedMode";

		private const string MicrosoftHostedKeyPath = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private const string MicrosoftHostedValueName = "DatacenterMode";

		private const string MicrosoftHostedServiceName = "ServiceName";

		private const string MicrosoftDatacenterDedicatedValueName = "DatacenterDedicated";

		private const string TreatPreReqErrorsAsWarningsKey = "TreatPreReqErrorsAsWarnings";

		private const string FfoDualWriteAllowedValueName = "FfoDualWriteAllowed";

		private const string FfoValueName = "ForefrontForOfficeMode";

		private const string FfoDeploymentValueName = "FfoDeploymentMode";

		private const string FfoRegion = "Region";

		private const string FfoRegionServiceInstance = "RegionServiceInstance";

		private const string FfoRegionTag = "RegionTag";

		private const string FfoServiceTag = "ServiceTag";

		private const string FfoDatacenter = "Datacenter";

		private const string FfoDomainDBSite = "DomainDBSite";

		private const string FfoFopeGlobalSite = "FopeGlobalSite";

		private const string FfoAlertEmail = "AlertEmail";

		private const string FfoArbitrationServiceUrl = "ArbitrationServiceUrl";

		private static bool? isFfoDualWriteAllowed = null;

		private static bool? isFfoDatacenter = null;

		private static bool? isFfoDatacenterDeployment = null;

		private static bool? isGallatinDatacenter = null;

		private static bool? isFFOGallatinDatacenter = null;

		private static string ffoRegionValue = null;

		private static string ffoRegionServiceInstanceValue = null;

		private static string ffoRegionTagValue = null;

		private static string ffoServiceTagValue = null;

		private static string ffoDatacenterValue = null;

		private static string ffoDomainDBSiteValue = null;

		private static string ffoFopeGlobalSiteValue = null;

		private static string ffoAlertEmail = null;

		private static string ffoArbitrationServiceUrlValue = null;

		private static bool? isMicrosoftHostedOnly;

		private static bool? treatPreReqErrorsAsWarningsKey;

		private static bool? isDatacenterDedicated;

		private static bool? isPartnerHostedOnly;
	}
}

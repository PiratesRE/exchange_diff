using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class OrganizationCache
	{
		static OrganizationCache()
		{
			int num = 0;
			if (!int.TryParse(ConfigurationManager.AppSettings["OrgCacheLifetimeInMinute"], out num))
			{
				num = 30;
			}
			else if (num < 15)
			{
				num = 15;
			}
			OrganizationCache.orgCacheLifeTime = new TimeSpan(0, num, 0);
			OrganizationCache.RegisterImpl("EntHasTargetDeliveryDomain", "Get-RemoteDomain", new OrganizationCache.LoadHandler(OrganizationCache.LoadTargetDeliveryDomain), !Util.IsDataCenter, OrganizationCacheExpirationType.Default, null);
			OrganizationCache.RegisterImpl("EntTargetDeliveryDomain", "Get-RemoteDomain", new OrganizationCache.LoadHandler(OrganizationCache.LoadTargetDeliveryDomain), !Util.IsDataCenter, OrganizationCacheExpirationType.Default, null);
			OrganizationCache.RegisterImpl("CrossPremiseUrlFormat", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadCrossPremiseUrl), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("CrossPremiseServer", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadCrossPremiseUrl), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("CrossPremiseUrlFormatWorldWide", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadCrossPremiseUrl), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("CrossPremiseServerWorldWide", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadCrossPremiseUrl), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("CrossPremiseUrlFormatGallatin", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadCrossPremiseUrl), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("CrossPremiseServerGallatin", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadCrossPremiseUrl), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("ServiceInstance", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadCrossPremiseServiceInstance), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("RestrictIOCToSP1OrGreaterWorldWide", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadRestrictIOCToSP1OrGreater), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("RestrictIOCToSP1OrGreaterGallatin", "ControlPanelAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadRestrictIOCToSP1OrGreater), !Util.IsDataCenter, OrganizationCacheExpirationType.Never, null);
			OrganizationCache.RegisterImpl("DCIsDirSyncRunning", "HybridAdmin", new OrganizationCache.LoadHandler(OrganizationCache.LoadDCIsDirSyncRunning), Util.IsMicrosoftHostedOnly, OrganizationCacheExpirationType.Default, null);
		}

		public static void Register(string key, string role, OrganizationCache.LoadHandler loader, bool skuMatch = true, OrganizationCacheExpirationType expiration = OrganizationCacheExpirationType.Default, TimeSpan? customExpireTime = null)
		{
			lock (OrganizationCache.definitionStore)
			{
				OrganizationCache.RegisterImpl(key, role, loader, skuMatch, expiration, customExpireTime);
			}
		}

		private static void RegisterImpl(string key, string role, OrganizationCache.LoadHandler loader, bool skuMatch = true, OrganizationCacheExpirationType expiration = OrganizationCacheExpirationType.Default, TimeSpan? customExpireTime = null)
		{
			if (expiration == OrganizationCacheExpirationType.Custom != (customExpireTime != null))
			{
				throw new ArgumentException("CustomExpireTime must be specified together with expiration as Custom.");
			}
			TimeSpan item = (expiration == OrganizationCacheExpirationType.Default) ? OrganizationCache.orgCacheLifeTime : ((expiration == OrganizationCacheExpirationType.Never) ? TimeSpan.MaxValue : customExpireTime.Value);
			Tuple<bool, string, OrganizationCache.LoadHandler, TimeSpan> value = new Tuple<bool, string, OrganizationCache.LoadHandler, TimeSpan>(skuMatch, role, loader, item);
			OrganizationCache.definitionStore.Add(key, value);
		}

		public static bool KeyRegistered(string key)
		{
			return OrganizationCache.definitionStore.ContainsKey(key);
		}

		private static Dictionary<string, Tuple<object, DateTime>> ValueStore
		{
			get
			{
				Dictionary<string, Tuple<object, DateTime>> result = null;
				if (OrganizationCache.orgIdForTest == null && !Util.IsDataCenter)
				{
					if (OrganizationCache.entValueStore == null)
					{
						lock (OrganizationCache.syncObject)
						{
							if (OrganizationCache.entValueStore == null)
							{
								OrganizationCache.entValueStore = new Dictionary<string, Tuple<object, DateTime>>(16, StringComparer.OrdinalIgnoreCase);
							}
						}
					}
					result = OrganizationCache.entValueStore;
				}
				else
				{
					if (OrganizationCache.dcValueStores == null)
					{
						lock (OrganizationCache.syncObject)
						{
							if (OrganizationCache.dcValueStores == null)
							{
								OrganizationCache.dcValueStores = new MruDictionaryCache<string, Dictionary<string, Tuple<object, DateTime>>>(32, 720);
							}
						}
					}
					string text = OrganizationCache.orgIdForTest ?? RbacPrincipal.Current.RbacConfiguration.OrganizationId.OrganizationalUnit.ToString();
					if (!OrganizationCache.dcValueStores.TryGetValue(text, out result))
					{
						lock (OrganizationCache.dcValueStores)
						{
							if (!OrganizationCache.dcValueStores.TryGetValue(text, out result))
							{
								OrganizationCache.dcValueStores.Add(text, new Dictionary<string, Tuple<object, DateTime>>(16, StringComparer.OrdinalIgnoreCase));
							}
						}
						result = OrganizationCache.dcValueStores[text];
					}
				}
				return result;
			}
		}

		internal static void SetTestTenantId(string testTenantId)
		{
			OrganizationCache.orgIdForTest = testTenantId;
		}

		internal static void ExpireEntry(string key)
		{
			if (OrganizationCache.ValueStore.ContainsKey(key))
			{
				OrganizationCache.ValueStore.Remove(key);
			}
		}

		public static T GetValue<T>(string key)
		{
			T result;
			OrganizationCache.TryGetValue<T>(key, out result);
			return result;
		}

		public static bool TryGetValue<T>(string key, out T value)
		{
			bool flag = false;
			value = default(T);
			Tuple<bool, string, OrganizationCache.LoadHandler, TimeSpan> tuple = null;
			if (OrganizationCache.definitionStore.TryGetValue(key, out tuple) && tuple.Item1)
			{
				Dictionary<string, Tuple<object, DateTime>> valueStore = OrganizationCache.ValueStore;
				flag = (string.IsNullOrEmpty(tuple.Item2) || RbacPrincipal.Current.IsInRole(tuple.Item2));
				if (flag)
				{
					Tuple<object, DateTime> tuple2;
					if (!valueStore.TryGetValue(key, out tuple2) || tuple2.Item2 < DateTime.UtcNow)
					{
						tuple.Item3(new OrganizationCache.AddValueHandler(OrganizationCache.AddValue), new OrganizationCache.LogErrorHandler(OrganizationCache.LogError));
						valueStore.TryGetValue(key, out tuple2);
					}
					if (tuple2 != null)
					{
						value = (T)((object)tuple2.Item1);
					}
					else
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		private static void AddValue(string key, object value)
		{
			TimeSpan item = OrganizationCache.definitionStore[key].Item4;
			DateTime item2 = (item == TimeSpan.MaxValue) ? DateTime.MaxValue : (DateTime.UtcNow + item);
			Dictionary<string, Tuple<object, DateTime>> valueStore = OrganizationCache.ValueStore;
			Tuple<object, DateTime> value2 = new Tuple<object, DateTime>(value, item2);
			lock (valueStore)
			{
				valueStore[key] = value2;
			}
		}

		private static void LogError(string key, string errorMessage)
		{
			EcpEventLogConstants.Tuple_UnableToDetectRbacRoleViaCmdlet.LogEvent(new object[]
			{
				EcpEventLogExtensions.GetUserNameToLog(),
				key,
				errorMessage
			});
		}

		public static bool EntHasTargetDeliveryDomain
		{
			get
			{
				return OrganizationCache.GetValue<bool>("EntHasTargetDeliveryDomain");
			}
		}

		public static string EntTargetDeliveryDomain
		{
			get
			{
				return OrganizationCache.GetValue<string>("EntTargetDeliveryDomain");
			}
		}

		private static void LoadTargetDeliveryDomain(OrganizationCache.AddValueHandler addValue, OrganizationCache.LogErrorHandler logError)
		{
			WebServiceReference webServiceReference = new WebServiceReference("~/DDI/DDIService.svc?schema=RemoteDomains");
			PowerShellResults<JsonDictionary<object>> list = webServiceReference.GetList(null, null);
			bool flag = false;
			string value = null;
			if (list.Output != null)
			{
				for (int i = 0; i < list.Output.Length; i++)
				{
					Dictionary<string, object> dictionary = list.Output[i];
					foreach (KeyValuePair<string, object> keyValuePair in dictionary)
					{
						if (keyValuePair.Key == "DomainName")
						{
							value = (string)keyValuePair.Value;
						}
						else if (keyValuePair.Key == "TargetDeliveryDomain")
						{
							flag = (bool)keyValuePair.Value;
						}
					}
					if (flag)
					{
						break;
					}
					value = null;
				}
			}
			if (!list.ErrorRecords.IsNullOrEmpty())
			{
				string errorMessage = list.ErrorRecords[0].ToString();
				logError("EntHasTargetDeliveryDomain", errorMessage);
			}
			addValue("EntTargetDeliveryDomain", value);
			addValue("EntHasTargetDeliveryDomain", flag);
		}

		public static string ServiceInstance
		{
			get
			{
				return OrganizationCache.GetValue<string>("ServiceInstance");
			}
		}

		public static bool EntHasServiceInstance
		{
			get
			{
				return !string.IsNullOrEmpty(OrganizationCache.ServiceInstance);
			}
		}

		private static void LoadCrossPremiseServiceInstance(OrganizationCache.AddValueHandler addValue, OrganizationCache.LogErrorHandler logError)
		{
			string value = string.Empty;
			try
			{
				WebServiceReference webServiceReference = new WebServiceReference("~/DDI/DDIService.svc?schema=HybridConfigurationWizardService&workflow=GetServiceInstance");
				PowerShellResults<JsonDictionary<object>> powerShellResults = (PowerShellResults<JsonDictionary<object>>)webServiceReference.GetObject(null);
				if (powerShellResults.Output.Length > 0)
				{
					value = (string)powerShellResults.Output[0].RawDictionary["ServiceInstance"];
				}
			}
			catch (Exception ex)
			{
				logError("ServiceInstance", ex.ToString());
			}
			addValue("ServiceInstance", value);
		}

		public static string CrossPremiseServer
		{
			get
			{
				return OrganizationCache.GetValue<string>("CrossPremiseServer");
			}
		}

		public static string CrossPremiseServerWorldWide
		{
			get
			{
				return OrganizationCache.GetValue<string>("CrossPremiseServerWorldWide");
			}
		}

		public static string CrossPremiseServerGallatin
		{
			get
			{
				return OrganizationCache.GetValue<string>("CrossPremiseServerGallatin");
			}
		}

		public static string CrossPremiseUrlFormat
		{
			get
			{
				return OrganizationCache.GetValue<string>("CrossPremiseUrlFormat");
			}
		}

		public static string CrossPremiseUrlFormatWorldWide
		{
			get
			{
				return OrganizationCache.GetValue<string>("CrossPremiseUrlFormatWorldWide");
			}
		}

		public static string CrossPremiseUrlFormatGallatin
		{
			get
			{
				return OrganizationCache.GetValue<string>("CrossPremiseUrlFormatGallatin");
			}
		}

		private static void LoadCrossPremiseUrl(OrganizationCache.AddValueHandler addValue, OrganizationCache.LogErrorHandler logError)
		{
			string value = string.Empty;
			string text = string.Empty;
			string text2 = string.Empty;
			string value2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			try
			{
				string text5 = ConfigurationManager.AppSettings["HybridServerUrl0"] ?? "https://outlook.office365.com/ecp/";
				string text6 = ConfigurationManager.AppSettings["HybridServerUrl1"] ?? "https://partner.outlook.cn/ecp/";
				text = new Uri(text5).Host;
				text3 = string.Format("{0}hybrid.aspx?xprs={{0}}&xprf={{1}}&xprv={1}&realm={{2}}&exsvurl=1", text5, Util.ApplicationVersion);
				text2 = new Uri(text6).Host;
				text4 = string.Format("{0}hybrid.aspx?xprs={{0}}&xprf={{1}}&xprv={1}&realm={{2}}&exsvurl=1", text6, Util.ApplicationVersion);
			}
			catch (UriFormatException ex)
			{
				logError("CrossPremiseServer", ex.ToString());
			}
			string serviceInstance;
			if ((serviceInstance = OrganizationCache.ServiceInstance) != null)
			{
				if (!(serviceInstance == "0"))
				{
					if (serviceInstance == "1")
					{
						value = text2;
						value2 = text4;
					}
				}
				else
				{
					value = text;
					value2 = text3;
				}
			}
			addValue("CrossPremiseServer", value);
			addValue("CrossPremiseUrlFormat", value2);
			addValue("CrossPremiseServerWorldWide", text);
			addValue("CrossPremiseUrlFormatWorldWide", text3);
			addValue("CrossPremiseServerGallatin", text2);
			addValue("CrossPremiseUrlFormatGallatin", text4);
		}

		public static bool DCIsDirSyncRunning
		{
			get
			{
				return OrganizationCache.GetValue<bool>("DCIsDirSyncRunning");
			}
		}

		private static void LoadDCIsDirSyncRunning(OrganizationCache.AddValueHandler addValue, OrganizationCache.LogErrorHandler logError)
		{
			OrganizationId organizationId = RbacPrincipal.Current.RbacConfiguration.OrganizationId;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 668, "LoadDCIsDirSyncRunning", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Organization\\OrganizationCache.cs");
			ADRawEntry adrawEntry = tenantOrTopologyConfigurationSession.ReadADRawEntry(organizationId.ConfigurationUnit, new PropertyDefinition[]
			{
				OrganizationSchema.IsDirSyncRunning
			});
			bool flag = adrawEntry != null && (bool)adrawEntry[OrganizationSchema.IsDirSyncRunning];
			addValue("DCIsDirSyncRunning", flag);
		}

		public static bool RestrictIOCToSP1OrGreaterWorldWide
		{
			get
			{
				return OrganizationCache.GetValue<bool>("RestrictIOCToSP1OrGreaterWorldWide");
			}
		}

		public static bool RestrictIOCToSP1OrGreaterGallatin
		{
			get
			{
				return OrganizationCache.GetValue<bool>("RestrictIOCToSP1OrGreaterGallatin");
			}
		}

		private static void LoadRestrictIOCToSP1OrGreater(OrganizationCache.AddValueHandler addValue, OrganizationCache.LogErrorHandler logError)
		{
			string text = "true";
			string text2 = "false";
			try
			{
				text = ConfigurationManager.AppSettings["RestrictIOCToSP1OrGreater0"];
				if (string.IsNullOrEmpty(text))
				{
					text = "true";
				}
				text2 = ConfigurationManager.AppSettings["RestrictIOCToSP1OrGreater1"];
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "false";
				}
			}
			catch (UriFormatException ex)
			{
				logError("RestrictIOCToSP1OrGreaterWorldWide", ex.ToString());
			}
			addValue("RestrictIOCToSP1OrGreaterWorldWide", !string.Equals(text, "false"));
			addValue("RestrictIOCToSP1OrGreaterGallatin", !string.Equals(text2, "false"));
		}

		public const string EntTargetDeliveryDomainKey = "EntTargetDeliveryDomain";

		public const string EntHasTargetDeliveryDomainKey = "EntHasTargetDeliveryDomain";

		public const string CrossPremiseUrlFormatKey = "CrossPremiseUrlFormat";

		public const string CrossPremiseServerKey = "CrossPremiseServer";

		public const string CrossPremiseUrlFormatWorldWideKey = "CrossPremiseUrlFormatWorldWide";

		public const string CrossPremiseServerWorldWideKey = "CrossPremiseServerWorldWide";

		public const string CrossPremiseUrlFormatGallatinKey = "CrossPremiseUrlFormatGallatin";

		public const string CrossPremiseServerGallatinKey = "CrossPremiseServerGallatin";

		public const string DCIsDirSyncRunningKey = "DCIsDirSyncRunning";

		public const string ServiceInstanceKey = "ServiceInstance";

		public const string RestrictIOCToSP1OrGreaterWorldWideKey = "RestrictIOCToSP1OrGreaterWorldWide";

		public const string RestrictIOCToSP1OrGreaterGallatinKey = "RestrictIOCToSP1OrGreaterGallatin";

		private const int OneOrgCacheSize = 16;

		private const int CacheSizeTenantNumber = 32;

		private const int TenantCacheRetireInMinutes = 720;

		private const int DefaultOrgCacheLifeTimeInMinute = 30;

		private const int MinOrgCacheLifeTimeInMinute = 15;

		private static readonly TimeSpan orgCacheLifeTime;

		private static readonly Dictionary<string, Tuple<bool, string, OrganizationCache.LoadHandler, TimeSpan>> definitionStore = new Dictionary<string, Tuple<bool, string, OrganizationCache.LoadHandler, TimeSpan>>(16, StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, Tuple<object, DateTime>> entValueStore;

		private static MruDictionaryCache<string, Dictionary<string, Tuple<object, DateTime>>> dcValueStores;

		private static readonly object syncObject = new object();

		private static string orgIdForTest = null;

		public delegate void LoadHandler(OrganizationCache.AddValueHandler addValue, OrganizationCache.LogErrorHandler logError);

		public delegate void AddValueHandler(string key, object value);

		public delegate void LogErrorHandler(string key, string errorMessage);
	}
}

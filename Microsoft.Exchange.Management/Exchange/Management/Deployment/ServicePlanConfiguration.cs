using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class ServicePlanConfiguration
	{
		internal IEnumerable<string> SupportedOffers
		{
			get
			{
				if (this.servicePlanLookupTable != null)
				{
					return this.servicePlanLookupTable.Keys;
				}
				return null;
			}
		}

		static ServicePlanConfiguration()
		{
			ServicePlanConfiguration.servicePlansFolder = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\ServicePlans");
			if (Datacenter.IsPartnerHostedOnly(false))
			{
				ServicePlanConfiguration.remapFile = Path.Combine(ServicePlanConfiguration.servicePlansFolder, "ServicePlanHostingRemap.csv");
				return;
			}
			ServicePlanConfiguration.remapFile = Path.Combine(ServicePlanConfiguration.servicePlansFolder, "ServicePlanRemap.csv");
		}

		private ServicePlanConfiguration()
		{
			this.servicePlanLookupTable = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			this.capabilityToOfferIdList = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			this.hydrationOfferLookupTable = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			this.reverseHydrationOfferLookupTable = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			this.pilotOfferLookupTable = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			this.reversePilotOfferLookupTable = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			this.allowedSourceSevicePlanForPilotTable = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			this.hydratedOffers = new List<string>();
			this.pilotOffers = new List<string>();
			this.LoadConfigFile();
			this.configFileLastReadTimeUtc = DateTime.UtcNow;
		}

		public static ServicePlanConfiguration GetInstance()
		{
			if (ServicePlanConfiguration.instance == null || ServicePlanConfiguration.instance.configFileLastReadTimeUtc < File.GetLastWriteTimeUtc(ServicePlanConfiguration.remapFile))
			{
				lock (typeof(ServicePlanConfiguration.SyncRoot))
				{
					if (ServicePlanConfiguration.instance == null || ServicePlanConfiguration.instance.configFileLastReadTimeUtc < File.GetLastWriteTimeUtc(ServicePlanConfiguration.remapFile))
					{
						ServicePlanConfiguration.instance = new ServicePlanConfiguration();
					}
				}
			}
			return ServicePlanConfiguration.instance;
		}

		internal string ResolveServicePlanName(string programId, string offerId)
		{
			string key = this.BuildLookupKey(programId, offerId);
			string result;
			if (this.servicePlanLookupTable.TryGetValue(key, out result))
			{
				return result;
			}
			throw new ArgumentException(Strings.MissingMapping(ServicePlanConfiguration.remapFile, programId, offerId));
		}

		public ServicePlan GetServicePlanSettings(string programId, string offerId)
		{
			return this.GetServicePlanSettings(this.ResolveServicePlanName(programId, offerId));
		}

		public ServicePlan GetServicePlanSettings(string servicePlanName)
		{
			string filePath = Path.Combine(ServicePlanConfiguration.servicePlansFolder, servicePlanName + ".servicePlan");
			ServicePlan servicePlan = ServicePlan.LoadFromFile(filePath);
			servicePlan.Name = servicePlanName;
			return servicePlan;
		}

		public bool TryGetOfferIdFromCapabilities(List<string> capabilities, out string offerId)
		{
			offerId = string.Empty;
			foreach (string text in capabilities)
			{
				if (this.capabilityToOfferIdList.ContainsKey(text))
				{
					if (!string.IsNullOrEmpty(offerId))
					{
						if (!string.Equals(offerId, this.capabilityToOfferIdList[text], StringComparison.InvariantCultureIgnoreCase))
						{
							offerId = null;
							throw new ArgumentException(Strings.CapabilityDoesNotMatchOthers(text));
						}
					}
					else
					{
						offerId = this.capabilityToOfferIdList[text];
					}
				}
			}
			return !string.IsNullOrEmpty(offerId);
		}

		public bool TryGetHydratedOfferId(string programId, string dehydratedOfferId, out string hydratedOfferId)
		{
			hydratedOfferId = string.Empty;
			string key = this.BuildLookupKey(programId, dehydratedOfferId);
			return this.hydrationOfferLookupTable.TryGetValue(key, out hydratedOfferId);
		}

		public bool TryGetReverseHydratedOfferId(string programId, string hydratedOfferId, out string dehydratedOfferId)
		{
			dehydratedOfferId = string.Empty;
			string key = this.BuildLookupKey(programId, hydratedOfferId);
			return this.reverseHydrationOfferLookupTable.TryGetValue(key, out dehydratedOfferId);
		}

		public bool TryGetPilotOfferId(string programId, string offerId, out string pilotOfferId)
		{
			pilotOfferId = string.Empty;
			string key = this.BuildLookupKey(programId, offerId);
			return this.pilotOfferLookupTable.TryGetValue(key, out pilotOfferId);
		}

		public bool TryGetReversePilotOfferId(string programId, string pilotOfferId, out string targetOfferId)
		{
			targetOfferId = string.Empty;
			string key = this.BuildLookupKey(programId, pilotOfferId);
			return this.reversePilotOfferLookupTable.TryGetValue(key, out targetOfferId);
		}

		public bool TryGetAllowedSorceServicePlanForPilot(string programId, string pilotOfferId, out string sourceServicePlan)
		{
			sourceServicePlan = string.Empty;
			string key = this.BuildLookupKey(programId, pilotOfferId);
			return this.allowedSourceSevicePlanForPilotTable.TryGetValue(key, out sourceServicePlan);
		}

		private void LoadConfigFile()
		{
			if (this.servicePlanLookupTable.Keys.Count > 0)
			{
				this.servicePlanLookupTable.Clear();
			}
			Exception ex = null;
			try
			{
				List<string> list = new List<string>();
				using (FileStream fileStream = new FileStream(ServicePlanConfiguration.remapFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (TextReader textReader = new StreamReader(fileStream))
					{
						int num = 0;
						string text;
						while ((text = textReader.ReadLine()) != null)
						{
							num++;
							text = text.Trim();
							if (text.Length != 0 && text[0] != '#')
							{
								string[] array = text.Split(new char[]
								{
									','
								});
								if (array.Length != 3 && array.Length != 4 && array.Length != 5 && array.Length != 6 && array.Length != 7)
								{
									throw new ArgumentException(Strings.WrongNumberOfTokens(num.ToString(), ServicePlanConfiguration.remapFile, 3.ToString()));
								}
								for (int i = 0; i < array.Length; i++)
								{
									array[i] = array[i].Trim();
									if (array[i].Length == 0 && i < 3)
									{
										throw new ArgumentException(Strings.EmptyToken(num.ToString(), ServicePlanConfiguration.remapFile, i.ToString()));
									}
								}
								string text2 = this.BuildLookupKey(array[0], array[1]);
								string text3;
								if (this.servicePlanLookupTable.TryGetValue(text2, out text3))
								{
									throw new ArgumentException(Strings.DuplicateTriplet(num.ToString(), ServicePlanConfiguration.remapFile, array[0], array[1]));
								}
								this.servicePlanLookupTable[text2] = array[2];
								if (array.Length >= 4 && !string.IsNullOrEmpty(array[3]) && array[0].Equals("MSOnline", StringComparison.InvariantCultureIgnoreCase))
								{
									if (!array[3].StartsWith("{") || !array[3].EndsWith("}"))
									{
										throw new ArgumentException(Strings.IncorrectlyFormattedColumn(num.ToString(), ServicePlanConfiguration.remapFile));
									}
									array[3] = array[3].TrimStart(new char[]
									{
										'{'
									});
									array[3] = array[3].TrimEnd(new char[]
									{
										'}'
									});
									string[] array2 = array[3].Split(new char[]
									{
										';'
									});
									for (int j = 0; j < array2.Length; j++)
									{
										array2[j] = array2[j].Trim();
										if (array2[j].Length == 0)
										{
											throw new ArgumentException(Strings.EmptyToken(num.ToString(), ServicePlanConfiguration.remapFile, j.ToString()));
										}
										if (this.capabilityToOfferIdList.ContainsKey(array2[j]))
										{
											throw new ArgumentException(Strings.DuplicateCapabilityMapping(num.ToString(), ServicePlanConfiguration.remapFile, array2[j]));
										}
										this.capabilityToOfferIdList[array2[j]] = array[1];
									}
								}
								if (array.Length >= 5 && !string.IsNullOrEmpty(array[4]))
								{
									string text4 = this.BuildLookupKey(array[0], array[4]);
									this.hydratedOffers.Add(text4);
									this.hydrationOfferLookupTable[text2] = array[4];
									this.reverseHydrationOfferLookupTable[text4] = array[1];
								}
								if (array.Length >= 6 && !string.IsNullOrEmpty(array[5]))
								{
									string text5 = this.BuildLookupKey(array[0], array[5]);
									this.pilotOffers.Add(text5);
									this.pilotOfferLookupTable[text2] = array[5];
									list.Add(text2);
									this.reversePilotOfferLookupTable[text5] = array[1];
								}
								if (array.Length >= 7 && !string.IsNullOrEmpty(array[6]) && !string.IsNullOrEmpty(array[5]))
								{
									string key = this.BuildLookupKey(array[0], array[5]);
									this.allowedSourceSevicePlanForPilotTable[key] = array[6];
								}
							}
						}
					}
				}
				foreach (string text6 in this.hydratedOffers)
				{
					string text7;
					if (!this.servicePlanLookupTable.TryGetValue(text6, out text7))
					{
						throw new ArgumentException(Strings.MissingHydratedOffer(ServicePlanConfiguration.remapFile, text6));
					}
				}
				foreach (string text8 in this.pilotOffers)
				{
					string text9;
					if (!this.servicePlanLookupTable.TryGetValue(text8, out text9))
					{
						throw new ArgumentException(Strings.MissingPilotOffer(ServicePlanConfiguration.remapFile, text8));
					}
				}
				foreach (string text10 in list)
				{
					string text11;
					if (!this.servicePlanLookupTable.TryGetValue(text10, out text11))
					{
						throw new ArgumentException(Strings.MissingPilotTargetOffer(ServicePlanConfiguration.remapFile, text10));
					}
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new IOException(Strings.NoPermissionToReadFile(ServicePlanConfiguration.remapFile), ex);
			}
		}

		private string BuildLookupKey(string programId, string offerId)
		{
			return string.Format("{0}-{1}", programId, offerId);
		}

		internal bool TryParseLookupKey(string key, out string programId, out string offerId)
		{
			programId = null;
			offerId = null;
			if (!string.IsNullOrEmpty(key))
			{
				string[] array = key.Split(new char[]
				{
					'-'
				});
				if (array.Length == 2 && !string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array[1]))
				{
					programId = array[0].Trim();
					offerId = array[1].Trim();
					return true;
				}
			}
			return false;
		}

		internal bool IsHydratedOffer(string plan)
		{
			foreach (string key in this.hydratedOffers)
			{
				string a;
				if (this.servicePlanLookupTable.TryGetValue(key, out a) && string.Equals(a, plan, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal bool IsPilotOffer(string programId, string offerId)
		{
			string value = this.BuildLookupKey(programId, offerId);
			return this.pilotOffers.Contains(value, StringComparer.OrdinalIgnoreCase);
		}

		public static bool IsDeprecatedServicePlan(string plan)
		{
			switch (plan)
			{
			case "EDU":
			case "SegmentedGalEDU":
			case "SearchDisabledEDU":
			case "FFDF":
			case "FFDF_V2":
			case "FFSDF":
			case "FFSDF_E14_R4":
			case "MSIT":
			case "OutlookLive":
			case "Skyline":
			case "Skyline_E14_R3":
			case "Skyline_E14_R4":
				return true;
			}
			return false;
		}

		public bool IsSharedConfigurationAllowedForServicePlan(string programId, string offerId)
		{
			return this.IsSharedConfigurationAllowedForServicePlan(this.GetServicePlanSettings(programId, offerId));
		}

		public bool IsSharedConfigurationAllowedForServicePlan(ServicePlan servicePlan)
		{
			return servicePlan.Organization.ShareableConfigurationEnabled;
		}

		public bool IsTemplateTenantServicePlan(ServicePlan servicePlan)
		{
			return servicePlan.Organization.TemplateTenant;
		}

		public bool DoAllCoreSCTsExistForVersion(ServerVersion version, PartitionId partitionId)
		{
			foreach (string offerId in this.coreOffers)
			{
				new SharedConfigurationInfo(version, "MSOnline", offerId);
				if (!SharedConfiguration.DoesSctExistForVersion(version, "MSOnline", offerId, partitionId))
				{
					return false;
				}
			}
			return true;
		}

		internal const string ServicePlanExtension = ".servicePlan";

		internal const string ServicePlansSubFolder = "ClientAccess\\ServicePlans";

		private const string remapFileName = "ServicePlanRemap.csv";

		private const string remapHostingFileName = "ServicePlanHostingRemap.csv";

		private const string MSOnlineProgramID = "MSOnline";

		private const int requiredNumberOfColumns = 3;

		private static readonly string servicePlansFolder;

		private static readonly string remapFile;

		private static ServicePlanConfiguration instance;

		private readonly Dictionary<string, string> servicePlanLookupTable;

		private readonly Dictionary<string, string> hydrationOfferLookupTable;

		private readonly Dictionary<string, string> reverseHydrationOfferLookupTable;

		private readonly Dictionary<string, string> pilotOfferLookupTable;

		private readonly Dictionary<string, string> reversePilotOfferLookupTable;

		private readonly Dictionary<string, string> allowedSourceSevicePlanForPilotTable;

		private readonly List<string> hydratedOffers;

		private readonly List<string> pilotOffers;

		private readonly string[] coreOffers = new string[]
		{
			"BPOS_L_Hydrated",
			"BPOS_M_Hydrated",
			"BPOS_S_Hydrated",
			"BPOS_Basic_CustomDomain_Hydrated"
		};

		private readonly DateTime configFileLastReadTimeUtc;

		private readonly Dictionary<string, string> capabilityToOfferIdList;

		internal static readonly HashSet<Tuple<ServicePlanOffer, ServicePlanOffer>> CrossSkuSupportedOffers = new HashSet<Tuple<ServicePlanOffer, ServicePlanOffer>>
		{
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_S"), new ServicePlanOffer("MSOnline", "BPOS_L")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_L"), new ServicePlanOffer("MSOnline", "BPOS_S")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_S_Hydrated"), new ServicePlanOffer("MSOnline", "BPOS_L_Hydrated")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_L_Hydrated"), new ServicePlanOffer("MSOnline", "BPOS_S_Hydrated")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_L"), new ServicePlanOffer("MSOnline", "BPOS_M")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_M"), new ServicePlanOffer("MSOnline", "BPOS_S")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_L_Hydrated"), new ServicePlanOffer("MSOnline", "BPOS_M_Hydrated")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_M_Hydrated"), new ServicePlanOffer("MSOnline", "BPOS_S_Hydrated")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_S"), new ServicePlanOffer("MSOnline", "BPOS_M")),
			Tuple.Create<ServicePlanOffer, ServicePlanOffer>(new ServicePlanOffer("MSOnline", "BPOS_S_Hydrated"), new ServicePlanOffer("MSOnline", "BPOS_M_Hydrated"))
		};

		private static class SyncRoot
		{
		}
	}
}

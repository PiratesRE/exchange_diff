using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	public class HybridConfigurationDetection : IHybridConfigurationDetection, IDisposable
	{
		public HybridConfigurationDetection(ILogger logger)
		{
			if (logger == null)
			{
				throw new HybridConfigurationDetectionException(Strings.NullLoggerHasBeenPassedIn, new ArgumentNullException());
			}
			this.lockObj = new object();
			this.logger = logger;
			this.onPremAcceptedDomainNameInOrgRelationship = new List<string>();
		}

		internal string[] DCAcceptedDomainName
		{
			get
			{
				string[] result;
				lock (this.lockObj)
				{
					result = this.dcAcceptedDomainName;
				}
				return result;
			}
			private set
			{
				lock (this.lockObj)
				{
					this.dcAcceptedDomainName = value;
				}
			}
		}

		internal bool DCIsUpgradingOrganization
		{
			get
			{
				bool result;
				lock (this.lockObj)
				{
					result = this.dcIsUpgradingOrganization;
				}
				return result;
			}
			private set
			{
				lock (this.lockObj)
				{
					this.dcIsUpgradingOrganization = value;
				}
			}
		}

		internal ExchangeObjectVersion DCPreviousAdminDisplayVersion
		{
			get
			{
				ExchangeObjectVersion result;
				lock (this.lockObj)
				{
					result = this.dcPreviousAdminDisplayVersion;
				}
				return result;
			}
			private set
			{
				lock (this.lockObj)
				{
					this.dcPreviousAdminDisplayVersion = value;
				}
			}
		}

		internal ExchangeObjectVersion DCAdminDisplayVersion
		{
			get
			{
				ExchangeObjectVersion result;
				lock (this.lockObj)
				{
					result = this.dcAdminDisplayVersion;
				}
				return result;
			}
			private set
			{
				lock (this.lockObj)
				{
					this.dcAdminDisplayVersion = value;
				}
			}
		}

		internal List<string> OnPremAcceptedDomainNameInOrgRelationship
		{
			get
			{
				List<string> result;
				lock (this.lockObj)
				{
					result = this.onPremAcceptedDomainNameInOrgRelationship;
				}
				return result;
			}
			private set
			{
				lock (this.lockObj)
				{
					this.onPremAcceptedDomainNameInOrgRelationship = value;
				}
			}
		}

		public bool RunTenantHybridTest(PSCredential psCredential, string organizationConfigHash)
		{
			string dataReadFromAttribute = string.Empty;
			this.logger.Log(Strings.RunningTenantHybridTest);
			if (psCredential == null && string.IsNullOrEmpty(organizationConfigHash))
			{
				throw new HybridConfigurationDetectionException(Strings.PSCredentialAndTheOrganizationHashIsNull);
			}
			if (!this.ShouldHybridTestRun())
			{
				return true;
			}
			ITenantHybridDetectionCmdlet tenantHybridDetectionCmdlet = new TenantHybridDetectionCmdlet();
			bool result;
			try
			{
				if (string.IsNullOrEmpty(organizationConfigHash))
				{
					this.logger.Log(Strings.ConnectingToTheDCToRunGetOrgConfig);
					organizationConfigHash = string.Empty;
					tenantHybridDetectionCmdlet.Connect(psCredential, null, this.logger);
					IEnumerable<OrganizationConfig> organizationConfig = tenantHybridDetectionCmdlet.GetOrganizationConfig();
					if (organizationConfig == null || !organizationConfig.Any<OrganizationConfig>())
					{
						this.logger.Log(Strings.OrgConfigDataIsEmptyOrNull);
						return false;
					}
					this.logger.Log(Strings.InflateAndDecodeReturnedDataFromGetOrgConfig);
					dataReadFromAttribute = this.InflateAndDecode(organizationConfig.ElementAt(0).OrganizationConfigHash);
				}
				else
				{
					this.hybridConfigurationEngineTest = true;
					this.logger.Log(Strings.InflatingAndDecodingPassedInHash);
					dataReadFromAttribute = this.InflateAndDecode(organizationConfigHash);
				}
				this.logger.Log(Strings.AttemptingToParseTheXmlData);
				if (!this.ParseXmlData(dataReadFromAttribute))
				{
					throw new HybridConfigurationDetectionException(Strings.DataReturnedFromDCIsInvalid);
				}
				result = this.TestDCSettingsForHybridEnabledTenant();
			}
			catch (CommandNotFoundException)
			{
				throw new HybridConfigurationDetectionException(Strings.GetOrganizationConfigCmdletNotFound);
			}
			catch (FileLoadException)
			{
				throw new HybridConfigurationDetectionException(Strings.GetOrganizationConfigCmdletNotFound);
			}
			catch (Exception ex)
			{
				if (ex is PSInvalidOperationException || ex is PSRemotingTransportException)
				{
					throw new HybridConfigurationDetectionException(Strings.InvalidPSCredential, ex);
				}
				if (ex.InnerException != null && ex.InnerException.Message.IndexOf("Get-OrganizationConfig", StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					throw new HybridConfigurationDetectionException(Strings.GetOrganizationConfigCmdletNotFound);
				}
				throw new HybridConfigurationDetectionException(new LocalizedString(ex.Message), ex);
			}
			finally
			{
				tenantHybridDetectionCmdlet.Dispose();
			}
			return result;
		}

		public bool RunTenantHybridTest(string pathToConfigFile)
		{
			this.logger.Log(Strings.RunningTentantHybridTestWithFile(pathToConfigFile));
			if (!this.ShouldHybridTestRun())
			{
				return true;
			}
			if (!this.TestDCConfigFile(pathToConfigFile))
			{
				throw new HybridConfigurationDetectionException(Strings.InvalidOrTamperedConfigFile(pathToConfigFile));
			}
			return this.TestDCSettingsForHybridEnabledTenant();
		}

		public bool RunOnPremisesHybridTest()
		{
			this.logger.Log(Strings.RunningOnPremTest);
			if (!this.ShouldHybridTestRun())
			{
				return false;
			}
			try
			{
				if (this.IsAdSchemaVersion15OrHigher())
				{
					return false;
				}
			}
			catch
			{
			}
			bool result;
			try
			{
				IOnPremisesHybridDetectionCmdlets onPremCmdlets = new OnPremisesHybridDetectionCmdlets();
				result = this.TestOnPremisesOrgRelationshipDomainsCrossWithAcceptedDomain(onPremCmdlets);
			}
			catch (Exception ex)
			{
				throw new HybridConfigurationDetectionException(Strings.OnPremisesTestFailedWithException(ex.Message), ex);
			}
			return result;
		}

		public void Dispose()
		{
		}

		internal bool TestDCSettingsForHybridEnabledTenant()
		{
			this.logger.Log(Strings.RunningTenantTestDCSettingsForHybridEnabledTenant);
			if (!this.dcIsDataCenter)
			{
				throw new HybridConfigurationDetectionException(Strings.GetOrgConfigWasRunOnPremises);
			}
			if (this.DCIsUpgradingOrganization && this.DCPreviousAdminDisplayVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
			{
				throw new HybridConfigurationDetectionException(Strings.TenantIsBeingUpgradedFromE14);
			}
			if (!this.DCAdminDisplayVersion.IsSameVersion(ExchangeObjectVersion.Exchange2012) && !this.DCAdminDisplayVersion.IsNewerThan(ExchangeObjectVersion.Exchange2012))
			{
				this.logger.Log(Strings.TestDCSettingsForHybridEnabledTenantFailed);
				throw new HybridConfigurationDetectionException(Strings.TenantHasNotYetBeenUpgradedToE15);
			}
			this.logger.Log(Strings.DisplayVersionDCBitUpgradeStatusBitAndVersionAreCorrect);
			if (this.hybridConfigurationEngineTest)
			{
				this.logger.Log(Strings.TenantIsRunningE15);
				return true;
			}
			this.logger.Log(Strings.CheckingTheAcceptedDomainAgainstOrgRelationshipDomains);
			if (this.OnPremAcceptedDomainNameInOrgRelationship.Count > 0 && this.dcAcceptedDomainName != null)
			{
				foreach (string value in this.OnPremAcceptedDomainNameInOrgRelationship)
				{
					if (this.dcAcceptedDomainName.Contains(value))
					{
						this.logger.Log(Strings.TenantIsRunningE15);
						return true;
					}
				}
				throw new HybridConfigurationDetectionException(Strings.NoMatchWasFoundBetweenTheOrgRelDomainsAndDCAcceptedDomains);
			}
			throw new HybridConfigurationDetectionException(Strings.EitherTheOnPremAcceptedDomainListOrTheDCAcceptedDomainsAreEmpty);
		}

		internal bool TestDCConfigFile(string pathToConfigFile)
		{
			bool result;
			try
			{
				this.logger.Log(Strings.TestingConfigFile(pathToConfigFile));
				if (!File.Exists(pathToConfigFile.ToString()))
				{
					throw new FileNotFoundException();
				}
				XmlTextReader xmlTextReader = new XmlTextReader(pathToConfigFile);
				bool flag = false;
				bool flag2 = false;
				List<string> list = new List<string>();
				string dataReadFromAttribute = string.Empty;
				while (!flag && !xmlTextReader.EOF && xmlTextReader.Read())
				{
					if (xmlTextReader.IsStartElement("S") && xmlTextReader.GetAttribute("N") == "OrganizationConfigHash")
					{
						flag = true;
						this.logger.Log(Strings.FoundOrgConfigHashInConfigFile);
						dataReadFromAttribute = this.InflateAndDecode(xmlTextReader.ReadString());
					}
					if (xmlTextReader.IsStartElement("Obj") && xmlTextReader.GetAttribute("N") == "ObjectClass")
					{
						flag2 = true;
						while (!xmlTextReader.IsStartElement("S"))
						{
							if (xmlTextReader.EOF)
							{
								break;
							}
							xmlTextReader.Read();
						}
						while (xmlTextReader.IsStartElement("S"))
						{
							list.Add(xmlTextReader.ReadString());
							xmlTextReader.ReadEndElement();
						}
					}
				}
				xmlTextReader.Close();
				if (flag)
				{
					if (!this.ParseXmlData(dataReadFromAttribute))
					{
						throw new HybridConfigurationDetectionException(Strings.DataReturnedFromDCIsInvalid);
					}
					result = flag;
				}
				else
				{
					if (flag2 && list.Contains("msExchConfigurationUnitContainer"))
					{
						throw new HybridConfigurationDetectionException(Strings.OrgConfigDataIsEmptyOrNull);
					}
					throw new HybridConfigurationDetectionException(Strings.TheFilePassedInIsNotFromGetOrganizationConfig);
				}
			}
			catch (HybridConfigurationDetectionException ex)
			{
				throw ex;
			}
			catch (FileNotFoundException)
			{
				throw new HybridConfigurationDetectionException(Strings.BadFilePassedIn);
			}
			catch (Exception exception)
			{
				throw new HybridConfigurationDetectionException(Strings.FailedResult, exception);
			}
			return result;
		}

		internal bool TestOnPremisesOrgRelationshipDomainsCrossWithAcceptedDomain(IOnPremisesHybridDetectionCmdlets onPremCmdlets)
		{
			string text = "outlook.com";
			bool result = false;
			new List<string>();
			IEnumerable<OrganizationRelationship> organizationRelationship = onPremCmdlets.GetOrganizationRelationship();
			IEnumerable<AcceptedDomain> acceptedDomain = onPremCmdlets.GetAcceptedDomain();
			if (organizationRelationship == null || acceptedDomain == null)
			{
				this.logger.LogInformation(Strings.EitherOrgRelOrAcceptDomainsWhereNull);
				return result;
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				if (registryKey != null)
				{
					text = (string)registryKey.GetValue("OrgRelationshipTargetAppUri");
					if (string.IsNullOrEmpty(text))
					{
						text = "outlook.com";
					}
				}
			}
			this.logger.LogInformation(Strings.OrgRelTargetAppUriToSearch(text));
			foreach (OrganizationRelationship organizationRelationship2 in organizationRelationship)
			{
				if (organizationRelationship2.TargetApplicationUri.ToString().ToLower().Contains(text))
				{
					foreach (SmtpDomain smtpDomain in organizationRelationship2.DomainNames)
					{
						foreach (AcceptedDomain acceptedDomain2 in acceptedDomain)
						{
							if (!string.IsNullOrEmpty(smtpDomain.Domain) && acceptedDomain2.DomainName != null && smtpDomain.Domain == acceptedDomain2.DomainName.Domain)
							{
								this.OnPremAcceptedDomainNameInOrgRelationship.Add(acceptedDomain2.DomainName.Domain);
								this.logger.LogInformation(Strings.DomainNameExistsInAcceptedDomainAndOrgRel(acceptedDomain2.DomainName.Domain));
								result = true;
							}
						}
					}
				}
			}
			this.logger.LogInformation(Strings.OnPremisesOrgRelationshipDomainsCrossWithAcceptedDomainReturnResult(result.ToString()));
			return result;
		}

		public bool IsAdSchemaVersion15OrHigher()
		{
			string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
			DirectoryContext context = new DirectoryContext(DirectoryContextType.Domain, domainName);
			DomainController domainController = DomainController.FindOne(context);
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainController.Name, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 657, "IsAdSchemaVersion15OrHigher", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Deployment\\HybridConfigurationDetection\\HybridConfigurationDetection.cs");
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 663, "IsAdSchemaVersion15OrHigher", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Deployment\\HybridConfigurationDetection\\HybridConfigurationDetection.cs");
			bool result;
			try
			{
				ADSchemaAttributeObject[] array = configurationSession.Find<ADSchemaAttributeObject>(topologyConfigurationSession.SchemaNamingContext, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ms-Exch-Schema-Version-Pt"), null, 1);
				if (array.Length != 0)
				{
					result = (array[0].RangeUpper >= 15137);
				}
				else
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				throw new HybridConfigurationDetectionException(new LocalizedString(ex.Message), ex);
			}
			return result;
		}

		internal bool ShouldHybridTestRun()
		{
			bool result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ExchangeServer\\v15\\Setup"))
			{
				if (registryKey != null && registryKey.GetValue("RunHybridDetection") != null)
				{
					this.logger.Log(Strings.TestNotRunDueToRegistryKey);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		internal bool ParseXmlData(string dataReadFromAttribute)
		{
			bool result;
			try
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				this.logger.LogInformation(Strings.ParsingXmlDataFromDCFileOrDCCmdlet);
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(dataReadFromAttribute)))
				{
					this.logger.Log(Strings.SearchingForAttributes);
					while (xmlReader.Read())
					{
						if (xmlReader.IsStartElement("IsDataCenter"))
						{
							this.dcIsDataCenter = Convert.ToBoolean(xmlReader.ReadString());
							this.logger.Log(Strings.DCIsDataCenterBitFound);
							flag4 = true;
						}
						else if (xmlReader.IsStartElement("AdminDisplayVersion"))
						{
							this.DCAdminDisplayVersion = ExchangeObjectVersion.Parse(xmlReader.ReadString());
							this.logger.Log(Strings.DCAdminDisplayVersionFound);
							flag = true;
						}
						else if (xmlReader.IsStartElement("PreviousAdminDisplayVersion"))
						{
							string text = xmlReader.ReadString();
							if (!string.IsNullOrEmpty(text))
							{
								this.DCPreviousAdminDisplayVersion = ExchangeObjectVersion.Parse(text);
							}
							else
							{
								this.DCPreviousAdminDisplayVersion = ExchangeObjectVersion.Exchange2010;
							}
							this.logger.Log(Strings.DCPreviousAdminDisplayVersionFound);
							flag5 = true;
						}
						else if (xmlReader.IsStartElement("IsUpgradingOrganization"))
						{
							this.DCIsUpgradingOrganization = Convert.ToBoolean(xmlReader.ReadString());
							this.logger.Log(Strings.DCIsUpgradingOrganizationFound);
							flag2 = true;
						}
						else if (xmlReader.IsStartElement("AcceptedDomain"))
						{
							List<string> list = new List<string>();
							while (xmlReader.Read())
							{
								if (xmlReader.IsStartElement("DomainName"))
								{
									list.Add(xmlReader.ReadString());
								}
							}
							this.DCAcceptedDomainName = list.ToArray();
							if (this.dcAcceptedDomainName.Length > 0)
							{
								flag3 = true;
								this.logger.Log(Strings.DCAcceptedDomainNameFound);
							}
						}
					}
				}
				if (flag2 && this.dcIsUpgradingOrganization)
				{
					result = (flag3 && flag2 && flag && flag4 && flag5);
				}
				else
				{
					result = (flag3 && flag2 && flag && flag4);
				}
			}
			catch (Exception exception)
			{
				throw new HybridConfigurationDetectionException(Strings.FailedResult, exception);
			}
			return result;
		}

		internal string InflateAndDecode(string input)
		{
			byte[] buffer = Convert.FromBase64String(input);
			this.logger.Log(Strings.InflateAndDecoding);
			string result;
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
				{
					using (StreamReader streamReader = new StreamReader(deflateStream, Encoding.UTF8))
					{
						result = streamReader.ReadToEnd();
					}
				}
			}
			return result;
		}

		private const string OrganizationConfigHashAttribute = "OrganizationConfigHash";

		private const string GetOrganizationConfig = "Get-OrganizationConfig";

		private const string ObjectClass = "ObjectClass";

		private const string AcceptedDomainNameAttribute = "AcceptedDomain";

		private const string DomainNameAttribute = "DomainName";

		private const string IsDataCenterAttribute = "IsDataCenter";

		private const string IsUpgradingOrganizationAttribute = "IsUpgradingOrganization";

		private const string AdminDisplayVersionAttribute = "AdminDisplayVersion";

		private const string PreviousAdminDisplayVersionAttribute = "PreviousAdminDisplayVersion";

		private const string RunHybridRegkeyPath = "Software\\Microsoft\\ExchangeServer\\v15\\Setup";

		private const string RunHybridRegkeyName = "RunHybridDetection";

		private const string OrgRelationshipTargetAppUri = "OrgRelationshipTargetAppUri";

		private const int Exchange15RtmADSchemaVersion = 15137;

		private ILogger logger;

		private string[] dcAcceptedDomainName;

		private bool dcIsUpgradingOrganization;

		private bool dcIsDataCenter;

		private bool hybridConfigurationEngineTest;

		private ExchangeObjectVersion dcAdminDisplayVersion;

		private ExchangeObjectVersion dcPreviousAdminDisplayVersion;

		private List<string> onPremAcceptedDomainNameInOrgRelationship;

		private object lockObj;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess
{
	internal class AutoPopulateDefinition
	{
		internal AutoPopulateDefinition(ProbeType probeType, ProbeDefinition definition)
		{
			this.probeType = probeType;
			this.probeDefinition = definition;
			this.targetResource = definition.TargetResource;
		}

		private MailboxDatabaseInfo MailboxDatabaseInfo
		{
			get
			{
				if (this.mailboxDatabaseInfo == null)
				{
					ICollection<MailboxDatabaseInfo> source;
					if (this.IsProbeOnCafe)
					{
						source = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
					}
					else
					{
						source = LocalEndpointManager.Instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
					}
					if (!string.IsNullOrEmpty(this.targetResource))
					{
						this.mailboxDatabaseInfo = (from dbInfo in source
						where dbInfo.MailboxDatabaseName == this.targetResource
						select dbInfo).First<MailboxDatabaseInfo>();
					}
					else
					{
						this.mailboxDatabaseInfo = (from dbInfo in source
						where !string.IsNullOrEmpty(dbInfo.MonitoringAccount)
						select dbInfo into db
						orderby db.MonitoringAccount
						select db).First<MailboxDatabaseInfo>();
					}
				}
				return this.mailboxDatabaseInfo;
			}
		}

		private bool IsProbeOnCafe
		{
			get
			{
				return this.probeType == ProbeType.Ctp;
			}
		}

		private bool IsPasswordRequired
		{
			get
			{
				return this.IsProbeOnCafe || VariantConfiguration.InvariantNoFlightingSnapshot.Global.DistributedKeyManagement.Enabled;
			}
		}

		internal void ValidateAndAutoFill(Dictionary<string, string> overridesPropertyBag)
		{
			if (overridesPropertyBag.ContainsKey("Endpoint"))
			{
				this.probeDefinition.Endpoint = overridesPropertyBag["Endpoint"];
			}
			else
			{
				this.probeDefinition.Endpoint = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;
			}
			if (overridesPropertyBag.ContainsKey("ExtensionAttributes"))
			{
				Dictionary<string, string> dictionary = DefinitionHelperBase.ConvertExtensionAttributesToDictionary(overridesPropertyBag["ExtensionAttributes"]);
				if (dictionary != null)
				{
					foreach (string key in AutoPopulateDefinition.ExtensionAttributeNames)
					{
						if (dictionary.ContainsKey(key))
						{
							this.probeDefinition.Attributes[key] = dictionary[key];
						}
					}
				}
			}
			if (overridesPropertyBag.ContainsKey("Account"))
			{
				string text = overridesPropertyBag["Account"];
				if (!this.IsPasswordRequired)
				{
					this.probeDefinition.SetAccountCommonAccessToken(text);
				}
				else
				{
					this.probeDefinition.Account = text;
				}
				if (overridesPropertyBag.ContainsKey("Password"))
				{
					this.probeDefinition.AccountPassword = overridesPropertyBag["Password"];
				}
				if (this.IsPasswordRequired)
				{
					if (!overridesPropertyBag.ContainsKey("Password"))
					{
						throw new LocalizedException(Strings.InputPasswordRequired);
					}
				}
				else if (overridesPropertyBag.ContainsKey("Password"))
				{
					throw new LocalizedException(Strings.InputPasswordNotRequired);
				}
				if (overridesPropertyBag.ContainsKey("AccountDisplayName"))
				{
					this.probeDefinition.AccountDisplayName = overridesPropertyBag["AccountDisplayName"];
				}
			}
			else
			{
				if (!this.probeDefinition.Attributes.ContainsKey("AccountLegacyDN"))
				{
					this.probeDefinition.Attributes.Add("AccountLegacyDN", this.MailboxDatabaseInfo.MonitoringAccountLegacyDN);
				}
				if (!this.probeDefinition.Attributes.ContainsKey("PersonalizedServerName"))
				{
					this.probeDefinition.Attributes.Add("PersonalizedServerName", string.Format("{0}@{1}", this.MailboxDatabaseInfo.MonitoringAccountMailboxGuid, this.MailboxDatabaseInfo.MonitoringAccountDomain));
				}
				if (!this.probeDefinition.Attributes.ContainsKey("AccountDisplayName"))
				{
					this.probeDefinition.Attributes.Add("AccountDisplayName", this.MailboxDatabaseInfo.MonitoringAccount);
				}
				if (this.IsPasswordRequired)
				{
					this.probeDefinition.AuthenticateAsUser(this.MailboxDatabaseInfo);
				}
				else
				{
					this.probeDefinition.AuthenticateAsCafeServer(this.MailboxDatabaseInfo);
				}
			}
			if (overridesPropertyBag.ContainsKey("SecondaryEndpoint"))
			{
				this.probeDefinition.SecondaryEndpoint = overridesPropertyBag["SecondaryEndpoint"];
			}
			else if (!this.IsProbeOnCafe)
			{
				this.probeDefinition.SecondaryEndpoint = this.probeDefinition.Endpoint;
			}
			else if (this.probeDefinition.Attributes.ContainsKey("PersonalizedServerName"))
			{
				this.probeDefinition.SecondaryEndpoint = this.probeDefinition.Attributes["PersonalizedServerName"];
			}
			if (overridesPropertyBag.ContainsKey("TimeoutSeconds"))
			{
				int timeoutSeconds;
				if (int.TryParse(overridesPropertyBag["TimeoutSeconds"], out timeoutSeconds))
				{
					this.probeDefinition.TimeoutSeconds = timeoutSeconds;
					return;
				}
			}
			else
			{
				this.probeDefinition.MakeTemplateForOnDemandExecution();
				this.probeDefinition.Enabled = true;
			}
		}

		private static string GetSerializedAccessToken(string accountName)
		{
			string result;
			using (WindowsIdentity windowsIdentity = new WindowsIdentity(accountName))
			{
				using (ClientSecurityContext clientSecurityContext = windowsIdentity.CreateClientSecurityContext(true))
				{
					SerializedAccessToken serializedAccessToken = new SerializedAccessToken(windowsIdentity.GetSafeName(true), windowsIdentity.AuthenticationType, clientSecurityContext);
					result = serializedAccessToken.ToString();
				}
			}
			return result;
		}

		private readonly string targetResource;

		private MailboxDatabaseInfo mailboxDatabaseInfo;

		private ProbeType probeType;

		private ProbeDefinition probeDefinition;

		private static string[] ExtensionAttributeNames = new string[]
		{
			"AccountDisplayName",
			"AccountLegacyDN",
			"MailboxLegacyDN",
			"PersonalizedServerName",
			"RpcProxyPort",
			"RpcProxyAuthenticationType",
			"RpcAuthenticationType"
		};
	}
}

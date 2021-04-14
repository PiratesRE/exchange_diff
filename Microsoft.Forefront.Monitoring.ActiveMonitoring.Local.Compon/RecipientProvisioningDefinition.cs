using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class RecipientProvisioningDefinition
	{
		public RecipientProvisioningDefinition(string extensionXml)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(extensionXml);
			this.ExtensionNode = xmlDocument.DocumentElement;
		}

		public TimeSpan AllowableLatency
		{
			get
			{
				return TimeSpan.Parse(this.GetRecipientAttribute("allowableLatency", false));
			}
		}

		public string RecipientType
		{
			get
			{
				return this.GetRecipientAttribute("recipientType", false).ToLower();
			}
		}

		public string NamePrefix
		{
			get
			{
				return this.GetRecipientAttribute("namePrefix", false);
			}
		}

		public bool GenerateUniqueUser
		{
			get
			{
				return this.GetRecipientAttribute("generateUniqueUser", true).ToLower() != "false";
			}
		}

		public string RunAsUser
		{
			get
			{
				return (string)this.Users.First((ADUser user) => Regex.IsMatch((string)user.WindowsLiveID, "admin", RegexOptions.IgnoreCase)).WindowsLiveID;
			}
		}

		public string RunAsUserPassword
		{
			get
			{
				SecureString loginPassword = this.ProbeOrganization.LoginPassword;
				if (loginPassword == null)
				{
					throw new ArgumentException("The probe organization does not have a password specified.");
				}
				IntPtr ptr = Marshal.SecureStringToBSTR(loginPassword);
				string result;
				try
				{
					result = Marshal.PtrToStringBSTR(ptr);
				}
				finally
				{
					Marshal.FreeBSTR(ptr);
				}
				return result;
			}
		}

		public bool CleanupRecipient
		{
			get
			{
				return this.GetRecipientAttribute("cleanupRecipient", true).ToLower() != "false";
			}
		}

		public bool AddLicense
		{
			get
			{
				bool result;
				bool.TryParse(this.GetRecipientAttribute("addLicense", true) ?? "false", out result);
				return result;
			}
		}

		public ITenantRecipientSession ProbeSession
		{
			get
			{
				if (this.probeSession == null)
				{
					if (this.ProbeOrganization == null)
					{
						throw new InvalidOperationException("The probe organization should always have a value.");
					}
					this.probeSession = new FfoTenantRecipientSession(this.ProbeOrganization.ProbeOrganizationId);
				}
				return this.probeSession;
			}
		}

		public string Endpoint
		{
			get
			{
				switch (ProbeOrganizationInfo.FindEnvironment(this.RunAsUser))
				{
				case ProbeEnvironment.Test:
					return "https://provisioningapi.msol-test.com/provisioningwebservice.svc";
				case ProbeEnvironment.Dogfood:
					return "https://provisioningapi.ccsctp.com/provisioningwebservice.svc";
				case ProbeEnvironment.Production:
					return "https://provisioningapi.microsoftonline.com/provisioningwebservice.svc";
				default:
					throw new ArgumentException("The windows live Id for the administrator does not have an expected suffix (e.g. @msol-test.com).");
				}
			}
		}

		public ProbeOrganizationInfo ProbeOrganization
		{
			get
			{
				if (this.probeOrganization == null)
				{
					string recipientAttribute = this.GetRecipientAttribute("featureTag", false);
					GlobalConfigSession globalConfigSession = new GlobalConfigSession();
					IEnumerable<ProbeOrganizationInfo> probeOrganizations = globalConfigSession.GetProbeOrganizations(recipientAttribute);
					if (probeOrganizations == null || !probeOrganizations.Any<ProbeOrganizationInfo>())
					{
						throw new ArgumentException("Cannot find any test tenant with feature tag = " + recipientAttribute);
					}
					this.probeOrganization = probeOrganizations.First<ProbeOrganizationInfo>();
				}
				return this.probeOrganization;
			}
		}

		private XmlNode ExtensionNode { get; set; }

		private IEnumerable<ADUser> Users
		{
			get
			{
				if (this.users == null)
				{
					this.users = this.ProbeSession.Find<ADUser>(null, QueryScope.SubTree, null, null, int.MaxValue);
				}
				return this.users;
			}
		}

		private XmlNode RecipientNode
		{
			get
			{
				if (this.recipientNode == null)
				{
					this.recipientNode = this.ExtensionNode.SelectSingleNode("WorkContext/Recipient");
				}
				return this.recipientNode;
			}
		}

		private string GetRecipientAttribute(string attributeName, bool optional = false)
		{
			XmlAttribute xmlAttribute = this.RecipientNode.Attributes[attributeName];
			if (!optional && xmlAttribute == null)
			{
				throw new ArgumentException(string.Format("Attribute cannot be found for name {0}.", attributeName));
			}
			if (xmlAttribute != null)
			{
				return xmlAttribute.Value;
			}
			return string.Empty;
		}

		private const string AllowableLatencyAttribute = "allowableLatency";

		private const string RecipientTypeAttribute = "recipientType";

		private const string NamePrefixAttribute = "namePrefix";

		private const string GenerateUniqueUserAttribute = "generateUniqueUser";

		private const string CleanUpRecipientAttribute = "cleanupRecipient";

		private const string AddLicenseAttribute = "addLicense";

		private const string TestEnvironmentProvisioningEndopint = "https://provisioningapi.msol-test.com/provisioningwebservice.svc";

		private const string DogfoodEnvironmentProvisioningEndpoint = "https://provisioningapi.ccsctp.com/provisioningwebservice.svc";

		private const string ProductionEnvironmentProvisioningEndpoint = "https://provisioningapi.microsoftonline.com/provisioningwebservice.svc";

		private XmlNode recipientNode;

		private ProbeOrganizationInfo probeOrganization;

		private ITenantRecipientSession probeSession;

		private IEnumerable<ADUser> users;
	}
}

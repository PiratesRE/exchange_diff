using System;
using System.Xml;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	internal class OrganizationInitializationDefinition
	{
		public OrganizationInitializationDefinition(string extensionXml)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(extensionXml);
			this.ExtensionNode = xmlDocument.DocumentElement;
		}

		public string DomainPrefix
		{
			get
			{
				return this.GetOrganizationAttribute("DomainPrefix", true) ?? Guid.NewGuid().ToString().Replace("-", string.Empty);
			}
		}

		public int TimeoutInMinutes
		{
			get
			{
				int result;
				int.TryParse(this.GetOrganizationAttribute("TimeoutWaitInMinutes", true) ?? "7", out result);
				return result;
			}
		}

		public bool WaitForEXOProperties
		{
			get
			{
				bool result;
				bool.TryParse(this.GetOrganizationAttribute("WaitForEXOProperties", true) ?? "true", out result);
				return result;
			}
		}

		public bool WaitForDeprovisioning
		{
			get
			{
				bool result;
				bool.TryParse(this.GetOrganizationAttribute("WaitForDeprovisioning", true) ?? "true", out result);
				return result;
			}
		}

		public string CustomerType
		{
			get
			{
				return this.GetOrganizationAttribute("CustomerType", true) ?? "FilteringOnly";
			}
		}

		public string FeatureTag
		{
			get
			{
				return this.GetOrganizationAttribute("FeatureTag", false);
			}
		}

		public string Version
		{
			get
			{
				return this.GetOrganizationAttribute("Version", true);
			}
		}

		public Guid TenantId
		{
			get
			{
				string input = this.GetOrganizationAttribute("TenantId", true) ?? Guid.Empty.ToString();
				Guid result;
				Guid.TryParse(input, out result);
				return result;
			}
		}

		public CompanyManagerProvider CompanyManager
		{
			get
			{
				if (this.companyManager == null)
				{
					this.companyManager = new CompanyManagerProvider
					{
						Proxy = null,
						Timeout = (int)TimeSpan.FromMinutes(3.0).TotalMilliseconds
					};
				}
				return this.companyManager;
			}
		}

		private XmlNode ExtensionNode { get; set; }

		private XmlNode OrganizationNode
		{
			get
			{
				return this.ExtensionNode.SelectSingleNode("WorkContext/Organization");
			}
		}

		private string GetOrganizationAttribute(string attributeName, bool optional = false)
		{
			XmlAttribute xmlAttribute = this.OrganizationNode.Attributes[attributeName];
			if (!optional && xmlAttribute == null)
			{
				throw new ArgumentException(string.Format("Attribute cannot be found for name {0}.", attributeName));
			}
			if (xmlAttribute != null)
			{
				return xmlAttribute.Value;
			}
			return null;
		}

		private const string FeatureTagAttribute = "FeatureTag";

		private const string DomainPrefixAttribute = "DomainPrefix";

		private const string CustomerTypeAttribute = "CustomerType";

		private const string VersionAttribute = "Version";

		private const string TimeoutWaitInMinutesAttribute = "TimeoutWaitInMinutes";

		private const string WaitForEXOPropertiesAttribute = "WaitForEXOProperties";

		private const string WaitForDeprovisioningAttribute = "WaitForDeprovisioning";

		private const string TenantIdAttribute = "TenantId";

		private CompanyManagerProvider companyManager;
	}
}

using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "TeamMailboxProvisioningPolicyConfig")]
	[Serializable]
	public sealed class TeamMailboxProvisioningPolicyConfigXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "AliasPrefix")]
		public string AliasPrefix { get; set; }

		[XmlElement(ElementName = "DefaultAliasPrefixEnabled")]
		public bool DefaultAliasPrefixEnabled { get; set; }
	}
}

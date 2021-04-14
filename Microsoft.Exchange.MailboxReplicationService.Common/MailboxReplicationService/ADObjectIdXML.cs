using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class ADObjectIdXML : PropertyValueBaseXML
	{
		[XmlElement(ElementName = "ObjectGuid")]
		public Guid ObjectGuid { get; set; }

		[XmlElement(ElementName = "DistinguishedName")]
		public string DistinguishedName { get; set; }

		[XmlElement(ElementName = "PartitionGuid")]
		public Guid PartitionGuid { get; set; }

		[XmlElement(ElementName = "PartitionFqdn")]
		public string PartitionFqdn { get; set; }

		internal override object RawValue
		{
			get
			{
				return ADObjectIdXML.Deserialize(this);
			}
		}

		public override string ToString()
		{
			return string.Format("{0} ({1},{2})", this.DistinguishedName, this.ObjectGuid, this.PartitionGuid);
		}

		internal static ADObjectIdXML Serialize(ADObjectId id)
		{
			if (id == null)
			{
				return null;
			}
			return new ADObjectIdXML
			{
				ObjectGuid = id.ObjectGuid,
				PartitionGuid = id.PartitionGuid,
				DistinguishedName = id.DistinguishedName,
				PartitionFqdn = id.PartitionFQDN
			};
		}

		internal static ADObjectId Deserialize(ADObjectIdXML value)
		{
			if (value == null)
			{
				return null;
			}
			return new ADObjectId(value.DistinguishedName, value.PartitionFqdn, value.ObjectGuid);
		}

		internal override bool TryGetValue(ProviderPropertyDefinition pdef, out object result)
		{
			result = ADObjectIdXML.Deserialize(this);
			return true;
		}

		internal override bool HasValue()
		{
			return this.ObjectGuid != Guid.Empty || !string.IsNullOrEmpty(this.DistinguishedName);
		}
	}
}

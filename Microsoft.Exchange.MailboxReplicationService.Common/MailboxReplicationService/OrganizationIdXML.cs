using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class OrganizationIdXML : PropertyValueBaseXML
	{
		[XmlElement(ElementName = "OrganizationalUnit")]
		public ADObjectIdXML OrganizationalUnit { get; set; }

		[XmlElement(ElementName = "ConfigurationUnit")]
		public ADObjectIdXML ConfigurationUnit { get; set; }

		internal override object RawValue
		{
			get
			{
				return OrganizationIdXML.Deserialize(this);
			}
		}

		public override string ToString()
		{
			return string.Format("{0}", this.OrganizationalUnit);
		}

		internal static OrganizationIdXML Serialize(OrganizationId id)
		{
			if (id == null)
			{
				return null;
			}
			return new OrganizationIdXML
			{
				OrganizationalUnit = ADObjectIdXML.Serialize(id.OrganizationalUnit),
				ConfigurationUnit = ADObjectIdXML.Serialize(id.ConfigurationUnit)
			};
		}

		internal static OrganizationId Deserialize(OrganizationIdXML value)
		{
			if (value == null)
			{
				return null;
			}
			return OrganizationIdXML.OrganizationIdGetter(ADObjectIdXML.Deserialize(value.OrganizationalUnit), ADObjectIdXML.Deserialize(value.ConfigurationUnit));
		}

		internal override bool TryGetValue(ProviderPropertyDefinition pdef, out object result)
		{
			result = OrganizationIdXML.Deserialize(this);
			return true;
		}

		internal override bool HasValue()
		{
			return this.OrganizationalUnit != null || this.ConfigurationUnit != null;
		}

		private static OrganizationId OrganizationIdGetter(ADObjectId orgUnit, ADObjectId configUnit)
		{
			if (orgUnit == null || configUnit == null)
			{
				return OrganizationId.ForestWideOrgId;
			}
			return new OrganizationId(orgUnit, configUnit);
		}
	}
}

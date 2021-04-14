using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class DomainSettings : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return base.Id;
			}
		}

		public ADObjectId HygieneConfigurationLinkProp
		{
			get
			{
				return (ADObjectId)this[DomainSettingsSchema.HygieneConfigurationLinkProp];
			}
			set
			{
				this[DomainSettingsSchema.HygieneConfigurationLinkProp] = value;
			}
		}

		public EdgeBlockMode EdgeBlockMode
		{
			get
			{
				return (EdgeBlockMode)this[DomainSettingsSchema.EdgeBlockModeProp];
			}
			set
			{
				this[DomainSettingsSchema.EdgeBlockModeProp] = value;
			}
		}

		public string MailServer
		{
			get
			{
				return (string)this[DomainSettingsSchema.MailServerProp];
			}
			set
			{
				this[DomainSettingsSchema.MailServerProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return DomainSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DomainSettings.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool ShouldValidatePropertyLinkInSameOrganization(ADPropertyDefinition property)
		{
			return false;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
		}

		private static readonly DomainSettingsSchema schema = ObjectSchema.GetInstance<DomainSettingsSchema>();

		private static string mostDerivedClass = "DomainSettings";
	}
}

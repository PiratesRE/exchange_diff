using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADRoutingInfo : ADObject
	{
		public ADRoutingInfo()
		{
		}

		internal ADRoutingInfo(IConfigurationSession session, string tenantId)
		{
			this.m_Session = session;
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal ADRoutingInfo(string tenantId)
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public override ObjectId Identity
		{
			get
			{
				return this.InfoId;
			}
		}

		public ADObjectId InfoId
		{
			get
			{
				return (ADObjectId)this[ADRoutingInfoSchema.IdProp];
			}
			set
			{
				this[ADRoutingInfoSchema.IdProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADRoutingInfo.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADRoutingInfo.mostDerivedClass;
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

		private static readonly ADRoutingInfoSchema schema = ObjectSchema.GetInstance<ADRoutingInfoSchema>();

		private static string mostDerivedClass = "ADRoutingInfo";
	}
}

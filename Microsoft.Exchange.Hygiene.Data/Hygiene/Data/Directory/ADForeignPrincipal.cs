using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class ADForeignPrincipal : ADObject
	{
		internal ADObjectId ForeignPrincipalId
		{
			get
			{
				return this[ADForeignPrincipalSchema.ForeignPrincipalIdProperty] as ADObjectId;
			}
			set
			{
				this[ADForeignPrincipalSchema.ForeignPrincipalIdProperty] = value;
			}
		}

		internal ADObjectId ForeignContextId
		{
			get
			{
				return this[ADForeignPrincipalSchema.ForeignContextIdProperty] as ADObjectId;
			}
			set
			{
				this[ADForeignPrincipalSchema.ForeignContextIdProperty] = value;
			}
		}

		internal string DisplayName
		{
			get
			{
				return this[ADForeignPrincipalSchema.DisplayNameProperty] as string;
			}
			set
			{
				this[ADForeignPrincipalSchema.DisplayNameProperty] = value;
			}
		}

		internal string Description
		{
			get
			{
				return this[ADForeignPrincipalSchema.DescriptionProperty] as string;
			}
			set
			{
				this[ADForeignPrincipalSchema.DescriptionProperty] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADForeignPrincipal.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADForeignPrincipal.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly ADForeignPrincipalSchema schema = ObjectSchema.GetInstance<ADForeignPrincipalSchema>();

		private static string mostDerivedClass = "ADForeignPrincipal";
	}
}

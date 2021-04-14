using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class ADAccount : ADObject
	{
		internal string DisplayName
		{
			get
			{
				return this[ADAccountSchema.DisplayNameProperty] as string;
			}
			set
			{
				this[ADAccountSchema.DisplayNameProperty] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADAccount.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADAccount.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly ADAccountSchema schema = ObjectSchema.GetInstance<ADAccountSchema>();

		private static string mostDerivedClass = "ADAccount";
	}
}

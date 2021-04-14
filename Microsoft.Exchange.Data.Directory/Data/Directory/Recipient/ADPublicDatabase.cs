using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	internal sealed class ADPublicDatabase : ADRecipient
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADPublicDatabase.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADPublicDatabase.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal ADPublicDatabase(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		public ADPublicDatabase()
		{
		}

		private static readonly ADPublicDatabaseSchema schema = ObjectSchema.GetInstance<ADPublicDatabaseSchema>();

		internal static string MostDerivedClass = "msExchPublicMdb";
	}
}

using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class ADComputerRecipient : ADUser
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADComputerRecipient.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADComputerRecipient.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return ADComputerRecipient.ImplicitFilterInternal;
			}
		}

		internal ADComputerRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		public ADComputerRecipient()
		{
		}

		private static readonly ADComputerRecipientSchema schema = ObjectSchema.GetInstance<ADComputerRecipientSchema>();

		internal new static string MostDerivedClass = "computer";

		private static string objectCategoryName = "computer";

		internal new static QueryFilter ImplicitFilterInternal = new AndFilter(new QueryFilter[]
		{
			ADObject.ObjectClassFilter(ADComputerRecipient.MostDerivedClass),
			ADObject.ObjectCategoryFilter(ADComputerRecipient.objectCategoryName)
		});
	}
}

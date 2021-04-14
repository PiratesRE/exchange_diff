using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class PredicateExtendedProperty : ConfigurablePropertyTable
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public Guid ID
		{
			get
			{
				return (Guid)this[PredicateExtendedProperty.IDProperty];
			}
			set
			{
				this[PredicateExtendedProperty.IDProperty] = value;
			}
		}

		public Guid PredicateID
		{
			get
			{
				return (Guid)this[PredicateExtendedProperty.PredicateIDProperty];
			}
			set
			{
				this[PredicateExtendedProperty.PredicateIDProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition IDProperty = new HygienePropertyDefinition("id_RuleId", typeof(Guid));

		public static readonly HygienePropertyDefinition PredicateIDProperty = new HygienePropertyDefinition("id_PredicateId", typeof(Guid));
	}
}

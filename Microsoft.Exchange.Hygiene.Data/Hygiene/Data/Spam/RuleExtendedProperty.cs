using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class RuleExtendedProperty : ConfigurablePropertyTable
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ID.ToString());
			}
		}

		public Guid ID
		{
			get
			{
				return (Guid)this[RuleExtendedProperty.IDProperty];
			}
			set
			{
				this[RuleExtendedProperty.IDProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition IDProperty = new HygienePropertyDefinition("id_RuleId", typeof(Guid));
	}
}

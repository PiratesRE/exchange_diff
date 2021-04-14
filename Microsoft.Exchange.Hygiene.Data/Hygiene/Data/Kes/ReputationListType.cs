using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	internal class ReputationListType : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ReputationListTypeID.ToString());
			}
		}

		public byte ReputationListTypeID
		{
			get
			{
				return (byte)this[ReputationListType.ReputationListTypeIDProperty];
			}
			set
			{
				this[ReputationListType.ReputationListTypeIDProperty] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[ReputationListType.NameProperty];
			}
			set
			{
				this[ReputationListType.NameProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition ReputationListTypeIDProperty = new HygienePropertyDefinition("ti_ReputationListTypeId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition NameProperty = new HygienePropertyDefinition("nvc_Description", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

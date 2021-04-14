using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	internal class ReputationList : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ReputationListID.ToString());
			}
		}

		public byte ReputationListID
		{
			get
			{
				return (byte)this[ReputationList.ReputationListIDProperty];
			}
			set
			{
				this[ReputationList.ReputationListIDProperty] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[ReputationList.NameProperty];
			}
			set
			{
				this[ReputationList.NameProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition ReputationListIDProperty = new HygienePropertyDefinition("ti_ReputationListId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition NameProperty = new HygienePropertyDefinition("nvc_Description", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}

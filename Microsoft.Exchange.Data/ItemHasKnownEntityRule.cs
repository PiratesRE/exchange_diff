using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ItemHasKnownEntityRule : ActivationRule
	{
		public ItemHasKnownEntityRule(KnownEntityType entityType, string filterName, string regExFilter, bool ignoreCase) : base("ItemHasKnownEntity")
		{
			this.EntityType = entityType;
			this.FilterName = filterName;
			this.RegExFilter = regExFilter;
			this.IgnoreCase = ignoreCase;
		}

		[DataMember]
		public KnownEntityType EntityType { get; set; }

		[DataMember]
		public string FilterName { get; set; }

		[DataMember]
		public string RegExFilter { get; set; }

		[DataMember]
		public bool IgnoreCase { get; set; }
	}
}

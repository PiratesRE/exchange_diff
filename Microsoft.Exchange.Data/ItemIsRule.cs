using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ItemIsRule : ActivationRule
	{
		public ItemIsRule(ItemIsRuleItemType itemType, string itemClass, bool includeSubClasses = false, ItemIsRuleFormType formType = ItemIsRuleFormType.Read) : base("ItemIs")
		{
			this.ItemClass = itemClass;
			this.IncludeSubClasses = includeSubClasses;
			this.ItemType = itemType;
			this.FormType = formType;
		}

		[DataMember]
		public ItemIsRuleItemType ItemType { get; set; }

		[DataMember]
		public ItemIsRuleFormType FormType { get; set; }

		[DataMember]
		public string ItemClass { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IncludeSubClasses { get; set; }
	}
}

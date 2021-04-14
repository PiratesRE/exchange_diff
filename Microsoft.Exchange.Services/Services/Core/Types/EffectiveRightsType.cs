using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EffectiveRightsType
	{
		[DataMember(Order = 1)]
		public bool CreateAssociated { get; set; }

		[DataMember(Order = 2)]
		public bool CreateContents { get; set; }

		[DataMember(Order = 3)]
		public bool CreateHierarchy { get; set; }

		[DataMember(Order = 4)]
		public bool Delete { get; set; }

		[DataMember(Order = 5)]
		public bool Modify { get; set; }

		[DataMember(Order = 6)]
		public bool Read { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 7)]
		public bool? ViewPrivateItems
		{
			get
			{
				if (this.ViewPrivateItemsSpecified)
				{
					return new bool?(this.viewPrivateItems);
				}
				return null;
			}
			set
			{
				this.ViewPrivateItemsSpecified = (value != null);
				if (value != null)
				{
					this.viewPrivateItems = value.Value;
				}
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool ViewPrivateItemsSpecified { get; set; }

		private bool viewPrivateItems;
	}
}

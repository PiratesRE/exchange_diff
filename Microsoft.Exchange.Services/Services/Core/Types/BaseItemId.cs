using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(OccurrenceItemId))]
	[XmlInclude(typeof(RecurringMasterItemId))]
	[XmlInclude(typeof(RecurringMasterItemIdRanges))]
	[KnownType(typeof(RecurringMasterItemId))]
	[KnownType(typeof(RecurringMasterItemIdRanges))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(ItemId))]
	[XmlInclude(typeof(RootItemIdType))]
	[XmlType(TypeName = "BaseItemIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(ItemId))]
	[KnownType(typeof(OccurrenceItemId))]
	[KnownType(typeof(RootItemIdType))]
	public class BaseItemId : ServiceObjectId
	{
		internal override BasicTypes BasicType
		{
			get
			{
				return BasicTypes.ItemOrAttachment;
			}
		}

		public BaseItemId()
		{
			if (base.GetType() == typeof(BaseItemId))
			{
				throw new NotImplementedException("Don't call me!");
			}
		}
	}
}

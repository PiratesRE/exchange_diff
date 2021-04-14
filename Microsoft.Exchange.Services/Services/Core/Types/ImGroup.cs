using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ImGroupType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ImGroup")]
	[Serializable]
	public class ImGroup
	{
		[DataMember(Order = 1)]
		[XmlElement]
		public string DisplayName { get; set; }

		[XmlElement]
		[DataMember(Order = 2)]
		public string GroupType { get; set; }

		[XmlElement]
		[DataMember(Order = 3)]
		public ItemId ExchangeStoreId { get; set; }

		[DataMember(Order = 4)]
		[XmlArray]
		[XmlArrayItem("ItemId", typeof(ItemId))]
		public ItemId[] MemberCorrelationKey { get; set; }

		[XmlArrayItem("ExtendedProperty", typeof(ExtendedPropertyType))]
		[XmlArray]
		[DataMember(Order = 5)]
		public ExtendedPropertyType[] ExtendedProperties { get; set; }

		[DataMember(Order = 6)]
		[XmlElement]
		public string SmtpAddress { get; set; }

		internal static ImGroup LoadFromRawImGroup(RawImGroup rawImGroup, MailboxSession session)
		{
			ImGroup imGroup = new ImGroup();
			imGroup.DisplayName = rawImGroup.DisplayName;
			imGroup.GroupType = rawImGroup.GroupType;
			imGroup.ExchangeStoreId = IdConverter.ConvertStoreItemIdToItemId(rawImGroup.ExchangeStoreId, session);
			if (rawImGroup.MemberCorrelationKey.Length > 0)
			{
				List<ItemId> list = new List<ItemId>(rawImGroup.MemberCorrelationKey.Length);
				foreach (StoreObjectId storeItemId in rawImGroup.MemberCorrelationKey)
				{
					list.Add(IdConverter.ConvertStoreItemIdToItemId(storeItemId, session));
				}
				imGroup.MemberCorrelationKey = list.ToArray();
			}
			imGroup.ExtendedProperties = rawImGroup.ExtendedProperties;
			imGroup.SmtpAddress = rawImGroup.SmtpAddress;
			return imGroup;
		}
	}
}

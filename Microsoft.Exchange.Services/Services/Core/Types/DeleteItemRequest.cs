using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(OccurrenceItemId))]
	[XmlType("DeleteItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[KnownType(typeof(ItemId))]
	[KnownType(typeof(RecurringMasterItemId))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DeleteItemRequest : BaseRequest
	{
		[XmlArray("ItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "ItemIds", IsRequired = true, Order = 1)]
		public BaseItemId[] Ids { get; set; }

		[IgnoreDataMember]
		[XmlAttribute("DeleteType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DisposalType DeleteType { get; set; }

		[XmlIgnore]
		[DataMember(Name = "DeleteType", IsRequired = true, Order = 2)]
		public string DeleteTypeString
		{
			get
			{
				return EnumUtilities.ToString<DisposalType>(this.DeleteType);
			}
			set
			{
				this.DeleteType = EnumUtilities.Parse<DisposalType>(value);
			}
		}

		[DataMember(IsRequired = false, Order = 3)]
		[XmlAttribute("SendMeetingCancellations", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SendMeetingCancellations { get; set; }

		[DataMember(IsRequired = false, Order = 4)]
		[XmlAttribute("AffectedTaskOccurrences", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string AffectedTaskOccurrences { get; set; }

		[DataMember(IsRequired = false, Order = 5)]
		[XmlAttribute(AttributeName = "SuppressReadReceipts")]
		public bool SuppressReadReceipts { get; set; }

		[DataMember(IsRequired = false, Order = 6)]
		[XmlIgnore]
		public bool ReturnMovedItemIds { get; set; }

		protected override List<ServiceObjectId> GetAllIds()
		{
			return new List<ServiceObjectId>(this.Ids);
		}

		[IgnoreDataMember]
		[XmlIgnore]
		internal List<StoreId> ItemStoreIds { get; private set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this.CanOptimizeCommandExecution(callContext))
			{
				return new DeleteItemBatch(callContext, this);
			}
			return new DeleteItem(callContext, this);
		}

		internal override bool CanOptimizeCommandExecution(CallContext callContext)
		{
			if (!base.AllowCommandOptimization("deleteitem"))
			{
				return false;
			}
			if (ExchangeVersion.Current == ExchangeVersion.Exchange2007)
			{
				return false;
			}
			if (this.Ids == null || this.Ids.Length < 2)
			{
				return false;
			}
			IdConverter idConverter = new IdConverter(callContext);
			List<StoreId> list = new List<StoreId>();
			Guid? guid = null;
			foreach (BaseItemId itemId in this.Ids)
			{
				StoreId storeId;
				Guid value;
				bool flag = idConverter.TryGetStoreIdAndMailboxGuidFromItemId(itemId, out storeId, out value) && this.ItemCanBeBulkDeleted(storeId);
				if (guid == null)
				{
					guid = new Guid?(value);
				}
				if (!flag || !value.Equals(guid.Value))
				{
					return false;
				}
				list.Add(storeId);
			}
			this.ItemStoreIds = list;
			return true;
		}

		private bool ItemCanBeBulkDeleted(StoreId storeId)
		{
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
			StoreObjectType objectType = storeObjectId.ObjectType;
			if (objectType != StoreObjectType.Unknown)
			{
				switch (objectType)
				{
				case StoreObjectType.CalendarItem:
				case StoreObjectType.CalendarItemOccurrence:
				case StoreObjectType.Task:
					return false;
				}
				return true;
			}
			return false;
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.Ids == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForItemIdList(callContext, this.Ids);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.Ids == null || this.Ids.Length < taskStep)
			{
				return null;
			}
			return base.GetResourceKeysForItemId(true, callContext, this.Ids[taskStep]);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.Ids == null || this.Ids.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
		}
	}
}

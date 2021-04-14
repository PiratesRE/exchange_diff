using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetItemRequest : BaseRequest, IRemoteArchiveRequest
	{
		[XmlElement(ElementName = "ItemShape")]
		[DataMember(Name = "ItemShape", IsRequired = true, Order = 1)]
		public ItemResponseShape ItemShape { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ShapeName", IsRequired = false)]
		public string ShapeName { get; set; }

		[DataMember(Name = "ItemIds", IsRequired = true, Order = 2)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemIdRanges", typeof(RecurringMasterItemIdRanges), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("ItemIds")]
		public BaseItemId[] Ids { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		internal List<StoreId> PrefetchItemStoreIds { get; private set; }

		[IgnoreDataMember]
		[XmlIgnore]
		internal bool PrefetchItems { get; private set; }

		protected override List<ServiceObjectId> GetAllIds()
		{
			return new List<ServiceObjectId>(this.Ids);
		}

		private bool CanPrefetchItems(CallContext callContext)
		{
			if (!base.AllowCommandOptimization("getitem"))
			{
				return false;
			}
			if (ExchangeVersion.Current == ExchangeVersion.Exchange2007)
			{
				return false;
			}
			if (this.Ids == null || this.Ids.Length == 1)
			{
				return false;
			}
			IdConverter idConverter = new IdConverter(callContext);
			List<StoreId> list = new List<StoreId>();
			Guid? guid = null;
			int num = 0;
			foreach (BaseItemId baseItemId in this.Ids)
			{
				StoreId item = null;
				Guid empty = Guid.Empty;
				bool flag = false;
				if (!string.IsNullOrEmpty(baseItemId.GetChangeKey()))
				{
					flag = idConverter.TryGetStoreIdAndMailboxGuidFromItemId(baseItemId, out item, out empty);
				}
				if (guid == null)
				{
					guid = new Guid?(empty);
				}
				if (!flag || !empty.Equals(guid.Value))
				{
					return false;
				}
				list.Add(item);
				if (num++ >= GetItemRequest.PrefetchItemSizeLimit)
				{
					break;
				}
			}
			this.PrefetchItemStoreIds = list;
			return true;
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return ((IRemoteArchiveRequest)this).GetRemoteArchiveServiceCommand(callContext);
			}
			this.PrefetchItems = this.CanPrefetchItems(callContext);
			return new GetItem(callContext, this);
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
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return null;
			}
			BaseServerIdInfo serverInfoForItemId = BaseRequest.GetServerInfoForItemId(callContext, this.Ids[taskStep]);
			return BaseRequest.ServerInfoToResourceKeys(false, serverInfoForItemId);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.Ids == null || this.Ids.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
		}

		ExchangeServiceBinding IRemoteArchiveRequest.ArchiveService { get; set; }

		bool IRemoteArchiveRequest.IsRemoteArchiveRequest(CallContext callContext)
		{
			return ComplianceUtil.TryCreateArchiveService(callContext, this, this.Ids != null, delegate
			{
				((IRemoteArchiveRequest)this).ArchiveService = ComplianceUtil.GetArchiveServiceForItemIdList(callContext, this.Ids);
			});
		}

		ServiceCommandBase IRemoteArchiveRequest.GetRemoteArchiveServiceCommand(CallContext callContext)
		{
			return new GetRemoteArchiveItem(callContext, this);
		}

		private const string PrefetchItemSizeLimitKeyName = "GetItemPrefetchSizeLimit";

		internal const string ItemIdsElementName = "ItemIds";

		private static readonly int PrefetchItemSizeLimit = Global.GetAppSettingAsInt("GetItemPrefetchSizeLimit", 250);
	}
}

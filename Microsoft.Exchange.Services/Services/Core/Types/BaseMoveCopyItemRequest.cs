using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(CopyItemRequest))]
	[XmlType("BaseMoveCopyItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[KnownType(typeof(CopyItemRequest))]
	[KnownType(typeof(MoveItemRequest))]
	[XmlInclude(typeof(MoveItemRequest))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class BaseMoveCopyItemRequest : BaseRequest
	{
		[XmlElement("ToFolderId")]
		[DataMember(IsRequired = true, Order = 1)]
		public TargetFolderId ToFolderId { get; set; }

		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "ItemIds", IsRequired = true, Order = 2)]
		[XmlArray("ItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseItemId[] Ids { get; set; }

		[DataMember(IsRequired = false, Order = 3)]
		[XmlElement("ReturnNewItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool ReturnNewItemIds
		{
			get
			{
				return this.returnNewItemIds;
			}
			set
			{
				this.returnNewItemIds = value;
			}
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			return new List<ServiceObjectId>(this.Ids)
			{
				this.ToFolderId.BaseFolderId
			};
		}

		[IgnoreDataMember]
		[XmlIgnore]
		internal List<StoreId> ItemStoreIds { get; private set; }

		internal override bool CanOptimizeCommandExecution(CallContext callContext)
		{
			if (!base.AllowCommandOptimization(this.CommandName) || !callContext.AuthZBehavior.IsAllowedToOptimizeMoveCopyCommand())
			{
				return false;
			}
			if (ExchangeVersion.Current == ExchangeVersion.Exchange2007)
			{
				return false;
			}
			if (this.Ids == null || this.Ids.Length == 0)
			{
				return false;
			}
			IdConverter idConverter = new IdConverter(callContext);
			List<StoreId> list = new List<StoreId>();
			Guid? guid = null;
			foreach (BaseItemId itemId in this.Ids)
			{
				StoreId item;
				Guid value;
				bool flag = idConverter.TryGetStoreIdAndMailboxGuidFromItemId(itemId, out item, out value);
				if (guid == null)
				{
					guid = new Guid?(value);
				}
				if (!flag || !value.Equals(guid.Value))
				{
					ExTraceGlobals.CopyItemCallTracer.TraceDebug((long)this.GetHashCode(), "Move/CopyItem operation cannot be optimized. Not all items are in the same mailbox.");
					return false;
				}
				list.Add(item);
			}
			if (list.Count((StoreId id) => IdConverter.GetAsStoreObjectId(id).ObjectType == StoreObjectType.CalendarItemOccurrence) > 0)
			{
				ExTraceGlobals.CopyItemCallTracer.TraceDebug((long)this.GetHashCode(), "Move/CopyItem operation cannot be optimized. List of items contains one or more calendar occurrences.");
				return false;
			}
			this.ItemStoreIds = list;
			return true;
		}

		protected abstract string CommandName { get; }

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForFolderId(callContext, this.ToFolderId.BaseFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			if (this.toFolderResourceKey == null)
			{
				BaseServerIdInfo serverInfoForFolderId = BaseRequest.GetServerInfoForFolderId(callContext, this.ToFolderId.BaseFolderId);
				if (serverInfoForFolderId != null)
				{
					this.toFolderResourceKey = serverInfoForFolderId.ToResourceKey(true);
				}
			}
			ResourceKey[] array = null;
			BaseServerIdInfo serverInfoForItemId = BaseRequest.GetServerInfoForItemId(callContext, this.Ids[currentStep]);
			if (serverInfoForItemId != null)
			{
				array = serverInfoForItemId.ToResourceKey(false);
			}
			List<ResourceKey> list = new List<ResourceKey>();
			if (this.toFolderResourceKey != null)
			{
				list.AddRange(this.toFolderResourceKey);
			}
			if (array != null)
			{
				list.AddRange(array);
			}
			if (list.Count != 0)
			{
				return list.ToArray();
			}
			return null;
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.Ids == null || this.Ids.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdMalformed), FaultParty.Sender);
			}
		}

		private ResourceKey[] toFolderResourceKey;

		private bool returnNewItemIds = true;
	}
}

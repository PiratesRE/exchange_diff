using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("UpdateItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UpdateItemRequest : BaseRequest
	{
		[DataMember(Name = "SavedItemFolderId", IsRequired = false)]
		[XmlElement("SavedItemFolderId")]
		public TargetFolderId SavedItemFolderId { get; set; }

		[XmlArrayItem("ItemChange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(ItemChange))]
		[DataMember(Name = "ItemChanges", IsRequired = true)]
		[XmlArray(ElementName = "ItemChanges", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ItemChange[] ItemChanges { get; set; }

		[XmlAttribute(AttributeName = "ConflictResolution")]
		[IgnoreDataMember]
		public ConflictResolutionType ConflictResolution { get; set; }

		[DataMember(Name = "ConflictResolution", IsRequired = true)]
		[XmlIgnore]
		public string ConflictResolutionString
		{
			get
			{
				return EnumUtilities.ToString<ConflictResolutionType>(this.ConflictResolution);
			}
			set
			{
				this.ConflictResolution = EnumUtilities.Parse<ConflictResolutionType>(value);
			}
		}

		[DataMember(Name = "MessageDisposition", IsRequired = false)]
		[XmlAttribute("MessageDisposition", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string MessageDisposition { get; set; }

		[DataMember(Name = "ItemShape", IsRequired = false)]
		[XmlIgnore]
		public ItemResponseShape ItemShape { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ShapeName", IsRequired = false)]
		public string ShapeName { get; set; }

		[DataMember(Name = "SendCalendarInvitationsOrCancellations", IsRequired = false)]
		[XmlAttribute("SendMeetingInvitationsOrCancellations", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SendCalendarInvitationsOrCancellations { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ComplianceId", IsRequired = false)]
		public string ComplianceId { get; set; }

		[DataMember(Name = "ClientSupportsIrm", IsRequired = false)]
		[XmlIgnore]
		public bool ClientSupportsIrm { get; set; }

		[XmlIgnore]
		[DataMember(Name = "PromoteInlineAttachments", IsRequired = false)]
		public bool PromoteInlineAttachments { get; set; }

		[DataMember(Name = "MarkRefAttachAsInline", IsRequired = false)]
		[XmlIgnore]
		public bool MarkRefAttachAsInline { get; set; }

		[DataMember(Name = "SendOnNotFoundError", IsRequired = false)]
		[XmlIgnore]
		public bool SendOnNotFoundError { get; set; }

		[XmlAttribute(AttributeName = "SuppressReadReceipts")]
		[DataMember(Name = "SuppressReadReceipts", IsRequired = false)]
		public bool SuppressReadReceipts { get; set; }

		[DataMember(Name = "ComposeOperation", IsRequired = false)]
		[XmlAttribute("ComposeOperation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string ComposeOperation { get; set; }

		[XmlIgnore]
		[DataMember(Name = "OutboundCharset", IsRequired = false)]
		public string OutboundCharset
		{
			get
			{
				return this.outboundCharset.ToString();
			}
			set
			{
				this.outboundCharset = (OutboundCharsetOptions)Enum.Parse(typeof(OutboundCharsetOptions), value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "UseGB18030", IsRequired = false)]
		public bool UseGB18030 { get; set; }

		[DataMember(Name = "UseISO885915", IsRequired = false)]
		[XmlIgnore]
		public bool UseISO885915 { get; set; }

		public OutboundCharsetOptions OutboundCharsetOptions
		{
			get
			{
				return this.outboundCharset;
			}
		}

		[DataMember(Name = "InternetMessageId", IsRequired = false)]
		[XmlIgnore]
		public string InternetMessageId { get; set; }

		protected override List<ServiceObjectId> GetAllIds()
		{
			List<ServiceObjectId> list = new List<ServiceObjectId>();
			foreach (ItemChange itemChange in this.ItemChanges)
			{
				list.Add(itemChange.ItemId);
			}
			return list;
		}

		[IgnoreDataMember]
		[XmlIgnore]
		internal List<KeyValuePair<ItemChange, StoreId>> MarkAsReadItemChanges { get; private set; }

		[XmlIgnore]
		[IgnoreDataMember]
		internal List<KeyValuePair<ItemChange, StoreId>> MarkAsUnreadItemChanges { get; private set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this.CanOptimizeCommandExecution(callContext))
			{
				return new UpdateItemIsReadBatch(callContext, this);
			}
			return new UpdateItem(callContext, this);
		}

		internal override bool CanOptimizeCommandExecution(CallContext callContext)
		{
			if (!base.AllowCommandOptimization("updateitem"))
			{
				return false;
			}
			if (ExchangeVersion.Current == ExchangeVersion.Exchange2007)
			{
				return false;
			}
			IdConverter idConverter = new IdConverter(callContext);
			List<KeyValuePair<ItemChange, StoreId>> list = new List<KeyValuePair<ItemChange, StoreId>>();
			List<KeyValuePair<ItemChange, StoreId>> list2 = new List<KeyValuePair<ItemChange, StoreId>>();
			Guid? guid = null;
			new List<ItemChange>();
			ItemChange[] itemChanges = this.ItemChanges;
			int i = 0;
			while (i < itemChanges.Length)
			{
				ItemChange itemChange = itemChanges[i];
				BaseItemId itemId = itemChange.ItemId;
				StoreId value;
				Guid value2;
				if (idConverter.TryGetStoreIdAndMailboxGuidFromItemId(itemId, out value, out value2))
				{
					if (guid == null)
					{
						guid = new Guid?(value2);
					}
					else if (!value2.Equals(guid.Value))
					{
						return false;
					}
					if (itemChange.PropertyUpdates.Length == 1)
					{
						PropertyUpdate propertyUpdate = itemChange.PropertyUpdates[0];
						SetItemPropertyUpdate setItemPropertyUpdate = propertyUpdate as SetItemPropertyUpdate;
						if (setItemPropertyUpdate != null)
						{
							PropertyUri propertyUri = setItemPropertyUpdate.PropertyPath as PropertyUri;
							if (propertyUri != null && propertyUri.Uri == PropertyUriEnum.IsRead)
							{
								KeyValuePair<ItemChange, StoreId> item = new KeyValuePair<ItemChange, StoreId>(itemChange, value);
								bool valueOrDefault = setItemPropertyUpdate.ServiceObject.GetValueOrDefault<bool>(MessageSchema.IsRead);
								if (valueOrDefault)
								{
									list.Add(item);
								}
								else
								{
									list2.Add(item);
								}
							}
						}
					}
					i++;
					continue;
				}
				return false;
			}
			this.MarkAsUnreadItemChanges = list2;
			this.MarkAsReadItemChanges = list;
			return this.MarkAsReadItemChanges.Count > 0 || this.MarkAsUnreadItemChanges.Count > 0;
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.ItemChanges == null || this.ItemChanges.Length == 0)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForItemId(callContext, this.ItemChanges[0].ItemId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			BaseServerIdInfo baseServerIdInfo = (this.SavedItemFolderId == null || this.SavedItemFolderId.BaseFolderId == null) ? null : BaseRequest.GetServerInfoForFolderId(callContext, this.SavedItemFolderId.BaseFolderId);
			BaseServerIdInfo baseServerIdInfo2 = (this.ItemChanges == null || this.ItemChanges.Length == 0) ? null : BaseRequest.GetServerInfoForItemId(callContext, this.ItemChanges[taskStep].ItemId);
			return BaseRequest.ServerInfosToResourceKeys(true, new BaseServerIdInfo[]
			{
				baseServerIdInfo,
				baseServerIdInfo2
			});
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.ItemChanges == null || this.ItemChanges.Length == 0)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidArgument), FaultParty.Sender);
			}
		}

		internal const string ElementName = "UpdateItem";

		internal const string SavedItemFolderIdElementName = "SavedItemFolderId";

		internal const string ItemChangesElementName = "ItemChanges";

		private OutboundCharsetOptions outboundCharset;
	}
}

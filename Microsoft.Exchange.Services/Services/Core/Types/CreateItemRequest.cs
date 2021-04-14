using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("CreateItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateItemRequest : BaseRequest
	{
		[DataMember(Name = "SavedItemFolderId", IsRequired = false, Order = 1)]
		[XmlElement("SavedItemFolderId")]
		public TargetFolderId SavedItemFolderId { get; set; }

		[IgnoreDataMember]
		[XmlElement("Items", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public NonEmptyArrayOfAllItemsType Items { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Items", IsRequired = true, Order = 2)]
		public ItemType[] ItemsArray
		{
			get
			{
				if (this.Items == null)
				{
					return null;
				}
				return this.Items.Items;
			}
			set
			{
				this.Items = new NonEmptyArrayOfAllItemsType
				{
					Items = value
				};
			}
		}

		[DataMember(IsRequired = false, Order = 3)]
		[XmlAttribute("MessageDisposition", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string MessageDisposition { get; set; }

		[DataMember(IsRequired = false, Order = 4)]
		[XmlAttribute("SendMeetingInvitations", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SendMeetingInvitations { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ItemShape", IsRequired = false, Order = 5)]
		public ItemResponseShape ItemShape { get; set; }

		[DataMember(Name = "ShapeName", IsRequired = false, Order = 6)]
		[XmlIgnore]
		public string ShapeName { get; set; }

		[DataMember(Name = "ComplianceId", IsRequired = false, Order = 7)]
		[XmlIgnore]
		public string ComplianceId { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 8)]
		public bool GenerateResponseMessageOnFailure { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 9)]
		[XmlIgnore]
		public bool FailResponseOnImportantUpdate { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 10)]
		[XmlIgnore]
		public bool GenerateMeetingResponseWithOldLocationIfChanged { get; set; }

		[DataMember(Name = "ClientSupportsIrm", IsRequired = false, Order = 11)]
		[XmlIgnore]
		public bool ClientSupportsIrm { get; set; }

		[DataMember(Name = "SendOnNotFoundError", IsRequired = false)]
		[XmlIgnore]
		public bool SendOnNotFoundError { get; set; }

		[XmlIgnore]
		[DataMember(Name = "TimeFormat", IsRequired = false, Order = 12)]
		public string TimeFormat { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 13)]
		public bool ShouldSuppressReadReceipt { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 14)]
		public string SubjectPrefix { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 15)]
		public bool IsNonDraft { get; set; }

		[DataMember(IsRequired = false, Order = 16)]
		[XmlAttribute("ComposeOperation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string ComposeOperation { get; set; }

		[DataMember(Name = "OutboundCharset", IsRequired = false, Order = 17)]
		[XmlIgnore]
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
		[DataMember(Name = "UseGB18030", IsRequired = false, Order = 18)]
		public bool UseGB18030 { get; set; }

		[XmlIgnore]
		[DataMember(Name = "UseISO885915", IsRequired = false, Order = 19)]
		public bool UseISO885915 { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 20)]
		public bool IsDraftEvent { get; set; }

		public OutboundCharsetOptions OutboundCharsetOptions
		{
			get
			{
				return this.outboundCharset;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CreateItem(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return this.GetServerInfoForStep(callContext, 0);
		}

		internal BaseServerIdInfo GetServerInfoForStep(CallContext callContext, int step)
		{
			BaseServerIdInfo baseServerIdInfo = null;
			if (this.SavedItemFolderId == null)
			{
				ItemType itemType = this.Items.Items[step];
				ResponseObjectCoreType responseObjectCoreType = itemType as ResponseObjectCoreType;
				if (responseObjectCoreType != null)
				{
					BaseItemId referenceItemId = responseObjectCoreType.ReferenceItemId;
					if (referenceItemId != null)
					{
						baseServerIdInfo = BaseRequest.GetServerInfoForItemId(callContext, referenceItemId);
					}
				}
				if (baseServerIdInfo == null)
				{
					baseServerIdInfo = callContext.GetServerInfoForEffectiveCaller();
				}
			}
			else
			{
				baseServerIdInfo = BaseRequest.GetServerInfoForFolderId(callContext, this.SavedItemFolderId.BaseFolderId);
			}
			return baseServerIdInfo;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			BaseServerIdInfo baseServerIdInfo = (this.SavedItemFolderId == null || this.SavedItemFolderId.BaseFolderId == null) ? null : BaseRequest.GetServerInfoForFolderId(callContext, this.SavedItemFolderId.BaseFolderId);
			BaseServerIdInfo serverInfoForStep = this.GetServerInfoForStep(callContext, taskStep);
			return BaseRequest.ServerInfosToResourceKeys(true, new BaseServerIdInfo[]
			{
				baseServerIdInfo,
				serverInfoForStep
			});
		}

		internal const string ElementName = "CreateItem";

		internal const string SavedItemFolderIdElementName = "SavedItemFolderId";

		internal const string ItemsElementName = "Items";

		private OutboundCharsetOptions outboundCharset;
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Conversation")]
	[Serializable]
	public class ConversationType : ServiceObject
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public BaseFolderId FolderId { get; set; }

		[DataMember(Order = 2)]
		public ItemId ConversationId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ItemId>(ConversationSchema.ConversationId);
			}
			set
			{
				base.PropertyBag[ConversationSchema.ConversationId] = value;
			}
		}

		[DataMember(Order = 4)]
		public string ConversationTopic
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.ConversationTopic);
			}
			set
			{
				base.PropertyBag[ConversationSchema.ConversationTopic] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 5)]
		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueRecipients
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.UniqueRecipients);
			}
			set
			{
				base.PropertyBag[ConversationSchema.UniqueRecipients] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 6)]
		[XmlArrayItem("String", IsNullable = false)]
		public string[] GlobalUniqueRecipients
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.GlobalUniqueRecipients);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalUniqueRecipients] = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 7)]
		public string[] UniqueUnreadSenders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.UniqueUnreadSenders);
			}
			set
			{
				base.PropertyBag[ConversationSchema.UniqueUnreadSenders] = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 8)]
		public string[] GlobalUniqueUnreadSenders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.GlobalUniqueUnreadSenders);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalUniqueUnreadSenders] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 9)]
		[XmlArrayItem("String", IsNullable = false)]
		public string[] UniqueSenders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.UniqueSenders);
			}
			set
			{
				base.PropertyBag[ConversationSchema.UniqueSenders] = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 10)]
		public string[] GlobalUniqueSenders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.GlobalUniqueSenders);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalUniqueSenders] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 11)]
		[DateTimeString]
		public string LastDeliveryTime
		{
			get
			{
				return base.GetValueOrDefault<string>(ConversationSchema.LastDeliveryTime);
			}
			set
			{
				this[ConversationSchema.LastDeliveryTime] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 12)]
		public string GlobalLastDeliveryTime
		{
			get
			{
				return base.GetValueOrDefault<string>(ConversationSchema.GlobalLastDeliveryTime);
			}
			set
			{
				this[ConversationSchema.GlobalLastDeliveryTime] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 13)]
		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.Categories);
			}
			set
			{
				base.PropertyBag[ConversationSchema.Categories] = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 14)]
		public string[] GlobalCategories
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.GlobalCategories);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalCategories] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Name = "FlagStatus", Order = 15)]
		[XmlIgnore]
		public string FlagStatusString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.FlagStatus);
			}
			set
			{
				base.PropertyBag[ConversationSchema.FlagStatus] = value;
			}
		}

		[XmlElement("FlagStatus")]
		[IgnoreDataMember]
		public FlagStatus FlagStatus
		{
			get
			{
				if (!this.FlagStatusSpecified)
				{
					return FlagStatus.NotFlagged;
				}
				return EnumUtilities.Parse<FlagStatus>(this.FlagStatusString);
			}
			set
			{
				this.FlagStatusString = EnumUtilities.ToString<FlagStatus>(value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool FlagStatusSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.FlagStatus);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Name = "GlobalFlagStatus", Order = 16)]
		public string GlobalFlagStatusString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.GlobalFlagStatus);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalFlagStatus] = value;
			}
		}

		[IgnoreDataMember]
		[XmlElement("GlobalFlagStatus")]
		public FlagStatus GlobalFlagStatus
		{
			get
			{
				if (!this.GlobalFlagStatusSpecified)
				{
					return FlagStatus.NotFlagged;
				}
				return EnumUtilities.Parse<FlagStatus>(this.GlobalFlagStatusString);
			}
			set
			{
				this.GlobalFlagStatusString = EnumUtilities.ToString<FlagStatus>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool GlobalFlagStatusSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalFlagStatus);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 17)]
		public bool? HasAttachments
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ConversationSchema.HasAttachments);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ConversationSchema.HasAttachments, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool HasAttachmentsSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.HasAttachments);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 18)]
		public bool? GlobalHasAttachments
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ConversationSchema.GlobalHasAttachments);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ConversationSchema.GlobalHasAttachments, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool GlobalHasAttachmentsSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalHasAttachments);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 19)]
		public int? MessageCount
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ConversationSchema.MessageCount);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ConversationSchema.MessageCount, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool MessageCountSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.MessageCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 20)]
		public int? GlobalMessageCount
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ConversationSchema.GlobalMessageCount);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ConversationSchema.GlobalMessageCount, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool GlobalMessageCountSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalMessageCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 21)]
		public int? UnreadCount
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ConversationSchema.UnreadCount);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ConversationSchema.UnreadCount, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool UnreadCountSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.UnreadCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 22)]
		public int? GlobalUnreadCount
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ConversationSchema.GlobalUnreadCount);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ConversationSchema.GlobalUnreadCount, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool GlobalUnreadCountSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalUnreadCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 23)]
		public int? Size
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ConversationSchema.Size);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ConversationSchema.Size, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool SizeSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.Size);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 24)]
		public int? GlobalSize
		{
			get
			{
				return base.PropertyBag.GetNullableValue<int>(ConversationSchema.GlobalSize);
			}
			set
			{
				base.PropertyBag.SetNullableValue<int>(ConversationSchema.GlobalSize, value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool GlobalSizeSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalSize);
			}
			set
			{
			}
		}

		[XmlArrayItem("ItemClass", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 25)]
		public string[] ItemClasses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.ItemClasses);
			}
			set
			{
				base.PropertyBag[ConversationSchema.ItemClasses] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 26)]
		[XmlArrayItem("ItemClass", IsNullable = false)]
		public string[] GlobalItemClasses
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(ConversationSchema.GlobalItemClasses);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalItemClasses] = value;
			}
		}

		[IgnoreDataMember]
		public ImportanceType Importance
		{
			get
			{
				if (!this.ImportanceSpecified)
				{
					return ImportanceType.Normal;
				}
				return EnumUtilities.Parse<ImportanceType>(this.ImportanceString);
			}
			set
			{
				this.ImportanceString = EnumUtilities.ToString<ImportanceType>(value);
			}
		}

		[DataMember(EmitDefaultValue = false, Name = "Importance", Order = 27)]
		[XmlIgnore]
		public string ImportanceString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.Importance);
			}
			set
			{
				base.PropertyBag[ConversationSchema.Importance] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ImportanceSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.Importance);
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		public ImportanceType GlobalImportance
		{
			get
			{
				if (!this.GlobalImportanceSpecified)
				{
					return ImportanceType.Normal;
				}
				return EnumUtilities.Parse<ImportanceType>(this.GlobalImportanceString);
			}
			set
			{
				this.GlobalImportanceString = EnumUtilities.ToString<ImportanceType>(value);
			}
		}

		[DataMember(EmitDefaultValue = false, Name = "GlobalImportance", Order = 28)]
		[XmlIgnore]
		public string GlobalImportanceString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.GlobalImportance);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalImportance] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool GlobalImportanceSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalImportance);
			}
			set
			{
			}
		}

		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 29)]
		[XmlArrayItem("ItemId", typeof(ItemId), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), IsNullable = false)]
		public BaseItemId[] ItemIds
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BaseItemId[]>(ConversationSchema.ItemIds);
			}
			set
			{
				base.PropertyBag[ConversationSchema.ItemIds] = value;
			}
		}

		[XmlArrayItem("ItemId", typeof(ItemId), IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 30)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), IsNullable = false)]
		public BaseItemId[] GlobalItemIds
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BaseItemId[]>(ConversationSchema.GlobalItemIds);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalItemIds] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 31)]
		public string LastModifiedTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.LastModifiedTime);
			}
			set
			{
				base.PropertyBag[ConversationSchema.LastModifiedTime] = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		[IgnoreDataMember]
		public byte[] InstanceKey
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<byte[]>(ConversationSchema.InstanceKey);
			}
			set
			{
				base.PropertyBag[ConversationSchema.InstanceKey] = value;
			}
		}

		[DataMember(Name = "InstanceKey", EmitDefaultValue = false, Order = 32)]
		[XmlIgnore]
		public string InstanceKeyString
		{
			get
			{
				byte[] instanceKey = this.InstanceKey;
				if (instanceKey == null)
				{
					return null;
				}
				return Convert.ToBase64String(instanceKey);
			}
			set
			{
				this.InstanceKey = ((value != null) ? Convert.FromBase64String(value) : null);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 33)]
		public string Preview
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.Preview);
			}
			set
			{
				base.PropertyBag[ConversationSchema.Preview] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool MailboxScopeSpecified { get; set; }

		[XmlElement]
		[IgnoreDataMember]
		public MailboxSearchLocation MailboxScope
		{
			get
			{
				return this.mailboxScope;
			}
			set
			{
				this.mailboxScope = value;
				this.MailboxScopeSpecified = true;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "MailboxScope", IsRequired = false, Order = 36)]
		public string MailboxScopeString
		{
			get
			{
				if (!this.MailboxScopeSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<MailboxSearchLocation>(this.mailboxScope);
			}
			set
			{
				this.MailboxScope = EnumUtilities.Parse<MailboxSearchLocation>(value);
			}
		}

		[IgnoreDataMember]
		[XmlElement]
		public IconIndexType IconIndex
		{
			get
			{
				if (!this.IconIndexSpecified)
				{
					return IconIndexType.Default;
				}
				return EnumUtilities.Parse<IconIndexType>(this.IconIndexString);
			}
			set
			{
				this.IconIndexString = EnumUtilities.ToString<IconIndexType>(value);
			}
		}

		[DataMember(Name = "IconIndex", EmitDefaultValue = false, Order = 37)]
		[XmlIgnore]
		public string IconIndexString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.IconIndex);
			}
			set
			{
				base.PropertyBag[ConversationSchema.IconIndex] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IconIndexSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.IconIndex);
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		[XmlElement]
		public IconIndexType GlobalIconIndex
		{
			get
			{
				if (!this.GlobalIconIndexSpecified)
				{
					return IconIndexType.Default;
				}
				return EnumUtilities.Parse<IconIndexType>(this.GlobalIconIndexString);
			}
			set
			{
				this.GlobalIconIndexString = EnumUtilities.ToString<IconIndexType>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "GlobalIconIndex", EmitDefaultValue = false, Order = 38)]
		public string GlobalIconIndexString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.GlobalIconIndex);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalIconIndex] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool GlobalIconIndexSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalIconIndex);
			}
			set
			{
			}
		}

		[XmlArrayItem("ItemId", typeof(ItemId), IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 39)]
		public BaseItemId[] DraftItemIds
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<BaseItemId[]>(ConversationSchema.DraftItemIds);
			}
			set
			{
				base.PropertyBag[ConversationSchema.DraftItemIds] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 40)]
		public bool? HasIrm
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ConversationSchema.HasIrm);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ConversationSchema.HasIrm, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool HasIrmSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.HasIrm);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 41)]
		public bool? GlobalHasIrm
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ConversationSchema.GlobalHasIrm);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ConversationSchema.GlobalHasIrm, value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool GlobalHasIrmSpecified
		{
			get
			{
				return base.IsSet(ConversationSchema.GlobalHasIrm);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 42)]
		[XmlIgnore]
		public bool? HasClutter
		{
			get
			{
				return base.PropertyBag.GetNullableValue<bool>(ConversationSchema.HasClutter);
			}
			set
			{
				base.PropertyBag.SetNullableValue<bool>(ConversationSchema.HasClutter, value);
			}
		}

		internal IEnumerable<StoreId> DraftStoreIds { get; set; }

		internal IList<BaseItemId> DraftItemIdsList { get; set; }

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.ConversationActionItem;
			}
		}

		internal static ConversationType MergeConversations(ConversationType x, ConversationType y)
		{
			ConversationType conversationType = new ConversationType();
			conversationType.Categories = ConversationHelper.MergeArray<string>(x.Categories, y.Categories);
			conversationType.ConversationId = x.ConversationId;
			conversationType.ConversationTopic = x.ConversationTopic;
			conversationType.DraftItemIds = ConversationHelper.MergeArray<BaseItemId>(x.DraftItemIds, y.DraftItemIds);
			conversationType.FlagStatus = x.FlagStatus;
			conversationType.FolderId = x.FolderId;
			conversationType.GlobalCategories = ConversationHelper.MergeArray<string>(x.GlobalCategories, y.GlobalCategories);
			conversationType.GlobalFlagStatus = x.GlobalFlagStatus;
			conversationType.GlobalHasAttachments = ConversationHelper.MergeBoolNullable(x.GlobalHasAttachments, y.GlobalHasAttachments);
			conversationType.GlobalImportance = x.GlobalImportance;
			conversationType.GlobalIconIndex = ((x.GlobalIconIndex == IconIndexType.Default) ? y.GlobalIconIndex : x.GlobalIconIndex);
			conversationType.GlobalItemClasses = ConversationHelper.MergeArray<string>(x.GlobalItemClasses, y.GlobalItemClasses);
			conversationType.GlobalItemIds = ConversationHelper.MergeArray<BaseItemId>(x.GlobalItemIds, y.GlobalItemIds);
			conversationType.GlobalLastDeliveryTime = ConversationHelper.MergeDates(x.GlobalLastDeliveryTime, y.GlobalLastDeliveryTime);
			conversationType.GlobalLastDeliveryOrRenewTime = ConversationHelper.MergeDates(x.GlobalLastDeliveryOrRenewTime, y.GlobalLastDeliveryOrRenewTime);
			conversationType.GlobalMessageCount = ConversationHelper.MergeInts(x.GlobalMessageCount, y.GlobalMessageCount);
			conversationType.GlobalSize = ConversationHelper.MergeInts(x.GlobalSize, y.GlobalSize);
			conversationType.GlobalRichContent = ConversationHelper.MergeArray<short>(x.GlobalRichContent, y.GlobalRichContent);
			conversationType.GlobalUniqueRecipients = ConversationHelper.MergeArray<string>(x.GlobalUniqueRecipients, y.GlobalUniqueRecipients);
			conversationType.GlobalUniqueSenders = ConversationHelper.MergeArray<string>(x.GlobalUniqueSenders, y.GlobalUniqueSenders);
			conversationType.GlobalUniqueUnreadSenders = ConversationHelper.MergeArray<string>(x.GlobalUniqueUnreadSenders, y.GlobalUniqueUnreadSenders);
			conversationType.GlobalUnreadCount = ConversationHelper.MergeInts(x.GlobalUnreadCount, y.GlobalUnreadCount);
			conversationType.HasAttachments = ConversationHelper.MergeBoolNullable(x.HasAttachments, y.HasAttachments);
			conversationType.IconIndex = ((x.IconIndex == IconIndexType.Default) ? y.IconIndex : x.IconIndex);
			conversationType.Importance = x.Importance;
			conversationType.SetInstanceKey(x.InstanceKey);
			conversationType.ItemClasses = ConversationHelper.MergeArray<string>(x.ItemClasses, y.ItemClasses);
			conversationType.ItemIds = ConversationHelper.MergeArray<BaseItemId>(x.ItemIds, y.ItemIds);
			conversationType.LastDeliveryTime = ConversationHelper.MergeDates(x.LastDeliveryTime, y.LastDeliveryTime);
			conversationType.LastDeliveryOrRenewTime = ConversationHelper.MergeDates(x.LastDeliveryOrRenewTime, y.LastDeliveryOrRenewTime);
			conversationType.LastModifiedTime = ConversationHelper.MergeDates(x.LastModifiedTime, y.LastModifiedTime);
			conversationType.MailboxScope = MailboxSearchLocation.All;
			conversationType.MessageCount = ConversationHelper.MergeInts(x.MessageCount, y.MessageCount);
			conversationType.Preview = x.Preview;
			conversationType.Size = ConversationHelper.MergeInts(x.Size, y.Size);
			conversationType.UniqueRecipients = ConversationHelper.MergeArray<string>(x.UniqueRecipients, y.UniqueRecipients);
			conversationType.UniqueSenders = ConversationHelper.MergeArray<string>(x.UniqueSenders, y.UniqueSenders);
			conversationType.UniqueUnreadSenders = ConversationHelper.MergeArray<string>(x.UniqueUnreadSenders, y.UniqueUnreadSenders);
			conversationType.UnreadCount = ConversationHelper.MergeInts(x.UnreadCount, y.UnreadCount);
			conversationType.HasClutter = x.HasClutter;
			return conversationType;
		}

		internal static ConversationType LoadFromAggregatedConversation(IStorePropertyBag aggregatedProperties, MailboxSession session, PropertyUriEnum[] properties)
		{
			ConversationType conversationType = new ConversationType();
			AggregatedConversationLoader.LoadProperties(conversationType, aggregatedProperties, session, properties);
			return conversationType;
		}

		internal override void AddExtendedPropertyValue(ExtendedPropertyType extendedProperty)
		{
			throw new InvalidOperationException("Conversations don't have extended properties. This method should not be called.");
		}

		private void SetInstanceKey(byte[] value)
		{
			base.PropertyBag[ConversationSchema.InstanceKey] = value;
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 40)]
		public string[] GlobalItemChangeKeys { get; set; }

		[XmlArrayItem("Boolean", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 41)]
		public bool[] GlobalItemReadFlags { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public object InternalInitialPost
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<object>(ConversationSchema.InitialPost);
			}
			set
			{
				base.PropertyBag[ConversationSchema.InitialPost] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public object InternalRecentReplys
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<object>(ConversationSchema.RecentReplys);
			}
			set
			{
				base.PropertyBag[ConversationSchema.RecentReplys] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 42)]
		[XmlIgnore]
		[XmlElement("MeetingResponse", typeof(MeetingResponseMessageType))]
		[XmlElement("Message", typeof(MessageType))]
		[XmlElement("MeetingRequest", typeof(MeetingRequestMessageType))]
		[XmlElement("MeetingCancellation", typeof(MeetingCancellationMessageType))]
		[XmlElement("MeetingMessage", typeof(MeetingMessageType))]
		public MessageType InitialPost
		{
			get
			{
				return (MessageType)this.InternalInitialPost;
			}
			set
			{
				this.InternalInitialPost = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "RecentReplys", EmitDefaultValue = false, IsRequired = false, Order = 43)]
		public MessageType[] RecentReplys
		{
			get
			{
				return (MessageType[])this.InternalRecentReplys;
			}
			set
			{
				this.InternalRecentReplys = value;
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 44)]
		public ItemId FamilyId
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ItemId>(ConversationSchema.FamilyId);
			}
			set
			{
				base.PropertyBag[ConversationSchema.FamilyId] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 45)]
		[XmlArrayItem("Int16", IsNullable = false)]
		public short[] GlobalRichContent
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<short[]>(ConversationSchema.GlobalRichContent);
			}
			set
			{
				base.PropertyBag[ConversationSchema.GlobalRichContent] = value;
			}
		}

		[XmlIgnore]
		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 46)]
		public string LastDeliveryOrRenewTime
		{
			get
			{
				return base.GetValueOrDefault<string>(ConversationSchema.LastDeliveryOrRenewTime);
			}
			set
			{
				this[ConversationSchema.LastDeliveryOrRenewTime] = value;
			}
		}

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Order = 47)]
		[DateTimeString]
		public string GlobalLastDeliveryOrRenewTime
		{
			get
			{
				return base.GetValueOrDefault<string>(ConversationSchema.GlobalLastDeliveryOrRenewTime);
			}
			set
			{
				this[ConversationSchema.GlobalLastDeliveryOrRenewTime] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 48)]
		[XmlIgnore]
		public string WorkingSetSourcePartition
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(ConversationSchema.WorkingSetSourcePartition);
			}
			set
			{
				base.PropertyBag[ConversationSchema.WorkingSetSourcePartition] = value;
			}
		}

		internal void BulkAssignProperties(PropertyDefinition[] propertyDefinitions, object[] propertyValues, Guid mailboxGuid, ExTimeZone timeZone = null)
		{
			this.ConversationId = new ItemId(IdConverter.ConversationIdToEwsId(mailboxGuid, this.GetItemProperty<ConversationId>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationId)), null);
			this.ConversationTopic = this.GetItemProperty<string>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationTopic);
			this.UniqueRecipients = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationMVTo);
			this.GlobalUniqueRecipients = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalMVTo);
			this.UniqueSenders = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationMVFrom);
			this.GlobalUniqueSenders = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalMVFrom);
			this.UniqueUnreadSenders = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationMVUnreadFrom);
			this.GlobalUniqueUnreadSenders = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalMVUnreadFrom);
			this.LastDeliveryTime = this.GetDateTimeProperty(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationLastDeliveryTime, timeZone);
			this.GlobalLastDeliveryTime = this.GetDateTimeProperty(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalLastDeliveryTime, timeZone);
			this.LastDeliveryOrRenewTime = this.GetDateTimeProperty(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationLastDeliveryOrRenewTime, timeZone);
			this.GlobalLastDeliveryOrRenewTime = this.GetDateTimeProperty(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalLastDeliveryOrRenewTime, timeZone);
			this.Categories = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationCategories);
			FlagType flagType = new FlagType();
			if (this.IsPropertyDefined(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationFlagStatus))
			{
				flagType.FlagStatus = (FlagStatus)this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationFlagStatus, 0);
				this.FlagStatus = flagType.FlagStatus;
			}
			flagType = new FlagType();
			if (this.IsPropertyDefined(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalFlagStatus))
			{
				flagType.FlagStatus = (FlagStatus)this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalFlagStatus, 0);
				this.GlobalFlagStatus = flagType.FlagStatus;
			}
			this.HasAttachments = new bool?(this.GetItemProperty<bool>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationHasAttach));
			this.GlobalHasAttachments = new bool?(this.GetItemProperty<bool>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalHasAttach));
			this.HasIrm = new bool?(this.GetItemProperty<bool>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationHasIrm));
			this.GlobalHasIrm = new bool?(this.GetItemProperty<bool>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalHasIrm));
			this.MessageCount = new int?(this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationMessageCount));
			this.GlobalMessageCount = new int?(this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalMessageCount));
			this.UnreadCount = new int?(this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationUnreadMessageCount));
			this.GlobalUnreadCount = new int?(this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalUnreadMessageCount));
			this.Size = new int?(this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationMessageSize));
			this.GlobalSize = new int?(this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalMessageSize));
			this.ItemClasses = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationMessageClasses);
			this.GlobalItemClasses = this.GetItemProperty<string[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalMessageClasses);
			this.ImportanceString = ((ImportanceType)this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationImportance, 1)).ToString();
			this.GlobalImportanceString = ((ImportanceType)this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalImportance, 1)).ToString();
			this.GlobalRichContent = this.GetItemProperty<short[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalRichContent);
			StoreId[] itemProperty = this.GetItemProperty<StoreId[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationItemIds, new StoreId[0]);
			this.ItemIds = new ItemId[itemProperty.Length];
			for (int i = 0; i < itemProperty.Length; i++)
			{
				this.ItemIds[i] = new ItemId(this.GetEwsId(itemProperty[i], mailboxGuid), null);
			}
			StoreId[] itemProperty2 = this.GetItemProperty<StoreId[]>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalItemIds, new StoreId[0]);
			this.GlobalItemIds = new ItemId[itemProperty2.Length];
			for (int j = 0; j < itemProperty2.Length; j++)
			{
				this.GlobalItemIds[j] = new ItemId(this.GetEwsId(itemProperty2[j], mailboxGuid), null);
			}
			this.LastModifiedTime = this.GetDateTimeProperty(propertyDefinitions, propertyValues, StoreObjectSchema.LastModifiedTime, timeZone);
			this.Preview = this.GetItemProperty<string>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationPreview);
			this.MailboxScopeString = MailboxSearchLocation.PrimaryOnly.ToString();
			IconIndex itemProperty3 = (IconIndex)this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationReplyForwardState);
			if (itemProperty3 > (IconIndex)0)
			{
				this.IconIndexString = itemProperty3.ToString();
			}
			itemProperty3 = (IconIndex)this.GetItemProperty<int>(propertyDefinitions, propertyValues, ConversationItemSchema.ConversationGlobalReplyForwardState);
			if (itemProperty3 > (IconIndex)0)
			{
				this.GlobalIconIndexString = itemProperty3.ToString();
			}
		}

		private T GetItemProperty<T>(PropertyDefinition[] propertyDefinitions, object[] propertyValues, PropertyDefinition propertyWanted)
		{
			return this.GetItemProperty<T>(propertyDefinitions, propertyValues, propertyWanted, default(T));
		}

		private T GetItemProperty<T>(PropertyDefinition[] propertyDefinitions, object[] propertyValues, PropertyDefinition propertyWanted, T defaultValue)
		{
			if (!this.IsPropertyDefined(propertyDefinitions, propertyValues, propertyWanted))
			{
				return defaultValue;
			}
			object obj = propertyValues[Array.IndexOf<PropertyDefinition>(propertyDefinitions, propertyWanted)];
			if (!(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		private bool IsPropertyDefined(PropertyDefinition[] propertyDefinitions, object[] propertyValues, PropertyDefinition propertyWanted)
		{
			int num = Array.IndexOf<PropertyDefinition>(propertyDefinitions, propertyWanted);
			return num >= 0 && num < propertyDefinitions.Length && propertyValues[num] != null && !(propertyValues[num] is PropertyError);
		}

		private string GetDateTimeProperty(PropertyDefinition[] propertyDefinitions, object[] propertyValues, PropertyDefinition propertyWanted, ExTimeZone timeZone)
		{
			ExDateTime itemProperty = this.GetItemProperty<ExDateTime>(propertyDefinitions, propertyValues, propertyWanted, ExDateTime.MinValue);
			if (ExDateTime.MinValue.Equals(itemProperty))
			{
				return null;
			}
			ExTimeZone exTimeZone = (timeZone == null || timeZone == ExTimeZone.UnspecifiedTimeZone) ? itemProperty.TimeZone : timeZone;
			if (exTimeZone == ExTimeZone.UtcTimeZone)
			{
				return ExDateTimeConverter.ToUtcXsdDateTime(itemProperty);
			}
			return ExDateTimeConverter.ToOffsetXsdDateTime(itemProperty, exTimeZone);
		}

		private string GetEwsId(StoreId storeId, Guid mailboxGuid)
		{
			if (storeId == null)
			{
				return null;
			}
			return StoreId.StoreIdToEwsId(mailboxGuid, storeId);
		}

		private MailboxSearchLocation mailboxScope;
	}
}

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(GetConversationItemsDiagnosticsRequest))]
	[XmlType(TypeName = "GetConversationItemsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(GetThreadedConversationItemsRequest))]
	[Serializable]
	public class GetConversationItemsRequest : BaseRequest
	{
		public GetConversationItemsRequest()
		{
			this.Init();
		}

		private void Init()
		{
			this.ItemShape = new ItemResponseShape();
			this.maxItemsToReturn = 100;
			this.returnSubmittedItems = false;
			this.returnModernConversationItems = false;
			this.SortOrder = ConversationNodeSortOrder.DateOrderDescending;
		}

		[DataMember(Name = "ItemShape", IsRequired = false)]
		[XmlElement]
		public ItemResponseShape ItemShape { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ShapeName", IsRequired = false)]
		public string ShapeName { get; set; }

		[DataMember(Name = "FoldersToIgnore", IsRequired = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public BaseFolderId[] FoldersToIgnore { get; set; }

		[XmlElement]
		[IgnoreDataMember]
		public ConversationNodeSortOrder SortOrder { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SortOrder", IsRequired = false)]
		public string SortOrderString
		{
			get
			{
				return EnumUtilities.ToString<ConversationNodeSortOrder>(this.SortOrder);
			}
			set
			{
				this.SortOrder = EnumUtilities.Parse<ConversationNodeSortOrder>(value);
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ReturnSubmittedItems", IsRequired = false)]
		public bool ReturnSubmittedItems
		{
			get
			{
				return this.returnSubmittedItems;
			}
			set
			{
				this.returnSubmittedItems = value;
			}
		}

		[DataMember(Name = "MaxItemsToReturn", IsRequired = false)]
		[XmlElement]
		public int MaxItemsToReturn
		{
			get
			{
				return this.maxItemsToReturn;
			}
			set
			{
				this.maxItemsToReturn = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool MailboxScopeSpecified { get; set; }

		[IgnoreDataMember]
		[XmlElement]
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

		[DataMember(Name = "MailboxScope", IsRequired = false)]
		[XmlIgnore]
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

		[XmlArrayItem("Conversation", typeof(ConversationRequestType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(Name = "Conversations", IsRequired = true)]
		public ConversationRequestType[] Conversations { get; set; }

		[DataMember(Name = "ReturnModernConversationItems", IsRequired = false)]
		[XmlIgnore]
		public bool ReturnModernConversationItems
		{
			get
			{
				return this.returnModernConversationItems;
			}
			set
			{
				this.returnModernConversationItems = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this.ReturnModernConversationItems)
			{
				return new GetModernConversationItems(callContext, this);
			}
			return new GetConversationItems(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, this.Conversations[0].ConversationId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.Conversations == null || taskStep < 0 || taskStep >= this.Conversations.Length)
			{
				return null;
			}
			return base.GetResourceKeysForItemId(false, callContext, this.Conversations[taskStep].ConversationId);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.FoldersToIgnore != null)
			{
				BaseFolderId[] foldersToIgnore = this.FoldersToIgnore;
				for (int i = 0; i < foldersToIgnore.Length; i++)
				{
					if (foldersToIgnore[i] == null)
					{
						throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorInvalidFolderId);
					}
				}
			}
			if (this.ReturnModernConversationItems && this.SortOrder != ConversationNodeSortOrder.DateOrderAscending && this.SortOrder != ConversationNodeSortOrder.DateOrderDescending)
			{
				throw new ServiceInvalidOperationException(CoreResources.ErrorInvalidParameter(this.SortOrder.ToString()), new ArgumentException("Only chronological sort orders are valid."));
			}
		}

		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}

		private const int DefaultMaxItemsToReturn = 100;

		private int maxItemsToReturn;

		private bool returnSubmittedItems;

		private bool returnModernConversationItems;

		private MailboxSearchLocation mailboxScope;
	}
}

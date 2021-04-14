using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("FindConversationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class FindConversationResponseMessage : ResponseMessage
	{
		public FindConversationResponseMessage()
		{
		}

		internal FindConversationResponseMessage(ServiceResultCode code, ServiceError error, ConversationType[] findConversationResults, HighlightTermType[] highlightTerms, int? totalConversationsInView, int? indexedOffset, bool isSearchInProgress) : base(code, error)
		{
			this.Conversations = findConversationResults;
			if (highlightTerms != null)
			{
				this.HighlightTerms = highlightTerms;
			}
			if (totalConversationsInView != null)
			{
				this.TotalConversationsInView = totalConversationsInView.Value;
			}
			else
			{
				this.TotalConversationsInViewSpecified = false;
			}
			if (indexedOffset != null)
			{
				this.IndexedOffset = indexedOffset.Value;
			}
			else
			{
				this.IndexedOffsetSpecified = false;
			}
			this.IsSearchInProgress = isSearchInProgress;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.FindConversationResponseMessage;
		}

		[XmlArray(ElementName = "Conversations")]
		[DataMember]
		[XmlArrayItem(ElementName = "Conversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public ConversationType[] Conversations { get; set; }

		[XmlArray(ElementName = "HighlightTerms")]
		[XmlArrayItem(ElementName = "Term", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "HighlightTerms", IsRequired = false, EmitDefaultValue = false)]
		public HighlightTermType[] HighlightTerms { get; set; }

		[XmlElement("TotalConversationsInView")]
		public int TotalConversationsInView
		{
			get
			{
				return this.totalConversationsInView;
			}
			set
			{
				this.TotalConversationsInViewSpecified = true;
				this.totalConversationsInView = value;
			}
		}

		[DataMember(Name = "TotalConversationsInView", EmitDefaultValue = false)]
		[XmlIgnore]
		public int? TotalConversationsInViewNullable
		{
			get
			{
				if (this.TotalConversationsInViewSpecified)
				{
					return new int?(this.TotalConversationsInView);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.TotalConversationsInView = value.Value;
					return;
				}
				this.TotalConversationsInViewSpecified = false;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool TotalConversationsInViewSpecified { get; set; }

		[XmlElement("IndexedOffset")]
		public int IndexedOffset
		{
			get
			{
				return this.indexedOffset;
			}
			set
			{
				this.IndexedOffsetSpecified = true;
				this.indexedOffset = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "IndexedOffset", EmitDefaultValue = false)]
		public int? IndexedOffsetNullable
		{
			get
			{
				if (this.IndexedOffsetSpecified)
				{
					return new int?(this.IndexedOffset);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.IndexedOffset = value.Value;
					return;
				}
				this.IndexedOffsetSpecified = false;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IndexedOffsetSpecified { get; set; }

		[DataMember(Name = "IsSearchInProgress", EmitDefaultValue = false)]
		[XmlIgnore]
		public bool IsSearchInProgress { get; set; }

		[XmlIgnore]
		[DataMember(Name = "SearchFolderId", EmitDefaultValue = false)]
		public FolderId SearchFolderId { get; set; }

		private int totalConversationsInView;

		private int indexedOffset;
	}
}

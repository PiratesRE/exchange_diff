using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MailboxMessageConfigurationOptions : OptionsPropertyChangeTracker
	{
		[DataMember]
		public AfterMoveOrDeleteBehavior AfterMoveOrDeleteBehavior
		{
			get
			{
				return this.afterMoveOrDeleteBehavior;
			}
			set
			{
				this.afterMoveOrDeleteBehavior = value;
				base.TrackPropertyChanged("AfterMoveOrDeleteBehavior");
			}
		}

		[DataMember]
		public bool AlwaysShowBcc
		{
			get
			{
				return this.alwaysShowBcc;
			}
			set
			{
				this.alwaysShowBcc = value;
				base.TrackPropertyChanged("AlwaysShowBcc");
			}
		}

		[DataMember]
		public bool AlwaysShowFrom
		{
			get
			{
				return this.alwaysShowFrom;
			}
			set
			{
				this.alwaysShowFrom = value;
				base.TrackPropertyChanged("AlwaysShowFrom");
			}
		}

		[DataMember]
		public bool AutoAddSignature
		{
			get
			{
				return this.autoAddSignature;
			}
			set
			{
				this.autoAddSignature = value;
				base.TrackPropertyChanged("AutoAddSignature");
			}
		}

		[DataMember]
		public bool AutoAddSignatureOnMobile
		{
			get
			{
				return this.autoAddSignatureOnMobile;
			}
			set
			{
				this.autoAddSignatureOnMobile = value;
				base.TrackPropertyChanged("AutoAddSignatureOnMobile");
			}
		}

		[DataMember]
		public bool CheckForForgottenAttachments
		{
			get
			{
				return this.checkForForgottenAttachments;
			}
			set
			{
				this.checkForForgottenAttachments = value;
				base.TrackPropertyChanged("CheckForForgottenAttachments");
			}
		}

		[DataMember]
		public ConversationSortOrder ConversationSortOrder
		{
			get
			{
				return this.conversationSortOrder;
			}
			set
			{
				this.conversationSortOrder = value;
				base.TrackPropertyChanged("ConversationSortOrder");
			}
		}

		[DataMember]
		public string DefaultFontColor
		{
			get
			{
				return this.defaultFontColor;
			}
			set
			{
				this.defaultFontColor = value;
				base.TrackPropertyChanged("DefaultFontColor");
			}
		}

		[DataMember]
		public FontFlags DefaultFontFlags
		{
			get
			{
				return this.defaultFontFlags;
			}
			set
			{
				this.defaultFontFlags = value;
				base.TrackPropertyChanged("DefaultFontFlags");
			}
		}

		[DataMember]
		public string DefaultFontName
		{
			get
			{
				return this.defaultFontName;
			}
			set
			{
				this.defaultFontName = value;
				base.TrackPropertyChanged("DefaultFontName");
			}
		}

		[DataMember]
		public int DefaultFontSize
		{
			get
			{
				return this.defaultFontSize;
			}
			set
			{
				this.defaultFontSize = value;
				base.TrackPropertyChanged("DefaultFontSize");
			}
		}

		[DataMember]
		public MailFormat DefaultFormat
		{
			get
			{
				return this.defaultFormat;
			}
			set
			{
				this.defaultFormat = value;
				base.TrackPropertyChanged("DefaultFormat");
			}
		}

		[DataMember]
		public EmailComposeMode EmailComposeMode
		{
			get
			{
				return this.emailComposeMode;
			}
			set
			{
				this.emailComposeMode = value;
				base.TrackPropertyChanged("EmailComposeMode");
			}
		}

		[DataMember]
		public bool EmptyDeletedItemsOnLogoff
		{
			get
			{
				return this.emptyDeletedItemsOnLogoff;
			}
			set
			{
				this.emptyDeletedItemsOnLogoff = value;
				base.TrackPropertyChanged("EmptyDeletedItemsOnLogoff");
			}
		}

		[DataMember]
		public bool HideDeletedItems
		{
			get
			{
				return this.hideDeletedItems;
			}
			set
			{
				this.hideDeletedItems = value;
				base.TrackPropertyChanged("HideDeletedItems");
			}
		}

		[DataMember]
		public NewItemNotification NewItemNotification
		{
			get
			{
				return this.newItemNotification;
			}
			set
			{
				this.newItemNotification = value;
				base.TrackPropertyChanged("NewItemNotification");
			}
		}

		[DataMember]
		public PreviewMarkAsReadBehavior PreviewMarkAsReadBehavior
		{
			get
			{
				return this.previewMarkAsReadBehavior;
			}
			set
			{
				this.previewMarkAsReadBehavior = value;
				base.TrackPropertyChanged("PreviewMarkAsReadBehavior");
			}
		}

		[DataMember]
		public int PreviewMarkAsReadDelaytime
		{
			get
			{
				return this.previewMarkAsReadDelayTime;
			}
			set
			{
				this.previewMarkAsReadDelayTime = value;
				base.TrackPropertyChanged("PreviewMarkAsReadDelaytime");
			}
		}

		[DataMember]
		public ReadReceiptResponse ReadReceiptResponse
		{
			get
			{
				return this.readReceiptResponse;
			}
			set
			{
				this.readReceiptResponse = value;
				base.TrackPropertyChanged("ReadReceiptResponse");
			}
		}

		[DataMember]
		public string SendAddressDefault
		{
			get
			{
				return this.sendAddressDefault;
			}
			set
			{
				this.sendAddressDefault = value;
				base.TrackPropertyChanged("SendAddressDefault");
			}
		}

		[DataMember]
		public bool ShowConversationAsTree
		{
			get
			{
				return this.showConversationAsTree;
			}
			set
			{
				this.showConversationAsTree = value;
				base.TrackPropertyChanged("ShowConversationAsTree");
			}
		}

		[DataMember]
		public string SignatureHtml
		{
			get
			{
				return this.signatureHtml;
			}
			set
			{
				this.signatureHtml = value;
				base.TrackPropertyChanged("SignatureHtml");
			}
		}

		[DataMember]
		public string SignatureText
		{
			get
			{
				return this.signatureText;
			}
			set
			{
				this.signatureText = value;
				base.TrackPropertyChanged("SignatureText");
			}
		}

		[DataMember]
		public string SignatureTextOnMobile
		{
			get
			{
				return this.signatureTextOnMobile;
			}
			set
			{
				this.signatureTextOnMobile = value;
				base.TrackPropertyChanged("SignatureTextOnMobile");
			}
		}

		[DataMember]
		public bool UseDefaultSignatureOnMobile
		{
			get
			{
				return this.useDefaultSignatureOnMobile;
			}
			set
			{
				this.useDefaultSignatureOnMobile = value;
				base.TrackPropertyChanged("UseDefaultSignatureOnMobile");
			}
		}

		private AfterMoveOrDeleteBehavior afterMoveOrDeleteBehavior;

		private bool alwaysShowBcc;

		private bool alwaysShowFrom;

		private bool autoAddSignature;

		private bool autoAddSignatureOnMobile;

		private bool checkForForgottenAttachments;

		private ConversationSortOrder conversationSortOrder;

		private string defaultFontColor;

		private FontFlags defaultFontFlags;

		private string defaultFontName;

		private int defaultFontSize;

		private MailFormat defaultFormat;

		private EmailComposeMode emailComposeMode;

		private bool emptyDeletedItemsOnLogoff;

		private bool hideDeletedItems;

		private NewItemNotification newItemNotification;

		private PreviewMarkAsReadBehavior previewMarkAsReadBehavior;

		private int previewMarkAsReadDelayTime;

		private ReadReceiptResponse readReceiptResponse;

		private bool showConversationAsTree;

		private string sendAddressDefault;

		private string signatureHtml;

		private string signatureText;

		private string signatureTextOnMobile;

		private bool useDefaultSignatureOnMobile;
	}
}

using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxMessageConfiguration : XsoMailboxConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return MailboxMessageConfiguration.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		[Parameter(Mandatory = false)]
		public AfterMoveOrDeleteBehavior AfterMoveOrDeleteBehavior
		{
			get
			{
				return (AfterMoveOrDeleteBehavior)this[MailboxMessageConfigurationSchema.AfterMoveOrDeleteBehavior];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.AfterMoveOrDeleteBehavior] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NewItemNotification NewItemNotification
		{
			get
			{
				return (NewItemNotification)this[MailboxMessageConfigurationSchema.NewItemNotification];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.NewItemNotification] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EmptyDeletedItemsOnLogoff
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.EmptyDeletedItemsOnLogoff];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.EmptyDeletedItemsOnLogoff] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoAddSignature
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.AutoAddSignature];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.AutoAddSignature] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SignatureText
		{
			get
			{
				return (string)this[MailboxMessageConfigurationSchema.SignatureText];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.SignatureText] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SignatureHtml
		{
			get
			{
				return (string)this[MailboxMessageConfigurationSchema.SignatureHtml];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.SignatureHtml] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoAddSignatureOnMobile
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.AutoAddSignatureOnMobile];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.AutoAddSignatureOnMobile] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SignatureTextOnMobile
		{
			get
			{
				return (string)this[MailboxMessageConfigurationSchema.SignatureTextOnMobile];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.SignatureTextOnMobile] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseDefaultSignatureOnMobile
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.UseDesktopSignature];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.UseDesktopSignature] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DefaultFontName
		{
			get
			{
				return (string)this[MailboxMessageConfigurationSchema.DefaultFontName];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.DefaultFontName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int DefaultFontSize
		{
			get
			{
				return (int)this[MailboxMessageConfigurationSchema.DefaultFontSize];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.DefaultFontSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DefaultFontColor
		{
			get
			{
				return (string)this[MailboxMessageConfigurationSchema.DefaultFontColor];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.DefaultFontColor] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FontFlags DefaultFontFlags
		{
			get
			{
				return (FontFlags)this[MailboxMessageConfigurationSchema.DefaultFontFlags];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.DefaultFontFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AlwaysShowBcc
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.AlwaysShowBcc];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.AlwaysShowBcc] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AlwaysShowFrom
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.AlwaysShowFrom];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.AlwaysShowFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailFormat DefaultFormat
		{
			get
			{
				return (MailFormat)this[MailboxMessageConfigurationSchema.DefaultFormat];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.DefaultFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ReadReceiptResponse ReadReceiptResponse
		{
			get
			{
				return (ReadReceiptResponse)this[MailboxMessageConfigurationSchema.ReadReceiptResponse];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.ReadReceiptResponse] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PreviewMarkAsReadBehavior PreviewMarkAsReadBehavior
		{
			get
			{
				return (PreviewMarkAsReadBehavior)this[MailboxMessageConfigurationSchema.PreviewMarkAsReadBehavior];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.PreviewMarkAsReadBehavior] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PreviewMarkAsReadDelaytime
		{
			get
			{
				return (int)this[MailboxMessageConfigurationSchema.PreviewMarkAsReadDelaytime];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.PreviewMarkAsReadDelaytime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ConversationSortOrder ConversationSortOrder
		{
			get
			{
				return (ConversationSortOrder)this[MailboxMessageConfigurationSchema.ConversationSortOrder];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.ConversationSortOrder] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ShowConversationAsTree
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.ShowConversationAsTree];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.ShowConversationAsTree] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HideDeletedItems
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.HideDeletedItems];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.HideDeletedItems] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SendAddressDefault
		{
			get
			{
				return (string)this[MailboxMessageConfigurationSchema.SendAddressDefault];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.SendAddressDefault] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EmailComposeMode EmailComposeMode
		{
			get
			{
				return (EmailComposeMode)this[MailboxMessageConfigurationSchema.EmailComposeMode];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.EmailComposeMode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CheckForForgottenAttachments
		{
			get
			{
				return (bool)this[MailboxMessageConfigurationSchema.CheckForForgottenAttachments];
			}
			set
			{
				this[MailboxMessageConfigurationSchema.CheckForForgottenAttachments] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			int num = (this.SignatureHtml == null) ? 0 : this.SignatureHtml.Length;
			num += ((this.SignatureText == null) ? 0 : this.SignatureText.Length);
			if (num > 8000)
			{
				errors.Add(new ObjectValidationError(ServerStrings.ErrorSigntureTooLarge, this.Identity, num.ToString()));
			}
		}

		internal const int MaxSignatureSize = 8000;

		internal const int MaxSignatureSizeOnMobile = 512;

		private static MailboxMessageConfigurationSchema schema = ObjectSchema.GetInstance<MailboxMessageConfigurationSchema>();
	}
}

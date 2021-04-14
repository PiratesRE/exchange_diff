using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class SharingMessageProvider
	{
		[XmlAttribute]
		public string Type { get; set; }

		[XmlAttribute]
		public string TargetRecipients { get; set; }

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/sharing/2008", IsNullable = false)]
		public string FolderId { get; set; }

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/sharing/2008", IsNullable = false)]
		public string MailboxId { get; set; }

		[XmlArrayItem(ElementName = "EncryptedSharedFolderData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IsNullable = false)]
		public EncryptedSharedFolderData[] EncryptedSharedFolderDataCollection { get; set; }

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/sharing/2008", IsNullable = false)]
		public string BrowseUrl { get; set; }

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/sharing/2008", IsNullable = false)]
		public string ICalUrl { get; set; }

		internal bool IsExchangeInternalProvider
		{
			get
			{
				return this.Type == "ms-exchange-internal";
			}
		}

		internal bool IsExchangeExternalProvider
		{
			get
			{
				return this.Type == "ms-exchange-external";
			}
		}

		internal bool IsExchangePubCalProvider
		{
			get
			{
				return this.Type == "ms-exchange-publish";
			}
		}

		internal ValidationResults Validate(SharingMessageKind sharingMessageKind)
		{
			string type;
			if ((type = this.Type) != null)
			{
				if (!(type == "ms-exchange-internal") && !(type == "ms-exchange-external"))
				{
					if (type == "ms-exchange-publish")
					{
						if (sharingMessageKind == SharingMessageKind.Invitation)
						{
							return this.ValidateAsPublishing();
						}
						return new ValidationResults(ValidationResult.Failure, "Unexpected sharing message kind: " + sharingMessageKind.ToString());
					}
				}
				else
				{
					switch (sharingMessageKind)
					{
					case SharingMessageKind.Invitation:
					case SharingMessageKind.AcceptOfRequest:
						return this.ValidateAsInvitationOrAcceptOfRequest();
					case SharingMessageKind.Request:
					case SharingMessageKind.DenyOfRequest:
						return this.ValidateAsRequestOrDenyOfRequest();
					default:
						return new ValidationResults(ValidationResult.Unknown, "unknown sharing message kind: " + sharingMessageKind.ToString());
					}
				}
			}
			return new ValidationResults(ValidationResult.Unknown, "unknown provider type: " + this.Type);
		}

		private ValidationResults ValidateAsRequestOrDenyOfRequest()
		{
			if (this.FolderId == null && this.MailboxId == null && this.EncryptedSharedFolderDataCollection == null)
			{
				return ValidationResults.Success;
			}
			return new ValidationResults(ValidationResult.Failure, "FolderId, MailboxId or EncryptedSharedFolderDataCollection not expected");
		}

		private ValidationResults ValidateAsInvitationOrAcceptOfRequest()
		{
			if (string.IsNullOrEmpty(this.FolderId))
			{
				return new ValidationResults(ValidationResult.Failure, "FolderId is expected");
			}
			if (this.MailboxId == null && this.EncryptedSharedFolderDataCollection == null)
			{
				return new ValidationResults(ValidationResult.Failure, "EncryptedSharedFolderDataCollection or MailboxId expected");
			}
			if (this.MailboxId != null && this.EncryptedSharedFolderDataCollection != null)
			{
				return new ValidationResults(ValidationResult.Failure, "EncryptedSharedFolderDataCollection and MailboxId are not allowed together");
			}
			return ValidationResults.Success;
		}

		private ValidationResults ValidateAsPublishing()
		{
			if (string.IsNullOrEmpty(this.BrowseUrl))
			{
				return new ValidationResults(ValidationResult.Failure, "BrowseUrl is expected");
			}
			if (this.FolderId != null || this.MailboxId != null || this.EncryptedSharedFolderDataCollection != null)
			{
				return new ValidationResults(ValidationResult.Failure, "FolderId, MailboxId or EncryptedSharedFolderDataCollection not expected");
			}
			return ValidationResults.Success;
		}

		internal const string ExchangeInternal = "ms-exchange-internal";

		internal const string ExchangeExternal = "ms-exchange-external";

		internal const string ExchangePubCal = "ms-exchange-publish";

		internal const string ExchangeConsumer = "ms-exchange-consumer";
	}
}

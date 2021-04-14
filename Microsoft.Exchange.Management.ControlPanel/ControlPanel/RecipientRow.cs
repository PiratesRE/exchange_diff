using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(RecipientRow))]
	public class RecipientRow : BaseRow
	{
		public RecipientRow(ReducedRecipient recipient) : base(recipient.ToIdentity(), recipient)
		{
			this.PrimarySmtpAddress = recipient.PrimarySmtpAddress.ToString();
			this.SpriteId = Icons.FromEnum(recipient.RecipientTypeDetails);
			this.RecipientTypeDetails = recipient.RecipientTypeDetails.ToString();
			this.LocRecipientTypeDetails = LocalizedDescriptionAttribute.FromEnum(typeof(RecipientTypeDetails), recipient.RecipientTypeDetails);
		}

		public RecipientRow(MailEnabledRecipient recipient) : base(recipient.ToIdentity(), recipient)
		{
			this.PrimarySmtpAddress = recipient.PrimarySmtpAddress.ToString();
			this.SpriteId = Icons.FromEnum(recipient.RecipientTypeDetails);
			this.RecipientTypeDetails = recipient.RecipientTypeDetails.ToString();
			this.LocRecipientTypeDetails = LocalizedDescriptionAttribute.FromEnum(typeof(RecipientTypeDetails), recipient.RecipientTypeDetails);
		}

		public RecipientRow(WindowsGroup group) : base(group.Id.ToIdentity(group.DisplayName), group)
		{
			this.PrimarySmtpAddress = group.WindowsEmailAddress.ToStringWithNull();
		}

		[DataMember]
		public string RecipientTypeDetails { get; protected set; }

		[DataMember]
		public string LocRecipientTypeDetails { get; protected set; }

		[DataMember]
		public string DisplayName
		{
			get
			{
				return base.Identity.DisplayName;
			}
			private set
			{
			}
		}

		[DataMember]
		public string PrimarySmtpAddress { get; protected set; }

		[DataMember]
		public string SpriteId { get; protected set; }
	}
}

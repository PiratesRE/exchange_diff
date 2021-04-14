using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecipientPickerObject : BaseRow
	{
		public RecipientPickerObject(ReducedRecipient recipient) : base(recipient.ToIdentity(), recipient)
		{
			this.Recipient = recipient;
		}

		public ReducedRecipient Recipient { get; private set; }

		[DataMember]
		public virtual string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.Recipient.DisplayName))
				{
					return this.Recipient.DisplayName;
				}
				return this.Recipient.Name;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public virtual string Alias
		{
			get
			{
				return this.Recipient.Alias;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public virtual string RecipientType
		{
			get
			{
				return this.Recipient.RecipientType.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public virtual string PrimarySmtpAddress
		{
			get
			{
				return this.Recipient.PrimarySmtpAddress.ToString();
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public virtual string DistinguishedName
		{
			get
			{
				return this.Recipient.DistinguishedName;
			}
			protected set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SpriteId
		{
			get
			{
				return Icons.FromEnum(this.Recipient.RecipientTypeDetails);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string IconAltText
		{
			get
			{
				return Icons.GenerateIconAltText(this.Recipient.RecipientTypeDetails);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}

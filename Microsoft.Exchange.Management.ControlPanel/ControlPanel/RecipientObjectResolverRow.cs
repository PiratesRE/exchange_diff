using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecipientObjectResolverRow : AdObjectResolverRow
	{
		public RecipientObjectResolverRow(ADRawEntry aDRawEntry) : base(aDRawEntry)
		{
		}

		public override string DisplayName
		{
			get
			{
				string text = (string)base.ADRawEntry[ADRecipientSchema.DisplayName];
				if (string.IsNullOrEmpty(text))
				{
					text = base.DisplayName;
				}
				return text;
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)base.ADRawEntry[ADRecipientSchema.RecipientTypeDetails];
			}
		}

		[DataMember]
		public string PrimarySmtpAddress
		{
			get
			{
				return ((SmtpAddress)base.ADRawEntry[ADRecipientSchema.PrimarySmtpAddress]).ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SpriteId
		{
			get
			{
				return Icons.FromEnum((RecipientTypeDetails)base.ADRawEntry[ADRecipientSchema.RecipientTypeDetails]);
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
				return Icons.GenerateIconAltText((RecipientTypeDetails)base.ADRawEntry[ADRecipientSchema.RecipientTypeDetails]);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			RecipientObjectResolverRow recipientObjectResolverRow = obj as RecipientObjectResolverRow;
			return recipientObjectResolverRow != null && (string.Equals(this.DisplayName, recipientObjectResolverRow.DisplayName) && string.Equals(this.PrimarySmtpAddress, recipientObjectResolverRow.PrimarySmtpAddress) && string.Equals(this.IconAltText, recipientObjectResolverRow.IconAltText) && string.Equals(this.SpriteId, recipientObjectResolverRow.SpriteId)) && this.RecipientTypeDetails == recipientObjectResolverRow.RecipientTypeDetails;
		}

		public override int GetHashCode()
		{
			return this.PrimarySmtpAddress.GetHashCode();
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(AdObjectResolverRow.Properties)
		{
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.RecipientTypeDetails
		}.ToArray();
	}
}

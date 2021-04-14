using System;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class Recipient : PrivatePropertyBag
	{
		internal Recipient(RecipientCollection parentCollection) : base(false)
		{
			this.parentCollection = parentCollection;
			PropertySchemaPopulation.InitializeRecipient(this.CurrentOperationContext, this);
		}

		internal Recipient(RecipientCollection parentCollection, byte[] propertyBlob) : this(parentCollection)
		{
			base.LoadFromPropertyBlob(this.CurrentOperationContext, propertyBlob);
		}

		public override ObjectPropertySchema Schema
		{
			get
			{
				return this.parentCollection.RecipientSchema;
			}
		}

		public override Context CurrentOperationContext
		{
			get
			{
				return this.parentCollection.ParentMessage.Mailbox.CurrentOperationContext;
			}
		}

		public override ReplidGuidMap ReplidGuidMap
		{
			get
			{
				return null;
			}
		}

		public DisplayType DisplayType
		{
			get
			{
				object propertyValue = this.GetPropertyValue(this.CurrentOperationContext, PropTag.Recipient.DisplayType);
				if (propertyValue == null)
				{
					return DisplayType.DT_MAILUSER;
				}
				return (DisplayType)((int)propertyValue);
			}
			set
			{
				this.SetProperty(this.CurrentOperationContext, PropTag.Recipient.DisplayType, (value == DisplayType.DT_MAILUSER) ? null : ((int)value));
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				object propertyValue = this.GetPropertyValue(this.CurrentOperationContext, PropTag.Recipient.ObjectType);
				if (propertyValue == null)
				{
					return ObjectType.MAPI_MAILUSER;
				}
				return (ObjectType)((int)propertyValue);
			}
			set
			{
				this.SetProperty(this.CurrentOperationContext, PropTag.Recipient.ObjectType, (value == ObjectType.MAPI_MAILUSER) ? null : ((int)value));
			}
		}

		public string Name
		{
			get
			{
				return (string)this.GetPropertyValue(this.CurrentOperationContext, PropTag.Recipient.DisplayName);
			}
			set
			{
				this.SetProperty(this.CurrentOperationContext, PropTag.Recipient.DisplayName, value);
			}
		}

		public string Email
		{
			get
			{
				return (string)this.GetPropertyValue(this.CurrentOperationContext, PropTag.Recipient.EmailAddress);
			}
			set
			{
				this.SetProperty(this.CurrentOperationContext, PropTag.Recipient.EmailAddress, value);
			}
		}

		public string LegacyDn
		{
			get
			{
				return (string)this.GetPropertyValue(this.CurrentOperationContext, PropTag.Recipient.UserDN);
			}
			set
			{
				this.SetProperty(this.CurrentOperationContext, PropTag.Recipient.UserDN, value);
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				object propertyValue = this.GetPropertyValue(this.CurrentOperationContext, PropTag.Recipient.RecipientType);
				if (propertyValue == null)
				{
					return RecipientType.To;
				}
				return (RecipientType)((int)propertyValue);
			}
			set
			{
				if (value != this.RecipientType)
				{
					this.SetProperty(this.CurrentOperationContext, PropTag.Recipient.RecipientType, (int)value);
				}
			}
		}

		public int RowId
		{
			get
			{
				return this.rowId;
			}
			set
			{
				this.rowId = value;
			}
		}

		public void Delete()
		{
			this.parentCollection.Remove(this);
		}

		public void ToBinary(Context context, out byte[] propertyBlob)
		{
			propertyBlob = base.SaveToPropertyBlob(context);
		}

		protected void OnDelete()
		{
			this.InvalidateMessageComputedProperty(this.RecipientType);
		}

		internal void InvalidateMessageComputedProperty(RecipientType type)
		{
			Message parentMessage = this.parentCollection.ParentMessage;
			switch (type)
			{
			case RecipientType.SubmittedTo:
			case RecipientType.SubmittedCc:
			case RecipientType.SubmittedBcc:
				break;
			default:
				switch (type)
				{
				case RecipientType.Orig:
					break;
				case RecipientType.To:
					parentMessage.MarkComputedPropertyAsInvalid(PropTag.Message.DisplayTo);
					return;
				case RecipientType.Cc:
					parentMessage.MarkComputedPropertyAsInvalid(PropTag.Message.DisplayCc);
					return;
				case RecipientType.Bcc:
					parentMessage.MarkComputedPropertyAsInvalid(PropTag.Message.DisplayBcc);
					break;
				default:
					if (type != RecipientType.P1)
					{
						return;
					}
					break;
				}
				break;
			}
		}

		protected override void OnDirty(Context context)
		{
			base.OnDirty(context);
		}

		protected override void OnPropertyChanged(StorePropTag propTag, long deltaSize)
		{
			this.parentCollection.Changed = true;
		}

		protected override StorePropTag MapPropTag(Context context, uint propertyTag)
		{
			return this.parentCollection.ParentMessage.Mailbox.GetStorePropTag(context, propertyTag, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient);
		}

		private RecipientCollection parentCollection;

		private int rowId;
	}
}

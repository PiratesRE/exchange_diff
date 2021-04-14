using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal abstract class XsoRecipientProperty : XsoStringProperty
	{
		internal RecipientItemType RecipientItemType { get; private set; }

		public XsoRecipientProperty(RecipientItemType recipientType, PropertyType type) : base(null, type)
		{
			this.RecipientItemType = recipientType;
		}

		public override string StringData
		{
			get
			{
				MessageItem messageItem = base.XsoItem as MessageItem;
				string text = null;
				try
				{
					if (messageItem != null)
					{
						text = EmailAddressConverter.GetRecipientString(messageItem.Recipients, this.RecipientItemType, messageItem.Session.MailboxOwner);
						AirSyncDiagnostics.TraceDebug<RecipientItemType, string>(ExTraceGlobals.XsoTracer, this, "value for Message '{0}':{1}", this.RecipientItemType, text);
					}
				}
				catch (PropertyErrorException ex)
				{
					if (ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
					{
						messageItem.Load();
						text = EmailAddressConverter.GetRecipientString(messageItem.Recipients, this.RecipientItemType, messageItem.Session.MailboxOwner);
						AirSyncDiagnostics.TraceDebug<RecipientItemType, string>(ExTraceGlobals.XsoTracer, this, "After calling Load- value for Message '{0}':{1}", this.RecipientItemType, text);
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					base.State = PropertyState.SetToDefault;
				}
				return text;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			MessageItem messageItem = (MessageItem)base.XsoItem;
			EmailAddressConverter.SetRecipientCollection(messageItem.Recipients, this.RecipientItemType, ((IStringProperty)srcProperty).StringData);
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			if (Command.CurrentCommand.Request.Version >= 160)
			{
				MessageItem messageItem = (MessageItem)base.XsoItem;
				EmailAddressConverter.ClearRecipients(messageItem.Recipients, this.RecipientItemType);
				return;
			}
			base.InternalSetToDefault(srcProperty);
		}
	}
}

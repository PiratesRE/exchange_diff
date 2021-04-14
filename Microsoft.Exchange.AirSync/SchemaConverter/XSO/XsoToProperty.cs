using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoToProperty : XsoRecipientProperty
	{
		public XsoToProperty() : base(RecipientItemType.To, PropertyType.ReadWrite)
		{
		}

		public XsoToProperty(PropertyType type) : base(RecipientItemType.To, type)
		{
		}

		public override string StringData
		{
			get
			{
				MessageItem messageItem = base.XsoItem as MessageItem;
				string text = null;
				if (messageItem != null)
				{
					if (!ObjectClass.IsReport(messageItem.ClassName))
					{
						try
						{
							text = EmailAddressConverter.GetRecipientString(messageItem.Recipients, RecipientItemType.To, messageItem.Session.MailboxOwner);
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "'To' loaded using message.Recipients :{0}.", text);
							goto IL_E8;
						}
						catch (PropertyErrorException ex)
						{
							if (ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
							{
								messageItem.Load();
								text = EmailAddressConverter.GetRecipientString(messageItem.Recipients, RecipientItemType.To, messageItem.Session.MailboxOwner);
							}
							goto IL_E8;
						}
					}
					if (messageItem.Sender != null)
					{
						try
						{
							text = EmailAddressConverter.GetParticipantString(messageItem.Sender, messageItem.Session.MailboxOwner);
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "'To' loaded using message.Sender :{0}.", text);
						}
						catch (PropertyErrorException ex2)
						{
							if (ex2.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
							{
								messageItem.Load();
								text = EmailAddressConverter.GetParticipantString(messageItem.Sender, messageItem.Session.MailboxOwner);
							}
						}
					}
				}
				IL_E8:
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
			if (!string.IsNullOrEmpty(((IStringProperty)srcProperty).StringData))
			{
				EmailAddressConverter.SetRecipientCollection(messageItem.Recipients, RecipientItemType.To, ((IStringProperty)srcProperty).StringData);
			}
		}
	}
}

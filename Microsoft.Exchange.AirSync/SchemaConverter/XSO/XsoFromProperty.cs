using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoFromProperty : XsoStringProperty
	{
		public XsoFromProperty() : base(null)
		{
		}

		public XsoFromProperty(PropertyType type) : base(null, type)
		{
		}

		public override string StringData
		{
			get
			{
				MessageItem messageItem = base.XsoItem as MessageItem;
				string text = null;
				if (messageItem == null)
				{
					PostItem postItem = base.XsoItem as PostItem;
					if (postItem == null)
					{
						throw new ConversionException("Should not find items other than messages and posts in Mail folders");
					}
					try
					{
						if (postItem.From != null)
						{
							text = EmailAddressConverter.GetParticipantString(postItem.From, postItem.Session.MailboxOwner);
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "PostItem 'From' : {0}.", text);
						}
						goto IL_125;
					}
					catch (PropertyErrorException ex)
					{
						if (ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
						{
							postItem.Load();
							text = EmailAddressConverter.GetParticipantString(postItem.From, postItem.Session.MailboxOwner);
							AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "After calling Load-PostItem 'From' : {0}.", text);
						}
						goto IL_125;
					}
				}
				try
				{
					if (messageItem.From != null)
					{
						text = EmailAddressConverter.GetParticipantString(messageItem.From, messageItem.Session.MailboxOwner);
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "MessageItem 'From' : {0}.", text);
					}
				}
				catch (PropertyErrorException ex2)
				{
					if (ex2.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
					{
						messageItem.Load();
						text = EmailAddressConverter.GetParticipantString(messageItem.From, messageItem.Session.MailboxOwner);
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "After calling Load-MessageItem 'From' : {0}.", text);
					}
				}
				IL_125:
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
				messageItem.From = EmailAddressConverter.CreateParticipant(((IStringProperty)srcProperty).StringData);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoReplyToProperty : XsoStringProperty
	{
		public XsoReplyToProperty(PropertyType type) : base(null, type)
		{
		}

		public override string StringData
		{
			get
			{
				MessageItem messageItem = base.XsoItem as MessageItem;
				StringBuilder stringBuilder = new StringBuilder();
				if (messageItem != null)
				{
					bool flag = true;
					IList<Participant> list = null;
					try
					{
						list = messageItem.ReplyTo;
					}
					catch (PropertyErrorException ex)
					{
						if (ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
						{
							messageItem.Load();
							list = messageItem.ReplyTo;
						}
					}
					foreach (Participant participant in list)
					{
						string participantString = EmailAddressConverter.GetParticipantString(participant, messageItem.Session.MailboxOwner);
						if (!flag)
						{
							stringBuilder.Append(";");
						}
						else
						{
							flag = false;
						}
						stringBuilder.Append(participantString);
					}
				}
				string text = stringBuilder.ToString();
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.XsoTracer, this, "ReplyTo list {0}.", text);
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
			messageItem.ReplyTo.Clear();
			IStringProperty stringProperty = (IStringProperty)srcProperty;
			string stringData = stringProperty.StringData;
			char[] separator = new char[]
			{
				',',
				';'
			};
			foreach (string text in stringData.Split(separator))
			{
				if (!string.IsNullOrEmpty(text))
				{
					messageItem.ReplyTo.Add(EmailAddressConverter.CreateParticipant(text));
				}
			}
		}
	}
}

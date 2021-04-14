using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.SignalApiEx
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SignalFeederEx : AbstractSignalFeeder
	{
		public SignalFeederEx(StoreSession session, StoreId folderId)
		{
			this.session = session;
			this.folderId = folderId;
		}

		internal override void SendMail(Signal signal, List<string> recipients)
		{
			using (MessageItem messageItem = MessageItem.Create(this.session, this.folderId))
			{
				messageItem.ClassName = "IPM.WorkingSet.Signal";
				foreach (string text in recipients)
				{
					Participant participant = new Participant(text, text, "EX");
					messageItem.Recipients.Add(participant, RecipientItemType.To);
				}
				Packer.Pack(signal, messageItem);
				messageItem.SendWithoutSavingMessage();
			}
		}

		private readonly StoreSession session;

		private readonly StoreId folderId;
	}
}

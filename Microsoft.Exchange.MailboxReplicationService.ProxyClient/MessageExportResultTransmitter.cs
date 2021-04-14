using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MessageExportResultTransmitter
	{
		public MessageExportResultTransmitter(IDataImport destination, bool clientIsDownlevel)
		{
			this.destination = destination;
			this.clientIsDownlevel = clientIsDownlevel;
		}

		public void SendMessageExportResults(List<BadMessageRec> badMessages)
		{
			List<MessageRec> list = null;
			if (this.clientIsDownlevel)
			{
				list = new List<MessageRec>();
				List<BadMessageRec> list2 = new List<BadMessageRec>();
				foreach (BadMessageRec badMessageRec in badMessages)
				{
					if (badMessageRec.Kind == BadItemKind.MissingItem)
					{
						list.Add(new MessageRec
						{
							EntryId = badMessageRec.EntryId,
							FolderId = badMessageRec.FolderId
						});
					}
					else
					{
						badMessageRec.XmlData = badMessageRec.Serialize(false);
						list2.Add(badMessageRec);
					}
				}
				badMessages = list2;
			}
			this.destination.SendMessage(new MessageExportResultsMessage(list, badMessages));
		}

		private IDataImport destination;

		private bool clientIsDownlevel;
	}
}

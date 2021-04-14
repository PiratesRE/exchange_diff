using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MessageExportResultReceiver : DisposableWrapper<IDataImport>, IDataImport, IDisposable
	{
		public MessageExportResultReceiver(IDataImport destination, bool ownsDestination) : base(destination, ownsDestination)
		{
			this.MissingMessages = new List<MessageRec>();
			this.BadMessages = new List<BadMessageRec>();
		}

		public List<MessageRec> MissingMessages { get; private set; }

		public List<BadMessageRec> BadMessages { get; private set; }

		IDataMessage IDataImport.SendMessageAndWaitForReply(IDataMessage message)
		{
			return base.WrappedObject.SendMessageAndWaitForReply(message);
		}

		void IDataImport.SendMessage(IDataMessage message)
		{
			MessageExportResultsMessage messageExportResultsMessage = message as MessageExportResultsMessage;
			if (messageExportResultsMessage == null)
			{
				base.WrappedObject.SendMessage(message);
				return;
			}
			this.MissingMessages = messageExportResultsMessage.MissingMessages;
			this.BadMessages = messageExportResultsMessage.BadMessages;
			foreach (BadMessageRec badMessageRec in this.BadMessages)
			{
				badMessageRec.Failure = FailureRec.Create(new RemotePermanentException(new LocalizedString(badMessageRec.XmlData), null));
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IMessagingDatabase
	{
		DataSource DataSource { get; }

		ServerInfoTable ServerInfoTable { get; }

		QueueTable QueueTable { get; }

		string CurrentState { get; }

		void SuspendDataCleanup();

		void ResumeDataCleanup();

		void BootLoadCompleted();

		IMailRecipientStorage NewRecipientStorage(long messageId);

		IMailItemStorage NewMailItemStorage(bool loadDefaults);

		IMailItemStorage LoadMailItemFromId(long msgId);

		IEnumerable<IMailRecipientStorage> LoadMailRecipientsFromMessageId(long messageId);

		IMailRecipientStorage LoadMailRecipientFromId(long recipientId);

		IReplayRequest NewReplayRequest(Guid correlationId, Destination destination, DateTime startTime, DateTime endTime, bool isTestRequest = false);

		IEnumerable<IReplayRequest> GetAllReplayRequests();

		Transaction BeginNewTransaction();

		void Attach(IMessagingDatabaseConfig config);

		void Detach();

		XElement GetDiagnosticInfo(DiagnosableParameters parameters);

		IEnumerable<MailItemAndRecipients> GetMessages(List<long> messageIds);

		MessagingDatabaseResultStatus ReadUnprocessedMessageIds(out Dictionary<byte, List<long>> unprocessedMessageIds);

		void Start();
	}
}

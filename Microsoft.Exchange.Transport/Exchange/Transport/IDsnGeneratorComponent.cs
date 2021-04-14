using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport
{
	internal interface IDsnGeneratorComponent : ITransportComponent
	{
		DsnMailOutHandlerDelegate DsnMailOutHandler { get; set; }

		DsnHumanReadableWriter DsnHumanReadableWriter { get; }

		QuarantineConfig QuarantineConfig { get; }

		void GenerateDSNs(IReadOnlyMailItem mailItem);

		void GenerateDSNs(IReadOnlyMailItem mailItem, DsnGenerator.CallerComponent callerComponent);

		void GenerateDSNs(IReadOnlyMailItem mailItem, string remoteServer, DsnGenerator.CallerComponent callerComponent);

		void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList);

		void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList, string remoteServer);

		void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList, LastError lastQueueLevelError);

		void GenerateDSNs(IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientList, string remoteServer, DsnGenerator.CallerComponent callerComponent, LastError lastQueueLevelError);

		void GenerateNDRForInvalidAddresses(bool switchPoisonContext, IReadOnlyMailItem mailItem, List<DsnRecipientInfo> recipientList);

		void GenerateNDRForInvalidAddresses(bool switchPoisonContext, IReadOnlyMailItem mailItem, List<DsnRecipientInfo> recipientList, DsnMailOutHandlerDelegate dsnMailOutHandler);

		void DecorateStoreNdr(TransportMailItem transportMessage, RoutingAddress ndrRecipient);

		void MonitorJobs();

		void DecorateMfn(TransportMailItem transportMessage, string displayName, string emailAddress);

		void CreateStoreQuotaMessageBody(TransportMailItem transportMailItem, QuotaType quotaType, string quotaMessageClass, int? localeId, int currentSize, int? maxSize, string folderName, bool isPrimaryMailbox, bool isPublicFolderMailbox);
	}
}

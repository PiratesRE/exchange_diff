using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IMailItemStorage
	{
		bool IsNew { get; }

		bool IsDeleted { get; }

		bool IsActive { get; set; }

		bool IsReadOnly { get; set; }

		bool PendingDatabaseUpdates { get; }

		bool IsInAsyncCommit { get; }

		long MsgId { get; }

		string FromAddress { get; set; }

		string AttributedFromAddress { get; set; }

		DateTime DateReceived { get; set; }

		TimeSpan ExtensionToExpiryDuration { get; set; }

		DsnFormat DsnFormat { get; set; }

		bool IsDiscardPending { get; set; }

		MailDirectionality Directionality { get; set; }

		string HeloDomain { get; set; }

		string Auth { get; set; }

		string EnvId { get; set; }

		BodyType BodyType { get; set; }

		string Oorg { get; set; }

		string ReceiveConnectorName { get; set; }

		int PoisonCount { get; set; }

		IPAddress SourceIPAddress { get; set; }

		string Subject { get; set; }

		string InternetMessageId { get; set; }

		Guid ShadowMessageId { get; set; }

		string ShadowServerContext { get; set; }

		string ShadowServerDiscardId { get; set; }

		IDataExternalComponent Recipients { get; set; }

		int Scl { get; set; }

		string PerfCounterAttribution { get; set; }

		IExtendedPropertyCollection ExtendedProperties { get; }

		XExch50Blob XExch50Blob { get; }

		LazyBytes FastIndexBlob { get; }

		bool IsJournalReport { get; }

		List<string> MoveToHosts { get; set; }

		bool RetryDeliveryIfRejected { get; set; }

		MultiValueHeader TransportPropertiesHeader { get; }

		DeliveryPriority? Priority { get; set; }

		DeliveryPriority BootloadingPriority { get; set; }

		string PrioritizationReason { get; set; }

		Guid NetworkMessageId { get; set; }

		Guid SystemProbeId { get; set; }

		RiskLevel RiskLevel { get; set; }

		MimeDocument MimeDocument { get; set; }

		EmailMessage Message { get; }

		string MimeFrom { get; set; }

		string MimeSenderAddress { get; set; }

		bool MimeNotSequential { get; set; }

		bool FallbackToRawOverride { get; set; }

		Encoding MimeDefaultEncoding { get; set; }

		bool MimeWriteStreamOpen { get; }

		long MimeSize { get; set; }

		MimePart RootPart { get; }

		Guid ExternalOrganizationId { get; set; }

		string ExoAccountForest { get; set; }

		string ExoTenantContainer { get; set; }

		string ProbeName { get; set; }

		bool PersistProbeTrace { get; set; }

		Stream OpenMimeReadStream(bool downConvert);

		Stream OpenMimeWriteStream(MimeLimits mimeLimits, bool expectBinaryContent);

		void OpenMimeDBWriter(DataTableCursor cursor, bool update, Func<bool> checkpointCallback, out Stream mimeMap, out CreateFixedStream mimeCreateFixedStream);

		Stream OpenMimeDBReader();

		MimeDocument LoadMimeFromDB(DecodingOptions decodingOptions);

		long GetCurrrentMimeSize();

		long RefreshMimeSize();

		void UpdateCachedHeaders();

		void RestoreLastSavedMime();

		void ResetMimeParserEohCallback();

		void MinimizeMemory();

		void Commit(TransactionCommitMode commitMode);

		void Materialize(Transaction transaction);

		IAsyncResult BeginCommit(AsyncCallback asyncCallback, object asyncState);

		bool EndCommit(IAsyncResult ar, out Exception exception);

		void MarkToDelete();

		void ReleaseFromActive();

		IMailItemStorage Clone();

		IMailItemStorage CloneCommitted(Action<IMailItemStorage> cloneVisitor);

		void AtomicChange(Action<IMailItemStorage> changeAction);

		IMailItemStorage CopyCommitted(Action<IMailItemStorage> copyVisitor);
	}
}

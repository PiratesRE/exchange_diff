using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct FailureData
	{
		public Guid RequestGuid { get; set; }

		public bool IsFatal { get; set; }

		public Exception Failure { get; set; }

		public RequestState RequestState { get; set; }

		public SyncStage SyncStage { get; set; }

		public string FolderName { get; set; }

		public string OperationType { get; set; }

		public string WatsonHash { get; set; }

		public Guid FailureGuid { get; set; }

		public int FailureLevel { get; set; }
	}
}

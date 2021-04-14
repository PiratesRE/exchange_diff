using System;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	internal static class TraceIDs
	{
		public const int ServiceLetStarted = 42415;

		public const int ServiceletStopped = 28310;

		public const int SpawningThreads = 19568;

		public const int WaitingForThreads = 28049;

		public const int TenantArbitrationMailboxFound = 25566;

		public const int NoTenantArbitrationMailboxFound = 36433;

		public const int AddingTenantToWorkerProcess = 28720;

		public const int StartingWorkerThread = 94668;

		public const int StartProcessingTenant = 54361;

		public const int StartProcessingRequest = 94597;

		public const int ServiceShuttingDown = 29459;

		public const int StartCollectingData = 28705;

		public const int StartSendingEmail = 47332;

		public const int ServiceletException = 21863;

		public const int WorkerException = 11881;

		public const int KnownWorkerException = 96633;

		public const int SearchItemNotFound = 20575;

		public const int NonMailboxRole = 83371;

		public const int FindLocalServerFailed = 67552;

		public const int PollIntervalRead = 13340;

		public const int ConfigurationError = 17790;
	}
}

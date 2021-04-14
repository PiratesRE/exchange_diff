using System;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal struct ExchangeVolumeDbStatusInfo
	{
		public bool DbFilesFound;

		public bool DbMissingInAD;

		public bool DbCopyStatusMissingOrFailedToRetrieve;

		public bool DbCopyStatusNotHealthy;

		public bool UnknownFilesFound;

		public Exception DbFilesException;

		public Exception UnknownFilesException;
	}
}

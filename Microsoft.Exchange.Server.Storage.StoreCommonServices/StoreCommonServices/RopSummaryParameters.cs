using System;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct RopSummaryParameters : ITraceParameters
	{
		public RopSummaryParameters(TimeSpan elapsedTime, uint errorCode, bool isNewActivity, JET_THREADSTATS threadStats, uint directoryOperations, uint offPageHits, TimeSpan cpuTimeKernel, TimeSpan cpuTimeUser, uint operationChunks, TimeSpan maximumChunkTime, TimeSpan lockWaitTime, TimeSpan directoryWaitTime, TimeSpan databaseTime, TimeSpan fastWaitTime)
		{
			this = new RopSummaryParameters(elapsedTime, errorCode, isNewActivity, threadStats, directoryOperations, offPageHits, cpuTimeKernel, cpuTimeUser, operationChunks, maximumChunkTime, lockWaitTime, directoryWaitTime, databaseTime, fastWaitTime, 0);
		}

		public RopSummaryParameters(TimeSpan elapsedTime, uint errorCode, bool isNewActivity, JET_THREADSTATS threadStats, uint directoryOperations, uint offPageHits, TimeSpan cpuTimeKernel, TimeSpan cpuTimeUser, uint operationChunks, TimeSpan maximumChunkTime, TimeSpan lockWaitTime, TimeSpan directoryWaitTime, TimeSpan databaseTime, TimeSpan fastWaitTime, int undefinedAlpha)
		{
			this = new RopSummaryParameters(elapsedTime, errorCode, isNewActivity, threadStats, directoryOperations, offPageHits, cpuTimeKernel, cpuTimeUser, operationChunks, maximumChunkTime, lockWaitTime, directoryWaitTime, databaseTime, fastWaitTime, undefinedAlpha, 0);
		}

		public RopSummaryParameters(TimeSpan elapsedTime, uint errorCode, bool isNewActivity, JET_THREADSTATS threadStats, uint directoryOperations, uint offPageHits, TimeSpan cpuTimeKernel, TimeSpan cpuTimeUser, uint operationChunks, TimeSpan maximumChunkTime, TimeSpan lockWaitTime, TimeSpan directoryWaitTime, TimeSpan databaseTime, TimeSpan fastWaitTime, int undefinedAlpha, int undefinedBeta)
		{
			this = new RopSummaryParameters(elapsedTime, errorCode, isNewActivity, threadStats, directoryOperations, offPageHits, cpuTimeKernel, cpuTimeUser, operationChunks, maximumChunkTime, lockWaitTime, directoryWaitTime, databaseTime, fastWaitTime, undefinedAlpha, undefinedBeta, 0);
		}

		public RopSummaryParameters(TimeSpan elapsedTime, uint errorCode, bool isNewActivity, JET_THREADSTATS threadStats, uint directoryOperations, uint offPageHits, TimeSpan cpuTimeKernel, TimeSpan cpuTimeUser, uint operationChunks, TimeSpan maximumChunkTime, TimeSpan lockWaitTime, TimeSpan directoryWaitTime, TimeSpan databaseTime, TimeSpan fastWaitTime, int undefinedAlpha, int undefinedBeta, int undefinedGamma)
		{
			this = new RopSummaryParameters(elapsedTime, errorCode, isNewActivity, threadStats, directoryOperations, offPageHits, cpuTimeKernel, cpuTimeUser, operationChunks, maximumChunkTime, lockWaitTime, directoryWaitTime, databaseTime, fastWaitTime, undefinedAlpha, undefinedBeta, undefinedGamma, 0);
		}

		public RopSummaryParameters(TimeSpan elapsedTime, uint errorCode, bool isNewActivity, JET_THREADSTATS threadStats, uint directoryOperations, uint offPageHits, TimeSpan cpuTimeKernel, TimeSpan cpuTimeUser, uint operationChunks, TimeSpan maximumChunkTime, TimeSpan lockWaitTime, TimeSpan directoryWaitTime, TimeSpan databaseTime, TimeSpan fastWaitTime, int undefinedAlpha, int undefinedBeta, int undefinedGamma, int undefinedDelta)
		{
			this = new RopSummaryParameters(elapsedTime, errorCode, isNewActivity, threadStats, directoryOperations, offPageHits, cpuTimeKernel, cpuTimeUser, operationChunks, maximumChunkTime, lockWaitTime, directoryWaitTime, databaseTime, fastWaitTime, undefinedAlpha, undefinedBeta, undefinedGamma, undefinedDelta, 0);
		}

		public RopSummaryParameters(TimeSpan elapsedTime, uint errorCode, bool isNewActivity, JET_THREADSTATS threadStats, uint directoryOperations, uint offPageHits, TimeSpan cpuTimeKernel, TimeSpan cpuTimeUser, uint operationChunks, TimeSpan maximumChunkTime, TimeSpan lockWaitTime, TimeSpan directoryWaitTime, TimeSpan databaseTime, TimeSpan fastWaitTime, int undefinedAlpha, int undefinedBeta, int undefinedGamma, int undefinedDelta, int undefinedOmega)
		{
			this.elapsedTime = elapsedTime;
			this.errorCode = errorCode;
			this.isNewActivity = isNewActivity;
			this.cbLogRecord = ((threadStats.cbLogRecord >= 0) ? threadStats.cbLogRecord : int.MaxValue);
			this.cPageDirtied = threadStats.cPageDirtied;
			this.cPagePreread = threadStats.cPagePreread;
			this.cPageRead = threadStats.cPageRead;
			this.cPageRedirtied = threadStats.cPageRedirtied;
			this.jetReservedAlpha = 0;
			this.jetReservedBeta = 0;
			this.directoryOperations = directoryOperations;
			this.offPageHits = offPageHits;
			this.cpuTimeKernel = cpuTimeKernel;
			this.cpuTimeUser = cpuTimeUser;
			this.operationChunks = operationChunks;
			this.maximumChunkTime = maximumChunkTime;
			this.lockWaitTime = lockWaitTime;
			this.directoryWaitTime = directoryWaitTime;
			this.databaseTime = databaseTime;
			this.fastWaitTime = fastWaitTime;
			this.undefinedAlpha = undefinedAlpha;
			this.undefinedBeta = undefinedBeta;
			this.undefinedGamma = undefinedGamma;
			this.undefinedDelta = undefinedDelta;
			this.undefinedOmega = undefinedOmega;
		}

		public bool HasDataToLog
		{
			get
			{
				return true;
			}
		}

		public TimeSpan ElapsedTime
		{
			get
			{
				return this.elapsedTime;
			}
		}

		public uint ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public bool IsNewActivity
		{
			get
			{
				return this.isNewActivity;
			}
		}

		public int LogBytes
		{
			get
			{
				return this.cbLogRecord;
			}
		}

		public int PagesPreread
		{
			get
			{
				return this.cPagePreread;
			}
		}

		public int PagesRead
		{
			get
			{
				return this.cPageRead;
			}
		}

		public int PagesDirtied
		{
			get
			{
				return this.cPageDirtied;
			}
		}

		public int JetReservedAlpha
		{
			get
			{
				return this.jetReservedAlpha;
			}
		}

		public int JetReservedBeta
		{
			get
			{
				return this.jetReservedBeta;
			}
		}

		public int PagesRedirtied
		{
			get
			{
				return this.cPageRedirtied;
			}
		}

		public uint DirectoryOperations
		{
			get
			{
				return this.directoryOperations;
			}
		}

		public uint OffPageHits
		{
			get
			{
				return this.offPageHits;
			}
		}

		public TimeSpan CpuTimeKernel
		{
			get
			{
				return this.cpuTimeKernel;
			}
		}

		public TimeSpan CpuTimeUser
		{
			get
			{
				return this.cpuTimeUser;
			}
		}

		public uint OperationChunks
		{
			get
			{
				return this.operationChunks;
			}
		}

		public TimeSpan MaximumChunkTime
		{
			get
			{
				return this.maximumChunkTime;
			}
		}

		public TimeSpan LockWaitTime
		{
			get
			{
				return this.lockWaitTime;
			}
		}

		public TimeSpan DirectoryWaitTime
		{
			get
			{
				return this.directoryWaitTime;
			}
		}

		public TimeSpan DatabaseTime
		{
			get
			{
				return this.databaseTime;
			}
		}

		public TimeSpan FastWaitTime
		{
			get
			{
				return this.fastWaitTime;
			}
		}

		public int UndefinedAlpha
		{
			get
			{
				return this.undefinedAlpha;
			}
		}

		public int UndefinedBeta
		{
			get
			{
				return this.undefinedBeta;
			}
		}

		public int UndefinedGamma
		{
			get
			{
				return this.undefinedGamma;
			}
		}

		public int UndefinedDelta
		{
			get
			{
				return this.undefinedDelta;
			}
		}

		public int UndefinedOmega
		{
			get
			{
				return this.undefinedOmega;
			}
		}

		public const int TraceSchemaVersion = 100;

		private readonly TimeSpan elapsedTime;

		private readonly uint errorCode;

		private readonly bool isNewActivity;

		private readonly int cbLogRecord;

		private readonly int cPagePreread;

		private readonly int cPageRead;

		private readonly int cPageDirtied;

		private readonly int cPageRedirtied;

		private readonly int jetReservedAlpha;

		private readonly int jetReservedBeta;

		private readonly uint directoryOperations;

		private readonly uint offPageHits;

		private readonly TimeSpan cpuTimeKernel;

		private readonly TimeSpan cpuTimeUser;

		private readonly uint operationChunks;

		private readonly TimeSpan maximumChunkTime;

		private readonly TimeSpan lockWaitTime;

		private readonly TimeSpan directoryWaitTime;

		private readonly TimeSpan databaseTime;

		private readonly TimeSpan fastWaitTime;

		private readonly int undefinedAlpha;

		private readonly int undefinedBeta;

		private readonly int undefinedGamma;

		private readonly int undefinedDelta;

		private readonly int undefinedOmega;
	}
}

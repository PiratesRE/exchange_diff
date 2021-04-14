using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class RopSummaryAggregator : TraceDataAggregator<RopSummaryParameters>
	{
		public RopSummaryAggregator()
		{
		}

		private RopSummaryAggregator(uint totalCalls, uint numberOfCallsSlow, TimeSpan maximumElapsedTime, uint numberOfCallsInError, uint lastKnownError, TimeSpan totalTime, uint numberOfActivities, int totalLogBytes, int totalPagesPreread, int totalPagesRead, int totalPagesDirtied, int totalPagesRedirtied, int totalJetReservedAlpha, int totalJetReservedBeta, uint totalDirectoryOperations, uint totalOffPageHits, TimeSpan totalCpuTimeKernel, TimeSpan totalCpuTimeUser, uint totalChunks, TimeSpan maximumChunkTime, TimeSpan totalLockWaitTime, TimeSpan totalDirectoryWaitTime, TimeSpan totalDatabaseTime, TimeSpan totalFastWaitTime, int totalUndefinedAlpha, int totalUndefinedBeta, int totalUndefinedGamma, int totalUndefinedDelta, int totalUndefinedOmega)
		{
			this.totalCalls = totalCalls;
			this.numberOfCallsSlow = numberOfCallsSlow;
			this.maximumElapsedTime = maximumElapsedTime;
			this.numberOfCallsInError = numberOfCallsInError;
			this.lastKnownError = lastKnownError;
			this.totalTime = totalTime;
			this.numberOfActivities = numberOfActivities;
			this.totalLogBytes = (uint)totalLogBytes;
			this.totalPagesPreread = (uint)totalPagesPreread;
			this.totalPagesRead = (uint)totalPagesRead;
			this.totalPagesDirtied = (uint)totalPagesDirtied;
			this.totalPagesRedirtied = (uint)totalPagesRedirtied;
			this.totalJetReservedAlpha = (uint)totalJetReservedAlpha;
			this.totalJetReservedBeta = (uint)totalJetReservedBeta;
			this.totalDirectoryOperations = totalDirectoryOperations;
			this.totalOffPageHits = totalOffPageHits;
			this.totalCpuTimeKernel = totalCpuTimeKernel;
			this.totalCpuTimeUser = totalCpuTimeUser;
			this.totalChunks = totalChunks;
			this.maximumChunkTime = maximumChunkTime;
			this.totalLockWaitTime = totalLockWaitTime;
			this.totalDirectoryWaitTime = totalDirectoryWaitTime;
			this.totalDatabaseTime = totalDatabaseTime;
			this.totalFastWaitTime = totalFastWaitTime;
			this.totalUndefinedAlpha = totalUndefinedAlpha;
			this.totalUndefinedBeta = totalUndefinedBeta;
			this.totalUndefinedGamma = totalUndefinedGamma;
			this.totalUndefinedDelta = totalUndefinedDelta;
			this.totalUndefinedOmega = totalUndefinedOmega;
		}

		public RopSummaryAggregator(RopSummaryParameters parameters) : this(1U, (parameters.ElapsedTime > DefaultSettings.Get.RopSummarySlowThreshold) ? 1U : 0U, parameters.ElapsedTime, (parameters.ErrorCode == 0U) ? 0U : 1U, parameters.ErrorCode, parameters.ElapsedTime, 1U, parameters.LogBytes, parameters.PagesPreread, parameters.PagesRead, parameters.PagesDirtied, parameters.PagesRedirtied, parameters.JetReservedAlpha, parameters.JetReservedBeta, parameters.DirectoryOperations, parameters.OffPageHits, parameters.CpuTimeKernel, parameters.CpuTimeUser, parameters.OperationChunks, parameters.MaximumChunkTime, parameters.LockWaitTime, parameters.DirectoryWaitTime, parameters.DatabaseTime, parameters.FastWaitTime, parameters.UndefinedAlpha, parameters.UndefinedBeta, parameters.UndefinedGamma, parameters.UndefinedDelta, parameters.UndefinedOmega)
		{
		}

		internal uint TotalCalls
		{
			get
			{
				return this.totalCalls;
			}
		}

		internal uint NumberOfCallsSlow
		{
			get
			{
				return this.numberOfCallsSlow;
			}
		}

		internal TimeSpan MaximumElapsedTime
		{
			get
			{
				return this.maximumElapsedTime;
			}
		}

		internal uint NumberOfCallsInError
		{
			get
			{
				return this.numberOfCallsInError;
			}
		}

		internal uint LastKnownError
		{
			get
			{
				return this.lastKnownError;
			}
		}

		internal TimeSpan TotalTime
		{
			get
			{
				return this.totalTime;
			}
		}

		internal uint NumberOfActivities
		{
			get
			{
				return this.numberOfActivities;
			}
		}

		internal uint TotalLogBytes
		{
			get
			{
				return this.totalLogBytes;
			}
		}

		internal uint TotalPagesPreread
		{
			get
			{
				return this.totalPagesPreread;
			}
		}

		internal uint TotalPagesRead
		{
			get
			{
				return this.totalPagesRead;
			}
		}

		internal uint TotalPagesDirtied
		{
			get
			{
				return this.totalPagesDirtied;
			}
		}

		internal uint TotalPagesRedirtied
		{
			get
			{
				return this.totalPagesRedirtied;
			}
		}

		internal uint TotalJetReservedAlpha
		{
			get
			{
				return this.totalJetReservedAlpha;
			}
		}

		internal uint TotalJetReservedBeta
		{
			get
			{
				return this.totalJetReservedBeta;
			}
		}

		internal uint TotalDirectoryOperations
		{
			get
			{
				return this.totalDirectoryOperations;
			}
		}

		internal uint TotalOffPageHits
		{
			get
			{
				return this.totalOffPageHits;
			}
		}

		internal TimeSpan TotalCpuTimeKernel
		{
			get
			{
				return this.totalCpuTimeKernel;
			}
		}

		internal TimeSpan TotalCpuTimeUser
		{
			get
			{
				return this.totalCpuTimeUser;
			}
		}

		internal uint TotalChunks
		{
			get
			{
				return this.totalChunks;
			}
		}

		internal TimeSpan MaximumChunkTime
		{
			get
			{
				return this.maximumChunkTime;
			}
		}

		internal TimeSpan TotalLockWaitTime
		{
			get
			{
				return this.totalLockWaitTime;
			}
		}

		internal TimeSpan TotalDirectoryWaitTime
		{
			get
			{
				return this.totalDirectoryWaitTime;
			}
		}

		internal TimeSpan TotalDatabaseTime
		{
			get
			{
				return this.totalDatabaseTime;
			}
		}

		internal TimeSpan TotalFastWaitTime
		{
			get
			{
				return this.totalFastWaitTime;
			}
		}

		internal int TotalUndefinedAlpha
		{
			get
			{
				return this.totalUndefinedAlpha;
			}
		}

		internal int TotalUndefinedBeta
		{
			get
			{
				return this.totalUndefinedBeta;
			}
		}

		internal int TotalUndefinedGamma
		{
			get
			{
				return this.totalUndefinedGamma;
			}
		}

		internal int TotalUndefinedDelta
		{
			get
			{
				return this.totalUndefinedDelta;
			}
		}

		internal int TotalUndefinedOmega
		{
			get
			{
				return this.totalUndefinedOmega;
			}
		}

		internal override void Add(RopSummaryParameters parameters)
		{
			this.totalCalls += 1U;
			this.numberOfCallsSlow += ((parameters.ElapsedTime > DefaultSettings.Get.RopSummarySlowThreshold) ? 1U : 0U);
			this.maximumElapsedTime = TimeSpan.FromTicks(Math.Max(this.maximumElapsedTime.Ticks, parameters.ElapsedTime.Ticks));
			this.numberOfCallsInError += ((parameters.ErrorCode != 0U) ? 1U : 0U);
			this.lastKnownError = ((parameters.ErrorCode != 0U) ? parameters.ErrorCode : this.lastKnownError);
			this.totalTime += parameters.ElapsedTime;
			this.numberOfActivities += (parameters.IsNewActivity ? 1U : 0U);
			this.totalLogBytes += (uint)parameters.LogBytes;
			this.totalPagesPreread += (uint)parameters.PagesPreread;
			this.totalPagesRead += (uint)parameters.PagesRead;
			this.totalPagesDirtied += (uint)parameters.PagesDirtied;
			this.totalPagesRedirtied += (uint)parameters.PagesRedirtied;
			this.totalJetReservedAlpha += (uint)parameters.JetReservedAlpha;
			this.totalJetReservedBeta += (uint)parameters.JetReservedBeta;
			this.totalDirectoryOperations += parameters.DirectoryOperations;
			this.totalOffPageHits += parameters.OffPageHits;
			this.totalCpuTimeKernel += parameters.CpuTimeKernel;
			this.totalCpuTimeUser += parameters.CpuTimeUser;
			this.totalChunks += parameters.OperationChunks;
			this.maximumChunkTime = TimeSpan.FromTicks(Math.Max(this.maximumChunkTime.Ticks, parameters.MaximumChunkTime.Ticks));
			this.totalLockWaitTime += parameters.LockWaitTime;
			this.totalDirectoryWaitTime += parameters.DirectoryWaitTime;
			this.totalDatabaseTime += parameters.DatabaseTime;
			this.totalFastWaitTime += parameters.FastWaitTime;
			this.totalUndefinedAlpha += parameters.UndefinedAlpha;
			this.totalUndefinedBeta += parameters.UndefinedBeta;
			this.totalUndefinedGamma += parameters.UndefinedGamma;
			this.totalUndefinedDelta += parameters.UndefinedDelta;
			this.totalUndefinedOmega += parameters.UndefinedOmega;
		}

		private uint totalCalls;

		private uint numberOfCallsSlow;

		private TimeSpan maximumElapsedTime;

		private uint numberOfCallsInError;

		private uint lastKnownError;

		private TimeSpan totalTime;

		private uint numberOfActivities;

		private uint totalLogBytes;

		private uint totalPagesPreread;

		private uint totalPagesRead;

		private uint totalPagesDirtied;

		private uint totalPagesRedirtied;

		private uint totalJetReservedAlpha;

		private uint totalJetReservedBeta;

		private uint totalDirectoryOperations;

		private uint totalOffPageHits;

		private TimeSpan totalCpuTimeKernel;

		private TimeSpan totalCpuTimeUser;

		private uint totalChunks;

		private TimeSpan maximumChunkTime;

		private TimeSpan totalLockWaitTime;

		private TimeSpan totalDirectoryWaitTime;

		private TimeSpan totalDatabaseTime;

		private TimeSpan totalFastWaitTime;

		private int totalUndefinedAlpha;

		private int totalUndefinedBeta;

		private int totalUndefinedGamma;

		private int totalUndefinedDelta;

		private int totalUndefinedOmega;
	}
}

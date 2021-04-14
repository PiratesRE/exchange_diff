using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class FileState
	{
		public FileState()
		{
			this.Reset();
		}

		private static void InternalCheck(bool condition, string formatString, params object[] messageArgs)
		{
			if (!condition)
			{
				string text = string.Format(formatString, messageArgs);
				string stackTrace = Environment.StackTrace;
				ExTraceGlobals.FileCheckerTracer.TraceError<string, string>(0L, "FileState internal check failed. Message is {0}, callstack is {1}", text, stackTrace);
				throw new FileStateInternalErrorException(text);
			}
		}

		public bool RequiredLogfilesArePresent()
		{
			lock (this)
			{
				this.InternalCheck();
				if (0L != this.m_lowestGenerationRequired)
				{
					long highestGenerationPresentWithE = this.HighestGenerationPresentWithE00;
					if (highestGenerationPresentWithE < this.m_highestGenerationRequired)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override string ToString()
		{
			string result;
			lock (this)
			{
				result = string.Format(CultureInfo.CurrentCulture, "LowestGenerationPresent: {1}{0}HighestGenerationPresent: {2}{0}LowestGenerationRequired: {3}{0}HighestGenerationRequired: {4}{0}LastGenerationBackedUp: {5}{0}CheckpointGeneration: {6}{0}LogfileSignature: {7}{0}LatestFullBackupTime: {8}{0}LatestIncrementalBackupTime: {9}{0}LatestDifferentialBackupTime: {10}{0}LatestCopyBackupTime: {11}{0}SnapshotBackup: {12}{0}SnapshotLatestFullBackup: {13}{0}SnapshotLatestIncrementalBackup: {14}{0}SnapshotLatestDifferentialBackup: {15}{0}SnapshotLatestCopyBackup: {16}{0}ConsistentDatabase: {17}", new object[]
				{
					Environment.NewLine,
					this.m_lowestGenerationPresent,
					this.m_highestGenerationPresent,
					this.m_lowestGenerationRequired,
					this.m_highestGenerationRequired,
					this.m_lastGenerationBackedUp,
					this.m_checkpointGeneration,
					this.m_logfileSignature,
					this.m_latestFullBackupTime,
					this.m_latestIncrementalBackupTime,
					this.m_latestDifferentialBackupTime,
					this.m_latestCopyBackupTime,
					this.m_snapshotBackup,
					this.m_snapshotLatestFullBackup,
					this.m_snapshotLatestIncrementalBackup,
					this.m_snapshotLatestDifferentialBackup,
					this.m_snapshotLatestCopyBackup,
					this.m_consistentDatabase
				});
			}
			return result;
		}

		public void Reset()
		{
			this.m_e00Generation = null;
			this.m_lowestGenerationPresent = 0L;
			this.m_highestGenerationPresent = 0L;
			this.m_lowestGenerationRequired = 0L;
			this.m_highestGenerationRequired = 0L;
			this.m_lastGenerationBackedUp = 0L;
			this.m_checkpointGeneration = 0L;
			this.m_logfileSignature = null;
			this.m_latestFullBackupTime = null;
			this.m_latestIncrementalBackupTime = null;
			this.m_latestDifferentialBackupTime = null;
			this.m_latestCopyBackupTime = null;
			this.m_snapshotBackup = null;
			this.m_snapshotLatestFullBackup = null;
			this.m_snapshotLatestIncrementalBackup = null;
			this.m_snapshotLatestDifferentialBackup = null;
			this.m_snapshotLatestCopyBackup = null;
			this.m_consistentDatabase = false;
			this.InternalCheck();
		}

		public void InternalCheck()
		{
			lock (this)
			{
				FileState.InternalCheck(this.m_lowestGenerationPresent <= this.m_highestGenerationPresent, "LowestGenerationPresent > HighestGenerationPresent", new object[0]);
				FileState.InternalCheck(this.m_lowestGenerationRequired <= this.m_highestGenerationRequired, "LowestGenerationRequired > HighestGenerationRequired", new object[0]);
				if (0L == this.m_lowestGenerationPresent)
				{
					FileState.InternalCheck(0L == this.m_highestGenerationPresent, "LowestGenerationPresent is 0, but HighestGenerationPresent is set", new object[0]);
				}
				else
				{
					FileState.InternalCheck(0L != this.m_highestGenerationPresent, "LowestGenerationPresent is set, but HighestGenerationPresent is 0", new object[0]);
				}
				if (0L == this.m_lowestGenerationRequired)
				{
					FileState.InternalCheck(0L == this.m_highestGenerationRequired, "LowestGenerationRequired is 0, but HighestGenerationRequired is set", new object[0]);
				}
				else
				{
					FileState.InternalCheck(0L != this.m_highestGenerationRequired, "LowestGenerationRequired is set, but HighestGenerationRequired is 0", new object[0]);
				}
				if (this.m_consistentDatabase)
				{
					FileState.InternalCheck(this.m_highestGenerationRequired == this.m_lowestGenerationRequired, "ConsistentDatabase is true, but HighestGenerationRequired != LowestGenerationRequired", new object[0]);
				}
				if (this.m_highestGenerationRequired != this.m_lowestGenerationRequired)
				{
					FileState.InternalCheck(!this.m_consistentDatabase, "HighestGenerationRequired != LowestGenerationRequired but ConsistentDatabase is true", new object[0]);
				}
				if (this.m_latestFullBackupTime != null)
				{
					FileState.InternalCheck(this.m_lastGenerationBackedUp > 0L, "m_latestFullBackupTime is set, but m_lastGenerationBackedUp is not set", new object[0]);
				}
				if (this.m_latestIncrementalBackupTime != null)
				{
					FileState.InternalCheck(this.m_lastGenerationBackedUp > 0L, "m_latestIncrementalBackupTime is set, but m_lastGenerationBackedUp is not set", new object[0]);
					FileState.InternalCheck(this.m_latestFullBackupTime != null, "m_latestIncrementalBackupTime is set, but m_latestFullBackupTime is not set", new object[0]);
				}
				if (this.m_latestDifferentialBackupTime != null)
				{
					FileState.InternalCheck(this.m_lastGenerationBackedUp > 0L, "m_latestDifferentialBackupTime is set, but m_lastGenerationBackedUp is not set", new object[0]);
					FileState.InternalCheck(this.m_latestFullBackupTime != null, "m_latestDifferentialBackupTime is set, but m_latestFullBackupTime is not set", new object[0]);
				}
			}
		}

		public void InternalCheckLogfileSignature()
		{
			lock (this)
			{
				if (0L != this.LowestGenerationPresent)
				{
					FileState.InternalCheck(this.LogfileSignature != null, "Logfiles are present, but LogfileSignature is not set", new object[0]);
				}
				if (0L != this.LowestGenerationRequired)
				{
					FileState.InternalCheck(this.LogfileSignature != null, "Logfiles are required, but LogfileSignature is not set", new object[0]);
				}
			}
		}

		public void GetLowestAndHighestGenerationsRequired(out bool databaseIsConsistent, out long lowestGenerationRequired, out long highestGenerationRequired)
		{
			lock (this)
			{
				databaseIsConsistent = this.m_consistentDatabase;
				lowestGenerationRequired = this.m_lowestGenerationRequired;
				highestGenerationRequired = this.m_highestGenerationRequired;
			}
		}

		public void SetLowestAndHighestGenerationsPresent(long lowestGenerationPresent, long highestGenerationPresent)
		{
			FileState.InternalCheck(lowestGenerationPresent <= highestGenerationPresent, "lowestGenerationPresent > highestGenerationPresent", new object[0]);
			if (lowestGenerationPresent == 0L)
			{
				FileState.InternalCheck(highestGenerationPresent == 0L, "Highest gen must be 0 when low gen is 0", new object[0]);
			}
			if (highestGenerationPresent == 0L)
			{
				FileState.InternalCheck(lowestGenerationPresent == 0L, "Lowest gen must be 0 when hi gen is 0", new object[0]);
			}
			lock (this)
			{
				this.InternalCheck();
				this.m_lowestGenerationPresent = lowestGenerationPresent;
				this.m_highestGenerationPresent = highestGenerationPresent;
				this.InternalCheck();
			}
		}

		public void SetLowestGenerationPresent(long lowestGenerationPresent)
		{
			lock (this)
			{
				FileState.InternalCheck(lowestGenerationPresent <= this.m_highestGenerationPresent, "LowestGenerationPresent > HighestGenerationPresent", new object[0]);
				this.InternalCheck();
				this.m_lowestGenerationPresent = lowestGenerationPresent;
				this.InternalCheck();
			}
		}

		public void SetLowestAndHighestGenerationsRequired(long lowestGenerationRequired, long highestGenerationRequired, bool databaseIsConsistent)
		{
			FileState.InternalCheck(lowestGenerationRequired >= 0L, "lowestGenerationRequired should be greater than 0 but we got {0}", new object[]
			{
				lowestGenerationRequired
			});
			FileState.InternalCheck(highestGenerationRequired >= 0L, "highestGenerationRequired should be greater than 0 but we got {0}", new object[]
			{
				highestGenerationRequired
			});
			FileState.InternalCheck(lowestGenerationRequired <= highestGenerationRequired, "lowestGenerationRequired > highestGenerationRequired", new object[0]);
			FileState.InternalCheck(!databaseIsConsistent || lowestGenerationRequired == highestGenerationRequired, "lowestGenerationRequired != highestGenerationRequired, but the database is consistent", new object[0]);
			lock (this)
			{
				this.InternalCheck();
				this.m_lowestGenerationRequired = lowestGenerationRequired;
				this.m_highestGenerationRequired = highestGenerationRequired;
				this.m_consistentDatabase = databaseIsConsistent;
				this.InternalCheck();
			}
		}

		public void SetE00LogGeneration(long e00generation)
		{
			FileState.InternalCheck(e00generation > 0L, "e00generation should be greater than 0!", new object[0]);
			lock (this)
			{
				this.InternalCheck();
				this.m_e00Generation = new long?(e00generation);
				this.InternalCheck();
			}
		}

		public void ClearE00LogGeneration()
		{
			lock (this)
			{
				this.InternalCheck();
				this.m_e00Generation = null;
				this.InternalCheck();
			}
		}

		public bool IsGenerationInRequiredRange(long generation)
		{
			bool result;
			lock (this)
			{
				if (this.m_lowestGenerationRequired != 0L)
				{
					result = (generation >= this.m_lowestGenerationRequired && generation <= this.m_highestGenerationRequired);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public long LowestGenerationPresent
		{
			get
			{
				long lowestGenerationPresent;
				lock (this)
				{
					this.InternalCheck();
					lowestGenerationPresent = this.m_lowestGenerationPresent;
				}
				return lowestGenerationPresent;
			}
		}

		public long HighestGenerationPresentWithE00
		{
			get
			{
				long result;
				lock (this)
				{
					this.InternalCheck();
					long num = this.m_highestGenerationPresent;
					if (this.m_e00Generation != null)
					{
						num = Math.Max(this.m_e00Generation.Value, num);
					}
					result = num;
				}
				return result;
			}
		}

		public long HighestGenerationPresent
		{
			get
			{
				long highestGenerationPresent;
				lock (this)
				{
					this.InternalCheck();
					highestGenerationPresent = this.m_highestGenerationPresent;
				}
				return highestGenerationPresent;
			}
		}

		public long LowestGenerationRequired
		{
			get
			{
				long lowestGenerationRequired;
				lock (this)
				{
					this.InternalCheck();
					lowestGenerationRequired = this.m_lowestGenerationRequired;
				}
				return lowestGenerationRequired;
			}
		}

		public long HighestGenerationRequired
		{
			get
			{
				long highestGenerationRequired;
				lock (this)
				{
					this.InternalCheck();
					highestGenerationRequired = this.m_highestGenerationRequired;
				}
				return highestGenerationRequired;
			}
		}

		public long? E00Generation
		{
			get
			{
				long? e00Generation;
				lock (this)
				{
					this.InternalCheck();
					e00Generation = this.m_e00Generation;
				}
				return e00Generation;
			}
		}

		public long LastGenerationBackedUp
		{
			get
			{
				long lastGenerationBackedUp;
				lock (this)
				{
					this.InternalCheck();
					lastGenerationBackedUp = this.m_lastGenerationBackedUp;
				}
				return lastGenerationBackedUp;
			}
			set
			{
				lock (this)
				{
					this.m_lastGenerationBackedUp = value;
					this.InternalCheck();
				}
			}
		}

		public long CheckpointGeneration
		{
			get
			{
				long checkpointGeneration;
				lock (this)
				{
					this.InternalCheck();
					checkpointGeneration = this.m_checkpointGeneration;
				}
				return checkpointGeneration;
			}
			set
			{
				lock (this)
				{
					this.m_checkpointGeneration = value;
					this.InternalCheck();
				}
			}
		}

		internal JET_SIGNATURE? LogfileSignature
		{
			get
			{
				JET_SIGNATURE? logfileSignature;
				lock (this)
				{
					this.InternalCheck();
					logfileSignature = this.m_logfileSignature;
				}
				return logfileSignature;
			}
			set
			{
				lock (this)
				{
					if (this.m_logfileSignature != null && (this.m_logfileSignature.Value.ulRandom != value.Value.ulRandom || this.m_logfileSignature.Value.logtimeCreate.ToUint64() != value.Value.logtimeCreate.ToUint64()))
					{
						FileState.InternalCheck(false, "Logfile signature is already set. Multiple assignment must be idempotent.", new object[0]);
					}
					this.m_logfileSignature = value;
					this.InternalCheck();
					this.InternalCheckLogfileSignature();
				}
			}
		}

		public DateTime? LatestFullBackupTime
		{
			get
			{
				DateTime? latestFullBackupTime;
				lock (this)
				{
					this.InternalCheck();
					latestFullBackupTime = this.m_latestFullBackupTime;
				}
				return latestFullBackupTime;
			}
			set
			{
				lock (this)
				{
					this.m_latestFullBackupTime = value;
					this.InternalCheck();
				}
			}
		}

		public DateTime? LatestIncrementalBackupTime
		{
			get
			{
				DateTime? latestIncrementalBackupTime;
				lock (this)
				{
					this.InternalCheck();
					latestIncrementalBackupTime = this.m_latestIncrementalBackupTime;
				}
				return latestIncrementalBackupTime;
			}
			set
			{
				lock (this)
				{
					this.m_latestIncrementalBackupTime = value;
					this.InternalCheck();
				}
			}
		}

		public DateTime? LatestDifferentialBackupTime
		{
			get
			{
				DateTime? latestDifferentialBackupTime;
				lock (this)
				{
					this.InternalCheck();
					latestDifferentialBackupTime = this.m_latestDifferentialBackupTime;
				}
				return latestDifferentialBackupTime;
			}
			set
			{
				lock (this)
				{
					this.m_latestDifferentialBackupTime = value;
					this.InternalCheck();
				}
			}
		}

		public DateTime? LatestCopyBackupTime
		{
			get
			{
				DateTime? latestCopyBackupTime;
				lock (this)
				{
					this.InternalCheck();
					latestCopyBackupTime = this.m_latestCopyBackupTime;
				}
				return latestCopyBackupTime;
			}
			set
			{
				lock (this)
				{
					this.m_latestCopyBackupTime = value;
					this.InternalCheck();
				}
			}
		}

		public bool? SnapshotBackup
		{
			get
			{
				bool? snapshotBackup;
				lock (this)
				{
					this.InternalCheck();
					snapshotBackup = this.m_snapshotBackup;
				}
				return snapshotBackup;
			}
			set
			{
				lock (this)
				{
					this.m_snapshotBackup = value;
					this.InternalCheck();
				}
			}
		}

		public bool? SnapshotLatestFullBackup
		{
			get
			{
				bool? snapshotLatestFullBackup;
				lock (this)
				{
					this.InternalCheck();
					snapshotLatestFullBackup = this.m_snapshotLatestFullBackup;
				}
				return snapshotLatestFullBackup;
			}
			set
			{
				lock (this)
				{
					this.m_snapshotLatestFullBackup = value;
					this.InternalCheck();
				}
			}
		}

		public bool? SnapshotLatestIncrementalBackup
		{
			get
			{
				bool? snapshotLatestIncrementalBackup;
				lock (this)
				{
					this.InternalCheck();
					snapshotLatestIncrementalBackup = this.m_snapshotLatestIncrementalBackup;
				}
				return snapshotLatestIncrementalBackup;
			}
			set
			{
				lock (this)
				{
					this.m_snapshotLatestIncrementalBackup = value;
					this.InternalCheck();
				}
			}
		}

		public bool? SnapshotLatestDifferentialBackup
		{
			get
			{
				bool? snapshotLatestDifferentialBackup;
				lock (this)
				{
					this.InternalCheck();
					snapshotLatestDifferentialBackup = this.m_snapshotLatestDifferentialBackup;
				}
				return snapshotLatestDifferentialBackup;
			}
			set
			{
				lock (this)
				{
					this.m_snapshotLatestDifferentialBackup = value;
					this.InternalCheck();
				}
			}
		}

		public bool? SnapshotLatestCopyBackup
		{
			get
			{
				bool? snapshotLatestCopyBackup;
				lock (this)
				{
					this.InternalCheck();
					snapshotLatestCopyBackup = this.m_snapshotLatestCopyBackup;
				}
				return snapshotLatestCopyBackup;
			}
			set
			{
				lock (this)
				{
					this.m_snapshotLatestCopyBackup = value;
					this.InternalCheck();
				}
			}
		}

		public bool ConsistentDatabase
		{
			get
			{
				bool consistentDatabase;
				lock (this)
				{
					this.InternalCheck();
					consistentDatabase = this.m_consistentDatabase;
				}
				return consistentDatabase;
			}
		}

		internal static void InternalTest()
		{
			FileState.TestInternalCheck();
			FileState fileState = new FileState();
			fileState.AssertMembersAreZero();
			fileState.SetLowestAndHighestGenerationsPresent(2L, 3L);
			fileState.SetLowestAndHighestGenerationsRequired(8L, 9L, false);
			fileState.LastGenerationBackedUp = 1L;
			fileState.CheckpointGeneration = 7L;
			DiagCore.RetailAssert(!fileState.ConsistentDatabase, "ConsistentDatabase is {0}, expected {1}", new object[]
			{
				fileState.ConsistentDatabase,
				false
			});
			DiagCore.RetailAssert(2L == fileState.LowestGenerationPresent, "LowestGenerationPresent is {0}, expected {1}", new object[]
			{
				fileState.LowestGenerationPresent,
				2
			});
			DiagCore.RetailAssert(3L == fileState.HighestGenerationPresent, "HighestGenerationPresent is {0}, expected {1}", new object[]
			{
				fileState.HighestGenerationPresent,
				3
			});
			DiagCore.RetailAssert(8L == fileState.LowestGenerationRequired, "LowestGenerationRequired is {0}, expected {1}", new object[]
			{
				fileState.LowestGenerationRequired,
				8
			});
			DiagCore.RetailAssert(9L == fileState.HighestGenerationRequired, "HighestGenerationRequired is {0}, expected {1}", new object[]
			{
				fileState.HighestGenerationRequired,
				9
			});
			DiagCore.RetailAssert(1L == fileState.LastGenerationBackedUp, "LastGenerationBackedUp is {0}, expected {1}", new object[]
			{
				fileState.LastGenerationBackedUp,
				1
			});
			DiagCore.RetailAssert(7L == fileState.CheckpointGeneration, "CheckpointGeneration is {0}, expected {1}", new object[]
			{
				fileState.CheckpointGeneration,
				7
			});
			fileState.Reset();
			fileState.AssertMembersAreZero();
			DiagCore.RetailAssert(fileState.RequiredLogfilesArePresent(), "RequiredLogfilesArePresent", new object[0]);
			fileState.SetLowestAndHighestGenerationsPresent(18L, 21L);
			fileState.SetLowestAndHighestGenerationsRequired(18L, 21L, false);
			DiagCore.RetailAssert(fileState.RequiredLogfilesArePresent(), "RequiredLogfilesArePresent", new object[0]);
			fileState.Reset();
			fileState.SetLowestAndHighestGenerationsPresent(18L, 21L);
			fileState.SetLowestAndHighestGenerationsRequired(19L, 20L, false);
			DiagCore.RetailAssert(fileState.RequiredLogfilesArePresent(), "RequiredLogfilesArePresent", new object[0]);
			fileState.Reset();
			fileState.SetLowestAndHighestGenerationsPresent(18L, 21L);
			fileState.SetLowestAndHighestGenerationsRequired(19L, 22L, false);
			DiagCore.RetailAssert(!fileState.RequiredLogfilesArePresent(), "RequiredLogfilesArePresent", new object[0]);
			fileState.Reset();
			fileState.SetLowestAndHighestGenerationsPresent(18L, 21L);
			fileState.SetLowestAndHighestGenerationsRequired(17L, 20L, false);
			DiagCore.RetailAssert(fileState.RequiredLogfilesArePresent(), "RequiredLogfilesArePresent", new object[0]);
			fileState.Reset();
		}

		private static void TestInternalCheck()
		{
			FileState.InternalCheck(true, "This InternalCheck should not fire", new object[0]);
			try
			{
				FileState.InternalCheck(false, "This InternalCheck should fire", new object[0]);
				DiagCore.RetailAssert(false, "Should have thrown FileStateInternalErrorException", new object[0]);
			}
			catch (FileStateInternalErrorException)
			{
			}
		}

		private void AssertMembersAreZero()
		{
			DiagCore.RetailAssert(0L == this.m_lowestGenerationPresent, "m_lowestGenerationPresent is not 0", new object[0]);
			DiagCore.RetailAssert(0L == this.m_highestGenerationPresent, "m_highestGenerationPresent is not 0", new object[0]);
			DiagCore.RetailAssert(0L == this.m_lowestGenerationRequired, "m_lowestGenerationRequired is not 0", new object[0]);
			DiagCore.RetailAssert(0L == this.m_highestGenerationRequired, "m_highestGenerationRequired is not 0", new object[0]);
			DiagCore.RetailAssert(0L == this.m_lastGenerationBackedUp, "m_lastGenerationBackedUp is not 0", new object[0]);
			DiagCore.RetailAssert(0L == this.m_checkpointGeneration, "m_checkpointGeneration is not 0", new object[0]);
			DiagCore.RetailAssert(this.m_logfileSignature == null, "m_logfileSignature is not null", new object[0]);
			DiagCore.RetailAssert(this.m_latestFullBackupTime == null, "m_latestFullBackupTime is not null", new object[0]);
			DiagCore.RetailAssert(this.m_latestIncrementalBackupTime == null, "m_latestIncrementalBackupTime is not null", new object[0]);
			DiagCore.RetailAssert(this.m_latestDifferentialBackupTime == null, "m_latestDifferentialBackupTime is not null", new object[0]);
			DiagCore.RetailAssert(this.m_latestCopyBackupTime == null, "m_latestCopyBackupTime is not null", new object[0]);
			DiagCore.RetailAssert(this.m_snapshotBackup == null, "m_snapshotBackup is not null", new object[0]);
			DiagCore.RetailAssert(this.m_snapshotLatestFullBackup == null, "m_snapshotLatestFullBackup is not null", new object[0]);
			DiagCore.RetailAssert(this.m_snapshotLatestIncrementalBackup == null, "m_snapshotLatestIncrementalBackup is not null", new object[0]);
			DiagCore.RetailAssert(this.m_snapshotLatestDifferentialBackup == null, "m_snapshotLatestDifferentialBackup is not null", new object[0]);
			DiagCore.RetailAssert(this.m_snapshotLatestCopyBackup == null, "m_snapshotLatestCopyBackup is not null", new object[0]);
			DiagCore.RetailAssert(!this.m_consistentDatabase, "m_consistentDatabase is not false", new object[0]);
		}

		private long? m_e00Generation;

		private long m_lowestGenerationPresent;

		private long m_highestGenerationPresent;

		private long m_lowestGenerationRequired;

		private long m_highestGenerationRequired;

		private long m_lastGenerationBackedUp;

		private long m_checkpointGeneration;

		private JET_SIGNATURE? m_logfileSignature;

		private DateTime? m_latestFullBackupTime;

		private DateTime? m_latestIncrementalBackupTime;

		private DateTime? m_latestDifferentialBackupTime;

		private DateTime? m_latestCopyBackupTime;

		private bool? m_snapshotBackup;

		private bool? m_snapshotLatestFullBackup;

		private bool? m_snapshotLatestIncrementalBackup;

		private bool? m_snapshotLatestDifferentialBackup;

		private bool? m_snapshotLatestCopyBackup;

		private bool m_consistentDatabase;
	}
}

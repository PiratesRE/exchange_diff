using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Security.Compliance;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class GranularWriter
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.GranularWriterTracer;
			}
		}

		public static bool IsDebugTraceEnabled
		{
			get
			{
				return ExTraceGlobals.GranularWriterTracer.IsTraceEnabled(TraceType.DebugTrace);
			}
		}

		public static bool BytesAreIdentical(byte[] b1, byte[] b2)
		{
			if (b1 == null)
			{
				return b2 == null || b2.Length == 0;
			}
			if (b2 == null)
			{
				return b1.Length == 0;
			}
			if (b1.Length != b2.Length)
			{
				return false;
			}
			for (int i = 0; i < b1.Length; i++)
			{
				if (b1[i] != b2[i])
				{
					return false;
				}
			}
			return true;
		}

		private void TrackInspectorGeneration(long gen, DateTime writeTimeUtc)
		{
			if (this.m_logCopier != null)
			{
				this.m_logCopier.TrackInspectorGeneration(gen, writeTimeUtc);
			}
		}

		public GranularWriter(LogCopier logCopier, IPerfmonCounters perfmonCounters, IReplayConfiguration replayConfiguration, ISetBroken setBroken)
		{
			this.m_logCopier = logCopier;
			this.m_setBroken = setBroken;
			this.m_perfmonCounters = perfmonCounters;
			this.m_config = replayConfiguration;
			this.m_perfmonCounters.GranularReplication = 0L;
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.m_config.IdentityGuid;
			}
		}

		internal string DatabaseName
		{
			get
			{
				return this.m_config.DatabaseName;
			}
		}

		private void SetSystemParameter(JET_param parameter, string value)
		{
			Api.JetSetSystemParameter(this.m_jetConsumer, JET_SESID.Nil, parameter, 0, value);
		}

		private void SetSystemParameter(JET_param parameter, int value)
		{
			Api.JetSetSystemParameter(this.m_jetConsumer, JET_SESID.Nil, parameter, value, null);
		}

		public void Initialize()
		{
			GranularWriter.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Initialize({0})", this.DatabaseName);
			this.m_lowestClosedGeneration = 0L;
			this.m_currentGeneration = 0L;
			this.m_currentGenerationState = GranularWriter.GranularLogState.Unknown;
			this.DiscardLogs();
			try
			{
				string name = this.m_config.DatabaseName + "_PassiveInBlockMode";
				Api.JetCreateInstance(out this.m_jetConsumer, name);
				string paramString = null;
				InstanceParameters instanceParameters = new InstanceParameters(this.m_jetConsumer);
				instanceParameters.BaseName = this.m_config.LogFilePrefix;
				instanceParameters.LogFileDirectory = this.m_config.LogInspectorPath;
				instanceParameters.SystemDirectory = this.m_config.DestinationSystemPath;
				instanceParameters.NoInformationEvent = true;
				instanceParameters.Recovery = false;
				instanceParameters.LogFileSize = 1024;
				instanceParameters.MaxTemporaryTables = 0;
				if (!RegistryParameters.DisableJetFailureItemPublish)
				{
					Api.JetSetSystemParameter(this.m_jetConsumer, JET_SESID.Nil, (JET_param)168, 1, paramString);
				}
				Api.JetInit(ref this.m_jetConsumer);
				this.m_jetConsumerInitialized = true;
				this.m_jetConsumerHealthy = true;
			}
			catch (EsentErrorException ex)
			{
				GranularWriter.Tracer.TraceError<string, EsentErrorException>((long)this.GetHashCode(), "Initialize({0}) failed:{1}", this.DatabaseName, ex);
				throw new GranularReplicationInitFailedException(ex.Message, ex);
			}
		}

		public bool IsHealthy
		{
			get
			{
				return this.m_jetConsumerHealthy;
			}
		}

		private string FormShortGranuleName(long gen)
		{
			return GranularReplication.FormPartialLogFileName(this.m_config.LogFilePrefix, gen);
		}

		public string FormFullGranuleName(long gen)
		{
			return Path.Combine(this.m_config.LogInspectorPath, this.FormShortGranuleName(gen));
		}

		private string FormInspectorLogfileName(long generation)
		{
			return Path.Combine(this.m_config.LogInspectorPath, this.m_config.BuildShortLogfileName(generation));
		}

		public void DiscardLogs()
		{
			GranularWriter.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DiscardLogs: {0}", this.m_config.LogInspectorPath);
			FailureTag failureTag = FailureTag.IoHard;
			Exception ex = null;
			string text = string.Empty;
			try
			{
				foreach (string text2 in Directory.GetFiles(this.m_config.LogInspectorPath, "*.jsl"))
				{
					text = text2;
					File.Delete(text2);
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
				failureTag = ReplicaInstance.IOExceptionToFailureTag(ex2);
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				GranularWriter.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "Deleting file '{0}' failed with exception {1}", text, ex);
				this.m_setBroken.SetBroken(failureTag, ReplayEventLogConstants.Tuple_CouldNotDeleteLogFile, ex, new string[]
				{
					text,
					ex.ToString()
				});
				throw new GranularReplicationTerminatedException(ex.Message, ex);
			}
		}

		private void ReleaseRangeOfClosedButUnfinalizedLogs(long lastToRelease, out long lowestGenReleased, out long highestGenReleased)
		{
			lowestGenReleased = 0L;
			highestGenReleased = 0L;
			while (this.m_lowestClosedGeneration <= lastToRelease)
			{
				string sourceFileName = this.FormFullGranuleName(this.m_lowestClosedGeneration);
				string destFileName = this.FormInspectorLogfileName(this.m_lowestClosedGeneration);
				File.Move(sourceFileName, destFileName);
				this.TrackInspectorGeneration(this.m_lowestClosedGeneration, DateTime.UtcNow);
				if (lowestGenReleased == 0L)
				{
					lowestGenReleased = this.m_lowestClosedGeneration;
				}
				highestGenReleased = this.m_lowestClosedGeneration;
				this.m_lowestClosedGeneration += 1L;
			}
			if (lowestGenReleased > 0L)
			{
				ReplayCrimsonEvents.ReleasedClosedButUnfinalizedLogs.Log<string, string, string, string>(this.DatabaseName, Environment.MachineName, string.Format("0x{0:x}", lowestGenReleased), string.Format("0x{0:x}", highestGenReleased));
			}
		}

		private void ReleaseAllClosedButUnfinalizedLogs(out long lowestGenReleased, out long highestGenReleased)
		{
			lowestGenReleased = 0L;
			highestGenReleased = 0L;
			if (this.m_lowestClosedGeneration > 0L)
			{
				this.ReleaseRangeOfClosedButUnfinalizedLogs(this.m_currentGeneration - 1L, out lowestGenReleased, out highestGenReleased);
			}
		}

		public bool UsePartialLogsDuringAcll(long expectedE00Gen)
		{
			if (expectedE00Gen != this.m_currentGeneration)
			{
				GranularWriter.Tracer.TraceError<long, long>((long)this.GetHashCode(), "UsePartialLogsDuringAcll could not use granular data. ExpectedE00=0x{0:X}. GranularPosition=0x{1:X}", expectedE00Gen, this.m_currentGeneration);
				return false;
			}
			Exception ex = null;
			bool result = false;
			long num = 0L;
			long num2 = 0L;
			try
			{
				this.ReleaseAllClosedButUnfinalizedLogs(out num, out num2);
				if (this.m_currentGeneration > 0L && (this.m_currentGenerationState == GranularWriter.GranularLogState.Open || this.m_currentGenerationState == GranularWriter.GranularLogState.Aborted))
				{
					if (num == 0L)
					{
						num = this.m_currentGeneration;
					}
					num2 = this.m_currentGeneration;
					if (this.m_currentGenerationState == GranularWriter.GranularLogState.Open)
					{
						this.Terminate();
					}
					string text = this.FormFullGranuleName(this.m_currentGeneration);
					if (this.m_lastTimeFromServer != null)
					{
						FileInfo fileInfo = new FileInfo(text);
						fileInfo.LastWriteTimeUtc = this.m_lastTimeFromServer.Value;
					}
					string destFileName = this.FormInspectorLogfileName(0L);
					File.Move(text, destFileName);
					GranularWriter.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Used PartialGranuleAsExx: {0}", text);
					result = true;
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				GranularWriter.Tracer.TraceError<Exception>((long)this.GetHashCode(), "UsePartialLogsDuringAcll fails: {0}", ex);
				ReplayCrimsonEvents.GranularLogsFailedDuringAcll.Log<string, string, string>(this.DatabaseName, Environment.MachineName, ex.Message);
			}
			GranularWriter.Tracer.TraceDebug<string, long, long>((long)this.GetHashCode(), "UsePartialLogsDuringAcll({0}): Incomplete logs used from gen 0x{1} to {2}", this.DatabaseName, num, num2);
			ReplayCrimsonEvents.GranularLogsUsedDuringAcll.Log<string, string, string, string>(this.DatabaseName, Environment.MachineName, string.Format("0x{0:x}", num), string.Format("0x{0:x}", num2));
			return result;
		}

		public bool TerminateWithRelease(long expectedGeneration)
		{
			bool result = false;
			lock (this.m_granularLock)
			{
				try
				{
					if (expectedGeneration != this.m_currentGeneration)
					{
						GranularWriter.Tracer.TraceError<long, long>((long)this.GetHashCode(), "TerminateWithRelease could not use granular data. ExpectedGen=0x{0:X}. GranularPosition=0x{1:X}", expectedGeneration, this.m_currentGeneration);
						return false;
					}
					long num = 0L;
					long num2 = 0L;
					this.ReleaseAllClosedButUnfinalizedLogs(out num, out num2);
					result = true;
				}
				finally
				{
					this.Terminate();
				}
			}
			return result;
		}

		public void Terminate()
		{
			GranularWriter.Tracer.TraceDebug<string>((long)this.GetHashCode(), "TerminateGranularReplication({0})", this.DatabaseName);
			this.m_perfmonCounters.GranularReplication = 0L;
			lock (this.m_granularLock)
			{
				if (this.m_currentGenerationState == GranularWriter.GranularLogState.Open)
				{
					this.m_currentGenerationState = GranularWriter.GranularLogState.Aborted;
				}
				if (this.m_jetConsumerInitialized)
				{
					try
					{
						Api.JetTerm(this.m_jetConsumer);
					}
					catch (EsentErrorException arg)
					{
						GranularWriter.Tracer.TraceError<EsentErrorException>((long)this.GetHashCode(), "Terminate failed: {0}", arg);
					}
					this.m_jetConsumerInitialized = false;
					this.m_jetConsumerHealthy = false;
				}
			}
		}

		public void Write(JET_EMITDATACTX emitCtx, byte[] databuf, int startOffset)
		{
			if (!this.m_jetConsumerInitialized)
			{
				throw new GranularReplicationTerminatedException("Already terminated");
			}
			try
			{
				StopwatchStamp stamp = StopwatchStamp.GetStamp();
				long num = (long)emitCtx.lgposLogData.lGeneration;
				int grbitOperationalFlags = (int)emitCtx.grbitOperationalFlags;
				bool flag = false;
				if (BitMasker.IsOn(grbitOperationalFlags, 16))
				{
					flag = true;
				}
				UnpublishedApi.JetConsumeLogData(this.m_jetConsumer, emitCtx, databuf, startOffset, (int)emitCtx.cbLogData, ShadowLogConsumeGrbit.FlushData);
				if (flag)
				{
					GranularWriter.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "WriteGranular({0}): 0x{1:X} is complete", this.DatabaseName, num);
					if (this.m_lowestClosedGeneration == 0L)
					{
						this.m_lowestClosedGeneration = num;
					}
					string text = this.FormFullGranuleName(num);
					FileInfo fileInfo = new FileInfo(text);
					fileInfo.LastWriteTimeUtc = emitCtx.logtimeEmit;
					if (this.m_granularCompletionsDisabled)
					{
						string destFileName = this.FormInspectorLogfileName(num);
						File.Move(text, destFileName);
						this.TrackInspectorGeneration(num, emitCtx.logtimeEmit);
						this.m_lowestClosedGeneration = num + 1L;
					}
					this.m_currentGenerationState = GranularWriter.GranularLogState.Expected;
					this.m_currentGeneration = num + 1L;
				}
				else if (BitMasker.IsOn(grbitOperationalFlags, 8))
				{
					this.m_currentGenerationState = GranularWriter.GranularLogState.Open;
					this.m_currentGeneration = num;
					this.m_lastTimeFromServer = new DateTime?(emitCtx.logtimeEmit);
				}
				long elapsedTicks = stamp.ElapsedTicks;
				this.m_perfmonCounters.RecordBlockModeConsumerWriteLatency(elapsedTicks);
				GranularWriter.Tracer.TracePerformance((long)this.GetHashCode(), "WriteGranular({0},0x{1:X}.0x{2:X}) EmitSeq=0x{3:X} took {4} uSec", new object[]
				{
					this.DatabaseName,
					emitCtx.lgposLogData.lGeneration,
					emitCtx.lgposLogData.isec,
					emitCtx.qwSequenceNum,
					StopwatchStamp.TicksToMicroSeconds(elapsedTicks)
				});
			}
			catch (EsentErrorException ex)
			{
				GranularWriter.Tracer.TraceError<EsentErrorException>((long)this.GetHashCode(), "JetConsumeLogData threw {0}", ex);
				this.m_jetConsumerHealthy = false;
				throw new GranularReplicationTerminatedException(ex.Message, ex);
			}
			catch (IOException ex2)
			{
				GranularWriter.Tracer.TraceError<IOException>((long)this.GetHashCode(), "IOException: {0}", ex2);
				this.m_jetConsumerHealthy = false;
				throw new GranularReplicationTerminatedException(ex2.Message, ex2);
			}
		}

		public void EnableGranularCompletions()
		{
			this.m_granularCompletionsDisabled = false;
		}

		public void DisableGranularCompletions()
		{
			if (!this.m_granularCompletionsDisabled)
			{
				this.m_granularCompletionsDisabled = true;
				long arg;
				long arg2;
				this.ReleaseAllClosedButUnfinalizedLogs(out arg, out arg2);
				GranularWriter.Tracer.TraceError<long, long>((long)this.GetHashCode(), "DisableGranularCompletions({0}): released 0x{1:X} to 0x{2:X}", arg, arg2);
			}
		}

		private void ThrowUnexpectedMessage(string msgContext)
		{
			throw new GranularReplicationTerminatedException(ReplayStrings.GranularReplicationMsgSequenceError(msgContext));
		}

		public void ProcessGranularLogCompleteMsg(GranularLogCompleteMsg msg)
		{
			if (this.m_lowestClosedGeneration == 0L || this.m_lowestClosedGeneration > msg.Generation)
			{
				GranularWriter.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "ProcessGranularLogCompleteMsg({0}): haven't found the completion of the first granule. Ignoring 0x{1:X}", this.DatabaseName, msg.Generation);
				return;
			}
			Exception ex = null;
			try
			{
				if (this.m_lowestClosedGeneration < msg.Generation)
				{
					GranularWriter.Tracer.TraceError<string, long, long>((long)this.GetHashCode(), "The server must have skipped verification.({0}): expecting 0x{1:X} got 0x{2:X}", this.DatabaseName, this.m_lowestClosedGeneration, msg.Generation);
					long num;
					long num2;
					this.ReleaseRangeOfClosedButUnfinalizedLogs(msg.Generation - 1L, out num, out num2);
				}
				string text = this.FormFullGranuleName(msg.Generation);
				ReplayStopwatch replayStopwatch = new ReplayStopwatch();
				replayStopwatch.Start();
				FileInfo fileInfo = new FileInfo(text);
				fileInfo.LastWriteTimeUtc = msg.LastWriteUtc;
				GranularWriter.Tracer.TracePerformance<string, long, ReplayStopwatch>((long)this.GetHashCode(), "GranularLogComplete({0},0x{1:X}) file timestamped at {2}", this.DatabaseName, msg.Generation, replayStopwatch);
				if (msg.ChecksumUsed == GranularLogCloseData.ChecksumAlgorithm.MD5)
				{
					this.ValidateLogFile(msg, text);
					GranularWriter.Tracer.TracePerformance<string, long, ReplayStopwatch>((long)this.GetHashCode(), "GranularLogComplete({0},0x{1:X}) validated at {2}", this.DatabaseName, msg.Generation, replayStopwatch);
				}
				string destFileName = this.FormInspectorLogfileName(msg.Generation);
				File.Move(text, destFileName);
				this.TrackInspectorGeneration(msg.Generation, msg.LastWriteUtc);
				GranularWriter.Tracer.TracePerformance<string, long, ReplayStopwatch>((long)this.GetHashCode(), "GranularLogComplete({0},0x{1:X}) moved at {2}", this.DatabaseName, msg.Generation, replayStopwatch);
				this.m_lowestClosedGeneration = msg.Generation + 1L;
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new GranularReplicationTerminatedException(string.Format("ProcessGranularLogCompleteMsg on 0x{0:X} failed: {1}", msg.Generation, ex.Message), ex);
			}
		}

		private void ValidateLogFile(GranularLogCompleteMsg msg, string fullSourceFileName)
		{
			if (msg.ChecksumUsed != GranularLogCloseData.ChecksumAlgorithm.MD5)
			{
				GranularWriter.Tracer.TraceError((long)this.GetHashCode(), "only MD5 supported for now");
				this.ThrowUnexpectedMessage("only MD5 supported for now");
			}
			Exception ex = null;
			try
			{
				using (SafeFileHandle safeFileHandle = LogCopy.OpenLogForRead(fullSourceFileName))
				{
					using (FileStream fileStream = LogCopy.OpenFileStream(safeFileHandle, true))
					{
						FileInfo fileInfo = new FileInfo(fullSourceFileName);
						if (fileInfo.Length != 1048576L)
						{
							throw new IOException(string.Format("Unexpected log file size: '{0}' has 0x{1:X} bytes", fullSourceFileName, fileInfo.Length));
						}
						byte[] buffer = new byte[1048576];
						int num = fileStream.Read(buffer, 0, 1048576);
						if (num != 1048576)
						{
							GranularWriter.Tracer.TraceError<int, int, string>((long)this.GetHashCode(), "ValidateLogFile. Expected {0} but got {1} bytes from {2}", 1048576, num, fullSourceFileName);
							throw new IOException(ReplayStrings.UnexpectedEOF(fullSourceFileName));
						}
						byte[] b;
						using (MessageDigestForNonCryptographicPurposes messageDigestForNonCryptographicPurposes = new MessageDigestForNonCryptographicPurposes())
						{
							b = messageDigestForNonCryptographicPurposes.ComputeHash(buffer);
						}
						if (!GranularWriter.BytesAreIdentical(b, msg.ChecksumBytes))
						{
							GranularWriter.Tracer.TraceError<string>((long)this.GetHashCode(), "ValidateLogFile: MD5 hash failure on '{0}'", fullSourceFileName);
							throw new GranularReplicationTerminatedException(string.Format("MD5 HASH fails on {0}", fullSourceFileName));
						}
					}
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new GranularReplicationTerminatedException(string.Format("ValidateLogFile on {0} failed: {1}", fullSourceFileName, ex.Message), ex);
			}
		}

		private readonly IPerfmonCounters m_perfmonCounters;

		private readonly IReplayConfiguration m_config;

		private ISetBroken m_setBroken;

		private long m_lowestClosedGeneration;

		private long m_currentGeneration;

		private DateTime? m_lastTimeFromServer;

		private GranularWriter.GranularLogState m_currentGenerationState;

		private JET_INSTANCE m_jetConsumer;

		private bool m_jetConsumerInitialized;

		private bool m_jetConsumerHealthy;

		private object m_granularLock = new object();

		private LogCopier m_logCopier;

		private bool m_granularCompletionsDisabled = true;

		private enum GranularLogState
		{
			Unknown,
			Open,
			Aborted,
			Expected
		}
	}
}

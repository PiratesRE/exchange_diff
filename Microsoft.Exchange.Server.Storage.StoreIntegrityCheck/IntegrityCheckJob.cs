using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class IntegrityCheckJob : IIntegrityCheckJob, IJobStateTracker, IJobProgressTracker
	{
		public IntegrityCheckJob(Guid jobGuid, Guid requestGuid, int mailboxNumber, Guid mailboxGuid, bool detectOnly, TaskId taskId, DateTime creationTime, JobSource jobSource, JobPriority jobPriority)
		{
			this.jobGuid = jobGuid;
			this.requestGuid = requestGuid;
			this.mailboxNumber = mailboxNumber;
			this.mailboxGuid = mailboxGuid;
			this.taskId = taskId;
			this.detectOnly = detectOnly;
			this.creationTime = creationTime;
			this.jobSource = jobSource;
			this.jobPriority = jobPriority;
			this.jobState = 0L;
			this.progressInfo = new ProgressInfo();
		}

		[Queryable]
		public Guid JobGuid
		{
			get
			{
				return this.jobGuid;
			}
		}

		[Queryable]
		public Guid RequestGuid
		{
			get
			{
				return this.requestGuid;
			}
		}

		[Queryable]
		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		[Queryable]
		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		[Queryable]
		public TaskId TaskId
		{
			get
			{
				return this.taskId;
			}
		}

		[Queryable]
		public bool DetectOnly
		{
			get
			{
				return this.detectOnly;
			}
		}

		[Queryable]
		public DateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		[Queryable]
		public JobSource Source
		{
			get
			{
				return this.jobSource;
			}
		}

		[Queryable]
		public JobPriority Priority
		{
			get
			{
				return this.jobPriority;
			}
		}

		[Queryable]
		public JobState State
		{
			get
			{
				return (JobState)Interlocked.Read(ref this.jobState);
			}
		}

		[Queryable]
		public short Progress
		{
			get
			{
				return this.progressInfo.Progress;
			}
		}

		public ProgressInfo CurrentProgress
		{
			get
			{
				return Interlocked.CompareExchange<ProgressInfo>(ref this.progressInfo, null, null);
			}
		}

		[Queryable]
		public TimeSpan TimeInServer
		{
			get
			{
				return this.progressInfo.TimeInServer;
			}
		}

		[Queryable]
		public DateTime? CompletedTime
		{
			get
			{
				return this.progressInfo.CompletedTime;
			}
		}

		[Queryable]
		public DateTime? LastExecutionTime
		{
			get
			{
				return this.progressInfo.LastExecutionTime;
			}
		}

		[Queryable]
		public int CorruptionsDetected
		{
			get
			{
				return this.progressInfo.CorruptionsDetected;
			}
		}

		[Queryable]
		public int CorruptionsFixed
		{
			get
			{
				return this.progressInfo.CorruptionsFixed;
			}
		}

		[Queryable]
		public ErrorCode Error
		{
			get
			{
				return this.progressInfo.Error;
			}
		}

		void IJobProgressTracker.Report(ProgressInfo newProgressInfo)
		{
			Interlocked.Exchange<ProgressInfo>(ref this.progressInfo, newProgressInfo);
		}

		void IJobStateTracker.MoveToState(JobState state)
		{
			Interlocked.Exchange(ref this.jobState, (long)state);
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, JobState>(0L, "Job {0} entered state {1}", this.JobGuid, state);
			}
		}

		public Properties GetProperties(StorePropTag[] propTags)
		{
			ProgressInfo currentProgress = this.CurrentProgress;
			Properties result = new Properties(propTags.Length);
			int i = 0;
			while (i < propTags.Length)
			{
				StorePropTag storePropTag = propTags[i];
				uint propTag = storePropTag.PropTag;
				if (propTag <= 268959747U)
				{
					if (propTag <= 268632067U)
					{
						if (propTag <= 268501064U)
						{
							if (propTag != 268435528U)
							{
								if (propTag != 268501064U)
								{
									goto IL_398;
								}
								result.Add(storePropTag, this.JobGuid);
							}
							else
							{
								result.Add(storePropTag, this.MailboxGuid);
							}
						}
						else if (propTag != 268566531U)
						{
							if (propTag != 268632067U)
							{
								goto IL_398;
							}
							result.Add(storePropTag, (int)this.TaskId);
						}
						else
						{
							result.Add(storePropTag, this.detectOnly ? 1 : 0);
						}
					}
					else if (propTag <= 268763200U)
					{
						if (propTag != 268697602U)
						{
							if (propTag != 268763200U)
							{
								goto IL_398;
							}
							result.Add(storePropTag, this.CreationTime);
						}
						else
						{
							result.Add(storePropTag, (short)this.State);
						}
					}
					else if (propTag != 268828736U)
					{
						if (propTag != 268894272U)
						{
							if (propTag != 268959747U)
							{
								goto IL_398;
							}
							result.Add(storePropTag, currentProgress.CorruptionsDetected);
						}
						else if (currentProgress.LastExecutionTime != null)
						{
							result.Add(storePropTag, currentProgress.LastExecutionTime);
						}
						else
						{
							result.Add(storePropTag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
						}
					}
					else if (currentProgress.CompletedTime != null)
					{
						result.Add(storePropTag, currentProgress.CompletedTime);
					}
					else
					{
						result.Add(storePropTag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
					}
				}
				else if (propTag <= 269222146U)
				{
					if (propTag <= 269090888U)
					{
						if (propTag != 269025283U)
						{
							if (propTag != 269090888U)
							{
								goto IL_398;
							}
							result.Add(storePropTag, this.RequestGuid);
						}
						else
						{
							result.Add(storePropTag, currentProgress.CorruptionsFixed);
						}
					}
					else if (propTag != 269156354U)
					{
						if (propTag != 269222146U)
						{
							goto IL_398;
						}
						if (currentProgress.Corruptions != null)
						{
							result.Add(storePropTag, IntegrityCheckJob.SerializedCorruptions(currentProgress.Corruptions));
						}
						else
						{
							result.Add(storePropTag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
						}
					}
					else
					{
						result.Add(storePropTag, currentProgress.Progress);
					}
				}
				else if (propTag <= 269352962U)
				{
					if (propTag != 269287426U)
					{
						if (propTag != 269352962U)
						{
							goto IL_398;
						}
						result.Add(storePropTag, (short)this.Priority);
					}
					else
					{
						result.Add(storePropTag, (short)this.Source);
					}
				}
				else if (propTag != 269418501U)
				{
					if (propTag != 269484035U)
					{
						if (propTag != 269549571U)
						{
							goto IL_398;
						}
						result.Add(storePropTag, (int)currentProgress.Error);
					}
					else
					{
						result.Add(storePropTag, this.MailboxNumber);
					}
				}
				else
				{
					result.Add(storePropTag, currentProgress.TimeInServer.TotalMilliseconds);
				}
				IL_3CA:
				i++;
				continue;
				IL_398:
				result.Add(storePropTag.ConvertToError(), LegacyHelper.BoxedErrorCodeNotFound);
				if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.OnlineIsintegTracer.TraceError<StorePropTag>(0L, "Unrecognized property {0}", storePropTag);
					goto IL_3CA;
				}
				goto IL_3CA;
			}
			return result;
		}

		internal static byte[] SerializedCorruptions(IList<Corruption> corruptions)
		{
			if (corruptions == null)
			{
				return null;
			}
			int num = Math.Min(corruptions.Count, 1129) * 58;
			byte[] array = new byte[num];
			int num2 = 0;
			foreach (Corruption corruption in corruptions)
			{
				ExchangeId exchangeId = corruption.FolderId ?? ExchangeId.Zero;
				ExchangeId exchangeId2 = corruption.MessageId ?? ExchangeId.Zero;
				num2 += ParseSerialize.SerializeInt32((int)corruption.CorruptionType, array, num2);
				num2 += ExchangeIdHelpers.To26ByteArray(exchangeId.Replid, exchangeId.Guid, exchangeId.Counter, array, num2);
				num2 += ExchangeIdHelpers.To26ByteArray(exchangeId2.Replid, exchangeId2.Guid, exchangeId2.Counter, array, num2);
				num2 += ParseSerialize.SerializeInt16(corruption.IsFixed ? 1 : 0, array, num2);
				if (num2 >= num)
				{
					break;
				}
			}
			return array;
		}

		private readonly Guid jobGuid;

		private readonly Guid requestGuid;

		private readonly Guid mailboxGuid;

		private readonly int mailboxNumber;

		private readonly TaskId taskId;

		private readonly bool detectOnly;

		private readonly DateTime creationTime;

		private readonly JobSource jobSource;

		private readonly JobPriority jobPriority;

		private long jobState;

		private ProgressInfo progressInfo;
	}
}

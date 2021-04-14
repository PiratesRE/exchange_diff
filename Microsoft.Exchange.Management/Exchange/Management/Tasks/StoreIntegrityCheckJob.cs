using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class StoreIntegrityCheckJob : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return StoreIntegrityCheckJob.schema;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				if (!(this.requestGuid != Guid.Empty))
				{
					return null;
				}
				if (this.jobGuid == Guid.Empty)
				{
					return new StoreIntegrityCheckJobIdentity(this.databaseId.Guid, this.requestGuid);
				}
				return new StoreIntegrityCheckJobIdentity(this.databaseId.Guid, this.requestGuid, this.jobGuid);
			}
		}

		public MailboxId Mailbox
		{
			get
			{
				if (this.mailboxGuid != Guid.Empty)
				{
					return new MailboxId(this.databaseId, this.mailboxGuid);
				}
				return null;
			}
		}

		public JobSource Source
		{
			get
			{
				return this.source;
			}
		}

		public JobPriority Priority
		{
			get
			{
				return this.priority;
			}
		}

		public bool DetectOnly
		{
			get
			{
				return (this.flags & JobFlags.DetectOnly) == JobFlags.DetectOnly;
			}
		}

		public JobState JobState
		{
			get
			{
				return this.state;
			}
		}

		public short Progress
		{
			get
			{
				return this.progress;
			}
		}

		public MailboxCorruptionType[] Tasks
		{
			get
			{
				return this.tasks;
			}
		}

		public DateTime? CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		public DateTime? FinishTime
		{
			get
			{
				return this.finishTime;
			}
		}

		public DateTime? LastExecutionTime
		{
			get
			{
				return this.lastExecutionTime;
			}
		}

		public int CorruptionsDetected
		{
			get
			{
				return this.corruptionsDetected;
			}
		}

		public int? ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public int CorruptionsFixed
		{
			get
			{
				return this.corruptionsFixed;
			}
		}

		public TimeSpan? TimeInServer
		{
			get
			{
				return this.timeInServer;
			}
		}

		public StoreIntegrityCheckJob.Corruption[] Corruptions
		{
			get
			{
				return this.corruptions;
			}
		}

		public StoreIntegrityCheckJob() : base(new SimpleProviderPropertyBag())
		{
		}

		internal StoreIntegrityCheckJob(SimpleProviderPropertyBag bag) : base(bag)
		{
		}

		internal StoreIntegrityCheckJob(DatabaseId databaseId, Guid requestGuid, JobFlags flags, MailboxCorruptionType[] taskIds) : this()
		{
			this.creationTime = new DateTime?(DateTime.UtcNow.ToLocalTime());
			this.flags = flags;
			this.tasks = taskIds;
			this.timeInServer = null;
		}

		internal StoreIntegrityCheckJob(DatabaseId databaseId, Guid mailboxGuid, Guid requestGuid, Guid jobGuid, JobFlags flags, MailboxCorruptionType[] taskIds, JobState state, JobSource source, JobPriority priority, short progress, DateTime? creationTime, DateTime? lastExecutionTime, DateTime? finishTime, TimeSpan? timeInServer, int? errorCode, int corruptionsDetected, int corruptionsFixed, StoreIntegrityCheckJob.Corruption[] corruptions) : this()
		{
			this.databaseId = databaseId;
			this.mailboxGuid = mailboxGuid;
			this.requestGuid = requestGuid;
			this.jobGuid = jobGuid;
			this.flags = flags;
			this.tasks = taskIds;
			this.state = state;
			this.source = source;
			this.priority = priority;
			this.progress = progress;
			this.creationTime = creationTime;
			this.lastExecutionTime = lastExecutionTime;
			this.finishTime = finishTime;
			this.timeInServer = timeInServer;
			this.errorCode = errorCode;
			this.corruptions = corruptions;
			this.corruptionsDetected = corruptionsDetected;
			this.corruptionsFixed = corruptionsFixed;
		}

		internal StoreIntegrityCheckJob(DatabaseId databaseId, PropValue[] propValues) : base(new SimpleProviderPropertyBag())
		{
			this.databaseId = databaseId;
			byte[] buffer = null;
			foreach (PropValue propValue in propValues)
			{
				if (!propValue.IsError() && !propValue.IsNull())
				{
					PropTag propTag = propValue.PropTag;
					if (propTag <= PropTag.IsIntegJobLastExecutionTime)
					{
						if (propTag <= PropTag.IsIntegJobTask)
						{
							if (propTag <= PropTag.IsIntegJobGuid)
							{
								if (propTag != PropTag.IsIntegJobMailboxGuid)
								{
									if (propTag == PropTag.IsIntegJobGuid)
									{
										this.jobGuid = propValue.GetGuid();
									}
								}
								else
								{
									this.mailboxGuid = propValue.GetGuid();
								}
							}
							else if (propTag != PropTag.IsIntegJobFlags)
							{
								if (propTag == PropTag.IsIntegJobTask)
								{
									this.tasks = new MailboxCorruptionType[]
									{
										(MailboxCorruptionType)propValue.GetInt()
									};
								}
							}
							else
							{
								this.flags = (JobFlags)propValue.GetInt();
							}
						}
						else if (propTag <= PropTag.IsIntegJobCreationTime)
						{
							if (propTag != PropTag.IsIntegJobState)
							{
								if (propTag == PropTag.IsIntegJobCreationTime)
								{
									this.creationTime = new DateTime?(propValue.GetDateTime().ToLocalTime());
								}
							}
							else
							{
								this.state = (JobState)propValue.GetShort();
							}
						}
						else if (propTag != PropTag.IsIntegJobFinishTime)
						{
							if (propTag == PropTag.IsIntegJobLastExecutionTime)
							{
								this.lastExecutionTime = new DateTime?(propValue.GetDateTime().ToLocalTime());
							}
						}
						else
						{
							this.finishTime = new DateTime?(propValue.GetDateTime().ToLocalTime());
						}
					}
					else if (propTag <= PropTag.IsIntegJobProgress)
					{
						if (propTag <= PropTag.IsIntegJobCorruptionsFixed)
						{
							if (propTag != PropTag.IsIntegJobCorruptionsDetected)
							{
								if (propTag == PropTag.IsIntegJobCorruptionsFixed)
								{
									this.corruptionsFixed = propValue.GetInt();
								}
							}
							else
							{
								this.corruptionsDetected = propValue.GetInt();
							}
						}
						else if (propTag != PropTag.IsIntegJobRequestGuid)
						{
							if (propTag == PropTag.IsIntegJobProgress)
							{
								this.progress = propValue.GetShort();
							}
						}
						else
						{
							this.requestGuid = propValue.GetGuid();
						}
					}
					else if (propTag <= PropTag.IsIntegJobSource)
					{
						if (propTag != PropTag.IsIntegJobCorruptions)
						{
							if (propTag == PropTag.IsIntegJobSource)
							{
								this.source = (JobSource)propValue.GetShort();
							}
						}
						else
						{
							buffer = propValue.GetBytes();
						}
					}
					else if (propTag != PropTag.IsIntegJobPriority)
					{
						if (propTag != PropTag.IsIntegJobTimeInServer)
						{
							if (propTag == PropTag.RtfSyncTrailingCount)
							{
								this.errorCode = new int?(propValue.GetInt());
								if (this.errorCode != null && this.errorCode == 0)
								{
									this.errorCode = null;
								}
							}
						}
						else
						{
							this.timeInServer = new TimeSpan?(TimeSpan.FromMilliseconds(propValue.GetDouble()));
						}
					}
					else
					{
						this.priority = (JobPriority)propValue.GetShort();
					}
				}
			}
			if (this.mailboxGuid != Guid.Empty)
			{
				this.corruptions = this.DeserializeCorruptions(this.mailboxGuid, buffer);
			}
		}

		internal static StoreIntegrityCheckJob Aggregate(IList<StoreIntegrityCheckJob> jobs)
		{
			if (jobs == null || jobs.Count == 0)
			{
				return null;
			}
			if (jobs.Count == 1)
			{
				return jobs[0];
			}
			HashSet<MailboxCorruptionType> hashSet = new HashSet<MailboxCorruptionType>();
			TimeSpan timeSpan = default(TimeSpan);
			List<StoreIntegrityCheckJob.Corruption> list = new List<StoreIntegrityCheckJob.Corruption>();
			DatabaseId databaseId = jobs[0].databaseId;
			Guid guid = jobs[0].requestGuid;
			Guid empty = jobs[0].mailboxGuid;
			Guid guid2 = jobs[0].jobGuid;
			JobFlags jobFlags = JobFlags.None;
			JobSource jobSource = jobs[0].source;
			JobPriority jobPriority = jobs[0].priority;
			JobState jobState = jobs[0].state;
			DateTime? dateTime = null;
			DateTime? dateTime2 = null;
			int num = 0;
			int num2 = 0;
			DateTime? dateTime3 = jobs[0].finishTime;
			long num3 = 0L;
			bool flag = false;
			for (int i = 0; i < jobs.Count; i++)
			{
				StoreIntegrityCheckJob storeIntegrityCheckJob = jobs[i];
				if (storeIntegrityCheckJob.mailboxGuid != empty)
				{
					empty = Guid.Empty;
				}
				if (!hashSet.Contains(storeIntegrityCheckJob.Tasks[0]))
				{
					hashSet.Add(storeIntegrityCheckJob.Tasks[0]);
				}
				if (dateTime == null)
				{
					dateTime = storeIntegrityCheckJob.creationTime;
				}
				else if (dateTime != null && storeIntegrityCheckJob.creationTime != null && dateTime.Value > storeIntegrityCheckJob.creationTime.Value)
				{
					dateTime = storeIntegrityCheckJob.creationTime;
				}
				if (dateTime2 == null)
				{
					dateTime2 = storeIntegrityCheckJob.lastExecutionTime;
				}
				if (dateTime2 != null && storeIntegrityCheckJob.lastExecutionTime != null && dateTime2 < storeIntegrityCheckJob.lastExecutionTime.Value)
				{
					dateTime2 = storeIntegrityCheckJob.lastExecutionTime;
				}
				if (dateTime3 != null && storeIntegrityCheckJob.finishTime != null)
				{
					if (dateTime3.Value < storeIntegrityCheckJob.finishTime.Value)
					{
						dateTime3 = storeIntegrityCheckJob.finishTime;
					}
				}
				else
				{
					dateTime3 = null;
				}
				if (storeIntegrityCheckJob.TimeInServer != null)
				{
					timeSpan += storeIntegrityCheckJob.TimeInServer.Value;
				}
				jobFlags |= storeIntegrityCheckJob.flags;
				if (storeIntegrityCheckJob.state != jobState)
				{
					if ((storeIntegrityCheckJob.state == JobState.Succeeded || storeIntegrityCheckJob.state == JobState.Failed) && (jobState == JobState.Succeeded || jobState == JobState.Failed))
					{
						jobState = JobState.Failed;
					}
					else
					{
						jobState = JobState.Running;
					}
				}
				num3 += (long)storeIntegrityCheckJob.progress;
				if (storeIntegrityCheckJob.corruptions != null)
				{
					list.AddRange(storeIntegrityCheckJob.corruptions);
				}
				num += storeIntegrityCheckJob.CorruptionsDetected;
				num2 += storeIntegrityCheckJob.CorruptionsFixed;
				if (storeIntegrityCheckJob.errorCode != null && storeIntegrityCheckJob.errorCode.Value != 0)
				{
					flag = true;
				}
			}
			num3 /= (long)jobs.Count;
			int? num4 = null;
			if (flag)
			{
				num4 = new int?(-2147467259);
			}
			return new StoreIntegrityCheckJob(databaseId, empty, guid, Guid.Empty, jobFlags, hashSet.ToArray<MailboxCorruptionType>(), jobState, jobSource, jobPriority, (short)num3, dateTime, dateTime2, dateTime3, new TimeSpan?(timeSpan), num4, num, num2, list.ToArray());
		}

		private StoreIntegrityCheckJob.Corruption[] DeserializeCorruptions(Guid mailboxGuid, byte[] buffer)
		{
			int num = 58;
			if (buffer == null)
			{
				return null;
			}
			int num2 = buffer.Length / num;
			StoreIntegrityCheckJob.Corruption[] array = new StoreIntegrityCheckJob.Corruption[num2];
			for (int i = 0; i < num2; i++)
			{
				int num3 = num * i;
				array[i] = default(StoreIntegrityCheckJob.Corruption);
				array[i].MailboxGuid = mailboxGuid;
				array[i].CorruptionType = (CorruptionType)BitConverter.ToInt32(buffer, num3);
				num3 += 4;
				array[i].FolderId = BitConverter.ToString(buffer, num3, 26).Replace("-", string.Empty);
				num3 += 26;
				array[i].MessageId = BitConverter.ToString(buffer, num3, 26).Replace("-", string.Empty);
				num3 += 26;
				array[i].IsFixed = (BitConverter.ToInt16(buffer, num3) != 0);
			}
			return array;
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<StoreIntegrityCheckRequestSchema>();

		private readonly DatabaseId databaseId;

		private readonly int corruptionsDetected;

		private readonly int corruptionsFixed;

		private readonly Guid requestGuid;

		private readonly Guid mailboxGuid;

		private readonly Guid jobGuid;

		private JobFlags flags;

		private readonly MailboxCorruptionType[] tasks;

		private readonly JobState state;

		private readonly JobSource source;

		private readonly JobPriority priority;

		private readonly short progress;

		private readonly DateTime? creationTime;

		private readonly DateTime? finishTime;

		private readonly DateTime? lastExecutionTime;

		private readonly TimeSpan? timeInServer;

		private readonly int? errorCode;

		private StoreIntegrityCheckJob.Corruption[] corruptions;

		[Serializable]
		public struct Corruption
		{
			public Guid MailboxGuid { get; internal set; }

			public CorruptionType CorruptionType { get; internal set; }

			public string FolderId { get; internal set; }

			public string MessageId { get; internal set; }

			public bool IsFixed { get; internal set; }

			public override string ToString()
			{
				return Strings.ISIntegCorruptionFormat(this.CorruptionType.ToString(), this.IsFixed);
			}
		}
	}
}

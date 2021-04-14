using System;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class StoreIntegrityCheckJob
	{
		public StoreIntegrityCheckJob()
		{
		}

		[DataMember(Name = "RequestGuid")]
		public Guid RequestGuid { get; set; }

		[DataMember(Name = "MailboxGuid")]
		public Guid MailboxGuid { get; set; }

		[DataMember(Name = "DatabaseGuid")]
		public Guid DatabaseGuid { get; set; }

		[DataMember(Name = "JobGuid")]
		public Guid JobGuid { get; set; }

		[DataMember(Name = "JobState")]
		public int JobState { get; set; }

		[DataMember(Name = "Progress")]
		public short Progress { get; set; }

		[DataMember(Name = "Priority")]
		public int Priority { get; set; }

		[DataMember(Name = "CorruptionsDetected")]
		public int CorruptionsDetected { get; set; }

		[DataMember(Name = "CorruptionsFixed")]
		public int CorruptionsFixed { get; set; }

		[DataMember(Name = "CreationTime")]
		public DateTime? CreationTime { get; set; }

		[DataMember(Name = "FinishTime")]
		public DateTime? FinishTime { get; set; }

		[DataMember(Name = "LastExecutionTime")]
		public DateTime? LastExecutionTime { get; set; }

		[DataMember(Name = "TimeInServer")]
		public TimeSpan? TimeInServer { get; set; }

		[DataMember(Name = "ErrorCode")]
		public int? ErrorCode { get; set; }

		internal StoreIntegrityCheckJob(PropValue[] propValues)
		{
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
										this.JobGuid = propValue.GetGuid();
									}
								}
								else
								{
									this.MailboxGuid = propValue.GetGuid();
								}
							}
							else if (propTag != PropTag.IsIntegJobFlags && propTag != PropTag.IsIntegJobTask)
							{
							}
						}
						else if (propTag <= PropTag.IsIntegJobCreationTime)
						{
							if (propTag != PropTag.IsIntegJobState)
							{
								if (propTag == PropTag.IsIntegJobCreationTime)
								{
									this.CreationTime = new DateTime?(propValue.GetDateTime().ToLocalTime());
								}
							}
							else
							{
								this.JobState = (int)propValue.GetShort();
							}
						}
						else if (propTag != PropTag.IsIntegJobFinishTime)
						{
							if (propTag == PropTag.IsIntegJobLastExecutionTime)
							{
								this.LastExecutionTime = new DateTime?(propValue.GetDateTime().ToLocalTime());
							}
						}
						else
						{
							this.FinishTime = new DateTime?(propValue.GetDateTime().ToLocalTime());
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
									this.CorruptionsFixed = propValue.GetInt();
								}
							}
							else
							{
								this.CorruptionsDetected = propValue.GetInt();
							}
						}
						else if (propTag != PropTag.IsIntegJobRequestGuid)
						{
							if (propTag == PropTag.IsIntegJobProgress)
							{
								this.Progress = propValue.GetShort();
							}
						}
						else
						{
							this.RequestGuid = propValue.GetGuid();
						}
					}
					else if (propTag <= PropTag.IsIntegJobSource)
					{
						if (propTag != PropTag.IsIntegJobCorruptions && propTag != PropTag.IsIntegJobSource)
						{
						}
					}
					else if (propTag != PropTag.IsIntegJobPriority)
					{
						if (propTag != PropTag.IsIntegJobTimeInServer)
						{
							if (propTag == PropTag.RtfSyncTrailingCount)
							{
								this.ErrorCode = new int?(propValue.GetInt());
							}
						}
						else
						{
							this.TimeInServer = new TimeSpan?(TimeSpan.FromMilliseconds(propValue.GetDouble()));
						}
					}
					else
					{
						this.Priority = (int)propValue.GetShort();
					}
				}
			}
		}

		public override string ToString()
		{
			return string.Format("RequestGuid: {0}, MailboxGuid: {1}, DatabaseGuid: {2}, JobGuid: {3}, JobState: {4}, Progress: {5}, Priority: {6}, CorruptionsDetected: {7}, CorruptionsFixed: {8}, CreationTime: {9}, FinishTime: {10}, LastExecutionTime: {11}, TimeInServer: {12}, ErrorCode: {13}", new object[]
			{
				this.RequestGuid,
				this.MailboxGuid,
				this.DatabaseGuid,
				this.JobGuid,
				this.JobState,
				this.Progress,
				this.Priority,
				this.CorruptionsDetected,
				this.CorruptionsFixed,
				(this.CreationTime != null) ? this.CreationTime.ToString() : "<null>",
				(this.FinishTime != null) ? this.FinishTime.ToString() : "<null>",
				(this.LastExecutionTime != null) ? this.LastExecutionTime.ToString() : "<null>",
				(this.TimeInServer != null) ? this.TimeInServer.ToString() : "<null>",
				(this.ErrorCode != null) ? this.ErrorCode.ToString() : "<null>"
			});
		}
	}
}

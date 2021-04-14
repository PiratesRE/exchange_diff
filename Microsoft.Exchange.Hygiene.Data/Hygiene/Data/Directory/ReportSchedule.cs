using System;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ReportSchedule : ConfigurablePropertyBag
	{
		public ReportSchedule(Guid tenantId, string scheduleName) : this(tenantId, scheduleName, DateTime.Today)
		{
		}

		public ReportSchedule(Guid tenantId, string scheduleName, DateTime scheduleStartTime) : this()
		{
			this.TenantId = tenantId;
			this.ScheduleName = scheduleName;
			this[ReportScheduleSchema.Id] = CombGuidGenerator.NewGuid();
			this.ScheduleStartTime = scheduleStartTime;
		}

		public ReportSchedule()
		{
		}

		public void SetExecutionStatus(Guid executionContextId, ReportExecutionStatusType executionStatus)
		{
			if (this.CurrentExecutionContextId != executionContextId)
			{
				throw new InvalidDataException(string.Format("The execution context Id does not match for schedule '{0}', tenant: {1}", this.ScheduleName ?? string.Empty, this.TenantId));
			}
			if (ReportSchedule.IsExecutionCompleted(executionStatus))
			{
				this.LastExecutionContextId = executionContextId;
				this.LastExecutionStatus = executionStatus;
				this.LastExecutionTime = DateTime.UtcNow;
				this.CurrentExecutionContextId = Guid.Empty;
				this.CurrentExecutionStatus = ReportExecutionStatusType.None;
				return;
			}
			this.CurrentExecutionStatus = executionStatus;
		}

		public void SetExecutionContext(Guid executionContextId)
		{
			this.CurrentExecutionContextId = executionContextId;
			this.CurrentExecutionStatus = ReportExecutionStatusType.Queued;
			this.LastScheduleTime = DateTime.UtcNow;
		}

		public DateTime GetNextScheduleTime()
		{
			DateTime result = SqlDateTime.MaxValue.Value;
			switch (this.ScheduleFrequency)
			{
			case ReportScheduleFrequencyType.Daily:
				result = this.LastScheduleTime.AddDays(1.0);
				break;
			case ReportScheduleFrequencyType.Monthly:
			{
				result = new DateTime(this.LastScheduleTime.Year, this.LastScheduleTime.Month, 1).AddMonths(1);
				DateTime dateTime = result.AddMonths(1).AddDays(-1.0);
				DateTime dateTime2 = result.AddDays((double)(this.ScheduleMask - 1));
				result = ((dateTime2 < dateTime) ? dateTime2 : dateTime);
				break;
			}
			}
			return result;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[ReportScheduleSchema.Id].ToString());
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[ReportScheduleSchema.TenantId];
			}
			set
			{
				this[ReportScheduleSchema.TenantId] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[ReportScheduleSchema.Enabled];
			}
			set
			{
				this[ReportScheduleSchema.Enabled] = value;
			}
		}

		public string ScheduleName
		{
			get
			{
				return this[ReportScheduleSchema.ScheduleName] as string;
			}
			private set
			{
				this[ReportScheduleSchema.ScheduleName] = value;
			}
		}

		public ReportScheduleFrequencyType ScheduleFrequency
		{
			get
			{
				return (ReportScheduleFrequencyType)this[ReportScheduleSchema.ScheduleFrequency];
			}
			set
			{
				this[ReportScheduleSchema.ScheduleFrequency] = value;
			}
		}

		public byte ScheduleMask
		{
			get
			{
				return (byte)this[ReportScheduleSchema.ScheduleMask];
			}
			set
			{
				this[ReportScheduleSchema.ScheduleMask] = value;
			}
		}

		public DateTime ScheduleStartTime
		{
			get
			{
				return (DateTime)this[ReportScheduleSchema.ScheduleStartTime];
			}
			private set
			{
				DateTime dateTime = value;
				if (dateTime < DateTime.Today)
				{
					dateTime = DateTime.Today;
				}
				this[ReportScheduleSchema.ScheduleStartTime] = dateTime;
			}
		}

		public string ReportName
		{
			get
			{
				return this[ReportScheduleSchema.ReportName] as string;
			}
			set
			{
				this[ReportScheduleSchema.ReportName] = value;
			}
		}

		public ReportFormatType ReportFormat
		{
			get
			{
				return (ReportFormatType)this[ReportScheduleSchema.ReportFormat];
			}
			set
			{
				this[ReportScheduleSchema.ReportFormat] = value;
			}
		}

		public string ReportSubject
		{
			get
			{
				return this[ReportScheduleSchema.ReportSubject] as string;
			}
			set
			{
				this[ReportScheduleSchema.ReportSubject] = value;
			}
		}

		public string ReportRecipients
		{
			get
			{
				return this[ReportScheduleSchema.ReportRecipients] as string;
			}
			set
			{
				this[ReportScheduleSchema.ReportRecipients] = value;
			}
		}

		public string ReportFilter
		{
			get
			{
				return this[ReportScheduleSchema.ReportFilter] as string;
			}
			set
			{
				this[ReportScheduleSchema.ReportFilter] = value;
			}
		}

		public string ReportLanguage
		{
			get
			{
				return this[ReportScheduleSchema.ReportLanguage] as string;
			}
			set
			{
				this[ReportScheduleSchema.ReportLanguage] = value;
			}
		}

		public Guid? BatchId
		{
			get
			{
				return (Guid?)this[ReportScheduleSchema.BatchId];
			}
			set
			{
				this[ReportScheduleSchema.BatchId] = value;
			}
		}

		public DateTime LastScheduleTime
		{
			get
			{
				return (DateTime)this[ReportScheduleSchema.LastScheduleTime];
			}
			private set
			{
				this[ReportScheduleSchema.LastScheduleTime] = value;
			}
		}

		public DateTime LastExecutionTime
		{
			get
			{
				return (DateTime)this[ReportScheduleSchema.LastExecutionTime];
			}
			private set
			{
				this[ReportScheduleSchema.LastExecutionTime] = value;
			}
		}

		public ReportExecutionStatusType LastExecutionStatus
		{
			get
			{
				return (ReportExecutionStatusType)this[ReportScheduleSchema.LastExecutionStatus];
			}
			private set
			{
				this[ReportScheduleSchema.LastExecutionStatus] = value;
			}
		}

		public Guid LastExecutionContextId
		{
			get
			{
				return (Guid)this[ReportScheduleSchema.LastExecutionContextId];
			}
			private set
			{
				this[ReportScheduleSchema.LastExecutionContextId] = value;
			}
		}

		public ReportExecutionStatusType CurrentExecutionStatus
		{
			get
			{
				return (ReportExecutionStatusType)this[ReportScheduleSchema.CurrentExecutionStatus];
			}
			private set
			{
				this[ReportScheduleSchema.CurrentExecutionStatus] = value;
			}
		}

		public Guid CurrentExecutionContextId
		{
			get
			{
				return (Guid)this[ReportScheduleSchema.CurrentExecutionContextId];
			}
			private set
			{
				this[ReportScheduleSchema.CurrentExecutionContextId] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(ReportScheduleSchema);
		}

		private static bool IsExecutionCompleted(ReportExecutionStatusType executionStatus)
		{
			return executionStatus == ReportExecutionStatusType.Completed || executionStatus == ReportExecutionStatusType.Failed || executionStatus == ReportExecutionStatusType.Cancelled;
		}
	}
}

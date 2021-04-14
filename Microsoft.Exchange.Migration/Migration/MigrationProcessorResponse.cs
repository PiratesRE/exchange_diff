using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationProcessorResponse
	{
		public MigrationProcessorResponse() : this(MigrationProcessorResult.Deleted, null)
		{
		}

		protected MigrationProcessorResponse(MigrationProcessorResult result, LocalizedException error = null)
		{
			this.Result = result;
			this.DebugInfo = null;
			this.Error = error;
			this.ClearPoison = false;
		}

		public MigrationProcessorResult Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		public TimeSpan? DelayTime
		{
			get
			{
				if (this.delayTime == null && this.Result == MigrationProcessorResult.Waiting)
				{
					return new TimeSpan?(this.DefaultDelay);
				}
				return this.delayTime;
			}
			set
			{
				if (value != null)
				{
					this.delayTime = new TimeSpan?(MigrationUtil.MinTimeSpan(this.MaxDelay, value.Value));
					return;
				}
				this.delayTime = null;
			}
		}

		public TimeSpan? ProcessingDuration { get; set; }

		public string DebugInfo { get; set; }

		public LocalizedException Error { get; set; }

		public ExDateTime? EarliestDelayedChildTime { get; set; }

		public bool ClearPoison { get; set; }

		protected virtual TimeSpan DefaultDelay
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorAverageWaitingJobDelay");
			}
		}

		protected virtual TimeSpan MaxDelay
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorMaxWaitingJobDelay");
			}
		}

		public static MigrationProcessorResponse Create(MigrationProcessorResult result, TimeSpan? delayTime = null, LocalizedException error = null)
		{
			MigrationProcessorResponse migrationProcessorResponse = new MigrationProcessorResponse(result, error);
			if (delayTime != null)
			{
				migrationProcessorResponse.DelayTime = delayTime;
			}
			return migrationProcessorResponse;
		}

		public static MigrationProcessorResponse CreateWaitingMax()
		{
			return MigrationProcessorResponse.Create(MigrationProcessorResult.Waiting, new TimeSpan?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorMaxWaitingJobDelay")), null);
		}

		public static T MergeResponses<T>(T current, T incoming) where T : MigrationProcessorResponse, new()
		{
			MigrationProcessorResult migrationProcessorResult = MigrationProcessorResult.Working;
			if (current.Result == incoming.Result)
			{
				migrationProcessorResult = current.Result;
			}
			else
			{
				foreach (MigrationProcessorResult migrationProcessorResult2 in MigrationProcessorResponse.JobResultPrecedence)
				{
					if (current.Result == migrationProcessorResult2 || incoming.Result == migrationProcessorResult2)
					{
						migrationProcessorResult = migrationProcessorResult2;
						break;
					}
				}
			}
			T t = Activator.CreateInstance<T>();
			t.Result = migrationProcessorResult;
			t.Merge(current, incoming);
			return t;
		}

		public override string ToString()
		{
			if (this.Result == MigrationProcessorResult.Waiting)
			{
				return string.Format("{0} ({1})", this.Result, this.DelayTime);
			}
			return string.Format("{0}", this.Result);
		}

		public virtual void Aggregate(MigrationProcessorResponse childResponse)
		{
			MigrationProcessorResult migrationProcessorResult = this.Result;
			foreach (MigrationProcessorResult migrationProcessorResult2 in MigrationProcessorResponse.JobResultPrecedence)
			{
				if (migrationProcessorResult == migrationProcessorResult2 || childResponse.Result == migrationProcessorResult2)
				{
					migrationProcessorResult = migrationProcessorResult2;
					break;
				}
			}
			if (childResponse.DelayTime != null)
			{
				ExDateTime exDateTime = ExDateTime.UtcNow + childResponse.DelayTime.Value;
				ExDateTime exDateTime2 = this.EarliestDelayedChildTime ?? ExDateTime.MaxValue;
				this.EarliestDelayedChildTime = new ExDateTime?((exDateTime < exDateTime2) ? exDateTime : exDateTime2);
			}
			this.result = migrationProcessorResult;
		}

		protected virtual void Merge(MigrationProcessorResponse left, MigrationProcessorResponse right)
		{
			if (this.Result == MigrationProcessorResult.Waiting)
			{
				if (left.Result == MigrationProcessorResult.Waiting && right.Result == MigrationProcessorResult.Waiting)
				{
					this.DelayTime = new TimeSpan?(MigrationUtil.MinTimeSpan(left.DelayTime.Value, right.DelayTime.Value));
					return;
				}
				if (left.Result == MigrationProcessorResult.Waiting)
				{
					this.DelayTime = left.DelayTime;
					return;
				}
				this.DelayTime = right.DelayTime;
			}
		}

		private static readonly MigrationProcessorResult[] JobResultPrecedence = new MigrationProcessorResult[]
		{
			MigrationProcessorResult.Working,
			MigrationProcessorResult.Waiting,
			MigrationProcessorResult.Completed,
			MigrationProcessorResult.Failed,
			MigrationProcessorResult.Suspended,
			MigrationProcessorResult.Deleted
		};

		private MigrationProcessorResult result;

		private TimeSpan? delayTime;
	}
}

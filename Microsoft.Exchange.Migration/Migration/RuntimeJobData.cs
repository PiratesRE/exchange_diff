using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RuntimeJobData : MigrationPersistableDictionary
	{
		public RuntimeJobData()
		{
		}

		public RuntimeJobData(Guid jobId) : this()
		{
			this.JobId = jobId;
		}

		internal event Action<IMigrationDataProvider> OnSaveRequested;

		public Guid JobId
		{
			get
			{
				string text = base.Get<string>("JobId");
				if (string.IsNullOrEmpty(text))
				{
					return Guid.Empty;
				}
				return Guid.Parse(text);
			}
			internal set
			{
				base.Set<string>("JobId", value.ToString());
			}
		}

		public ExDateTime? RunningJobStartTime
		{
			get
			{
				return base.GetNullable<ExDateTime>("RunningJobStartTime");
			}
			set
			{
				base.SetNullable<ExDateTime>("RunningJobStartTime", value);
			}
		}

		public ExDateTime? RunningJobInitialStartTime
		{
			get
			{
				return base.GetNullable<ExDateTime>("RunningJobInitialStartTime");
			}
			set
			{
				base.SetNullable<ExDateTime>("RunningJobInitialStartTime", value);
			}
		}

		public TimeSpan RunningJobElapsedTime
		{
			get
			{
				TimeSpan? nullable = base.GetNullable<TimeSpan>("RunningJobElapsedTime");
				if (nullable == null)
				{
					return TimeSpan.Zero;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				base.Set<TimeSpan>("RunningJobElapsedTime", value);
			}
		}

		public int? RunningJobNumItemsProcessed
		{
			get
			{
				return base.GetNullable<int>("RunningJobNumItemsProcessed");
			}
			set
			{
				base.SetNullable<int>("RunningJobNumItemsProcessed", value);
			}
		}

		public int? RunningJobNumItemsOutstanding
		{
			get
			{
				return base.GetNullable<int>("RunningJobNumItemsOutstanding");
			}
			set
			{
				base.SetNullable<int>("RunningJobNumItemsOutstanding", value);
			}
		}

		public string RunningJobDebugInfo
		{
			get
			{
				return base.Get<string>("RunningJobDebugInfo");
			}
			set
			{
				base.Set<string>("RunningJobDebugInfo", value);
			}
		}

		public int RunningJobNumRuns
		{
			get
			{
				int? nullable = base.GetNullable<int>("RunningJobNumRuns");
				if (nullable == null)
				{
					return 0;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				base.Set<int>("RunningJobNumRuns", value);
			}
		}

		public string JobHistory
		{
			get
			{
				return base.Get<string>("JobHistory") ?? string.Empty;
			}
			set
			{
				base.Set<string>("JobHistory", value);
			}
		}

		public bool IsRunning
		{
			get
			{
				return this.RunningJobInitialStartTime != null;
			}
		}

		public static RuntimeJobData Deserialize(Guid jobId, string serializedData)
		{
			RuntimeJobData runtimeJobData = new RuntimeJobData(jobId);
			if (!string.IsNullOrEmpty(serializedData))
			{
				runtimeJobData.DeserializeData(serializedData);
			}
			return runtimeJobData;
		}

		public void InitializeRunningJobSettings()
		{
			this.RunningJobStartTime = null;
			this.RunningJobInitialStartTime = null;
			this.RunningJobElapsedTime = TimeSpan.Zero;
			this.RunningJobNumRuns = 0;
			this.RunningJobNumItemsProcessed = null;
			this.RunningJobNumItemsOutstanding = null;
			this.RunningJobDebugInfo = null;
		}

		public void ResetRunspaceData()
		{
			if (this.RunningJobInitialStartTime == null)
			{
				throw new MigrationDataCorruptionException("always expect to have a running job set");
			}
			TimeSpan timeSpan = ExDateTime.UtcNow - this.RunningJobInitialStartTime.Value;
			if (this.RunningJobNumItemsProcessed != null && this.RunningJobNumItemsProcessed.Value > 0)
			{
				MigrationLogger.Log(MigrationEventType.Information, "resetting job {0} overall time {1} actual time {2} num of runs {3} num items processed {4} num items outstanding {5} debug {6}", new object[]
				{
					this.JobId,
					timeSpan,
					this.RunningJobElapsedTime,
					this.RunningJobNumRuns,
					this.RunningJobNumItemsProcessed,
					this.RunningJobNumItemsOutstanding,
					this.RunningJobDebugInfo
				});
				this.JobHistory = MigrationHelper.AppendDiagnosticHistory(this.JobHistory, new string[]
				{
					this.JobId.ToString(),
					timeSpan.ToString(),
					this.RunningJobElapsedTime.TotalSeconds.ToString(),
					this.RunningJobNumRuns.ToString(),
					this.RunningJobNumItemsProcessed.ToString(),
					this.RunningJobNumItemsOutstanding.ToString(),
					this.RunningJobDebugInfo
				});
			}
			this.InitializeRunningJobSettings();
		}

		public MigrationProcessorResult CloseRunspace(IMigrationDataProvider dataProvider, LegacyMigrationJobProcessorResponse response, bool supportsInterrupting)
		{
			bool flag = response.Result == MigrationProcessorResult.Completed;
			if (!flag && supportsInterrupting)
			{
				if (response.Result == MigrationProcessorResult.Waiting)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "interrupting job because it's waiting", new object[0]);
					flag = true;
				}
				else if (response.Result == MigrationProcessorResult.Working && (response.NumItemsOutstanding == null || response.NumItemsOutstanding.Value <= 0))
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "interrupting job because no outstanding items {0}", new object[]
					{
						response.NumItemsOutstanding
					});
					flag = true;
				}
			}
			if (flag)
			{
				this.ResetRunspaceData();
			}
			else
			{
				if (this.RunningJobStartTime == null)
				{
					throw new MigrationDataCorruptionException("always expect to have a start time if job is set");
				}
				this.RunningJobElapsedTime = ExDateTime.UtcNow - this.RunningJobStartTime.Value;
				this.RunningJobNumRuns++;
				this.RunningJobNumItemsProcessed = response.NumItemsProcessed;
				this.RunningJobNumItemsOutstanding = response.NumItemsOutstanding;
				this.RunningJobDebugInfo = response.DebugInfo;
				if (this.RunningJobNumItemsProcessed != null && this.RunningJobNumItemsProcessed.Value > 0)
				{
					MigrationLogger.Log(MigrationEventType.Information, "job {0} finished a run of {1} with time {2} with debug {3}, items processed {4} items outstanding {5} but not removing", new object[]
					{
						this.JobId,
						this.RunningJobNumRuns,
						this.RunningJobElapsedTime,
						this.RunningJobDebugInfo,
						this.RunningJobNumItemsProcessed,
						this.RunningJobNumItemsOutstanding
					});
				}
			}
			this.RunningJobStartTime = null;
			this.SaveProperties(dataProvider);
			return response.Result;
		}

		internal void SaveProperties(IMigrationDataProvider dataProvider)
		{
			if (this.OnSaveRequested != null)
			{
				this.OnSaveRequested(dataProvider);
			}
		}
	}
}

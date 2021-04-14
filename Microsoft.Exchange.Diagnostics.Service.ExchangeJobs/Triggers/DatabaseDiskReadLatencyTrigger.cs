using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Common;
using Microsoft.Exchange.LogAnalyzer.Analyzers.Perflog;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Triggers
{
	public class DatabaseDiskReadLatencyTrigger : PerInstanceTrigger
	{
		static DatabaseDiskReadLatencyTrigger()
		{
			DatabaseDiskReadLatencyTrigger.excludedInstances = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"Information Store/_Total"
			};
			DatabaseDiskReadLatencyTrigger.additionalContext = new HashSet<DiagnosticMeasurement>(DiagnosticMeasurement.CounterFilterComparer.Comparer)
			{
				DatabaseDiskReadLatencyTrigger.readsAverageLatency
			};
		}

		public DatabaseDiskReadLatencyTrigger(IJob job) : base(job, "MSExchange Database ==> Instances\\(Information Store.+?\\)\\\\I/O Database Reads Average Latency", new PerfLogCounterTrigger.TriggerConfiguration("DatabaseDiskReadLatencyTrigger", double.NaN, 250.0, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(10.0), 0), DatabaseDiskReadLatencyTrigger.additionalContext, DatabaseDiskReadLatencyTrigger.excludedInstances)
		{
			this.allDatabasesOnLocalServer = null;
			this.lastMailboxDatabaseRefreshTime = DateTime.MinValue;
			this.databaseRefreshFrequency = Configuration.GetConfigTimeSpan("DatabaseRefreshFrequency", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromDays(1.0));
		}

		protected override bool ShouldTrigger(PerfLogCounterTrigger.SurpassedThresholdContext context)
		{
			if (base.ShouldTrigger(context))
			{
				int num = 0;
				float num2 = 0f;
				string text = null;
				List<MailboxDatabase> list = null;
				this.allDatabasesOnLocalServer = this.GetAllDatabasesOnLocalServer();
				if (this.allDatabasesOnLocalServer != null)
				{
					if (!string.IsNullOrWhiteSpace(context.Counter.InstanceName))
					{
						text = this.GetDatabaseNameFromInstanceName(context.Counter.InstanceName);
					}
					if (!string.IsNullOrEmpty(text))
					{
						string databaseGroupFromName = this.GetDatabaseGroupFromName(text);
						list = this.PopulateDatabasesOnSameDisk(databaseGroupFromName);
					}
					if (list != null && list.Count > 0)
					{
						foreach (MailboxDatabase mailboxDatabase in list)
						{
							DiagnosticMeasurement measure = DiagnosticMeasurement.GetMeasure(context.Counter.MachineName, DatabaseDiskReadLatencyTrigger.readsAverageLatency.ObjectName, DatabaseDiskReadLatencyTrigger.readsAverageLatency.CounterName, string.Format("Information Store - {0}/_Total", mailboxDatabase.Name));
							ValueStatistics valueStatistics;
							if (context.AdditionalData.TryGetValue(measure, out valueStatistics))
							{
								num2 += valueStatistics.Mean.Value;
								num++;
							}
						}
						if (num > 0 && (double)(num2 / (float)num) <= 250.0)
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		private string GetDatabaseNameFromInstanceName(string instanceName)
		{
			string result = null;
			string[] array = instanceName.Split(new char[]
			{
				'/'
			});
			if (array.Length >= 1)
			{
				int num = array[0].IndexOf('-', 0);
				if (num != -1)
				{
					result = array[0].Substring(num + 1).Trim();
				}
			}
			return result;
		}

		private IEnumerable<MailboxDatabase> GetAllDatabasesOnLocalServer()
		{
			IEnumerable<MailboxDatabase> enumerable = this.allDatabasesOnLocalServer;
			if ((enumerable == null || DateTime.UtcNow - this.lastMailboxDatabaseRefreshTime >= this.databaseRefreshFrequency) && MailboxDatabase.TryDiscoverLocalMailboxDatabases(out enumerable))
			{
				this.lastMailboxDatabaseRefreshTime = DateTime.UtcNow;
			}
			return enumerable;
		}

		private string GetDatabaseGroupFromName(string databaseName)
		{
			if (!string.IsNullOrWhiteSpace(databaseName) && this.allDatabasesOnLocalServer != null)
			{
				foreach (MailboxDatabase mailboxDatabase in this.allDatabasesOnLocalServer)
				{
					if (mailboxDatabase.Name.Equals(databaseName))
					{
						return mailboxDatabase.DatabaseGroup;
					}
				}
			}
			return null;
		}

		private List<MailboxDatabase> PopulateDatabasesOnSameDisk(string databaseGroup)
		{
			List<MailboxDatabase> list = null;
			if (!string.IsNullOrWhiteSpace(databaseGroup) && this.allDatabasesOnLocalServer != null)
			{
				list = new List<MailboxDatabase>();
				foreach (MailboxDatabase mailboxDatabase in this.allDatabasesOnLocalServer)
				{
					if (databaseGroup.Equals(mailboxDatabase.DatabaseGroup))
					{
						list.Add(mailboxDatabase);
					}
				}
			}
			return list;
		}

		private const double ReadsAverageLatencyThreshold = 250.0;

		private static readonly HashSet<string> excludedInstances;

		private static readonly HashSet<DiagnosticMeasurement> additionalContext;

		private static readonly DiagnosticMeasurement readsAverageLatency = DiagnosticMeasurement.GetMeasure("MSExchange Database ==> Instances", "I/O Database Reads Average Latency");

		private readonly TimeSpan databaseRefreshFrequency;

		private DateTime lastMailboxDatabaseRefreshTime;

		private IEnumerable<MailboxDatabase> allDatabasesOnLocalServer;
	}
}

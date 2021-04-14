using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Common;
using Microsoft.Exchange.LogAnalyzer.Extensions.Perflog;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.CalculatedCounters
{
	public sealed class MailboxDatabaseCalculatedCounters : ICalculatedCounter
	{
		static MailboxDatabaseCalculatedCounters()
		{
			Dictionary<string, Func<float[], float>> dictionary = new Dictionary<string, Func<float[], float>>(StringComparer.OrdinalIgnoreCase);
			dictionary.Add("Consumed Megabytes", (float[] propertyValues) => MailboxDatabaseCalculatedCounters.ConvertBytesToMegabytes(propertyValues[0]));
			dictionary.Add("Available New Mailbox Space In Megabytes", (float[] propertyValues) => MailboxDatabaseCalculatedCounters.ConvertBytesToMegabytes(propertyValues[0]));
			dictionary.Add("Logical Consumed Megabytes", (float[] propertyValues) => MailboxDatabaseCalculatedCounters.ConvertBytesToMegabytes(propertyValues[0]));
			dictionary.Add("Disconnected Logical Consumed Megabytes", (float[] propertyValues) => MailboxDatabaseCalculatedCounters.ConvertBytesToMegabytes(propertyValues[0] + propertyValues[1]));
			dictionary.Add("Mailbox Count", (float[] propertyValues) => propertyValues[0]);
			dictionary.Add("Disconnected Mailbox Count", (float[] propertyValues) => propertyValues[0]);
			dictionary.Add("Log Consumed Megabytes", (float[] propertyValues) => MailboxDatabaseCalculatedCounters.ConvertBytesToMegabytes(propertyValues[0]));
			dictionary.Add("Catalog Consumed Megabytes", (float[] propertyValues) => MailboxDatabaseCalculatedCounters.ConvertBytesToMegabytes(propertyValues[0]));
			dictionary.Add("Logical To Physical Size Ratio", (float[] propertyValues) => propertyValues[0]);
			MailboxDatabaseCalculatedCounters.CounterLogic = dictionary;
		}

		public MailboxDatabaseCalculatedCounters()
		{
			this.databaseRefreshFrequency = Configuration.GetConfigTimeSpan("MailboxDatabaseCalculatedCounterADRefreshFrequency", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(7.0), TimeSpan.FromDays(1.0));
			this.statisticsRefreshFrequency = Configuration.GetConfigTimeSpan("MailboxDatabaseCalculatedCounterStatisticsRefreshFrequency", TimeSpan.FromMinutes(5.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			this.lastMailboxDatabaseNameRefresh = DateTime.MinValue;
			this.lastStatisticsRefresh = DateTime.MinValue;
			this.IsValidRole = ServerRole.IsRole("Mailbox");
		}

		internal bool IsValidRole { get; set; }

		public void OnLogHeader(List<KeyValuePair<int, DiagnosticMeasurement>> currentInputCounters)
		{
		}

		public void OnLogLine(Dictionary<DiagnosticMeasurement, float?> countersAndValues, DateTime? timestamp = null)
		{
			if (!this.IsValidRole)
			{
				return;
			}
			Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> dictionary;
			if (this.TryRefreshCounters(out dictionary))
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (KeyValuePair<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> keyValuePair in dictionary)
				{
					foreach (MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue diagnosticMeasurementValue in keyValuePair.Value)
					{
						if (!diagnosticMeasurementValue.Measure.InstanceName.Equals("_Total", StringComparison.OrdinalIgnoreCase) || hashSet.Add(diagnosticMeasurementValue.Measure.CounterName))
						{
							countersAndValues.Add(diagnosticMeasurementValue.Measure, diagnosticMeasurementValue.Value);
						}
					}
				}
			}
		}

		internal static float ConvertBytesToMegabytes(float value)
		{
			return value / 1024f / 1024f;
		}

		internal static Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> CreateCounters(IEnumerable<MailboxDatabase> databases)
		{
			if (databases == null)
			{
				throw new ArgumentNullException("databases");
			}
			Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> dictionary = new Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>>();
			Dictionary<string, MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue> dictionary2 = new Dictionary<string, MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>(StringComparer.OrdinalIgnoreCase);
			foreach (MailboxDatabase mailboxDatabase in databases)
			{
				if (!dictionary.ContainsKey(mailboxDatabase))
				{
					List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue> list = new List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>(MailboxDatabaseCalculatedCounters.CounterNames.Count * 2);
					foreach (string text in MailboxDatabaseCalculatedCounters.CounterNames)
					{
						DiagnosticMeasurement measure = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchangeIS Store", text, mailboxDatabase.Name);
						list.Add(new MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue(measure));
						MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue diagnosticMeasurementValue;
						if (!dictionary2.TryGetValue(text, out diagnosticMeasurementValue))
						{
							measure = DiagnosticMeasurement.GetMeasure(Environment.MachineName, "MSExchangeIS Store", text, "_Total");
							diagnosticMeasurementValue = new MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue(measure);
							dictionary2.Add(text, diagnosticMeasurementValue);
						}
						list.Add(diagnosticMeasurementValue);
					}
					dictionary.Add(mailboxDatabase, list);
				}
			}
			return dictionary;
		}

		internal static bool TryCreateCounters(IEnumerable<MailboxDatabase> databases, out Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> counters)
		{
			if (databases == null)
			{
				throw new ArgumentNullException("databases");
			}
			counters = null;
			Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> dictionary = MailboxDatabaseCalculatedCounters.CreateCounters(databases);
			List<MailboxDatabase> list = new List<MailboxDatabase>(dictionary.Count);
			foreach (KeyValuePair<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> keyValuePair in dictionary)
			{
				Dictionary<string, float> dictionary2;
				if (!keyValuePair.Key.TryLoadStatistics(out dictionary2))
				{
					list.Add(keyValuePair.Key);
				}
				else
				{
					foreach (MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue diagnosticMeasurementValue in keyValuePair.Value)
					{
						string[] array;
						Func<float[], float> func;
						if (MailboxDatabaseCalculatedCounters.CounterProperties.TryGetValue(diagnosticMeasurementValue.Measure.CounterName, out array) && MailboxDatabaseCalculatedCounters.CounterLogic.TryGetValue(diagnosticMeasurementValue.Measure.CounterName, out func))
						{
							bool flag = true;
							float[] array2 = new float[array.Length];
							for (int i = 0; i < array.Length; i++)
							{
								if (!dictionary2.TryGetValue(array[i], out array2[i]))
								{
									flag = false;
									break;
								}
							}
							if (!flag)
							{
								list.Add(keyValuePair.Key);
								break;
							}
							if (diagnosticMeasurementValue.Value == null)
							{
								diagnosticMeasurementValue.Value = new float?(0f);
							}
							MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue diagnosticMeasurementValue2 = diagnosticMeasurementValue;
							float? value = diagnosticMeasurementValue2.Value;
							float num = func(array2);
							diagnosticMeasurementValue2.Value = ((value != null) ? new float?(value.GetValueOrDefault() + num) : null);
						}
					}
				}
			}
			foreach (MailboxDatabase key in list)
			{
				dictionary.Remove(key);
			}
			bool flag2 = dictionary.Any<KeyValuePair<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>>>();
			if (flag2)
			{
				counters = dictionary;
			}
			return flag2;
		}

		internal void RefreshCounters(bool updateDatabases, bool updateStatistics)
		{
			if (updateDatabases)
			{
				IEnumerable<MailboxDatabase> enumerable;
				if (MailboxDatabase.TryDiscoverLocalMailboxDatabases(out enumerable))
				{
					this.currentDatabases = enumerable;
				}
				else
				{
					this.lastMailboxDatabaseNameRefresh = DateTime.MinValue;
					Log.LogErrorMessage("Unable to discover any mailbox databases in AD for the '{0}' server.", new object[]
					{
						Environment.MachineName
					});
				}
			}
			if (updateStatistics && this.currentDatabases != null)
			{
				Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> dictionary;
				if (MailboxDatabaseCalculatedCounters.TryCreateCounters(this.currentDatabases, out dictionary))
				{
					this.currentCounters = dictionary;
				}
				else
				{
					this.lastStatisticsRefresh = DateTime.MinValue;
					Log.LogErrorMessage("Unable to create counters.", new object[0]);
				}
			}
			this.doingWork = false;
		}

		private bool TryRefreshCounters(out Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> currentCounters)
		{
			bool updateDatabases = DateTime.UtcNow - this.lastMailboxDatabaseNameRefresh >= this.databaseRefreshFrequency || this.currentCounters == null || this.currentDatabases == null;
			bool updateStatistics = updateDatabases | DateTime.UtcNow - this.lastStatisticsRefresh >= this.statisticsRefreshFrequency;
			if ((updateDatabases || updateStatistics) && !this.doingWork)
			{
				if (updateDatabases)
				{
					this.lastMailboxDatabaseNameRefresh = DateTime.UtcNow;
				}
				if (updateStatistics)
				{
					this.lastStatisticsRefresh = DateTime.UtcNow;
				}
				this.doingWork = true;
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					this.RefreshCounters(updateDatabases, updateStatistics);
				});
			}
			currentCounters = this.currentCounters;
			return currentCounters != null;
		}

		public const string MailboxDatabaseCalculatedCounterADRefreshFrequencyProperty = "MailboxDatabaseCalculatedCounterADRefreshFrequency";

		public const string MailboxDatabaseCalculatedCounterStatisticsRefreshFrequencyProperty = "MailboxDatabaseCalculatedCounterStatisticsRefreshFrequency";

		internal const string MSExchangeISStoreObjectName = "MSExchangeIS Store";

		internal const string ConsumedMegabytesCounterName = "Consumed Megabytes";

		internal const string AvailableNewMailboxSpaceInMegabytesCounterName = "Available New Mailbox Space In Megabytes";

		internal const string LogicalConsumedMegabytesCounterName = "Logical Consumed Megabytes";

		internal const string DisconnectedLogicalConsumedMegabytesCounterName = "Disconnected Logical Consumed Megabytes";

		internal const string CatalogConsumedMegabytesCounterName = "Catalog Consumed Megabytes";

		internal const string LogConsumedMegabytesCounterName = "Log Consumed Megabytes";

		internal const string MailboxCountCounterName = "Mailbox Count";

		internal const string DisconnectedMailboxCountCounterName = "Disconnected Mailbox Count";

		internal const string LogicalPhysicalSizeRatioCounterName = "Logical To Physical Size Ratio";

		internal const string TotalInstanceName = "_Total";

		private static readonly List<string> CounterNames = new List<string>
		{
			"Consumed Megabytes",
			"Available New Mailbox Space In Megabytes",
			"Logical Consumed Megabytes",
			"Disconnected Logical Consumed Megabytes",
			"Mailbox Count",
			"Disconnected Mailbox Count",
			"Catalog Consumed Megabytes",
			"Log Consumed Megabytes",
			"Logical To Physical Size Ratio"
		};

		private static readonly Dictionary<string, string[]> CounterProperties = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"Consumed Megabytes",
				new string[]
				{
					"DatabasePhysicalUsedSize"
				}
			},
			{
				"Available New Mailbox Space In Megabytes",
				new string[]
				{
					"AvailableNewMailboxSpace"
				}
			},
			{
				"Logical Consumed Megabytes",
				new string[]
				{
					"DatabaseLogicalSize"
				}
			},
			{
				"Disconnected Logical Consumed Megabytes",
				new string[]
				{
					"DisconnectedTotalItemSize",
					"DisconnectedTotalDeletedItemSize"
				}
			},
			{
				"Mailbox Count",
				new string[]
				{
					"MailboxCount"
				}
			},
			{
				"Disconnected Mailbox Count",
				new string[]
				{
					"DisconnectedMailboxCount"
				}
			},
			{
				"Log Consumed Megabytes",
				new string[]
				{
					"LogSize"
				}
			},
			{
				"Catalog Consumed Megabytes",
				new string[]
				{
					"CatalogSize"
				}
			},
			{
				"Logical To Physical Size Ratio",
				new string[]
				{
					"LogicalPhysicalSizeRatio"
				}
			}
		};

		private static readonly Dictionary<string, Func<float[], float>> CounterLogic;

		private readonly TimeSpan databaseRefreshFrequency;

		private readonly TimeSpan statisticsRefreshFrequency;

		private Dictionary<MailboxDatabase, List<MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue>> currentCounters;

		private IEnumerable<MailboxDatabase> currentDatabases;

		private DateTime lastMailboxDatabaseNameRefresh;

		private DateTime lastStatisticsRefresh;

		private volatile bool doingWork;

		internal sealed class DiagnosticMeasurementValue
		{
			public DiagnosticMeasurementValue(DiagnosticMeasurement measure)
			{
				if (measure == null)
				{
					throw new ArgumentNullException("measure");
				}
				this.measure = measure;
			}

			public DiagnosticMeasurementValue(MailboxDatabaseCalculatedCounters.DiagnosticMeasurementValue other)
			{
				this.measure = other.Measure;
				this.Value = other.Value;
			}

			public DiagnosticMeasurement Measure
			{
				get
				{
					return this.measure;
				}
			}

			public float? Value { get; set; }

			private readonly DiagnosticMeasurement measure;
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.EDiscovery;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Logging;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure
{
	internal class Recorder
	{
		public Recorder(ISearchPolicy policy)
		{
			this.stopwatch.Start();
			this.policy = policy;
		}

		public long Timestamp
		{
			get
			{
				return this.stopwatch.ElapsedMilliseconds + this.offset;
			}
		}

		public IEnumerable<KeyValuePair<string, Recorder.Record>> Records
		{
			get
			{
				return this.records;
			}
		}

		public long ConversionTime { get; set; }

		public long LoggingTime { get; set; }

		public static void Trace(long id, TraceType traceType, object parameter1)
		{
			if (ExTraceGlobals.WebServiceTracer.IsTraceEnabled(traceType))
			{
				string formatString = "{0}";
				switch (traceType)
				{
				case TraceType.DebugTrace:
					ExTraceGlobals.WebServiceTracer.TraceDebug(id, formatString, new object[]
					{
						parameter1
					});
					return;
				case TraceType.WarningTrace:
					ExTraceGlobals.WebServiceTracer.TraceWarning(id, formatString, new object[]
					{
						parameter1
					});
					return;
				case TraceType.ErrorTrace:
					ExTraceGlobals.WebServiceTracer.TraceError(id, formatString, new object[]
					{
						parameter1
					});
					return;
				case TraceType.PerformanceTrace:
					ExTraceGlobals.WebServiceTracer.TracePerformance(id, formatString, new object[]
					{
						parameter1
					});
					return;
				}
				ExTraceGlobals.WebServiceTracer.Information(id, formatString, new object[]
				{
					parameter1
				});
			}
		}

		public static void Trace(long id, TraceType traceType, object parameter1, object parameter2)
		{
			if (ExTraceGlobals.WebServiceTracer.IsTraceEnabled(traceType))
			{
				string formatString = "{0} {1}";
				switch (traceType)
				{
				case TraceType.DebugTrace:
					ExTraceGlobals.WebServiceTracer.TraceDebug(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				case TraceType.WarningTrace:
					ExTraceGlobals.WebServiceTracer.TraceWarning(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				case TraceType.ErrorTrace:
					ExTraceGlobals.WebServiceTracer.TraceError(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				case TraceType.PerformanceTrace:
					ExTraceGlobals.WebServiceTracer.TracePerformance(id, formatString, new object[]
					{
						parameter1,
						parameter2
					});
					return;
				}
				ExTraceGlobals.WebServiceTracer.Information(id, formatString, new object[]
				{
					parameter1,
					parameter2
				});
			}
		}

		public static void Trace(long id, TraceType traceType, params object[] parameters)
		{
			if (parameters != null && parameters.Length > 0 && ExTraceGlobals.WebServiceTracer.IsTraceEnabled(traceType))
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < parameters.Length; i++)
				{
					stringBuilder.AppendFormat("{{{0}}}", i);
				}
				string formatString = stringBuilder.ToString();
				switch (traceType)
				{
				case TraceType.DebugTrace:
					ExTraceGlobals.WebServiceTracer.TraceDebug(id, formatString, parameters);
					return;
				case TraceType.WarningTrace:
					ExTraceGlobals.WebServiceTracer.TraceWarning(id, formatString, parameters);
					return;
				case TraceType.ErrorTrace:
					ExTraceGlobals.WebServiceTracer.TraceError(id, formatString, parameters);
					return;
				case TraceType.PerformanceTrace:
					ExTraceGlobals.WebServiceTracer.TracePerformance(id, formatString, parameters);
					return;
				}
				ExTraceGlobals.WebServiceTracer.Information(id, formatString, parameters);
			}
		}

		public Recorder.Record Start(string description, TraceType traceType = TraceType.InfoTrace, bool isAggregate = true)
		{
			return new Recorder.Record
			{
				Description = description,
				TraceType = traceType,
				IsAggregateRecord = isAggregate
			};
		}

		public void End(Recorder.Record record)
		{
			this.SafeLog(delegate
			{
				long timestamp = this.Timestamp;
				try
				{
					record.Attributes["DURATION"] = (double)this.Timestamp - record.StartTime;
					if (!record.IsAggregateRecord || record.TraceType == TraceType.ErrorTrace || record.TraceType == TraceType.FatalTrace)
					{
						this.WriteLog(record);
					}
					if (record.IsAggregateRecord)
					{
						this.AppendRecord(record);
					}
				}
				finally
				{
					this.LoggingTime += this.Timestamp - timestamp;
				}
			});
		}

		public void Merge(string server, NameValueCollection headers)
		{
			this.SafeLog(delegate
			{
				long timestamp = this.Timestamp;
				try
				{
					if (headers != null && !string.IsNullOrEmpty(server))
					{
						StringBuilder stringBuilder = new StringBuilder();
						this.servers.TryAdd(server, server);
						for (int i = 0; i < 10; i++)
						{
							string name = string.Format("DiscoveryLog{0}", i);
							string value = headers.Get(name);
							if (string.IsNullOrEmpty(value))
							{
								break;
							}
							stringBuilder.Append(value);
						}
						if (stringBuilder.Length > 0)
						{
							JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
							ICollection<Recorder.Record> collection = javaScriptSerializer.Deserialize<ICollection<Recorder.Record>>(stringBuilder.ToString());
							foreach (Recorder.Record record in collection)
							{
								this.AppendRecord(record);
							}
						}
					}
				}
				finally
				{
					this.LoggingTime += this.Timestamp - timestamp;
				}
			});
		}

		public void Write(NameValueCollection headers, Action<string, string> protocolLog)
		{
			this.SafeLog(delegate
			{
				StringBuilder stringBuilder = new StringBuilder();
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				Recorder.Record record = this.Start("Logging", TraceType.InfoTrace, true);
				record.Attributes["CONVTIME"] = this.ConversionTime;
				record.Attributes["LOGTIME"] = this.LoggingTime;
				this.End(record);
				javaScriptSerializer.Serialize(this.records.Values, stringBuilder);
				for (int i = 0; i < 10; i++)
				{
					string text = string.Format("DiscoveryLog{0}", i);
					string text2 = this.Segment(stringBuilder, i, 10000);
					if (!string.IsNullOrEmpty(text2))
					{
						this.WriteLog(text, text2, TraceType.InfoTrace);
						if (protocolLog != null)
						{
							protocolLog(text, text2.Replace(',', '~'));
						}
					}
					if (headers != null)
					{
						string value = this.Segment(stringBuilder, i, 500);
						if (!string.IsNullOrEmpty(value))
						{
							headers[text] = value;
						}
					}
				}
			});
		}

		public void WriteTimestampHeader(IDictionary<string, string> headers)
		{
			this.SafeLog(delegate
			{
				if (headers != null)
				{
					string value = string.Format("{0},{1}", this.Timestamp, DateTime.UtcNow.Ticks);
					headers["X-DiscoveryLogTimestamp"] = value;
				}
			});
		}

		public void ReadTimestampHeader(NameValueCollection headers)
		{
			this.SafeLog(delegate
			{
				if (headers != null && headers.AllKeys.Contains("X-DiscoveryLogTimestamp"))
				{
					string text = headers["X-DiscoveryLogTimestamp"];
					if (!string.IsNullOrWhiteSpace(text))
					{
						string[] array = text.Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							long num = 0L;
							long num2 = 0L;
							if (long.TryParse(array[0], out num2) && long.TryParse(array[1], out num))
							{
								long num3 = DateTime.UtcNow.Ticks - num;
								if (num3 > 0L)
								{
									this.offset = num3 + num2;
									Recorder.Record record = this.Start("CallLatency", TraceType.InfoTrace, true);
									record.Attributes["WORKDURATION"] = num3;
									this.End(record);
								}
							}
						}
					}
				}
			});
		}

		private void AppendRecord(Recorder.Record record)
		{
			this.records.AddOrUpdate(record.Description, record, (string key, Recorder.Record existing) => this.UpdateRecord(existing, record));
		}

		private Recorder.Record UpdateRecord(Recorder.Record existing, Recorder.Record source)
		{
			if (existing != null && source != null)
			{
				using (IEnumerator<string> enumerator = source.Attributes.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Recorder.<>c__DisplayClass15 CS$<>8__locals2 = new Recorder.<>c__DisplayClass15();
						CS$<>8__locals2.key = enumerator.Current;
						object sourceValue;
						if (source.Attributes.TryGetValue(CS$<>8__locals2.key, out sourceValue))
						{
							existing.Attributes.AddOrUpdate(CS$<>8__locals2.key, sourceValue, delegate(string existingKey, object existingValue)
							{
								if (existingValue == null)
								{
									return sourceValue;
								}
								double num;
								double num2;
								if (double.TryParse(sourceValue.ToString(), out num) && double.TryParse(existingValue.ToString(), out num2))
								{
									return num + num2;
								}
								for (int i = 1; i < 10; i++)
								{
									string key = string.Format("{0}{1}", CS$<>8__locals2.key, i);
									if (existing.Attributes.TryAdd(key, sourceValue))
									{
										break;
									}
								}
								return existingValue;
							});
						}
					}
					goto IL_BE;
				}
			}
			if (source != null)
			{
				return source;
			}
			IL_BE:
			return existing;
		}

		private void WriteLog(Recorder.Record record)
		{
			StringBuilder stringBuilder = new StringBuilder();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			javaScriptSerializer.Serialize(record, stringBuilder);
			this.WriteLog(record.Description, stringBuilder.ToString(), record.TraceType);
		}

		private void WriteLog(string title, string data, TraceType traceType)
		{
			string stateAttribute = null;
			if (this.policy != null && this.policy.CallerInfo != null)
			{
				stateAttribute = this.policy.CallerInfo.UserAgent;
			}
			ResultSeverityLevel severity = ResultSeverityLevel.Error;
			if (traceType != TraceType.ErrorTrace || traceType != TraceType.FatalTrace)
			{
				severity = ResultSeverityLevel.Informational;
			}
			if (traceType == TraceType.FatalTrace || traceType == TraceType.ErrorTrace)
			{
				string text = (traceType == TraceType.FatalTrace) ? "Search.FailureMonitor" : "Mailbox.FailureMonitor";
				LogItem logItem = new LogItem(ExchangeComponent.EdiscoveryProtocol.Name, text, data, ResultSeverityLevel.Error);
				logItem.ResultName = string.Format("{0}/{1}", ExchangeComponent.EdiscoveryProtocol.Name, text);
				logItem.CustomProperties["tenant"] = this.GetTenantDomain();
				logItem.Publish(false);
				return;
			}
			LogItem.Publish("EDiscovery", "Reporting", title, data, stateAttribute, severity, false);
		}

		private string GetTenantDomain()
		{
			object organizationId = this.policy.CallerInfo.OrganizationId;
			if (organizationId != null)
			{
				return organizationId.ToString();
			}
			return string.Empty;
		}

		private string Segment(StringBuilder data, int segment, int segmentSize)
		{
			if (segmentSize > 0)
			{
				int num = segment * segmentSize;
				if (data.Length > num)
				{
					int num2 = Math.Min(data.Length - num, segmentSize);
					if (num2 > 0)
					{
						return data.ToString(num, num2);
					}
				}
			}
			return null;
		}

		private void SafeLog(Action logFunc)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						logFunc();
					}
					catch (ArgumentException)
					{
					}
					catch (InvalidOperationException)
					{
					}
					catch (ReflectionTypeLoadException)
					{
					}
				});
			}
			catch (GrayException)
			{
			}
		}

		private const string DiscoveryLogKey = "DiscoveryLog{0}";

		private const string DiscoveryTimestampHeader = "X-DiscoveryLogTimestamp";

		private const int DiscoveryMaxLogs = 10;

		private const int DiscoveryMaxHeaderSize = 500;

		private const int DiscoveryMaxProtocolSize = 10000;

		private ConcurrentDictionary<string, Recorder.Record> records = new ConcurrentDictionary<string, Recorder.Record>();

		private ConcurrentDictionary<string, string> servers = new ConcurrentDictionary<string, string>();

		private Stopwatch stopwatch = new Stopwatch();

		private ISearchPolicy policy;

		private long offset;

		public class Layer
		{
			public const long EntryPoint = 1L;

			public const long Infrastrucuture = 2L;

			public const long Task = 4L;

			public const long ExternalProvider = 5L;
		}

		public class Record
		{
			public Record()
			{
				this.Attributes = new ConcurrentDictionary<string, object>();
				this.IsAggregateRecord = true;
			}

			[ScriptIgnore]
			public bool IsAggregateRecord { get; set; }

			[ScriptIgnore]
			public double StartTime { get; set; }

			public string Description { get; set; }

			public TraceType TraceType { get; set; }

			public ConcurrentDictionary<string, object> Attributes { get; set; }
		}
	}
}

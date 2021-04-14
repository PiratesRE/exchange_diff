using System;
using System.Collections.Specialized;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.UnifiedGroups;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal sealed class LogWriter : ILogWriter
	{
		private LogWriter()
		{
			LogManager.Initialize(this);
		}

		public static void SimpleLog(object toString)
		{
			LogManager.SendTraceTag(0U, 1, 4, "{0}", new object[]
			{
				toString
			});
		}

		public static void TraceAndLog(LogWriter.TraceMethod traceMethod, LogTraceLevel level, int hashcode, string formatString, params object[] data)
		{
			traceMethod((long)hashcode, formatString, data);
			LogManager.SendTraceTag(0U, 1, level, formatString, data);
		}

		internal static void Initialize()
		{
			ExAssert.RetailAssert(LogWriter.singleton != null, "singleton should not be null");
		}

		public void Initialize(NameValueCollection parameters)
		{
		}

		public bool ShouldTrace(LogCategory category, LogTraceLevel level)
		{
			return LogWriter.Tracer.IsTraceEnabled(TraceType.ErrorTrace) || LogWriter.Tracer.IsTraceEnabled(TraceType.WarningTrace) || LogWriter.Tracer.IsTraceEnabled(TraceType.DebugTrace);
		}

		public void CorrelationCloseExistingAndStartNew(Guid correlationId)
		{
			this.CorrelationEnd();
			this.CorrelationStart(correlationId);
		}

		public void CorrelationStart(Guid correlationId)
		{
			if (LogWriter.currentCorrelationId != Guid.Empty)
			{
				LogWriter.Tracer.TraceWarning<Guid, Guid>((long)this.GetHashCode(), "Unexpected CorrelationStart() call when already started. CorrelationId={0}, CurrentCorrelationId={1}", correlationId, LogWriter.currentCorrelationId);
			}
			else
			{
				LogWriter.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "CorrelationStart: CorrelationId={0}", correlationId);
			}
			LogWriter.currentCorrelationId = correlationId;
			LogWriter.clearThreadScope = false;
			if (ActivityContext.GetCurrentActivityScope() == null)
			{
				IActivityScope activityScope = CorrelationContext.GetActivityScope(correlationId);
				if (activityScope != null)
				{
					ActivityContext.SetThreadScope(activityScope);
					LogWriter.clearThreadScope = true;
				}
			}
		}

		public void CorrelationEnd()
		{
			if (LogWriter.currentCorrelationId == Guid.Empty)
			{
				LogWriter.Tracer.TraceWarning((long)this.GetHashCode(), "Unexpected CorrelationEnd() call when no active correlation.");
			}
			else
			{
				LogWriter.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "CorrelationEnd: CorrelationId={0}", LogWriter.currentCorrelationId);
			}
			if (LogWriter.clearThreadScope)
			{
				ActivityContext.ClearThreadScope();
				LogWriter.clearThreadScope = false;
			}
			LogWriter.currentCorrelationId = Guid.Empty;
		}

		public Guid CorrelationGet()
		{
			return LogWriter.currentCorrelationId;
		}

		public void RecordRequest(RequestMonitor requestMonitor)
		{
			LogWriter.Tracer.TraceDebug<Guid, int, RequestType>((long)this.GetHashCode(), "RecordRequest: CorrelationId={0}, CurrentStage={1}, RequestType={2}", requestMonitor.CorrelationId, requestMonitor.CurrentStage, requestMonitor.RequestType);
		}

		public void SendTraceTag(uint tagID, LogCategory category, LogTraceLevel level, string output, params object[] data)
		{
			string text = string.Format(CultureInfo.InvariantCulture, output, data);
			LogWriter.Tracer.TraceDebug((long)this.GetHashCode(), "SendTraceTag: currentCorrelationId={0}, tagId={1}, category={2}, level={3}. {4}", new object[]
			{
				LogWriter.currentCorrelationId,
				tagID,
				category,
				level,
				text
			});
			FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
			{
				{
					FederatedDirectoryLogSchema.TraceTag.ActivityId,
					LogWriter.currentCorrelationId
				},
				{
					FederatedDirectoryLogSchema.TraceTag.Message,
					text
				}
			});
		}

		public void SendExceptionTag(uint tagID, LogCategory category, Exception ex, string output, params object[] data)
		{
			string text = string.Format(CultureInfo.InvariantCulture, output, data);
			LogWriter.Tracer.TraceError((long)this.GetHashCode(), "SendExceptionTag: currentCorrelationId={0}, tagId={1}, category={2}, exception={3}. {4}", new object[]
			{
				LogWriter.currentCorrelationId,
				tagID,
				category,
				ex,
				text
			});
			FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.ExceptionTag>
			{
				{
					FederatedDirectoryLogSchema.ExceptionTag.ActivityId,
					LogWriter.currentCorrelationId
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.ExceptionType,
					ex.GetType()
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.ExceptionDetail,
					ex
				},
				{
					FederatedDirectoryLogSchema.ExceptionTag.Message,
					text
				}
			});
		}

		public void AssertTag(uint tagID, LogCategory category, bool condition, string output, params object[] data)
		{
			if (!condition)
			{
				string text = string.Format(CultureInfo.InvariantCulture, output, data);
				LogWriter.Tracer.TraceError((long)this.GetHashCode(), "AssertTag: currentCorrelationId={0}, tagId={1}, category={2}. {3}", new object[]
				{
					LogWriter.currentCorrelationId,
					tagID,
					category,
					text
				});
				FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.AssertTag>
				{
					{
						FederatedDirectoryLogSchema.AssertTag.ActivityId,
						LogWriter.currentCorrelationId
					},
					{
						FederatedDirectoryLogSchema.AssertTag.Message,
						text
					}
				});
			}
		}

		public void ShipAssertTag(uint tagID, LogCategory category, bool condition, string output, params object[] data)
		{
			if (!condition)
			{
				string text = string.Format(CultureInfo.InvariantCulture, output, data);
				LogWriter.Tracer.TraceError((long)this.GetHashCode(), "ShipAssertTag: activityId={0}, tagId={1}, category={2}. {3}", new object[]
				{
					LogWriter.currentCorrelationId,
					tagID,
					category,
					text
				});
				FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.ShipAssertTag>
				{
					{
						FederatedDirectoryLogSchema.ShipAssertTag.ActivityId,
						LogWriter.currentCorrelationId
					},
					{
						FederatedDirectoryLogSchema.ShipAssertTag.Message,
						text
					}
				});
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.FederatedDirectoryTracer;

		private static readonly LogWriter singleton = new LogWriter();

		[ThreadStatic]
		private static Guid currentCorrelationId;

		[ThreadStatic]
		private static bool clearThreadScope;

		public delegate void TraceMethod(long id, string formatString, params object[] args);
	}
}

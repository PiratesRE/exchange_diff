using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics
{
	public class TroubleshootingContext
	{
		private static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (TroubleshootingContext.faultInjectionTracer == null)
				{
					TroubleshootingContext.faultInjectionTracer = new FaultInjectionTrace(TroubleshootingContext.diagnosticsComponentGuid, 1);
				}
				return TroubleshootingContext.faultInjectionTracer;
			}
		}

		public MemoryTraceBuilder MemoryTraceBuilder
		{
			get
			{
				return this.memoryTraceBuilder;
			}
		}

		public TroubleshootingContext(string location)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			this.location = location;
		}

		public void TraceOperationCompletedAndUpdateContext()
		{
			this.TraceOperationCompletedAndUpdateContext((long)this.GetHashCode());
		}

		public void TraceOperationCompletedAndUpdateContext(long id)
		{
			if (!ExTraceConfiguration.Instance.InMemoryTracingEnabled)
			{
				return;
			}
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			this.AddOperationCompletedMarker(memoryTraceBuilder, id);
			int startIndex = memoryTraceBuilder.FindLastEntryIndex(2, TroubleshootingContext.markerEntry.TraceType, TroubleshootingContext.markerEntry.TraceTag, TroubleshootingContext.markerEntry.ComponentGuid, TroubleshootingContext.markerEntry.FormatString);
			lock (this)
			{
				if (this.memoryTraceBuilder == null)
				{
					this.memoryTraceBuilder = new MemoryTraceBuilder(1000, 128000);
				}
				memoryTraceBuilder.CopyTo(this.memoryTraceBuilder, startIndex);
			}
		}

		public IEnumerable<TraceEntry> GetTraces()
		{
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			if (memoryTraceBuilder == null)
			{
				return new TraceEntry[0];
			}
			return memoryTraceBuilder.GetTraces();
		}

		public void SendExceptionReportWithTraces(Exception exception, bool terminating)
		{
			this.SendExceptionReportWithTraces(exception, terminating, false);
		}

		public void SendExceptionReportWithTraces(Exception exception, bool terminating, bool verbose)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			if (ExTraceConfiguration.Instance.InMemoryTracingEnabled)
			{
				this.ReportProblem(this.memoryTraceBuilder, exception, null, true, terminating, verbose);
			}
		}

		public void SendTroubleshootingReportWithTraces(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			string functionNameFromException = TroubleshootingContext.GetFunctionNameFromException(exception);
			this.SendTroubleshootingReportWithTraces(exception, functionNameFromException);
		}

		public void SendTroubleshootingReportWithTraces(Exception exception, string functionName)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			if (ExTraceConfiguration.Instance.InMemoryTracingEnabled)
			{
				this.ReportProblem(this.memoryTraceBuilder, exception, functionName, false, false);
			}
		}

		private static void DumpExceptionInfo(Exception e, StringBuilder output)
		{
			while (e != null)
			{
				output.AppendFormat("{0}\n{1}\n\n", e.Message, e.StackTrace);
				e = e.InnerException;
			}
		}

		private void AddOperationCompletedMarker(MemoryTraceBuilder memoryTraceBuilder, long id)
		{
			memoryTraceBuilder.BeginEntry(TroubleshootingContext.markerEntry.TraceType, TroubleshootingContext.markerEntry.ComponentGuid, TroubleshootingContext.markerEntry.TraceTag, id, TroubleshootingContext.markerEntry.FormatString);
			memoryTraceBuilder.EndEntry();
		}

		private void ReportProblem(MemoryTraceBuilder contextTraceBuilder, Exception exception, string functionName, bool isExceptionReport, bool isExceptionReportTerminating)
		{
			this.ReportProblem(contextTraceBuilder, exception, functionName, isExceptionReport, isExceptionReportTerminating, false);
		}

		private static string GetFunctionNameFromException(Exception exception)
		{
			string result;
			if (exception.TargetSite != null && exception.TargetSite.ReflectedType != null && exception.TargetSite.ReflectedType.FullName != null && exception.TargetSite.Name != null)
			{
				result = exception.TargetSite.ReflectedType.FullName + "." + exception.TargetSite.Name;
			}
			else
			{
				result = "unknown";
			}
			return result;
		}

		private static bool IsTestTopology()
		{
			return string.Equals("EXTST-", ExWatson.LabName, StringComparison.OrdinalIgnoreCase);
		}

		private void ReportProblem(MemoryTraceBuilder contextTraceBuilder, Exception exception, string functionName, bool isExceptionReport, bool isExceptionReportTerminating, bool verbose)
		{
			using (TempFileStream tempFileStream = TempFileStream.CreateInstance("Traces_", false))
			{
				using (StreamWriter streamWriter = new StreamWriter(tempFileStream))
				{
					bool addHeader = true;
					if (contextTraceBuilder != null)
					{
						lock (this)
						{
							contextTraceBuilder.Dump(streamWriter, addHeader, verbose);
						}
						addHeader = false;
					}
					MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
					if (memoryTraceBuilder != null)
					{
						memoryTraceBuilder.Dump(streamWriter, addHeader, verbose);
					}
					streamWriter.Flush();
				}
				StringBuilder stringBuilder = new StringBuilder(1024);
				TroubleshootingContext.DumpExceptionInfo(exception, stringBuilder);
				if (TroubleshootingContext.IsTestTopology())
				{
					string path = ExWatson.AppName + "_" + DateTime.UtcNow.ToString("yyyyMMdd_hhmmss") + ".trace";
					try
					{
						File.Copy(tempFileStream.FilePath, Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("SystemDrive"), "\\dumps"), path));
					}
					catch
					{
					}
				}
				if (exception != TroubleshootingContext.FaultInjectionInvalidOperationException)
				{
					if (isExceptionReport)
					{
						WatsonExtraFileReportAction watsonExtraFileReportAction = null;
						try
						{
							watsonExtraFileReportAction = new WatsonExtraFileReportAction(tempFileStream.FilePath);
							ExWatson.RegisterReportAction(watsonExtraFileReportAction, WatsonActionScope.Thread);
							ExWatson.SendReport(exception, isExceptionReportTerminating ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None, null);
							goto IL_152;
						}
						finally
						{
							if (watsonExtraFileReportAction != null)
							{
								ExWatson.UnregisterReportAction(watsonExtraFileReportAction, WatsonActionScope.Thread);
							}
						}
					}
					ExWatson.SendTroubleshootingWatsonReport("15.00.1497.012", this.location, "UnexpectedCondition:" + exception.GetType().Name, exception.StackTrace, functionName, stringBuilder.ToString(), tempFileStream.FilePath);
					IL_152:
					File.Delete(tempFileStream.FilePath);
				}
			}
		}

		private const string AssemblyVersion = "15.00.1497.012";

		private const int MaxTraceBufferEntryCount = 1000;

		private const int MaxTraceBufferSize = 128000;

		private const int EstimatedReportLength = 1024;

		private const int TagFaultInjection = 1;

		internal static readonly Exception FaultInjectionInvalidOperationException = new InvalidOperationException("Fault injection created exception: EEB7F5D9-C4EA-41a7-81DD-C8F7B98216B5");

		private static readonly Guid diagnosticsComponentGuid = new Guid("20e99398-d277-4ead-acde-0dbe119f7ce6");

		private static readonly TraceEntry markerEntry = new TraceEntry(TraceType.InfoTrace, ExTraceGlobals.CommonTracer.Category, 0, 0L, "<Operation completed>", 0, 0);

		private static FaultInjectionTrace faultInjectionTracer;

		private MemoryTraceBuilder memoryTraceBuilder;

		private string location;
	}
}

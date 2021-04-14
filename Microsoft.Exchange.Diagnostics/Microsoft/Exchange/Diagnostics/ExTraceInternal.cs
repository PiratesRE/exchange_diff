using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	public static class ExTraceInternal
	{
		public static void Trace(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string message)
		{
			if (ExTraceConfiguration.Instance.ConsoleTracingEnabled)
			{
				Console.WriteLine("{0}: {1}", id, message);
			}
			if (ExTraceConfiguration.Instance.SystemDiagnosticsTracingEnabled)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("{0}: {1}", id, message));
			}
			if (ETWTrace.IsEnabled)
			{
				ETWTrace.Write(lid, traceType, componentGuid, traceTag, id, message);
			}
		}

		public static void Trace<T0>(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, T0 argument0)
		{
			if (ExTraceConfiguration.Instance.ConsoleTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}: ", id);
				stringBuilder.AppendFormat(format, argument0);
				Console.WriteLine(stringBuilder.ToString());
			}
			if (ExTraceConfiguration.Instance.SystemDiagnosticsTracingEnabled)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.AppendFormat("{0}: ", id);
				stringBuilder2.AppendFormat(format, argument0);
				System.Diagnostics.Trace.WriteLine(stringBuilder2.ToString());
			}
			if (ETWTrace.IsEnabled)
			{
				string message;
				try
				{
					message = string.Format(ExTraceInternal.formatProvider, format, new object[]
					{
						argument0
					});
				}
				catch (FormatException ex)
				{
					message = ex.ToString();
				}
				ETWTrace.Write(lid, traceType, componentGuid, traceTag, id, message);
			}
		}

		public static void Trace<T0, T1>(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, T0 argument0, T1 argument1)
		{
			if (ExTraceConfiguration.Instance.ConsoleTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}: ", id);
				stringBuilder.AppendFormat(format, argument0, argument1);
				Console.WriteLine(stringBuilder.ToString());
			}
			if (ExTraceConfiguration.Instance.SystemDiagnosticsTracingEnabled)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.AppendFormat("{0}: ", id);
				stringBuilder2.AppendFormat(format, argument0, argument1);
				System.Diagnostics.Trace.WriteLine(stringBuilder2.ToString());
			}
			if (ETWTrace.IsEnabled)
			{
				string message;
				try
				{
					message = string.Format(ExTraceInternal.formatProvider, format, new object[]
					{
						argument0,
						argument1
					});
				}
				catch (FormatException ex)
				{
					message = ex.ToString();
				}
				ETWTrace.Write(lid, traceType, componentGuid, traceTag, id, message);
			}
		}

		public static void Trace<T0, T1, T2>(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, T0 argument0, T1 argument1, T2 argument2)
		{
			if (ExTraceConfiguration.Instance.ConsoleTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}: ", id);
				stringBuilder.AppendFormat(format, argument0, argument1, argument2);
				Console.WriteLine(stringBuilder.ToString());
			}
			if (ExTraceConfiguration.Instance.SystemDiagnosticsTracingEnabled)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.AppendFormat("{0}: ", id);
				stringBuilder2.AppendFormat(format, argument0, argument1, argument2);
				System.Diagnostics.Trace.WriteLine(stringBuilder2.ToString());
			}
			if (ETWTrace.IsEnabled)
			{
				string message;
				try
				{
					message = string.Format(ExTraceInternal.formatProvider, format, new object[]
					{
						argument0,
						argument1,
						argument2
					});
				}
				catch (FormatException ex)
				{
					message = ex.ToString();
				}
				ETWTrace.Write(lid, traceType, componentGuid, traceTag, id, message);
			}
		}

		public static void Trace(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, object[] arguments)
		{
			if (arguments == null)
			{
				return;
			}
			if (ExTraceConfiguration.Instance.ConsoleTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}: ", id);
				stringBuilder.AppendFormat(format, arguments);
				Console.WriteLine(stringBuilder.ToString());
			}
			if (ExTraceConfiguration.Instance.SystemDiagnosticsTracingEnabled)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.AppendFormat("{0}: ", id);
				stringBuilder2.AppendFormat(format, arguments);
				System.Diagnostics.Trace.WriteLine(stringBuilder2.ToString());
			}
			if (ETWTrace.IsEnabled)
			{
				string message;
				try
				{
					message = string.Format(ExTraceInternal.formatProvider, format, arguments);
				}
				catch (FormatException ex)
				{
					message = ex.ToString();
				}
				ETWTrace.Write(lid, traceType, componentGuid, traceTag, id, message);
			}
		}

		public static void TraceInMemory(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string message)
		{
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			bool flag = false;
			try
			{
				if (!memoryTraceBuilder.InsideTraceCall)
				{
					memoryTraceBuilder.BeginEntry(traceType, componentGuid, traceTag, id, message);
					memoryTraceBuilder.EndEntry();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					memoryTraceBuilder.Reset();
				}
			}
		}

		public static void TraceInMemory<T0>(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, T0 argument0)
		{
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			bool flag = false;
			try
			{
				if (!memoryTraceBuilder.InsideTraceCall)
				{
					memoryTraceBuilder.BeginEntry(traceType, componentGuid, traceTag, id, format);
					memoryTraceBuilder.AddArgument<T0>(argument0);
					memoryTraceBuilder.EndEntry();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					memoryTraceBuilder.Reset();
				}
			}
		}

		public static void TraceInMemory<T0, T1>(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, T0 argument0, T1 argument1)
		{
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			bool flag = false;
			try
			{
				if (!memoryTraceBuilder.InsideTraceCall)
				{
					memoryTraceBuilder.BeginEntry(traceType, componentGuid, traceTag, id, format);
					memoryTraceBuilder.AddArgument<T0>(argument0);
					memoryTraceBuilder.AddArgument<T1>(argument1);
					memoryTraceBuilder.EndEntry();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					memoryTraceBuilder.Reset();
				}
			}
		}

		public static void TraceInMemory<T0, T1, T2>(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, T0 argument0, T1 argument1, T2 argument2)
		{
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			bool flag = false;
			try
			{
				if (!memoryTraceBuilder.InsideTraceCall)
				{
					memoryTraceBuilder.BeginEntry(traceType, componentGuid, traceTag, id, format);
					memoryTraceBuilder.AddArgument<T0>(argument0);
					memoryTraceBuilder.AddArgument<T1>(argument1);
					memoryTraceBuilder.AddArgument<T2>(argument2);
					memoryTraceBuilder.EndEntry();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					memoryTraceBuilder.Reset();
				}
			}
		}

		public static void TraceInMemory(int lid, TraceType traceType, Guid componentGuid, int traceTag, long id, string format, object[] arguments)
		{
			if (arguments == null)
			{
				return;
			}
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			bool flag = false;
			try
			{
				if (!memoryTraceBuilder.InsideTraceCall)
				{
					memoryTraceBuilder.BeginEntry(traceType, componentGuid, traceTag, id, format);
					for (int i = 0; i < arguments.Length; i++)
					{
						ExTraceInternal.AddTraceArgument(memoryTraceBuilder, arguments[i]);
					}
					memoryTraceBuilder.EndEntry();
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					memoryTraceBuilder.Reset();
				}
			}
		}

		public static List<KeyValuePair<TraceEntry, List<object>>> GetMemoryTrace()
		{
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.GetMemoryTraceBuilder();
			if (memoryTraceBuilder != null)
			{
				return memoryTraceBuilder.GetTraceEntries();
			}
			return null;
		}

		public static void DumpMemoryTrace(TextWriter writer)
		{
			if (writer == null)
			{
				return;
			}
			bool addHeader = true;
			lock (ExTraceInternal.traceBuilderList)
			{
				writer.WriteLine("ThreadId;ComponentGuid;Instance ID;TraceTag;TraceType;TimeStamp;Message;");
				foreach (WeakReference weakReference in ExTraceInternal.traceBuilderList)
				{
					MemoryTraceBuilder memoryTraceBuilder = weakReference.Target as MemoryTraceBuilder;
					if (memoryTraceBuilder != null)
					{
						memoryTraceBuilder.Dump(writer, addHeader, true);
						addHeader = false;
					}
				}
			}
		}

		internal static bool AreAnyTraceProvidersEnabled
		{
			get
			{
				return ETWTrace.IsEnabled || ExTraceConfiguration.Instance.InMemoryTracingEnabled || ExTraceConfiguration.Instance.ConsoleTracingEnabled || ExTraceConfiguration.Instance.SystemDiagnosticsTracingEnabled || ExTraceConfiguration.Instance.FaultInjectionConfiguration.Count > 0;
			}
		}

		internal static void AddTraceArgument(MemoryTraceBuilder builder, object argument)
		{
			if (argument == null)
			{
				builder.AddArgument(string.Empty);
				return;
			}
			if (argument is int)
			{
				builder.AddArgument((int)argument);
				return;
			}
			if (argument is long)
			{
				builder.AddArgument((long)argument);
				return;
			}
			if (argument is Guid)
			{
				builder.AddArgument((Guid)argument);
				return;
			}
			ITraceable traceable = argument as ITraceable;
			if (traceable != null)
			{
				builder.AddArgument<ITraceable>(traceable);
				return;
			}
			builder.AddArgument<object>(argument);
		}

		internal static MemoryTraceBuilder GetMemoryTraceBuilder()
		{
			MemoryTraceBuilder memoryTraceBuilder = ExTraceInternal.memoryTraceBuilder;
			if (memoryTraceBuilder != null)
			{
				return memoryTraceBuilder;
			}
			return ExTraceInternal.CreateMemoryTraceBuilder();
		}

		private static MemoryTraceBuilder CreateMemoryTraceBuilder()
		{
			MemoryTraceBuilder memoryTraceBuilder = new MemoryTraceBuilder(1000, 64000);
			lock (ExTraceInternal.traceBuilderList)
			{
				ExTraceInternal.traceBuilderList.RemoveAll((WeakReference reference) => reference.Target == null);
				ExTraceInternal.traceBuilderList.Add(new WeakReference(memoryTraceBuilder));
			}
			ExTraceInternal.memoryTraceBuilder = memoryTraceBuilder;
			return memoryTraceBuilder;
		}

		private const int TraceBufferSize = 64000;

		private const int MaximumTraceEntries = 1000;

		private static ExFormatProvider formatProvider = new ExFormatProvider();

		[ThreadStatic]
		private static MemoryTraceBuilder memoryTraceBuilder;

		private static List<WeakReference> traceBuilderList = new List<WeakReference>(128);
	}
}

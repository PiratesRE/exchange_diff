using System;

namespace System.Diagnostics.Tracing
{
	internal sealed class UInt64TypeInfo : TraceLoggingTypeInfo<ulong>
	{
		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.AddScalar(name, Statics.Format64(format, TraceLoggingDataType.UInt64));
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref ulong value)
		{
			collector.AddScalar(value);
		}
	}
}

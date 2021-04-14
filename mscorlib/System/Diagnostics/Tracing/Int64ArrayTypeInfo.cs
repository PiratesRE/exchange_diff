using System;

namespace System.Diagnostics.Tracing
{
	internal sealed class Int64ArrayTypeInfo : TraceLoggingTypeInfo<long[]>
	{
		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.AddArray(name, Statics.Format64(format, TraceLoggingDataType.Int64));
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref long[] value)
		{
			collector.AddArray(value);
		}
	}
}

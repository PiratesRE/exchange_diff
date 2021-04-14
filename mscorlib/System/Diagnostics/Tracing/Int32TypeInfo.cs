using System;

namespace System.Diagnostics.Tracing
{
	internal sealed class Int32TypeInfo : TraceLoggingTypeInfo<int>
	{
		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.AddScalar(name, Statics.Format32(format, TraceLoggingDataType.Int32));
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref int value)
		{
			collector.AddScalar(value);
		}
	}
}

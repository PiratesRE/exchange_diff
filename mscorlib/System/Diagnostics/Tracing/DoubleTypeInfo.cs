using System;

namespace System.Diagnostics.Tracing
{
	internal sealed class DoubleTypeInfo : TraceLoggingTypeInfo<double>
	{
		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.AddScalar(name, Statics.Format64(format, TraceLoggingDataType.Double));
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref double value)
		{
			collector.AddScalar(value);
		}
	}
}

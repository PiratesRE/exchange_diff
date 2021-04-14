using System;

namespace System.Diagnostics.Tracing
{
	internal sealed class SingleTypeInfo : TraceLoggingTypeInfo<float>
	{
		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.AddScalar(name, Statics.Format32(format, TraceLoggingDataType.Float));
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref float value)
		{
			collector.AddScalar(value);
		}
	}
}

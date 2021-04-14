using System;

namespace System.Diagnostics.Tracing
{
	internal sealed class NullTypeInfo<DataType> : TraceLoggingTypeInfo<DataType>
	{
		public override void WriteMetadata(TraceLoggingMetadataCollector collector, string name, EventFieldFormat format)
		{
			collector.AddGroup(name);
		}

		public override void WriteData(TraceLoggingDataCollector collector, ref DataType value)
		{
		}

		public override object GetData(object value)
		{
			return null;
		}
	}
}

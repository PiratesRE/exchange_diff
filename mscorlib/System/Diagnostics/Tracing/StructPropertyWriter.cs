using System;

namespace System.Diagnostics.Tracing
{
	internal class StructPropertyWriter<ContainerType, ValueType> : PropertyAccessor<ContainerType>
	{
		public StructPropertyWriter(PropertyAnalysis property)
		{
			this.valueTypeInfo = (TraceLoggingTypeInfo<ValueType>)property.typeInfo;
			this.getter = (StructPropertyWriter<ContainerType, ValueType>.Getter)Statics.CreateDelegate(typeof(StructPropertyWriter<ContainerType, ValueType>.Getter), property.getterInfo);
		}

		public override void Write(TraceLoggingDataCollector collector, ref ContainerType container)
		{
			ValueType valueType = (container == null) ? default(ValueType) : this.getter(ref container);
			this.valueTypeInfo.WriteData(collector, ref valueType);
		}

		public override object GetData(ContainerType container)
		{
			return (container == null) ? default(ValueType) : this.getter(ref container);
		}

		private readonly TraceLoggingTypeInfo<ValueType> valueTypeInfo;

		private readonly StructPropertyWriter<ContainerType, ValueType>.Getter getter;

		private delegate ValueType Getter(ref ContainerType container);
	}
}

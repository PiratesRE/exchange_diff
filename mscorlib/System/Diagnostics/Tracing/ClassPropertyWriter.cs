using System;

namespace System.Diagnostics.Tracing
{
	internal class ClassPropertyWriter<ContainerType, ValueType> : PropertyAccessor<ContainerType>
	{
		public ClassPropertyWriter(PropertyAnalysis property)
		{
			this.valueTypeInfo = (TraceLoggingTypeInfo<ValueType>)property.typeInfo;
			this.getter = (ClassPropertyWriter<ContainerType, ValueType>.Getter)Statics.CreateDelegate(typeof(ClassPropertyWriter<ContainerType, ValueType>.Getter), property.getterInfo);
		}

		public override void Write(TraceLoggingDataCollector collector, ref ContainerType container)
		{
			ValueType valueType = (container == null) ? default(ValueType) : this.getter(container);
			this.valueTypeInfo.WriteData(collector, ref valueType);
		}

		public override object GetData(ContainerType container)
		{
			return (container == null) ? default(ValueType) : this.getter(container);
		}

		private readonly TraceLoggingTypeInfo<ValueType> valueTypeInfo;

		private readonly ClassPropertyWriter<ContainerType, ValueType>.Getter getter;

		private delegate ValueType Getter(ContainerType container);
	}
}

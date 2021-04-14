using System;
using System.Reflection;

namespace System.Diagnostics.Tracing
{
	internal class NonGenericProperytWriter<ContainerType> : PropertyAccessor<ContainerType>
	{
		public NonGenericProperytWriter(PropertyAnalysis property)
		{
			this.getterInfo = property.getterInfo;
			this.typeInfo = property.typeInfo;
		}

		public override void Write(TraceLoggingDataCollector collector, ref ContainerType container)
		{
			object value = (container == null) ? null : this.getterInfo.Invoke(container, null);
			this.typeInfo.WriteObjectData(collector, value);
		}

		public override object GetData(ContainerType container)
		{
			if (container != null)
			{
				return this.getterInfo.Invoke(container, null);
			}
			return null;
		}

		private readonly TraceLoggingTypeInfo typeInfo;

		private readonly MethodInfo getterInfo;
	}
}

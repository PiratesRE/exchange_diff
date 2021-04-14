using System;
using System.Reflection;

namespace System.Diagnostics.Tracing
{
	internal sealed class PropertyAnalysis
	{
		public PropertyAnalysis(string name, MethodInfo getterInfo, TraceLoggingTypeInfo typeInfo, EventFieldAttribute fieldAttribute)
		{
			this.name = name;
			this.getterInfo = getterInfo;
			this.typeInfo = typeInfo;
			this.fieldAttribute = fieldAttribute;
		}

		internal readonly string name;

		internal readonly MethodInfo getterInfo;

		internal readonly TraceLoggingTypeInfo typeInfo;

		internal readonly EventFieldAttribute fieldAttribute;
	}
}

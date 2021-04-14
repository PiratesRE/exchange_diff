using System;
using System.Threading;

namespace System.Diagnostics.Tracing
{
	internal class SimpleEventTypes<T> : TraceLoggingEventTypes
	{
		private SimpleEventTypes(TraceLoggingTypeInfo<T> typeInfo) : base(typeInfo.Name, typeInfo.Tags, new TraceLoggingTypeInfo[]
		{
			typeInfo
		})
		{
			this.typeInfo = typeInfo;
		}

		public static SimpleEventTypes<T> Instance
		{
			get
			{
				return SimpleEventTypes<T>.instance ?? SimpleEventTypes<T>.InitInstance();
			}
		}

		private static SimpleEventTypes<T> InitInstance()
		{
			SimpleEventTypes<T> value = new SimpleEventTypes<T>(TraceLoggingTypeInfo<T>.Instance);
			Interlocked.CompareExchange<SimpleEventTypes<T>>(ref SimpleEventTypes<T>.instance, value, null);
			return SimpleEventTypes<T>.instance;
		}

		private static SimpleEventTypes<T> instance;

		internal readonly TraceLoggingTypeInfo<T> typeInfo;
	}
}

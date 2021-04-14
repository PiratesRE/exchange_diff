using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal sealed class DebugContext
	{
		internal static bool TestOnlyDebugInfoOff
		{
			get
			{
				return DebugContext.testOnlyDebugInfoOff;
			}
			set
			{
				DebugContext.testOnlyDebugInfoOff = value;
			}
		}

		internal static void SetActivityId(Guid value)
		{
			DebugContext.SetProperty(DebugProperties.ActivityId, value);
		}

		[Conditional("DEBUG")]
		internal static void SetComponent(string value)
		{
		}

		[Conditional("DEBUG")]
		internal static void SetComponentInstance(string value)
		{
		}

		[Conditional("DEBUG")]
		internal static void SetAction(string value)
		{
		}

		internal static void UpdateFrom(IActivityScope activityScope)
		{
			DebugContext.SetActivityId(activityScope.ActivityId);
		}

		internal static object GetDebugProperty(DebugProperties debugProperty)
		{
			return CallContext.LogicalGetData(DebugContext.propertyNames[(int)debugProperty]);
		}

		internal static string GetDebugInfo()
		{
			return string.Empty;
		}

		[Conditional("DEBUG")]
		internal static void Refresh()
		{
			int? num = new int?(Environment.CurrentManagedThreadId);
			int? num2 = (int?)DebugContext.GetDebugProperty(DebugProperties.ThreadId);
			if (num2 != null)
			{
				int? num3 = num;
				int? num4 = num2;
				if (num3.GetValueOrDefault() == num4.GetValueOrDefault())
				{
					bool flag = num3 != null != (num4 != null);
				}
			}
		}

		internal static void Clear()
		{
			CallContext.FreeNamedDataSlot(DebugContext.propertyNames[1]);
		}

		[Conditional("DEBUG")]
		private static void SetDebugProperty(DebugProperties debugProperty, object value)
		{
			DebugContext.SetProperty(debugProperty, value);
		}

		private static void SetProperty(DebugProperties debugProperty, object value)
		{
			if (value != null)
			{
				value.GetType();
				CallContext.LogicalSetData(DebugContext.propertyNames[(int)debugProperty], value);
				return;
			}
			CallContext.FreeNamedDataSlot(DebugContext.propertyNames[(int)debugProperty]);
		}

		internal static readonly bool TestOnlyIsDebugBuild = false;

		private static readonly string[] propertyNames = Enum.GetNames(typeof(DebugProperties));

		private static bool testOnlyDebugInfoOff = false;
	}
}

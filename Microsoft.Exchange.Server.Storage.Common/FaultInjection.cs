using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class FaultInjection
	{
		public static void InjectFault(Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		public static void InjectFault(Hookable<Action> action)
		{
			if (action.Value != null)
			{
				action.Value();
			}
		}

		public static ErrorCode InjectError(Hookable<Func<ErrorCode>> action)
		{
			if (action.Value == null)
			{
				return ErrorCode.NoError;
			}
			return action.Value();
		}

		public static T Replace<T>(Hookable<Func<T>> action, T original)
		{
			if (action.Value == null)
			{
				return original;
			}
			return action.Value();
		}
	}
}

using System;

namespace Microsoft.Exchange.Diagnostics.Internal
{
	internal static class Util
	{
		public static T EvaluateOrDefault<T>(Util.TryDelegate<T> expression, T defaultValue)
		{
			T result;
			try
			{
				result = expression();
			}
			catch
			{
				result = defaultValue;
			}
			return result;
		}

		public static T EvaluateOrDefault<T>(Util.TryDelegate<T> expression, T defaultValue, Util.CatchDelegate onThrow)
		{
			T result;
			try
			{
				result = expression();
			}
			catch (Exception ex)
			{
				onThrow(ex);
				result = defaultValue;
			}
			return result;
		}

		public delegate T TryDelegate<T>();

		public delegate void CatchDelegate(Exception ex);
	}
}

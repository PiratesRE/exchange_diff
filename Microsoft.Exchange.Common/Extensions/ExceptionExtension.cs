using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Exchange.Extensions
{
	public static class ExceptionExtension
	{
		public static string GetFullMessage(this Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			while (exception != null)
			{
				string message = exception.Message;
				if (message != text)
				{
					text = message;
					stringBuilder.AppendLine(text);
				}
				exception = exception.InnerException;
				if (exception != null)
				{
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToTraceString(this Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (exception != null)
			{
				stringBuilder.AppendLine(exception.ToString());
				string customMessage = exception.GetCustomMessage();
				if (!string.IsNullOrEmpty(customMessage))
				{
					stringBuilder.AppendLine(customMessage);
				}
				exception = exception.InnerException;
				if (exception != null)
				{
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		public static void PreserveExceptionStack(this Exception e)
		{
			if (e != null && ExceptionExtension.preserveStackTraceMethod != null)
			{
				ExceptionExtension.preserveStackTraceMethod.Invoke(e, null);
			}
		}

		public static bool ContainsInnerException<T>(this Exception exception)
		{
			if (exception == null)
			{
				return false;
			}
			for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
			{
				if (innerException is T)
				{
					return true;
				}
			}
			return false;
		}

		public static string GetCustomMessage(this Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (exception is IdentityNotMappedException)
			{
				using (IEnumerator<IdentityReference> enumerator = (exception as IdentityNotMappedException).UnmappedIdentities.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IdentityReference identityReference = enumerator.Current;
						stringBuilder.AppendLine(identityReference.ToString());
					}
					goto IL_90;
				}
			}
			if (exception is SecurityException)
			{
				SecurityException ex = exception as SecurityException;
				stringBuilder.AppendLine("Demand: " + Convert.ToString(ex.Demanded));
				stringBuilder.AppendLine("First Failed Demand: " + Convert.ToString(ex.FirstPermissionThatFailed));
			}
			IL_90:
			return stringBuilder.ToString();
		}

		private static readonly MethodInfo preserveStackTraceMethod = typeof(Exception).GetTypeInfo().DeclaredMethods.FirstOrDefault((MethodInfo m) => string.Equals(m.Name, "InternalPreserveStackTrace", StringComparison.OrdinalIgnoreCase) && !m.IsStatic && !m.IsPublic);
	}
}

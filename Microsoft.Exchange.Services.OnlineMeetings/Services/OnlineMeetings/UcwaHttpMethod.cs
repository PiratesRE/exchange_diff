using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal static class UcwaHttpMethod
	{
		internal static bool IsSupportedMethod(string method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			string value = UcwaHttpMethod.Normalize(method);
			return "DELETE".Equals(value, StringComparison.Ordinal) || "GET".Equals(value, StringComparison.Ordinal) || "PATCH".Equals(value, StringComparison.Ordinal) || "PUT".Equals(value, StringComparison.Ordinal) || "POST".Equals(value, StringComparison.Ordinal);
		}

		internal static bool IsDeleteMethod(string method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return "DELETE".Equals(UcwaHttpMethod.Normalize(method), StringComparison.Ordinal);
		}

		internal static bool IsGetMethod(string method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return "GET".Equals(UcwaHttpMethod.Normalize(method), StringComparison.Ordinal);
		}

		internal static bool IsPatchMethod(string method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return "PATCH".Equals(UcwaHttpMethod.Normalize(method), StringComparison.Ordinal);
		}

		internal static bool IsPostMethod(string method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return "POST".Equals(UcwaHttpMethod.Normalize(method), StringComparison.Ordinal);
		}

		internal static bool IsPutMethod(string method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return "PUT".Equals(UcwaHttpMethod.Normalize(method), StringComparison.Ordinal);
		}

		internal static string Normalize(string method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return method.ToUpperInvariant();
		}

		public const string Delete = "DELETE";

		public const string Get = "GET";

		public const string Patch = "PATCH";

		public const string Post = "POST";

		public const string Put = "PUT";
	}
}

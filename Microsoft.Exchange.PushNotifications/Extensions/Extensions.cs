using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.PushNotifications.Extensions
{
	internal static class Extensions
	{
		public static string ToNullableString<T>(this T? nullable) where T : struct
		{
			if (nullable == null)
			{
				return "null";
			}
			T value = nullable.Value;
			return value.ToString();
		}

		public static string ToNullableString(this string nullable)
		{
			return nullable ?? "null";
		}

		public static string ToNullableString<T>(this T nullable, Func<T, string> toString = null)
		{
			if (nullable == null)
			{
				return "null";
			}
			if (toString == null)
			{
				return nullable.ToString();
			}
			return toString(nullable);
		}

		public static string ToNullableString<Key, Value>(this IDictionary<Key, Value> elements, Func<Key, string> toKeyString = null, Func<Value, string> toValueString = null)
		{
			if (elements == null)
			{
				return "null";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Key key in elements.Keys)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("; ");
				}
				string arg = (toKeyString != null) ? toKeyString(key) : key.ToNullableString(null);
				string arg2 = (toValueString != null) ? toValueString(elements[key]) : elements[key].ToNullableString(null);
				stringBuilder.AppendFormat("{0}:{{{1}}}", arg, arg2);
			}
			return stringBuilder.ToString();
		}

		public static string ToNullableString<T>(this IEnumerable<T> elements, Func<T, string> toString = null)
		{
			if (elements == null)
			{
				return "null";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (T t in elements)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.AppendFormat("{0}", (toString != null) ? toString(t) : t.ToNullableString(null));
			}
			return string.Format("[{0}]", stringBuilder.ToString());
		}

		public static string ToTraceString(this NameValueCollection headers, string[] excludedKeys = null)
		{
			if (headers != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in headers.AllKeys)
				{
					if (excludedKeys == null || !excludedKeys.Contains(text))
					{
						stringBuilder.AppendFormat("{0}={1};", text, headers[text]);
					}
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string ToTraceString(this SocketException soex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(soex.ExtractErrorCodes());
			stringBuilder.AppendLine();
			stringBuilder.Append(soex.ToTraceString());
			return stringBuilder.ToString();
		}

		public static string ToTraceString(this IOException ioex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			SocketException ex = ioex.InnerException as SocketException;
			if (ex != null)
			{
				stringBuilder.Append(ex.ExtractErrorCodes());
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(ioex.ToTraceString());
			return stringBuilder.ToString();
		}

		public static string ToTraceString(this AuthenticationException aex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Win32Exception ex = aex.InnerException as Win32Exception;
			if (ex != null)
			{
				stringBuilder.Append(ex.ExtractErrorCodes());
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(aex.ToTraceString());
			return stringBuilder.ToString();
		}

		public static bool SupportsSubscriptions(this PushNotificationPlatform platform)
		{
			return platform == PushNotificationPlatform.APNS || platform == PushNotificationPlatform.PendingGet || platform == PushNotificationPlatform.WNS || platform == PushNotificationPlatform.GCM || platform == PushNotificationPlatform.WebApp;
		}

		public static bool RequiresEncryptedAuthenticationKey(this PushNotificationPlatform platform)
		{
			return platform == PushNotificationPlatform.WNS || platform == PushNotificationPlatform.GCM || platform == PushNotificationPlatform.Azure || platform == PushNotificationPlatform.AzureHubCreation;
		}

		public static bool SupportsIssueRegistrationSecret(this PushNotificationPlatform platform)
		{
			return platform == PushNotificationPlatform.APNS || platform == PushNotificationPlatform.WNS || platform == PushNotificationPlatform.GCM;
		}

		private static string ExtractErrorCodes(this SocketException soex)
		{
			if (soex != null)
			{
				return string.Format("SocketException: ErrorCode:{0}; NativeErrorCode:{1}; SocketErrorCode:{2}", soex.ErrorCode, soex.NativeErrorCode, soex.SocketErrorCode);
			}
			return string.Empty;
		}

		private static string ExtractErrorCodes(this Win32Exception w32ex)
		{
			if (w32ex != null)
			{
				return string.Format("Win32Exception: ErrorCode:{0}; NativeErrorCode:{1}; Source:{2};", w32ex.ErrorCode, w32ex.NativeErrorCode, w32ex.Source);
			}
			return string.Empty;
		}

		public const string NullString = "null";
	}
}

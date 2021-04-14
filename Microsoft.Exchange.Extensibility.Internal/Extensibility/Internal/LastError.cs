using System;
using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Extensibility.Internal
{
	public class LastError
	{
		public static bool TryParseSmtpResponseString(string lastErrorDetailsString, out string smtpResponse)
		{
			SmtpResponse smtpResponse2;
			if (SmtpResponse.TryParse(lastErrorDetailsString, out smtpResponse2))
			{
				smtpResponse = lastErrorDetailsString;
				return true;
			}
			return LastError.TryParseSubstring(lastErrorDetailsString, "LED", out smtpResponse);
		}

		public static bool TryConvertLastRetryTimeToLocalTime(string lastErrorDetailsString, out string convertedString)
		{
			return LastError.TryConvertLastRetryTime(lastErrorDetailsString, DateTimeStyles.AssumeUniversal, out convertedString);
		}

		public static bool TryConvertLastRetryTimeToUniversalTime(string lastErrorDetailsString, out string convertedString)
		{
			return LastError.TryConvertLastRetryTime(lastErrorDetailsString, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal, out convertedString);
		}

		public LastError(string lastAttemptFqdn, IPEndPoint lastAttemptEndpoint, DateTime? retryTime, SmtpResponse errorDetails) : this(lastAttemptFqdn, lastAttemptEndpoint, retryTime, errorDetails, null)
		{
		}

		public LastError(string lastAttemptFqdn, IPEndPoint lastAttemptEndpoint, DateTime? retryTime, SmtpResponse errorDetails, LastError innerError)
		{
			this.lastAttemptFqdn = lastAttemptFqdn;
			this.lastAttemptEndpoint = lastAttemptEndpoint;
			this.retryTime = retryTime;
			this.errorDetails = errorDetails;
			this.innerError = innerError;
		}

		public string LastAttemptFqdn
		{
			get
			{
				return this.lastAttemptFqdn;
			}
		}

		public IPEndPoint LastAttemptEndpoint
		{
			get
			{
				return this.lastAttemptEndpoint;
			}
		}

		public string LastAttemptIp
		{
			get
			{
				if (this.lastAttemptEndpoint == null)
				{
					return string.Empty;
				}
				return this.lastAttemptEndpoint.Address.ToString();
			}
		}

		public DateTime? LastRetryTime
		{
			get
			{
				return this.retryTime;
			}
		}

		public SmtpResponse LastErrorDetails
		{
			get
			{
				return this.errorDetails;
			}
		}

		public LastError InnerError
		{
			get
			{
				return this.innerError;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{{{0}={1}}};{{{2}={3}}};{{{4}={5}}};{{{6}={7}}}]", new object[]
			{
				"LRT",
				this.LastRetryTime.ToString() ?? string.Empty,
				"LED",
				this.LastErrorDetails.ToString(),
				"FQDN",
				this.LastAttemptFqdn,
				"IP",
				this.LastAttemptIp
			});
		}

		public string GetFormattedErrorDetails()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.LastRetryTime != null)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} - ", new object[]
				{
					this.LastRetryTime.ToString()
				});
			}
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Remote Server ", new object[0]);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, this.GetReceivingServerErrorDetails("at "), new object[0]);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " returned '{0}'", new object[]
			{
				this.LastErrorDetails.ToString()
			});
			return stringBuilder.ToString();
		}

		public string GetReceivingServerErrorDetails(string prefix = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (prefix != null && (!string.IsNullOrEmpty(this.LastAttemptFqdn) || !string.IsNullOrEmpty(this.LastAttemptIp)))
			{
				stringBuilder.Append(prefix);
			}
			if (!string.IsNullOrEmpty(this.LastAttemptFqdn))
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} ", new object[]
				{
					this.LastAttemptFqdn
				});
			}
			if (!string.IsNullOrEmpty(this.LastAttemptIp))
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "({0})", new object[]
				{
					this.LastAttemptIp
				});
			}
			return stringBuilder.ToString();
		}

		private static bool TryParseSubstring(string lastErrorDetailsString, string startDelimiter, out string substring)
		{
			substring = string.Empty;
			if (lastErrorDetailsString == null)
			{
				return false;
			}
			int num = LastError.IndexOf(lastErrorDetailsString, startDelimiter);
			if (num == -1)
			{
				return false;
			}
			int num2 = lastErrorDetailsString.IndexOf("}", num);
			if (num2 < num)
			{
				return false;
			}
			substring = lastErrorDetailsString.Substring(num, num2 - num);
			return true;
		}

		private static bool TryConvertLastRetryTime(string lastErrorDetailsString, DateTimeStyles styles, out string convertedString)
		{
			convertedString = string.Empty;
			string text;
			if (!LastError.TryParseSubstring(lastErrorDetailsString, "LRT", out text))
			{
				return false;
			}
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			DateTime dateTime;
			if (!DateTime.TryParse(text, null, styles, out dateTime))
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder(lastErrorDetailsString);
			stringBuilder.Replace(text, dateTime.ToString());
			convertedString = stringBuilder.ToString();
			return true;
		}

		private static int IndexOf(string lastErrorDetailsString, string startDelimiter)
		{
			string text = string.Format("{{{0}=", startDelimiter);
			int num = lastErrorDetailsString.IndexOf(text);
			if (num == -1)
			{
				return num;
			}
			return num + text.Length;
		}

		private const string LRTMarker = "LRT";

		private const string LEDMarker = "LED";

		private const string FQDNMarker = "FQDN";

		private const string IPMarker = "IP";

		private const string EndDelimiter = "}";

		private readonly string lastAttemptFqdn;

		private readonly IPEndPoint lastAttemptEndpoint;

		private readonly DateTime? retryTime;

		private readonly SmtpResponse errorDetails;

		private readonly LastError innerError;
	}
}

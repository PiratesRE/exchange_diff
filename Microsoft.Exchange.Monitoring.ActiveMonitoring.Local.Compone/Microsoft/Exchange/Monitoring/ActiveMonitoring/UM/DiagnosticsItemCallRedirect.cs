using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class DiagnosticsItemCallRedirect : DiagnosticsItemBase
	{
		internal static bool IsExpectedErrorId(int errorid)
		{
			return errorid == 15637 || errorid == 15643;
		}

		public string FullRedirectTarget
		{
			get
			{
				Match match = DiagnosticsItemCallRedirect.RedirectPattern.Match(base.Reason);
				if (match.Success)
				{
					return match.Groups["uri"].Value;
				}
				return string.Empty;
			}
		}

		public DateTime RedirectTime
		{
			get
			{
				Match match = DiagnosticsItemCallRedirect.RedirectPattern.Match(base.Reason);
				DateTime dateTime;
				if (match.Success && DateTime.TryParse(match.Groups["time"].Value, out dateTime))
				{
					return dateTime.ToUniversalTime();
				}
				return DateTime.MinValue;
			}
		}

		public string RedirectTarget
		{
			get
			{
				string text = this.FullRedirectTarget;
				Match match = DiagnosticsItemCallRedirect.ToUriRegex.Match(text);
				Match match2 = DiagnosticsItemCallRedirect.MsfeRegex.Match(text);
				if (match.Success)
				{
					if (match2.Success)
					{
						text = string.Format("sip:{0}:{1}", match2.Groups["msfe"].Value, match.Groups["port"].Value);
					}
					else
					{
						text = string.Format("sip:{0}:{1}", match.Groups["host"].Value, match.Groups["port"].Value);
					}
				}
				return text;
			}
		}

		private static Regex RedirectPattern = new Regex("Redirecting to:(?<uri>.+?)(;time=(?<time>.+))*$");

		private static Regex ToUriRegex = new Regex("sip:(?<user>[^@]+)@(?<host>[^:]+):(?<port>\\d+);");

		private static Regex MsfeRegex = new Regex("ms-fe=(?<msfe>[^;]+)(;|$)");
	}
}

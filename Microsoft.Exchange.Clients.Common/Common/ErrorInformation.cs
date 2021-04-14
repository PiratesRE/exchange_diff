using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.Exchange.Clients.Common
{
	public class ErrorInformation
	{
		public SupportLevel? SupportLevel { get; set; }

		public Exception Exception { get; set; }

		public string Message { get; set; }

		public ErrorMode? Mode { get; set; }

		public Strings.IDs? MessageId { get; set; }

		public string MessageParameter
		{
			get
			{
				if (this.messageParameters.Count == 0)
				{
					return null;
				}
				return this.messageParameters[0];
			}
			set
			{
				if (this.messageParameters.Count == 0)
				{
					this.messageParameters.Add(value);
					return;
				}
				this.messageParameters[0] = value;
			}
		}

		public bool SendWatsonReport { get; set; }

		public bool SharePointApp { get; set; }

		public bool SiteMailbox { get; set; }

		public string GroupMailboxDestination { get; set; }

		public string Lids { get; set; }

		public string CustomParameterName { get; set; }

		public string CustomParameterValue { get; set; }

		public static List<string> ParseMessageParameters(string errorMessage, HttpRequest request)
		{
			int parameterCount = ErrorInformation.GetParameterCount(errorMessage);
			List<string> list = new List<string>();
			string item;
			if ((item = request.QueryString["msgParam"]) != null)
			{
				list.Add(item);
				int num = 1;
				while (num < parameterCount && (item = request.QueryString["msgParam" + num]) != null)
				{
					list.Add(item);
					num++;
				}
			}
			if (list.Count < parameterCount)
			{
				throw new ArgumentException("Error message had less format parameters than were passed in the query string");
			}
			return list;
		}

		private static int GetParameterCount(string message)
		{
			Regex regex = new Regex("{(?<index>[0-9]+)(:[^}])?}", RegexOptions.Compiled);
			MatchCollection matchCollection = regex.Matches(message);
			int num = -1;
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				int num2 = int.Parse(match.Result("${index}"));
				num = ((num2 > num) ? num2 : num);
			}
			return num;
		}

		public void AddMessageParameter(string param)
		{
			this.messageParameters.Add(param);
		}

		public void AppendMessageParametersToUrl(StringBuilder urlBuilder)
		{
			int num = 0;
			foreach (string str in this.messageParameters)
			{
				if (num == 0)
				{
					urlBuilder.AppendFormat("&{0}={1}", "msgParam", HttpUtility.UrlEncode(str));
				}
				else
				{
					urlBuilder.AppendFormat("&{0}={1}", "msgParam" + num, HttpUtility.UrlEncode(str));
				}
				num++;
			}
		}

		public void SetCustomParameter(string name, string value)
		{
			this.CustomParameterName = name;
			this.CustomParameterValue = value;
		}

		public void AppendCustomParameterToURL(StringBuilder urlBuilder)
		{
			if (string.IsNullOrEmpty(this.CustomParameterName) || string.IsNullOrEmpty(this.CustomParameterValue))
			{
				return;
			}
			urlBuilder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(this.CustomParameterName), HttpUtility.UrlEncode(this.CustomParameterValue));
		}

		public const string ErrorPageName = "errorfe.aspx";

		public const string ErrorPageUrl = "/owa/auth/errorfe.aspx";

		public const string FirstErrorInfoFormat = "?{0}={1}";

		public const string AddionalErrorInfoFormat = "&{0}={1}";

		public const string ErrorHttpCodeQueryStringName = "httpCode";

		public const string ErrorMessageQueryStringName = "msg";

		public const string ErrorMessageParameterQueryStringName = "msgParam";

		public const string AuthErrorSourceQueryStringName = "authError";

		public const string AuthModeQueryStringName = "m";

		public const string SupportLevelHeaderName = "X-OWASuppLevel";

		public const string SupportLevelLogKey = "suplvl";

		private List<string> messageParameters = new List<string>();
	}
}

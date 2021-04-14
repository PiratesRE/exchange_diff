using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal class FailureDsnComparer : IEqualityComparer<string>
	{
		public static FailureDsnComparer Instance
		{
			get
			{
				return FailureDsnComparer.instance;
			}
		}

		public bool Equals(string s1, string s2)
		{
			string x;
			string x2;
			string y;
			string y2;
			return object.ReferenceEquals(s1, s2) || (this.TryParseStatusCodes(s1, out x, out x2) && this.TryParseStatusCodes(s2, out y, out y2) && StringComparer.OrdinalIgnoreCase.Equals(x, y) && StringComparer.OrdinalIgnoreCase.Equals(x2, y2));
		}

		public int GetHashCode(string s)
		{
			string text;
			string text2;
			if (!this.TryParseStatusCodes(s, out text, out text2))
			{
				return 0;
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length + text2.Length + 1);
			stringBuilder.Append(text);
			stringBuilder.Append(' ');
			stringBuilder.Append(text2);
			return StringComparer.OrdinalIgnoreCase.GetHashCode(stringBuilder.ToString());
		}

		private bool TryParseStatusCodes(string s, out string statusCode, out string enhancedStatusCode)
		{
			statusCode = null;
			enhancedStatusCode = null;
			if (!SmtpResponseParser.IsValidStatusCode(s))
			{
				return false;
			}
			statusCode = s.Substring(0, 3);
			EnhancedStatusCodeImpl enhancedStatusCodeImpl;
			enhancedStatusCode = (EnhancedStatusCodeImpl.TryParse(s, 4, out enhancedStatusCodeImpl) ? enhancedStatusCodeImpl.Value : string.Empty);
			return true;
		}

		private static readonly FailureDsnComparer instance = new FailureDsnComparer();
	}
}

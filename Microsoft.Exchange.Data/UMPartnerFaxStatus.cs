using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	internal class UMPartnerFaxStatus
	{
		protected UMPartnerFaxStatus(string status)
		{
			this.Status = status;
		}

		public string Status { get; set; }

		public bool MissedCall { get; set; }

		public bool IsCompleteFax { get; set; }

		public FaxResultType Type { get; set; }

		public static bool TryParse(string status, out UMPartnerFaxStatus faxStatus)
		{
			faxStatus = null;
			if (string.IsNullOrEmpty(status))
			{
				return false;
			}
			Regex regex = new Regex(UMPartnerFaxStatus.CompleteFaxStatusFormat, RegexOptions.Compiled);
			Regex regex2 = new Regex(UMPartnerFaxStatus.IncompleteFaxStatusFormat, RegexOptions.Compiled);
			Regex regex3 = new Regex(UMPartnerFaxStatus.CancelledFaxStatusFormat, RegexOptions.Compiled);
			Regex regex4 = new Regex(UMPartnerFaxStatus.ServerErrorFaxStatusFormat, RegexOptions.Compiled);
			faxStatus = new UMPartnerFaxStatus(status);
			bool result = true;
			if (regex.Match(status).Success)
			{
				faxStatus.Type = FaxResultType.CompleteFax;
				faxStatus.MissedCall = false;
				faxStatus.IsCompleteFax = true;
			}
			else if (regex2.Match(status).Success)
			{
				faxStatus.Type = FaxResultType.IncompleteFax;
				faxStatus.MissedCall = false;
				faxStatus.IsCompleteFax = false;
			}
			else if (regex3.Match(status).Success)
			{
				faxStatus.Type = FaxResultType.CancelledFax;
				faxStatus.MissedCall = true;
			}
			else if (regex4.Match(status).Success)
			{
				faxStatus.Type = FaxResultType.ServerErrorFax;
				faxStatus.MissedCall = true;
			}
			else
			{
				faxStatus.Type = FaxResultType.None;
				result = false;
			}
			return result;
		}

		private static readonly string CompleteFaxStatusFormat = "\\b2.0";

		private static readonly string IncompleteFaxStatusFormat = "\\b2.6";

		private static readonly string CancelledFaxStatusFormat = "\\b2.4";

		private static readonly string ServerErrorFaxStatusFormat = "\\b5.0";
	}
}

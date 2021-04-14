using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ResponseDetails
	{
		internal bool HasTransientErrors { get; set; }

		internal bool HasFatalErrors { get; set; }

		internal bool HasConnectionErrors { get; set; }

		internal bool HasReadStatusErrors { get; set; }

		internal ResponseDetails()
		{
		}

		public void GetErrors(ref bool hasTransientErrors, ref bool hasFatalErrors, ref bool hasConnectionErrors, ref bool hasReadStatusErrors)
		{
			if (this.HasTransientErrors)
			{
				hasTransientErrors = true;
			}
			if (this.HasFatalErrors)
			{
				hasFatalErrors = true;
			}
			if (this.HasConnectionErrors)
			{
				hasConnectionErrors = true;
			}
			if (this.HasReadStatusErrors)
			{
				hasReadStatusErrors = true;
			}
		}

		public List<string> GetStrings()
		{
			string text = null;
			if (this.HasTransientErrors || this.HasFatalErrors || this.HasConnectionErrors || this.HasReadStatusErrors)
			{
				if (this.HasConnectionErrors)
				{
					text = "WebServiceError:" + 'C';
				}
				else if (this.HasFatalErrors)
				{
					text = "WebServiceError:" + 'F';
				}
				else if (this.HasTransientErrors)
				{
					text = "WebServiceError:" + 'T';
				}
				else if (this.HasReadStatusErrors)
				{
					text = "WebServiceError:" + 'R';
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new List<string>
			{
				text
			};
		}

		internal static ResponseDetails GetResponseDetails(string[] data)
		{
			ResponseDetails responseDetails = new ResponseDetails();
			if (data != null)
			{
				int i = 0;
				while (i < data.Length)
				{
					string text = data[i];
					int num = text.IndexOf("WebServiceError:", StringComparison.Ordinal);
					int num2 = num + "WebServiceError:".Length;
					if (num != -1 && num2 < text.Length)
					{
						char c = text[num2];
						if (c == 'C')
						{
							responseDetails.HasConnectionErrors = true;
							break;
						}
						if (c == 'F')
						{
							responseDetails.HasFatalErrors = true;
							break;
						}
						switch (c)
						{
						case 'R':
							responseDetails.HasReadStatusErrors = true;
							return responseDetails;
						case 'S':
							return responseDetails;
						case 'T':
							responseDetails.HasTransientErrors = true;
							return responseDetails;
						default:
							return responseDetails;
						}
					}
					else
					{
						i++;
					}
				}
			}
			return responseDetails;
		}

		private const string ErrrorLabel = "WebServiceError:";

		private const char TrasientErrorValue = 'T';

		private const char FatalErrorValue = 'F';

		private const char ConnectionErrorValue = 'C';

		private const char ReadStatusErrorValue = 'R';
	}
}

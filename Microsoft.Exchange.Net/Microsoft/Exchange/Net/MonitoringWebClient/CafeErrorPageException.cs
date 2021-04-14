using System;
using System.Text;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class CafeErrorPageException : HttpWebResponseWrapperException
	{
		public CafeErrorPage CafeErrorPage { get; private set; }

		public CafeErrorPageException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, CafeErrorPage cafeErrorPage) : base(message, request, response)
		{
			this.CafeErrorPage = cafeErrorPage;
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0}{1}", base.GetType().FullName, Environment.NewLine);
				stringBuilder.AppendFormat("{0}{1}", this.CafeErrorPage, Environment.NewLine);
				stringBuilder.Append(base.Message);
				return stringBuilder.ToString();
			}
		}

		public override string ExceptionHint
		{
			get
			{
				return string.Concat(new object[]
				{
					"CafeErrorPage: ",
					this.CafeErrorPage.FailureReason,
					" ",
					this.CafeErrorPage.RequestFailureContext.Error
				});
			}
		}
	}
}

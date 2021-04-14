using System;
using System.Web;
using Microsoft.Exchange.Clients.Common.FBL;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.HttpProxy
{
	public class SvmFeedback : OwaPage
	{
		public bool IsProcessingSuccessful
		{
			get
			{
				return SvmFeedback.isProcessingSuccessful;
			}
		}

		public bool? IsClassifyRequest
		{
			get
			{
				return SvmFeedback.isClassifyRequest;
			}
		}

		public bool? IsOptIn
		{
			get
			{
				return SvmFeedback.isOptIn;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			SvmFeedback.ProcessSvmFeedbackRequest();
		}

		private static void ProcessSvmFeedbackRequest()
		{
			HttpRequest request = HttpContext.Current.Request;
			if (request.IsSecureConnection)
			{
				string query = request.Url.Query;
				OwaFblHandler owaFblHandler = new OwaFblHandler();
				try
				{
					SvmFeedback.isProcessingSuccessful = owaFblHandler.TryProcessFbl(query, out SvmFeedback.isClassifyRequest, out SvmFeedback.isOptIn);
				}
				catch (Exception)
				{
					SvmFeedback.isProcessingSuccessful = false;
				}
			}
		}

		private static bool isProcessingSuccessful;

		private static bool? isClassifyRequest;

		private static bool? isOptIn;
	}
}

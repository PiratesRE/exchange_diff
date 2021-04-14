using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SpeechRecognitionAsyncResult : AsyncResult
	{
		public SpeechRecognitionAsyncResult(AsyncCallback callback, object context) : base(callback, context)
		{
		}

		public int StatusCode { get; set; }

		public string StatusDescription { get; set; }

		public string ResponseText { get; set; }

		public double ThrottlingDelay { get; set; }

		public string ThrottlingNotEnforcedReason { get; set; }
	}
}

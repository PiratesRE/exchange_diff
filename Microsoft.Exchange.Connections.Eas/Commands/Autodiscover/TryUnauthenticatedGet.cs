using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TryUnauthenticatedGet : AutodiscoverStep
	{
		protected internal TryUnauthenticatedGet(EasConnectionSettings easConnectionSettings) : base(easConnectionSettings, Step.TryDnsLookupOfSrvRecord)
		{
			this.WebRequestTimeout = new TimeSpan(0, 0, 10).Milliseconds;
		}

		internal override bool UseSsl
		{
			get
			{
				return false;
			}
		}

		protected override string RequestMethodName
		{
			get
			{
				return "GET";
			}
		}

		private int WebRequestTimeout { get; set; }

		public override Step ExecuteStep(StepContext stepContext)
		{
			string autodiscoverDomain = base.GetAutodiscoverDomain(base.EasConnectionSettings.EasEndpointSettings.Domain);
			stepContext.ProbeStack.Push(autodiscoverDomain);
			return base.PrimitiveExecuteStep(stepContext);
		}

		protected override bool IsStepAllowable(StepContext stepContext)
		{
			return stepContext.Request.AutodiscoverOption != AutodiscoverOption.ExistingEndpoint;
		}

		protected override HttpWebRequest ConditionWebRequest(HttpWebRequest webRequest)
		{
			webRequest.Method = this.RequestMethodName;
			webRequest.UserAgent = base.EasConnectionSettings.UserAgent;
			webRequest.AllowAutoRedirect = false;
			webRequest.PreAuthenticate = false;
			webRequest.Timeout = 10000;
			return webRequest;
		}

		protected override void AddWebRequestBody(HttpWebRequest webRequest, AutodiscoverRequest easRequest)
		{
		}
	}
}

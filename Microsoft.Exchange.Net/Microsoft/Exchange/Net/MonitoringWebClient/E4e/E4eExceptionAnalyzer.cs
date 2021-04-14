using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.MonitoringWebClient.E4e
{
	internal class E4eExceptionAnalyzer : BaseExceptionAnalyzer
	{
		public E4eExceptionAnalyzer(Dictionary<string, RequestTarget> hostNameSourceMapping) : base(hostNameSourceMapping)
		{
		}

		protected override Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> ComponentMatrix
		{
			get
			{
				return E4eExceptionAnalyzer.FailureMatrix;
			}
		}

		protected override FailingComponent DefaultComponent
		{
			get
			{
				return FailingComponent.E4e;
			}
		}

		private static readonly Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> FailureMatrix = new Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>>();
	}
}

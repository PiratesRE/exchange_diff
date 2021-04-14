using System;
using System.Text;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class ScenarioException : Exception
	{
		public RequestTarget FailureSource { get; private set; }

		public FailureReason FailureReason { get; private set; }

		public FailingComponent FailingComponent { get; private set; }

		public virtual string ExceptionHint { get; private set; }

		public ScenarioException(string message, Exception innerException, RequestTarget failureSource, FailureReason failureReason, FailingComponent failingComponent, string exceptionHint) : base(message, innerException)
		{
			this.FailureSource = failureSource;
			this.FailureReason = failureReason;
			this.FailingComponent = failingComponent;
			this.ExceptionHint = exceptionHint;
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(MonitoringWebClientStrings.ScenarioExceptionMessageHeader(base.GetType().FullName, base.Message, this.FailureSource.ToString(), this.FailureReason.ToString(), this.FailingComponent.ToString(), this.ExceptionHint));
				if (base.InnerException != null)
				{
					stringBuilder.Append(MonitoringWebClientStrings.ScenarioExceptionInnerException(base.InnerException.Message));
				}
				return stringBuilder.ToString();
			}
		}
	}
}

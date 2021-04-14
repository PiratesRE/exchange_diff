using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	[Serializable]
	public class ExportException : Exception
	{
		public ExportException()
		{
			this.ErrorType = ExportErrorType.Unknown;
		}

		public ExportException(string message)
		{
			this.errorMessage = message;
			this.ErrorType = ExportErrorType.Unknown;
		}

		public ExportException(ExportErrorType errorType)
		{
			this.ErrorType = errorType;
		}

		public ExportException(ExportErrorType errorType, string message)
		{
			this.errorMessage = message;
			this.ErrorType = errorType;
		}

		public ExportException(ExportErrorType errorType, Exception innerException) : base(string.Empty, innerException)
		{
			this.ErrorType = errorType;
		}

		public ExportErrorType ErrorType { get; set; }

		public Uri ServiceEndpoint { get; set; }

		public ServiceHttpContext ServiceHttpContext { get; set; }

		public string ScenarioData { get; set; }

		public override string Message
		{
			get
			{
				return string.Concat(new object[]
				{
					"Export failed with error type: '",
					this.ErrorType,
					"'.",
					(!string.IsNullOrEmpty(this.errorMessage)) ? (" Message: " + this.errorMessage) : string.Empty,
					(base.InnerException != null) ? (" Details: " + base.InnerException.Message) : string.Empty,
					(this.ServiceEndpoint != null) ? (" Endpoint: " + this.ServiceEndpoint.ToString()) : string.Empty,
					(this.ServiceHttpContext != null) ? (" HttpContext: " + this.ServiceHttpContext.ToString()) : string.Empty,
					(!string.IsNullOrWhiteSpace(this.ScenarioData)) ? (" ScenarioData: " + this.ScenarioData) : string.Empty
				});
			}
		}

		private readonly string errorMessage;
	}
}

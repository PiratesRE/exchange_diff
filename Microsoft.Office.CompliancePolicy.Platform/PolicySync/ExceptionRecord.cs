using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class ExceptionRecord
	{
		public ExceptionRecord()
		{
		}

		public ExceptionRecord(Exception exception)
		{
			this.Type = exception.GetType().ToString();
			this.Message = exception.Message;
			this.Tag = exception.GetHashCode().ToString();
		}

		public string Type { get; set; }

		public string Message { get; set; }

		public string Tag { get; set; }
	}
}

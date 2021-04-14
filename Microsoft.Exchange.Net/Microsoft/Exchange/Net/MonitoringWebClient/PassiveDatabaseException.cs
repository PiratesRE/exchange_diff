using System;
using System.Text;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class PassiveDatabaseException : HttpWebResponseWrapperException
	{
		public PassiveDatabaseException(string message, HttpWebRequestWrapper request, HttpWebResponseWrapper response, string passiveDetectionHint) : base(message, request, response)
		{
			this.passiveDetectionHint = passiveDetectionHint;
		}

		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(base.GetType().FullName + Environment.NewLine);
				stringBuilder.Append(this.ExceptionHint + Environment.NewLine);
				stringBuilder.Append(base.Message);
				return stringBuilder.ToString();
			}
		}

		public override string ExceptionHint
		{
			get
			{
				return "PassiveDatabase: " + this.passiveDetectionHint;
			}
		}

		private readonly string passiveDetectionHint;
	}
}

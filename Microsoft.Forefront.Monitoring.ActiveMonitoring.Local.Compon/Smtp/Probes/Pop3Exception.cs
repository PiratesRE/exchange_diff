using System;
using System.Runtime.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	[Serializable]
	public class Pop3Exception : Exception
	{
		public Pop3Exception()
		{
		}

		public Pop3Exception(string message) : base(message)
		{
		}

		public Pop3Exception(string message, Exception inner) : base(message, inner)
		{
		}

		public Pop3Exception(string message, string response, string command) : base(message)
		{
			this.LastResponse = response;
			this.LastCommand = command;
		}

		protected Pop3Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public string LastResponse { get; private set; }

		public string LastCommand { get; private set; }
	}
}

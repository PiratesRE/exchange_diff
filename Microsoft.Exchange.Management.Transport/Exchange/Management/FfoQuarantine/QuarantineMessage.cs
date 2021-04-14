using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	[Serializable]
	public class QuarantineMessage
	{
		public QuarantineMessage()
		{
			this.RecipientAddress = new List<string>();
			this.QuarantinedUser = new List<string>();
			this.ReleasedUser = new List<string>();
		}

		public string Identity { get; set; }

		public DateTime ReceivedTime { get; set; }

		public string Organization { get; set; }

		public string MessageId { get; set; }

		public string SenderAddress { get; set; }

		public List<string> RecipientAddress { get; set; }

		public string Subject { get; set; }

		public int Size { get; set; }

		public QuarantineMessageType Type { get; set; }

		public DateTime Expires { get; set; }

		public List<string> QuarantinedUser { get; set; }

		public List<string> ReleasedUser { get; set; }

		public bool Reported { get; set; }

		public QuarantineMessageDirection Direction { get; set; }
	}
}

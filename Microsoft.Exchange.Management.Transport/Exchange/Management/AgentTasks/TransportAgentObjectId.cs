using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[Serializable]
	public class TransportAgentObjectId : ObjectId
	{
		public TransportAgentObjectId(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			this.identity = identity;
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.identity);
		}

		public override string ToString()
		{
			return this.identity;
		}

		private readonly string identity;
	}
}

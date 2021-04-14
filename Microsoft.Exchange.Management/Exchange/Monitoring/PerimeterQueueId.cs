using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	internal class PerimeterQueueId : ObjectId
	{
		public PerimeterQueueId(string identity)
		{
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

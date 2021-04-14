using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Sharing
{
	[Serializable]
	public class WindowsLiveIdIdentity : ObjectId
	{
		internal WindowsLiveIdIdentity(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			this.identity = identity;
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.ToString());
		}

		public override string ToString()
		{
			return this.identity;
		}

		private readonly string identity;
	}
}

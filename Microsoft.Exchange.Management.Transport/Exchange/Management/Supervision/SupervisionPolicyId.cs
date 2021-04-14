using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Supervision
{
	[Serializable]
	public sealed class SupervisionPolicyId : ObjectId
	{
		public SupervisionPolicyId(string orgname)
		{
			if (string.IsNullOrEmpty(orgname))
			{
				throw new ArgumentNullException("orgname");
			}
			this.id = orgname;
		}

		public override string ToString()
		{
			return this.id;
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.id);
		}

		private readonly string id;
	}
}

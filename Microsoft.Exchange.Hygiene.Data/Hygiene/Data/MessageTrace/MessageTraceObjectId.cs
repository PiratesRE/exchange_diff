using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	[Serializable]
	internal sealed class MessageTraceObjectId : ObjectId
	{
		public MessageTraceObjectId(Guid organizationalUnitRoot, Guid exMessageId)
		{
			this.organizationalUnitRoot = organizationalUnitRoot;
			this.exMessageId = exMessageId;
		}

		public Guid ExMessageId
		{
			get
			{
				return this.exMessageId;
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return this.organizationalUnitRoot;
			}
		}

		public override string ToString()
		{
			return string.Format("Tenant={0},Message={1}", this.OrganizationalUnitRoot, this.ExMessageId);
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}

		private readonly Guid organizationalUnitRoot;

		private readonly Guid exMessageId;
	}
}

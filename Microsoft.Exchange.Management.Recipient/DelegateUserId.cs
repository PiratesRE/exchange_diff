using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public sealed class DelegateUserId : ObjectId
	{
		public DelegateUserId(string id)
		{
			this.identity = id;
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.identity;
		}

		private readonly string identity;
	}
}

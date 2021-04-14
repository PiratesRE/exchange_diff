using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class ObjectId
	{
		public abstract byte[] GetBytes();
	}
}

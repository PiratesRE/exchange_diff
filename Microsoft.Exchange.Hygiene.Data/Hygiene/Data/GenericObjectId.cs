using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Serializable]
	internal sealed class GenericObjectId : ObjectId
	{
		public GenericObjectId(int id)
		{
			this.bytes = BitConverter.GetBytes(id);
		}

		public GenericObjectId(Guid id)
		{
			this.bytes = id.ToByteArray();
		}

		public override byte[] GetBytes()
		{
			return this.bytes;
		}

		private byte[] bytes;
	}
}

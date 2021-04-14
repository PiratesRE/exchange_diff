using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	[Serializable]
	public sealed class NextHopSolutionKeyId : ObjectId
	{
		public NextHopSolutionKeyId(Guid uniqueIdentifier)
		{
			this.uniqueIdentifier = uniqueIdentifier;
		}

		public override byte[] GetBytes()
		{
			return this.uniqueIdentifier.ToByteArray();
		}

		public static NextHopSolutionKeyId DefaultNextHopSolutionKeyId = new NextHopSolutionKeyId(Guid.Empty);

		private readonly Guid uniqueIdentifier;
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManySplitsException : EsentObsoleteException
	{
		public EsentTooManySplitsException() : base("Infinite split", JET_err.TooManySplits)
		{
		}

		private EsentTooManySplitsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyIOException : EsentResourceException
	{
		public EsentTooManyIOException() : base("System busy due to too many IOs", JET_err.TooManyIO)
		{
		}

		private EsentTooManyIOException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

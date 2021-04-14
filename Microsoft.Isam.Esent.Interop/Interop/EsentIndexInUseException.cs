using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexInUseException : EsentStateException
	{
		public EsentIndexInUseException() : base("Index is in use", JET_err.IndexInUse)
		{
		}

		private EsentIndexInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

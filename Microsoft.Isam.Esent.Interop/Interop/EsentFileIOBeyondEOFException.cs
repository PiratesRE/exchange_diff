using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileIOBeyondEOFException : EsentCorruptionException
	{
		public EsentFileIOBeyondEOFException() : base("a read was issued to a location beyond EOF (writes will expand the file)", JET_err.FileIOBeyondEOF)
		{
		}

		private EsentFileIOBeyondEOFException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileIOSparseException : EsentObsoleteException
	{
		public EsentFileIOSparseException() : base("an I/O was issued to a location that was sparse", JET_err.FileIOSparse)
		{
		}

		private EsentFileIOSparseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

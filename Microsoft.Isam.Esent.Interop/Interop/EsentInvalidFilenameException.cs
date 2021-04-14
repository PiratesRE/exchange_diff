using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidFilenameException : EsentObsoleteException
	{
		public EsentInvalidFilenameException() : base("Filename is invalid", JET_err.InvalidFilename)
		{
		}

		private EsentInvalidFilenameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

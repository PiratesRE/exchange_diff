using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidLogDirectoryException : EsentObsoleteException
	{
		public EsentInvalidLogDirectoryException() : base("Invalid log directory", JET_err.InvalidLogDirectory)
		{
		}

		private EsentInvalidLogDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

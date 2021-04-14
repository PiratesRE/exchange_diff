using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidPathException : EsentUsageException
	{
		public EsentInvalidPathException() : base("Invalid file path", JET_err.InvalidPath)
		{
		}

		private EsentInvalidPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidSystemPathException : EsentObsoleteException
	{
		public EsentInvalidSystemPathException() : base("Invalid system path", JET_err.InvalidSystemPath)
		{
		}

		private EsentInvalidSystemPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

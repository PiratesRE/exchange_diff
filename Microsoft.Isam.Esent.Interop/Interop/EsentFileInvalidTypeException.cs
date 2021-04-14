using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileInvalidTypeException : EsentInconsistentException
	{
		public EsentFileInvalidTypeException() : base("Invalid file type", JET_err.FileInvalidType)
		{
		}

		private EsentFileInvalidTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

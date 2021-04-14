using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileAccessDeniedException : EsentIOException
	{
		public EsentFileAccessDeniedException() : base("Cannot access file, the file is locked or in use", JET_err.FileAccessDenied)
		{
		}

		private EsentFileAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDirtyShutdownException : EsentStateException
	{
		public EsentDirtyShutdownException() : base("The instance was shutdown successfully but all the attached databases were left in a dirty state by request via JET_bitTermDirty", JET_err.DirtyShutdown)
		{
		}

		private EsentDirtyShutdownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

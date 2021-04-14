using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentAccessDeniedException : EsentObsoleteException
	{
		public EsentAccessDeniedException() : base("Access denied", JET_err.AccessDenied)
		{
		}

		private EsentAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

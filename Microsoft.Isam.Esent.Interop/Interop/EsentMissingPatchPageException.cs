using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMissingPatchPageException : EsentObsoleteException
	{
		public EsentMissingPatchPageException() : base("Patch file page not found during recovery", JET_err.MissingPatchPage)
		{
		}

		private EsentMissingPatchPageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

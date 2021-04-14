using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadPatchPageException : EsentObsoleteException
	{
		public EsentBadPatchPageException() : base("Patch file page is not valid", JET_err.BadPatchPage)
		{
		}

		private EsentBadPatchPageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

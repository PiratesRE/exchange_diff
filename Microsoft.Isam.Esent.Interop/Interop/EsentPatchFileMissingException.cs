using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPatchFileMissingException : EsentObsoleteException
	{
		public EsentPatchFileMissingException() : base("Hard restore detected that patch file is missing from backup set", JET_err.PatchFileMissing)
		{
		}

		private EsentPatchFileMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabasePatchFileMismatchException : EsentObsoleteException
	{
		public EsentDatabasePatchFileMismatchException() : base("Patch file is not generated from this backup", JET_err.DatabasePatchFileMismatch)
		{
		}

		private EsentDatabasePatchFileMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

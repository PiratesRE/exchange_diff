using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMissingFileToBackupException : EsentInconsistentException
	{
		public EsentMissingFileToBackupException() : base("Some log or patch files are missing during backup", JET_err.MissingFileToBackup)
		{
		}

		private EsentMissingFileToBackupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

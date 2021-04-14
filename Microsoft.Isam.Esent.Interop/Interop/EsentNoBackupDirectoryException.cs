using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNoBackupDirectoryException : EsentUsageException
	{
		public EsentNoBackupDirectoryException() : base("No backup directory given", JET_err.NoBackupDirectory)
		{
		}

		private EsentNoBackupDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMakeBackupDirectoryFailException : EsentIOException
	{
		public EsentMakeBackupDirectoryFailException() : base("Could not make backup temp directory", JET_err.MakeBackupDirectoryFail)
		{
		}

		private EsentMakeBackupDirectoryFailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

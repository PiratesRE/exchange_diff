using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDeleteBackupFileFailException : EsentIOException
	{
		public EsentDeleteBackupFileFailException() : base("Could not delete backup file", JET_err.DeleteBackupFileFail)
		{
		}

		private EsentDeleteBackupFileFailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

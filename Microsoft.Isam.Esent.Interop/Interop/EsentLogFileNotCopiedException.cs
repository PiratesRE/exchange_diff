using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogFileNotCopiedException : EsentUsageException
	{
		public EsentLogFileNotCopiedException() : base("log truncation attempted but not all required logs were copied", JET_err.LogFileNotCopied)
		{
		}

		private EsentLogFileNotCopiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

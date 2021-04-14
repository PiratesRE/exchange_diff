using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTempFileOpenErrorException : EsentObsoleteException
	{
		public EsentTempFileOpenErrorException() : base("Temp file could not be opened", JET_err.TempFileOpenError)
		{
		}

		private EsentTempFileOpenErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

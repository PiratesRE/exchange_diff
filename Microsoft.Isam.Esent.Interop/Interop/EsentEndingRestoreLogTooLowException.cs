using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentEndingRestoreLogTooLowException : EsentInconsistentException
	{
		public EsentEndingRestoreLogTooLowException() : base("The starting log number too low for the restore", JET_err.EndingRestoreLogTooLow)
		{
		}

		private EsentEndingRestoreLogTooLowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

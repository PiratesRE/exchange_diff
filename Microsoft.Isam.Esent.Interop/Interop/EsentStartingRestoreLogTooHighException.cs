using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentStartingRestoreLogTooHighException : EsentInconsistentException
	{
		public EsentStartingRestoreLogTooHighException() : base("The starting log number too high for the restore", JET_err.StartingRestoreLogTooHigh)
		{
		}

		private EsentStartingRestoreLogTooHighException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

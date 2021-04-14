using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyInstancesException : EsentQuotaException
	{
		public EsentTooManyInstancesException() : base("Cannot start any more database instances", JET_err.TooManyInstances)
		{
		}

		private EsentTooManyInstancesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

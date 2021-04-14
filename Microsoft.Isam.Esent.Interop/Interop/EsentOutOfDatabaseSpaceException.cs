using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOutOfDatabaseSpaceException : EsentQuotaException
	{
		public EsentOutOfDatabaseSpaceException() : base("Maximum database size reached", JET_err.OutOfDatabaseSpace)
		{
		}

		private EsentOutOfDatabaseSpaceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

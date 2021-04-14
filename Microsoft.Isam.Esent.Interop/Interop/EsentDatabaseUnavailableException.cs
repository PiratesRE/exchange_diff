using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseUnavailableException : EsentObsoleteException
	{
		public EsentDatabaseUnavailableException() : base("This database cannot be used because it encountered a fatal error", JET_err.DatabaseUnavailable)
		{
		}

		private EsentDatabaseUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

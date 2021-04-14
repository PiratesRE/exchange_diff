using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseSignInUseException : EsentUsageException
	{
		public EsentDatabaseSignInUseException() : base("Database with same signature in use", JET_err.DatabaseSignInUse)
		{
		}

		private EsentDatabaseSignInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

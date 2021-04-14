using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyActiveUsersException : EsentUsageException
	{
		public EsentTooManyActiveUsersException() : base("Too many active database users", JET_err.TooManyActiveUsers)
		{
		}

		private EsentTooManyActiveUsersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

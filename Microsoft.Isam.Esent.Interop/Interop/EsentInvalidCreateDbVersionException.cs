using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidCreateDbVersionException : EsentInconsistentException
	{
		public EsentInvalidCreateDbVersionException() : base("recovery tried to replay a database creation, but the database was originally created with an incompatible (likely older) version of the database engine", JET_err.InvalidCreateDbVersion)
		{
		}

		private EsentInvalidCreateDbVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotDisableVersioningException : EsentUsageException
	{
		public EsentCannotDisableVersioningException() : base("Cannot disable versioning for this database", JET_err.CannotDisableVersioning)
		{
		}

		private EsentCannotDisableVersioningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

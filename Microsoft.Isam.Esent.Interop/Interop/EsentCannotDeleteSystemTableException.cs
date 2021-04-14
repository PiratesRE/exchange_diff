using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotDeleteSystemTableException : EsentUsageException
	{
		public EsentCannotDeleteSystemTableException() : base("Illegal attempt to delete a system table", JET_err.CannotDeleteSystemTable)
		{
		}

		private EsentCannotDeleteSystemTableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

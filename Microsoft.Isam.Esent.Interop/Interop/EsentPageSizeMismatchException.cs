using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPageSizeMismatchException : EsentInconsistentException
	{
		public EsentPageSizeMismatchException() : base("The database page size does not match the engine", JET_err.PageSizeMismatch)
		{
		}

		private EsentPageSizeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

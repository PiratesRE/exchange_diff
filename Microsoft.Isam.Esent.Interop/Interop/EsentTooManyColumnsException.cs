using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTooManyColumnsException : EsentUsageException
	{
		public EsentTooManyColumnsException() : base("Too many columns defined", JET_err.TooManyColumns)
		{
		}

		private EsentTooManyColumnsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

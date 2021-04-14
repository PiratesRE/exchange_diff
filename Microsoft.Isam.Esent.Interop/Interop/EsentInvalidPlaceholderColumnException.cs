using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidPlaceholderColumnException : EsentUsageException
	{
		public EsentInvalidPlaceholderColumnException() : base("Tried to convert column to a primary index placeholder, but column doesn't meet necessary criteria", JET_err.InvalidPlaceholderColumn)
		{
		}

		private EsentInvalidPlaceholderColumnException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

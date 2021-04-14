using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentExistingLogFileIsNotContiguousException : EsentInconsistentException
	{
		public EsentExistingLogFileIsNotContiguousException() : base("Existing log file is not contiguous", JET_err.ExistingLogFileIsNotContiguous)
		{
		}

		private EsentExistingLogFileIsNotContiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentGivenLogFileIsNotContiguousException : EsentInconsistentException
	{
		public EsentGivenLogFileIsNotContiguousException() : base("Restore log file is not contiguous", JET_err.GivenLogFileIsNotContiguous)
		{
		}

		private EsentGivenLogFileIsNotContiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

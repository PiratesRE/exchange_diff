using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentNoCurrentIndexException : EsentUsageException
	{
		public EsentNoCurrentIndexException() : base("Invalid w/o a current index", JET_err.NoCurrentIndex)
		{
		}

		private EsentNoCurrentIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

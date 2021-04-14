using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexMustStayException : EsentUsageException
	{
		public EsentIndexMustStayException() : base("Cannot delete clustered index", JET_err.IndexMustStay)
		{
		}

		private EsentIndexMustStayException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexDuplicateException : EsentUsageException
	{
		public EsentIndexDuplicateException() : base("Index is already defined", JET_err.IndexDuplicate)
		{
		}

		private EsentIndexDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

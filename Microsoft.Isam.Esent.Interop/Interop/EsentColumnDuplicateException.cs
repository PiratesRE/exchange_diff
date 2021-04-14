using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentColumnDuplicateException : EsentUsageException
	{
		public EsentColumnDuplicateException() : base("Field is already defined", JET_err.ColumnDuplicate)
		{
		}

		private EsentColumnDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

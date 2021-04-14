using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTableDuplicateException : EsentStateException
	{
		public EsentTableDuplicateException() : base("Table already exists", JET_err.TableDuplicate)
		{
		}

		private EsentTableDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

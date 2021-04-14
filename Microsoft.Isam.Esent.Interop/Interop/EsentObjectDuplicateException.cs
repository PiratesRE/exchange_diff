using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentObjectDuplicateException : EsentObsoleteException
	{
		public EsentObjectDuplicateException() : base("Table or object name in use", JET_err.ObjectDuplicate)
		{
		}

		private EsentObjectDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

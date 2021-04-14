using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexInvalidDefException : EsentUsageException
	{
		public EsentIndexInvalidDefException() : base("Illegal index definition", JET_err.IndexInvalidDef)
		{
		}

		private EsentIndexInvalidDefException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

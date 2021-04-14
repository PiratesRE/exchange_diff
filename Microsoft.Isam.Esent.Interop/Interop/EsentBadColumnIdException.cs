using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadColumnIdException : EsentUsageException
	{
		public EsentBadColumnIdException() : base("Column Id Incorrect", JET_err.BadColumnId)
		{
		}

		private EsentBadColumnIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

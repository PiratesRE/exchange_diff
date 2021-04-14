using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTransTooDeepException : EsentUsageException
	{
		public EsentTransTooDeepException() : base("Transactions nested too deeply", JET_err.TransTooDeep)
		{
		}

		private EsentTransTooDeepException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

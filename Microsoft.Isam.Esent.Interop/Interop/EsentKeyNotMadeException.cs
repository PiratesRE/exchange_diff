using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentKeyNotMadeException : EsentUsageException
	{
		public EsentKeyNotMadeException() : base("No call to JetMakeKey", JET_err.KeyNotMade)
		{
		}

		private EsentKeyNotMadeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

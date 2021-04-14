using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentKeyIsMadeException : EsentUsageException
	{
		public EsentKeyIsMadeException() : base("The key is completely made", JET_err.KeyIsMade)
		{
		}

		private EsentKeyIsMadeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

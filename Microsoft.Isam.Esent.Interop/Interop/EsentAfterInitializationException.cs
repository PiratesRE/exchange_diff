using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentAfterInitializationException : EsentUsageException
	{
		public EsentAfterInitializationException() : base("Cannot Restore after init.", JET_err.AfterInitialization)
		{
		}

		private EsentAfterInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

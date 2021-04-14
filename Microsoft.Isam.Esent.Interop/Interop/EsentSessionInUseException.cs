using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSessionInUseException : EsentUsageException
	{
		public EsentSessionInUseException() : base("Tried to terminate session in use", JET_err.SessionInUse)
		{
		}

		private EsentSessionInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLSAlreadySetException : EsentUsageException
	{
		public EsentLSAlreadySetException() : base("Attempted to set Local Storage for an object which already had it set", JET_err.LSAlreadySet)
		{
		}

		private EsentLSAlreadySetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLSNotSetException : EsentStateException
	{
		public EsentLSNotSetException() : base("Attempted to retrieve Local Storage from an object which didn't have it set", JET_err.LSNotSet)
		{
		}

		private EsentLSNotSetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

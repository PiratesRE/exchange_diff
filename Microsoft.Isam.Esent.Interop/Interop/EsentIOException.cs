using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentIOException : EsentOperationException
	{
		protected EsentIOException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentIOException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentOperationException : EsentErrorException
	{
		protected EsentOperationException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

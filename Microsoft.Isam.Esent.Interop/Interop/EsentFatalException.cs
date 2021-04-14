using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentFatalException : EsentOperationException
	{
		protected EsentFatalException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentFatalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

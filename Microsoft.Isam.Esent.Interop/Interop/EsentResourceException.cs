using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentResourceException : EsentOperationException
	{
		protected EsentResourceException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

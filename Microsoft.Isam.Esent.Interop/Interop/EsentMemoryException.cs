using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentMemoryException : EsentResourceException
	{
		protected EsentMemoryException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

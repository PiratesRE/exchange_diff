using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentStateException : EsentApiException
	{
		protected EsentStateException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

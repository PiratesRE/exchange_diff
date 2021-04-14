using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentApiException : EsentErrorException
	{
		protected EsentApiException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentApiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

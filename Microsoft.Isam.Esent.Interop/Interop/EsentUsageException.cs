using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentUsageException : EsentApiException
	{
		protected EsentUsageException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentUsageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentQuotaException : EsentResourceException
	{
		protected EsentQuotaException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentQuotaException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

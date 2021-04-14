using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentObsoleteException : EsentApiException
	{
		protected EsentObsoleteException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentObsoleteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentDataException : EsentErrorException
	{
		protected EsentDataException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentCorruptionException : EsentDataException
	{
		protected EsentCorruptionException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentCorruptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

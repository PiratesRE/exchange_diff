using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentInconsistentException : EsentDataException
	{
		protected EsentInconsistentException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentInconsistentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

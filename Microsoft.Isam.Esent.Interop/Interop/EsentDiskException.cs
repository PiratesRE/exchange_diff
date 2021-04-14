using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentDiskException : EsentResourceException
	{
		protected EsentDiskException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentDiskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

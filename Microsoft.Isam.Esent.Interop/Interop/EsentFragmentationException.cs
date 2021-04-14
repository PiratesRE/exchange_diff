using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public abstract class EsentFragmentationException : EsentDataException
	{
		protected EsentFragmentationException(string description, JET_err err) : base(description, err)
		{
		}

		protected EsentFragmentationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

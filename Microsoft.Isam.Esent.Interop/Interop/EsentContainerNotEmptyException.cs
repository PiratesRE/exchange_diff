using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentContainerNotEmptyException : EsentObsoleteException
	{
		public EsentContainerNotEmptyException() : base("Container is not empty", JET_err.ContainerNotEmpty)
		{
		}

		private EsentContainerNotEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentPreviousVersionException : EsentErrorException
	{
		public EsentPreviousVersionException() : base("Version already existed. Recovery failure", JET_err.PreviousVersion)
		{
		}

		private EsentPreviousVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

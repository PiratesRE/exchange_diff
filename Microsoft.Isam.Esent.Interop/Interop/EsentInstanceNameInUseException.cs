using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInstanceNameInUseException : EsentUsageException
	{
		public EsentInstanceNameInUseException() : base("Instance Name already in use", JET_err.InstanceNameInUse)
		{
		}

		private EsentInstanceNameInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

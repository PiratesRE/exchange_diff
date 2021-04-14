using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTaskDroppedException : EsentResourceException
	{
		public EsentTaskDroppedException() : base("A requested async task could not be executed", JET_err.TaskDropped)
		{
		}

		private EsentTaskDroppedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

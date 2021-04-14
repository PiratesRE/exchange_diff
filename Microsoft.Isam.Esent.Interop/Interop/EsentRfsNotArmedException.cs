using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRfsNotArmedException : EsentObsoleteException
	{
		public EsentRfsNotArmedException() : base("Resource Failure Simulator not initialized", JET_err.RfsNotArmed)
		{
		}

		private EsentRfsNotArmedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

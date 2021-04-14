using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDisabledFunctionalityException : EsentUsageException
	{
		public EsentDisabledFunctionalityException() : base("You are running MinESE, that does not have all features compiled in.  This functionality is only supported in a full version of ESE.", JET_err.DisabledFunctionality)
		{
		}

		private EsentDisabledFunctionalityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSpaceHintsInvalidException : EsentUsageException
	{
		public EsentSpaceHintsInvalidException() : base("An element of the JET space hints structure was not correct or actionable.", JET_err.SpaceHintsInvalid)
		{
		}

		private EsentSpaceHintsInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentSystemParameterConflictException : EsentUsageException
	{
		public EsentSystemParameterConflictException() : base("Global system parameters have already been set, but to a conflicting or disagreeable state to the specified values.", JET_err.SystemParameterConflict)
		{
		}

		private EsentSystemParameterConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentUpdateMustVersionException : EsentUsageException
	{
		public EsentUpdateMustVersionException() : base("No version updates only for uncommitted tables", JET_err.UpdateMustVersion)
		{
		}

		private EsentUpdateMustVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

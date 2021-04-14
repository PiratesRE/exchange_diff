using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexCantBuildException : EsentObsoleteException
	{
		public EsentIndexCantBuildException() : base("Index build failed", JET_err.IndexCantBuild)
		{
		}

		private EsentIndexCantBuildException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

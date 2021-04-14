using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTaggedNotNULLException : EsentObsoleteException
	{
		public EsentTaggedNotNULLException() : base("No non-NULL tagged columns", JET_err.TaggedNotNULL)
		{
		}

		private EsentTaggedNotNULLException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

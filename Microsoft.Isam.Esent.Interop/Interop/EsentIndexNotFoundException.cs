using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentIndexNotFoundException : EsentStateException
	{
		public EsentIndexNotFoundException() : base("No such index", JET_err.IndexNotFound)
		{
		}

		private EsentIndexNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

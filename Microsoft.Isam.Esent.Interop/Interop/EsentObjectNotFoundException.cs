using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentObjectNotFoundException : EsentStateException
	{
		public EsentObjectNotFoundException() : base("No such table or object", JET_err.ObjectNotFound)
		{
		}

		private EsentObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

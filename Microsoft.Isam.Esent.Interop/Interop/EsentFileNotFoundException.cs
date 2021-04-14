using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileNotFoundException : EsentStateException
	{
		public EsentFileNotFoundException() : base("File not found", JET_err.FileNotFound)
		{
		}

		private EsentFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

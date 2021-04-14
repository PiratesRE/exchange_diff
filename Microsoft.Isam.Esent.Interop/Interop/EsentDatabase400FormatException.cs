using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabase400FormatException : EsentObsoleteException
	{
		public EsentDatabase400FormatException() : base("The database is in an older (400) format", JET_err.Database400Format)
		{
		}

		private EsentDatabase400FormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

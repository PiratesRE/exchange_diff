using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabase200FormatException : EsentObsoleteException
	{
		public EsentDatabase200FormatException() : base("The database is in an older (200) format", JET_err.Database200Format)
		{
		}

		private EsentDatabase200FormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

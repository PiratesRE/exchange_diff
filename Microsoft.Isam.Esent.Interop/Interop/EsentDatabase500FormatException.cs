using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabase500FormatException : EsentObsoleteException
	{
		public EsentDatabase500FormatException() : base("The database is in an older (500) format", JET_err.Database500Format)
		{
		}

		private EsentDatabase500FormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

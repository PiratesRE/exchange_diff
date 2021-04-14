using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidPrereadException : EsentUsageException
	{
		public EsentInvalidPrereadException() : base("Cannot preread long values when current index secondary", JET_err.InvalidPreread)
		{
		}

		private EsentInvalidPrereadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

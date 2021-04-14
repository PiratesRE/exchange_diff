using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentBadLogVersionException : EsentInconsistentException
	{
		public EsentBadLogVersionException() : base("Version of log file is not compatible with Jet version", JET_err.BadLogVersion)
		{
		}

		private EsentBadLogVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFeatureNotAvailableException : EsentUsageException
	{
		public EsentFeatureNotAvailableException() : base("API not supported", JET_err.FeatureNotAvailable)
		{
		}

		private EsentFeatureNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

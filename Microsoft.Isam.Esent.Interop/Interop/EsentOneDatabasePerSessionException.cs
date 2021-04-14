using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentOneDatabasePerSessionException : EsentUsageException
	{
		public EsentOneDatabasePerSessionException() : base("Just one open user database per session is allowed (JET_paramOneDatabasePerSession)", JET_err.OneDatabasePerSession)
		{
		}

		private EsentOneDatabasePerSessionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentMustDisableLoggingForDbUpgradeException : EsentObsoleteException
	{
		public EsentMustDisableLoggingForDbUpgradeException() : base("Cannot have logging enabled while attempting to upgrade db", JET_err.MustDisableLoggingForDbUpgrade)
		{
		}

		private EsentMustDisableLoggingForDbUpgradeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseAlreadyUpgradedException : EsentStateException
	{
		public EsentDatabaseAlreadyUpgradedException() : base("Attempted to upgrade a database that is already current", JET_err.DatabaseAlreadyUpgraded)
		{
		}

		private EsentDatabaseAlreadyUpgradedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

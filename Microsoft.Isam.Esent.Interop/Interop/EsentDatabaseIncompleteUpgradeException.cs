using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseIncompleteUpgradeException : EsentStateException
	{
		public EsentDatabaseIncompleteUpgradeException() : base("Attempted to use a database which was only partially converted to the current format -- must restore from backup", JET_err.DatabaseIncompleteUpgrade)
		{
		}

		private EsentDatabaseIncompleteUpgradeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

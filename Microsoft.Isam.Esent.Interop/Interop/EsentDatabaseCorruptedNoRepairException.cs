using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseCorruptedNoRepairException : EsentUsageException
	{
		public EsentDatabaseCorruptedNoRepairException() : base("Corrupted db but repair not allowed", JET_err.DatabaseCorruptedNoRepair)
		{
		}

		private EsentDatabaseCorruptedNoRepairException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

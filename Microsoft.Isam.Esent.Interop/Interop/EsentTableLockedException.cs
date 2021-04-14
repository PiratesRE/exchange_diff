using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTableLockedException : EsentUsageException
	{
		public EsentTableLockedException() : base("Table is exclusively locked", JET_err.TableLocked)
		{
		}

		private EsentTableLockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

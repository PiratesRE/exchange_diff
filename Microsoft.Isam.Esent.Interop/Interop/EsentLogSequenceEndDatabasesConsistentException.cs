using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLogSequenceEndDatabasesConsistentException : EsentFragmentationException
	{
		public EsentLogSequenceEndDatabasesConsistentException() : base("databases have been recovered, but all possible log generations in the current sequence are used; delete all log files and the checkpoint file and backup the databases before continuing", JET_err.LogSequenceEndDatabasesConsistent)
		{
		}

		private EsentLogSequenceEndDatabasesConsistentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

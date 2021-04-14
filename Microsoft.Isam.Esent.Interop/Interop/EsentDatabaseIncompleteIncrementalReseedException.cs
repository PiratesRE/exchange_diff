using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDatabaseIncompleteIncrementalReseedException : EsentInconsistentException
	{
		public EsentDatabaseIncompleteIncrementalReseedException() : base("The database cannot be attached because it is currently being rebuilt as part of an incremental reseed.", JET_err.DatabaseIncompleteIncrementalReseed)
		{
		}

		private EsentDatabaseIncompleteIncrementalReseedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

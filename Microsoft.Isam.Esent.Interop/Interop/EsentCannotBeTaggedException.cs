using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotBeTaggedException : EsentUsageException
	{
		public EsentCannotBeTaggedException() : base("AutoIncrement and Version cannot be tagged", JET_err.CannotBeTagged)
		{
		}

		private EsentCannotBeTaggedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

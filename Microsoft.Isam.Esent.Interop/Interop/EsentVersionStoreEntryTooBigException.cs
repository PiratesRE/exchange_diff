using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentVersionStoreEntryTooBigException : EsentErrorException
	{
		public EsentVersionStoreEntryTooBigException() : base("Attempted to create a version store entry (RCE) larger than a version bucket", JET_err.VersionStoreEntryTooBig)
		{
		}

		private EsentVersionStoreEntryTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

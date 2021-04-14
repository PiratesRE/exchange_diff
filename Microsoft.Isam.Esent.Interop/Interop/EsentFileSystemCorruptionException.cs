using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileSystemCorruptionException : EsentCorruptionException
	{
		public EsentFileSystemCorruptionException() : base("File system operation failed with an error indicating the file system is corrupt.", JET_err.FileSystemCorruption)
		{
		}

		private EsentFileSystemCorruptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

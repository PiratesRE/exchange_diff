using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentRedoAbruptEndedException : EsentCorruptionException
	{
		public EsentRedoAbruptEndedException() : base("Redo abruptly ended due to sudden failure in reading logs from log file", JET_err.RedoAbruptEnded)
		{
		}

		private EsentRedoAbruptEndedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

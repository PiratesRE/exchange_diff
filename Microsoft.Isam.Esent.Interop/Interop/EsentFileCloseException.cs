using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentFileCloseException : EsentObsoleteException
	{
		public EsentFileCloseException() : base("Could not close file", JET_err.FileClose)
		{
		}

		private EsentFileCloseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

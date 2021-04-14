using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDataHasChangedException : EsentObsoleteException
	{
		public EsentDataHasChangedException() : base("Data has changed, operation aborted", JET_err.DataHasChanged)
		{
		}

		private EsentDataHasChangedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

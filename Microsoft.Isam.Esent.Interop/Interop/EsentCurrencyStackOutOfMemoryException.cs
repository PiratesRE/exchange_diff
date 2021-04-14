using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCurrencyStackOutOfMemoryException : EsentObsoleteException
	{
		public EsentCurrencyStackOutOfMemoryException() : base("UNUSED: lCSRPerfFUCB * g_lCursorsMax exceeded (XJET only)", JET_err.CurrencyStackOutOfMemory)
		{
		}

		private EsentCurrencyStackOutOfMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

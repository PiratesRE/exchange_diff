using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTableInUseException : EsentStateException
	{
		public EsentTableInUseException() : base("Table is in use, cannot lock", JET_err.TableInUse)
		{
		}

		private EsentTableInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

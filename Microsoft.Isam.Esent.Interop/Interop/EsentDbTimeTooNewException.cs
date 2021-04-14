using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDbTimeTooNewException : EsentInconsistentException
	{
		public EsentDbTimeTooNewException() : base("dbtime on page in advance of the dbtimeBefore in record", JET_err.DbTimeTooNew)
		{
		}

		private EsentDbTimeTooNewException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

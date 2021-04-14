using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentDbTimeTooOldException : EsentInconsistentException
	{
		public EsentDbTimeTooOldException() : base("dbtime on page smaller than dbtimeBefore in record", JET_err.DbTimeTooOld)
		{
		}

		private EsentDbTimeTooOldException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

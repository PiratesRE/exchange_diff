using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentTableNotEmptyException : EsentUsageException
	{
		public EsentTableNotEmptyException() : base("Table is not empty", JET_err.TableNotEmpty)
		{
		}

		private EsentTableNotEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

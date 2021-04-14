using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCannotDeleteTemplateTableException : EsentUsageException
	{
		public EsentCannotDeleteTemplateTableException() : base("Illegal attempt to delete a template table", JET_err.CannotDeleteTemplateTable)
		{
		}

		private EsentCannotDeleteTemplateTableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

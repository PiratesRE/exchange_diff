using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidLanguageIdException : EsentUsageException
	{
		public EsentInvalidLanguageIdException() : base("Invalid or unknown language id", JET_err.InvalidLanguageId)
		{
		}

		private EsentInvalidLanguageIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

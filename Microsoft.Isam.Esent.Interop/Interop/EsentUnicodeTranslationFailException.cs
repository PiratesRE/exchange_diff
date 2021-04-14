using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentUnicodeTranslationFailException : EsentOperationException
	{
		public EsentUnicodeTranslationFailException() : base("Unicode normalization failed", JET_err.UnicodeTranslationFail)
		{
		}

		private EsentUnicodeTranslationFailException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

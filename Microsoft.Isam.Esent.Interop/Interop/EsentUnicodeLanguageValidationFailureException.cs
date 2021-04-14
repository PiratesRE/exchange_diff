using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentUnicodeLanguageValidationFailureException : EsentOperationException
	{
		public EsentUnicodeLanguageValidationFailureException() : base("Can not validate the language", JET_err.UnicodeLanguageValidationFailure)
		{
		}

		private EsentUnicodeLanguageValidationFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentUnicodeTranslationBufferTooSmallException : EsentObsoleteException
	{
		public EsentUnicodeTranslationBufferTooSmallException() : base("Unicode translation buffer too small", JET_err.UnicodeTranslationBufferTooSmall)
		{
		}

		private EsentUnicodeTranslationBufferTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

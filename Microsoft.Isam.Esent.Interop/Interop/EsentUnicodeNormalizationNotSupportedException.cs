using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentUnicodeNormalizationNotSupportedException : EsentUsageException
	{
		public EsentUnicodeNormalizationNotSupportedException() : base("OS does not provide support for Unicode normalisation (and no normalisation callback was specified)", JET_err.UnicodeNormalizationNotSupported)
		{
		}

		private EsentUnicodeNormalizationNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

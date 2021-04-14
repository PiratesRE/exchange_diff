using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLanguageNotSupportedException : EsentObsoleteException
	{
		public EsentLanguageNotSupportedException() : base("Windows installation does not support language", JET_err.LanguageNotSupported)
		{
		}

		private EsentLanguageNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

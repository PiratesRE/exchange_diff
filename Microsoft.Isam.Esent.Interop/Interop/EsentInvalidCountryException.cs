using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidCountryException : EsentObsoleteException
	{
		public EsentInvalidCountryException() : base("Invalid or unknown country/region code", JET_err.InvalidCountry)
		{
		}

		private EsentInvalidCountryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

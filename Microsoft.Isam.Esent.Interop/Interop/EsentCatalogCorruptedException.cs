using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentCatalogCorruptedException : EsentCorruptionException
	{
		public EsentCatalogCorruptedException() : base("Corruption detected in catalog", JET_err.CatalogCorrupted)
		{
		}

		private EsentCatalogCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

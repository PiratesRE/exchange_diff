using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentEntryPointNotFoundException : EsentUsageException
	{
		public EsentEntryPointNotFoundException() : base("An entry point in a DLL we require could not be found", JET_err.EntryPointNotFound)
		{
		}

		private EsentEntryPointNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

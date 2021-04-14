using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentLSCallbackNotSpecifiedException : EsentUsageException
	{
		public EsentLSCallbackNotSpecifiedException() : base("Attempted to use Local Storage without a callback function being specified", JET_err.LSCallbackNotSpecified)
		{
		}

		private EsentLSCallbackNotSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

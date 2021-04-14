using System;
using System.Runtime.Serialization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class EsentInvalidSettingsException : EsentUsageException
	{
		public EsentInvalidSettingsException() : base("System parameters were set improperly", JET_err.InvalidSettings)
		{
		}

		private EsentInvalidSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

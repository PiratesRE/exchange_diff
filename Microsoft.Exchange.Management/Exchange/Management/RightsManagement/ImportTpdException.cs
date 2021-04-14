using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ImportTpdException : Exception
	{
		public ImportTpdException()
		{
		}

		public ImportTpdException(string message, Exception innerException = null) : base(message, innerException)
		{
		}

		protected ImportTpdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class PrivateKeyDecryptionFailedException : Exception
	{
		public PrivateKeyDecryptionFailedException()
		{
		}

		public PrivateKeyDecryptionFailedException(string message, Exception innerException = null) : base(message, innerException)
		{
		}

		protected PrivateKeyDecryptionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

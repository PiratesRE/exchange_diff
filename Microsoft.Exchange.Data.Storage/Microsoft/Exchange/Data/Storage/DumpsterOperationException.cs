using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DumpsterOperationException : StoragePermanentException
	{
		public DumpsterOperationException(LocalizedString message) : base(message)
		{
		}

		public DumpsterOperationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		private DumpsterOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

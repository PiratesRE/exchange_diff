using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class FolderCycleException : StoragePermanentException
	{
		public FolderCycleException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FolderCycleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

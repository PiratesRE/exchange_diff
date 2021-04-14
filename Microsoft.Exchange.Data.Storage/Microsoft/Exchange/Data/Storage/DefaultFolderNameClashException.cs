using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class DefaultFolderNameClashException : CorruptDataException
	{
		public DefaultFolderNameClashException(LocalizedString message) : base(message)
		{
		}

		private DefaultFolderNameClashException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public sealed class DefaultFolderPropertyValidationException : CorruptDataException
	{
		public DefaultFolderPropertyValidationException(LocalizedString message) : base(message)
		{
		}

		private DefaultFolderPropertyValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

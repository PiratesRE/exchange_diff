using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PropertyChangeMetadataFormatException : CorruptDataException
	{
		public PropertyChangeMetadataFormatException(LocalizedString message) : base(message)
		{
		}

		public PropertyChangeMetadataFormatException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PropertyChangeMetadataFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

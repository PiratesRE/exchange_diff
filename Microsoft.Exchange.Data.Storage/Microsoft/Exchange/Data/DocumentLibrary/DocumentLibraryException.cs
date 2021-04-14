using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class DocumentLibraryException : LocalizedException
	{
		public DocumentLibraryException(string message) : base(new LocalizedString(message))
		{
		}

		public DocumentLibraryException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}
	}
}

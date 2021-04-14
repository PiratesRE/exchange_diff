using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class CannotMoveDefaultFolderException : StoragePermanentException
	{
		public CannotMoveDefaultFolderException(LocalizedString message) : base(message)
		{
		}

		protected CannotMoveDefaultFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class DelegateUserNoFreeBusyFolderException : StoragePermanentException
	{
		public DelegateUserNoFreeBusyFolderException(LocalizedString message) : base(message)
		{
		}

		protected DelegateUserNoFreeBusyFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

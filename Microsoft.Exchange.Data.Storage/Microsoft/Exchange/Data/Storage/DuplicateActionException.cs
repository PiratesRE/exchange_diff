using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class DuplicateActionException : StoragePermanentException
	{
		public DuplicateActionException(LocalizedString message) : base(message)
		{
		}

		public DuplicateActionException(LocalizedString message, Exception e) : base(message, e)
		{
		}

		protected DuplicateActionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

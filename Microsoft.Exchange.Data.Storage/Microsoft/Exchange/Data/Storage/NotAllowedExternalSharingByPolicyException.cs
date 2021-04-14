using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class NotAllowedExternalSharingByPolicyException : StoragePermanentException
	{
		public NotAllowedExternalSharingByPolicyException() : base(ServerStrings.NotAllowedExternalSharingByPolicy)
		{
		}

		protected NotAllowedExternalSharingByPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

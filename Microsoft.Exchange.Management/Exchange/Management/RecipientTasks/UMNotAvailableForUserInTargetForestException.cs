using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMNotAvailableForUserInTargetForestException : LocalizedException
	{
		public UMNotAvailableForUserInTargetForestException() : base(Strings.UMNotAvailableForUserInTargetForest)
		{
		}

		public UMNotAvailableForUserInTargetForestException(Exception innerException) : base(Strings.UMNotAvailableForUserInTargetForest, innerException)
		{
		}

		protected UMNotAvailableForUserInTargetForestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

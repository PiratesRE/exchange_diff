using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Management.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MaximumNumberOfBatchesReachedException : LocalizedException
	{
		public MaximumNumberOfBatchesReachedException() : base(Strings.MaximumNumberOfBatchesReached)
		{
		}

		public MaximumNumberOfBatchesReachedException(Exception innerException) : base(Strings.MaximumNumberOfBatchesReached, innerException)
		{
		}

		protected MaximumNumberOfBatchesReachedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

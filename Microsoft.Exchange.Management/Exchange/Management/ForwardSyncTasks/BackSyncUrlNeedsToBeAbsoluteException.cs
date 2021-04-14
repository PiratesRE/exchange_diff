using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BackSyncUrlNeedsToBeAbsoluteException : LocalizedException
	{
		public BackSyncUrlNeedsToBeAbsoluteException() : base(Strings.BackSyncUrlNeedsToBeAbsolute)
		{
		}

		public BackSyncUrlNeedsToBeAbsoluteException(Exception innerException) : base(Strings.BackSyncUrlNeedsToBeAbsolute, innerException)
		{
		}

		protected BackSyncUrlNeedsToBeAbsoluteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

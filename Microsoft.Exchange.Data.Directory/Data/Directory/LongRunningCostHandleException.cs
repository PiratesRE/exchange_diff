using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LongRunningCostHandleException : ADTransientException
	{
		public LongRunningCostHandleException() : base(DirectoryStrings.LongRunningCostHandle)
		{
		}

		public LongRunningCostHandleException(Exception innerException) : base(DirectoryStrings.LongRunningCostHandle, innerException)
		{
		}

		protected LongRunningCostHandleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

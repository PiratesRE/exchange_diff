using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TestSearchOperationAbortedException : LocalizedException
	{
		public TestSearchOperationAbortedException() : base(Strings.TestSearchOperationAborted)
		{
		}

		public TestSearchOperationAbortedException(Exception innerException) : base(Strings.TestSearchOperationAborted, innerException)
		{
		}

		protected TestSearchOperationAbortedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

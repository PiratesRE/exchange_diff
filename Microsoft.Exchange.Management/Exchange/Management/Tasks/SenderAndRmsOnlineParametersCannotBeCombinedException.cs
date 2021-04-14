using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SenderAndRmsOnlineParametersCannotBeCombinedException : LocalizedException
	{
		public SenderAndRmsOnlineParametersCannotBeCombinedException() : base(Strings.SenderAndRmsOnlineParametersCannotBeCombined)
		{
		}

		public SenderAndRmsOnlineParametersCannotBeCombinedException(Exception innerException) : base(Strings.SenderAndRmsOnlineParametersCannotBeCombined, innerException)
		{
		}

		protected SenderAndRmsOnlineParametersCannotBeCombinedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_InvalidDTMFSequenceReceived : LocalizedException
	{
		public TUC_InvalidDTMFSequenceReceived() : base(Strings.InvalidDTMFSequenceReceived)
		{
		}

		public TUC_InvalidDTMFSequenceReceived(Exception innerException) : base(Strings.InvalidDTMFSequenceReceived, innerException)
		{
		}

		protected TUC_InvalidDTMFSequenceReceived(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

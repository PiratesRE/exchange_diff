using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_InvalidIPAddressReceived : LocalizedException
	{
		public TUC_InvalidIPAddressReceived() : base(Strings.InvalidIPAddressReceived)
		{
		}

		public TUC_InvalidIPAddressReceived(Exception innerException) : base(Strings.InvalidIPAddressReceived, innerException)
		{
		}

		protected TUC_InvalidIPAddressReceived(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

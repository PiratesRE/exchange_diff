using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_NoDTMFSwereReceived : LocalizedException
	{
		public TUC_NoDTMFSwereReceived() : base(Strings.NoDTMFSwereReceived)
		{
		}

		public TUC_NoDTMFSwereReceived(Exception innerException) : base(Strings.NoDTMFSwereReceived, innerException)
		{
		}

		protected TUC_NoDTMFSwereReceived(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UCMAPreReqUpgradeException : LocalizedException
	{
		public UCMAPreReqUpgradeException() : base(Strings.UCMAPreReqUpgradeException)
		{
		}

		public UCMAPreReqUpgradeException(Exception innerException) : base(Strings.UCMAPreReqUpgradeException, innerException)
		{
		}

		protected UCMAPreReqUpgradeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UCMAPreReqException : LocalizedException
	{
		public UCMAPreReqException() : base(Strings.UCMAPreReqException)
		{
		}

		public UCMAPreReqException(Exception innerException) : base(Strings.UCMAPreReqException, innerException)
		{
		}

		protected UCMAPreReqException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

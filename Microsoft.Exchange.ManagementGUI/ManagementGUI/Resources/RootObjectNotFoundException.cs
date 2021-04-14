using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ManagementGUI.Resources
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RootObjectNotFoundException : LocalizedException
	{
		public RootObjectNotFoundException() : base(Strings.RootObjectNotFound)
		{
		}

		public RootObjectNotFoundException(Exception innerException) : base(Strings.RootObjectNotFound, innerException)
		{
		}

		protected RootObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

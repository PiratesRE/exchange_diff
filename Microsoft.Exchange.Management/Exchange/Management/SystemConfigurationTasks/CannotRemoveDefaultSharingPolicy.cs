using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveDefaultSharingPolicy : LocalizedException
	{
		public CannotRemoveDefaultSharingPolicy() : base(Strings.CannotRemoveDefaultSharingPolicy)
		{
		}

		public CannotRemoveDefaultSharingPolicy(Exception innerException) : base(Strings.CannotRemoveDefaultSharingPolicy, innerException)
		{
		}

		protected CannotRemoveDefaultSharingPolicy(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

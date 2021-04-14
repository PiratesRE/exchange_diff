using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskBothConfigurationOnlyAndQuorumOnlySpecifiedException : LocalizedException
	{
		public TaskBothConfigurationOnlyAndQuorumOnlySpecifiedException() : base(Strings.TaskBothConfigurationOnlyAndQuorumOnlySpecified)
		{
		}

		public TaskBothConfigurationOnlyAndQuorumOnlySpecifiedException(Exception innerException) : base(Strings.TaskBothConfigurationOnlyAndQuorumOnlySpecified, innerException)
		{
		}

		protected TaskBothConfigurationOnlyAndQuorumOnlySpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

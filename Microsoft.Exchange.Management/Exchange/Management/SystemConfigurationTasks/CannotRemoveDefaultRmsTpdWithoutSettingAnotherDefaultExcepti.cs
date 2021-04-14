using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveDefaultRmsTpdWithoutSettingAnotherDefaultException : LocalizedException
	{
		public CannotRemoveDefaultRmsTpdWithoutSettingAnotherDefaultException() : base(Strings.CannotRemoveDefaultRmsTpdWithoutSettingAnotherDefault)
		{
		}

		public CannotRemoveDefaultRmsTpdWithoutSettingAnotherDefaultException(Exception innerException) : base(Strings.CannotRemoveDefaultRmsTpdWithoutSettingAnotherDefault, innerException)
		{
		}

		protected CannotRemoveDefaultRmsTpdWithoutSettingAnotherDefaultException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

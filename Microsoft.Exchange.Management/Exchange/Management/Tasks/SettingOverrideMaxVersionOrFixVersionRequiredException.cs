using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideMaxVersionOrFixVersionRequiredException : SettingOverrideException
	{
		public SettingOverrideMaxVersionOrFixVersionRequiredException() : base(Strings.ErrorMaxVersionOrFixVersionRequired)
		{
		}

		public SettingOverrideMaxVersionOrFixVersionRequiredException(Exception innerException) : base(Strings.ErrorMaxVersionOrFixVersionRequired, innerException)
		{
		}

		protected SettingOverrideMaxVersionOrFixVersionRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

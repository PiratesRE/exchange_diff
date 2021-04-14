using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideMaxVersionAndFixVersionSpecifiedException : SettingOverrideException
	{
		public SettingOverrideMaxVersionAndFixVersionSpecifiedException() : base(Strings.ErrorMaxVersionAndFixVersionSpecified)
		{
		}

		public SettingOverrideMaxVersionAndFixVersionSpecifiedException(Exception innerException) : base(Strings.ErrorMaxVersionAndFixVersionSpecified, innerException)
		{
		}

		protected SettingOverrideMaxVersionAndFixVersionSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

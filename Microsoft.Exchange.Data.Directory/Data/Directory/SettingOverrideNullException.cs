using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideNullException : SettingOverrideException
	{
		public SettingOverrideNullException() : base(DirectoryStrings.ErrorSettingOverrideNull)
		{
		}

		public SettingOverrideNullException(Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideNull, innerException)
		{
		}

		protected SettingOverrideNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

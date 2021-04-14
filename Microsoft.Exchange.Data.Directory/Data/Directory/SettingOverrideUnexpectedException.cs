using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideUnexpectedException : SettingOverrideException
	{
		public SettingOverrideUnexpectedException(string errorType) : base(DirectoryStrings.ErrorSettingOverrideUnexpected(errorType))
		{
			this.errorType = errorType;
		}

		public SettingOverrideUnexpectedException(string errorType, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideUnexpected(errorType), innerException)
		{
			this.errorType = errorType;
		}

		protected SettingOverrideUnexpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorType = (string)info.GetValue("errorType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorType", this.errorType);
		}

		public string ErrorType
		{
			get
			{
				return this.errorType;
			}
		}

		private readonly string errorType;
	}
}

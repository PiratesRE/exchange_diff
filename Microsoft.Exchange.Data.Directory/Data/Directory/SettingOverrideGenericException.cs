using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideGenericException : SettingOverrideException
	{
		public SettingOverrideGenericException(string errorType, string componentName, string objectName, string parameters) : base(DirectoryStrings.ErrorSettingOverrideUnknown(errorType, componentName, objectName, parameters))
		{
			this.errorType = errorType;
			this.componentName = componentName;
			this.objectName = objectName;
			this.parameters = parameters;
		}

		public SettingOverrideGenericException(string errorType, string componentName, string objectName, string parameters, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideUnknown(errorType, componentName, objectName, parameters), innerException)
		{
			this.errorType = errorType;
			this.componentName = componentName;
			this.objectName = objectName;
			this.parameters = parameters;
		}

		protected SettingOverrideGenericException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorType = (string)info.GetValue("errorType", typeof(string));
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.objectName = (string)info.GetValue("objectName", typeof(string));
			this.parameters = (string)info.GetValue("parameters", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorType", this.errorType);
			info.AddValue("componentName", this.componentName);
			info.AddValue("objectName", this.objectName);
			info.AddValue("parameters", this.parameters);
		}

		public string ErrorType
		{
			get
			{
				return this.errorType;
			}
		}

		public string ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		public string ObjectName
		{
			get
			{
				return this.objectName;
			}
		}

		public string Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		private readonly string errorType;

		private readonly string componentName;

		private readonly string objectName;

		private readonly string parameters;
	}
}

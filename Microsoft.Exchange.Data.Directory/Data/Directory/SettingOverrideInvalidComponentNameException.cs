using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideInvalidComponentNameException : SettingOverrideException
	{
		public SettingOverrideInvalidComponentNameException(string componentName, string availableComponents) : base(DirectoryStrings.ErrorSettingOverrideInvalidComponentName(componentName, availableComponents))
		{
			this.componentName = componentName;
			this.availableComponents = availableComponents;
		}

		public SettingOverrideInvalidComponentNameException(string componentName, string availableComponents, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideInvalidComponentName(componentName, availableComponents), innerException)
		{
			this.componentName = componentName;
			this.availableComponents = availableComponents;
		}

		protected SettingOverrideInvalidComponentNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.availableComponents = (string)info.GetValue("availableComponents", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("componentName", this.componentName);
			info.AddValue("availableComponents", this.availableComponents);
		}

		public string ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		public string AvailableComponents
		{
			get
			{
				return this.availableComponents;
			}
		}

		private readonly string componentName;

		private readonly string availableComponents;
	}
}

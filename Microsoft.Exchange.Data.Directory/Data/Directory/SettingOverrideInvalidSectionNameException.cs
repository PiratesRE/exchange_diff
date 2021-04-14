using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideInvalidSectionNameException : SettingOverrideException
	{
		public SettingOverrideInvalidSectionNameException(string componentName, string sectionName, string availableObjects) : base(DirectoryStrings.ErrorSettingOverrideInvalidSectionName(componentName, sectionName, availableObjects))
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.availableObjects = availableObjects;
		}

		public SettingOverrideInvalidSectionNameException(string componentName, string sectionName, string availableObjects, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideInvalidSectionName(componentName, sectionName, availableObjects), innerException)
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.availableObjects = availableObjects;
		}

		protected SettingOverrideInvalidSectionNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.sectionName = (string)info.GetValue("sectionName", typeof(string));
			this.availableObjects = (string)info.GetValue("availableObjects", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("componentName", this.componentName);
			info.AddValue("sectionName", this.sectionName);
			info.AddValue("availableObjects", this.availableObjects);
		}

		public string ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		public string SectionName
		{
			get
			{
				return this.sectionName;
			}
		}

		public string AvailableObjects
		{
			get
			{
				return this.availableObjects;
			}
		}

		private readonly string componentName;

		private readonly string sectionName;

		private readonly string availableObjects;
	}
}

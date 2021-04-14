using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SettingOverrideInvalidVariantNameException : SettingOverrideException
	{
		public SettingOverrideInvalidVariantNameException(string componentName, string sectionName, string variantName, string availableVariantNames) : base(DirectoryStrings.ErrorSettingOverrideInvalidVariantName(componentName, sectionName, variantName, availableVariantNames))
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.variantName = variantName;
			this.availableVariantNames = availableVariantNames;
		}

		public SettingOverrideInvalidVariantNameException(string componentName, string sectionName, string variantName, string availableVariantNames, Exception innerException) : base(DirectoryStrings.ErrorSettingOverrideInvalidVariantName(componentName, sectionName, variantName, availableVariantNames), innerException)
		{
			this.componentName = componentName;
			this.sectionName = sectionName;
			this.variantName = variantName;
			this.availableVariantNames = availableVariantNames;
		}

		protected SettingOverrideInvalidVariantNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentName = (string)info.GetValue("componentName", typeof(string));
			this.sectionName = (string)info.GetValue("sectionName", typeof(string));
			this.variantName = (string)info.GetValue("variantName", typeof(string));
			this.availableVariantNames = (string)info.GetValue("availableVariantNames", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("componentName", this.componentName);
			info.AddValue("sectionName", this.sectionName);
			info.AddValue("variantName", this.variantName);
			info.AddValue("availableVariantNames", this.availableVariantNames);
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

		public string VariantName
		{
			get
			{
				return this.variantName;
			}
		}

		public string AvailableVariantNames
		{
			get
			{
				return this.availableVariantNames;
			}
		}

		private readonly string componentName;

		private readonly string sectionName;

		private readonly string variantName;

		private readonly string availableVariantNames;
	}
}

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell.AllSettings
{
	[DebuggerStepThrough]
	[DataContract(Name = "ShellSettingsResponse", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell.AllSettings")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ShellSettingsResponse : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public string AllSettingsControlCSS
		{
			get
			{
				return this.AllSettingsControlCSSField;
			}
			set
			{
				this.AllSettingsControlCSSField = value;
			}
		}

		[DataMember]
		public string AllSettingsControlJS
		{
			get
			{
				return this.AllSettingsControlJSField;
			}
			set
			{
				this.AllSettingsControlJSField = value;
			}
		}

		[DataMember]
		public string ClientData
		{
			get
			{
				return this.ClientDataField;
			}
			set
			{
				this.ClientDataField = value;
			}
		}

		[DataMember]
		public ShellSetting[] ShellSettings
		{
			get
			{
				return this.ShellSettingsField;
			}
			set
			{
				this.ShellSettingsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string AllSettingsControlCSSField;

		private string AllSettingsControlJSField;

		private string ClientDataField;

		private ShellSetting[] ShellSettingsField;
	}
}

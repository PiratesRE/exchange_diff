using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell.AllSettings
{
	[DataContract(Name = "ShellSetting", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell.AllSettings")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class ShellSetting : IExtensibleDataObject
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
		public NavBarLinkData SettingEntry
		{
			get
			{
				return this.SettingEntryField;
			}
			set
			{
				this.SettingEntryField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private NavBarLinkData SettingEntryField;
	}
}

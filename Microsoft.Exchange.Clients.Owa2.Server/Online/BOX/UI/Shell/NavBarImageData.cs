using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell
{
	[DataContract(Name = "NavBarImageData", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class NavBarImageData : IExtensibleDataObject
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
		public string AltText
		{
			get
			{
				return this.AltTextField;
			}
			set
			{
				this.AltTextField = value;
			}
		}

		[DataMember]
		public NavBarImageClipInfo ClipInfo
		{
			get
			{
				return this.ClipInfoField;
			}
			set
			{
				this.ClipInfoField = value;
			}
		}

		[DataMember]
		public NavBarImageClipInfo HoverClipInfo
		{
			get
			{
				return this.HoverClipInfoField;
			}
			set
			{
				this.HoverClipInfoField = value;
			}
		}

		[DataMember]
		public NavBarImageClipInfo PressedClipInfo
		{
			get
			{
				return this.PressedClipInfoField;
			}
			set
			{
				this.PressedClipInfoField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string AltTextField;

		private NavBarImageClipInfo ClipInfoField;

		private NavBarImageClipInfo HoverClipInfoField;

		private NavBarImageClipInfo PressedClipInfoField;
	}
}

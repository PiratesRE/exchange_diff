using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[DataContract(Name = "NavBarInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[KnownType(typeof(ShellInfo))]
	public class NavBarInfo : IExtensibleDataObject
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
		public string NavBarDataJson
		{
			get
			{
				return this.NavBarDataJsonField;
			}
			set
			{
				this.NavBarDataJsonField = value;
			}
		}

		[DataMember]
		public string SharedCSSUrl
		{
			get
			{
				return this.SharedCSSUrlField;
			}
			set
			{
				this.SharedCSSUrlField = value;
			}
		}

		[DataMember]
		public string SharedJSUrl
		{
			get
			{
				return this.SharedJSUrlField;
			}
			set
			{
				this.SharedJSUrlField = value;
			}
		}

		[DataMember]
		public string Version
		{
			get
			{
				return this.VersionField;
			}
			set
			{
				this.VersionField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string NavBarDataJsonField;

		private string SharedCSSUrlField;

		private string SharedJSUrlField;

		private string VersionField;
	}
}

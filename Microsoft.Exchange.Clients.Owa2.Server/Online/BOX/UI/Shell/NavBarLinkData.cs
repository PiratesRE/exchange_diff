using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell
{
	[DebuggerStepThrough]
	[DataContract(Name = "NavBarLinkData", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class NavBarLinkData : IExtensibleDataObject
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
		public string Id
		{
			get
			{
				return this.IdField;
			}
			set
			{
				this.IdField = value;
			}
		}

		[DataMember]
		public string MenuName
		{
			get
			{
				return this.MenuNameField;
			}
			set
			{
				this.MenuNameField = value;
			}
		}

		[DataMember]
		public NavBarLinkData[] SubLinks
		{
			get
			{
				return this.SubLinksField;
			}
			set
			{
				this.SubLinksField = value;
			}
		}

		[DataMember]
		public string TargetWindow
		{
			get
			{
				return this.TargetWindowField;
			}
			set
			{
				this.TargetWindowField = value;
			}
		}

		[DataMember]
		public string Text
		{
			get
			{
				return this.TextField;
			}
			set
			{
				this.TextField = value;
			}
		}

		[DataMember]
		public string Title
		{
			get
			{
				return this.TitleField;
			}
			set
			{
				this.TitleField = value;
			}
		}

		[DataMember]
		public string Url
		{
			get
			{
				return this.UrlField;
			}
			set
			{
				this.UrlField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string IdField;

		private string MenuNameField;

		private NavBarLinkData[] SubLinksField;

		private string TargetWindowField;

		private string TextField;

		private string TitleField;

		private string UrlField;
	}
}

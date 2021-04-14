using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.UI.Shell
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "NavBarImageClipInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.UI.Shell")]
	public class NavBarImageClipInfo : IExtensibleDataObject
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
		public int Height
		{
			get
			{
				return this.HeightField;
			}
			set
			{
				this.HeightField = value;
			}
		}

		[DataMember]
		public int Width
		{
			get
			{
				return this.WidthField;
			}
			set
			{
				this.WidthField = value;
			}
		}

		[DataMember]
		public int X
		{
			get
			{
				return this.XField;
			}
			set
			{
				this.XField = value;
			}
		}

		[DataMember]
		public int Y
		{
			get
			{
				return this.YField;
			}
			set
			{
				this.YField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private int HeightField;

		private int WidthField;

		private int XField;

		private int YField;
	}
}

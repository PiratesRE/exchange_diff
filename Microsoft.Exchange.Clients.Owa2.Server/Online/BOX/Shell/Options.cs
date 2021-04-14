using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[DataContract(Name = "Options", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class Options : IExtensibleDataObject
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
		public Filters? Filters
		{
			get
			{
				return this.FiltersField;
			}
			set
			{
				this.FiltersField = value;
			}
		}

		[DataMember]
		public bool ReturnFooterInfo
		{
			get
			{
				return this.ReturnFooterInfoField;
			}
			set
			{
				this.ReturnFooterInfoField = value;
			}
		}

		[DataMember]
		public bool ReturnHttpsUrls
		{
			get
			{
				return this.ReturnHttpsUrlsField;
			}
			set
			{
				this.ReturnHttpsUrlsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Filters? FiltersField;

		private bool ReturnFooterInfoField;

		private bool ReturnHttpsUrlsField;
	}
}

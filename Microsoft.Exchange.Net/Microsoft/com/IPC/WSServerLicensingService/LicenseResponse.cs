using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.com.IPC.WSService;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[DataContract(Name = "LicenseResponse", Namespace = "http://microsoft.com/IPC/WSServerLicensingService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	public class LicenseResponse : IExtensibleDataObject
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

		[DataMember(EmitDefaultValue = false)]
		public string EndUseLicense
		{
			get
			{
				return this.EndUseLicenseField;
			}
			set
			{
				this.EndUseLicenseField = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public ActiveFederationFault Fault
		{
			get
			{
				return this.FaultField;
			}
			set
			{
				this.FaultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string EndUseLicenseField;

		private ActiveFederationFault FaultField;
	}
}

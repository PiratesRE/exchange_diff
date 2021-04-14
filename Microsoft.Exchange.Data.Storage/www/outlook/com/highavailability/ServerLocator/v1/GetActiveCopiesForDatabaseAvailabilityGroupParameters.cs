using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace www.outlook.com.highavailability.ServerLocator.v1
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GetActiveCopiesForDatabaseAvailabilityGroupParameters", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	[DebuggerStepThrough]
	public class GetActiveCopiesForDatabaseAvailabilityGroupParameters : IExtensibleDataObject
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
		public bool CachedData
		{
			get
			{
				return this.CachedDataField;
			}
			set
			{
				this.CachedDataField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private bool CachedDataField;
	}
}

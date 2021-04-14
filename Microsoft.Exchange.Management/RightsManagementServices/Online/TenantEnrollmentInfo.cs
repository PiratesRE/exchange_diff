using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "TenantEnrollmentInfo", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	[KnownType(typeof(TenantInfo))]
	public class TenantEnrollmentInfo : IExtensibleDataObject
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
		public string TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		[DataMember(Order = 1)]
		public string FriendlyName
		{
			get
			{
				return this.FriendlyNameField;
			}
			set
			{
				this.FriendlyNameField = value;
			}
		}

		[DataMember(Order = 2)]
		public string OnpremiseRightsMgmtSvcDomainName
		{
			get
			{
				return this.OnpremiseRightsMgmtSvcDomainNameField;
			}
			set
			{
				this.OnpremiseRightsMgmtSvcDomainNameField = value;
			}
		}

		[DataMember(Order = 3)]
		public CryptoModeScheme CryptoMode
		{
			get
			{
				return this.CryptoModeField;
			}
			set
			{
				this.CryptoModeField = value;
			}
		}

		[DataMember(Order = 4)]
		public HierarchyType Hierarchy
		{
			get
			{
				return this.HierarchyField;
			}
			set
			{
				this.HierarchyField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string TenantIdField;

		private string FriendlyNameField;

		private string OnpremiseRightsMgmtSvcDomainNameField;

		private CryptoModeScheme CryptoModeField;

		private HierarchyType HierarchyField;
	}
}

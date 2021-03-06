using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Online.BOX.Shell
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "NavBarInfoRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[KnownType(typeof(ShellInfoRequest))]
	public class NavBarInfoRequest : IExtensibleDataObject
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
		public string BrandId
		{
			get
			{
				return this.BrandIdField;
			}
			set
			{
				this.BrandIdField = value;
			}
		}

		[DataMember]
		public string CultureName
		{
			get
			{
				return this.CultureNameField;
			}
			set
			{
				this.CultureNameField = value;
			}
		}

		[DataMember]
		public NavBarMainLinkID CurrentMainLinkID
		{
			get
			{
				return this.CurrentMainLinkIDField;
			}
			set
			{
				this.CurrentMainLinkIDField = value;
			}
		}

		[DataMember]
		public string TrackingGuid
		{
			get
			{
				return this.TrackingGuidField;
			}
			set
			{
				this.TrackingGuidField = value;
			}
		}

		[DataMember]
		public string UserPrincipalName
		{
			get
			{
				return this.UserPrincipalNameField;
			}
			set
			{
				this.UserPrincipalNameField = value;
			}
		}

		[DataMember]
		public string UserPuid
		{
			get
			{
				return this.UserPuidField;
			}
			set
			{
				this.UserPuidField = value;
			}
		}

		[DataMember]
		public WorkloadAuthenticationId WorkloadId
		{
			get
			{
				return this.WorkloadIdField;
			}
			set
			{
				this.WorkloadIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string BrandIdField;

		private string CultureNameField;

		private NavBarMainLinkID CurrentMainLinkIDField;

		private string TrackingGuidField;

		private string UserPrincipalNameField;

		private string UserPuidField;

		private WorkloadAuthenticationId WorkloadIdField;
	}
}

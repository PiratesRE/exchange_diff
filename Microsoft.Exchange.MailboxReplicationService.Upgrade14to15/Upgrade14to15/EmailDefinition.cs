using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "EmailDefinition", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.BDM.Pets.Email.Platform")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class EmailDefinition : IExtensibleDataObject
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
		public EmailAttribute[] Attributes
		{
			get
			{
				return this.AttributesField;
			}
			set
			{
				this.AttributesField = value;
			}
		}

		[DataMember]
		public string[] BCCList
		{
			get
			{
				return this.BCCListField;
			}
			set
			{
				this.BCCListField = value;
			}
		}

		[DataMember]
		public string[] CClist
		{
			get
			{
				return this.CClistField;
			}
			set
			{
				this.CClistField = value;
			}
		}

		[DataMember]
		public string EmailAddress
		{
			get
			{
				return this.EmailAddressField;
			}
			set
			{
				this.EmailAddressField = value;
			}
		}

		[DataMember]
		public string EmailId
		{
			get
			{
				return this.EmailIdField;
			}
			set
			{
				this.EmailIdField = value;
			}
		}

		[DataMember]
		public string LocaleId
		{
			get
			{
				return this.LocaleIdField;
			}
			set
			{
				this.LocaleIdField = value;
			}
		}

		[DataMember]
		public Guid TrackingId
		{
			get
			{
				return this.TrackingIdField;
			}
			set
			{
				this.TrackingIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private EmailAttribute[] AttributesField;

		private string[] BCCListField;

		private string[] CClistField;

		private string EmailAddressField;

		private string EmailIdField;

		private string LocaleIdField;

		private Guid TrackingIdField;
	}
}

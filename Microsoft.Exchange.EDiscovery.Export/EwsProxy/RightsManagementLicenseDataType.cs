using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class RightsManagementLicenseDataType
	{
		public int RightsManagedMessageDecryptionStatus
		{
			get
			{
				return this.rightsManagedMessageDecryptionStatusField;
			}
			set
			{
				this.rightsManagedMessageDecryptionStatusField = value;
			}
		}

		[XmlIgnore]
		public bool RightsManagedMessageDecryptionStatusSpecified
		{
			get
			{
				return this.rightsManagedMessageDecryptionStatusFieldSpecified;
			}
			set
			{
				this.rightsManagedMessageDecryptionStatusFieldSpecified = value;
			}
		}

		public string RmsTemplateId
		{
			get
			{
				return this.rmsTemplateIdField;
			}
			set
			{
				this.rmsTemplateIdField = value;
			}
		}

		public string TemplateName
		{
			get
			{
				return this.templateNameField;
			}
			set
			{
				this.templateNameField = value;
			}
		}

		public string TemplateDescription
		{
			get
			{
				return this.templateDescriptionField;
			}
			set
			{
				this.templateDescriptionField = value;
			}
		}

		public bool EditAllowed
		{
			get
			{
				return this.editAllowedField;
			}
			set
			{
				this.editAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool EditAllowedSpecified
		{
			get
			{
				return this.editAllowedFieldSpecified;
			}
			set
			{
				this.editAllowedFieldSpecified = value;
			}
		}

		public bool ReplyAllowed
		{
			get
			{
				return this.replyAllowedField;
			}
			set
			{
				this.replyAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool ReplyAllowedSpecified
		{
			get
			{
				return this.replyAllowedFieldSpecified;
			}
			set
			{
				this.replyAllowedFieldSpecified = value;
			}
		}

		public bool ReplyAllAllowed
		{
			get
			{
				return this.replyAllAllowedField;
			}
			set
			{
				this.replyAllAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool ReplyAllAllowedSpecified
		{
			get
			{
				return this.replyAllAllowedFieldSpecified;
			}
			set
			{
				this.replyAllAllowedFieldSpecified = value;
			}
		}

		public bool ForwardAllowed
		{
			get
			{
				return this.forwardAllowedField;
			}
			set
			{
				this.forwardAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool ForwardAllowedSpecified
		{
			get
			{
				return this.forwardAllowedFieldSpecified;
			}
			set
			{
				this.forwardAllowedFieldSpecified = value;
			}
		}

		public bool ModifyRecipientsAllowed
		{
			get
			{
				return this.modifyRecipientsAllowedField;
			}
			set
			{
				this.modifyRecipientsAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool ModifyRecipientsAllowedSpecified
		{
			get
			{
				return this.modifyRecipientsAllowedFieldSpecified;
			}
			set
			{
				this.modifyRecipientsAllowedFieldSpecified = value;
			}
		}

		public bool ExtractAllowed
		{
			get
			{
				return this.extractAllowedField;
			}
			set
			{
				this.extractAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool ExtractAllowedSpecified
		{
			get
			{
				return this.extractAllowedFieldSpecified;
			}
			set
			{
				this.extractAllowedFieldSpecified = value;
			}
		}

		public bool PrintAllowed
		{
			get
			{
				return this.printAllowedField;
			}
			set
			{
				this.printAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool PrintAllowedSpecified
		{
			get
			{
				return this.printAllowedFieldSpecified;
			}
			set
			{
				this.printAllowedFieldSpecified = value;
			}
		}

		public bool ExportAllowed
		{
			get
			{
				return this.exportAllowedField;
			}
			set
			{
				this.exportAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool ExportAllowedSpecified
		{
			get
			{
				return this.exportAllowedFieldSpecified;
			}
			set
			{
				this.exportAllowedFieldSpecified = value;
			}
		}

		public bool ProgrammaticAccessAllowed
		{
			get
			{
				return this.programmaticAccessAllowedField;
			}
			set
			{
				this.programmaticAccessAllowedField = value;
			}
		}

		[XmlIgnore]
		public bool ProgrammaticAccessAllowedSpecified
		{
			get
			{
				return this.programmaticAccessAllowedFieldSpecified;
			}
			set
			{
				this.programmaticAccessAllowedFieldSpecified = value;
			}
		}

		public bool IsOwner
		{
			get
			{
				return this.isOwnerField;
			}
			set
			{
				this.isOwnerField = value;
			}
		}

		[XmlIgnore]
		public bool IsOwnerSpecified
		{
			get
			{
				return this.isOwnerFieldSpecified;
			}
			set
			{
				this.isOwnerFieldSpecified = value;
			}
		}

		public string ContentOwner
		{
			get
			{
				return this.contentOwnerField;
			}
			set
			{
				this.contentOwnerField = value;
			}
		}

		public string ContentExpiryDate
		{
			get
			{
				return this.contentExpiryDateField;
			}
			set
			{
				this.contentExpiryDateField = value;
			}
		}

		private int rightsManagedMessageDecryptionStatusField;

		private bool rightsManagedMessageDecryptionStatusFieldSpecified;

		private string rmsTemplateIdField;

		private string templateNameField;

		private string templateDescriptionField;

		private bool editAllowedField;

		private bool editAllowedFieldSpecified;

		private bool replyAllowedField;

		private bool replyAllowedFieldSpecified;

		private bool replyAllAllowedField;

		private bool replyAllAllowedFieldSpecified;

		private bool forwardAllowedField;

		private bool forwardAllowedFieldSpecified;

		private bool modifyRecipientsAllowedField;

		private bool modifyRecipientsAllowedFieldSpecified;

		private bool extractAllowedField;

		private bool extractAllowedFieldSpecified;

		private bool printAllowedField;

		private bool printAllowedFieldSpecified;

		private bool exportAllowedField;

		private bool exportAllowedFieldSpecified;

		private bool programmaticAccessAllowedField;

		private bool programmaticAccessAllowedFieldSpecified;

		private bool isOwnerField;

		private bool isOwnerFieldSpecified;

		private string contentOwnerField;

		private string contentExpiryDateField;
	}
}

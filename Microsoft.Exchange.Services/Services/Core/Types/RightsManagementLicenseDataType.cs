using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class RightsManagementLicenseDataType
	{
		public RightsManagementLicenseDataType()
		{
		}

		internal RightsManagementLicenseDataType(ContentRight usageRights)
		{
			this.EditAllowed = usageRights.IsUsageRightGranted(ContentRight.Edit);
			this.ReplyAllowed = usageRights.IsUsageRightGranted(ContentRight.Reply);
			this.ReplyAllAllowed = usageRights.IsUsageRightGranted(ContentRight.ReplyAll);
			this.ForwardAllowed = usageRights.IsUsageRightGranted(ContentRight.Forward);
			this.PrintAllowed = usageRights.IsUsageRightGranted(ContentRight.Print);
			this.ExtractAllowed = usageRights.IsUsageRightGranted(ContentRight.Extract);
			this.ProgrammaticAccessAllowed = usageRights.IsUsageRightGranted(ContentRight.ObjectModel);
			this.IsOwner = usageRights.IsUsageRightGranted(ContentRight.Owner);
		}

		[DataMember(EmitDefaultValue = false)]
		public int RightsManagedMessageDecryptionStatus { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string RmsTemplateId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string TemplateName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string TemplateDescription { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool EditAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ReplyAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ReplyAllAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ForwardAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ModifyRecipientsAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExtractAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool PrintAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ExportAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool ProgrammaticAccessAllowed { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IsOwner { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ContentOwner { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string ContentExpiryDate { get; set; }

		[IgnoreDataMember]
		[XmlElement("BodyType")]
		public BodyType BodyType { get; set; }

		[XmlIgnore]
		[DataMember(Name = "BodyType", EmitDefaultValue = false)]
		public string BodyTypeString
		{
			get
			{
				return EnumUtilities.ToString<BodyType>(this.BodyType);
			}
			set
			{
				this.BodyType = EnumUtilities.Parse<BodyType>(value);
			}
		}

		internal static RightsManagementLicenseDataType CreateNoRightsTemplate()
		{
			return new RightsManagementLicenseDataType
			{
				RmsTemplateId = Guid.Empty.ToString()
			};
		}
	}
}

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "UserIdType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class UserId
	{
		[XmlElement("SID")]
		[DataMember(Name = "SID", EmitDefaultValue = false)]
		public string Sid { get; set; }

		[XmlElement("PrimarySmtpAddress")]
		[DataMember(Name = "PrimarySmtpAddress", EmitDefaultValue = false)]
		public string PrimarySmtpAddress { get; set; }

		[DataMember(Name = "DisplayName", EmitDefaultValue = false)]
		[XmlElement("DisplayName")]
		public string DisplayName { get; set; }

		[IgnoreDataMember]
		[XmlElement("DistinguishedUser")]
		[DefaultValue(DistinguishedUserType.None)]
		public DistinguishedUserType DistinguishedUser
		{
			get
			{
				return this.distinguishedUser;
			}
			set
			{
				this.distinguishedUser = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "DistinguishedUser", EmitDefaultValue = false)]
		public string DistinguishedUserString
		{
			get
			{
				if (this.DistinguishedUser == DistinguishedUserType.None)
				{
					return null;
				}
				return EnumUtilities.ToString<DistinguishedUserType>(this.DistinguishedUser);
			}
			set
			{
				this.DistinguishedUser = EnumUtilities.Parse<DistinguishedUserType>(value);
			}
		}

		[DataMember(Name = "ExternalUserIdentity", EmitDefaultValue = false)]
		[XmlElement("ExternalUserIdentity", IsNullable = false)]
		public string ExternalUserIdentity { get; set; }

		private DistinguishedUserType distinguishedUser;
	}
}

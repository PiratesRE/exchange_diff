using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[KnownType(typeof(Permissions))]
	[XmlType("Permissions")]
	[DataContract(Name = "Permissions")]
	public class Permissions
	{
		[DataMember(Name = "AdmissionType", EmitDefaultValue = true)]
		[XmlElement("AdmissionType")]
		public AdmissionType AdmissionType { get; set; }

		internal static Permissions ConvertFrom(AccessLevel accessLevel)
		{
			Permissions permissions = new Permissions();
			switch (accessLevel)
			{
			case AccessLevel.SameEnterprise:
				permissions.AdmissionType = AdmissionType.ucOpenAuthenticated;
				break;
			case AccessLevel.Locked:
				permissions.AdmissionType = AdmissionType.ucLocked;
				break;
			case AccessLevel.Invited:
				permissions.AdmissionType = AdmissionType.ucClosedAuthenticated;
				break;
			case AccessLevel.Everyone:
				permissions.AdmissionType = AdmissionType.ucAnonymous;
				break;
			default:
				throw new InvalidEnumArgumentException("Invalid value for AdmissionType");
			}
			return permissions;
		}
	}
}

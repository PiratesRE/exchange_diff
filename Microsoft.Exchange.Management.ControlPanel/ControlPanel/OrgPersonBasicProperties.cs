using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class OrgPersonBasicProperties : SetObjectProperties
	{
		[DataMember]
		public string FirstName
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.FirstName];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.FirstName] = value;
			}
		}

		[DataMember]
		public string Initials
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Initials];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Initials] = value;
			}
		}

		[DataMember]
		public string LastName
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.LastName];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.LastName] = value;
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.DisplayName];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.DisplayName] = value;
			}
		}
	}
}

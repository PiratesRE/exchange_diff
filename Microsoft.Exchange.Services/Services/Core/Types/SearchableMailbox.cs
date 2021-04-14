using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "SearchableMailbox", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SearchableMailboxType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SearchableMailbox : IComparable
	{
		public SearchableMailbox()
		{
		}

		internal SearchableMailbox(Guid guid, string primarySmtpAddress, bool isExternalMailbox, string externalEmailAddress, string displayName, bool isMembershipGroup, string referenceId)
		{
			this.Guid = guid;
			this.PrimarySmtpAddress = primarySmtpAddress;
			this.IsExternalMailbox = isExternalMailbox;
			this.ExternalEmailAddress = externalEmailAddress;
			this.DisplayName = displayName;
			this.IsMembershipGroup = isMembershipGroup;
			this.ReferenceId = referenceId;
		}

		[DataMember(Name = "Guid", IsRequired = true)]
		[XmlElement("Guid")]
		public Guid Guid { get; set; }

		[XmlElement("PrimarySmtpAddress")]
		[DataMember(Name = "PrimarySmtpAddress", IsRequired = true)]
		public string PrimarySmtpAddress { get; set; }

		[XmlElement("IsExternalMailbox")]
		[DataMember(Name = "IsExternalMailbox", IsRequired = true)]
		public bool IsExternalMailbox { get; set; }

		[DataMember(Name = "ExternalEmailAddress", IsRequired = true)]
		[XmlElement("ExternalEmailAddress")]
		public string ExternalEmailAddress { get; set; }

		[DataMember(Name = "DisplayName", IsRequired = true)]
		[XmlElement("DisplayName")]
		public string DisplayName { get; set; }

		[XmlElement("IsMembershipGroup")]
		[DataMember(Name = "IsMembershipGroup", IsRequired = true)]
		public bool IsMembershipGroup { get; set; }

		[XmlElement("ReferenceId")]
		[DataMember(Name = "ReferenceId", IsRequired = true)]
		public string ReferenceId { get; set; }

		public override bool Equals(object obj)
		{
			SearchableMailbox searchableMailbox = obj as SearchableMailbox;
			return searchableMailbox != null && string.Compare(this.ReferenceId, searchableMailbox.ReferenceId, StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int CompareTo(object obj)
		{
			SearchableMailbox searchableMailbox = (SearchableMailbox)obj;
			return string.Compare(this.DisplayName, searchableMailbox.DisplayName);
		}
	}
}

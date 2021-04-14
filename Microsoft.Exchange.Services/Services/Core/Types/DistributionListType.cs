using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "DistributionList")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class DistributionListType : ItemType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string DisplayName
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(DistributionListSchema.DisplayName);
			}
			set
			{
				base.PropertyBag[DistributionListSchema.DisplayName] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string FileAs
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(DistributionListSchema.FileAs);
			}
			set
			{
				base.PropertyBag[DistributionListSchema.FileAs] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public ContactSourceType ContactSource
		{
			get
			{
				return this.contactSource;
			}
			set
			{
				this.contactSource = value;
				this.contactSourceSpecified = true;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ContactSourceSpecified
		{
			get
			{
				return this.contactSourceSpecified;
			}
			set
			{
				this.contactSourceSpecified = value;
			}
		}

		[XmlArrayItem("Member", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 4)]
		public MemberType[] Members
		{
			get
			{
				return base.GetValueOrDefault<MemberType[]>(DistributionListSchema.Members);
			}
			set
			{
				this[DistributionListSchema.Members] = value;
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.DistributionList;
			}
		}

		private ContactSourceType contactSource = ContactSourceType.Store;

		private bool contactSourceSpecified;
	}
}

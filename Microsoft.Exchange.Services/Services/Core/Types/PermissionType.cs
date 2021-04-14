using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Permission")]
	[Serializable]
	public class PermissionType : BasePermissionType
	{
		[XmlElement]
		[IgnoreDataMember]
		public PermissionReadAccess? ReadItems { get; set; }

		[XmlIgnore]
		[DataMember(Name = "ReadItems", EmitDefaultValue = false, Order = 1)]
		public string ReadItemsString
		{
			get
			{
				if (this.ReadItems == null)
				{
					return null;
				}
				return EnumUtilities.ToString<PermissionReadAccess>(this.ReadItems.Value);
			}
			set
			{
				this.ReadItems = new PermissionReadAccess?(EnumUtilities.Parse<PermissionReadAccess>(value));
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ReadItemsSpecified
		{
			get
			{
				return this.ReadItems != null;
			}
			set
			{
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public PermissionLevelType PermissionLevel { get; set; }

		[DataMember(Name = "PermissionLevel", EmitDefaultValue = false, Order = 2)]
		[XmlIgnore]
		public string PermissionLevelString
		{
			get
			{
				return EnumUtilities.ToString<PermissionLevelType>(this.PermissionLevel);
			}
			set
			{
				this.PermissionLevel = EnumUtilities.Parse<PermissionLevelType>(value);
			}
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(SearchFolderType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Folder")]
	[KnownType(typeof(TasksFolderType))]
	[XmlInclude(typeof(TasksFolderType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(SearchFolderType))]
	[Serializable]
	public class FolderType : BaseFolderType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public PermissionSetType PermissionSet
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<PermissionSetType>(FolderSchema.PermissionSet);
			}
			set
			{
				base.PropertyBag[FolderSchema.PermissionSet] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public int? UnreadCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int?>(FolderSchema.UnreadCount);
			}
			set
			{
				base.PropertyBag[FolderSchema.UnreadCount] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool UnreadCountSpecified
		{
			get
			{
				return base.IsSet(FolderSchema.UnreadCount);
			}
			set
			{
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.Folder;
			}
		}
	}
}

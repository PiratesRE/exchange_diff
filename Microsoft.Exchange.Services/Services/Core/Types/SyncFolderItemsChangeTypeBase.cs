using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(SyncFolderItemsReadFlagType))]
	[KnownType(typeof(SyncFolderItemsCreateOrUpdateType))]
	[KnownType(typeof(SyncFolderItemsDeleteType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class SyncFolderItemsChangeTypeBase
	{
		public SyncFolderItemsChangeTypeBase()
		{
		}

		public abstract SyncFolderItemsChangesEnum ChangeType { get; }

		[XmlIgnore]
		[DataMember(Name = "ChangeType", IsRequired = true)]
		public string ChangeTypeString
		{
			get
			{
				return EnumUtilities.ToString<SyncFolderItemsChangesEnum>(this.ChangeType);
			}
			set
			{
			}
		}
	}
}

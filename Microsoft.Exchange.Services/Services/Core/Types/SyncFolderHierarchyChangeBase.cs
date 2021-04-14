using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(SyncFolderHierarchyDeleteType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(SyncFolderHierarchyCreateOrUpdateType))]
	public abstract class SyncFolderHierarchyChangeBase
	{
		public SyncFolderHierarchyChangeBase()
		{
		}

		public abstract SyncFolderHierarchyChangesEnum ChangeType { get; }

		[DataMember(Name = "ChangeType", IsRequired = true)]
		[XmlIgnore]
		public string ChangeTypeString
		{
			get
			{
				return EnumUtilities.ToString<SyncFolderHierarchyChangesEnum>(this.ChangeType);
			}
			set
			{
			}
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "TasksFolder")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TasksFolderType : FolderType
	{
		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.TasksFolder;
			}
		}
	}
}

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "RetentionActionType", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "RetentionActionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public enum RetentionActionType
	{
		None,
		MoveToDeletedItems,
		MoveToFolder,
		DeleteAndAllowRecovery,
		PermanentlyDelete,
		MarkAsPastRetentionLimit,
		MoveToArchive
	}
}

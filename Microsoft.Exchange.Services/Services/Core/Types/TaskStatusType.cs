using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "TaskStatusType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum TaskStatusType
	{
		NotStarted,
		InProgress,
		Completed,
		WaitingOnOthers,
		Deferred
	}
}

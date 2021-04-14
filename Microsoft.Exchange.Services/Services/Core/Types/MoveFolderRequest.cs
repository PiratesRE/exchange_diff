using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("MoveFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class MoveFolderRequest : BaseMoveCopyFolderRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new MoveFolder(callContext, this);
		}

		internal const string ElementName = "MoveFolder";
	}
}

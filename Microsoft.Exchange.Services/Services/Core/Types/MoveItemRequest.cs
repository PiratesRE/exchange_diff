using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("MoveItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class MoveItemRequest : BaseMoveCopyItemRequest
	{
		protected override string CommandName
		{
			get
			{
				return "moveitem";
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this.CanOptimizeCommandExecution(callContext))
			{
				return new MoveItemBatch(callContext, this);
			}
			return new MoveItem(callContext, this);
		}

		internal const string ElementName = "MoveItem";
	}
}

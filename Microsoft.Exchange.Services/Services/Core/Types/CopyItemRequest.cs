using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("CopyItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CopyItemRequest : BaseMoveCopyItemRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this.CanOptimizeCommandExecution(callContext))
			{
				return new CopyItemBatch(callContext, this);
			}
			return new CopyItem(callContext, this);
		}

		protected override string CommandName
		{
			get
			{
				return "copyitem";
			}
		}

		internal const string ElementName = "CopyItem";
	}
}

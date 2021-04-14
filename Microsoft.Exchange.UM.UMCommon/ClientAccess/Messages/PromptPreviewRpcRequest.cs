using System;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class PromptPreviewRpcRequest : RequestBase
	{
		internal override string GetFriendlyName()
		{
			return Strings.PromptPreviewRpcRequest;
		}
	}
}

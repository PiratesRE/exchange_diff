using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public struct MapiPropertyProblem
	{
		public StorePropTag MapiPropTag;

		public ErrorCodeValue ErrorCode;
	}
}

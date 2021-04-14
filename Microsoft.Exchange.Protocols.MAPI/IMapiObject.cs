using System;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal interface IMapiObject : IDisposable, IServerObject, ICountableObject
	{
	}
}

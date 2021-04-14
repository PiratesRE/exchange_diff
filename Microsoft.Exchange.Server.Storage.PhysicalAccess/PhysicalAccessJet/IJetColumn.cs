using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal interface IJetColumn : IColumn
	{
		byte[] GetValueAsBytes(IJetSimpleQueryOperator cursor);
	}
}

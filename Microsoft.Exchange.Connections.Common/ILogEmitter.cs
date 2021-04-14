using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface ILogEmitter
	{
		void Emit(string formatString, params object[] args);
	}
}

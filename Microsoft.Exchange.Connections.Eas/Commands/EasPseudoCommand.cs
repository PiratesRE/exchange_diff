using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EasPseudoCommand<TRequest, TResponse> : EasCommand<TRequest, TResponse> where TResponse : IHaveAnHttpStatus, new()
	{
		protected internal EasPseudoCommand(Command command, EasConnectionSettings easConnectionSettings) : base(command, easConnectionSettings)
		{
		}
	}
}

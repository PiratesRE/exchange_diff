using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EasServerCommand<TRequest, TResponse, TStatus> : EasCommand<TRequest, TResponse> where TResponse : IEasServerResponse<TStatus>, new() where TStatus : struct, IConvertible
	{
		protected internal EasServerCommand(Command command, EasConnectionSettings easConnectionSettings) : base(command, easConnectionSettings)
		{
			base.InitializeExpectedHttpStatusCodes(typeof(HttpStatus));
		}

		internal override TResponse Execute(TRequest request)
		{
			TResponse tresponse = base.Execute(request);
			TStatus status = tresponse.ConvertStatusToEnum();
			tresponse.ThrowIfStatusIsFailed(status);
			return tresponse;
		}
	}
}

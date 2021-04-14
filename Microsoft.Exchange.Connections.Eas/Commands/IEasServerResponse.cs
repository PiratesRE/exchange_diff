using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	public interface IEasServerResponse<T> : IHaveAnHttpStatus where T : struct, IConvertible
	{
		bool IsSucceeded(T status);

		T ConvertStatusToEnum();
	}
}

using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	public interface IHaveAnHttpStatus
	{
		HttpStatus HttpStatus { get; set; }
	}
}

using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public interface IEcpValidator
	{
		string DefaultErrorMessage { get; }

		string TypeId { get; }
	}
}

using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IMonadDataProvider
	{
		object[] ExecuteCommand(string command);
	}
}

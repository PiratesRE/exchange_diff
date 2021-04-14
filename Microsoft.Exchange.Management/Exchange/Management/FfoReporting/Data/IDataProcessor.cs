using System;

namespace Microsoft.Exchange.Management.FfoReporting.Data
{
	internal interface IDataProcessor
	{
		object Process(object input);
	}
}

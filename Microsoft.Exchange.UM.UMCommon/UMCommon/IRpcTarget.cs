using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IRpcTarget
	{
		string Name { get; }

		ADConfigurationObject ConfigObject { get; }
	}
}

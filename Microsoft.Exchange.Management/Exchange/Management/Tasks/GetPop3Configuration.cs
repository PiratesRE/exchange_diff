using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "PopSettings")]
	[LocDescription(Strings.IDs.GetPop3ConfigurationTask)]
	public sealed class GetPop3Configuration : GetPopImapConfiguration<Pop3AdConfiguration>
	{
	}
}

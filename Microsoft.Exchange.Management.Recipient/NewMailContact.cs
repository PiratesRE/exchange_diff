using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MailContact", SupportsShouldProcess = true)]
	public sealed class NewMailContact : NewMailContactBase
	{
		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			MailContact result2 = new MailContact((ADContact)result);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}
	}
}

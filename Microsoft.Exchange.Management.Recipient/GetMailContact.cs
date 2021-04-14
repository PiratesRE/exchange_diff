using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[OutputType(new Type[]
	{
		typeof(MailContact)
	})]
	[Cmdlet("Get", "MailContact", DefaultParameterSetName = "Identity")]
	public sealed class GetMailContact : GetMailContactBase
	{
		[Parameter(Mandatory = false)]
		public new long UsnForReconciliationSearch
		{
			get
			{
				return base.UsnForReconciliationSearch;
			}
			set
			{
				base.UsnForReconciliationSearch = value;
			}
		}
	}
}

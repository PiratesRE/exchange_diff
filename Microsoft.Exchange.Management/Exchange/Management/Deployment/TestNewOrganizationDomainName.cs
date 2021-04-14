using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Test", "NewOrganizationDomainName")]
	public sealed class TestNewOrganizationDomainName : Task
	{
		[Parameter(Mandatory = true)]
		public SmtpDomain Value
		{
			get
			{
				return (SmtpDomain)base.Fields["Value"];
			}
			set
			{
				base.Fields["Value"] = value;
			}
		}

		private string ParameterName
		{
			get
			{
				return "DomainName";
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (this.Value.ToString().Length > 64)
			{
				base.WriteError(new ArgumentException(Strings.ErrorNameValueStringTooLong(this.ParameterName, 64, this.Value.ToString().Length)), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}

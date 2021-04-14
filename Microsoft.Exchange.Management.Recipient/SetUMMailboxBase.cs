using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetUMMailboxBase<TIdentity, TPublicObject> : SetRecipientObjectTask<TIdentity, TPublicObject, ADUser> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new()
	{
		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeArbitrationMailbox(adrecipient, false))
			{
				TIdentity identity = this.Identity;
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, this.Identity);
			}
			return adrecipient;
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter UMMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields["UMMailboxPolicy"];
			}
			set
			{
				base.Fields["UMMailboxPolicy"] = value;
			}
		}

		public SetUMMailboxBase()
		{
		}

		private const string UMMailboxPolicyName = "UMMailboxPolicy";
	}
}

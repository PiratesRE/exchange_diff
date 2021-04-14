using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class RemoveDlpPolicyImpl : CmdletImplementation
	{
		public RemoveDlpPolicyImpl(RemoveDlpPolicy taskObject)
		{
			this.taskObject = taskObject;
		}

		public override void Validate()
		{
			if (this.taskObject.Identity == null)
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorInvalidDlpPolicyIdentity, RemoveDlpPolicyImpl.Identity), ErrorCategory.InvalidArgument, this.taskObject.Identity);
			}
		}

		public override void ProcessRecord()
		{
			try
			{
				DlpUtils.DeleteEtrsByDlpPolicy(this.taskObject.GetDataObject().ImmutableId, base.DataSession);
			}
			catch (ParserException ex)
			{
				this.taskObject.WriteError(new ArgumentException(Strings.RemoveDlpPolicyCorruptRule(this.taskObject.Identity.ToString(), ex.Message)), ErrorCategory.ParserError, this.taskObject.Identity);
			}
			base.DataSession.Delete(this.taskObject.GetDataObject());
		}

		public static readonly string Identity = "Identity";

		private RemoveDlpPolicy taskObject;
	}
}

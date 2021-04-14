using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class RemoveDlpPolicyTemplateImpl : CmdletImplementation
	{
		public RemoveDlpPolicyTemplateImpl(RemoveDlpPolicyTemplate taskObject)
		{
			this.taskObject = taskObject;
		}

		public override void Validate()
		{
			if (this.taskObject.Identity == null)
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorInvalidDlpPolicyTemplateIdentity, RemoveDlpPolicyImpl.Identity), ErrorCategory.InvalidArgument, this.taskObject.Identity);
				return;
			}
			if (!DlpUtils.GetOutOfBoxDlpTemplates(base.DataSession, this.taskObject.Identity.ToString()).Any<ADComplianceProgram>())
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorDlpPolicyTemplateIsNotInstalled(this.taskObject.Identity.ToString())), ErrorCategory.InvalidArgument, this.taskObject.Identity);
			}
		}

		public override void ProcessRecord()
		{
			DlpUtils.DeleteOutOfBoxDlpPolicy(base.DataSession, this.taskObject.Identity.ToString());
		}

		public static readonly string Identity = "Identity";

		private RemoveDlpPolicyTemplate taskObject;
	}
}

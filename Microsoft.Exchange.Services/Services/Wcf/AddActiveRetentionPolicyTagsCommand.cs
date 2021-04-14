using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class AddActiveRetentionPolicyTagsCommand : RetentionPolicyTagsCommand<IdentityCollectionRequest, OptionsResponseBase>
	{
		public AddActiveRetentionPolicyTagsCommand(CallContext callContext, IdentityCollectionRequest request) : base(callContext, request)
		{
		}

		protected override OptionsResponseBase CreateTaskAndExecute()
		{
			this.GetAllUserAssociatedTags();
			this.MergeOldTagsWithNewTags();
			this.SetMergedUserTags();
			return this.GenerateSuccessResponse();
		}

		private void GetAllUserAssociatedTags()
		{
			this.allUserAssociatedTags = base.GetRetentionPolicyTags(true, new ElcFolderType[]
			{
				ElcFolderType.Personal
			}, false);
		}

		private void MergeOldTagsWithNewTags()
		{
			this.mergedUserTags = new Dictionary<string, RetentionPolicyTagIdParameter>(StringComparer.InvariantCultureIgnoreCase);
			foreach (PresentationRetentionPolicyTag presentationRetentionPolicyTag in this.allUserAssociatedTags)
			{
				this.mergedUserTags[presentationRetentionPolicyTag.Guid.ToString()] = new RetentionPolicyTagIdParameter(presentationRetentionPolicyTag.Guid.ToString());
			}
			foreach (Identity identity in this.request.IdentityCollection.Identities)
			{
				this.mergedUserTags[identity.RawIdentity] = new RetentionPolicyTagIdParameter(identity.RawIdentity);
			}
		}

		private void SetMergedUserTags()
		{
			base.SetRetentionPolicyTags(this.mergedUserTags.Values.ToArray<RetentionPolicyTagIdParameter>());
		}

		private OptionsResponseBase GenerateSuccessResponse()
		{
			return new OptionsResponseBase
			{
				WasSuccessful = true
			};
		}

		private List<PresentationRetentionPolicyTag> allUserAssociatedTags;

		private Dictionary<string, RetentionPolicyTagIdParameter> mergedUserTags;
	}
}

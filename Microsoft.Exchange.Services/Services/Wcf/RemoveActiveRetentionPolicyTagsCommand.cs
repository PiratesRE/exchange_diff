using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RemoveActiveRetentionPolicyTagsCommand : RetentionPolicyTagsCommand<IdentityCollectionRequest, OptionsResponseBase>
	{
		public RemoveActiveRetentionPolicyTagsCommand(CallContext callContext, IdentityCollectionRequest request) : base(callContext, request)
		{
		}

		protected override OptionsResponseBase CreateTaskAndExecute()
		{
			this.GetAllUserAssociatedTags();
			this.SubtractInputTagsFromOldTags();
			this.SetUpdateUserTags();
			return this.GenerateSuccessResponse();
		}

		private void GetAllUserAssociatedTags()
		{
			this.allUserAssociatedTags = base.GetRetentionPolicyTags(true, new ElcFolderType[]
			{
				ElcFolderType.Personal
			}, false);
		}

		private void SubtractInputTagsFromOldTags()
		{
			HashSet<string> tagIdsToBeRemoved = new HashSet<string>(from identity in this.request.IdentityCollection.Identities
			select identity.RawIdentity, StringComparer.InvariantCultureIgnoreCase);
			this.updatedUserTags = (from tag in this.allUserAssociatedTags
			where !tagIdsToBeRemoved.Contains(tag.Guid.ToString())
			select new RetentionPolicyTagIdParameter(tag.Guid.ToString())).ToArray<RetentionPolicyTagIdParameter>();
		}

		private void SetUpdateUserTags()
		{
			base.SetRetentionPolicyTags(this.updatedUserTags);
		}

		private OptionsResponseBase GenerateSuccessResponse()
		{
			return new OptionsResponseBase
			{
				WasSuccessful = true
			};
		}

		private List<PresentationRetentionPolicyTag> allUserAssociatedTags;

		private RetentionPolicyTagIdParameter[] updatedUserTags;
	}
}

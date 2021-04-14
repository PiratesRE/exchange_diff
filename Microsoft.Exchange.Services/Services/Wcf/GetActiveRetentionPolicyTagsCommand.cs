using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetActiveRetentionPolicyTagsCommand : RetentionPolicyTagsCommand<GetRetentionPolicyTagsRequest, GetRetentionPolicyTagsResponse>
	{
		public GetActiveRetentionPolicyTagsCommand(CallContext callContext, GetRetentionPolicyTagsRequest request) : base(callContext, request)
		{
		}

		protected override GetRetentionPolicyTagsResponse CreateTaskAndExecute()
		{
			this.GetAllUserAssociatedTags();
			this.GetAllOptionalUserAssociatedTags();
			this.RemoveArchiveTagsIfUserDoesNotHaveArchive();
			return this.MergeResultsAndGenerateSuccessResponse();
		}

		private void GetAllUserAssociatedTags()
		{
			this.allUserAssociatedTags = base.GetRetentionPolicyTags(true, new Microsoft.Exchange.Services.Core.Types.ElcFolderType[]
			{
				Microsoft.Exchange.Services.Core.Types.ElcFolderType.Personal,
				Microsoft.Exchange.Services.Core.Types.ElcFolderType.All
			}, false);
		}

		private void GetAllOptionalUserAssociatedTags()
		{
			this.allOptionalUserAssociatedTags = base.GetRetentionPolicyTags(true, new Microsoft.Exchange.Services.Core.Types.ElcFolderType[]
			{
				Microsoft.Exchange.Services.Core.Types.ElcFolderType.Personal
			}, true);
		}

		private void RemoveArchiveTagsIfUserDoesNotHaveArchive()
		{
			if (!base.UserHasArchive)
			{
				this.allUserAssociatedTags = (from tag in this.allUserAssociatedTags
				where tag.RetentionAction != Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType.MoveToArchive
				select tag).ToList<PresentationRetentionPolicyTag>();
			}
		}

		private GetRetentionPolicyTagsResponse MergeResultsAndGenerateSuccessResponse()
		{
			HashSet<Guid> allOptionalUserAssociatedTagIds = new HashSet<Guid>(from tag in this.allOptionalUserAssociatedTags
			select tag.RetentionId);
			return new GetRetentionPolicyTagsResponse
			{
				RetentionPolicyTagDisplayCollection = 
				{
					RetentionPolicyTags = (from tag in this.allUserAssociatedTags
					select new RetentionPolicyTagDisplay
					{
						AgeLimitForRetentionDays = ((tag.AgeLimitForRetention != null) ? new int?(tag.AgeLimitForRetention.Value.Days) : null),
						Description = tag.GetLocalizedFolderComment(new CultureInfo[]
						{
							EWSSettings.ClientCulture
						}),
						DisplayName = tag.GetLocalizedFolderName(new CultureInfo[]
						{
							EWSSettings.ClientCulture
						}),
						Identity = new Identity(tag.Id),
						OptionalTag = allOptionalUserAssociatedTagIds.Contains(tag.RetentionId),
						RetentionAction = (Microsoft.Exchange.Services.Core.Types.RetentionActionType)tag.RetentionAction,
						RetentionEnabled = tag.RetentionEnabled,
						RetentionId = tag.RetentionId,
						Type = (Microsoft.Exchange.Services.Core.Types.ElcFolderType)tag.Type
					}).ToArray<RetentionPolicyTagDisplay>()
				},
				WasSuccessful = true
			};
		}

		private List<PresentationRetentionPolicyTag> allUserAssociatedTags;

		private List<PresentationRetentionPolicyTag> allOptionalUserAssociatedTags;
	}
}

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
	internal sealed class GetAvailableRetentionPolicyTagsCommand : RetentionPolicyTagsCommand<GetRetentionPolicyTagsRequest, GetRetentionPolicyTagsResponse>
	{
		public GetAvailableRetentionPolicyTagsCommand(CallContext callContext, GetRetentionPolicyTagsRequest request) : base(callContext, request)
		{
		}

		protected override GetRetentionPolicyTagsResponse CreateTaskAndExecute()
		{
			this.GetAllTags();
			this.GetAllUserAssociatedTags();
			this.SubtractUserTagsFromAllTags();
			this.RemoveArchiveTagsIfUserDoesNotHaveArchive();
			return this.GenerateSuccessResponse();
		}

		private void GetAllTags()
		{
			this.allTags = base.GetRetentionPolicyTags(false, new Microsoft.Exchange.Services.Core.Types.ElcFolderType[]
			{
				Microsoft.Exchange.Services.Core.Types.ElcFolderType.Personal
			}, false);
		}

		private void GetAllUserAssociatedTags()
		{
			this.allUserAssociatedTags = base.GetRetentionPolicyTags(true, new Microsoft.Exchange.Services.Core.Types.ElcFolderType[]
			{
				Microsoft.Exchange.Services.Core.Types.ElcFolderType.Personal
			}, false);
		}

		private void SubtractUserTagsFromAllTags()
		{
			HashSet<Guid> allUserAssociatedTagIds = new HashSet<Guid>(from tag in this.allUserAssociatedTags
			select tag.RetentionId);
			this.allTagsAvailableForAdding = (from tag in this.allTags
			where !allUserAssociatedTagIds.Contains(tag.RetentionId)
			select tag).ToList<PresentationRetentionPolicyTag>();
		}

		private void RemoveArchiveTagsIfUserDoesNotHaveArchive()
		{
			if (!base.UserHasArchive)
			{
				this.allTagsAvailableForAdding = (from tag in this.allTagsAvailableForAdding
				where tag.RetentionAction != Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType.MoveToArchive
				select tag).ToList<PresentationRetentionPolicyTag>();
			}
		}

		private GetRetentionPolicyTagsResponse GenerateSuccessResponse()
		{
			GetRetentionPolicyTagsResponse getRetentionPolicyTagsResponse = new GetRetentionPolicyTagsResponse();
			getRetentionPolicyTagsResponse.RetentionPolicyTagDisplayCollection.RetentionPolicyTags = (from tag in this.allTagsAvailableForAdding
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
				OptionalTag = true,
				RetentionAction = (Microsoft.Exchange.Services.Core.Types.RetentionActionType)tag.RetentionAction,
				RetentionEnabled = tag.RetentionEnabled,
				RetentionId = tag.RetentionId,
				Type = (Microsoft.Exchange.Services.Core.Types.ElcFolderType)tag.Type
			}).ToArray<RetentionPolicyTagDisplay>();
			getRetentionPolicyTagsResponse.WasSuccessful = true;
			return getRetentionPolicyTagsResponse;
		}

		private List<PresentationRetentionPolicyTag> allTags;

		private List<PresentationRetentionPolicyTag> allUserAssociatedTags;

		private List<PresentationRetentionPolicyTag> allTagsAvailableForAdding;
	}
}

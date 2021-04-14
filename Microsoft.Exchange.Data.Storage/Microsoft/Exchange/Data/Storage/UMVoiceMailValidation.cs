using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UMVoiceMailValidation : SearchFolderValidation
	{
		internal UMVoiceMailValidation() : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		internal static QueryFilter GetUMVoicemailQueryFilter(DefaultFolderContext context)
		{
			return new AndFilter(new QueryFilter[]
			{
				SearchFolderValidation.GetSearchExclusionFoldersFilter(context, null, UMVoiceMailValidation.excludeFromUMSearchFolder),
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Microsoft.Voicemail.UM.CA"),
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA"),
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM"),
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Microsoft.Exchange.Voice.UM.CA"),
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Microsoft.Voicemail.UM"),
					new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Microsoft.Exchange.Voice.UM")
				})
			});
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			if (!base.EnsureIsValid(context, folder))
			{
				return false;
			}
			OutlookSearchFolder outlookSearchFolder = folder as OutlookSearchFolder;
			return outlookSearchFolder != null && this.ValidateUMVoiceMailFilter(context, outlookSearchFolder);
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			folder[InternalSchema.OutlookSearchFolderClsId] = UMVoiceMailValidation.UmVoiceMailClsId;
			folder.ClassName = "IPF.Note.Microsoft.Voicemail";
			OutlookSearchFolder outlookSearchFolder = (OutlookSearchFolder)folder;
			outlookSearchFolder.Save();
			outlookSearchFolder.ApplyContinuousSearch(UMVoiceMailValidation.CreateUMVoiceMailSearchCriteria(context));
			outlookSearchFolder.Load(null);
			outlookSearchFolder.MakeVisibleToOutlook(true);
		}

		private static SearchFolderCriteria CreateUMVoiceMailSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(UMVoiceMailValidation.GetUMVoicemailQueryFilter(context), new StoreId[]
			{
				context[DefaultFolderType.Root]
			})
			{
				DeepTraversal = true
			};
		}

		private bool ValidateUMVoiceMailFilter(DefaultFolderContext context, OutlookSearchFolder folder)
		{
			SearchFolderCriteria searchFolderCriteria = SearchFolderValidation.TryGetSearchCriteria(folder);
			if (searchFolderCriteria == null || !UMVoiceMailValidation.GetUMVoicemailQueryFilter(context).Equals(searchFolderCriteria.SearchQuery))
			{
				folder.ApplyContinuousSearch(UMVoiceMailValidation.CreateUMVoiceMailSearchCriteria(context));
				folder.MakeVisibleToOutlook(true);
			}
			return true;
		}

		private static readonly DefaultFolderType[] excludeFromUMSearchFolder = new DefaultFolderType[]
		{
			DefaultFolderType.DeletedItems,
			DefaultFolderType.JunkEmail,
			DefaultFolderType.SentItems,
			DefaultFolderType.Conflicts,
			DefaultFolderType.LocalFailures,
			DefaultFolderType.ServerFailures,
			DefaultFolderType.SyncIssues,
			DefaultFolderType.Outbox,
			DefaultFolderType.Drafts,
			DefaultFolderType.DocumentSyncIssues
		};

		private static readonly Guid UmVoiceMailClsId = new Guid("{F9D57CDE-EACF-4493-B0EC-B58EF594A3F7}");
	}
}

using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UMFaxValidation : SearchFolderValidation
	{
		internal UMFaxValidation() : base(new IValidator[]
		{
			new MatchMapiFolderType(FolderType.Search)
		})
		{
		}

		internal static QueryFilter GetUMFaxQueryFilter(DefaultFolderContext context)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Note.Microsoft.Fax.CA");
		}

		internal static SearchFolderCriteria CreateUMFaxSearchCriteria(DefaultFolderContext context)
		{
			return new SearchFolderCriteria(UMFaxValidation.GetUMFaxQueryFilter(context), new StoreId[]
			{
				context[DefaultFolderType.Inbox]
			})
			{
				DeepTraversal = true
			};
		}

		internal override bool EnsureIsValid(DefaultFolderContext context, Folder folder)
		{
			if (!base.EnsureIsValid(context, folder))
			{
				return false;
			}
			OutlookSearchFolder outlookSearchFolder = folder as OutlookSearchFolder;
			return outlookSearchFolder != null && this.ValidateUMFaxFilter(context, outlookSearchFolder);
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			folder[InternalSchema.OutlookSearchFolderClsId] = UMFaxValidation.UmFaxClsId;
			folder.ClassName = "IPF.Note.Microsoft.Fax";
			OutlookSearchFolder outlookSearchFolder = (OutlookSearchFolder)folder;
			outlookSearchFolder.Save();
			outlookSearchFolder.ApplyContinuousSearch(UMFaxValidation.CreateUMFaxSearchCriteria(context));
			outlookSearchFolder.Load(null);
			outlookSearchFolder.MakeVisibleToOutlook(true);
		}

		private bool ValidateUMFaxFilter(DefaultFolderContext context, OutlookSearchFolder folder)
		{
			SearchFolderCriteria searchFolderCriteria = SearchFolderValidation.TryGetSearchCriteria(folder);
			if (searchFolderCriteria == null || !UMFaxValidation.GetUMFaxQueryFilter(context).Equals(searchFolderCriteria.SearchQuery))
			{
				folder.ApplyContinuousSearch(UMFaxValidation.CreateUMFaxSearchCriteria(context));
			}
			return true;
		}

		private static readonly Guid UmFaxClsId = new Guid("{22875B0C-FEF8-4150-952A-5D0EEE323D99}");
	}
}

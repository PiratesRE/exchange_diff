using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class BrowsePeopleInMailbox : FindPeopleImplementation
	{
		public BrowsePeopleInMailbox(FindPeopleParameters parameters, MailboxSession mailboxSession, IdAndSession idAndSession) : base(parameters, BrowsePeopleInMailbox.AdditionalSupportedProperties, true)
		{
			ServiceCommandBase.ThrowIfNull(mailboxSession, "mailboxSession", "BrowsePeopleInMailbox::BrowsePeopleInMailbox");
			ServiceCommandBase.ThrowIfNull(idAndSession, "idAndSession", "BrowsePeopleInMailbox::BrowsePeopleInMailbox");
			this.mailboxSession = mailboxSession;
			this.idAndSession = idAndSession;
		}

		public override void Validate()
		{
			base.Validate();
			if (base.AggregationRestriction != null)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
		}

		protected override void ValidatePaging()
		{
			base.ValidatePaging();
			if (!(base.Paging is IndexedPageView) && !(base.Paging is SeekToConditionWithOffsetPageView))
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorInvalidIndexedPagingParameters);
			}
		}

		public override FindPeopleResult Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			FindPeopleResult findPeopleResult = this.ExecuteInternal();
			stopwatch.Stop();
			base.Log(FindPeopleMetadata.PersonalSearchTime, stopwatch.ElapsedMilliseconds);
			base.Log(FindPeopleMetadata.PersonalCount, findPeopleResult.PersonaList.Length);
			base.Log(FindPeopleMetadata.TotalNumberOfPeopleInView, findPeopleResult.TotalNumberOfPeopleInView);
			return findPeopleResult;
		}

		private static void SeekToCondition(IQueryResult queryResult, SeekToConditionWithOffsetPageView pageView)
		{
			ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
			QueryFilter seekFilter = serviceObjectToFilterConverter.Convert(pageView.Condition.Item);
			queryResult.SeekToCondition((SeekReference)pageView.Origin, seekFilter);
		}

		private FindPeopleResult ExecuteInternal()
		{
			int totalNumberOfPeopleInView = -1;
			int num = -1;
			PropertyListForViewRowDeterminer propertyListForViewRowDeterminer = PropertyListForViewRowDeterminer.BuildForPersonObjects(base.PersonaShape);
			List<PropertyDefinition> list = new List<PropertyDefinition>(propertyListForViewRowDeterminer.GetPropertiesToFetch());
			SortBy[] sortColumns = Microsoft.Exchange.Services.Core.Search.SortResults.ToXsoSortBy(base.SortResults);
			QueryFilter queryFilter = base.GetRestrictionFilter();
			if (queryFilter != null)
			{
				queryFilter = BasePagingType.ApplyQueryAppend(queryFilter, base.Paging);
			}
			FindPeopleResult result;
			using (Folder folder = Folder.Bind(this.mailboxSession, this.idAndSession.Id, null))
			{
				IndexedPageView indexedPageView = null;
				using (IQueryResult queryResult = ((CoreFolder)folder.CoreObject).QueryExecutor.ItemQuery(ItemQueryType.ConversationView, queryFilter, sortColumns, BrowsePeopleInMailbox.conversationIdProperty))
				{
					if (base.Paging is IndexedPageView)
					{
						indexedPageView = (IndexedPageView)base.Paging;
					}
					else
					{
						SeekToConditionWithOffsetPageView seekToConditionWithOffsetPageView = (SeekToConditionWithOffsetPageView)base.Paging;
						BrowsePeopleInMailbox.SeekToCondition(queryResult, seekToConditionWithOffsetPageView);
						num = Math.Min(queryResult.CurrentRow, Math.Max(0, queryResult.EstimatedRowCount - 1));
						indexedPageView = new IndexedPageView
						{
							Offset = Math.Max(0, num + seekToConditionWithOffsetPageView.Offset),
							MaxRows = base.Paging.MaxRows
						};
					}
					totalNumberOfPeopleInView = queryResult.EstimatedRowCount;
				}
				using (IQueryResult queryResult2 = folder.PersonItemQuery(queryFilter, null, sortColumns, list))
				{
					BasePageResult basePageResult = BasePagingType.ApplyPostQueryPaging(queryResult2, indexedPageView);
					int offset = indexedPageView.Offset;
					Stopwatch stopwatch = Stopwatch.StartNew();
					Persona[] personaList = basePageResult.View.ConvertPersonViewToPersonaObjects(list.ToArray(), propertyListForViewRowDeterminer, this.idAndSession);
					stopwatch.Stop();
					base.Log(FindPeopleMetadata.PersonalDataConversion, stopwatch.ElapsedMilliseconds);
					result = FindPeopleResult.CreateMailboxBrowseResult(personaList, totalNumberOfPeopleInView, offset, num);
				}
			}
			return result;
		}

		private static readonly PropertyDefinition[] conversationIdProperty = new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationId
		};

		private static readonly HashSet<PropertyPath> AdditionalSupportedProperties = new HashSet<PropertyPath>
		{
			PersonaSchema.DisplayNameFirstLastHeader.PropertyPath,
			PersonaSchema.DisplayNameLastFirstHeader.PropertyPath,
			PersonaSchema.ThirdPartyPhotoUrls.PropertyPath,
			PersonaSchema.Attributions.PropertyPath,
			PersonaSchema.Alias.PropertyPath,
			PersonaSchema.RelevanceScore.PropertyPath
		};

		private readonly MailboxSession mailboxSession;

		private readonly IdAndSession idAndSession;
	}
}

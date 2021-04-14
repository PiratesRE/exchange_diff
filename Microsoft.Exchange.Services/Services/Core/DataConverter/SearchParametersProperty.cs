using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class SearchParametersProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public SearchParametersProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		private void SetSearchParameters(SearchParametersType searchParameters, SearchFolder folder)
		{
			IdConverter idConverter = this.commandContext.IdConverter;
			List<StoreId> list = new List<StoreId>();
			foreach (BaseFolderId folderId in searchParameters.BaseFolderIds)
			{
				IdAndSession idAndSession = idConverter.ConvertFolderIdToIdAndSession(folderId, IdConverter.ConvertOption.IgnoreChangeKey | IdConverter.ConvertOption.NoBind);
				list.Add(idAndSession.Id);
			}
			RestrictionType restriction = searchParameters.Restriction;
			QueryFilter searchQuery = null;
			if (restriction != null && restriction.Item != null)
			{
				ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
				searchQuery = serviceObjectToFilterConverter.Convert(restriction.Item);
			}
			bool deepTraversal = searchParameters.Traversal == SearchFolderTraversalType.Deep;
			folder.ApplyContinuousSearch(new SearchFolderCriteria(searchQuery, list.ToArray())
			{
				DeepTraversal = deepTraversal
			});
		}

		private SearchParametersType CreateSearchParameters(SearchFolder folder, ToServiceObjectCommandSettings commandSettings)
		{
			SearchParametersType searchParametersType = new SearchParametersType();
			SearchFolderCriteria searchCriteria = folder.GetSearchCriteria();
			searchParametersType.Traversal = (searchCriteria.DeepTraversal ? SearchFolderTraversalType.Deep : SearchFolderTraversalType.Shallow);
			searchParametersType.TraversalSpecified = true;
			if (searchCriteria.SearchQuery != null)
			{
				QueryFilterToSearchExpressionConverter queryFilterToSearchExpressionConverter = new QueryFilterToSearchExpressionConverter();
				searchParametersType.Restriction = new RestrictionType
				{
					Item = queryFilterToSearchExpressionConverter.Convert(searchCriteria.SearchQuery)
				};
			}
			List<BaseFolderId> list = new List<BaseFolderId>();
			foreach (StoreId storeId in searchCriteria.FolderScope)
			{
				ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, commandSettings.IdAndSession, null);
				list.Add(new FolderId
				{
					Id = concatenatedId.Id,
					ChangeKey = concatenatedId.ChangeKey
				});
			}
			searchParametersType.BaseFolderIds = list.ToArray();
			return searchParametersType;
		}

		public static SearchParametersProperty CreateCommand(CommandContext commandContext)
		{
			return new SearchParametersProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			SearchFolder searchFolder = storeObject as SearchFolder;
			try
			{
				serviceObject[propertyInformation] = this.CreateSearchParameters(searchFolder, commandSettings);
			}
			catch (UnsupportedTypeForConversionException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>((long)this.GetHashCode(), "SearchParameterProperty.ToServiceObject encountered UnsupportedTypeForConversionException for searchFolder: {0}", searchFolder.DisplayName);
			}
		}

		void ISetCommand.Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			SearchFolder folder = storeObject as SearchFolder;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			SearchParametersType valueOrDefault = serviceObject.GetValueOrDefault<SearchParametersType>(propertyInformation);
			this.SetSearchParameters(valueOrDefault, folder);
		}

		void ISetUpdateCommand.SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			this.SetUpdate(setPropertyUpdate, updateCommandSettings);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			SearchFolder folder = updateCommandSettings.StoreObject as SearchFolder;
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			SearchParametersType valueOrDefault = serviceObject.GetValueOrDefault<SearchParametersType>(propertyInformation);
			this.SetSearchParameters(valueOrDefault, folder);
		}

		void IToXmlCommand.ToXml()
		{
			throw new InvalidOperationException("SearchParametersProperty.ToXml should not be called.");
		}
	}
}

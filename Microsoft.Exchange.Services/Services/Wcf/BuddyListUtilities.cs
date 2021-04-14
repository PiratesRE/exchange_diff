using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class BuddyListUtilities
	{
		internal static List<IStorePropertyBag> FetchAllPropertyBags(this IQueryResult queryResult)
		{
			if (queryResult == null)
			{
				throw new ArgumentNullException("queryResult");
			}
			List<IStorePropertyBag> list = new List<IStorePropertyBag>();
			IStorePropertyBag[] propertyBags;
			do
			{
				propertyBags = queryResult.GetPropertyBags(10000);
				list.AddRange(propertyBags);
			}
			while (propertyBags.Length > 0);
			return list;
		}

		internal static T TryGetValueOrDefault<T>(this IStorePropertyBag propertyBag, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = propertyBag.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		internal static StoreId GetSubFolderIdByClass(Folder folder, string folderClassName)
		{
			StoreId result = null;
			using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, new TextFilter(StoreObjectSchema.ContainerClass, folderClassName, MatchOptions.FullString, MatchFlags.Default), null, new PropertyDefinition[]
			{
				FolderSchema.Id
			}))
			{
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
				if (propertyBags.Length > 0)
				{
					result = (StoreId)propertyBags[0].TryGetProperty(FolderSchema.Id);
				}
			}
			return result;
		}
	}
}

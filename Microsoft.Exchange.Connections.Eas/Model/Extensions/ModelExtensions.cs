using System;
using Microsoft.Exchange.Connections.Eas.Commands;
using Microsoft.Exchange.Connections.Eas.Model.Common.WindowsLive;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSync;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Extensions
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ModelExtensions
	{
		public static EasFolderType GetEasFolderType(this Add add)
		{
			return (EasFolderType)add.Type;
		}

		public static EasFolderType GetEasFolderType(this Update update)
		{
			return (EasFolderType)update.Type;
		}

		public static WlasSystemCategoryId GetSystemCategoryId(this CategoryId categoryId)
		{
			return (WlasSystemCategoryId)categoryId.Id;
		}

		public static bool HasMoreAvailable(this Collection collection)
		{
			return collection.MoreAvailable != null;
		}

		public static FlagStatus? GetFlagStatus(this Fetch fetch)
		{
			if (fetch.Properties != null && fetch.Properties.Flag != null)
			{
				int status = fetch.Properties.Flag.Status;
				return new FlagStatus?((FlagStatus)status);
			}
			return null;
		}

		public static bool IsRead(this Fetch fetch)
		{
			if (fetch.Properties != null && fetch.Properties.Read != null)
			{
				byte value = fetch.Properties.Read.Value;
				return value == 1;
			}
			return false;
		}

		public static bool IsRead(this AddCommand addCommand)
		{
			return ModelExtensions.IsRead(addCommand.ApplicationData) ?? false;
		}

		public static bool? IsRead(this ChangeCommand changeCommand)
		{
			return ModelExtensions.IsRead(changeCommand.ApplicationData);
		}

		private static bool? IsRead(ApplicationData applicationData)
		{
			if (applicationData != null && applicationData.Read != null)
			{
				byte value = applicationData.Read.Value;
				return new bool?(value == 1);
			}
			return null;
		}
	}
}

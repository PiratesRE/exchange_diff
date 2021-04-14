using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core
{
	internal static class MasterCategoryListHelper
	{
		internal static MasterCategoryListType GetMasterCategoryListType(MailboxSession mailboxSession, CultureInfo culture)
		{
			MasterCategoryListType masterCategoryListType = new MasterCategoryListType();
			Category[] defaultCategories = MasterCategoryListHelper.CreateDefaultCategoriesList(culture);
			masterCategoryListType.DefaultList = MasterCategoryListHelper.CreateDefaultList(defaultCategories);
			masterCategoryListType.MasterList = MasterCategoryListHelper.InternalGetMasterList(mailboxSession, defaultCategories);
			return masterCategoryListType;
		}

		internal static MasterCategoryList GetMasterCategoryList(MailboxSession mailboxSession, CultureInfo culture = null)
		{
			MasterCategoryList masterCategoryList = MasterCategoryListHelper.InternalGetMasterCategoryList(mailboxSession);
			if (masterCategoryList.Count == 0)
			{
				MasterCategoryListHelper.AddDefaultCategoriesToMasterCategoryList(masterCategoryList, culture);
				masterCategoryList.Save();
			}
			return masterCategoryList;
		}

		internal static CategoryType[] GetMasterList(MasterCategoryList mcl)
		{
			if (mcl == null)
			{
				throw new ArgumentNullException("mcl");
			}
			List<CategoryType> list = new List<CategoryType>();
			foreach (Category category in mcl)
			{
				list.Add(new CategoryType(category.Name, category.Color));
			}
			return list.ToArray();
		}

		internal static CategoryType[] GetMasterList(MasterCategoryListType mcl)
		{
			if (mcl == null)
			{
				throw new ArgumentNullException("mcl");
			}
			List<CategoryType> list = new List<CategoryType>();
			if (mcl.MasterList.Length == 0)
			{
				foreach (CategoryType item in mcl.DefaultList)
				{
					list.Add(item);
				}
			}
			else
			{
				foreach (CategoryType item2 in mcl.MasterList)
				{
					list.Add(item2);
				}
			}
			return list.ToArray();
		}

		private static CategoryType[] InternalGetMasterList(MailboxSession mailboxSession, Category[] defaultCategories)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			MasterCategoryList masterCategoryList = MasterCategoryListHelper.InternalGetMasterCategoryList(mailboxSession);
			if (masterCategoryList.Count == 0)
			{
				MasterCategoryListHelper.AddDefaultCategoriesToMasterCategoryList(masterCategoryList, defaultCategories);
				masterCategoryList.Save();
			}
			return MasterCategoryListHelper.GetMasterList(masterCategoryList);
		}

		private static MasterCategoryList InternalGetMasterCategoryList(MailboxSession mailboxSession)
		{
			MasterCategoryList result = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				ExTraceGlobals.MasterCategoryListCallTracer.TraceDebug(0L, "MasterCategoryListHelper::GetMasterCategoryList - force reload of the master category list.");
				result = mailboxSession.GetMasterCategoryList(true);
			}
			catch (CorruptDataException ex)
			{
				ExTraceGlobals.MasterCategoryListCallTracer.TraceError<string, string>(0L, "MasterCategoryListHelper.GetMasterCategoryList: Corrupt MCL. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
				flag2 = true;
			}
			catch (ObjectExistedException ex2)
			{
				ExTraceGlobals.MasterCategoryListCallTracer.TraceError<string, string>(0L, "MasterCategoryListHelper.GetMasterCategoryList: ObjectExistedException. Error: {0}. Stack: {1}.", ex2.Message, ex2.StackTrace);
				flag = true;
			}
			if (flag || flag2)
			{
				result = MasterCategoryListHelper.RefetchMcl(flag2, mailboxSession);
			}
			return result;
		}

		private static MasterCategoryList RefetchMcl(bool deleteBeforeRefetch, MailboxSession mailboxSession)
		{
			if (deleteBeforeRefetch)
			{
				ExTraceGlobals.MasterCategoryListCallTracer.TraceDebug(0L, "MasterCategoryListHelper::RefetchMcl - delete master category list before reload of the master category list.");
				mailboxSession.DeleteMasterCategoryList();
			}
			ExTraceGlobals.MasterCategoryListCallTracer.TraceDebug(0L, "MasterCategoryListHelper::RefetchMcl - force reload of the master category list.");
			return mailboxSession.GetMasterCategoryList(true);
		}

		private static void AddDefaultCategoriesToMasterCategoryList(MasterCategoryList masterCategoryList, CultureInfo culture)
		{
			Category[] defaultCategories = MasterCategoryListHelper.CreateDefaultCategoriesList(culture);
			MasterCategoryListHelper.AddDefaultCategoriesToMasterCategoryList(masterCategoryList, defaultCategories);
		}

		private static void AddDefaultCategoriesToMasterCategoryList(MasterCategoryList masterCategoryList, Category[] defaultCategories)
		{
			int num = defaultCategories.Length;
			for (int i = 0; i < num; i++)
			{
				masterCategoryList.Add(defaultCategories[i]);
			}
		}

		private static Category[] CreateDefaultCategoriesList(CultureInfo culture)
		{
			return new Category[]
			{
				Category.Create(ForwardReplyUtilities.ClientsResourceManager.GetString("Red", culture), 0, true),
				Category.Create(ForwardReplyUtilities.ClientsResourceManager.GetString("Orange", culture), 1, true),
				Category.Create(ForwardReplyUtilities.ClientsResourceManager.GetString("Yellow", culture), 3, true),
				Category.Create(ForwardReplyUtilities.ClientsResourceManager.GetString("Green", culture), 4, true),
				Category.Create(ForwardReplyUtilities.ClientsResourceManager.GetString("Blue", culture), 7, true),
				Category.Create(ForwardReplyUtilities.ClientsResourceManager.GetString("Purple", culture), 8, true)
			};
		}

		private static CategoryType[] CreateDefaultList(Category[] defaultCategories)
		{
			int num = defaultCategories.Length;
			CategoryType[] array = new CategoryType[num];
			for (int i = 0; i < num; i++)
			{
				Category category = defaultCategories[i];
				array[i] = new CategoryType(category.Name, category.Color);
			}
			return array;
		}

		private const string RedCategory = "Red";

		private const string OrangeCategory = "Orange";

		private const string YellowCategory = "Yellow";

		private const string GreenCategory = "Green";

		private const string BlueCategory = "Blue";

		private const string PurpleCategory = "Purple";

		private enum CategoryColors
		{
			Red,
			Orange,
			Yellow = 3,
			Green,
			Blue = 7,
			Purple
		}
	}
}

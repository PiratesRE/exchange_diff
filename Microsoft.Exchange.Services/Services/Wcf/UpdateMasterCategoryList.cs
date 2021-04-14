using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class UpdateMasterCategoryList : ServiceCommand<UpdateMasterCategoryListResponse>
	{
		public UpdateMasterCategoryList(CallContext context, UpdateMasterCategoryListRequest request) : base(context)
		{
			this.request = request;
		}

		protected override UpdateMasterCategoryListResponse InternalExecute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			bool flag = false;
			this.masterCategoryList = MasterCategoryListHelper.GetMasterCategoryList(mailboxIdentityMailboxSession, base.CallContext.OwaCulture);
			if (this.request.RemoveCategoryList != null)
			{
				bool flag2 = this.RemoveCategories();
				if (flag2)
				{
					flag = true;
				}
			}
			if (this.request.AddCategoryList != null)
			{
				bool flag2 = this.AddCategories();
				if (flag2)
				{
					flag = true;
				}
			}
			if (this.request.ChangeCategoryColorList != null)
			{
				bool flag2 = this.ChangeCategoriesColor();
				if (flag2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.masterCategoryList.Save();
			}
			return new UpdateMasterCategoryListResponse(MasterCategoryListHelper.GetMasterList(this.masterCategoryList));
		}

		private bool AddCategories()
		{
			bool result = false;
			int num = this.request.AddCategoryList.Length;
			for (int i = 0; i < num; i++)
			{
				bool flag = this.AddCategory(this.request.AddCategoryList[i]);
				if (flag)
				{
					result = true;
				}
			}
			return result;
		}

		private bool RemoveCategories()
		{
			bool result = false;
			int num = this.request.RemoveCategoryList.Length;
			for (int i = 0; i < num; i++)
			{
				bool flag = this.DeleteCategory(this.request.RemoveCategoryList[i]);
				if (flag)
				{
					result = true;
				}
			}
			return result;
		}

		private bool ChangeCategoriesColor()
		{
			bool result = false;
			int num = this.request.ChangeCategoryColorList.Length;
			for (int i = 0; i < num; i++)
			{
				bool flag = this.ChangeCategoryColor(this.request.ChangeCategoryColorList[i]);
				if (flag)
				{
					result = true;
				}
			}
			return result;
		}

		private bool AddCategory(CategoryType categoryType)
		{
			string text = categoryType.Name;
			text = text.Trim();
			int color = categoryType.Color;
			if (!text.Contains(",") && !text.Contains(";") && !text.Contains("؛") && !text.Contains("﹔"))
			{
				text.Contains("；");
			}
			bool result = false;
			if (!this.masterCategoryList.Contains(text))
			{
				Category item = Category.Create(text, color, false);
				this.masterCategoryList.Add(item);
				result = true;
			}
			return result;
		}

		private bool ChangeCategoryColor(CategoryType categoryType)
		{
			string name = categoryType.Name;
			int color = categoryType.Color;
			bool result = false;
			if (this.masterCategoryList.Contains(name) && this.masterCategoryList[name].Color != color)
			{
				this.masterCategoryList[name].Color = color;
				result = true;
			}
			return result;
		}

		private bool DeleteCategory(string categoryName)
		{
			bool result = false;
			if (this.masterCategoryList.Contains(categoryName))
			{
				this.masterCategoryList.Remove(categoryName);
				result = true;
			}
			return result;
		}

		private bool IsValidCategoryColor(int color)
		{
			return -1 <= color && color <= 24;
		}

		private const string GuidPattern = "^(\\{{0,1}([0-9A-F]){8}-([0-9A-F]){4}-([0-9A-F]){4}-([0-9A-F]){4}-([0-9A-F]){12}\\}{0,1})$";

		private static readonly Regex guidRegEx = new Regex("^(\\{{0,1}([0-9A-F]){8}-([0-9A-F]){4}-([0-9A-F]){4}-([0-9A-F]){4}-([0-9A-F]){12}\\}{0,1})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private UpdateMasterCategoryListRequest request;

		private MasterCategoryList masterCategoryList;
	}
}

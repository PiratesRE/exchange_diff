using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class FileAsDropDownList : DropDownList
	{
		public FileAsDropDownList(string id, FileAsMapping fileAsMapping)
		{
			int num = (int)fileAsMapping;
			base..ctor(id, num.ToString(), null);
			this.fileAsMapping = fileAsMapping;
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			Utilities.HtmlEncode(ContactUtilities.GetFileAsString(this.fileAsMapping), writer);
		}

		protected override DropDownListItem[] CreateListItems()
		{
			DropDownListItem[] array = new DropDownListItem[FileAsDropDownList.fileAsMappingList.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new DropDownListItem((int)FileAsDropDownList.fileAsMappingList[i], ContactUtilities.GetFileAsString(FileAsDropDownList.fileAsMappingList[i]), false);
			}
			return array;
		}

		private static readonly FileAsMapping[] fileAsMappingList = new FileAsMapping[]
		{
			FileAsMapping.LastCommaFirst,
			FileAsMapping.FirstSpaceLast,
			FileAsMapping.Company,
			FileAsMapping.LastCommaFirstCompany,
			FileAsMapping.CompanyLastCommaFirst,
			FileAsMapping.LastFirst,
			FileAsMapping.LastFirstCompany,
			FileAsMapping.CompanyLastFirst,
			FileAsMapping.LastFirstSuffix,
			FileAsMapping.LastSpaceFirstCompany,
			FileAsMapping.CompanyLastSpaceFirst,
			FileAsMapping.LastSpaceFirst
		};

		private FileAsMapping fileAsMapping;
	}
}

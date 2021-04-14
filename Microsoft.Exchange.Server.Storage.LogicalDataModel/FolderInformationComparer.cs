using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class FolderInformationComparer : IComparer<IFolderInformation>
	{
		internal FolderInformationComparer(CompareInfo compareInfo)
		{
			this.compareInfo = compareInfo;
		}

		internal CompareInfo CompareInfo
		{
			get
			{
				return this.compareInfo;
			}
		}

		public int Compare(IFolderInformation x, IFolderInformation y)
		{
			int num = this.compareInfo.Compare(x.DisplayName, y.DisplayName, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
			if (num != 0)
			{
				return num;
			}
			return x.Fid.CompareTo(y.Fid);
		}

		private CompareInfo compareInfo;
	}
}

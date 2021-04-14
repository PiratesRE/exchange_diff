using System;
using System.DirectoryServices.Protocols;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class SearchStatsControl : DirectoryControl
	{
		public SearchStatsControl() : base(SearchStatsControl.SearchStatOid, null, false, true)
		{
		}

		private SearchStatsControl(byte[] value) : base(SearchStatsControl.SearchStatOid, value, false, true)
		{
		}

		public static SearchStatsControl FindSearchStatsControl(DirectoryResponse response)
		{
			return SearchStatsControl.FindSearchStatsControl(response.Controls);
		}

		public static SearchStatsControl FindSearchStatsControl(DirectoryRequest request)
		{
			DirectoryControl[] array = new DirectoryControl[request.Controls.Count];
			request.Controls.CopyTo(array, 0);
			return SearchStatsControl.FindSearchStatsControl(array);
		}

		private static SearchStatsControl FindSearchStatsControl(DirectoryControl[] controls)
		{
			foreach (DirectoryControl directoryControl in controls)
			{
				if (directoryControl.Type.Equals(SearchStatsControl.SearchStatOid, StringComparison.OrdinalIgnoreCase))
				{
					return new SearchStatsControl(directoryControl.GetValue());
				}
			}
			return null;
		}

		private static readonly string SearchStatOid = "1.2.840.113556.1.4.970";
	}
}

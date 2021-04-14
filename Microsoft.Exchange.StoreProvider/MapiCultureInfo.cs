using System;
using System.Globalization;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiCultureInfo : CultureInfo
	{
		private MapiCultureInfo(CultureInfo stringCulture, CultureInfo sortCulture, int codePage) : base(stringCulture.Name)
		{
			this.sortCultureInfo = sortCulture;
			this.codePage = codePage;
		}

		public static CultureInfo CreateCultureInfo(int stringLCID, int sortLCID, int codePage)
		{
			if (stringLCID == 1024)
			{
				return CultureInfo.InvariantCulture;
			}
			CultureInfo result;
			try
			{
				CultureInfo cultureFromLcid = LocaleMap.GetCultureFromLcid(stringLCID);
				CultureInfo sortCulture = null;
				if (sortLCID != 1024)
				{
					try
					{
						sortCulture = LocaleMap.GetCultureFromLcid(sortLCID);
					}
					catch (ArgumentException)
					{
						sortCulture = null;
					}
				}
				MapiCultureInfo mapiCultureInfo = new MapiCultureInfo(cultureFromLcid, sortCulture, codePage);
				if (!mapiCultureInfo.IsNeutralCulture)
				{
					result = mapiCultureInfo;
				}
				else
				{
					CultureInfo specificCulture = LocaleMap.GetSpecificCulture(mapiCultureInfo.Name);
					result = new MapiCultureInfo(specificCulture, sortCulture, codePage);
				}
			}
			catch (Exception ex)
			{
				if (!(ex is CultureNotFoundException) && !(ex is ArgumentException))
				{
					throw;
				}
				result = new MapiCultureInfo(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture, codePage);
			}
			return result;
		}

		public static CultureInfo AdjustFromClientRequest(CultureInfo clientRequested, CultureInfo currentSelection)
		{
			MapiCultureInfo mapiCultureInfo = clientRequested as MapiCultureInfo;
			if (mapiCultureInfo == null)
			{
				return currentSelection;
			}
			MapiCultureInfo mapiCultureInfo2 = currentSelection as MapiCultureInfo;
			if (mapiCultureInfo2 != null)
			{
				return currentSelection;
			}
			return new MapiCultureInfo(currentSelection, currentSelection, mapiCultureInfo.codePage);
		}

		internal static void RetrieveConnectParameters(CultureInfo cultureInfo, out int stringLCID, out int sortLCID, out int codePage)
		{
			if (cultureInfo == null)
			{
				stringLCID = 1024;
				sortLCID = 1024;
				codePage = LocaleMap.GetANSICodePage(CultureInfo.InvariantCulture);
				return;
			}
			stringLCID = LocaleMap.GetLcidFromCulture(cultureInfo);
			sortLCID = LocaleMap.GetCompareLcidFromCulture(cultureInfo);
			MapiCultureInfo mapiCultureInfo = cultureInfo as MapiCultureInfo;
			if (mapiCultureInfo != null)
			{
				codePage = mapiCultureInfo.codePage;
				return;
			}
			codePage = LocaleMap.GetANSICodePage(cultureInfo);
		}

		public override TextInfo TextInfo
		{
			get
			{
				if (this.sortCultureInfo == null)
				{
					return base.TextInfo;
				}
				return this.sortCultureInfo.TextInfo;
			}
		}

		public override CompareInfo CompareInfo
		{
			get
			{
				if (this.sortCultureInfo == null)
				{
					return base.CompareInfo;
				}
				return this.sortCultureInfo.CompareInfo;
			}
		}

		private readonly CultureInfo sortCultureInfo;

		private readonly int codePage;
	}
}

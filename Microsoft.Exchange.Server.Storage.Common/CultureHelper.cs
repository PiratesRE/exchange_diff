using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class CultureHelper
	{
		public static CultureInfo GetCultureFromLcid(int lcid)
		{
			return CultureInfo.GetCultureInfo(lcid);
		}

		public static int GetLcidFromCulture(CultureInfo culture)
		{
			return culture.LCID;
		}

		public static bool IsValidCodePage(IExecutionContext context, int codePage)
		{
			bool result;
			try
			{
				CodePageMap.GetEncoding(codePage);
				result = true;
			}
			catch (ArgumentException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				result = false;
			}
			catch (NotSupportedException exception2)
			{
				context.Diagnostics.OnExceptionCatch(exception2);
				result = false;
			}
			return result;
		}

		public static bool IsValidLcid(IExecutionContext context, int lcid)
		{
			if (lcid == CultureHelper.Invariant || lcid == 0 || lcid == 2048 || lcid == 1024 || lcid == 4096)
			{
				return true;
			}
			if (CultureHelper.frequentlyUsedCultures.Contains(lcid))
			{
				return true;
			}
			bool result;
			try
			{
				CultureHelper.GetCultureFromLcid(lcid);
				result = true;
			}
			catch (ArgumentException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				result = false;
			}
			return result;
		}

		public static CultureInfo TranslateLcid(int lcid)
		{
			if (CultureHelper.Invariant == lcid)
			{
				return CultureHelper.DefaultCultureInfo;
			}
			if (lcid == 0)
			{
				return CultureHelper.DefaultCultureInfo;
			}
			if (2048 == lcid)
			{
				return CultureHelper.DefaultCultureInfo;
			}
			if (1024 == lcid)
			{
				return CultureHelper.DefaultCultureInfo;
			}
			if (4096 == lcid)
			{
				return CultureHelper.DefaultCultureInfo;
			}
			CultureInfo cultureInfo = null;
			if (CultureHelper.frequentlyUsedCultures.TryGetValue(lcid, out cultureInfo))
			{
				return cultureInfo;
			}
			try
			{
				cultureInfo = CultureHelper.GetCultureFromLcid(lcid);
			}
			catch (ArgumentException ex)
			{
				Globals.AssertRetail(false, string.Concat(new object[]
				{
					"Unexpected culture in TranslateLCID. Lcid = [",
					lcid,
					"] exception = [",
					ex.ToString(),
					"]"
				}));
				return CultureHelper.DefaultCultureInfo;
			}
			PersistentAvlTree<int, CultureInfo> persistentAvlTree;
			PersistentAvlTree<int, CultureInfo> value;
			do
			{
				persistentAvlTree = CultureHelper.frequentlyUsedCultures;
				if (persistentAvlTree.Contains(lcid))
				{
					break;
				}
				value = persistentAvlTree.Add(lcid, cultureInfo);
			}
			while (!object.ReferenceEquals(Interlocked.CompareExchange<PersistentAvlTree<int, CultureInfo>>(ref CultureHelper.frequentlyUsedCultures, value, persistentAvlTree), persistentAvlTree));
			return cultureInfo;
		}

		public const int LocaleNeutral = 0;

		public const int SystemDefault = 2048;

		public const int UserDefault = 1024;

		public const int CustomCulture = 4096;

		public static readonly int Invariant = CultureInfo.InvariantCulture.LCID;

		public static readonly CultureInfo DefaultCultureInfo = new CultureInfo("en-US");

		private static PersistentAvlTree<int, CultureInfo> frequentlyUsedCultures = PersistentAvlTree<int, CultureInfo>.Empty;
	}
}

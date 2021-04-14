using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal static class JetSimpleQueryOperatorHelper
	{
		public static int GetCount(IJetSimpleQueryOperator jetQueryOperator)
		{
			int num = 0;
			int num2;
			bool flag = jetQueryOperator.MoveFirst(out num2);
			while (flag)
			{
				num++;
				flag = jetQueryOperator.MoveNext();
			}
			return num;
		}

		public static int GetOrdinalPosition(IJetSimpleQueryOperator jetQueryOperator, SortOrder sortOrder, StartStopKey stopKey, CompareInfo compareInfo)
		{
			int num = 0;
			int num2;
			for (bool flag = jetQueryOperator.MoveFirst(out num2); flag && !JetSimpleQueryOperatorHelper.TestKeyValue(jetQueryOperator, sortOrder, stopKey, compareInfo); flag = jetQueryOperator.MoveNext())
			{
				num++;
			}
			return num;
		}

		private static bool TestKeyValue(IJetSimpleQueryOperator jetQueryOperator, SortOrder sortOrder, StartStopKey stopKey, CompareInfo compareInfo)
		{
			for (int i = 0; i < stopKey.Count; i++)
			{
				object columnValue = jetQueryOperator.GetColumnValue(sortOrder.Columns[i]);
				int num = ValueHelper.ValuesCompare(columnValue, stopKey.Values[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
				if (!sortOrder.Ascending[i])
				{
					num = -num;
				}
				if (num < 0)
				{
					return false;
				}
				if (num > 0)
				{
					return true;
				}
			}
			return stopKey.Inclusive;
		}
	}
}

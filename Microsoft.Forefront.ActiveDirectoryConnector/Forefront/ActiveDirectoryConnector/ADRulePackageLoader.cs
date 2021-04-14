using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Internal.ManagedWPP;
using Microsoft.Mce.Interop.Api;

namespace Microsoft.Forefront.ActiveDirectoryConnector
{
	[Guid("AD89A2A7-BB0E-4716-A174-E8323A1766BF")]
	[ComVisible(true)]
	public class ADRulePackageLoader : IRulePackageLoader
	{
		public void GetRulePackages(uint ulRulePackageRequestDetailsSize, RULE_PACKAGE_REQUEST_DETAILS[] rulePackageRequestDetails)
		{
			try
			{
				ADRulePackageLoader.QueryRulePackages<RULE_PACKAGE_REQUEST_DETAILS>(rulePackageRequestDetails, (RULE_PACKAGE_REQUEST_DETAILS x) => x.RulePackageSetID, (RULE_PACKAGE_REQUEST_DETAILS x) => x.RulePackageID, new ADRulePackageLoader.UpdateAction<RULE_PACKAGE_REQUEST_DETAILS>(ADRulePackageLoader.Update));
			}
			catch (COMException)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(3, 10, this.GetHashCode(), TraceProvider.MakeStringArg(ex.Message));
				}
				throw new COMException(ex.Message, -2147220986);
			}
		}

		public void GetUpdatedRulePackageInfo(uint ulRulePackageTimestampDetailsSize, RULE_PACKAGE_TIMESTAMP_DETAILS[] rulePackageTimestampDetails)
		{
			try
			{
				ADRulePackageLoader.QueryRulePackages<RULE_PACKAGE_TIMESTAMP_DETAILS>(rulePackageTimestampDetails, (RULE_PACKAGE_TIMESTAMP_DETAILS x) => x.RulePackageSetID, (RULE_PACKAGE_TIMESTAMP_DETAILS x) => x.RulePackageID, new ADRulePackageLoader.UpdateAction<RULE_PACKAGE_TIMESTAMP_DETAILS>(ADRulePackageLoader.Update));
			}
			catch (Exception ex)
			{
				if (Tracing.tracer.Level >= 2 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_1cd3dee55f704f6905d1e53a161baad7.WPP_is(3, 11, this.GetHashCode(), TraceProvider.MakeStringArg(ex.Message));
				}
				throw new COMException(ex.Message, -2147220986);
			}
		}

		private static void QueryRulePackages<TElement>(TElement[] rulePackages, Func<TElement, string> getRulePackageSetId, Func<TElement, string> getRulePackageId, ADRulePackageLoader.UpdateAction<TElement> updateAction) where TElement : struct
		{
			Func<string, string, bool> func = null;
			var enumerable = from rulePackageGroup in rulePackages.GroupBy(getRulePackageSetId)
			select new
			{
				Id = rulePackageGroup.Key,
				PackageIds = from rulePackage in rulePackageGroup
				select getRulePackageId(rulePackage)
			};
			foreach (var <>f__AnonymousType in enumerable)
			{
				foreach (ADHelpers.ADRulePackageInfo adrulePackageInfo in ADHelpers.GetClassificationRulePackages(<>f__AnonymousType.Id, <>f__AnonymousType.PackageIds))
				{
					for (int i = 0; i < rulePackages.Length; i++)
					{
						if (func == null)
						{
							func = ((string lhs, string rhs) => 0 == string.Compare(lhs, rhs, StringComparison.OrdinalIgnoreCase));
						}
						Func<string, string, bool> func2 = func;
						if (func2(getRulePackageSetId(rulePackages[i]), adrulePackageInfo.RulePackageSetId) && func2(getRulePackageId(rulePackages[i]), adrulePackageInfo.RulePackageId))
						{
							updateAction(ref rulePackages[i], adrulePackageInfo);
							break;
						}
					}
				}
			}
		}

		private static void Update(ref RULE_PACKAGE_REQUEST_DETAILS rulePackageRequestDetails, ADHelpers.ADRulePackageInfo adRulePackageInfo)
		{
			rulePackageRequestDetails.RulePackage = adRulePackageInfo.Xml;
			rulePackageRequestDetails.LastUpdatedTime = adRulePackageInfo.ModifiedDate.ToString("u", CultureInfo.InvariantCulture);
		}

		private static void Update(ref RULE_PACKAGE_TIMESTAMP_DETAILS rulePackageTimestampDetails, ADHelpers.ADRulePackageInfo adRulePackageInfo)
		{
			DateTime t = DateTime.ParseExact(rulePackageTimestampDetails.LastUpdatedTime, "u", CultureInfo.InvariantCulture);
			rulePackageTimestampDetails.RulePackageChanged = (adRulePackageInfo.ModifiedDate > t);
		}

		private delegate void UpdateAction<TElement>(ref TElement element, ADHelpers.ADRulePackageInfo adRulePackageInfo) where TElement : struct;
	}
}

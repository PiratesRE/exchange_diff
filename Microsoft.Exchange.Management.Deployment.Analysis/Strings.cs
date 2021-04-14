using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(967409192U, "CanceledMessage");
			Strings.stringIDs.Add(260127048U, "CannotReturnNullForResult");
			Strings.stringIDs.Add(1163620802U, "CanOnlyHaveOneFeatureOfEachType");
			Strings.stringIDs.Add(3944784639U, "CannotGetStartTimeBeforeMemberStart");
			Strings.stringIDs.Add(718315535U, "CannotGetMembersBeforeDiscovery");
			Strings.stringIDs.Add(1536053379U, "CannotGetStopTimeBeforeCompletion");
			Strings.stringIDs.Add(4284209231U, "CannotGetMemberNameBeforeDiscovery");
			Strings.stringIDs.Add(1141647571U, "AccessedFailedResult");
			Strings.stringIDs.Add(4164286807U, "EmptyResults");
			Strings.stringIDs.Add(59961664U, "CannotGetCancellationExceptionWithoutCancellation");
			Strings.stringIDs.Add(2424943235U, "CannotAddNullFeature");
			Strings.stringIDs.Add(1813193876U, "FilteredResult");
			Strings.stringIDs.Add(2464627714U, "NullResult");
			Strings.stringIDs.Add(1032838303U, "CannotGetStopTimeBeforeMemberCompletion");
			Strings.stringIDs.Add(337673388U, "CannotModifyReadOnlyProperty");
			Strings.stringIDs.Add(556668270U, "CriticalMessage");
			Strings.stringIDs.Add(2543736554U, "FailedResult");
			Strings.stringIDs.Add(3833953363U, "CannotGetStartTimeBeforeStart");
			Strings.stringIDs.Add(3425349017U, "AnalysisMustBeCompleteToCreateConclusionSet");
			Strings.stringIDs.Add(1376602368U, "AccessedValueWhenMultipleResults");
		}

		public static LocalizedString CanceledMessage
		{
			get
			{
				return new LocalizedString("CanceledMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotReturnNullForResult
		{
			get
			{
				return new LocalizedString("CannotReturnNullForResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanOnlyHaveOneFeatureOfEachType
		{
			get
			{
				return new LocalizedString("CanOnlyHaveOneFeatureOfEachType", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetStartTimeBeforeMemberStart
		{
			get
			{
				return new LocalizedString("CannotGetStartTimeBeforeMemberStart", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ResultAncestorNotFound(string ancestorName)
		{
			return new LocalizedString("ResultAncestorNotFound", Strings.ResourceManager, new object[]
			{
				ancestorName
			});
		}

		public static LocalizedString CannotGetMembersBeforeDiscovery
		{
			get
			{
				return new LocalizedString("CannotGetMembersBeforeDiscovery", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetStopTimeBeforeCompletion
		{
			get
			{
				return new LocalizedString("CannotGetStopTimeBeforeCompletion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetMemberNameBeforeDiscovery
		{
			get
			{
				return new LocalizedString("CannotGetMemberNameBeforeDiscovery", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessedFailedResult
		{
			get
			{
				return new LocalizedString("AccessedFailedResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyResults
		{
			get
			{
				return new LocalizedString("EmptyResults", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetCancellationExceptionWithoutCancellation
		{
			get
			{
				return new LocalizedString("CannotGetCancellationExceptionWithoutCancellation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotAddNullFeature
		{
			get
			{
				return new LocalizedString("CannotAddNullFeature", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FilteredResult
		{
			get
			{
				return new LocalizedString("FilteredResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullResult
		{
			get
			{
				return new LocalizedString("NullResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FeatureMissing(string featureType)
		{
			return new LocalizedString("FeatureMissing", Strings.ResourceManager, new object[]
			{
				featureType
			});
		}

		public static LocalizedString CannotGetStopTimeBeforeMemberCompletion
		{
			get
			{
				return new LocalizedString("CannotGetStopTimeBeforeMemberCompletion", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotModifyReadOnlyProperty
		{
			get
			{
				return new LocalizedString("CannotModifyReadOnlyProperty", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CriticalMessage
		{
			get
			{
				return new LocalizedString("CriticalMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedResult
		{
			get
			{
				return new LocalizedString("FailedResult", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetStartTimeBeforeStart
		{
			get
			{
				return new LocalizedString("CannotGetStartTimeBeforeStart", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AnalysisMustBeCompleteToCreateConclusionSet
		{
			get
			{
				return new LocalizedString("AnalysisMustBeCompleteToCreateConclusionSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessedValueWhenMultipleResults
		{
			get
			{
				return new LocalizedString("AccessedValueWhenMultipleResults", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(20);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.Deployment.Analysis.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			CanceledMessage = 967409192U,
			CannotReturnNullForResult = 260127048U,
			CanOnlyHaveOneFeatureOfEachType = 1163620802U,
			CannotGetStartTimeBeforeMemberStart = 3944784639U,
			CannotGetMembersBeforeDiscovery = 718315535U,
			CannotGetStopTimeBeforeCompletion = 1536053379U,
			CannotGetMemberNameBeforeDiscovery = 4284209231U,
			AccessedFailedResult = 1141647571U,
			EmptyResults = 4164286807U,
			CannotGetCancellationExceptionWithoutCancellation = 59961664U,
			CannotAddNullFeature = 2424943235U,
			FilteredResult = 1813193876U,
			NullResult = 2464627714U,
			CannotGetStopTimeBeforeMemberCompletion = 1032838303U,
			CannotModifyReadOnlyProperty = 337673388U,
			CriticalMessage = 556668270U,
			FailedResult = 2543736554U,
			CannotGetStartTimeBeforeStart = 3833953363U,
			AnalysisMustBeCompleteToCreateConclusionSet = 3425349017U,
			AccessedValueWhenMultipleResults = 1376602368U
		}

		private enum ParamIDs
		{
			ResultAncestorNotFound,
			FeatureMissing
		}
	}
}

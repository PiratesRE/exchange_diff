using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PAAUtils
	{
		internal static LatencyDetectionContextFactory GetCallAnsweringDataFactory
		{
			get
			{
				if (PAAUtils.getCallAnsweringDataFactory == null)
				{
					lock (PAAUtils.syncObj)
					{
						if (PAAUtils.getCallAnsweringDataFactory == null)
						{
							PAAUtils.getCallAnsweringDataFactory = PAAUtils.CreateLatencyDetectionFactory("UM.GetCallAnsweringData");
						}
					}
				}
				return PAAUtils.getCallAnsweringDataFactory;
			}
		}

		internal static LatencyDetectionContextFactory GetAutoAttendantsFromStoreFactory
		{
			get
			{
				if (PAAUtils.getAutoAttendantsFactory == null)
				{
					lock (PAAUtils.syncObj)
					{
						if (PAAUtils.getAutoAttendantsFactory == null)
						{
							PAAUtils.getAutoAttendantsFactory = PAAUtils.CreateLatencyDetectionFactory("PAA.GetAutoAttendants");
						}
					}
				}
				return PAAUtils.getAutoAttendantsFactory;
			}
		}

		internal static LatencyDetectionContextFactory PAAEvaluationFactory
		{
			get
			{
				if (PAAUtils.paaEvaluationFactory == null)
				{
					lock (PAAUtils.syncObj)
					{
						if (PAAUtils.paaEvaluationFactory == null)
						{
							PAAUtils.paaEvaluationFactory = PAAUtils.CreateLatencyDetectionFactory("PAA.Evaluate");
						}
					}
				}
				return PAAUtils.paaEvaluationFactory;
			}
		}

		internal static LatencyDetectionContextFactory BuildContactCacheFactory
		{
			get
			{
				if (PAAUtils.buildContactCacheFactory == null)
				{
					lock (PAAUtils.syncObj)
					{
						if (PAAUtils.buildContactCacheFactory == null)
						{
							PAAUtils.buildContactCacheFactory = PAAUtils.CreateLatencyDetectionFactory("PAA.PersonalContactCache.BuildCache");
						}
					}
				}
				return PAAUtils.buildContactCacheFactory;
			}
		}

		internal static LatencyDetectionContextFactory ResolvePersonalContactsFactory
		{
			get
			{
				if (PAAUtils.resolvePersonalContactsFactory == null)
				{
					lock (PAAUtils.syncObj)
					{
						if (PAAUtils.resolvePersonalContactsFactory == null)
						{
							PAAUtils.resolvePersonalContactsFactory = PAAUtils.CreateLatencyDetectionFactory("PAA.UserDataLoader.ResolvePersonalContacts");
						}
					}
				}
				return PAAUtils.resolvePersonalContactsFactory;
			}
		}

		internal static LatencyDetectionContextFactory GetFreeBusyInfoFactory
		{
			get
			{
				if (PAAUtils.getFreeBusyInfoFactory == null)
				{
					lock (PAAUtils.syncObj)
					{
						if (PAAUtils.getFreeBusyInfoFactory == null)
						{
							PAAUtils.getFreeBusyInfoFactory = PAAUtils.CreateLatencyDetectionFactory("PAA.UserDataLoader.GetFreeBusyInformation");
						}
					}
				}
				return PAAUtils.getFreeBusyInfoFactory;
			}
		}

		internal static bool IsCompatible(Version version)
		{
			bool flag = version.Major == PAAConstants.CurrentVersion.Major && version.Minor == PAAConstants.CurrentVersion.Minor && version.Build == PAAConstants.CurrentVersion.Build && version.Revision == PAAConstants.CurrentVersion.Revision;
			CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, null, "PAAUtils::IsCompatible(Version: {0} CurrentVersion: {1}) returning {2}", new object[]
			{
				version.ToString(),
				PAAConstants.CurrentVersion.ToString(),
				flag
			});
			return flag;
		}

		private static LatencyDetectionContextFactory CreateLatencyDetectionFactory(string locationIdentity)
		{
			return LatencyDetectionContextFactory.CreateFactory(locationIdentity);
		}

		private static object syncObj = new object();

		private static LatencyDetectionContextFactory getAutoAttendantsFactory = null;

		private static LatencyDetectionContextFactory paaEvaluationFactory = null;

		private static LatencyDetectionContextFactory buildContactCacheFactory = null;

		private static LatencyDetectionContextFactory resolvePersonalContactsFactory = null;

		private static LatencyDetectionContextFactory getFreeBusyInfoFactory = null;

		private static LatencyDetectionContextFactory getCallAnsweringDataFactory = null;
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal static class AnnotationStrings
	{
		static AnnotationStrings()
		{
			AnnotationStrings.stringIDs.Add(2599184111U, "Title_CsActiveUserDaily");
			AnnotationStrings.stringIDs.Add(916893256U, "Title_TrafficReport");
			AnnotationStrings.stringIDs.Add(2457455276U, "Title_ExternalActivityWeekly");
			AnnotationStrings.stringIDs.Add(1460033953U, "Title_StaleMailboxDetail");
			AnnotationStrings.stringIDs.Add(919006352U, "Title_ConnectionByClientTypeDetailYearly");
			AnnotationStrings.stringIDs.Add(2728899750U, "Title_MailboxUsage");
			AnnotationStrings.stringIDs.Add(4227251602U, "Title_SPOSkyDriveProDeployedMonthly");
			AnnotationStrings.stringIDs.Add(3697180487U, "Title_MailboxUsageDetail");
			AnnotationStrings.stringIDs.Add(2447679595U, "Title_CsActiveUserMonthly");
			AnnotationStrings.stringIDs.Add(1719022488U, "Title_CsAVConferenceTimeDaily");
			AnnotationStrings.stringIDs.Add(4092464326U, "Title_CsP2PAVTimeWeekly");
			AnnotationStrings.stringIDs.Add(548826438U, "Title_ClientOSReport");
			AnnotationStrings.stringIDs.Add(1607544303U, "Title_SPOActiveUserWeekly");
			AnnotationStrings.stringIDs.Add(4116169389U, "Minutes");
			AnnotationStrings.stringIDs.Add(2007411791U, "Title_CsClientDeviceMonthly");
			AnnotationStrings.stringIDs.Add(1173626453U, "Title_ClientBrowserDetailReport");
			AnnotationStrings.stringIDs.Add(4086740914U, "Title_SpamMessageDetail");
			AnnotationStrings.stringIDs.Add(873044688U, "Title_ConnectionByClientTypeDaily");
			AnnotationStrings.stringIDs.Add(2102105265U, "Title_SummaryReport");
			AnnotationStrings.stringIDs.Add(3778010946U, "Title_ExternalActivityByUserWeekly");
			AnnotationStrings.stringIDs.Add(4124840583U, "Title_ConnectionByClientTypeDetailWeekly");
			AnnotationStrings.stringIDs.Add(3104710592U, "Title_MessageTraceDetail");
			AnnotationStrings.stringIDs.Add(3127208035U, "Title_SPOTenantStorageMetricWeekly");
			AnnotationStrings.stringIDs.Add(3560391903U, "NA");
			AnnotationStrings.stringIDs.Add(4116066136U, "SizeMB");
			AnnotationStrings.stringIDs.Add(3142174497U, "Title_ConnectionByClientTypeYearly");
			AnnotationStrings.stringIDs.Add(2449782919U, "Title_SPOSkyDriveProStorageWeekly");
			AnnotationStrings.stringIDs.Add(4117984753U, "Title_MxRecordReport");
			AnnotationStrings.stringIDs.Add(3288860276U, "Title_FilterValueList");
			AnnotationStrings.stringIDs.Add(3807172670U, "Title_SPOSkyDriveProDeployedWeekly");
			AnnotationStrings.stringIDs.Add(3489420090U, "Title_ScorecardClientDevice");
			AnnotationStrings.stringIDs.Add(2483434450U, "Title_CsP2PSessionDaily");
			AnnotationStrings.stringIDs.Add(1637029953U, "Title_MessageTrace");
			AnnotationStrings.stringIDs.Add(2188089486U, "Title_CsConferenceDaily");
			AnnotationStrings.stringIDs.Add(3696480905U, "Title_SPOODBFileActivity");
			AnnotationStrings.stringIDs.Add(1466734612U, "Title_ScorecardMetrics");
			AnnotationStrings.stringIDs.Add(2474312838U, "Title_GroupActivityMonthly");
			AnnotationStrings.stringIDs.Add(3490604815U, "Title_SPOTenantStorageMetricMonthly");
			AnnotationStrings.stringIDs.Add(3438387218U, "Title_StaleMailbox");
			AnnotationStrings.stringIDs.Add(2626245559U, "Title_CsActiveUserWeekly");
			AnnotationStrings.stringIDs.Add(2699807741U, "Title_DeviceDashboardSummaryReport");
			AnnotationStrings.stringIDs.Add(3798591559U, "Title_SPOActiveUserDaily");
			AnnotationStrings.stringIDs.Add(1566437488U, "Title_ClientBrowserReport");
			AnnotationStrings.stringIDs.Add(1406692960U, "Title_MailboxActivityYearly");
			AnnotationStrings.stringIDs.Add(1816522614U, "Title_GroupActivityDaily");
			AnnotationStrings.stringIDs.Add(369410763U, "Title_ScorecardClientOutlook");
			AnnotationStrings.stringIDs.Add(444818027U, "Title_MailboxActivityMonthly");
			AnnotationStrings.stringIDs.Add(2167864011U, "Title_DeviceDetailsReport");
			AnnotationStrings.stringIDs.Add(2938655794U, "Title_CsP2PAVTimeMonthly");
			AnnotationStrings.stringIDs.Add(31883008U, "Title_CsActiveUserYearly");
			AnnotationStrings.stringIDs.Add(1167376766U, "Title_OutboundConnectorReport");
			AnnotationStrings.stringIDs.Add(4159483935U, "Title_MailboxActivityDaily");
			AnnotationStrings.stringIDs.Add(4136517238U, "Title_CsConferenceMonthly");
			AnnotationStrings.stringIDs.Add(4055793796U, "CountOfMailboxes");
			AnnotationStrings.stringIDs.Add(2411970335U, "Title_TopTrafficReport");
			AnnotationStrings.stringIDs.Add(1441569752U, "Title_ConnectionByClientTypeWeekly");
			AnnotationStrings.stringIDs.Add(2900754885U, "Title_SPOTeamSiteStorageMonthly");
			AnnotationStrings.stringIDs.Add(3750041507U, "Title_SPOSkyDriveProStorageMonthly");
			AnnotationStrings.stringIDs.Add(3134462204U, "Title_CsAVConferenceTimeMonthly");
			AnnotationStrings.stringIDs.Add(4032142024U, "Title_ExternalActivityMonthly");
			AnnotationStrings.stringIDs.Add(664019258U, "Title_MalwareMessageDetail");
			AnnotationStrings.stringIDs.Add(1554779067U, "Count");
			AnnotationStrings.stringIDs.Add(2391708396U, "Title_ConnectionByClientTypeMonthly");
			AnnotationStrings.stringIDs.Add(3789075813U, "Title_ClientOSDetailReport");
			AnnotationStrings.stringIDs.Add(1491838153U, "Title_PartnerClientExpiringSubscription");
			AnnotationStrings.stringIDs.Add(4073762434U, "Title_ServiceDeliveryReport");
			AnnotationStrings.stringIDs.Add(1120340038U, "Title_LicenseVsUsageSummary");
			AnnotationStrings.stringIDs.Add(2269495169U, "Title_DlpMessageDetail");
			AnnotationStrings.stringIDs.Add(2874211435U, "Title_SPOTenantStorageMetricDaily");
			AnnotationStrings.stringIDs.Add(3703323930U, "Title_PartnerCustomerUser");
			AnnotationStrings.stringIDs.Add(2188740169U, "Title_ExternalActivityByDomainDaily");
			AnnotationStrings.stringIDs.Add(3901694418U, "Title_PolicyTrafficReport");
			AnnotationStrings.stringIDs.Add(95966325U, "Title_PolicyMessageDetail");
			AnnotationStrings.stringIDs.Add(2331474543U, "Title_MessageDetailReport");
			AnnotationStrings.stringIDs.Add(3348137534U, "Title_ExternalActivityByUserDaily");
			AnnotationStrings.stringIDs.Add(878571178U, "Title_ExternalActivitySummaryWeekly");
			AnnotationStrings.stringIDs.Add(1703063467U, "Title_SPOActiveUserMonthly");
			AnnotationStrings.stringIDs.Add(2654343028U, "Title_ExternalActivityDaily");
			AnnotationStrings.stringIDs.Add(203909934U, "Title_ExternalActivitySummaryMonthly");
			AnnotationStrings.stringIDs.Add(1942412854U, "Title_SPOODBUserStatistics");
			AnnotationStrings.stringIDs.Add(2115947028U, "CountOfGroups");
			AnnotationStrings.stringIDs.Add(2328219770U, "Date");
			AnnotationStrings.stringIDs.Add(3914274640U, "Title_SPOTeamSiteDeployedMonthly");
			AnnotationStrings.stringIDs.Add(1964655722U, "Title_GroupActivityWeekly");
			AnnotationStrings.stringIDs.Add(3192449161U, "Title_ExternalActivityByDomainMonthly");
			AnnotationStrings.stringIDs.Add(2465396958U, "Title_ExternalActivityByUserMonthly");
			AnnotationStrings.stringIDs.Add(719677643U, "Title_ConnectionByClientTypeDetailMonthly");
			AnnotationStrings.stringIDs.Add(4178626464U, "Title_CsAVConferenceTimeWeekly");
			AnnotationStrings.stringIDs.Add(330021932U, "Title_SPOTeamSiteDeployedWeekly");
			AnnotationStrings.stringIDs.Add(1681059041U, "Title_SPOTeamSiteStorageWeekly");
			AnnotationStrings.stringIDs.Add(1464124746U, "Title_CsConferenceWeekly");
			AnnotationStrings.stringIDs.Add(849685939U, "Title_GroupActivityYearly");
			AnnotationStrings.stringIDs.Add(4557591U, "Title_MailboxActivityWeekly");
			AnnotationStrings.stringIDs.Add(3439325998U, "Title_CsP2PAVTimeDaily");
			AnnotationStrings.stringIDs.Add(1215094758U, "ActiveUserCount");
			AnnotationStrings.stringIDs.Add(3177100050U, "Title_ExternalActivitySummaryDaily");
			AnnotationStrings.stringIDs.Add(4215466264U, "CountOfAccounts");
			AnnotationStrings.stringIDs.Add(3807350126U, "Title_CsP2PSessionMonthly");
			AnnotationStrings.stringIDs.Add(1382414506U, "Title_CsP2PSessionWeekly");
			AnnotationStrings.stringIDs.Add(4205497791U, "Title_ConnectionByClientTypeDetailDaily");
			AnnotationStrings.stringIDs.Add(386864173U, "Title_ExternalActivityByDomainWeekly");
			AnnotationStrings.stringIDs.Add(3075209078U, "Title_ScorecardClientOS");
		}

		public static LocalizedString Title_CsActiveUserDaily
		{
			get
			{
				return new LocalizedString("Title_CsActiveUserDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_TrafficReport
		{
			get
			{
				return new LocalizedString("Title_TrafficReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityWeekly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_StaleMailboxDetail
		{
			get
			{
				return new LocalizedString("Title_StaleMailboxDetail", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeDetailYearly
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeDetailYearly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MailboxUsage
		{
			get
			{
				return new LocalizedString("Title_MailboxUsage", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOSkyDriveProDeployedMonthly
		{
			get
			{
				return new LocalizedString("Title_SPOSkyDriveProDeployedMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MailboxUsageDetail
		{
			get
			{
				return new LocalizedString("Title_MailboxUsageDetail", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsActiveUserMonthly
		{
			get
			{
				return new LocalizedString("Title_CsActiveUserMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsAVConferenceTimeDaily
		{
			get
			{
				return new LocalizedString("Title_CsAVConferenceTimeDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsP2PAVTimeWeekly
		{
			get
			{
				return new LocalizedString("Title_CsP2PAVTimeWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ClientOSReport
		{
			get
			{
				return new LocalizedString("Title_ClientOSReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOActiveUserWeekly
		{
			get
			{
				return new LocalizedString("Title_SPOActiveUserWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Minutes
		{
			get
			{
				return new LocalizedString("Minutes", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsClientDeviceMonthly
		{
			get
			{
				return new LocalizedString("Title_CsClientDeviceMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ClientBrowserDetailReport
		{
			get
			{
				return new LocalizedString("Title_ClientBrowserDetailReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SpamMessageDetail
		{
			get
			{
				return new LocalizedString("Title_SpamMessageDetail", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeDaily
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SummaryReport
		{
			get
			{
				return new LocalizedString("Title_SummaryReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityByUserWeekly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityByUserWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeDetailWeekly
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeDetailWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MessageTraceDetail
		{
			get
			{
				return new LocalizedString("Title_MessageTraceDetail", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOTenantStorageMetricWeekly
		{
			get
			{
				return new LocalizedString("Title_SPOTenantStorageMetricWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NA
		{
			get
			{
				return new LocalizedString("NA", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SizeMB
		{
			get
			{
				return new LocalizedString("SizeMB", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeYearly
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeYearly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOSkyDriveProStorageWeekly
		{
			get
			{
				return new LocalizedString("Title_SPOSkyDriveProStorageWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MxRecordReport
		{
			get
			{
				return new LocalizedString("Title_MxRecordReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_FilterValueList
		{
			get
			{
				return new LocalizedString("Title_FilterValueList", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOSkyDriveProDeployedWeekly
		{
			get
			{
				return new LocalizedString("Title_SPOSkyDriveProDeployedWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ScorecardClientDevice
		{
			get
			{
				return new LocalizedString("Title_ScorecardClientDevice", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsP2PSessionDaily
		{
			get
			{
				return new LocalizedString("Title_CsP2PSessionDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MessageTrace
		{
			get
			{
				return new LocalizedString("Title_MessageTrace", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsConferenceDaily
		{
			get
			{
				return new LocalizedString("Title_CsConferenceDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOODBFileActivity
		{
			get
			{
				return new LocalizedString("Title_SPOODBFileActivity", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ScorecardMetrics
		{
			get
			{
				return new LocalizedString("Title_ScorecardMetrics", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_GroupActivityMonthly
		{
			get
			{
				return new LocalizedString("Title_GroupActivityMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOTenantStorageMetricMonthly
		{
			get
			{
				return new LocalizedString("Title_SPOTenantStorageMetricMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_StaleMailbox
		{
			get
			{
				return new LocalizedString("Title_StaleMailbox", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsActiveUserWeekly
		{
			get
			{
				return new LocalizedString("Title_CsActiveUserWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_DeviceDashboardSummaryReport
		{
			get
			{
				return new LocalizedString("Title_DeviceDashboardSummaryReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOActiveUserDaily
		{
			get
			{
				return new LocalizedString("Title_SPOActiveUserDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ClientBrowserReport
		{
			get
			{
				return new LocalizedString("Title_ClientBrowserReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MailboxActivityYearly
		{
			get
			{
				return new LocalizedString("Title_MailboxActivityYearly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_GroupActivityDaily
		{
			get
			{
				return new LocalizedString("Title_GroupActivityDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ScorecardClientOutlook
		{
			get
			{
				return new LocalizedString("Title_ScorecardClientOutlook", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MailboxActivityMonthly
		{
			get
			{
				return new LocalizedString("Title_MailboxActivityMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_DeviceDetailsReport
		{
			get
			{
				return new LocalizedString("Title_DeviceDetailsReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsP2PAVTimeMonthly
		{
			get
			{
				return new LocalizedString("Title_CsP2PAVTimeMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsActiveUserYearly
		{
			get
			{
				return new LocalizedString("Title_CsActiveUserYearly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_OutboundConnectorReport
		{
			get
			{
				return new LocalizedString("Title_OutboundConnectorReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MailboxActivityDaily
		{
			get
			{
				return new LocalizedString("Title_MailboxActivityDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsConferenceMonthly
		{
			get
			{
				return new LocalizedString("Title_CsConferenceMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CountOfMailboxes
		{
			get
			{
				return new LocalizedString("CountOfMailboxes", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_TopTrafficReport
		{
			get
			{
				return new LocalizedString("Title_TopTrafficReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeWeekly
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOTeamSiteStorageMonthly
		{
			get
			{
				return new LocalizedString("Title_SPOTeamSiteStorageMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOSkyDriveProStorageMonthly
		{
			get
			{
				return new LocalizedString("Title_SPOSkyDriveProStorageMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsAVConferenceTimeMonthly
		{
			get
			{
				return new LocalizedString("Title_CsAVConferenceTimeMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityMonthly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MalwareMessageDetail
		{
			get
			{
				return new LocalizedString("Title_MalwareMessageDetail", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Count
		{
			get
			{
				return new LocalizedString("Count", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeMonthly
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ClientOSDetailReport
		{
			get
			{
				return new LocalizedString("Title_ClientOSDetailReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_PartnerClientExpiringSubscription
		{
			get
			{
				return new LocalizedString("Title_PartnerClientExpiringSubscription", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ServiceDeliveryReport
		{
			get
			{
				return new LocalizedString("Title_ServiceDeliveryReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_LicenseVsUsageSummary
		{
			get
			{
				return new LocalizedString("Title_LicenseVsUsageSummary", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_DlpMessageDetail
		{
			get
			{
				return new LocalizedString("Title_DlpMessageDetail", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOTenantStorageMetricDaily
		{
			get
			{
				return new LocalizedString("Title_SPOTenantStorageMetricDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_PartnerCustomerUser
		{
			get
			{
				return new LocalizedString("Title_PartnerCustomerUser", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityByDomainDaily
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityByDomainDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_PolicyTrafficReport
		{
			get
			{
				return new LocalizedString("Title_PolicyTrafficReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_PolicyMessageDetail
		{
			get
			{
				return new LocalizedString("Title_PolicyMessageDetail", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MessageDetailReport
		{
			get
			{
				return new LocalizedString("Title_MessageDetailReport", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityByUserDaily
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityByUserDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivitySummaryWeekly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivitySummaryWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOActiveUserMonthly
		{
			get
			{
				return new LocalizedString("Title_SPOActiveUserMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityDaily
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivitySummaryMonthly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivitySummaryMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOODBUserStatistics
		{
			get
			{
				return new LocalizedString("Title_SPOODBUserStatistics", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CountOfGroups
		{
			get
			{
				return new LocalizedString("CountOfGroups", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Date
		{
			get
			{
				return new LocalizedString("Date", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOTeamSiteDeployedMonthly
		{
			get
			{
				return new LocalizedString("Title_SPOTeamSiteDeployedMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_GroupActivityWeekly
		{
			get
			{
				return new LocalizedString("Title_GroupActivityWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityByDomainMonthly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityByDomainMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityByUserMonthly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityByUserMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeDetailMonthly
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeDetailMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsAVConferenceTimeWeekly
		{
			get
			{
				return new LocalizedString("Title_CsAVConferenceTimeWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOTeamSiteDeployedWeekly
		{
			get
			{
				return new LocalizedString("Title_SPOTeamSiteDeployedWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_SPOTeamSiteStorageWeekly
		{
			get
			{
				return new LocalizedString("Title_SPOTeamSiteStorageWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsConferenceWeekly
		{
			get
			{
				return new LocalizedString("Title_CsConferenceWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_GroupActivityYearly
		{
			get
			{
				return new LocalizedString("Title_GroupActivityYearly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_MailboxActivityWeekly
		{
			get
			{
				return new LocalizedString("Title_MailboxActivityWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsP2PAVTimeDaily
		{
			get
			{
				return new LocalizedString("Title_CsP2PAVTimeDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveUserCount
		{
			get
			{
				return new LocalizedString("ActiveUserCount", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivitySummaryDaily
		{
			get
			{
				return new LocalizedString("Title_ExternalActivitySummaryDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CountOfAccounts
		{
			get
			{
				return new LocalizedString("CountOfAccounts", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsP2PSessionMonthly
		{
			get
			{
				return new LocalizedString("Title_CsP2PSessionMonthly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_CsP2PSessionWeekly
		{
			get
			{
				return new LocalizedString("Title_CsP2PSessionWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ConnectionByClientTypeDetailDaily
		{
			get
			{
				return new LocalizedString("Title_ConnectionByClientTypeDetailDaily", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ExternalActivityByDomainWeekly
		{
			get
			{
				return new LocalizedString("Title_ExternalActivityByDomainWeekly", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Title_ScorecardClientOS
		{
			get
			{
				return new LocalizedString("Title_ScorecardClientOS", AnnotationStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(AnnotationStrings.IDs key)
		{
			return new LocalizedString(AnnotationStrings.stringIDs[(uint)key], AnnotationStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(102);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Management.ReportingWebService.AnnotationStrings", typeof(AnnotationStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			Title_CsActiveUserDaily = 2599184111U,
			Title_TrafficReport = 916893256U,
			Title_ExternalActivityWeekly = 2457455276U,
			Title_StaleMailboxDetail = 1460033953U,
			Title_ConnectionByClientTypeDetailYearly = 919006352U,
			Title_MailboxUsage = 2728899750U,
			Title_SPOSkyDriveProDeployedMonthly = 4227251602U,
			Title_MailboxUsageDetail = 3697180487U,
			Title_CsActiveUserMonthly = 2447679595U,
			Title_CsAVConferenceTimeDaily = 1719022488U,
			Title_CsP2PAVTimeWeekly = 4092464326U,
			Title_ClientOSReport = 548826438U,
			Title_SPOActiveUserWeekly = 1607544303U,
			Minutes = 4116169389U,
			Title_CsClientDeviceMonthly = 2007411791U,
			Title_ClientBrowserDetailReport = 1173626453U,
			Title_SpamMessageDetail = 4086740914U,
			Title_ConnectionByClientTypeDaily = 873044688U,
			Title_SummaryReport = 2102105265U,
			Title_ExternalActivityByUserWeekly = 3778010946U,
			Title_ConnectionByClientTypeDetailWeekly = 4124840583U,
			Title_MessageTraceDetail = 3104710592U,
			Title_SPOTenantStorageMetricWeekly = 3127208035U,
			NA = 3560391903U,
			SizeMB = 4116066136U,
			Title_ConnectionByClientTypeYearly = 3142174497U,
			Title_SPOSkyDriveProStorageWeekly = 2449782919U,
			Title_MxRecordReport = 4117984753U,
			Title_FilterValueList = 3288860276U,
			Title_SPOSkyDriveProDeployedWeekly = 3807172670U,
			Title_ScorecardClientDevice = 3489420090U,
			Title_CsP2PSessionDaily = 2483434450U,
			Title_MessageTrace = 1637029953U,
			Title_CsConferenceDaily = 2188089486U,
			Title_SPOODBFileActivity = 3696480905U,
			Title_ScorecardMetrics = 1466734612U,
			Title_GroupActivityMonthly = 2474312838U,
			Title_SPOTenantStorageMetricMonthly = 3490604815U,
			Title_StaleMailbox = 3438387218U,
			Title_CsActiveUserWeekly = 2626245559U,
			Title_DeviceDashboardSummaryReport = 2699807741U,
			Title_SPOActiveUserDaily = 3798591559U,
			Title_ClientBrowserReport = 1566437488U,
			Title_MailboxActivityYearly = 1406692960U,
			Title_GroupActivityDaily = 1816522614U,
			Title_ScorecardClientOutlook = 369410763U,
			Title_MailboxActivityMonthly = 444818027U,
			Title_DeviceDetailsReport = 2167864011U,
			Title_CsP2PAVTimeMonthly = 2938655794U,
			Title_CsActiveUserYearly = 31883008U,
			Title_OutboundConnectorReport = 1167376766U,
			Title_MailboxActivityDaily = 4159483935U,
			Title_CsConferenceMonthly = 4136517238U,
			CountOfMailboxes = 4055793796U,
			Title_TopTrafficReport = 2411970335U,
			Title_ConnectionByClientTypeWeekly = 1441569752U,
			Title_SPOTeamSiteStorageMonthly = 2900754885U,
			Title_SPOSkyDriveProStorageMonthly = 3750041507U,
			Title_CsAVConferenceTimeMonthly = 3134462204U,
			Title_ExternalActivityMonthly = 4032142024U,
			Title_MalwareMessageDetail = 664019258U,
			Count = 1554779067U,
			Title_ConnectionByClientTypeMonthly = 2391708396U,
			Title_ClientOSDetailReport = 3789075813U,
			Title_PartnerClientExpiringSubscription = 1491838153U,
			Title_ServiceDeliveryReport = 4073762434U,
			Title_LicenseVsUsageSummary = 1120340038U,
			Title_DlpMessageDetail = 2269495169U,
			Title_SPOTenantStorageMetricDaily = 2874211435U,
			Title_PartnerCustomerUser = 3703323930U,
			Title_ExternalActivityByDomainDaily = 2188740169U,
			Title_PolicyTrafficReport = 3901694418U,
			Title_PolicyMessageDetail = 95966325U,
			Title_MessageDetailReport = 2331474543U,
			Title_ExternalActivityByUserDaily = 3348137534U,
			Title_ExternalActivitySummaryWeekly = 878571178U,
			Title_SPOActiveUserMonthly = 1703063467U,
			Title_ExternalActivityDaily = 2654343028U,
			Title_ExternalActivitySummaryMonthly = 203909934U,
			Title_SPOODBUserStatistics = 1942412854U,
			CountOfGroups = 2115947028U,
			Date = 2328219770U,
			Title_SPOTeamSiteDeployedMonthly = 3914274640U,
			Title_GroupActivityWeekly = 1964655722U,
			Title_ExternalActivityByDomainMonthly = 3192449161U,
			Title_ExternalActivityByUserMonthly = 2465396958U,
			Title_ConnectionByClientTypeDetailMonthly = 719677643U,
			Title_CsAVConferenceTimeWeekly = 4178626464U,
			Title_SPOTeamSiteDeployedWeekly = 330021932U,
			Title_SPOTeamSiteStorageWeekly = 1681059041U,
			Title_CsConferenceWeekly = 1464124746U,
			Title_GroupActivityYearly = 849685939U,
			Title_MailboxActivityWeekly = 4557591U,
			Title_CsP2PAVTimeDaily = 3439325998U,
			ActiveUserCount = 1215094758U,
			Title_ExternalActivitySummaryDaily = 3177100050U,
			CountOfAccounts = 4215466264U,
			Title_CsP2PSessionMonthly = 3807350126U,
			Title_CsP2PSessionWeekly = 1382414506U,
			Title_ConnectionByClientTypeDetailDaily = 4205497791U,
			Title_ExternalActivityByDomainWeekly = 386864173U,
			Title_ScorecardClientOS = 3075209078U
		}
	}
}

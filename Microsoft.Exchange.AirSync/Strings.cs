using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AirSync
{
	public static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3901821180U, "MDMQuarantinedMailBodyStep3");
			Strings.stringIDs.Add(617896851U, "ABQMailBodyDeviceUserAgent");
			Strings.stringIDs.Add(119192957U, "ReportItemFolder");
			Strings.stringIDs.Add(3727360630U, "UserName");
			Strings.stringIDs.Add(3163280220U, "ReportPrefix");
			Strings.stringIDs.Add(740650552U, "ReportForward5");
			Strings.stringIDs.Add(1084313144U, "ReportMailboxInfo");
			Strings.stringIDs.Add(1143935079U, "ReportForward6");
			Strings.stringIDs.Add(282544891U, "ABQMailBodyDeviceInformation");
			Strings.stringIDs.Add(4003050722U, "ABQMailBodyDeviceModel");
			Strings.stringIDs.Add(166564534U, "MailboxLogReportBody");
			Strings.stringIDs.Add(4140072557U, "RemoteWipeConfirmationMessageBody2");
			Strings.stringIDs.Add(3901821178U, "MDMQuarantinedMailBodyStep1");
			Strings.stringIDs.Add(2910600975U, "RemoteWipeConfirmationMessageSubject");
			Strings.stringIDs.Add(1023822817U, "BootstrapMailForWM61Subject");
			Strings.stringIDs.Add(2910997761U, "DeviceStatisticsTaskMailboxLogAttachmentNote");
			Strings.stringIDs.Add(4060027620U, "BootstrapMailForWM61Body3");
			Strings.stringIDs.Add(2048398875U, "ReportDeviceId");
			Strings.stringIDs.Add(4060027610U, "BootstrapMailForWM61Body9");
			Strings.stringIDs.Add(137157223U, "UnfamiliarLocationAccountTeam");
			Strings.stringIDs.Add(872007324U, "ReportAssemblyInfo");
			Strings.stringIDs.Add(4060027616U, "BootstrapMailForWM61Body7");
			Strings.stringIDs.Add(4060027615U, "BootstrapMailForWM61Body6");
			Strings.stringIDs.Add(1008143499U, "UnfamiliarLocationTitle");
			Strings.stringIDs.Add(3323369056U, "DeviceType");
			Strings.stringIDs.Add(4060027609U, "BootstrapMailForWM61Body8");
			Strings.stringIDs.Add(940098429U, "MDMNonCompliantMailSubject");
			Strings.stringIDs.Add(2136772851U, "ReportItemType");
			Strings.stringIDs.Add(4049839131U, "ABQMailBodyDeviceOS");
			Strings.stringIDs.Add(4227611602U, "BootstrapMailForWM61Body10");
			Strings.stringIDs.Add(3066249380U, "ReportForward1");
			Strings.stringIDs.Add(3469533907U, "ReportForward2");
			Strings.stringIDs.Add(802699486U, "ABQMailBodyEASVersion");
			Strings.stringIDs.Add(3872818434U, "ReportForward7");
			Strings.stringIDs.Add(1411189202U, "RemoteWipeConfirmationMessageBody3");
			Strings.stringIDs.Add(1844802661U, "DeviceId");
			Strings.stringIDs.Add(38328364U, "ABQMailBodyDeviceAccessState");
			Strings.stringIDs.Add(2769639219U, "UnfamiliarLocationParagraph2");
			Strings.stringIDs.Add(981673616U, "DeviceStatisticsTaskMailboxLogSubject");
			Strings.stringIDs.Add(3272201393U, "UnfamiliarLocationButton");
			Strings.stringIDs.Add(2910205670U, "MDMQuarantinedMailBasicRetryText");
			Strings.stringIDs.Add(2092201661U, "AccessBlockedMailSubject");
			Strings.stringIDs.Add(1349060152U, "AutoBlockedMailSubject");
			Strings.stringIDs.Add(172566471U, "AutoBlockedMailBody1");
			Strings.stringIDs.Add(194232551U, "ReportSyncInfo");
			Strings.stringIDs.Add(566036320U, "ABQMailBodyDeviceAccessStateReason");
			Strings.stringIDs.Add(2491124383U, "IRMCorruptProtectedMessageBodyText");
			Strings.stringIDs.Add(2465427168U, "DeviceDiscoveryMailBody1");
			Strings.stringIDs.Add(3901821179U, "MDMQuarantinedMailBodyStep2");
			Strings.stringIDs.Add(2887039700U, "ABQMailBodyDeviceID");
			Strings.stringIDs.Add(4239215784U, "ReportAndLogSender");
			Strings.stringIDs.Add(579459593U, "ReportStackTrace");
			Strings.stringIDs.Add(3415785961U, "ReportDebugInfo");
			Strings.stringIDs.Add(1903449966U, "ReportForward3");
			Strings.stringIDs.Add(620933251U, "UnfamiliarLocationSubject");
			Strings.stringIDs.Add(2904427437U, "MDMNonCompliantMailBody");
			Strings.stringIDs.Add(2306734493U, "ReportForward4");
			Strings.stringIDs.Add(2951832077U, "MailboxLogReportHeader");
			Strings.stringIDs.Add(2687744413U, "MDMQuarantinedMailBodyEnrollLink");
			Strings.stringIDs.Add(3303701745U, "SMIMENotSupportedBodyText");
			Strings.stringIDs.Add(2987669051U, "ABQMailBodyDeviceIMEI");
			Strings.stringIDs.Add(886329592U, "ReportSubject");
			Strings.stringIDs.Add(55458219U, "MDMQuarantinedMailBody");
			Strings.stringIDs.Add(1248113741U, "ReportCASInfo");
			Strings.stringIDs.Add(118380958U, "MDMNonCompliantMailBodyLinkText");
			Strings.stringIDs.Add(310873002U, "MailboxLogReportSubject");
			Strings.stringIDs.Add(70812490U, "IRMNoViewRightsBodyText");
			Strings.stringIDs.Add(1986464879U, "ReportItemSubject");
			Strings.stringIDs.Add(4060027613U, "BootstrapMailForWM61Body4");
			Strings.stringIDs.Add(1862974783U, "MDMQuarantinedMailSubject");
			Strings.stringIDs.Add(3713119782U, "ReportUnknown");
			Strings.stringIDs.Add(3889310586U, "AccessBlockedMailBody1");
			Strings.stringIDs.Add(2315459723U, "RemoteWipeConfirmationMessageBody1Owa");
			Strings.stringIDs.Add(715035320U, "IRMServerNotAvailableBodyText");
			Strings.stringIDs.Add(2510964059U, "UnfamiliarLocationSubTitle");
			Strings.stringIDs.Add(2727787888U, "RemoteWipeConfirmationMessageHeader");
			Strings.stringIDs.Add(2328219770U, "Date");
			Strings.stringIDs.Add(2710599231U, "DeviceDiscoveryMailSubject");
			Strings.stringIDs.Add(3200123789U, "MaxDevicesExceededMailSubject");
			Strings.stringIDs.Add(2870296499U, "ReportItemCreated");
			Strings.stringIDs.Add(251622507U, "ABQMailBodyDeviceType");
			Strings.stringIDs.Add(4019774947U, "RemoteWipeConfirmationMessageBody1Task");
			Strings.stringIDs.Add(3997627200U, "UnfamiliarLocationClosing");
			Strings.stringIDs.Add(2817448987U, "IRMLicenseExpiredBodyText");
			Strings.stringIDs.Add(3675706513U, "SmsSearchFolder");
			Strings.stringIDs.Add(3017151488U, "IRMPreLicensingFailureBodyText");
			Strings.stringIDs.Add(4060027618U, "BootstrapMailForWM61Body1");
			Strings.stringIDs.Add(4060027619U, "BootstrapMailForWM61Body2");
			Strings.stringIDs.Add(3889721364U, "MDMQuarantinedMailBodyActivateLink");
			Strings.stringIDs.Add(2439737583U, "QuarantinedMailSubject");
			Strings.stringIDs.Add(3714305626U, "UnfamiliarLocationFromName");
			Strings.stringIDs.Add(4060027614U, "BootstrapMailForWM61Body5");
			Strings.stringIDs.Add(586931743U, "MDMQuarantinedMailBodyRetryLink");
			Strings.stringIDs.Add(3715154655U, "IRMServerNotConfiguredBodyText");
			Strings.stringIDs.Add(765518032U, "QuarantinedMailBody1");
			Strings.stringIDs.Add(2153172051U, "IRMReachNotConfiguredBodyText");
		}

		public static LocalizedString MDMQuarantinedMailBodyStep3
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBodyStep3", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceUserAgent
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceUserAgent", "Ex23A244", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportItemFolder
		{
			get
			{
				return new LocalizedString("ReportItemFolder", "Ex549398", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserName
		{
			get
			{
				return new LocalizedString("UserName", "Ex819B3C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportPrefix
		{
			get
			{
				return new LocalizedString("ReportPrefix", "Ex65ACB8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportForward5
		{
			get
			{
				return new LocalizedString("ReportForward5", "ExF77AF9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportMailboxInfo
		{
			get
			{
				return new LocalizedString("ReportMailboxInfo", "ExD21FCF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportForward6
		{
			get
			{
				return new LocalizedString("ReportForward6", "Ex8F26E5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceInformation
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceInformation", "Ex9B4560", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceModel
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceModel", "Ex0DA9E3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxLogReportBody
		{
			get
			{
				return new LocalizedString("MailboxLogReportBody", "Ex5D698F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteWipeConfirmationMessageBody2
		{
			get
			{
				return new LocalizedString("RemoteWipeConfirmationMessageBody2", "Ex9C72C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMQuarantinedMailBodyStep1
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBodyStep1", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteWipeConfirmationMessageSubject
		{
			get
			{
				return new LocalizedString("RemoteWipeConfirmationMessageSubject", "Ex4C0A92", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Subject
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Subject", "Ex0DF123", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceStatisticsTaskMailboxLogAttachmentNote
		{
			get
			{
				return new LocalizedString("DeviceStatisticsTaskMailboxLogAttachmentNote", "Ex350069", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body3
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body3", "ExC3537C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportDeviceId
		{
			get
			{
				return new LocalizedString("ReportDeviceId", "ExEE4F83", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body9
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body9", "Ex30C07A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationAccountTeam
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationAccountTeam", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportAssemblyInfo
		{
			get
			{
				return new LocalizedString("ReportAssemblyInfo", "Ex1D1D3C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body7
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body7", "Ex42AB62", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body6
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body6", "ExFDE36F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationTitle
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationTitle", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceType
		{
			get
			{
				return new LocalizedString("DeviceType", "Ex6E6370", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body8
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body8", "Ex3D14BD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMNonCompliantMailSubject
		{
			get
			{
				return new LocalizedString("MDMNonCompliantMailSubject", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportItemType
		{
			get
			{
				return new LocalizedString("ReportItemType", "ExA8C906", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceOS
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceOS", "Ex9BB95A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body10
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body10", "Ex003B35", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportForward1
		{
			get
			{
				return new LocalizedString("ReportForward1", "Ex4178A1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportForward2
		{
			get
			{
				return new LocalizedString("ReportForward2", "Ex25FC20", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyEASVersion
		{
			get
			{
				return new LocalizedString("ABQMailBodyEASVersion", "Ex49B0E4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportForward7
		{
			get
			{
				return new LocalizedString("ReportForward7", "ExBE9861", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteWipeConfirmationMessageBody3
		{
			get
			{
				return new LocalizedString("RemoteWipeConfirmationMessageBody3", "Ex4339C1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceId
		{
			get
			{
				return new LocalizedString("DeviceId", "ExDB714E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceAccessState
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceAccessState", "Ex29B825", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationParagraph2
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationParagraph2", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanceledDelegatedSubjectPrefix(string displayName)
		{
			return new LocalizedString("CanceledDelegatedSubjectPrefix", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString UnfamiliarLocationParagraph1(string address)
		{
			return new LocalizedString("UnfamiliarLocationParagraph1", "", false, false, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString DeviceStatisticsTaskMailboxLogSubject
		{
			get
			{
				return new LocalizedString("DeviceStatisticsTaskMailboxLogSubject", "ExD59891", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationButton
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationButton", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMQuarantinedMailBasicRetryText
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBasicRetryText", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessBlockedMailSubject
		{
			get
			{
				return new LocalizedString("AccessBlockedMailSubject", "Ex2F80BF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoBlockedMailSubject
		{
			get
			{
				return new LocalizedString("AutoBlockedMailSubject", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoBlockedMailBody1
		{
			get
			{
				return new LocalizedString("AutoBlockedMailBody1", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSyncInfo
		{
			get
			{
				return new LocalizedString("ReportSyncInfo", "ExA4FA6B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceAccessStateReason
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceAccessStateReason", "Ex19E5D5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IRMCorruptProtectedMessageBodyText
		{
			get
			{
				return new LocalizedString("IRMCorruptProtectedMessageBodyText", "ExC2EE82", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceDiscoveryMailBody1
		{
			get
			{
				return new LocalizedString("DeviceDiscoveryMailBody1", "Ex936CB2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMQuarantinedMailBodyStep2
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBodyStep2", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceID
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceID", "Ex0DDECE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportAndLogSender
		{
			get
			{
				return new LocalizedString("ReportAndLogSender", "Ex4A6291", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportStackTrace
		{
			get
			{
				return new LocalizedString("ReportStackTrace", "Ex31A5A2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportDebugInfo
		{
			get
			{
				return new LocalizedString("ReportDebugInfo", "Ex07E341", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportForward3
		{
			get
			{
				return new LocalizedString("ReportForward3", "Ex627990", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationSubject
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationSubject", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMNonCompliantMailBody
		{
			get
			{
				return new LocalizedString("MDMNonCompliantMailBody", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportForward4
		{
			get
			{
				return new LocalizedString("ReportForward4", "ExCA7500", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxLogReportHeader
		{
			get
			{
				return new LocalizedString("MailboxLogReportHeader", "Ex7D0CC0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMQuarantinedMailBodyEnrollLink
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBodyEnrollLink", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SMIMENotSupportedBodyText
		{
			get
			{
				return new LocalizedString("SMIMENotSupportedBodyText", "ExDBF215", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxDevicesExceededMailBody(int deviceCount, uint maxDevicesLimit)
		{
			return new LocalizedString("MaxDevicesExceededMailBody", "Ex1A6E84", false, true, Strings.ResourceManager, new object[]
			{
				deviceCount,
				maxDevicesLimit
			});
		}

		public static LocalizedString ABQMailBodyDeviceIMEI
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceIMEI", "Ex8CD364", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSubject
		{
			get
			{
				return new LocalizedString("ReportSubject", "ExA8B473", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMQuarantinedMailBody
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBody", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportCASInfo
		{
			get
			{
				return new LocalizedString("ReportCASInfo", "Ex72C054", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMNonCompliantMailBodyLinkText
		{
			get
			{
				return new LocalizedString("MDMNonCompliantMailBodyLinkText", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxLogReportSubject
		{
			get
			{
				return new LocalizedString("MailboxLogReportSubject", "ExBF791D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IRMNoViewRightsBodyText
		{
			get
			{
				return new LocalizedString("IRMNoViewRightsBodyText", "Ex1F0166", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportItemSubject
		{
			get
			{
				return new LocalizedString("ReportItemSubject", "Ex813431", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body4
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body4", "ExDAB65E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMQuarantinedMailSubject
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailSubject", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportUnknown
		{
			get
			{
				return new LocalizedString("ReportUnknown", "Ex5C525C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccessBlockedMailBody1
		{
			get
			{
				return new LocalizedString("AccessBlockedMailBody1", "ExCB10C3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteWipeConfirmationMessageBody1Owa
		{
			get
			{
				return new LocalizedString("RemoteWipeConfirmationMessageBody1Owa", "ExD6AD11", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IRMServerNotAvailableBodyText
		{
			get
			{
				return new LocalizedString("IRMServerNotAvailableBodyText", "ExC2B231", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationSubTitle
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationSubTitle", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteWipeConfirmationMessageHeader
		{
			get
			{
				return new LocalizedString("RemoteWipeConfirmationMessageHeader", "Ex65A4C0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Date
		{
			get
			{
				return new LocalizedString("Date", "ExCB2187", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeviceDiscoveryMailSubject
		{
			get
			{
				return new LocalizedString("DeviceDiscoveryMailSubject", "ExC9E961", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxDevicesExceededMailSubject
		{
			get
			{
				return new LocalizedString("MaxDevicesExceededMailSubject", "Ex4E91F8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportItemCreated
		{
			get
			{
				return new LocalizedString("ReportItemCreated", "Ex80CA2B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodyDeviceType
		{
			get
			{
				return new LocalizedString("ABQMailBodyDeviceType", "ExA64CA8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteWipeConfirmationMessageBody1Task
		{
			get
			{
				return new LocalizedString("RemoteWipeConfirmationMessageBody1Task", "Ex2DCEEF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationClosing
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationClosing", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IRMLicenseExpiredBodyText
		{
			get
			{
				return new LocalizedString("IRMLicenseExpiredBodyText", "Ex0D4AAF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SmsSearchFolder
		{
			get
			{
				return new LocalizedString("SmsSearchFolder", "ExFE7AB9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IRMPreLicensingFailureBodyText
		{
			get
			{
				return new LocalizedString("IRMPreLicensingFailureBodyText", "ExF14228", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body1
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body1", "Ex892220", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body2
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body2", "ExE766BC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ABQMailBodySentAt(string dateTime, string recipientsSMTP)
		{
			return new LocalizedString("ABQMailBodySentAt", "Ex9B1624", false, true, Strings.ResourceManager, new object[]
			{
				dateTime,
				recipientsSMTP
			});
		}

		public static LocalizedString MDMQuarantinedMailBodyActivateLink
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBodyActivateLink", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QuarantinedMailSubject
		{
			get
			{
				return new LocalizedString("QuarantinedMailSubject", "Ex11B316", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnfamiliarLocationFromName
		{
			get
			{
				return new LocalizedString("UnfamiliarLocationFromName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BootstrapMailForWM61Body5
		{
			get
			{
				return new LocalizedString("BootstrapMailForWM61Body5", "Ex4EC313", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MDMQuarantinedMailBodyRetryLink
		{
			get
			{
				return new LocalizedString("MDMQuarantinedMailBodyRetryLink", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IRMServerNotConfiguredBodyText
		{
			get
			{
				return new LocalizedString("IRMServerNotConfiguredBodyText", "Ex04E16A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedSubjectPrefix(string displayName)
		{
			return new LocalizedString("DelegatedSubjectPrefix", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString QuarantinedMailBody1
		{
			get
			{
				return new LocalizedString("QuarantinedMailBody1", "ExBED365", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IRMReachNotConfiguredBodyText
		{
			get
			{
				return new LocalizedString("IRMReachNotConfiguredBodyText", "Ex5527F8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(96);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.AirSync.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			MDMQuarantinedMailBodyStep3 = 3901821180U,
			ABQMailBodyDeviceUserAgent = 617896851U,
			ReportItemFolder = 119192957U,
			UserName = 3727360630U,
			ReportPrefix = 3163280220U,
			ReportForward5 = 740650552U,
			ReportMailboxInfo = 1084313144U,
			ReportForward6 = 1143935079U,
			ABQMailBodyDeviceInformation = 282544891U,
			ABQMailBodyDeviceModel = 4003050722U,
			MailboxLogReportBody = 166564534U,
			RemoteWipeConfirmationMessageBody2 = 4140072557U,
			MDMQuarantinedMailBodyStep1 = 3901821178U,
			RemoteWipeConfirmationMessageSubject = 2910600975U,
			BootstrapMailForWM61Subject = 1023822817U,
			DeviceStatisticsTaskMailboxLogAttachmentNote = 2910997761U,
			BootstrapMailForWM61Body3 = 4060027620U,
			ReportDeviceId = 2048398875U,
			BootstrapMailForWM61Body9 = 4060027610U,
			UnfamiliarLocationAccountTeam = 137157223U,
			ReportAssemblyInfo = 872007324U,
			BootstrapMailForWM61Body7 = 4060027616U,
			BootstrapMailForWM61Body6 = 4060027615U,
			UnfamiliarLocationTitle = 1008143499U,
			DeviceType = 3323369056U,
			BootstrapMailForWM61Body8 = 4060027609U,
			MDMNonCompliantMailSubject = 940098429U,
			ReportItemType = 2136772851U,
			ABQMailBodyDeviceOS = 4049839131U,
			BootstrapMailForWM61Body10 = 4227611602U,
			ReportForward1 = 3066249380U,
			ReportForward2 = 3469533907U,
			ABQMailBodyEASVersion = 802699486U,
			ReportForward7 = 3872818434U,
			RemoteWipeConfirmationMessageBody3 = 1411189202U,
			DeviceId = 1844802661U,
			ABQMailBodyDeviceAccessState = 38328364U,
			UnfamiliarLocationParagraph2 = 2769639219U,
			DeviceStatisticsTaskMailboxLogSubject = 981673616U,
			UnfamiliarLocationButton = 3272201393U,
			MDMQuarantinedMailBasicRetryText = 2910205670U,
			AccessBlockedMailSubject = 2092201661U,
			AutoBlockedMailSubject = 1349060152U,
			AutoBlockedMailBody1 = 172566471U,
			ReportSyncInfo = 194232551U,
			ABQMailBodyDeviceAccessStateReason = 566036320U,
			IRMCorruptProtectedMessageBodyText = 2491124383U,
			DeviceDiscoveryMailBody1 = 2465427168U,
			MDMQuarantinedMailBodyStep2 = 3901821179U,
			ABQMailBodyDeviceID = 2887039700U,
			ReportAndLogSender = 4239215784U,
			ReportStackTrace = 579459593U,
			ReportDebugInfo = 3415785961U,
			ReportForward3 = 1903449966U,
			UnfamiliarLocationSubject = 620933251U,
			MDMNonCompliantMailBody = 2904427437U,
			ReportForward4 = 2306734493U,
			MailboxLogReportHeader = 2951832077U,
			MDMQuarantinedMailBodyEnrollLink = 2687744413U,
			SMIMENotSupportedBodyText = 3303701745U,
			ABQMailBodyDeviceIMEI = 2987669051U,
			ReportSubject = 886329592U,
			MDMQuarantinedMailBody = 55458219U,
			ReportCASInfo = 1248113741U,
			MDMNonCompliantMailBodyLinkText = 118380958U,
			MailboxLogReportSubject = 310873002U,
			IRMNoViewRightsBodyText = 70812490U,
			ReportItemSubject = 1986464879U,
			BootstrapMailForWM61Body4 = 4060027613U,
			MDMQuarantinedMailSubject = 1862974783U,
			ReportUnknown = 3713119782U,
			AccessBlockedMailBody1 = 3889310586U,
			RemoteWipeConfirmationMessageBody1Owa = 2315459723U,
			IRMServerNotAvailableBodyText = 715035320U,
			UnfamiliarLocationSubTitle = 2510964059U,
			RemoteWipeConfirmationMessageHeader = 2727787888U,
			Date = 2328219770U,
			DeviceDiscoveryMailSubject = 2710599231U,
			MaxDevicesExceededMailSubject = 3200123789U,
			ReportItemCreated = 2870296499U,
			ABQMailBodyDeviceType = 251622507U,
			RemoteWipeConfirmationMessageBody1Task = 4019774947U,
			UnfamiliarLocationClosing = 3997627200U,
			IRMLicenseExpiredBodyText = 2817448987U,
			SmsSearchFolder = 3675706513U,
			IRMPreLicensingFailureBodyText = 3017151488U,
			BootstrapMailForWM61Body1 = 4060027618U,
			BootstrapMailForWM61Body2,
			MDMQuarantinedMailBodyActivateLink = 3889721364U,
			QuarantinedMailSubject = 2439737583U,
			UnfamiliarLocationFromName = 3714305626U,
			BootstrapMailForWM61Body5 = 4060027614U,
			MDMQuarantinedMailBodyRetryLink = 586931743U,
			IRMServerNotConfiguredBodyText = 3715154655U,
			QuarantinedMailBody1 = 765518032U,
			IRMReachNotConfiguredBodyText = 2153172051U
		}

		private enum ParamIDs
		{
			CanceledDelegatedSubjectPrefix,
			UnfamiliarLocationParagraph1,
			MaxDevicesExceededMailBody,
			ABQMailBodySentAt,
			DelegatedSubjectPrefix
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1200652681U, "NspiTaskTitle");
			Strings.stringIDs.Add(4108158018U, "OutlookTaskDescription");
			Strings.stringIDs.Add(2906070402U, "AccountDisplayName");
			Strings.stringIDs.Add(1735106406U, "NspiBindTaskTitle");
			Strings.stringIDs.Add(3751775428U, "ScomAlertLoggerTaskStartProperties");
			Strings.stringIDs.Add(519952531U, "NspiTaskDescription");
			Strings.stringIDs.Add(2627813696U, "NspiGetMatchesTaskTitle");
			Strings.stringIDs.Add(2088641341U, "EmsmdbTaskTitle");
			Strings.stringIDs.Add(4012877537U, "VerifyRpcProxyTaskDescription");
			Strings.stringIDs.Add(328709952U, "DiscoverWebProxyTaskDescription");
			Strings.stringIDs.Add(1000340169U, "NspiQueryHomeMDBTaskDescription");
			Strings.stringIDs.Add(3433159169U, "AsyncTaskDescription");
			Strings.stringIDs.Add(2307157568U, "RfriTaskTitle");
			Strings.stringIDs.Add(2471490565U, "NspiGetPropsTaskTitle");
			Strings.stringIDs.Add(3194835879U, "NspiGetPropsTaskDescription");
			Strings.stringIDs.Add(3762287994U, "NspiGetNetworkAddressesPropertyTaskDescription");
			Strings.stringIDs.Add(4086077102U, "NspiGetHierarchyInfoTaskTitle");
			Strings.stringIDs.Add(375864490U, "InputPasswordRequired");
			Strings.stringIDs.Add(3350259510U, "NspiGetNetworkAddressesPropertyTaskTitle");
			Strings.stringIDs.Add(3206764939U, "VerifyRpcProxyTaskTitle");
			Strings.stringIDs.Add(1328936299U, "WrongAuthForPersonalizedServer");
			Strings.stringIDs.Add(2224461727U, "EmsmdbTaskDescription");
			Strings.stringIDs.Add(1666890870U, "WrongDefinitionType");
			Strings.stringIDs.Add(4202007121U, "RfriGetFqdnTaskTitle");
			Strings.stringIDs.Add(972661725U, "SecondaryEndpoint");
			Strings.stringIDs.Add(3707932024U, "NspiGetMatchesTaskDescription");
			Strings.stringIDs.Add(3047902021U, "EmsmdbConnectTaskDescription");
			Strings.stringIDs.Add(3157018328U, "RfriGetNewDsaTaskDescription");
			Strings.stringIDs.Add(1880067925U, "DummyTaskTitle");
			Strings.stringIDs.Add(2936865088U, "NspiQueryRowsTaskTitle");
			Strings.stringIDs.Add(3754643782U, "ExtensionAttributes");
			Strings.stringIDs.Add(3942413427U, "DummyTaskDescription");
			Strings.stringIDs.Add(1195865221U, "InputPasswordNotRequired");
			Strings.stringIDs.Add(4271094603U, "NspiGetNetworkAddressesTaskDescription");
			Strings.stringIDs.Add(3049602642U, "NspiGetHierarchyInfoTaskDescription");
			Strings.stringIDs.Add(1196155619U, "Endpoint");
			Strings.stringIDs.Add(613263819U, "NspiQueryHomeMDBTaskTitle");
			Strings.stringIDs.Add(1958074863U, "RfriGetFqdnTaskDescription");
			Strings.stringIDs.Add(52542312U, "DiscoverWebProxyTaskTitle");
			Strings.stringIDs.Add(3709524604U, "RfriGetNewDsaTaskTitle");
			Strings.stringIDs.Add(547095242U, "OutlookTaskTitle");
			Strings.stringIDs.Add(4266394152U, "ScomAlertLoggerTaskPropertyNullValue");
			Strings.stringIDs.Add(611708359U, "RetryTaskDescription");
			Strings.stringIDs.Add(433052918U, "EmsmdbLogonTaskTitle");
			Strings.stringIDs.Add(342672400U, "RcaOutlookTaskTitle");
			Strings.stringIDs.Add(1152705677U, "MonitoringAccount");
			Strings.stringIDs.Add(1587959117U, "PFEmsmdbTaskDescription");
			Strings.stringIDs.Add(3982499041U, "ScomAlertLoggerTaskCompletedProperties");
			Strings.stringIDs.Add(3321608102U, "EmsmdbLogonTaskDescription");
			Strings.stringIDs.Add(2896583015U, "EmsmdbConnectTaskTitle");
			Strings.stringIDs.Add(3911446795U, "PFEmsmdbTaskTitle");
			Strings.stringIDs.Add(3508157170U, "MonitoringAccountPassword");
			Strings.stringIDs.Add(1342291712U, "PFEmsmdbLogonTaskDescription");
			Strings.stringIDs.Add(620115544U, "RcaOutlookTaskDescription");
			Strings.stringIDs.Add(2112045541U, "NspiGetNetworkAddressesTaskTitle");
			Strings.stringIDs.Add(46727295U, "NspiUnbindTaskTitle");
			Strings.stringIDs.Add(2723049729U, "TaskExceptionMessage");
			Strings.stringIDs.Add(1442961372U, "NspiQueryRowsTaskDescription");
			Strings.stringIDs.Add(4002896033U, "NspiUnbindTaskDescription");
			Strings.stringIDs.Add(33448767U, "NspiDNToEphTaskDescription");
			Strings.stringIDs.Add(43152595U, "PFEmsmdbConnectTaskDescription");
			Strings.stringIDs.Add(763528660U, "RfriTaskDescription");
			Strings.stringIDs.Add(1421893477U, "RetryTaskTitle");
			Strings.stringIDs.Add(1091351742U, "NspiBindTaskDescription");
			Strings.stringIDs.Add(4164811492U, "Identity");
			Strings.stringIDs.Add(1994569285U, "NspiDNToEphTaskTitle");
			Strings.stringIDs.Add(3045810739U, "AsyncTaskTitle");
		}

		public static LocalizedString NspiTaskTitle
		{
			get
			{
				return new LocalizedString("NspiTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookTaskDescription
		{
			get
			{
				return new LocalizedString("OutlookTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AccountDisplayName
		{
			get
			{
				return new LocalizedString("AccountDisplayName", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiBindTaskTitle
		{
			get
			{
				return new LocalizedString("NspiBindTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScomAlertLoggerTaskStartProperties
		{
			get
			{
				return new LocalizedString("ScomAlertLoggerTaskStartProperties", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiTaskDescription
		{
			get
			{
				return new LocalizedString("NspiTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetMatchesTaskTitle
		{
			get
			{
				return new LocalizedString("NspiGetMatchesTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmsmdbTaskTitle
		{
			get
			{
				return new LocalizedString("EmsmdbTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerifyRpcProxyTaskDescription
		{
			get
			{
				return new LocalizedString("VerifyRpcProxyTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscoverWebProxyTaskDescription
		{
			get
			{
				return new LocalizedString("DiscoverWebProxyTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiQueryHomeMDBTaskDescription
		{
			get
			{
				return new LocalizedString("NspiQueryHomeMDBTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncTaskDescription
		{
			get
			{
				return new LocalizedString("AsyncTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RfriTaskTitle
		{
			get
			{
				return new LocalizedString("RfriTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetPropsTaskTitle
		{
			get
			{
				return new LocalizedString("NspiGetPropsTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetPropsTaskDescription
		{
			get
			{
				return new LocalizedString("NspiGetPropsTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetNetworkAddressesPropertyTaskDescription
		{
			get
			{
				return new LocalizedString("NspiGetNetworkAddressesPropertyTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetHierarchyInfoTaskTitle
		{
			get
			{
				return new LocalizedString("NspiGetHierarchyInfoTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScomAlertLoggerTaskFailed(LocalizedString taskName)
		{
			return new LocalizedString("ScomAlertLoggerTaskFailed", Strings.ResourceManager, new object[]
			{
				taskName
			});
		}

		public static LocalizedString InputPasswordRequired
		{
			get
			{
				return new LocalizedString("InputPasswordRequired", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetNetworkAddressesPropertyTaskTitle
		{
			get
			{
				return new LocalizedString("NspiGetNetworkAddressesPropertyTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VerifyRpcProxyTaskTitle
		{
			get
			{
				return new LocalizedString("VerifyRpcProxyTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongAuthForPersonalizedServer
		{
			get
			{
				return new LocalizedString("WrongAuthForPersonalizedServer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmsmdbTaskDescription
		{
			get
			{
				return new LocalizedString("EmsmdbTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongDefinitionType
		{
			get
			{
				return new LocalizedString("WrongDefinitionType", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RfriGetFqdnTaskTitle
		{
			get
			{
				return new LocalizedString("RfriGetFqdnTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecondaryEndpoint
		{
			get
			{
				return new LocalizedString("SecondaryEndpoint", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetMatchesTaskDescription
		{
			get
			{
				return new LocalizedString("NspiGetMatchesTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmsmdbConnectTaskDescription
		{
			get
			{
				return new LocalizedString("EmsmdbConnectTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RfriGetNewDsaTaskDescription
		{
			get
			{
				return new LocalizedString("RfriGetNewDsaTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DummyTaskTitle
		{
			get
			{
				return new LocalizedString("DummyTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiQueryRowsTaskTitle
		{
			get
			{
				return new LocalizedString("NspiQueryRowsTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompositeTaskTitle(int numberOfTasks)
		{
			return new LocalizedString("CompositeTaskTitle", Strings.ResourceManager, new object[]
			{
				numberOfTasks
			});
		}

		public static LocalizedString ExtensionAttributes
		{
			get
			{
				return new LocalizedString("ExtensionAttributes", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcCallResultErrorCodeDescription(string callResultType)
		{
			return new LocalizedString("RpcCallResultErrorCodeDescription", Strings.ResourceManager, new object[]
			{
				callResultType
			});
		}

		public static LocalizedString DummyTaskDescription
		{
			get
			{
				return new LocalizedString("DummyTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompositeTaskDescription(int numberOfTasks)
		{
			return new LocalizedString("CompositeTaskDescription", Strings.ResourceManager, new object[]
			{
				numberOfTasks
			});
		}

		public static LocalizedString NetworkCredentialString(string domain, string userName)
		{
			return new LocalizedString("NetworkCredentialString", Strings.ResourceManager, new object[]
			{
				domain,
				userName
			});
		}

		public static LocalizedString InputPasswordNotRequired
		{
			get
			{
				return new LocalizedString("InputPasswordNotRequired", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetNetworkAddressesTaskDescription
		{
			get
			{
				return new LocalizedString("NspiGetNetworkAddressesTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetHierarchyInfoTaskDescription
		{
			get
			{
				return new LocalizedString("NspiGetHierarchyInfoTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Endpoint
		{
			get
			{
				return new LocalizedString("Endpoint", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiQueryHomeMDBTaskTitle
		{
			get
			{
				return new LocalizedString("NspiQueryHomeMDBTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RfriGetFqdnTaskDescription
		{
			get
			{
				return new LocalizedString("RfriGetFqdnTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscoverWebProxyTaskTitle
		{
			get
			{
				return new LocalizedString("DiscoverWebProxyTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RfriGetNewDsaTaskTitle
		{
			get
			{
				return new LocalizedString("RfriGetNewDsaTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OutlookTaskTitle
		{
			get
			{
				return new LocalizedString("OutlookTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScomAlertLoggerTaskPropertyNullValue
		{
			get
			{
				return new LocalizedString("ScomAlertLoggerTaskPropertyNullValue", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetryTaskDescription
		{
			get
			{
				return new LocalizedString("RetryTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmsmdbLogonTaskTitle
		{
			get
			{
				return new LocalizedString("EmsmdbLogonTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RcaOutlookTaskTitle
		{
			get
			{
				return new LocalizedString("RcaOutlookTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScomAlertLoggerTaskProperty(string requiredProperty, string requiredPropertyValue)
		{
			return new LocalizedString("ScomAlertLoggerTaskProperty", Strings.ResourceManager, new object[]
			{
				requiredProperty,
				requiredPropertyValue
			});
		}

		public static LocalizedString ScomAlertLoggerIndent(LocalizedString textToIndent)
		{
			return new LocalizedString("ScomAlertLoggerIndent", Strings.ResourceManager, new object[]
			{
				textToIndent
			});
		}

		public static LocalizedString ScomAlertLoggerTaskSucceeded(LocalizedString taskName)
		{
			return new LocalizedString("ScomAlertLoggerTaskSucceeded", Strings.ResourceManager, new object[]
			{
				taskName
			});
		}

		public static LocalizedString MonitoringAccount
		{
			get
			{
				return new LocalizedString("MonitoringAccount", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PFEmsmdbTaskDescription
		{
			get
			{
				return new LocalizedString("PFEmsmdbTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScomAlertLoggerTaskStarted(LocalizedString taskName)
		{
			return new LocalizedString("ScomAlertLoggerTaskStarted", Strings.ResourceManager, new object[]
			{
				taskName
			});
		}

		public static LocalizedString ScomAlertLoggerTaskCompletedProperties
		{
			get
			{
				return new LocalizedString("ScomAlertLoggerTaskCompletedProperties", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmsmdbLogonTaskDescription
		{
			get
			{
				return new LocalizedString("EmsmdbLogonTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmsmdbConnectTaskTitle
		{
			get
			{
				return new LocalizedString("EmsmdbConnectTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PFEmsmdbTaskTitle
		{
			get
			{
				return new LocalizedString("PFEmsmdbTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MonitoringAccountPassword
		{
			get
			{
				return new LocalizedString("MonitoringAccountPassword", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyNotFoundExceptionMessage(string propertyName)
		{
			return new LocalizedString("PropertyNotFoundExceptionMessage", Strings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString PFEmsmdbLogonTaskDescription
		{
			get
			{
				return new LocalizedString("PFEmsmdbLogonTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RcaOutlookTaskDescription
		{
			get
			{
				return new LocalizedString("RcaOutlookTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiGetNetworkAddressesTaskTitle
		{
			get
			{
				return new LocalizedString("NspiGetNetworkAddressesTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiUnbindTaskTitle
		{
			get
			{
				return new LocalizedString("NspiUnbindTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskExceptionMessage
		{
			get
			{
				return new LocalizedString("TaskExceptionMessage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ScomAlertLoggerTaskDescription(LocalizedString taskDescription)
		{
			return new LocalizedString("ScomAlertLoggerTaskDescription", Strings.ResourceManager, new object[]
			{
				taskDescription
			});
		}

		public static LocalizedString NspiQueryRowsTaskDescription
		{
			get
			{
				return new LocalizedString("NspiQueryRowsTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiUnbindTaskDescription
		{
			get
			{
				return new LocalizedString("NspiUnbindTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiDNToEphTaskDescription
		{
			get
			{
				return new LocalizedString("NspiDNToEphTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PFEmsmdbConnectTaskDescription
		{
			get
			{
				return new LocalizedString("PFEmsmdbConnectTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RfriTaskDescription
		{
			get
			{
				return new LocalizedString("RfriTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RetryTaskTitle
		{
			get
			{
				return new LocalizedString("RetryTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiBindTaskDescription
		{
			get
			{
				return new LocalizedString("NspiBindTaskDescription", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Identity
		{
			get
			{
				return new LocalizedString("Identity", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NspiDNToEphTaskTitle
		{
			get
			{
				return new LocalizedString("NspiDNToEphTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncTaskTitle
		{
			get
			{
				return new LocalizedString("AsyncTaskTitle", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ListOfItems(string list, string newItem)
		{
			return new LocalizedString("ListOfItems", Strings.ResourceManager, new object[]
			{
				list,
				newItem
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(67);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.RpcClientAccess.Monitoring.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NspiTaskTitle = 1200652681U,
			OutlookTaskDescription = 4108158018U,
			AccountDisplayName = 2906070402U,
			NspiBindTaskTitle = 1735106406U,
			ScomAlertLoggerTaskStartProperties = 3751775428U,
			NspiTaskDescription = 519952531U,
			NspiGetMatchesTaskTitle = 2627813696U,
			EmsmdbTaskTitle = 2088641341U,
			VerifyRpcProxyTaskDescription = 4012877537U,
			DiscoverWebProxyTaskDescription = 328709952U,
			NspiQueryHomeMDBTaskDescription = 1000340169U,
			AsyncTaskDescription = 3433159169U,
			RfriTaskTitle = 2307157568U,
			NspiGetPropsTaskTitle = 2471490565U,
			NspiGetPropsTaskDescription = 3194835879U,
			NspiGetNetworkAddressesPropertyTaskDescription = 3762287994U,
			NspiGetHierarchyInfoTaskTitle = 4086077102U,
			InputPasswordRequired = 375864490U,
			NspiGetNetworkAddressesPropertyTaskTitle = 3350259510U,
			VerifyRpcProxyTaskTitle = 3206764939U,
			WrongAuthForPersonalizedServer = 1328936299U,
			EmsmdbTaskDescription = 2224461727U,
			WrongDefinitionType = 1666890870U,
			RfriGetFqdnTaskTitle = 4202007121U,
			SecondaryEndpoint = 972661725U,
			NspiGetMatchesTaskDescription = 3707932024U,
			EmsmdbConnectTaskDescription = 3047902021U,
			RfriGetNewDsaTaskDescription = 3157018328U,
			DummyTaskTitle = 1880067925U,
			NspiQueryRowsTaskTitle = 2936865088U,
			ExtensionAttributes = 3754643782U,
			DummyTaskDescription = 3942413427U,
			InputPasswordNotRequired = 1195865221U,
			NspiGetNetworkAddressesTaskDescription = 4271094603U,
			NspiGetHierarchyInfoTaskDescription = 3049602642U,
			Endpoint = 1196155619U,
			NspiQueryHomeMDBTaskTitle = 613263819U,
			RfriGetFqdnTaskDescription = 1958074863U,
			DiscoverWebProxyTaskTitle = 52542312U,
			RfriGetNewDsaTaskTitle = 3709524604U,
			OutlookTaskTitle = 547095242U,
			ScomAlertLoggerTaskPropertyNullValue = 4266394152U,
			RetryTaskDescription = 611708359U,
			EmsmdbLogonTaskTitle = 433052918U,
			RcaOutlookTaskTitle = 342672400U,
			MonitoringAccount = 1152705677U,
			PFEmsmdbTaskDescription = 1587959117U,
			ScomAlertLoggerTaskCompletedProperties = 3982499041U,
			EmsmdbLogonTaskDescription = 3321608102U,
			EmsmdbConnectTaskTitle = 2896583015U,
			PFEmsmdbTaskTitle = 3911446795U,
			MonitoringAccountPassword = 3508157170U,
			PFEmsmdbLogonTaskDescription = 1342291712U,
			RcaOutlookTaskDescription = 620115544U,
			NspiGetNetworkAddressesTaskTitle = 2112045541U,
			NspiUnbindTaskTitle = 46727295U,
			TaskExceptionMessage = 2723049729U,
			NspiQueryRowsTaskDescription = 1442961372U,
			NspiUnbindTaskDescription = 4002896033U,
			NspiDNToEphTaskDescription = 33448767U,
			PFEmsmdbConnectTaskDescription = 43152595U,
			RfriTaskDescription = 763528660U,
			RetryTaskTitle = 1421893477U,
			NspiBindTaskDescription = 1091351742U,
			Identity = 4164811492U,
			NspiDNToEphTaskTitle = 1994569285U,
			AsyncTaskTitle = 3045810739U
		}

		private enum ParamIDs
		{
			ScomAlertLoggerTaskFailed,
			CompositeTaskTitle,
			RpcCallResultErrorCodeDescription,
			CompositeTaskDescription,
			NetworkCredentialString,
			ScomAlertLoggerTaskProperty,
			ScomAlertLoggerIndent,
			ScomAlertLoggerTaskSucceeded,
			ScomAlertLoggerTaskStarted,
			PropertyNotFoundExceptionMessage,
			ScomAlertLoggerTaskDescription,
			ListOfItems
		}
	}
}

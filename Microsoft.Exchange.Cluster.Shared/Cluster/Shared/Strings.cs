using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1578974238U, "FixUpIpAddressStatusUnchanged");
			Strings.stringIDs.Add(4106740448U, "RemoteClusterWin2k3ToWin2k8NotSupportedException");
			Strings.stringIDs.Add(3232505775U, "GroupStatePending");
			Strings.stringIDs.Add(801548279U, "NoErrorSpecified");
			Strings.stringIDs.Add(487199380U, "GroupStatePartialOnline");
			Strings.stringIDs.Add(3972288899U, "GroupStateFailed");
			Strings.stringIDs.Add(2874666956U, "GroupStateUnknown");
			Strings.stringIDs.Add(2356112387U, "GroupStateOnline");
			Strings.stringIDs.Add(344705101U, "GroupStateOffline");
			Strings.stringIDs.Add(330900787U, "ClusterNotRunningException");
			Strings.stringIDs.Add(4244700832U, "RemoteClusterWin2k8ToWin2k3NotSupportedException");
		}

		public static LocalizedString IpResourceCreationOnWrongTypeOfNetworkException(string network)
		{
			return new LocalizedString("IpResourceCreationOnWrongTypeOfNetworkException", Strings.ResourceManager, new object[]
			{
				network
			});
		}

		public static LocalizedString FixUpIpAddressStatusUnchanged
		{
			get
			{
				return new LocalizedString("FixUpIpAddressStatusUnchanged", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusCommonRetryableTransientException(string msg)
		{
			return new LocalizedString("ClusCommonRetryableTransientException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString OfflineOperationTimedOutException(string objectName, int count, int secondsTimeout)
		{
			return new LocalizedString("OfflineOperationTimedOutException", Strings.ResourceManager, new object[]
			{
				objectName,
				count,
				secondsTimeout
			});
		}

		public static LocalizedString RemoteClusterWin2k3ToWin2k8NotSupportedException
		{
			get
			{
				return new LocalizedString("RemoteClusterWin2k3ToWin2k8NotSupportedException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestedNetworkIsNotDhcpEnabled(string network)
		{
			return new LocalizedString("RequestedNetworkIsNotDhcpEnabled", Strings.ResourceManager, new object[]
			{
				network
			});
		}

		public static LocalizedString FailedToFindNetwork(string network)
		{
			return new LocalizedString("FailedToFindNetwork", Strings.ResourceManager, new object[]
			{
				network
			});
		}

		public static LocalizedString DxStoreKeyApiFailedMessage(string api, string keyName, string msg)
		{
			return new LocalizedString("DxStoreKeyApiFailedMessage", Strings.ResourceManager, new object[]
			{
				api,
				keyName,
				msg
			});
		}

		public static LocalizedString ClusCommonFailException(string error)
		{
			return new LocalizedString("ClusCommonFailException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ClusResourceAlreadyExistsException(string resourceName)
		{
			return new LocalizedString("ClusResourceAlreadyExistsException", Strings.ResourceManager, new object[]
			{
				resourceName
			});
		}

		public static LocalizedString ClusCommonNonRetryableTransientException(string msg)
		{
			return new LocalizedString("ClusCommonNonRetryableTransientException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString RegistryParameterKeyNotOpenedException(string keyName)
		{
			return new LocalizedString("RegistryParameterKeyNotOpenedException", Strings.ResourceManager, new object[]
			{
				keyName
			});
		}

		public static LocalizedString RegistryParameterWriteException(string valueName, string errMsg)
		{
			return new LocalizedString("RegistryParameterWriteException", Strings.ResourceManager, new object[]
			{
				valueName,
				errMsg
			});
		}

		public static LocalizedString RegistryParameterException(string errorMsg)
		{
			return new LocalizedString("RegistryParameterException", Strings.ResourceManager, new object[]
			{
				errorMsg
			});
		}

		public static LocalizedString ClusterFileNotFoundException(string nodeName)
		{
			return new LocalizedString("ClusterFileNotFoundException", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString AmGetFqdnFailedADError(string nodeName, string adError)
		{
			return new LocalizedString("AmGetFqdnFailedADError", Strings.ResourceManager, new object[]
			{
				nodeName,
				adError
			});
		}

		public static LocalizedString ClusCommonTransientException(string error)
		{
			return new LocalizedString("ClusCommonTransientException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ClusterEvictWithoutCleanupException(string nodeName)
		{
			return new LocalizedString("ClusterEvictWithoutCleanupException", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString GroupStatePending
		{
			get
			{
				return new LocalizedString("GroupStatePending", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterNotJoinedException(string nodeName)
		{
			return new LocalizedString("ClusterNotJoinedException", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString AmCoreGroupRegNotFound(string regvalueName)
		{
			return new LocalizedString("AmCoreGroupRegNotFound", Strings.ResourceManager, new object[]
			{
				regvalueName
			});
		}

		public static LocalizedString IPv4NetworksHasDuplicateEntries(string duplicate)
		{
			return new LocalizedString("IPv4NetworksHasDuplicateEntries", Strings.ResourceManager, new object[]
			{
				duplicate
			});
		}

		public static LocalizedString AmServerNameResolveFqdnException(string error)
		{
			return new LocalizedString("AmServerNameResolveFqdnException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ClusCommonTaskPendingException(string msg)
		{
			return new LocalizedString("ClusCommonTaskPendingException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ClusterNotInstalledException(string nodeName)
		{
			return new LocalizedString("ClusterNotInstalledException", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString NoErrorSpecified
		{
			get
			{
				return new LocalizedString("NoErrorSpecified", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupStatePartialOnline
		{
			get
			{
				return new LocalizedString("GroupStatePartialOnline", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidResourceOpException(string resName)
		{
			return new LocalizedString("InvalidResourceOpException", Strings.ResourceManager, new object[]
			{
				resName
			});
		}

		public static LocalizedString ClusterNodeNotFoundException(string nodeName)
		{
			return new LocalizedString("ClusterNodeNotFoundException", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString RequestedNetworkIsNotIPv6Enabled(string network)
		{
			return new LocalizedString("RequestedNetworkIsNotIPv6Enabled", Strings.ResourceManager, new object[]
			{
				network
			});
		}

		public static LocalizedString ClusterApiException(string msg)
		{
			return new LocalizedString("ClusterApiException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString DxStoreKeyNotFoundException(string keyName)
		{
			return new LocalizedString("DxStoreKeyNotFoundException", Strings.ResourceManager, new object[]
			{
				keyName
			});
		}

		public static LocalizedString RegistryParameterReadException(string valueName, string errMsg)
		{
			return new LocalizedString("RegistryParameterReadException", Strings.ResourceManager, new object[]
			{
				valueName,
				errMsg
			});
		}

		public static LocalizedString DxStoreKeyInvalidKeyException(string keyName)
		{
			return new LocalizedString("DxStoreKeyInvalidKeyException", Strings.ResourceManager, new object[]
			{
				keyName
			});
		}

		public static LocalizedString DagTaskNotEnoughStaticIPAddresses(string network, string staticIps)
		{
			return new LocalizedString("DagTaskNotEnoughStaticIPAddresses", Strings.ResourceManager, new object[]
			{
				network,
				staticIps
			});
		}

		public static LocalizedString ClusteringNotInstalledOnLHException(string errorMessage)
		{
			return new LocalizedString("ClusteringNotInstalledOnLHException", Strings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString OperationValidOnlyOnLonghornException(string resName)
		{
			return new LocalizedString("OperationValidOnlyOnLonghornException", Strings.ResourceManager, new object[]
			{
				resName
			});
		}

		public static LocalizedString ClusterUnsupportedRegistryTypeException(string typeName)
		{
			return new LocalizedString("ClusterUnsupportedRegistryTypeException", Strings.ResourceManager, new object[]
			{
				typeName
			});
		}

		public static LocalizedString FixUpIpAddressStatusUpdated(int deletedResources, int newResources)
		{
			return new LocalizedString("FixUpIpAddressStatusUpdated", Strings.ResourceManager, new object[]
			{
				deletedResources,
				newResources
			});
		}

		public static LocalizedString NoClusResourceFoundException(string groupName, string resourceName)
		{
			return new LocalizedString("NoClusResourceFoundException", Strings.ResourceManager, new object[]
			{
				groupName,
				resourceName
			});
		}

		public static LocalizedString AmGetFqdnFailedNotFound(string nodeName)
		{
			return new LocalizedString("AmGetFqdnFailedNotFound", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString FailToOfflineClusResourceException(string groupName, string resourceId, string reason)
		{
			return new LocalizedString("FailToOfflineClusResourceException", Strings.ResourceManager, new object[]
			{
				groupName,
				resourceId,
				reason
			});
		}

		public static LocalizedString ClusterNodeJoinedException(string nodeName)
		{
			return new LocalizedString("ClusterNodeJoinedException", Strings.ResourceManager, new object[]
			{
				nodeName
			});
		}

		public static LocalizedString ClusterDatabaseTransientException(string msg)
		{
			return new LocalizedString("ClusterDatabaseTransientException", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString OpenClusterTimedoutException(string serverName, int timeoutInSeconds, string context)
		{
			return new LocalizedString("OpenClusterTimedoutException", Strings.ResourceManager, new object[]
			{
				serverName,
				timeoutInSeconds,
				context
			});
		}

		public static LocalizedString IPv6NetworksHasDuplicateEntries(string duplicate)
		{
			return new LocalizedString("IPv6NetworksHasDuplicateEntries", Strings.ResourceManager, new object[]
			{
				duplicate
			});
		}

		public static LocalizedString NoInstancesFoundForManagementPath(string path)
		{
			return new LocalizedString("NoInstancesFoundForManagementPath", Strings.ResourceManager, new object[]
			{
				path
			});
		}

		public static LocalizedString GroupStateFailed
		{
			get
			{
				return new LocalizedString("GroupStateFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClusterApiErrorMessage(string method, int error, string message)
		{
			return new LocalizedString("ClusterApiErrorMessage", Strings.ResourceManager, new object[]
			{
				method,
				error,
				message
			});
		}

		public static LocalizedString ClusCommonValidationFailedException(string error)
		{
			return new LocalizedString("ClusCommonValidationFailedException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ResPropTypeNotSupportedException(string propType)
		{
			return new LocalizedString("ResPropTypeNotSupportedException", Strings.ResourceManager, new object[]
			{
				propType
			});
		}

		public static LocalizedString DxStorePropertyNotFoundException(string propertyName)
		{
			return new LocalizedString("DxStorePropertyNotFoundException", Strings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString GroupStateUnknown
		{
			get
			{
				return new LocalizedString("GroupStateUnknown", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupStateOnline
		{
			get
			{
				return new LocalizedString("GroupStateOnline", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupStateOffline
		{
			get
			{
				return new LocalizedString("GroupStateOffline", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IPv4AddressesHasDuplicateEntries(string duplicate)
		{
			return new LocalizedString("IPv4AddressesHasDuplicateEntries", Strings.ResourceManager, new object[]
			{
				duplicate
			});
		}

		public static LocalizedString FailToOnlineClusResourceException(string groupName, string resourceId, string reason)
		{
			return new LocalizedString("FailToOnlineClusResourceException", Strings.ResourceManager, new object[]
			{
				groupName,
				resourceId,
				reason
			});
		}

		public static LocalizedString DxStoreKeyApiOperationException(string operationName, string keyName)
		{
			return new LocalizedString("DxStoreKeyApiOperationException", Strings.ResourceManager, new object[]
			{
				operationName,
				keyName
			});
		}

		public static LocalizedString CouldNotFindServerObject(string serverName)
		{
			return new LocalizedString("CouldNotFindServerObject", Strings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString InvalidSubnetSpec(string userInput)
		{
			return new LocalizedString("InvalidSubnetSpec", Strings.ResourceManager, new object[]
			{
				userInput
			});
		}

		public static LocalizedString ClusterException(string clusterError)
		{
			return new LocalizedString("ClusterException", Strings.ResourceManager, new object[]
			{
				clusterError
			});
		}

		public static LocalizedString ClusterNoServerToConnect(string dagName)
		{
			return new LocalizedString("ClusterNoServerToConnect", Strings.ResourceManager, new object[]
			{
				dagName
			});
		}

		public static LocalizedString ClusterNotRunningException
		{
			get
			{
				return new LocalizedString("ClusterNotRunningException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteClusterWin2k8ToWin2k3NotSupportedException
		{
			get
			{
				return new LocalizedString("RemoteClusterWin2k8ToWin2k3NotSupportedException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSuchNetwork(string netName)
		{
			return new LocalizedString("NoSuchNetwork", Strings.ResourceManager, new object[]
			{
				netName
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(11);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Cluster.Shared.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			FixUpIpAddressStatusUnchanged = 1578974238U,
			RemoteClusterWin2k3ToWin2k8NotSupportedException = 4106740448U,
			GroupStatePending = 3232505775U,
			NoErrorSpecified = 801548279U,
			GroupStatePartialOnline = 487199380U,
			GroupStateFailed = 3972288899U,
			GroupStateUnknown = 2874666956U,
			GroupStateOnline = 2356112387U,
			GroupStateOffline = 344705101U,
			ClusterNotRunningException = 330900787U,
			RemoteClusterWin2k8ToWin2k3NotSupportedException = 4244700832U
		}

		private enum ParamIDs
		{
			IpResourceCreationOnWrongTypeOfNetworkException,
			ClusCommonRetryableTransientException,
			OfflineOperationTimedOutException,
			RequestedNetworkIsNotDhcpEnabled,
			FailedToFindNetwork,
			DxStoreKeyApiFailedMessage,
			ClusCommonFailException,
			ClusResourceAlreadyExistsException,
			ClusCommonNonRetryableTransientException,
			RegistryParameterKeyNotOpenedException,
			RegistryParameterWriteException,
			RegistryParameterException,
			ClusterFileNotFoundException,
			AmGetFqdnFailedADError,
			ClusCommonTransientException,
			ClusterEvictWithoutCleanupException,
			ClusterNotJoinedException,
			AmCoreGroupRegNotFound,
			IPv4NetworksHasDuplicateEntries,
			AmServerNameResolveFqdnException,
			ClusCommonTaskPendingException,
			ClusterNotInstalledException,
			InvalidResourceOpException,
			ClusterNodeNotFoundException,
			RequestedNetworkIsNotIPv6Enabled,
			ClusterApiException,
			DxStoreKeyNotFoundException,
			RegistryParameterReadException,
			DxStoreKeyInvalidKeyException,
			DagTaskNotEnoughStaticIPAddresses,
			ClusteringNotInstalledOnLHException,
			OperationValidOnlyOnLonghornException,
			ClusterUnsupportedRegistryTypeException,
			FixUpIpAddressStatusUpdated,
			NoClusResourceFoundException,
			AmGetFqdnFailedNotFound,
			FailToOfflineClusResourceException,
			ClusterNodeJoinedException,
			ClusterDatabaseTransientException,
			OpenClusterTimedoutException,
			IPv6NetworksHasDuplicateEntries,
			NoInstancesFoundForManagementPath,
			ClusterApiErrorMessage,
			ClusCommonValidationFailedException,
			ResPropTypeNotSupportedException,
			DxStorePropertyNotFoundException,
			IPv4AddressesHasDuplicateEntries,
			FailToOnlineClusResourceException,
			DxStoreKeyApiOperationException,
			CouldNotFindServerObject,
			InvalidSubnetSpec,
			ClusterException,
			ClusterNoServerToConnect,
			NoSuchNetwork
		}
	}
}

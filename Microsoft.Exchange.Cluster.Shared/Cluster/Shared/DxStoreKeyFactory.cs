using System;
using System.Collections.Concurrent;
using System.ServiceModel.Description;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class DxStoreKeyFactory
	{
		public DxStoreKeyFactory(string componentName, Func<Exception, Exception> exceptionTranslator, DxStoreRegistryConfigProvider configProvider = null, string groupName = null, string self = null, bool isZeroboxMode = false)
		{
			this.ExceptionTranslator = exceptionTranslator;
			if (configProvider == null)
			{
				configProvider = new DistributedStore.DxStoreRegistryProviderWithVariantConfig();
				configProvider.Initialize(componentName, self, null, null, isZeroboxMode);
			}
			this.ConfigProvider = configProvider;
			this.GroupConfig = configProvider.GetGroupConfig(groupName, true);
			if (this.GroupConfig.Settings.IsUseHttpTransportForClientCommunication)
			{
				HttpConfiguration.Configure(this.GroupConfig);
			}
		}

		public DxStoreRegistryConfigProvider ConfigProvider { get; set; }

		public InstanceGroupConfig GroupConfig { get; set; }

		public Func<Exception, Exception> ExceptionTranslator { get; set; }

		public void RunOperationAndTranslateException(OperationCategory operationCategory, string keyName, Action action)
		{
			this.RunOperationAndTranslateException<int>(operationCategory, keyName, delegate()
			{
				action();
				return 0;
			}, false);
		}

		public T RunOperationAndTranslateException<T>(OperationCategory operationCategory, string keyName, Func<T> action, bool isBestEffort = false)
		{
			try
			{
				return action();
			}
			catch (Exception innerException)
			{
				if (!isBestEffort)
				{
					DxStoreKeyApiOperationException ex = new DxStoreKeyApiOperationException(operationCategory.ToString(), keyName ?? string.Empty, innerException);
					if (this.ExceptionTranslator != null)
					{
						throw this.ExceptionTranslator(ex);
					}
					throw ex;
				}
			}
			return default(T);
		}

		public CachedChannelFactory<IDxStoreAccess> GetFactory(string nodeName = null, WcfTimeout wcfTimeout = null)
		{
			if (this.GroupConfig.Settings.IsUseHttpTransportForClientCommunication)
			{
				return null;
			}
			CachedChannelFactory<IDxStoreAccess> cachedChannelFactory = null;
			bool flag = false;
			if (this.IsSelf(nodeName))
			{
				cachedChannelFactory = this.localChannelFactory;
				flag = true;
			}
			if (cachedChannelFactory == null)
			{
				ServiceEndpoint storeAccessEndpoint = EndpointBuilder.GetStoreAccessEndpoint(this.GroupConfig, nodeName, this.IsDefaultGroupIdentifier(this.GroupConfig.Name), false, wcfTimeout);
				cachedChannelFactory = this.GetFactoryByEndPoint(storeAccessEndpoint);
			}
			if (flag && this.localChannelFactory == null)
			{
				this.localChannelFactory = cachedChannelFactory;
			}
			return cachedChannelFactory;
		}

		public IDistributedStoreKey GetBaseKey(DxStoreKeyAccessMode mode, CachedChannelFactory<IDxStoreAccess> channelFactory = null, string nodeName = null, bool isPrivate = false)
		{
			return this.RunOperationAndTranslateException<DxStoreKey>(OperationCategory.GetBaseKey, string.Empty, delegate()
			{
				channelFactory = (channelFactory ?? this.GetFactory(nodeName, null));
				IDxStoreAccessClient dxStoreAccessClient;
				if (this.GroupConfig.Settings.IsUseHttpTransportForClientCommunication)
				{
					dxStoreAccessClient = new HttpStoreAccessClient(this.GroupConfig.Self, HttpClient.TargetInfo.BuildFromNode(nodeName, this.GroupConfig), this.GroupConfig.Settings.StoreAccessHttpTimeoutInMSec);
				}
				else
				{
					dxStoreAccessClient = new WcfStoreAccessClient(channelFactory, null);
				}
				DxStoreAccessRequest.CheckKey checkKey = new DxStoreAccessRequest.CheckKey();
				checkKey.Initialize(string.Empty, isPrivate, this.ConfigProvider.Self);
				DxStoreAccessReply.CheckKey checkKey2 = dxStoreAccessClient.CheckKey(checkKey, null);
				if (checkKey2.ReadResult.IsStale)
				{
					throw new DxStoreInstanceStaleStoreException();
				}
				if (!checkKey2.IsExist)
				{
					throw new DxStoreKeyNotFoundException(string.Empty);
				}
				DxStoreKey.BaseKeyParameters baseParameters = new DxStoreKey.BaseKeyParameters
				{
					Client = dxStoreAccessClient,
					KeyFactory = this,
					Self = this.ConfigProvider.Self,
					IsPrivate = isPrivate,
					DefaultReadOptions = new ReadOptions(),
					DefaultWriteOptions = new WriteOptions()
				};
				return new DxStoreKey(string.Empty, mode, baseParameters);
			}, false);
		}

		private CachedChannelFactory<IDxStoreAccess> GetFactoryByEndPoint(ServiceEndpoint serviceEndPoint)
		{
			return this.factoryByEndPoint.GetOrAdd(serviceEndPoint, (ServiceEndpoint e) => new CachedChannelFactory<IDxStoreAccess>(e));
		}

		private bool IsSelf(string nodeName)
		{
			return string.IsNullOrEmpty(nodeName) || Utils.IsEqual(nodeName, this.ConfigProvider.Self, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsDefaultGroupIdentifier(string groupName)
		{
			return string.IsNullOrEmpty(groupName) || Utils.IsEqual(groupName, "B1563499-EA40-4101-A9E6-59A8EB26FF1E", StringComparison.OrdinalIgnoreCase);
		}

		private ConcurrentDictionary<ServiceEndpoint, CachedChannelFactory<IDxStoreAccess>> factoryByEndPoint = new ConcurrentDictionary<ServiceEndpoint, CachedChannelFactory<IDxStoreAccess>>();

		private CachedChannelFactory<IDxStoreAccess> localChannelFactory;
	}
}

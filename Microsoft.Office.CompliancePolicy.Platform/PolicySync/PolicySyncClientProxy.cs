using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public sealed class PolicySyncClientProxy : IPolicySyncWebserviceClient, IDisposable
	{
		private PolicySyncClientProxy(EndpointAddress endpointAddress, ICredentials credentials, string partnerName, ExecutionLog logProvider, bool enableGzipCompression)
		{
			string text = endpointAddress.Uri.AbsoluteUri.TrimEnd(new char[]
			{
				'/'
			}) + '/';
			if (text.EndsWith("/soap/"))
			{
				int length = text.Length - "soap/".Length;
				text = text.Substring(0, length);
			}
			this.syncEndpointBaseUri = new Uri(text);
			this.partnerName = partnerName;
			this.httpClient = new HttpClient(logProvider);
			this.sessionConfig.ContentType = "application/json";
			this.sessionConfig.Accept = "application/json";
			this.sessionConfig.Method = "POST";
			this.sessionConfig.Headers = new WebHeaderCollection();
			this.sessionConfig.ReadWebExceptionResponseStream = true;
			this.sessionConfig.MaximumResponseBodyLength = 26214400L;
			if (enableGzipCompression)
			{
				this.sessionConfig.Headers["Accept-Encoding"] = "gzip";
			}
			if (credentials != null)
			{
				this.sessionConfig.Credentials = credentials;
				this.sessionConfig.Headers["Authorization"] = "Bearer";
				this.sessionConfig.PreAuthenticate = true;
			}
			this.getChangesUri = new Uri(this.syncEndpointBaseUri, "GetChanges");
			this.publishStatusUri = new Uri(this.syncEndpointBaseUri, "PublishStatus");
		}

		public static PolicySyncClientProxy Create(EndpointAddress endpointAddress, ICredentials credentials, string partnerName, ExecutionLog logProvider, bool enableGzipCompression = true)
		{
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ArgumentValidator.ThrowIfNullOrEmpty("partnerName", partnerName);
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			return new PolicySyncClientProxy(endpointAddress, credentials, partnerName, logProvider, enableGzipCompression);
		}

		public PolicyChange GetSingleTenantChanges(TenantCookie tenantCookie)
		{
			ArgumentValidator.ThrowIfNull("tenantCookie", tenantCookie);
			PolicyChange result = null;
			this.CallWithRequestCleanupWrapper(delegate
			{
				this.SetRequestPayload(typeof(GetChangesRequest), this.CreateGetChangesRequest(tenantCookie));
				ICancelableAsyncResult cancelableAsyncResult = this.httpClient.BeginDownload(this.getChangesUri, this.sessionConfig, null, null);
				cancelableAsyncResult.AsyncWaitHandle.WaitOne();
				PolicyChangeBatch policyChangeBatch = this.ProcessDownload<PolicyChangeBatch>(cancelableAsyncResult);
				result = PolicySyncClientProxy.GetPolicyChangeFromBatch(tenantCookie.TenantId, policyChangeBatch);
			});
			return result;
		}

		public IAsyncResult BeginGetSingleTenantChanges(TenantCookie tenantCookie, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("tenantCookie", tenantCookie);
			ArgumentValidator.ThrowIfNull("userCallback must not be null. if calling in synchronous fashion, use GetSingleTenantChanges API instead", userCallback);
			this.SetRequestPayload(typeof(GetChangesRequest), this.CreateGetChangesRequest(tenantCookie));
			this.currentTenantId = tenantCookie.TenantId;
			return this.httpClient.BeginDownload(this.getChangesUri, this.sessionConfig, userCallback, stateObject);
		}

		public PolicyChange EndGetSingleTenantChanges(IAsyncResult asyncResult)
		{
			ICancelableAsyncResult cancelableAsyncResult = asyncResult as ICancelableAsyncResult;
			ArgumentValidator.ThrowIfNull("asyncResult must not be null and must be ICancelableAsyncResult type", asyncResult);
			PolicyChange result = null;
			this.CallWithRequestCleanupWrapper(delegate
			{
				Guid tenantId = this.currentTenantId;
				PolicyChangeBatch policyChangeBatch = this.ProcessDownload<PolicyChangeBatch>(cancelableAsyncResult);
				result = PolicySyncClientProxy.GetPolicyChangeFromBatch(tenantId, policyChangeBatch);
			});
			return result;
		}

		public PolicyChangeBatch GetChanges(GetChangesRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			PolicyChangeBatch result = null;
			this.CallWithRequestCleanupWrapper(delegate
			{
				this.SetRequestPayload(typeof(GetChangesRequest), request);
				ICancelableAsyncResult cancelableAsyncResult = this.httpClient.BeginDownload(this.getChangesUri, this.sessionConfig, null, null);
				cancelableAsyncResult.AsyncWaitHandle.WaitOne();
				result = this.ProcessDownload<PolicyChangeBatch>(cancelableAsyncResult);
			});
			return result;
		}

		public IAsyncResult BeginGetChanges(GetChangesRequest request, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("userCallback must not be null. if calling in synchronous fashion, use GetChanges API instead", userCallback);
			this.SetRequestPayload(typeof(GetChangesRequest), request);
			return this.httpClient.BeginDownload(this.getChangesUri, this.sessionConfig, userCallback, stateObject);
		}

		public PolicyChangeBatch EndGetChanges(IAsyncResult asyncResult)
		{
			ICancelableAsyncResult cancelableAsyncResult = asyncResult as ICancelableAsyncResult;
			ArgumentValidator.ThrowIfNull("asyncResult must not be null and must be ICancelableAsyncResult type", asyncResult);
			PolicyChangeBatch result = null;
			this.CallWithRequestCleanupWrapper(delegate
			{
				result = this.ProcessDownload<PolicyChangeBatch>(cancelableAsyncResult);
			});
			return result;
		}

		public PolicyConfigurationBase GetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects)
		{
			ArgumentValidator.ThrowIfNull("callerContext", callerContext);
			ArgumentValidator.ThrowIfNull("objectType", objectType);
			Uri getObjectUri = this.GetObjectRequestUri(tenantId, objectType, objectId, includeDeletedObjects);
			GetObjectRequest request = new GetObjectRequest
			{
				callerContext = SyncCallerContext.Create(this.partnerName)
			};
			PolicyConfigurationBase getResult = null;
			this.CallWithRequestCleanupWrapper(delegate
			{
				this.SetRequestPayload(typeof(GetObjectRequest), request);
				ICancelableAsyncResult cancelableAsyncResult = this.httpClient.BeginDownload(getObjectUri, this.sessionConfig, null, null);
				cancelableAsyncResult.AsyncWaitHandle.WaitOne();
				getResult = this.ParseGetObjectResult(objectType, cancelableAsyncResult);
			});
			return getResult;
		}

		public IAsyncResult BeginGetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("callerContext", callerContext);
			ArgumentValidator.ThrowIfNull("objectType", objectType);
			Uri objectRequestUri = this.GetObjectRequestUri(tenantId, objectType, objectId, includeDeletedObjects);
			GetObjectRequest request = new GetObjectRequest
			{
				callerContext = SyncCallerContext.Create(this.partnerName)
			};
			this.SetRequestPayload(typeof(GetObjectRequest), request);
			this.currentObjectType = objectType;
			return this.httpClient.BeginDownload(objectRequestUri, this.sessionConfig, userCallback, stateObject);
		}

		public PolicyConfigurationBase EndGetObject(IAsyncResult asyncResult)
		{
			ICancelableAsyncResult cancelableAsyncResult = asyncResult as ICancelableAsyncResult;
			ArgumentValidator.ThrowIfNull("asyncResult must not be null and must be ICancelableAsyncResult type", asyncResult);
			PolicyConfigurationBase getResult = null;
			this.CallWithRequestCleanupWrapper(delegate
			{
				getResult = this.ParseGetObjectResult(this.currentObjectType, cancelableAsyncResult);
			});
			return getResult;
		}

		public void PublishStatus(PublishStatusRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			this.CallWithRequestCleanupWrapper(delegate
			{
				this.SetRequestPayload(typeof(PublishStatusRequest), request);
				ICancelableAsyncResult cancelableAsyncResult = this.httpClient.BeginDownload(this.publishStatusUri, this.sessionConfig, null, null);
				cancelableAsyncResult.AsyncWaitHandle.WaitOne();
				this.ProcessDownload<PolicySyncClientProxy.EmptyResultType>(cancelableAsyncResult);
			});
		}

		public IAsyncResult BeginPublishStatus(PublishStatusRequest request, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			ArgumentValidator.ThrowIfNull("userCallback must not be null. if calling in sync fashion, use PublishStatus API instead", userCallback);
			this.SetRequestPayload(typeof(PublishStatusRequest), request);
			return this.httpClient.BeginDownload(this.publishStatusUri, this.sessionConfig, userCallback, stateObject);
		}

		public void EndPublishStatus(IAsyncResult asyncResult)
		{
			ICancelableAsyncResult cancelableAsyncResult = asyncResult as ICancelableAsyncResult;
			ArgumentValidator.ThrowIfNull("asyncResult must not be null and must be ICancelableAsyncResult type", asyncResult);
			this.CallWithRequestCleanupWrapper(delegate
			{
				this.ProcessDownload<PolicySyncClientProxy.EmptyResultType>(cancelableAsyncResult);
			});
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static T DeserializeResult<T>(Stream resultStream)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
			return (T)((object)dataContractJsonSerializer.ReadObject(resultStream));
		}

		private static void ProcessFailedDownload(DownloadResult downloadResult)
		{
			if (downloadResult.StatusCode == HttpStatusCode.BadRequest && downloadResult.ResponseStream != null)
			{
				SyncAgentExceptionBase syncAgentExceptionBase = null;
				try
				{
					PolicySyncTransientFaultResult policySyncTransientFaultResult = PolicySyncClientProxy.DeserializeResult<PolicySyncTransientFaultResult>(downloadResult.ResponseStream);
					if (policySyncTransientFaultResult != null && policySyncTransientFaultResult.Detail != null)
					{
						syncAgentExceptionBase = new SyncAgentTransientException(policySyncTransientFaultResult.Detail.Message, policySyncTransientFaultResult.Detail.IsPerObjectException, SyncAgentErrorCode.Generic);
					}
				}
				catch (Exception)
				{
					try
					{
						downloadResult.ResponseStream.Position = 0L;
						PolicySyncPermanentFaultResult policySyncPermanentFaultResult = PolicySyncClientProxy.DeserializeResult<PolicySyncPermanentFaultResult>(downloadResult.ResponseStream);
						if (policySyncPermanentFaultResult != null && policySyncPermanentFaultResult.Detail != null)
						{
							syncAgentExceptionBase = new SyncAgentPermanentException(policySyncPermanentFaultResult.Detail.Message, policySyncPermanentFaultResult.Detail.IsPerObjectException, SyncAgentErrorCode.Generic);
						}
					}
					catch (Exception)
					{
					}
				}
				if (syncAgentExceptionBase != null)
				{
					throw syncAgentExceptionBase;
				}
			}
			if (downloadResult.IsRetryable)
			{
				throw new SyncAgentTransientException(downloadResult.Exception.Message, downloadResult.Exception, false, SyncAgentErrorCode.Generic);
			}
			throw new SyncAgentPermanentException(downloadResult.Exception.Message, downloadResult.Exception, false, SyncAgentErrorCode.Generic);
		}

		private static PolicyChange GetPolicyChangeFromBatch(Guid tenantId, PolicyChangeBatch policyChangeBatch)
		{
			PolicyChange policyChange = null;
			if (policyChangeBatch != null)
			{
				policyChange = new PolicyChange();
				policyChange.Changes = policyChangeBatch.Changes;
				TenantCookie newCookie = null;
				if (policyChangeBatch.NewCookies != null)
				{
					policyChangeBatch.NewCookies.TryGetCookie(tenantId, out newCookie);
				}
				policyChange.NewCookie = newCookie;
			}
			return policyChange;
		}

		private void Dispose(bool disposing)
		{
			if (this.disposed && disposing && this.httpClient != null)
			{
				this.httpClient.Dispose();
			}
			this.disposed = true;
		}

		private T ProcessDownload<T>(ICancelableAsyncResult asyncResult)
		{
			T result = default(T);
			DownloadResult downloadResult = null;
			try
			{
				downloadResult = this.httpClient.EndDownload(asyncResult);
				if (downloadResult.IsSucceeded)
				{
					result = this.ProcessSuccessfulDownload<T>(downloadResult);
				}
				else
				{
					PolicySyncClientProxy.ProcessFailedDownload(downloadResult);
				}
			}
			finally
			{
				if (downloadResult.ResponseStream != null)
				{
					downloadResult.ResponseStream.Dispose();
					downloadResult.ResponseStream = null;
				}
			}
			return result;
		}

		private T ProcessSuccessfulDownload<T>(DownloadResult downloadResult)
		{
			T result = default(T);
			if (typeof(T) == typeof(PolicySyncClientProxy.EmptyResultType))
			{
				return result;
			}
			string text = downloadResult.ResponseHeaders[HttpResponseHeader.ContentType];
			if (string.IsNullOrEmpty(text) || text.IndexOfAny("application/json".ToCharArray()) == -1)
			{
				throw new SyncAgentPermanentException(string.Format(CultureInfo.InvariantCulture, "Expect the returned content type header to be 'application/json' but got {0} instead", new object[]
				{
					text
				}));
			}
			if (downloadResult.ResponseStream == null || downloadResult.ResponseStream.Length == 0L)
			{
				throw new SyncAgentPermanentException("Unexpected response stream being empty");
			}
			downloadResult.ResponseStream.Position = 0L;
			string text2 = downloadResult.ResponseHeaders[HttpResponseHeader.ContentEncoding];
			if (!string.IsNullOrEmpty(text2) && text2.Equals("gzip", StringComparison.OrdinalIgnoreCase))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (GZipStream gzipStream = new GZipStream(downloadResult.ResponseStream, CompressionMode.Decompress, true))
					{
						byte[] array = new byte[4096];
						for (;;)
						{
							int num = gzipStream.Read(array, 0, array.Length);
							if (num == 0)
							{
								break;
							}
							memoryStream.Write(array, 0, num);
						}
					}
					memoryStream.Flush();
					memoryStream.Position = 0L;
					return PolicySyncClientProxy.DeserializeResult<T>(memoryStream);
				}
			}
			return PolicySyncClientProxy.DeserializeResult<T>(downloadResult.ResponseStream);
		}

		private void CallWithRequestCleanupWrapper(Action action)
		{
			try
			{
				action();
			}
			finally
			{
				if (this.sessionConfig.RequestStream != null)
				{
					this.sessionConfig.RequestStream.Dispose();
					this.sessionConfig.RequestStream = null;
				}
				this.currentTenantId = Guid.Empty;
			}
		}

		private void SetRequestPayload(Type requestType, object request)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(requestType);
			MemoryStream memoryStream = new MemoryStream();
			dataContractJsonSerializer.WriteObject(memoryStream, request);
			memoryStream.Flush();
			memoryStream.Position = 0L;
			this.sessionConfig.RequestStream = memoryStream;
		}

		private GetChangesRequest CreateGetChangesRequest(TenantCookie tenantCookie)
		{
			TenantCookieCollection tenantCookieCollection = new TenantCookieCollection(tenantCookie.Workload, tenantCookie.ObjectType);
			tenantCookieCollection[tenantCookie.TenantId] = tenantCookie;
			return new GetChangesRequest
			{
				CallerContext = SyncCallerContext.Create(this.partnerName),
				TenantCookies = tenantCookieCollection
			};
		}

		private Uri GetObjectRequestUri(Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects)
		{
			string relativeUri = string.Format("GetObject?tenantId={0}&objectType={1}&objectId={2}&includeDeletedObjects={3}", new object[]
			{
				tenantId,
				objectType,
				objectId,
				includeDeletedObjects
			});
			return new Uri(this.syncEndpointBaseUri, relativeUri);
		}

		private PolicyConfigurationBase ParseGetObjectResult(ConfigurationObjectType objectType, ICancelableAsyncResult asyncResult)
		{
			PolicyConfigurationBase result = null;
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
			{
				GetObjectResultForPolicy getObjectResultForPolicy = this.ProcessDownload<GetObjectResultForPolicy>(asyncResult);
				result = getObjectResultForPolicy.GetObjectResult;
				break;
			}
			case ConfigurationObjectType.Rule:
			{
				GetObjectResultForRule getObjectResultForRule = this.ProcessDownload<GetObjectResultForRule>(asyncResult);
				result = getObjectResultForRule.GetObjectResult;
				break;
			}
			case ConfigurationObjectType.Association:
			{
				GetObjectResultForAssociation getObjectResultForAssociation = this.ProcessDownload<GetObjectResultForAssociation>(asyncResult);
				result = getObjectResultForAssociation.GetObjectResult;
				break;
			}
			case ConfigurationObjectType.Binding:
			{
				GetObjectResultForBinding getObjectResultForBinding = this.ProcessDownload<GetObjectResultForBinding>(asyncResult);
				result = getObjectResultForBinding.GetObjectResult;
				break;
			}
			}
			return result;
		}

		private readonly HttpClient httpClient;

		private readonly HttpSessionConfig sessionConfig = new HttpSessionConfig();

		private readonly Uri syncEndpointBaseUri;

		private readonly string partnerName;

		private readonly Uri getChangesUri;

		private readonly Uri publishStatusUri;

		private Guid currentTenantId;

		private ConfigurationObjectType currentObjectType;

		private bool disposed;

		private sealed class EmptyResultType
		{
		}
	}
}

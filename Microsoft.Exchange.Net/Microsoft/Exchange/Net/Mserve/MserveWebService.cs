using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Mserve;
using Microsoft.Exchange.Net.Mserve.ProvisionRequest;
using Microsoft.Exchange.Net.Mserve.ProvisionResponse;
using Microsoft.Exchange.Net.Mserve.SettingsRequest;
using Microsoft.Exchange.Net.Mserve.SettingsResponse;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net.Mserve
{
	internal class MserveWebService : IMserveService
	{
		public MserveWebService(string provisionUrl, string settingsUrl, string remoteCertSubject, string clientToken, bool batchMode)
		{
			if (string.IsNullOrEmpty(provisionUrl))
			{
				throw new ArgumentNullException("provisionUrl");
			}
			if (string.IsNullOrEmpty(settingsUrl))
			{
				throw new ArgumentNullException("settingsUrl");
			}
			if (string.IsNullOrEmpty(clientToken))
			{
				throw new ArgumentNullException("clientToken");
			}
			this.isMicrosoftHostedOnly = Datacenter.IsMicrosoftHostedOnly(true);
			this.remoteCertSubject = remoteCertSubject;
			this.batchMode = batchMode;
			string query = string.Format(CultureInfo.InvariantCulture, "Dspk={0}", new object[]
			{
				clientToken
			});
			this.provisionUriWithoutQuery = new Uri(provisionUrl);
			this.provisionUri = new UriBuilder(this.provisionUriWithoutQuery)
			{
				Query = query
			}.Uri;
			this.settingUri = new UriBuilder(new Uri(settingsUrl))
			{
				Query = query
			}.Uri;
			this.perfCounters = new MservePerfCounters();
		}

		public string LastResponseDiagnosticInfo { get; private set; }

		public string LastResponseTransactionId { get; private set; }

		public string LastIpUsed { get; private set; }

		public bool TrackDuplicatedAddEntries
		{
			get
			{
				return this.trackDuplicatedAddEntries;
			}
			set
			{
				this.trackDuplicatedAddEntries = value;
			}
		}

		internal static MserveCacheServiceMode CurrentMserveCacheServiceMode
		{
			get
			{
				int tickCount = Environment.TickCount;
				if (MserveWebService.whenRegKeyLastChecked == 0 || TickDiffer.Elapsed(MserveWebService.whenRegKeyLastChecked, tickCount) > MserveWebService.MserveCacheServiceRegistryCheckInterval)
				{
					lock (MserveWebService.modeLockObject)
					{
						MserveCacheServiceMode mserveCacheServiceMode = MserveWebService.currentMserveCacheServiceMode;
						try
						{
							using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
							{
								if (registryKey != null)
								{
									int num = (int)registryKey.GetValue("MserveCacheServiceEnabled", 0);
									if (Enum.IsDefined(typeof(MserveCacheServiceMode), num))
									{
										MserveWebService.currentMserveCacheServiceMode = (MserveCacheServiceMode)num;
										if (MserveWebService.currentMserveCacheServiceMode != mserveCacheServiceMode)
										{
											MserveCacheServiceProvider.EventLog.LogEvent(CommonEventLogConstants.Tuple_MserveCacheServiceModeChanged, "MserveCacheServiceModeChanged", new object[]
											{
												mserveCacheServiceMode.ToString(),
												MserveWebService.currentMserveCacheServiceMode.ToString()
											});
										}
									}
									else
									{
										MserveWebService.currentMserveCacheServiceMode = MserveCacheServiceMode.NotEnabled;
									}
								}
								else
								{
									MserveWebService.currentMserveCacheServiceMode = MserveCacheServiceMode.NotEnabled;
								}
							}
						}
						catch (Exception ex)
						{
							ExTraceGlobals.MserveCacheServiceTracer.TraceError<string>(0L, "Exception occured while reading MserveCacheServiceMode from registry. Exception : {0}", ex.ToString());
							MserveWebService.currentMserveCacheServiceMode = MserveCacheServiceMode.NotEnabled;
						}
						if (mserveCacheServiceMode != MserveWebService.currentMserveCacheServiceMode)
						{
							ExTraceGlobals.MserveCacheServiceTracer.TraceDebug<MserveCacheServiceMode, MserveCacheServiceMode>(0L, "The MserveCacheServiceMode has changed from {0} to {1}", mserveCacheServiceMode, MserveWebService.currentMserveCacheServiceMode);
						}
						MserveWebService.whenRegKeyLastChecked = Environment.TickCount;
					}
				}
				return MserveWebService.currentMserveCacheServiceMode;
			}
			set
			{
				lock (MserveWebService.modeLockObject)
				{
					MserveWebService.currentMserveCacheServiceMode = value;
				}
			}
		}

		public static bool IsTransientException(MserveException exception)
		{
			return exception.InnerException != null && (exception.InnerException is WebException || exception.InnerException is IOException || exception.InnerException is TransientException);
		}

		public void Initialize()
		{
			this.Initialize(0);
		}

		public void Initialize(int initialChunkSize)
		{
			lock (MserveWebService.mutex)
			{
				if (!MserveWebService.initialized)
				{
					CertificateValidationManager.RegisterCallback("MserveWebService", new RemoteCertificateValidationCallback(MserveWebService.ValidateCertificate));
					MserveWebService.staticRemoteCertSubject = this.remoteCertSubject;
				}
				if (initialChunkSize == 0)
				{
					if (MserveWebService.chunkSize == 0)
					{
						MserveWebService.chunkSize = this.FetchChunkSize();
					}
				}
				else
				{
					MserveWebService.chunkSize = initialChunkSize;
				}
				MserveWebService.initialized = true;
			}
		}

		public void SetConnectionLeaseTimeout(int timeout)
		{
			Uri[] array = new Uri[]
			{
				this.provisionUri,
				this.settingUri
			};
			foreach (Uri address in array)
			{
				try
				{
					ServicePoint servicePoint = ServicePointManager.FindServicePoint(address);
					servicePoint.ConnectionLeaseTimeout = timeout;
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		public ICancelableAsyncResult BeginReadPartnerId(string address, CancelableAsyncCallback callback, object state)
		{
			int num = -1;
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentNullException("address");
			}
			Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provision = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision();
			provision.Delete = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType[1];
			provision.Add = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType[1];
			provision.Read = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType[1];
			Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType accountType = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType();
			accountType.Name = address;
			provision.Read[0] = accountType;
			ExTraceGlobals.DeltaSyncAPITracer.TraceDebug<string>((long)this.GetHashCode(), "Read entry {0}", address);
			if (this.UseOfflineMserveCacheServiceFirst() || this.UseOfflineMserveCacheService())
			{
				Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provision2 = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision();
				provision2.Responses = new ProvisionResponses();
				CommandStatusCode commandStatusCode = this.ProcessReadRequest(provision, provision2);
				if (commandStatusCode == CommandStatusCode.Success)
				{
					provision2.Status = 1;
					this.ProcessOneOffReadResponse(provision2, out num);
					ExTraceGlobals.MserveCacheServiceTracer.TraceDebug<int>((long)this.GetHashCode(), "Processing one-off read request in BeginReadPartnerId() for Mserve Cache. PartnerId = {0}", num);
				}
			}
			HttpSessionConfig httpSessionConfig = this.InitializeHttpSessionConfig(true);
			httpSessionConfig.RequestStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(httpSessionConfig.RequestStream, Encoding.UTF8);
			MserveWebService.provRequestSerializer.Serialize(streamWriter, provision);
			httpSessionConfig.RequestStream.Position = 0L;
			this.LogProvisionRequest(this.provisionUriWithoutQuery.ToString());
			HttpClient httpClient = new HttpClient();
			OutstandingAsyncReadConfig outstandingAsyncReadConfig = new OutstandingAsyncReadConfig(httpClient, httpSessionConfig, streamWriter, callback, num, state);
			new CancelableHttpAsyncResult(callback, state, httpClient);
			ICancelableAsyncResult internalResult = httpClient.BeginDownload(this.provisionUri, httpSessionConfig, MserveWebService.asyncApiCallback, outstandingAsyncReadConfig);
			return new CancelableMservAsyncResult(internalResult, outstandingAsyncReadConfig, state);
		}

		public int EndReadPartnerId(ICancelableAsyncResult asyncResult)
		{
			CancelableMservAsyncResult cancelableMservAsyncResult = (CancelableMservAsyncResult)asyncResult;
			int result = -1;
			Exception ex = null;
			string text = string.Empty;
			Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provision = null;
			int result2;
			try
			{
				this.perfCounters.UpdateTotalRequestPerfCounters(1);
				if (this.UseOfflineMserveCacheServiceFirst() && cancelableMservAsyncResult.ReadConfig.CachePartnerId != -1)
				{
					result2 = cancelableMservAsyncResult.ReadConfig.CachePartnerId;
				}
				else
				{
					if (this.UseRealMserveWebService())
					{
						this.perfCounters.UpdateRequestPerfCountersForMserveWebService(1, 0, 0);
						DownloadResult downloadResult = cancelableMservAsyncResult.ReadConfig.Client.EndDownload(cancelableMservAsyncResult.InternalResult);
						if (downloadResult.IsSucceeded)
						{
							Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponse = null;
							StreamReader streamReader = new StreamReader(downloadResult.ResponseStream, Encoding.UTF8);
							try
							{
								provResponse = (Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision)MserveWebService.provResponseSerializer.Deserialize(streamReader);
							}
							catch (InvalidOperationException ex2)
							{
								ex = ((ex2.InnerException != null) ? ex2.InnerException : ex2);
								text = "Failure during deserialization";
							}
							finally
							{
								if (streamReader != null)
								{
									streamReader.Close();
								}
								if (downloadResult.ResponseStream != null)
								{
									downloadResult.ResponseStream.Close();
									downloadResult.ResponseStream = null;
								}
							}
							if (ex == null)
							{
								this.ProcessOneOffReadResponse(provResponse, out result);
								return result;
							}
						}
						else
						{
							ex = downloadResult.Exception;
							text = "Exception during Mserve lookup";
						}
					}
					if (ex != null)
					{
						this.perfCounters.UpdateFailurePerfCountersForMserveWebService(1);
					}
					if (this.UseOfflineMserveCacheService() && cancelableMservAsyncResult.ReadConfig.CachePartnerId != -1)
					{
						result2 = cancelableMservAsyncResult.ReadConfig.CachePartnerId;
					}
					else
					{
						if (provision != null && provision.Read != null && provision.Read.Length > 0)
						{
							this.CleanupQueueAndResultSet(provision.Read, 1);
						}
						this.perfCounters.UpdateTotalFailuresPerfCounters(1);
						if (!string.IsNullOrEmpty(text))
						{
							throw new MserveException(text, ex);
						}
						result2 = -1;
					}
				}
			}
			finally
			{
				cancelableMservAsyncResult.ReadConfig.XmlStreamWriter.Dispose();
				cancelableMservAsyncResult.ReadConfig.Client.Dispose();
				cancelableMservAsyncResult.ReadConfig.SessionConfig.RequestStream.Dispose();
			}
			return result2;
		}

		public List<RecipientSyncOperation> Synchronize(List<RecipientSyncOperation> opList)
		{
			foreach (RecipientSyncOperation op in opList)
			{
				this.QueueOperation(op);
			}
			List<RecipientSyncOperation> result = new List<RecipientSyncOperation>(this.completedResultSet);
			this.completedResultSet.Clear();
			return result;
		}

		public List<RecipientSyncOperation> Synchronize(RecipientSyncOperation op)
		{
			this.QueueOperation(op);
			List<RecipientSyncOperation> result = new List<RecipientSyncOperation>(this.completedResultSet);
			this.completedResultSet.Clear();
			return result;
		}

		public List<RecipientSyncOperation> Synchronize()
		{
			return this.Synchronize(true, null);
		}

		public List<RecipientSyncOperation> Synchronize(bool keepAlive, int? timeout)
		{
			this.Flush(keepAlive, timeout);
			List<RecipientSyncOperation> list = new List<RecipientSyncOperation>(this.resultSet);
			list.AddRange(this.completedResultSet);
			this.Reset();
			return list;
		}

		public void Reset()
		{
			this.pendingQueue.Clear();
			this.resultSet.Clear();
			this.completedResultSet.Clear();
		}

		private static void AsyncApiCallback(ICancelableAsyncResult result)
		{
			OutstandingAsyncReadConfig outstandingAsyncReadConfig = (OutstandingAsyncReadConfig)result.AsyncState;
			CancelableMservAsyncResult asyncResult = new CancelableMservAsyncResult(result, outstandingAsyncReadConfig, outstandingAsyncReadConfig.ClientState);
			outstandingAsyncReadConfig.ClientCallback(asyncResult);
		}

		private static bool IsRetryableDeltaSyncError(int status)
		{
			return status >= 5000;
		}

		private HttpSessionConfig InitializeHttpSessionConfig(bool keepAlive)
		{
			HttpSessionConfig httpSessionConfig = new HttpSessionConfig(60000);
			httpSessionConfig.Method = "POST";
			httpSessionConfig.UserAgent = "ExchangeHostedServices/1.0";
			httpSessionConfig.ContentType = "text/xml";
			httpSessionConfig.Headers = new WebHeaderCollection();
			httpSessionConfig.Headers[CertificateValidationManager.ComponentIdHeaderName] = "MserveWebService";
			httpSessionConfig.KeepAlive = keepAlive;
			return httpSessionConfig;
		}

		private void Flush(bool keepAlive, int? timeout)
		{
			Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provision = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision();
			ExTraceGlobals.DeltaSyncAPITracer.TraceDebug<int, int>((long)this.GetHashCode(), "About to flush. The pending queue count is {0} and chunk size is {1}", this.pendingQueue.Count, MserveWebService.chunkSize);
			while (this.pendingQueue.Count > 0)
			{
				provision.Delete = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType[MserveWebService.chunkSize];
				provision.Add = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType[MserveWebService.chunkSize];
				provision.Read = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType[MserveWebService.chunkSize];
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				foreach (KeyValuePair<string, RecipientPendingOperation> keyValuePair in this.pendingQueue)
				{
					Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType accountType = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType();
					accountType.Name = keyValuePair.Key;
					switch (keyValuePair.Value.Type)
					{
					case OperationType.Add:
						accountType.PartnerID = keyValuePair.Value.RecipientSyncOperation.PartnerId.ToString(CultureInfo.InvariantCulture);
						provision.Add[num2++] = accountType;
						this.LogProvisionAccountDetail("Add", accountType);
						break;
					case OperationType.Delete:
						accountType.PartnerID = keyValuePair.Value.RecipientSyncOperation.PartnerId.ToString(CultureInfo.InvariantCulture);
						provision.Delete[num++] = accountType;
						this.LogProvisionAccountDetail("Delete", accountType);
						break;
					case OperationType.Read:
						provision.Read[num3++] = accountType;
						this.LogProvisionAccountDetail("Read", accountType);
						break;
					default:
						throw new MserveException(string.Concat(new object[]
						{
							"Synchronization failed when preparing request for ",
							accountType.Name,
							" because of unknown type ",
							keyValuePair.Value.Type
						}), null);
					}
					num4++;
					ExTraceGlobals.DeltaSyncAPITracer.TraceDebug<OperationType, string, string>((long)this.GetHashCode(), "{0} entry {1} with partner Id {2}", keyValuePair.Value.Type, accountType.Name, accountType.PartnerID);
					if (num4 == MserveWebService.chunkSize)
					{
						break;
					}
				}
				int num5 = this.pendingQueue.Count - num4;
				Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponse = this.SendProvisionRequest(provision, keepAlive, timeout);
				if (!this.ProcessProvisionResponse(provResponse))
				{
					ExTraceGlobals.DeltaSyncAPITracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "Retrying request.  Attempted to send {0}, pending queue count is {1}, chunk size is {2}", num4, this.pendingQueue.Count, MserveWebService.chunkSize);
				}
				else if (this.pendingQueue.Count != num5)
				{
					throw new MserveException(string.Format("Synchronization failed when flushing because {0} items were left in the pending queue, {1} items expected", this.pendingQueue.Count, num5), null);
				}
			}
		}

		private void QueueOperation(RecipientSyncOperation op)
		{
			this.resultSet.Add(op);
			foreach (string text in op.RemovedEntries)
			{
				if (this.pendingQueue.ContainsKey(text))
				{
					if (!this.pendingQueue[text].IsDelete)
					{
						ExTraceGlobals.DeltaSyncAPITracer.TraceError<string>((long)this.GetHashCode(), "Synchronization failed when inserting remove entry {0} when it is already in the add or read entries.", text);
						throw new MserveException("Synchronization failed when inserting remove entry " + text + " when it is already in the add or read entries", null);
					}
					ExTraceGlobals.DeltaSyncAPITracer.TraceWarning<string>((long)this.GetHashCode(), "Duplicate delete entry for {0} detected. Ignore it", text);
				}
				else
				{
					RecipientPendingOperation value = new RecipientPendingOperation(op, OperationType.Delete);
					this.pendingQueue.Add(text, value);
					if (this.pendingQueue.Count == MserveWebService.chunkSize)
					{
						this.Flush(true, null);
					}
				}
			}
			foreach (string text2 in op.AddedEntries)
			{
				if (this.pendingQueue.ContainsKey(text2))
				{
					if (!this.pendingQueue[text2].IsAdd)
					{
						ExTraceGlobals.DeltaSyncAPITracer.TraceError<string>((long)this.GetHashCode(), "Synchronization failed when inserting add entry {0} when it is already in the remove or read entries.", text2);
						throw new MserveException("Synchronization failed when inserting add entry " + text2 + " when it is already in the remove or read entries", null);
					}
					ExTraceGlobals.DeltaSyncAPITracer.TraceError<string>((long)this.GetHashCode(), "Duplicate add entry for {0} detected. Ignore it", text2);
				}
				else
				{
					RecipientPendingOperation value2 = new RecipientPendingOperation(op, OperationType.Add);
					this.pendingQueue.Add(text2, value2);
					if (this.pendingQueue.Count == MserveWebService.chunkSize)
					{
						this.Flush(true, null);
					}
				}
			}
			foreach (string text3 in op.ReadEntries)
			{
				if (this.pendingQueue.ContainsKey(text3))
				{
					if (!this.pendingQueue[text3].IsRead)
					{
						ExTraceGlobals.DeltaSyncAPITracer.TraceError<string>((long)this.GetHashCode(), "Synchronization failed when inserting read entry {0} when it is already in the remove or add entries.", text3);
						throw new MserveException("Synchronization failed when inserting read entry " + text3 + " when it is already in the remove or add entries", null);
					}
					ExTraceGlobals.DeltaSyncAPITracer.TraceError<string>((long)this.GetHashCode(), "Duplicate read entry for {0} detected. Ignore it", text3);
				}
				else
				{
					RecipientPendingOperation value3 = new RecipientPendingOperation(op, OperationType.Read);
					this.pendingQueue.Add(text3, value3);
					if (this.pendingQueue.Count == MserveWebService.chunkSize)
					{
						this.Flush(true, null);
					}
				}
			}
		}

		private Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision SendProvisionRequest(Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provRequest, bool keepAlive, int? timeout)
		{
			Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provision = null;
			Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provision2 = null;
			Exception ex = null;
			string text = string.Empty;
			bool flag = false;
			int num = provRequest.Add.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
			int num2 = provRequest.Delete.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
			int num3 = provRequest.Read.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
			this.perfCounters.UpdateTotalRequestPerfCounters(num3 + num + num2);
			if (num3 > 0 && this.UseOfflineMserveCacheServiceFirst())
			{
				this.ProcessAndRemoveReadRequest(provRequest, out provision2, true, ref flag);
			}
			num3 = provRequest.Read.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
			int num4 = num3 + num2 + num;
			if (num4 > 0 && this.UseRealMserveWebService())
			{
				this.perfCounters.UpdateRequestPerfCountersForMserveWebService(num3, num, num2);
				HttpSessionConfig httpSessionConfig = this.InitializeHttpSessionConfig(keepAlive);
				if (timeout != null)
				{
					httpSessionConfig.Timeout = timeout.Value;
				}
				this.LastResponseDiagnosticInfo = null;
				this.LastResponseTransactionId = null;
				this.LastIpUsed = this.ResolveToIpAddress(this.provisionUri.Host, false);
				using (httpSessionConfig.RequestStream = new MemoryStream())
				{
					MserveWebService.provRequestSerializer.Serialize(new StreamWriter(httpSessionConfig.RequestStream, Encoding.UTF8), provRequest);
					httpSessionConfig.RequestStream.Position = 0L;
					this.LogProvisionRequest(this.provisionUriWithoutQuery.ToString());
					using (HttpClient httpClient = new HttpClient())
					{
						ICancelableAsyncResult asyncResult = httpClient.BeginDownload(this.provisionUri, httpSessionConfig, null, null);
						DownloadResult downloadResult = httpClient.EndDownload(asyncResult);
						if (downloadResult.ResponseHeaders != null)
						{
							this.LastResponseDiagnosticInfo = downloadResult.ResponseHeaders["X-WindowsLive-Hydra"];
							this.LastResponseTransactionId = downloadResult.ResponseHeaders["X-TransactionID"];
						}
						else
						{
							this.LastIpUsed = this.ResolveToIpAddress(this.provisionUri.Host, true);
						}
						if (downloadResult.IsSucceeded)
						{
							try
							{
								provision = (Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision)MserveWebService.provResponseSerializer.Deserialize(new StreamReader(downloadResult.ResponseStream, Encoding.UTF8));
							}
							catch (InvalidOperationException ex2)
							{
								ex = ((ex2.InnerException != null) ? ex2.InnerException : ex2);
								text = "Failure during deserialization";
							}
							finally
							{
								if (downloadResult.ResponseStream != null)
								{
									downloadResult.ResponseStream.Close();
									downloadResult.ResponseStream = null;
								}
							}
							if (ex == null)
							{
								if (flag)
								{
									int num5 = provision2.Responses.Read.Count((Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType s) => s != null);
									if (num5 > 0)
									{
										provision.Responses.Read = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType[num5];
										for (int i = 0; i < num5; i++)
										{
											provision.Responses.Read[i] = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType();
											provision.Responses.Read[i] = provision2.Responses.Read[i];
										}
									}
								}
								return provision;
							}
						}
						else
						{
							ex = downloadResult.Exception;
							text = "Exception during Mserve lookup";
						}
					}
				}
			}
			if (ex != null)
			{
				this.perfCounters.UpdateFailurePerfCountersForMserveWebService(num4);
			}
			if (num4 > 0 && this.UseOfflineMserveCacheService())
			{
				provision = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision();
				provision.Responses = new ProvisionResponses();
				CommandStatusCode commandStatusCode = this.ProcessReadRequest(provRequest, provision);
				if (ex != null && commandStatusCode != CommandStatusCode.Success)
				{
					text = string.Format("{0}. Also failed in Mserve Cache lookup. Cache failure type: {1}.", text, commandStatusCode.ToString());
					throw new MserveException(text, ex);
				}
				provision.Status = 1;
				ExTraceGlobals.MserveCacheServiceTracer.TraceDebug<int>((long)this.GetHashCode(), "Successfully processed provisioning request using MserveCacheService. ReadRequest count {0}", (provision.Responses.Read != null) ? provision.Responses.Read.Length : 0);
				return provision;
			}
			else
			{
				if (provision2 != null && num == 0 && num2 == 0)
				{
					return provision2;
				}
				this.CleanupQueueAndResultSet(provRequest.Read, num3);
				this.CleanupQueueAndResultSet(provRequest.Add, num);
				this.CleanupQueueAndResultSet(provRequest.Delete, num2);
				this.perfCounters.UpdateTotalFailuresPerfCounters(num4);
				throw new MserveException(text, ex);
			}
		}

		private CommandStatusCode ProcessAddRequest(Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provRequest, Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponse)
		{
			int num = provRequest.Add.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
			if (num > 0)
			{
				this.CleanupQueueAndResultSet(provRequest.Add, num);
				throw new InvalidMserveRequestException();
			}
			return CommandStatusCode.Success;
		}

		private CommandStatusCode ProcessDeleteRequest(Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provRequest, Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponse)
		{
			int num = provRequest.Delete.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
			if (num > 0)
			{
				this.CleanupQueueAndResultSet(provRequest.Delete, num);
				throw new InvalidMserveRequestException();
			}
			return CommandStatusCode.Success;
		}

		private CommandStatusCode ProcessReadRequest(Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provRequest, Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponse)
		{
			CommandStatusCode result = CommandStatusCode.Success;
			int num = provRequest.Read.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
			if (num > 0)
			{
				provResponse.Responses.Read = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType[num];
				for (int i = 0; i < num; i++)
				{
					Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType accountType = provRequest.Read[i];
					if (accountType != null)
					{
						provResponse.Responses.Read[i] = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType();
						provResponse.Responses.Read[i].Name = accountType.Name;
						this.perfCounters.UpdateRequestPerfCountersForMserveCacheService(1, 0, 0);
						result = this.ProcessCacheLookup(ref provResponse.Responses.Read[i], true);
					}
				}
			}
			return result;
		}

		private CommandStatusCode ProcessCacheLookup(ref Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType readResponse, bool updateFailurePerfCounters)
		{
			CommandStatusCode result = CommandStatusCode.Success;
			try
			{
				MserveCacheServiceProvider instance = MserveCacheServiceProvider.GetInstance();
				string text = instance.ReadMserveData(readResponse.Name);
				if (text.Equals(-1.ToString()))
				{
					result = CommandStatusCode.UserDoesNotExist;
					if (updateFailurePerfCounters)
					{
						this.UpdateCacheFailurePerfCounters();
					}
				}
				ExTraceGlobals.MserveCacheServiceTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Successfully read Mserve entry from cache for request {0}, partnerId/minorPartnerId = {1}", readResponse.Name, text);
				readResponse.PartnerID = text;
				readResponse.Status = 1;
			}
			catch (Exception ex)
			{
				if (updateFailurePerfCounters)
				{
					this.UpdateCacheFailurePerfCounters();
				}
				ExTraceGlobals.MserveCacheServiceTracer.TraceError<string, string>((long)this.GetHashCode(), "Got exception while reading Mserve entry in cache for request {0}. Exception: {1}", readResponse.Name, ex.ToString());
				readResponse.PartnerID = -1.ToString();
				readResponse.Status = 5101;
				result = CommandStatusCode.MserveCacheServiceChannelFaulted;
			}
			return result;
		}

		private void UpdateCacheFailurePerfCounters()
		{
			this.perfCounters.UpdateFailurePerfCountersForMservCacheService(1);
			if (MserveWebService.CurrentMserveCacheServiceMode != MserveCacheServiceMode.EnabledForReadOnly)
			{
				this.perfCounters.UpdateTotalFailuresPerfCounters(1);
			}
		}

		private void CleanupQueueAndResultSet(Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType[] requests, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType accountType = requests[i];
				if (this.pendingQueue.ContainsKey(accountType.Name))
				{
					RecipientSyncOperation recipientSyncOperation = this.pendingQueue[accountType.Name].RecipientSyncOperation;
					this.pendingQueue.Remove(accountType.Name);
					if (this.resultSet.Contains(recipientSyncOperation))
					{
						this.resultSet.Remove(recipientSyncOperation);
					}
				}
			}
		}

		private bool ProcessAndRemoveReadRequest(Microsoft.Exchange.Net.Mserve.ProvisionRequest.Provision provRequest, out Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponseForReadOnly, bool removeReadRequests, ref bool mergeReadResponseNeeded)
		{
			provResponseForReadOnly = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision();
			provResponseForReadOnly.Responses = new ProvisionResponses();
			if (this.ProcessReadRequest(provRequest, provResponseForReadOnly) != CommandStatusCode.Success)
			{
				ExTraceGlobals.MserveCacheServiceTracer.TraceDebug((long)this.GetHashCode(), "CurrentMserveCacheServiceMode is EnabledForReadOnly, but some entries could not be found in Cache. Will try to read again from Mserve Web service");
				provResponseForReadOnly = null;
				return false;
			}
			ExTraceGlobals.MserveCacheServiceTracer.TraceDebug((long)this.GetHashCode(), "Mserve Read requests are processed successfully from Cache");
			if (removeReadRequests)
			{
				ExTraceGlobals.MserveCacheServiceTracer.TraceDebug((long)this.GetHashCode(), "Removing read requests from ProvisionRequest");
				int num = provRequest.Read.Count((Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType s) => s != null);
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						provRequest.Read[i] = null;
					}
					mergeReadResponseNeeded = true;
					ExTraceGlobals.MserveCacheServiceTracer.TraceDebug<int>((long)this.GetHashCode(), "Removed {0} read requests from ProvisionRequest. Need to merge read responses with write responses coming from Mserve", num);
				}
			}
			provResponseForReadOnly.Status = 1;
			return true;
		}

		private string ResolveToIpAddress(string hostName, bool flushCache)
		{
			DnsResult dnsResult = null;
			DnsQuery query = new DnsQuery(DnsRecordType.A, hostName);
			if (flushCache)
			{
				MserveWebService.dnsCache.FlushCache();
			}
			dnsResult = MserveWebService.dnsCache.Find(query);
			if (dnsResult == null)
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
					IPAddress[] addressList = hostEntry.AddressList;
					if (addressList.Length > 0)
					{
						dnsResult = new DnsResult(DnsStatus.Success, addressList[0], TimeSpan.FromMinutes(1.0));
						MserveWebService.dnsCache.Add(query, dnsResult);
					}
				}
				catch (SocketException)
				{
				}
			}
			if (dnsResult != null)
			{
				return dnsResult.Server.ToString();
			}
			return string.Empty;
		}

		private bool UseRealMserveWebService()
		{
			return MserveWebService.CurrentMserveCacheServiceMode == MserveCacheServiceMode.NotEnabled || MserveWebService.CurrentMserveCacheServiceMode == MserveCacheServiceMode.EnabledWithFallback || MserveWebService.CurrentMserveCacheServiceMode == MserveCacheServiceMode.EnabledForReadOnly;
		}

		private bool UseOfflineMserveCacheService()
		{
			return this.isMicrosoftHostedOnly && (MserveWebService.CurrentMserveCacheServiceMode == MserveCacheServiceMode.EnabledWithFallback || MserveWebService.CurrentMserveCacheServiceMode == MserveCacheServiceMode.AlwaysEnabled);
		}

		private bool UseOfflineMserveCacheServiceFirst()
		{
			return this.isMicrosoftHostedOnly && MserveWebService.CurrentMserveCacheServiceMode == MserveCacheServiceMode.EnabledForReadOnly;
		}

		private void AckAccountResponse(string name, OperationType type, string partnerId, int status)
		{
			RecipientSyncOperation recipientSyncOperation = this.pendingQueue[name].RecipientSyncOperation;
			recipientSyncOperation.CompletedSyncCount++;
			this.pendingQueue.Remove(name);
			if (status == 1)
			{
				if (type == OperationType.Read)
				{
					int partnerId2;
					if (int.TryParse(partnerId, NumberStyles.Number, CultureInfo.InvariantCulture, out partnerId2))
					{
						recipientSyncOperation.PartnerId = partnerId2;
					}
					else
					{
						recipientSyncOperation.PartnerId = -1;
					}
				}
				if (this.batchMode && type != OperationType.Read)
				{
					recipientSyncOperation.PendingSyncStateCommitEntries[type].Add(name);
				}
			}
			else
			{
				if (!this.batchMode)
				{
					this.resultSet.Remove(recipientSyncOperation);
					throw new MserveException(string.Format("Provision Request for account {0} failed with status {1}, PartnerId {2}", name, status, partnerId), null);
				}
				if (this.trackDuplicatedAddEntries && status == 3215)
				{
					recipientSyncOperation.DuplicatedAddEntries.Add(name);
				}
				else if (MserveWebService.IsRetryableDeltaSyncError(status))
				{
					recipientSyncOperation.RetryableEntries[type].Add(new FailedAddress(name, status, true));
				}
				else
				{
					recipientSyncOperation.NonRetryableEntries[type].Add(new FailedAddress(name, status, false));
					if (type != OperationType.Read)
					{
						recipientSyncOperation.PendingSyncStateCommitEntries[type].Add(name);
					}
				}
			}
			if (recipientSyncOperation.Synchronized)
			{
				this.completedResultSet.Add(recipientSyncOperation);
				this.resultSet.Remove(recipientSyncOperation);
			}
		}

		private void ProcessAccountType(Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType[] accountTypes, OperationType type)
		{
			int num = 0;
			switch (type)
			{
			case OperationType.Add:
				num = (this.trackDuplicatedAddEntries ? 0 : 3215);
				break;
			case OperationType.Delete:
				num = 3201;
				break;
			case OperationType.Read:
				num = 3201;
				break;
			}
			if (accountTypes != null)
			{
				foreach (Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType accountType in accountTypes)
				{
					this.LogResponseAccountDetail(type, accountType);
					this.AckAccountResponse(accountType.Name, type, accountType.PartnerID, (accountType.Status == num) ? 1 : accountType.Status);
				}
			}
		}

		private bool ProcessProvisionResponse(Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponse)
		{
			ExTraceGlobals.DeltaSyncAPITracer.TraceDebug<int>((long)this.GetHashCode(), "Response status is {0}", provResponse.Status);
			this.LogProvisionResponse(provResponse);
			if (provResponse.Status == 4301)
			{
				MserveWebService.chunkSize = this.FetchChunkSize();
				return false;
			}
			if (provResponse.Status != 1)
			{
				this.Reset();
				ExTraceGlobals.DeltaSyncAPITracer.TraceError<int>((long)this.GetHashCode(), "Provision request failed at top level with status {0}", provResponse.Status);
				throw new MserveException(string.Format("Provision request failed at the top level with error: {0} {1}", provResponse.Status, (provResponse.Fault != null) ? provResponse.Fault.Detail : string.Empty), null);
			}
			this.ProcessAccountType(provResponse.Responses.Add, OperationType.Add);
			this.ProcessAccountType(provResponse.Responses.Delete, OperationType.Delete);
			this.ProcessAccountType(provResponse.Responses.Read, OperationType.Read);
			return true;
		}

		private int ProcessOneOffReadAccountType(Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType[] accountTypes)
		{
			if (accountTypes == null || accountTypes.Length != 1)
			{
				throw new MserveException(string.Format("Unexpected number of responses. Expected 1, got {0}", (accountTypes != null) ? accountTypes.Length.ToString() : "null"));
			}
			this.LogResponseAccountDetail(OperationType.Read, accountTypes[0]);
			int result = -1;
			if (accountTypes[0].Status == 3201 || accountTypes[0].Status == 1)
			{
				if (!int.TryParse(accountTypes[0].PartnerID, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				{
					result = -1;
				}
				return result;
			}
			throw new MserveException(string.Format("Provision Request for account {0} failed with status {1}, PartnerId {2}", accountTypes[0].Name, accountTypes[0].Status, accountTypes[0].PartnerID));
		}

		private bool ProcessOneOffReadResponse(Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision provResponse, out int partnerId)
		{
			ExTraceGlobals.DeltaSyncAPITracer.TraceDebug<int>((long)this.GetHashCode(), "Response status is {0}", provResponse.Status);
			if (provResponse.Status != 1)
			{
				string arg = string.Empty;
				if (provResponse.Fault != null)
				{
					arg = string.Format("Code: {0}, String: {1}, Details: {2}", provResponse.Fault.Faultcode, provResponse.Fault.Faultstring, provResponse.Fault.Detail);
				}
				ExTraceGlobals.DeltaSyncAPITracer.TraceError<int, string>((long)this.GetHashCode(), "Provision request failed at top level with error {0} {1}", provResponse.Status, arg);
				throw new MserveException(string.Format("Provision request failed at the top level with error: {0} {1}", provResponse.Status, arg));
			}
			partnerId = this.ProcessOneOffReadAccountType(provResponse.Responses.Read);
			return true;
		}

		private int FetchChunkSize()
		{
			ExTraceGlobals.DeltaSyncAPITracer.TraceDebug((long)this.GetHashCode(), "FetchChunkSize");
			Exception ex = null;
			string message = string.Empty;
			if (this.UseRealMserveWebService())
			{
				Microsoft.Exchange.Net.Mserve.SettingsRequest.Settings settings = new Microsoft.Exchange.Net.Mserve.SettingsRequest.Settings();
				settings.ServiceSettings = new ServiceSettingsType();
				settings.ServiceSettings.Properties = new ServiceSettingsTypeProperties();
				settings.ServiceSettings.Properties.Get = new object();
				Microsoft.Exchange.Net.Mserve.SettingsResponse.Settings settings2 = null;
				HttpSessionConfig httpSessionConfig = this.InitializeHttpSessionConfig(true);
				using (httpSessionConfig.RequestStream = new MemoryStream())
				{
					MserveWebService.settingsRequestSerializer.Serialize(new StreamWriter(httpSessionConfig.RequestStream, Encoding.UTF8), settings);
					httpSessionConfig.RequestStream.Position = 0L;
					using (HttpClient httpClient = new HttpClient())
					{
						ICancelableAsyncResult asyncResult = httpClient.BeginDownload(this.settingUri, httpSessionConfig, null, null);
						DownloadResult downloadResult = httpClient.EndDownload(asyncResult);
						if (downloadResult.IsSucceeded)
						{
							try
							{
								settings2 = (Microsoft.Exchange.Net.Mserve.SettingsResponse.Settings)MserveWebService.settingsResponseSerializer.Deserialize(new StreamReader(downloadResult.ResponseStream, Encoding.UTF8));
							}
							catch (InvalidOperationException ex2)
							{
								ex = ((ex2.InnerException != null) ? ex2.InnerException : ex2);
								message = "Failure during deserialization";
							}
							finally
							{
								if (downloadResult.ResponseStream != null)
								{
									downloadResult.ResponseStream.Close();
									downloadResult.ResponseStream = null;
								}
							}
							if (ex == null)
							{
								if (settings2.Status == 1)
								{
									return settings2.ServiceSettings.Properties.Get.MaxNumberOfProvisionCommands;
								}
								ex = new MserveException(string.Format("Reading Config Error: {0} {1}", settings2.Status, (settings2.Fault != null) ? settings2.Fault.Detail : string.Empty), null);
								message = ex.Message;
							}
						}
						else
						{
							ex = downloadResult.Exception;
							message = "Exception during Mserve lookup";
						}
					}
				}
			}
			if (this.UseOfflineMserveCacheService() || MserveWebService.CurrentMserveCacheServiceMode == MserveCacheServiceMode.EnabledForReadOnly)
			{
				MserveCacheServiceProvider instance = MserveCacheServiceProvider.GetInstance();
				int result = 0;
				try
				{
					result = instance.GetChunkSize();
				}
				catch (Exception ex3)
				{
					ExTraceGlobals.MserveCacheServiceTracer.TraceError<string, int>(0L, "Got exception while reading chunk size from Mserve. Exception: {0}. Returning default chunk size {1}", ex3.ToString(), MserveWebService.chunkSize);
					return MserveWebService.chunkSize;
				}
				return result;
			}
			throw new MserveException(message, ex);
		}

		protected virtual void LogProvisionAccountDetail(string type, Microsoft.Exchange.Net.Mserve.ProvisionRequest.AccountType account)
		{
		}

		protected virtual void LogResponseAccountDetail(OperationType type, Microsoft.Exchange.Net.Mserve.ProvisionResponse.AccountType account)
		{
		}

		protected virtual void LogProvisionRequest(string url)
		{
		}

		protected virtual void LogProvisionResponse(Microsoft.Exchange.Net.Mserve.ProvisionResponse.Provision response)
		{
		}

		private static bool ValidateCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslError)
		{
			if (sslError == SslPolicyErrors.None)
			{
				return true;
			}
			if (sslError == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				if (MserveWebService.staticRemoteCertSubject == null)
				{
					return true;
				}
				if (string.Compare(MserveWebService.staticRemoteCertSubject, cert.Subject, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public const int InvalidPartnerId = -1;

		internal const string DomainEntryAddressFormat = "E5CB63F56E8B4b69A1F70C192276D6AD@{0}";

		internal const string DomainEntryAddressFormatForOrgGuid = "43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved";

		internal const string DomainEntryAddressFormatStartNewOrganization = "21668DE042684883B19BCB376E3BE474@{0}";

		internal const string DomainEntryAddressFormatMinorPartnerId = "7f66cd009b304aeda37ffdeea1733ff6@{0}";

		internal const string DomainEntryAddressFormatMinorPartnerIdForOrgGuid = "3da19c7b44a74bd3896daaf008594b6c@{0}.exchangereserved";

		internal const string DomainEntryAddressFormatTenantNegoConfig = "ade5142cfe3d4ff19fed54a7f6087a98@{0}";

		internal const string DomainEntryAddressFormatTenantOAuthClientProfileConfig = "0f01471e875a455a80c59def2a36ee3f@{0}";

		private const string userAgent = "ExchangeHostedServices/1.0";

		private const string contentType = "text/xml";

		private const string certificateValidationComponentName = "MserveWebService";

		private const string partnerTokenAuthQueryStringFormat = "Dspk={0}";

		private const string mServDiagnosticHeader = "X-WindowsLive-Hydra";

		private const string mservTransactionHeader = "X-TransactionID";

		internal const string ExchangeLabsRegkeyPath = "SOFTWARE\\Microsoft\\ExchangeLabs";

		internal const string MserveCacheServiceModeRegkeyValue = "MserveCacheServiceEnabled";

		private const MserveCacheServiceMode defaultMserveCacheServiceMode = MserveCacheServiceMode.NotEnabled;

		private static readonly CancelableAsyncCallback asyncApiCallback = new CancelableAsyncCallback(MserveWebService.AsyncApiCallback);

		private static readonly DnsCache dnsCache = new DnsCache(10);

		private bool trackDuplicatedAddEntries;

		private static int chunkSize = 100;

		private readonly Uri provisionUri;

		private readonly Uri provisionUriWithoutQuery;

		private readonly Uri settingUri;

		private string remoteCertSubject;

		private bool isMicrosoftHostedOnly;

		private static int whenRegKeyLastChecked = 0;

		private static object modeLockObject = new object();

		private static MserveCacheServiceMode currentMserveCacheServiceMode = MserveCacheServiceMode.NotEnabled;

		private static TimeSpan MserveCacheServiceRegistryCheckInterval = TimeSpan.FromMinutes(5.0);

		private MservePerfCounters perfCounters;

		private Dictionary<string, RecipientPendingOperation> pendingQueue = new Dictionary<string, RecipientPendingOperation>();

		private List<RecipientSyncOperation> resultSet = new List<RecipientSyncOperation>();

		private List<RecipientSyncOperation> completedResultSet = new List<RecipientSyncOperation>();

		private bool batchMode;

		private static bool initialized;

		private static readonly object mutex = new object();

		private static string staticRemoteCertSubject;

		private static Microsoft.Exchange.Net.Mserve.ProvisionRequest.ProvisionSerializer provRequestSerializer = new Microsoft.Exchange.Net.Mserve.ProvisionRequest.ProvisionSerializer();

		private static Microsoft.Exchange.Net.Mserve.ProvisionResponse.ProvisionSerializer provResponseSerializer = new Microsoft.Exchange.Net.Mserve.ProvisionResponse.ProvisionSerializer();

		private static Microsoft.Exchange.Net.Mserve.SettingsRequest.SettingsSerializer settingsRequestSerializer = new Microsoft.Exchange.Net.Mserve.SettingsRequest.SettingsSerializer();

		private static Microsoft.Exchange.Net.Mserve.SettingsResponse.SettingsSerializer settingsResponseSerializer = new Microsoft.Exchange.Net.Mserve.SettingsResponse.SettingsSerializer();
	}
}

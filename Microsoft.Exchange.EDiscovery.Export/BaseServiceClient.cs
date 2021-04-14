using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web.Services.Protocols;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal abstract class BaseServiceClient<ServiceBindingType, FunctionalInterfaceType> : IServiceClient<FunctionalInterfaceType>, IDisposable where ServiceBindingType : HttpWebClientProtocol, IServiceBinding, new()
	{
		protected BaseServiceClient(Uri serviceEndpoint, IServiceCallingContext<ServiceBindingType> serviceCallingContext, CancellationToken abortTokenForTasks)
		{
			this.ServiceEndpoint = serviceEndpoint;
			this.ServiceCallingContext = serviceCallingContext;
			this.AbortTokenForTasks = abortTokenForTasks;
		}

		public Uri ServiceEndpoint { get; private set; }

		public abstract FunctionalInterfaceType FunctionalInterface { get; }

		public string CurrentMailbox { get; set; }

		public IAutoDiscoverClient AutoDiscoverInterface { get; set; }

		private protected ServiceBindingType ServiceBinding { protected get; private set; }

		private protected CancellationToken AbortTokenForTasks { protected get; private set; }

		private protected IServiceCallingContext<ServiceBindingType> ServiceCallingContext { protected get; private set; }

		public static bool IsTransientError(string code)
		{
			ResponseCodeType code2;
			return Enum.TryParse<ResponseCodeType>(code, out code2) && BaseServiceClient<ServiceBindingType, FunctionalInterfaceType>.IsTransientError(code2);
		}

		public static bool IsRebindableError(string code)
		{
			ResponseCodeType code2;
			return Enum.TryParse<ResponseCodeType>(code, out code2) && BaseServiceClient<ServiceBindingType, FunctionalInterfaceType>.IsRebindableError(code2);
		}

		public static bool IsTransientError(ResponseCodeType code)
		{
			if (code <= ResponseCodeType.ErrorInternalServerTransientError)
			{
				if (code <= ResponseCodeType.ErrorDeleteItemsFailed)
				{
					switch (code)
					{
					case ResponseCodeType.ErrorADOperation:
					case ResponseCodeType.ErrorADUnavailable:
						break;
					case ResponseCodeType.ErrorADSessionFilter:
						return false;
					default:
						if (code != ResponseCodeType.ErrorBatchProcessingStopped && code != ResponseCodeType.ErrorDeleteItemsFailed)
						{
							return false;
						}
						break;
					}
				}
				else if (code != ResponseCodeType.ErrorExceededConnectionCount && code != ResponseCodeType.ErrorFolderSavePropertyError)
				{
					switch (code)
					{
					case ResponseCodeType.ErrorInsufficientResources:
					case ResponseCodeType.ErrorInternalServerTransientError:
						break;
					case ResponseCodeType.ErrorInternalServerError:
						return false;
					default:
						return false;
					}
				}
			}
			else if (code <= ResponseCodeType.ErrorNotEnoughMemory)
			{
				if (code != ResponseCodeType.ErrorItemSavePropertyError)
				{
					switch (code)
					{
					case ResponseCodeType.ErrorMailboxMoveInProgress:
					case ResponseCodeType.ErrorMailboxStoreUnavailable:
						break;
					default:
						if (code != ResponseCodeType.ErrorNotEnoughMemory)
						{
							return false;
						}
						break;
					}
				}
			}
			else if (code <= ResponseCodeType.ErrorTimeoutExpired)
			{
				switch (code)
				{
				case ResponseCodeType.ErrorServerBusy:
				case ResponseCodeType.ErrorStaleObject:
					break;
				case ResponseCodeType.ErrorServiceDiscoveryFailed:
					return false;
				default:
					if (code != ResponseCodeType.ErrorTimeoutExpired)
					{
						return false;
					}
					break;
				}
			}
			else if (code != ResponseCodeType.ErrorTooManyObjectsOpened && code != ResponseCodeType.ErrorMessageTrackingTransientError)
			{
				return false;
			}
			return true;
		}

		public static bool IsRebindableError(ResponseCodeType code)
		{
			switch (code)
			{
			case ResponseCodeType.ErrorMailboxMoveInProgress:
			case ResponseCodeType.ErrorMailboxStoreUnavailable:
				break;
			default:
				switch (code)
				{
				case ResponseCodeType.ErrorProxyServiceDiscoveryFailed:
				case ResponseCodeType.ErrorProxyTokenExpired:
					break;
				default:
					if (code != ResponseCodeType.ErrorMailboxFailover)
					{
						return false;
					}
					break;
				}
				break;
			}
			return true;
		}

		public virtual bool Connect()
		{
			if (this.ServiceBinding == null)
			{
				this.ServiceBinding = this.ServiceCallingContext.CreateServiceBinding(this.ServiceEndpoint);
			}
			return this.ServiceBinding != null;
		}

		public void Dispose()
		{
			if (this.ServiceBinding != null)
			{
				ServiceBindingType serviceBinding = this.ServiceBinding;
				serviceBinding.Dispose();
				this.ServiceBinding = default(ServiceBindingType);
			}
		}

		protected void InternalCallService<BaseResponseMessageType>(Func<BaseResponseMessageType> delegateServiceCall, Action<BaseResponseMessageType> responseProcessor, Func<Exception, Exception> exceptionHandler, Func<bool> authorizationHandler, Action<Uri> urlRedirectionHandler)
		{
			int num = 0;
			DateTime t = DateTime.UtcNow.Add(ConstantProvider.TotalRetryTimeWindow);
			ScenarioData.Current["BI"] = DateTime.UtcNow.Ticks.ToString();
			Exception threadException;
			Exception ex;
			Exception ex2;
			for (;;)
			{
				ex = null;
				threadException = null;
				ex2 = null;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				ScenarioData.Current["R"] = num.ToString();
				ScenarioData.Current["RT"] = DateTime.UtcNow.Ticks.ToString();
				if (this.ServiceBinding != null)
				{
					ServiceBindingType serviceBinding = this.ServiceBinding;
					serviceBinding.UserAgent = ScenarioData.Current.ToString();
				}
				try
				{
					BaseResponseMessageType response = default(BaseResponseMessageType);
					bool flag4 = false;
					Thread thread = new Thread(delegate()
					{
						try
						{
							response = delegateServiceCall();
						}
						catch (Exception threadException)
						{
							threadException = threadException;
						}
					});
					thread.Start();
					while (!flag4)
					{
						if (this.AbortTokenForTasks.IsCancellationRequested)
						{
							throw new ExportException(ExportErrorType.StopRequested);
						}
						flag4 = thread.Join(5000);
					}
					if (!flag4)
					{
						thread.Abort();
					}
					if (threadException != null)
					{
						throw threadException;
					}
					if (responseProcessor != null)
					{
						responseProcessor(response);
					}
				}
				catch (RetryException ex3)
				{
					ex = ex3.InnerException;
					flag = true;
					if (ex3.ResetRetryCounter)
					{
						Tracer.TraceError("BaseServiceClient.InternalCallService: Resetting retry in RetryException.", new object[0]);
						flag2 = true;
					}
				}
				catch (ExportException ex4)
				{
					ex = ex4;
					if (ex4.ErrorType == ExportErrorType.Unauthorized)
					{
						flag3 = true;
					}
				}
				catch (WebException ex5)
				{
					ex = ex5;
					if (ex5.Status == WebExceptionStatus.TrustFailure)
					{
						Tracer.TraceError("BaseServiceClient.InternalCallService: Unable to establish trust on exception. No retry.", new object[0]);
						flag = false;
					}
					else if (ex5.Status == WebExceptionStatus.ConnectFailure)
					{
						SocketException ex6 = ex5.InnerException as SocketException;
						flag = (ex6 == null || ex6.SocketErrorCode != SocketError.ConnectionRefused);
						if (!flag)
						{
							flag = this.Rebind();
						}
						Tracer.TraceError("BaseServiceClient.InternalCallService: Connect failure. Retry: {0}.", new object[]
						{
							flag
						});
					}
					else if (ex5.Status == WebExceptionStatus.NameResolutionFailure)
					{
						Tracer.TraceError("BaseServiceClient.InternalCallService: DNS look up failure. No retry.", new object[0]);
						flag = this.Rebind();
					}
					else
					{
						HttpWebResponse httpWebResponse = ex5.Response as HttpWebResponse;
						flag = true;
						if (httpWebResponse != null)
						{
							HttpStatusCode statusCode = httpWebResponse.StatusCode;
							switch (statusCode)
							{
							case HttpStatusCode.MovedPermanently:
							case HttpStatusCode.Found:
								break;
							default:
								if (statusCode != HttpStatusCode.TemporaryRedirect)
								{
									if (statusCode == HttpStatusCode.Unauthorized)
									{
										Tracer.TraceError("BaseServiceClient.InternalCallService: HTTP 401 unauthorized", new object[0]);
										flag = false;
										flag3 = true;
										ex = new ExportException(ExportErrorType.Unauthorized, ex5);
										goto IL_342;
									}
									goto IL_342;
								}
								break;
							}
							string text = httpWebResponse.Headers[HttpResponseHeader.Location];
							Tracer.TraceInformation("BaseServiceClient.InternalCallService: HTTP redirection to {0}", new object[]
							{
								text
							});
							ex = new ExportException(ExportErrorType.UnexpectedWebServiceUrlRedirection, ex5);
							flag = false;
							if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
							{
								Uri uri = new Uri(text);
								if (uri.Scheme == Uri.UriSchemeHttps && urlRedirectionHandler != null)
								{
									this.ServiceEndpoint = uri;
									urlRedirectionHandler(uri);
									ex = null;
									flag = true;
									flag2 = true;
									Tracer.TraceError("BaseServiceClient.InternalCallService: Resetting retry during HTTP redirection.", new object[0]);
								}
							}
						}
					}
					IL_342:;
				}
				catch (SoapException ex7)
				{
					if (ex7.Code != null && ex7.Code.Name == "ErrorAccessDenied")
					{
						flag = false;
						flag3 = true;
						ex = new ExportException(ExportErrorType.Unauthorized, ex7);
					}
					else
					{
						ex = ex7;
						flag = false;
						if (ex7.Code != null)
						{
							if (BaseServiceClient<ServiceBindingType, FunctionalInterfaceType>.IsRebindableError(ex7.Code.Name))
							{
								flag = this.Rebind();
							}
							if (BaseServiceClient<ServiceBindingType, FunctionalInterfaceType>.IsTransientError(ex7.Code.Name))
							{
								flag = true;
							}
						}
					}
				}
				catch (TimeoutException ex8)
				{
					ex = ex8;
					flag = true;
				}
				catch (InvalidOperationException ex9)
				{
					ex = ex9;
					flag = true;
				}
				if (ex != null)
				{
					if (exceptionHandler != null)
					{
						Tracer.TraceError("BaseServiceClient.InternalCallService: Exception handler is handling exception : {0}", new object[]
						{
							ex
						});
						ex2 = exceptionHandler(ex);
						Tracer.TraceError("BaseServiceClient.InternalCallService: Exception after being handled : {0}", new object[]
						{
							ex2
						});
					}
					else
					{
						ex2 = new ExportException(ExportErrorType.ExchangeWebServiceCallFailed, ex);
					}
				}
				if (flag && !flag2)
				{
					int num2 = this.GetRetryWaitTime(num);
					Tracer.TraceError("BaseServiceClient.InternalCallService: Retry after {0} milliseconds on exception : {1}", new object[]
					{
						num2,
						ex
					});
					while (num2 > 0 && t > DateTime.UtcNow)
					{
						Thread.Sleep((num2 > 5000) ? 5000 : num2);
						num2 -= 5000;
						if (this.AbortTokenForTasks.IsCancellationRequested)
						{
							goto Block_8;
						}
					}
				}
				if (flag3)
				{
					Tracer.TraceInformation("BaseServiceClient.InternalCallService: Unauthorized", new object[0]);
					if (authorizationHandler == null || !authorizationHandler())
					{
						goto IL_4CB;
					}
					flag = true;
				}
				if (flag2)
				{
					num = 0;
				}
				else
				{
					num++;
				}
				if (!flag || !(t > DateTime.UtcNow))
				{
					goto IL_4EC;
				}
			}
			Block_8:
			throw new ExportException(ExportErrorType.StopRequested);
			IL_4CB:
			throw ex;
			IL_4EC:
			if (ScenarioData.Current.ContainsKey("BI"))
			{
				ScenarioData.Current.Remove("BI");
			}
			if (ex2 != null)
			{
				Tracer.TraceError("BaseServiceClient.InternalCallService: Exception thrown after all possible actions: {0}", new object[]
				{
					ex2
				});
				throw ex2;
			}
		}

		protected bool Rebind()
		{
			TimeSpan t = new TimeSpan(0, 1, 0);
			Tracer.TraceInformation("BaseServiceClient.Rebind: Request to rebind Mailbox: {0} current Url: {1}", new object[]
			{
				this.CurrentMailbox,
				this.ServiceEndpoint
			});
			try
			{
				if (ConstantProvider.RebindWithAutoDiscoveryEnabled && ConstantProvider.RebindAutoDiscoveryUrl != null && !string.IsNullOrEmpty(this.CurrentMailbox) && this.AutoDiscoverInterface != null)
				{
					if (!this.ServiceEndpoint.Host.Equals(ConstantProvider.RebindAutoDiscoveryUrl.Host, StringComparison.InvariantCultureIgnoreCase))
					{
						if (!this.CurrentMailbox.Equals(this.lastRebindMailbox) || DateTime.UtcNow - this.lastRebindTime > t)
						{
							Tracer.TraceInformation("BaseServiceClient.Rebind: AutoDiscovery to rebind Mailbox: {0} current Url: {1} with AutoDiscovery: {2}", new object[]
							{
								this.CurrentMailbox,
								this.ServiceEndpoint,
								ConstantProvider.RebindAutoDiscoveryUrl
							});
							List<AutoDiscoverResult> userEwsEndpoints = this.AutoDiscoverInterface.GetUserEwsEndpoints(new string[]
							{
								this.CurrentMailbox
							});
							if (userEwsEndpoints != null && userEwsEndpoints.Count > 0 && userEwsEndpoints[0].ResultCode == AutoDiscoverResultCode.Success)
							{
								Uri uri = new Uri(userEwsEndpoints[0].ResultValue);
								if (!uri.Host.Equals(this.ServiceEndpoint.Host, StringComparison.InvariantCultureIgnoreCase))
								{
									Tracer.TraceInformation("BaseServiceClient.Rebind: Rebind Mailbox: {0} current Url: {1} with NewURL: {2}", new object[]
									{
										this.CurrentMailbox,
										this.ServiceEndpoint,
										uri
									});
									if (this.ServiceBinding != null)
									{
										ServiceBindingType serviceBinding = this.ServiceBinding;
										serviceBinding.Url = uri.ToString();
									}
									this.ServiceEndpoint = uri;
									this.lastRebindMailbox = this.CurrentMailbox;
									this.lastRebindTime = DateTime.UtcNow;
									return this.Connect();
								}
								Tracer.TraceError("BaseServiceClient.Rebind: Autodiscovered host is same as current Host: {0}", new object[]
								{
									this.ServiceEndpoint
								});
							}
							else
							{
								Tracer.TraceError("BaseServiceClient.Rebind: Autodiscovery failed with Error: {0}", new object[]
								{
									(userEwsEndpoints != null && userEwsEndpoints.Count > 0) ? userEwsEndpoints[0].ResultCode.ToString() : "Unknown"
								});
							}
						}
						else
						{
							Tracer.TraceError("BaseServiceClient.Rebind: AutoDiscovery attempted too frequently to rebind Mailbox: {0} current Url: {1} with AutoDiscovery: {2}", new object[]
							{
								this.CurrentMailbox,
								this.ServiceEndpoint,
								ConstantProvider.RebindAutoDiscoveryUrl
							});
						}
					}
					else
					{
						Tracer.TraceError("BaseServiceClient.Rebind: Autodiscovery host is same as failing Host: {0} ", new object[]
						{
							this.ServiceEndpoint
						});
					}
				}
				else
				{
					Tracer.TraceInformation("BaseServiceClient.Rebind: settings do not allow rebind", new object[0]);
				}
			}
			catch (Exception ex)
			{
				Tracer.TraceError("BaseServiceClient.Rebind: Exception thrown during action: {0}", new object[]
				{
					ex
				});
			}
			return false;
		}

		private int GetRetryWaitTime(int retryAttempt)
		{
			int num = (ConstantProvider.RetrySchedule == null) ? 0 : ConstantProvider.RetrySchedule.Length;
			TimeSpan timeSpan;
			if (retryAttempt < 0 || retryAttempt >= num)
			{
				timeSpan = ConstantProvider.RetrySchedule[num - 1];
			}
			else
			{
				timeSpan = ConstantProvider.RetrySchedule[retryAttempt];
			}
			return (int)timeSpan.TotalMilliseconds;
		}

		private string lastRebindMailbox;

		private DateTime lastRebindTime = DateTime.UtcNow;
	}
}

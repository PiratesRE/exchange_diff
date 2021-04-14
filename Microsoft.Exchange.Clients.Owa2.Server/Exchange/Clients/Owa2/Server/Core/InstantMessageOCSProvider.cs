using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.InstantMessaging;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class InstantMessageOCSProvider : InstantMessageProvider
	{
		static InstantMessageOCSProvider()
		{
			ActivityContext.RegisterMetadata(typeof(InstantMessageOCSProvider.InstantMessagingCallbackData));
		}

		private InstantMessageOCSProvider(IUserContext userContext, InstantMessageNotifier notifier) : base(userContext, notifier)
		{
			base.Notifier.ChangeUserPresenceAfterInactivity += this.ChangeUserPresenceAfterInactivity;
			this.serverName = InstantMessageProvider.OcsServerName;
			this.userState = InstantMessagePresenceType.Offline;
		}

		internal static IEndpointManager EndpointManager { get; private set; }

		internal static IUtilities Utilities { get; private set; }

		internal override bool IsUserUcsMode
		{
			get
			{
				return this.isUserInUCSMode;
			}
		}

		internal override bool IsSessionStarted
		{
			get
			{
				return this.isEndpointRegistered && this.isSelfDataEstablished && this.isContactGroupEstablished;
			}
		}

		private static string ApplicationUserAgent
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "OWA/{0}", new object[]
				{
					FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
				});
			}
		}

		private string TracingContext
		{
			get
			{
				if (this.tracingContext == null)
				{
					this.tracingContext = string.Format("User={0}, Sip address={1}, Lyncserver={2}", base.UserContext.PrimarySmtpAddress.IsValidAddress ? base.UserContext.PrimarySmtpAddress.ToString() : "null", (!string.IsNullOrEmpty(base.UserContext.SipUri)) ? base.UserContext.SipUri : "null", (!string.IsNullOrEmpty(this.serverName)) ? this.serverName : "null");
				}
				return this.tracingContext;
			}
		}

		public override string ToString()
		{
			return this.tracingContext + ", " + this.userState;
		}

		internal static bool InitializeProvider()
		{
			if (!InstantMessageOCSProvider.isInitialized)
			{
				lock (InstantMessageOCSProvider.initializationLock)
				{
					if (!InstantMessageOCSProvider.isInitialized)
					{
						Stopwatch stopwatch = Stopwatch.StartNew();
						int mtlsPortNumber;
						string certificateIssuer;
						byte[] certificateSerialNumber;
						if (!InstantMessageOCSProvider.TryLoadProviderSettings(out mtlsPortNumber, out certificateIssuer, out certificateSerialNumber))
						{
							return false;
						}
						stopwatch.Stop();
						InstantMessageProvider.Log(InstantMessageSignIn.LogMetadata.GetCertificate, stopwatch.ElapsedMilliseconds);
						InstantMessageOCSProvider.isInitialized = InstantMessageOCSProvider.InitializeEndpointManager(certificateIssuer, certificateSerialNumber, mtlsPortNumber);
					}
				}
			}
			return InstantMessageOCSProvider.isInitialized;
		}

		internal static bool InitializeEndpointManager(string certificateIssuer, byte[] certificateSerialNumber, int mtlsPortNumber)
		{
			bool flag = false;
			string text = string.Empty;
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (string.IsNullOrEmpty(OwaRegistryKeys.IMImplementationDllPath))
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. No registry setting for IM Provider.");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderNoRegistrySetting, string.Empty, new object[]
				{
					OwaRegistryKeys.IMImplementationDllPathKey,
					OwaRegistryKeys.IMImplementationDllPath
				});
				OwaDiagnostics.PublishMonitoringEventNotification(ExchangeComponent.OwaDependency.Name, Feature.InstantMessage.ToString(), "InstantMessageOCSProvider.InitializeEndpointManager. No registry setting for IM Provider.", ResultSeverityLevel.Error);
				return false;
			}
			if (!File.Exists(OwaRegistryKeys.IMImplementationDllPath))
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. IM provider not found.  File: {0}", new object[]
				{
					OwaRegistryKeys.IMImplementationDllPath
				});
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderFileDoesNotExist, string.Empty, new object[]
				{
					OwaRegistryKeys.IMImplementationDllPath
				});
				OwaDiagnostics.PublishMonitoringEventNotification(ExchangeComponent.OwaDependency.Name, Feature.InstantMessage.ToString(), "InstantMessageOCSProvider.InitializeEndpointManager. IM provider not found.", ResultSeverityLevel.Error);
				return false;
			}
			stopwatch.Stop();
			InstantMessageProvider.Log(InstantMessageSignIn.LogMetadata.FindProvider, stopwatch.ElapsedMilliseconds);
			IEndpointManager endpointManager = null;
			IUtilities utilities = null;
			Type[] array = new Type[]
			{
				typeof(string),
				typeof(byte[]),
				typeof(string)
			};
			object[] array2 = new object[]
			{
				certificateIssuer,
				certificateSerialNumber,
				InstantMessageOCSProvider.ApplicationUserAgent
			};
			Type[] array3 = new Type[0];
			object[] array4 = new object[0];
			Type[][] variousConstructorParameters = new Type[][]
			{
				array,
				array3
			};
			object[][] variousConstructorValues = new object[][]
			{
				array2,
				array4
			};
			Type[][] variousConstructorParameters2 = new Type[][]
			{
				array3
			};
			object[][] variousConstructorValues2 = new object[][]
			{
				array4
			};
			bool result;
			try
			{
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				Assembly assembly = Assembly.LoadFrom(OwaRegistryKeys.IMImplementationDllPath);
				stopwatch2.Stop();
				InstantMessageProvider.Log(InstantMessageSignIn.LogMetadata.LoadDll, stopwatch2.ElapsedMilliseconds);
				Stopwatch stopwatch3 = Stopwatch.StartNew();
				foreach (Type type in assembly.GetTypes())
				{
					bool flag2;
					InstantMessageOCSProvider.InitializeType<IEndpointManager>(type, ref endpointManager, variousConstructorParameters, variousConstructorValues, out flag2);
					if (flag2)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Failed during initialization of IEndPointManager.");
						return false;
					}
					InstantMessageOCSProvider.InitializeType<IUtilities>(type, ref utilities, variousConstructorParameters2, variousConstructorValues2, out flag2);
					if (flag2)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Failed during initialization of IUtilities.");
						return false;
					}
				}
				stopwatch3.Stop();
				InstantMessageProvider.Log(InstantMessageSignIn.LogMetadata.CreateEndpointManager, stopwatch3.ElapsedMilliseconds);
				if (endpointManager == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. No constructor found.");
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderNoValidConstructor, string.Empty, new object[]
					{
						OwaRegistryKeys.IMImplementationDllPath
					});
					result = false;
				}
				else
				{
					Stopwatch stopwatch4 = Stopwatch.StartNew();
					foreach (int num in InstantMessageOCSProvider.mtlsPortNumbers)
					{
						try
						{
							int num2 = (mtlsPortNumber == -1) ? num : mtlsPortNumber;
							ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Initializing Endpoint with port number : {0}", new object[]
							{
								num2
							});
							endpointManager.Initialize(null, num2);
							flag = true;
							OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_IMEndpointManagerInitializedSuccessfully);
							break;
						}
						catch (InstantMessagingException ex)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Exception hit while initializing.  Exception: {0}", new object[]
							{
								ex
							});
							text = ((ex.Message != null) ? ex.Message : string.Empty);
							if (ex.Code != 18105 || mtlsPortNumber != -1)
							{
								break;
							}
						}
					}
					stopwatch4.Stop();
					InstantMessageProvider.Log(InstantMessageSignIn.LogMetadata.InitializeEndpointManager, stopwatch4.ElapsedMilliseconds);
					if (!flag)
					{
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderExceptionDuringLoad, string.Empty, new object[]
						{
							OwaRegistryKeys.IMImplementationDllPath,
							text
						});
						result = false;
					}
					else
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessageOCSProvider.InitializeEndpointManager. EndpointManager initialized successfully");
						OwaDiagnostics.PublishMonitoringEventNotification(ExchangeComponent.OwaDependency.Name, Feature.InstantMessage.ToString(), "InstantMessageOCSProvider.InitializeEndpointManager. EndpointManager initialized successfully", ResultSeverityLevel.Informational);
						InstantMessageOCSProvider.EndpointManager = endpointManager;
						InstantMessageOCSProvider.Utilities = utilities;
						IEndpointManager2 endpointManager2 = endpointManager as IEndpointManager2;
						if (endpointManager2 != null)
						{
							endpointManager2.IsPrivacyModeSupported = true;
						}
						result = true;
					}
				}
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				text = ((innerException != null && innerException.Message != null) ? innerException.Message : string.Empty);
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Exception hit while initializing.  Exception: {0}", new object[]
				{
					innerException
				});
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderExceptionDuringLoad, string.Empty, new object[]
				{
					OwaRegistryKeys.IMImplementationDllPath,
					text
				});
				result = false;
			}
			return result;
		}

		internal static void DisposeEndpointManager()
		{
			if (InstantMessageOCSProvider.EndpointManager != null)
			{
				IDisposable disposable = InstantMessageOCSProvider.EndpointManager as IDisposable;
				if (disposable != null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessageOCSProvider.DisposeEndpointManager. Disposing EndpointManager");
					disposable.Dispose();
				}
				InstantMessageOCSProvider.EndpointManager = null;
			}
		}

		internal static InstantMessageOCSProvider Create(IUserContext userContext, InstantMessageNotifier notifier)
		{
			if (string.IsNullOrEmpty(userContext.SipUri))
			{
				return null;
			}
			return new InstantMessageOCSProvider(userContext, notifier);
		}

		private static bool TryLoadProviderSettings(out int mtlsPortNumber, out string certificateIssuer, out byte[] certificateSerialNumber)
		{
			mtlsPortNumber = -1;
			certificateIssuer = null;
			certificateSerialNumber = null;
			InstantMessagingConfiguration instance = InstantMessagingConfiguration.GetInstance(VdirConfiguration.Instance);
			if (!instance.CheckConfiguration())
			{
				return false;
			}
			mtlsPortNumber = instance.PortNumber;
			InstantMessageCertUtils.GetIMCertInfo(instance.CertificateThumbprint, out certificateIssuer, out certificateSerialNumber);
			return !string.IsNullOrEmpty(certificateIssuer) && certificateSerialNumber != null;
		}

		internal override int ParticipateInConversation(int conversationId)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Chat Id: {0}, Context: {1}", new object[]
			{
				conversationId,
				this.TracingContext
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. EndPoint is null.");
				return -3;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.ParticipateInConversation", InstantMessageServiceError.SessionDisconnected, null);
				return -4;
			}
			IConversation conversation = endpoint.GetConversation(conversationId);
			if (conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Conversation is null.");
				return -9;
			}
			if (conversation.IsConference)
			{
				conversation.BeginParticipate(new AsyncCallback(this.ParticipateCallback), conversation);
			}
			else
			{
				IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Instant Messaging Modality is null.");
					return -7;
				}
				iimmodality.BeginParticipate(new AsyncCallback(this.InstantMessagingParticipateCallback), iimmodality);
			}
			return conversationId;
		}

		internal override int SendChatMessage(ChatMessage message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendChatMessage. Context: {0}, Chat Id: {1}, ", new object[]
			{
				this.TracingContext,
				message.ChatSessionId
			});
			if (string.IsNullOrEmpty(message.Body))
			{
				return -5;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessage. EndPoint is null. Context: {0}", new object[]
				{
					this.TracingContext
				});
				return -3;
			}
			IConversation conversation = endpoint.GetConversation(message.ChatSessionId);
			if (conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessage. Conversation is null.");
				return this.SendNewChatMessage(message);
			}
			IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
			if (iimmodality == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessage. Instant Messaging Modality is null.");
				return -7;
			}
			if (conversation.State != 5)
			{
				this.QueueMessage(conversation, message.Format, message.Body);
			}
			else
			{
				this.SendAndClearMessageList(conversation.Cid);
				iimmodality.BeginSendMessage(message.Format, message.Body, new AsyncCallback(this.SendMessageCallback), iimmodality);
			}
			return conversation.Cid;
		}

		internal override int SendNewChatMessage(ChatMessage message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (string.IsNullOrEmpty(message.Body))
			{
				return -5;
			}
			if (message.Recipients == null || message.Recipients.Length == 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. message.Recipients is null or empty.");
				return -6;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. EndPoint is null.Context: {0}", new object[]
				{
					this.TracingContext
				});
				return -3;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. Sending UN response to the client Context: {0}", new object[]
				{
					this.TracingContext
				});
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SendNewChatMessage", InstantMessageServiceError.SessionDisconnected, null);
				return -4;
			}
			List<string> list;
			if (message.Recipients.Count((string str) => !InstantMessageUtilities.IsSipUri(str)) > 0)
			{
				list = new List<string>();
				foreach (string emailAddress in message.Recipients)
				{
					list.Add(InstantMessageUtilities.GetSipUri(emailAddress, base.UserContext));
				}
			}
			else
			{
				list = message.Recipients.ToList<string>();
			}
			IConversation conversation = endpoint.CreateConversation(list.ToArray(), message.Subject, this.defaultContentType, message.Body);
			if (conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. Conversation is null. Context: {0}", new object[]
				{
					this.TracingContext
				});
				return -8;
			}
			IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
			if (iimmodality == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. Instant Messaging Modality is null. Context: {0}", new object[]
				{
					this.TracingContext
				});
				return -7;
			}
			iimmodality.MessageReceived += new IMEventHandler(this.OnMessageReceived);
			iimmodality.MessageSendFailed += new IMEventHandler(this.OnMessageSendFailed);
			iimmodality.ComposingStateChanged += new IMEventHandler(this.OnComposingStateChanged);
			iimmodality.ModalityParticipantUpdated += new IMEventHandler(this.OnModalityParticipantUpdated);
			iimmodality.ModalityParticipantRemoved += new IMEventHandler(this.OnModalityParticipantRemoved);
			conversation.ConversationStateChanged += new IMEventHandler(this.OnConversationStateChanged);
			conversation.ParticipantUpdated += new IMEventHandler(this.OnParticipantUpdated);
			conversation.ParticipantRemoved += new IMEventHandler(this.OnParticipantRemoved);
			conversation.RegisterModality(iimmodality);
			conversation.BeginParticipate(new AsyncCallback(this.ParticipateCallback), conversation);
			return conversation.Cid;
		}

		internal override void EndChatSession(int chatSessionId, bool disconnectSession)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.EndChatSession. Context: {0}, Chat Id: {1}", new object[]
			{
				this.TracingContext,
				chatSessionId
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.EndChatSession. EndPoint list is null.");
				return;
			}
			IConversation conversation = endpoint.GetConversation(chatSessionId);
			if (conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.EndChatSession. Conversation is null.");
				return;
			}
			this.TerminateConversation(conversation);
		}

		internal override void NotifyTyping(int chatSessionId, bool typingCanceled)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Endpoint is null. Context: {0}", new object[]
					{
						this.TracingContext
					});
				}
				else
				{
					IConversation conversation = endpoint.GetConversation(chatSessionId);
					if (conversation == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Conversation is null.");
					}
					else
					{
						IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Instant Messaging Modality is null.");
						}
						else if (typingCanceled)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Idle.");
							iimmodality.BeginNotifyComposing(0, new AsyncCallback(this.NotifyComposingCallback), iimmodality);
						}
						else
						{
							ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Composing.");
							iimmodality.BeginNotifyComposing(1, new AsyncCallback(this.NotifyComposingCallback), iimmodality);
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Could not notify the typing state. {0}", new object[]
				{
					ex
				});
			}
		}

		internal void NotifyDeliverySuccess(DeliverySuccessNotification notification)
		{
			notification.Context.BeginNotifyDeliverySuccess(notification.MessageId, new AsyncCallback(this.NotifyDeliverySuccessCallback), notification);
		}

		internal void NotifyDeliverySuccessCallback(IAsyncResult result)
		{
			DeliverySuccessNotification deliverySuccessNotification = (DeliverySuccessNotification)result.AsyncState;
			IIMModality instantMessaging = deliverySuccessNotification.Context;
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.NotifyDeliverySuccessCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSNotifier.NotifyDeliverySuccessCallback.");
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "NotifyDeliverySuccessCallback"))
				{
					return;
				}
				if (instantMessaging == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyDeliverySuccessCallback. Instant Messaging Modality is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNull);
					return;
				}
				try
				{
					instantMessaging.EndNotifyDeliverySuccess(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception ex)
				{
					if (this.EndpointDisconnected(logger, ex, endpoint, "NotifyDeliverySuccessCallback"))
					{
						InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Notifier, "InstantMessageOCSProvider.InstantMessagingParticipateCallback", InstantMessageServiceError.SessionDisconnected, ex);
					}
					else
					{
						if (instantMessaging != null && instantMessaging.IsConnected)
						{
							throw;
						}
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyDeliverySuccessCallback. Ignoring exception because IM conversation is not connected : {0}.", new object[]
						{
							ex
						});
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNotConnected);
					}
				}
			});
		}

		internal override int PublishSelfPresence(InstantMessagePresenceType presence)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfPresence. Context: {0} , Presence: {1}", new object[]
			{
				this.TracingContext,
				presence
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfPresence. EndPoint is null.");
				return -3;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfPresence. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.PublishSelfPresence", InstantMessageServiceError.SessionDisconnected, null);
				return -4;
			}
			endpoint.BeginPublishSelfState(this.ConvertToUserState(presence), new AsyncCallback(this.PublishSelfStateCallback), endpoint);
			this.userState = presence;
			return 0;
		}

		internal override void PublishResetStatus()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishResetStatus. Context: {0}", new object[]
			{
				this.TracingContext
			});
			this.PublishSelfPresence(InstantMessagePresenceType.Online);
		}

		internal override void QueryPresence(string[] sipUris)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (sipUris == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. Subscription array is null.");
				return;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.QueryPresence", InstantMessageServiceError.SessionDisconnected, null);
				return;
			}
			try
			{
				HashSet<string> hashSet = new HashSet<string>();
				StringBuilder stringBuilder = new StringBuilder(6400);
				StringBuilder stringBuilder2 = new StringBuilder(6400);
				StringBuilder stringBuilder3 = new StringBuilder(6400);
				foreach (string text in sipUris)
				{
					bool flag = hashSet.Count < 128;
					bool flag2 = flag;
					if (flag2 && InstantMessageOCSProvider.Utilities != null)
					{
						flag2 = InstantMessageOCSProvider.Utilities.IsValidSipUri(text);
					}
					if (flag2)
					{
						hashSet.Add(text);
						stringBuilder = stringBuilder.Append(text);
						stringBuilder = stringBuilder.Append(",");
					}
					else
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. Skipping querying presence for {0} Context: {1}", new object[]
						{
							text,
							this.TracingContext
						});
						if (flag)
						{
							stringBuilder2 = stringBuilder2.Append(text);
							stringBuilder2 = stringBuilder2.Append(",");
						}
						else
						{
							stringBuilder3 = stringBuilder3.Append(text);
							stringBuilder3 = stringBuilder3.Append(",");
						}
					}
				}
				if (hashSet.Count == 0)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. Not a single sipUri is valid. Context: {0}.", new object[]
					{
						this.TracingContext
					});
				}
				else
				{
					endpoint.BeginQueryPresence(hashSet.ToArray<string>(), 1, true, new AsyncCallback(this.QueryPresenceCallback), endpoint);
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. Requesting presence for {0} sips with Context: {1}", new object[]
					{
						hashSet.Count,
						this.TracingContext
					});
					InstantMessageProvider.Log(InstantMessagingQueryPresenceData.LyncServer, this.serverName);
					InstantMessageProvider.Log(InstantMessagingQueryPresenceData.UCSMode, this.isUserInUCSMode);
					InstantMessageProvider.Log(InstantMessagingQueryPresenceData.PrivacyMode, this.isUserInPrivateMode);
					InstantMessageProvider.Log(InstantMessagingQueryPresenceData.QueriedSIPs, ExtensibleLogger.FormatPIIValue(stringBuilder.ToString()));
					InstantMessageProvider.Log(InstantMessagingQueryPresenceData.SkippedSIPs, ExtensibleLogger.FormatPIIValue(stringBuilder2.ToString()));
					InstantMessageProvider.Log(InstantMessagingQueryPresenceData.InvalidSIPs, ExtensibleLogger.FormatPIIValue(stringBuilder3.ToString()));
					InstantMessageProvider.Log(InstantMessagingQueryPresenceData.UserContext, base.UserContext.Key);
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback. Ignoring exception after the connection is closed : {0}.", new object[]
					{
						ex
					});
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.QueryPresence", InstantMessageServiceError.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. Exception message is {0}.", new object[]
						{
							ex
						});
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.QueryPresence", base.UserContext, ex);
					}
				}
			}
		}

		internal override void AddSubscription(string[] sipUris)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (sipUris == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Subscription array is null.");
				return;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddSubscription", InstantMessageServiceError.SessionDisconnected, null);
				return;
			}
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Adding subscription for {0} sips.Context: {1}.", new object[]
				{
					sipUris.Length,
					this.TracingContext
				});
				endpoint.BeginSubscribePresence(sipUris, new AsyncCallback(this.SubscribePresenceCallback), endpoint);
				StringBuilder stringBuilder = new StringBuilder(6400);
				foreach (string value in sipUris)
				{
					stringBuilder = stringBuilder.Append(value);
					stringBuilder = stringBuilder.Append(",");
				}
				this.RecordSubscriptionMetadata(stringBuilder.ToString(), null);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Ignoring exception after the connection is closed : {0}.", new object[]
					{
						ex
					});
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddSubscription", InstantMessageServiceError.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Exception message is {0}.", new object[]
						{
							ex
						});
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AddSubscription", base.UserContext, ex);
					}
				}
			}
		}

		internal override void RemoveSubscription(string sipUri)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveSubscription. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (sipUri == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveSubscription. Target is null.");
				return;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveSubscription. EndPoint is null.");
				return;
			}
			try
			{
				endpoint.BeginUnsubscribePresence(sipUri, new AsyncCallback(this.UnsubscribePresenceCallback), endpoint);
				this.RecordSubscriptionMetadata(null, sipUri);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveSubscription. Ignoring exception after the connection is closed : {0}.", new object[]
					{
						ex
					});
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RemoveSubscription", base.UserContext, ex);
				}
			}
		}

		internal override void AddBuddy(IMailboxSession mailboxsession, InstantMessageBuddy buddy, InstantMessageGroup group)
		{
			if (this.isUserInUCSMode)
			{
				this.AddBuddyInUcsMode(mailboxsession, buddy.FirstName, buddy.LastName, buddy.SipUri, buddy.DisplayName);
				return;
			}
			this.AddBuddy(buddy, group, false);
		}

		internal override void RemoveBuddy(IMailboxSession mailboxsession, InstantMessageBuddy buddy, StoreId contactId)
		{
			if (this.isUserInUCSMode)
			{
				this.RemoveBuddyInUcsMode(mailboxsession, buddy.SipUri, contactId);
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (buddy.SipUri == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. SipUri is null.");
				return;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. EndPoint list is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy.  Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RemoveBuddy", InstantMessageServiceError.SessionDisconnected, null);
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. Contact SIP Uri: {0}", new object[]
			{
				buddy.SipUri
			});
			try
			{
				endpoint.BeginDeleteContact(buddy.SipUri, new AsyncCallback(this.RemoveBuddyCallback), buddy.SipUri);
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. InvalidOperationException.");
					throw new OwaInvalidOperationException("Remove buddy threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. Exception message is {0}.", new object[]
				{
					ex
				});
				throw;
			}
		}

		internal override void AcceptBuddy(IMailboxSession mailboxsession, InstantMessageBuddy buddy, InstantMessageGroup group)
		{
			this.AddBuddy(buddy, group, true);
			this.RecordBuddyMetadata(buddy.SipUri, null);
		}

		internal override void DeclineBuddy(InstantMessageBuddy buddy)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddy. Context: {0}", new object[]
			{
				this.TracingContext
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddy. EndPoint is null.");
				return;
			}
			try
			{
				endpoint.BeginAckSubscriber(InstantMessageUtilities.FromSipFormat(buddy.SipUri), new AsyncCallback(this.DeclineBuddyAckSubscriberCallback), buddy.SipUri);
				this.RecordBuddyMetadata(buddy.SipUri, null);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(Strings.GetLocalizedString(-1660189394), InstantMessageUtilities.FromSipFormat(buddy.SipUri), CultureInfo.InvariantCulture);
				this.HandleFailedResult(errMsg);
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddy. {0}.", new object[]
				{
					ex
				});
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.DeclineBuddy", base.UserContext, exception);
			}
		}

		internal override void EstablishSession()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.EstablishSession. Context: {0}", new object[]
			{
				this.TracingContext
			});
			IEndpoint endpoint = this.ucEndpoint;
			InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload(base.Notifier, true, this.isUserInUCSMode);
			if (endpoint == null)
			{
				lock (this)
				{
					endpoint = this.ucEndpoint;
					if (endpoint == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.EstablishOCSSession. New Session.");
						this.CreateEndpointAndBeginSignIn();
						return;
					}
				}
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.EstablishOCSSession. Existing Session.");
			this.TerminateActiveConversations();
			if (endpoint.EndpointState == 2)
			{
				this.GetBuddyList();
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.EstablishSession. Sending UN response to the client");
			this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.EstablishSession", InstantMessageServiceError.SessionDisconnected, null);
		}

		internal override void GetExpandedGroups(MailboxSession session)
		{
			base.ExpandedGroupIds = InstantMessageUtilities.GetExpandedGroups(session);
		}

		private void TerminateOCSSession()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateOCSSession. Context: {0}", new object[]
			{
				this.TracingContext
			});
			this.TerminateActiveConversations();
			this.SignOutEndpoint(this.ucEndpoint);
		}

		protected override void CreateGroup(string groupName)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. Context: {0}", new object[]
			{
				this.TracingContext
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.CreateGroup", InstantMessageServiceError.SessionDisconnected, null);
				return;
			}
			try
			{
				endpoint.BeginAddGroup(groupName, null, new AsyncCallback(this.CreateGroupCallback), groupName);
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. InvalidOperationException.");
					throw new OwaInvalidOperationException("Add group threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. Exception message is {0}.", new object[]
				{
					ex
				});
				throw;
			}
		}

		internal override int ResetPresence()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (!this.isSelfDataEstablished)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence. SelfDataSession not established.");
				return 0;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence. Endpoint is null.");
				return -3;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.ResetPresence", InstantMessageServiceError.SessionDisconnected, null);
				return -4;
			}
			endpoint.BeginPublishMachineState(3700, new AsyncCallback(this.PublishMachineStateCallback), endpoint);
			this.IsActivityBasedPresenceSet = false;
			this.isCurrentlyActivityBasedAway = false;
			return 0;
		}

		private void AcceptBuddyAckSubscriber(string sipUri)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriber. Context: {0}", new object[]
			{
				this.TracingContext
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriber. EndPoint is null.");
				return;
			}
			try
			{
				endpoint.BeginAckSubscriber(InstantMessageUtilities.FromSipFormat(sipUri), new AsyncCallback(this.AcceptBuddyAckSubscriberCallback), sipUri);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(Strings.GetLocalizedString(-536320848), InstantMessageUtilities.FromSipFormat(sipUri), CultureInfo.InvariantCulture);
				this.HandleFailedResult(errMsg);
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriber. {0}.", new object[]
				{
					ex
				});
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AcceptBuddyAckSubscriber", base.UserContext, exception);
			}
		}

		protected override void GetBuddyList()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.GetBuddyList. Context: {0}", new object[]
			{
				this.TracingContext
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.GetBuddyList. Endpoint is null.");
				return;
			}
			endpoint.BeginRefreshPresenceSession(new AsyncCallback(this.RefreshPresenceSessionCallback), endpoint);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.InternalDispose. Disposing: {0} Context: {1}", new object[]
			{
				isDisposing,
				this.TracingContext
			});
			if (isDisposing)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.InternalDispose StackTrace: {0}{1}", new object[]
				{
					Environment.NewLine,
					new StackTrace().ToString()
				});
				if (base.Notifier != null)
				{
					base.Notifier.ChangeUserPresenceAfterInactivity -= this.ChangeUserPresenceAfterInactivity;
				}
				this.TerminateOCSSession();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.InternalGetDisposeTracker.");
			return DisposeTracker.Get<InstantMessageOCSProvider>(this);
		}

		private static void InitializeType<T>(Type type, ref T foundInstance, Type[][] variousConstructorParameters, object[][] variousConstructorValues, out bool terminalError)
		{
			terminalError = false;
			if (!type.IsClass || !type.IsPublic || type.GetInterface(typeof(T).FullName) == null)
			{
				return;
			}
			if (foundInstance != null)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderMultipleClasses, string.Empty, new object[]
				{
					OwaRegistryKeys.IMImplementationDllPath
				});
				terminalError = true;
				return;
			}
			for (int i = 0; i < variousConstructorParameters.Length; i++)
			{
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, variousConstructorParameters[i], null);
				if (constructor != null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessageOCSProvider.Initialize. Creating: {0}", new object[]
					{
						typeof(T).FullName
					});
					foundInstance = (T)((object)Activator.CreateInstance(type, variousConstructorValues[i]));
					return;
				}
			}
		}

		private void AddBuddy(InstantMessageBuddy buddy, InstantMessageGroup group, bool ackSubscriber)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (buddy.SipUri == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. SipUri is null.");
				throw new OwaInvalidOperationException("SipUri is null.");
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. EndPoint is null.");
				throw new OwaInvalidOperationException("EndPoint is null.");
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddBuddy", InstantMessageServiceError.SessionDisconnected, null);
				return;
			}
			if (group.Id == null)
			{
				if (group.GroupType != InstantMessageGroupType.OtherContacts)
				{
					throw new OwaInvalidOperationException("Group id is null");
				}
				if (this.otheContactsGroupId == null)
				{
					throw new OwaInvalidOperationException("The OtherContacts group is not created for user" + buddy.SipUri);
				}
				group.Id = this.otheContactsGroupId;
			}
			int num;
			if (!int.TryParse(group.Id, out num))
			{
				throw new OwaInvalidOperationException("The group id is not valid. GroupId:" + group.Id);
			}
			int[] array = new int[]
			{
				num
			};
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. Contact SIP Uri: {0}", new object[]
			{
				buddy.SipUri
			});
			InstantMessageOCSProvider.AddBuddyContext addBuddyContext = new InstantMessageOCSProvider.AddBuddyContext(buddy, ackSubscriber);
			try
			{
				if (ackSubscriber)
				{
					this.AcceptBuddyAckSubscriber(InstantMessageUtilities.ToSipFormat(buddy.SipUri));
				}
				else
				{
					endpoint.BeginAddContact(InstantMessageUtilities.ToSipFormat(buddy.SipUri), string.Empty, true, false, array, new AsyncCallback(this.AddBuddyCallback), addBuddyContext);
				}
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. InvalidOperationException.");
					throw new OwaInvalidOperationException("Add buddy threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. Exception message is {0}.", new object[]
				{
					ex
				});
				throw;
			}
		}

		private void DeclineBuddyAckSubscriberCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.DeclineBuddyAckSubscriberCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				string sipAddress = result.AsyncState as string;
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback. Endpoint is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EndpointNull);
					return;
				}
				try
				{
					endpoint.EndAckSubscriber(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception ex)
				{
					this.RecordExceptionProperties(logger, ex);
					if (!this.isUserInPrivateMode)
					{
						string errMsg = string.Format(Strings.GetLocalizedString(-1660189394), InstantMessageUtilities.FromSipFormat(sipAddress), CultureInfo.InvariantCulture);
						this.HandleFailedResult(errMsg);
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback. {0}.", new object[]
						{
							ex
						});
					}
					else
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback. Ignoring exception on auto acknowledge in privacy + notification mode.");
					}
					throw;
				}
			});
		}

		private void ChangeUserPresenceAfterInactivity(object sender, EventArgs e)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (!this.isSelfDataEstablished)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. SelfDataSession not established.");
				return;
			}
			if (this.IsActivityBasedPresenceSet)
			{
				long num = InstantMessageProvider.GetElapsedMilliseconds() - base.UserContext.LastUserRequestTime;
				if (this.isCurrentlyActivityBasedAway || num < (long)(InstantMessageProvider.ActivityBasedPresenceDuration * 2))
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. No presence change needed for now.");
					return;
				}
			}
			if (this.userState == InstantMessagePresenceType.Online || this.userState == InstantMessagePresenceType.IdleOnline || this.userState == InstantMessagePresenceType.Busy || this.userState == InstantMessagePresenceType.IdleBusy || this.userState == InstantMessagePresenceType.DoNotDisturb)
			{
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. EndPoint list is null.");
					return;
				}
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. Sending UN response to the client");
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity", InstantMessageServiceError.SessionDisconnected, null);
					return;
				}
				if (!this.IsActivityBasedPresenceSet && (this.userState == InstantMessagePresenceType.Online || this.userState == InstantMessagePresenceType.Busy || this.userState == InstantMessagePresenceType.DoNotDisturb))
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. Changing machine state to IdleOnline.");
					endpoint.BeginPublishMachineState(5200, new AsyncCallback(this.PublishMachineStateCallback), endpoint);
					this.IsActivityBasedPresenceSet = true;
					return;
				}
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. Changing machine state to Away.");
				endpoint.BeginPublishMachineState(15700, new AsyncCallback(this.PublishMachineStateCallback), endpoint);
				this.IsActivityBasedPresenceSet = true;
				this.isCurrentlyActivityBasedAway = true;
			}
		}

		private UserStateEnum ConvertToUserState(InstantMessagePresenceType presence)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ConvertToUserState");
			if (presence <= InstantMessagePresenceType.Busy)
			{
				if (presence == InstantMessagePresenceType.None)
				{
					return 0;
				}
				if (presence == InstantMessagePresenceType.Online)
				{
					return 3500;
				}
				if (presence == InstantMessagePresenceType.Busy)
				{
					return 6500;
				}
			}
			else if (presence <= InstantMessagePresenceType.BeRightBack)
			{
				if (presence == InstantMessagePresenceType.DoNotDisturb)
				{
					return 9500;
				}
				if (presence == InstantMessagePresenceType.BeRightBack)
				{
					return 12500;
				}
			}
			else
			{
				if (presence == InstantMessagePresenceType.Away)
				{
					return 15500;
				}
				if (presence == InstantMessagePresenceType.Offline)
				{
					return 18500;
				}
			}
			return 0;
		}

		private void TerminateConversation(IConversation conversation)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateConversation. Conversation ID: {0} Context: {1}", new object[]
			{
				conversation.Cid,
				this.TracingContext
			});
			conversation.BeginTerminate(new AsyncCallback(this.ConversationTerminateCallback), conversation);
		}

		private void TerminateActiveConversations()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateActiveConversations. Context: {0}", new object[]
			{
				this.TracingContext
			});
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateActiveConversations. EndPoint list is null.");
				return;
			}
			ICollection<IConversation> conversations = endpoint.GetConversations();
			foreach (IConversation conversation in conversations)
			{
				this.TerminateConversation(conversation);
			}
		}

		private void SignOutCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.SignOutCallback", delegate(RequestDetailsLogger logger)
			{
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				try
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SignOutCallback. Context: {0}", new object[]
					{
						this.TracingContext
					});
					if (endpoint == null)
					{
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EndpointNull);
					}
					else
					{
						endpoint.EndSignOut(result);
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "SignOutCallback"))
					{
						throw;
					}
				}
			});
		}

		private void QueryPresenceCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.QueryPresenceCallback", delegate(RequestDetailsLogger logger)
			{
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "QueryPresenceCallback"))
				{
					return;
				}
				try
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback. Context: {0}", new object[]
					{
						this.TracingContext
					});
					endpoint.EndQueryPresence(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "QueryPresenceCallback"))
					{
						throw;
					}
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.QueryPresenceCallback", InstantMessageServiceError.SessionDisconnected, exception);
				}
			});
		}

		private void PublishSelfStateCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.PublishSelfStateCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfStateCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "PublishSelfStateCallback"))
				{
					return;
				}
				try
				{
					endpoint.EndPublishSelfState(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception ex)
				{
					if (!this.EndpointDisconnected(logger, ex, endpoint, "PublishSelfStateCallback"))
					{
						throw;
					}
					this.GenerateOperationFailurePayload("PublishSelfStateCallback", InstantMessagePayload.ReportError(ex.Message, InstantMessageOperationType.PresenceChange), ex);
				}
			});
		}

		private void PublishMachineStateCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.PublishMachineStateCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMachineStateCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMachineStateCallback. Endpoint is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EndpointNull);
					return;
				}
				try
				{
					endpoint.EndPublishMachineState(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception ex)
				{
					if (!this.EndpointDisconnected(logger, ex, endpoint, "PublishMachineStateCallback"))
					{
						this.GenerateOperationFailurePayload("PublishMachineStateCallback", InstantMessagePayload.ReportError(ex.Message, InstantMessageOperationType.Unspecified), ex);
						throw;
					}
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.PublishMachineStateCallback", InstantMessageServiceError.SessionDisconnected, ex);
				}
			});
		}

		private void AddBuddyCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.AddBuddyCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddyCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "AddBuddyCallback"))
				{
					return;
				}
				InstantMessageOCSProvider.AddBuddyContext addBuddyContext = result.AsyncState as InstantMessageOCSProvider.AddBuddyContext;
				if (addBuddyContext == null)
				{
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.AsyncStateNull);
					return;
				}
				if (addBuddyContext.Buddy == null)
				{
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					return;
				}
				try
				{
					endpoint.EndAddContact(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					this.GenerateOperationFailurePayload("AddBuddyCallback", InstantMessagePayload.ReportError(addBuddyContext.Buddy.SipUri, InstantMessageOperationType.AddContact), exception);
					if (!this.EndpointDisconnected(logger, exception, endpoint, "AddBuddyCallback"))
					{
						throw;
					}
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddBuddyCallback", InstantMessageServiceError.SessionDisconnected, exception);
				}
			});
		}

		private void AcceptBuddyAckSubscriberCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.AcceptBuddyAckSubscriberCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriberCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				string sipAddress = result.AsyncState as string;
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "AcceptBuddyAckSubscriberCallback"))
				{
					return;
				}
				try
				{
					endpoint.EndAckSubscriber(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					string errMsg = string.Format(Strings.GetLocalizedString(-536320848), InstantMessageUtilities.FromSipFormat(sipAddress), CultureInfo.InvariantCulture);
					this.HandleFailedResult(errMsg);
					if (!this.EndpointDisconnected(logger, exception, endpoint, "AcceptBuddyAckSubscriberCallback"))
					{
						throw;
					}
				}
			});
		}

		private void RemoveBuddyCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.RemoveBuddyCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				string text = null;
				text = (result.AsyncState as string);
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "RemoveBuddyCallback"))
				{
					return;
				}
				Culture.InternalSetAsyncThreadCulture(this.UserContext.UserCulture);
				try
				{
					endpoint.EndDeleteContact(result);
					InstantMessageNotifier notifier = this.Notifier;
					if (notifier == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyCallback. Notifier is null.");
					}
					else
					{
						lock (notifier)
						{
							notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.DeleteBuddy)
							{
								DeletedBuddySipUrls = new string[]
								{
									text
								}
							});
						}
						notifier.PickupData();
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
					}
				}
				catch (Exception exception)
				{
					this.GenerateOperationFailurePayload("RemoveBuddyCallback", InstantMessagePayload.ReportError(text, InstantMessageOperationType.RemoveContact), exception);
					if (!this.EndpointDisconnected(logger, exception, endpoint, "RemoveBuddyCallback"))
					{
						throw;
					}
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RemoveBuddyCallback", InstantMessageServiceError.SessionDisconnected, exception);
				}
			});
		}

		private void SubscribePresenceCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.SubscribePresenceCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SubscribePresenceCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "SubscribePresenceCallback"))
				{
					return;
				}
				try
				{
					endpoint.EndSubscribePresence(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "SubscribePresenceCallback"))
					{
						throw;
					}
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SubscribePresenceCallback", InstantMessageServiceError.SessionDisconnected, exception);
				}
			});
		}

		private void UnsubscribePresenceCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.UnsubscribePresenceCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.UnsubscribePresenceCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "UnsubscribePresenceCallback"))
				{
					return;
				}
				try
				{
					endpoint.EndUnsubscribePresence(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "UnsubscribePresenceCallback"))
					{
						throw;
					}
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.UnsubscribePresenceCallback", InstantMessageServiceError.SessionDisconnected, exception);
				}
			});
		}

		private void RefreshPresenceSessionCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.RefreshPresenceSessionCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RefreshPresenceSessionCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = null;
				endpoint = (result.AsyncState as IEndpoint);
				if (this.EndpointNullOrNotConnected(logger, endpoint, "RefreshPresenceSessionCallback"))
				{
					return;
				}
				try
				{
					endpoint.EndRefreshPresenceSession(result);
					InstantMessagePayloadUtilities.GenerateUpdatePresencePayload(this.Notifier, (int)this.userState);
					InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload(this.Notifier, false, this.isUserInUCSMode);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RefreshPresenceSessionCallback", InstantMessageServiceError.SessionDisconnected, exception);
					if (!this.EndpointDisconnected(logger, exception, endpoint, "RefreshPresenceSessionCallback"))
					{
						throw;
					}
				}
			});
		}

		private void SignInCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.SignInCallback", delegate(RequestDetailsLogger logger)
			{
				IEndpoint endpoint = null;
				try
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback. Context: {0}", new object[]
					{
						this.TracingContext
					});
					endpoint = (result.AsyncState as IEndpoint);
					if (endpoint == null)
					{
						if (this.Notifier != null)
						{
							this.Notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.ServiceUnavailable)
							{
								ServiceError = InstantMessageServiceError.SipEndpointConnectionFailure
							});
						}
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback. EndPoint is null. Context: {0}", new object[]
						{
							this.TracingContext
						});
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EndpointNull);
					}
					else
					{
						endpoint.EndSignIn(result);
						if (this.IsDisposed)
						{
							this.TerminateEndpoint(endpoint);
						}
						else
						{
							this.IsEarlierSignInSuccessful = true;
							bool arePerfCountersEnabled = InstantMessageProvider.ArePerfCountersEnabled;
							IEndpoint endpoint2 = Interlocked.Exchange<IEndpoint>(ref this.ucEndpoint, endpoint);
							if (endpoint2 != null)
							{
								this.TerminateEndpoint(endpoint2);
								if (InstantMessageProvider.ArePerfCountersEnabled)
								{
									bool flag = this.isEndpointRegistered;
								}
							}
							this.isEndpointRegistered = true;
							IEndpoint2 endpoint3 = endpoint as IEndpoint2;
							if (endpoint3 != null && endpoint3.PrivacyModeState == 2)
							{
								this.ProcessSubscribers();
								this.isUserInPrivateMode = true;
							}
							if (endpoint3 != null && endpoint3.UcsMigrationState == 2)
							{
								this.isUserInUCSMode = true;
							}
							if (this.otheContactsGroupId == null && !this.isUserInUCSMode)
							{
								this.CreateGroup("~");
							}
							InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload(this.Notifier, false, this.isUserInUCSMode);
							lock (this)
							{
								if (this.isRefreshNeeded)
								{
									this.GetBuddyList();
									this.isRefreshNeeded = false;
								}
							}
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
						}
					}
				}
				catch (InstantMessagingException ex)
				{
					this.RecordExceptionProperties(logger, ex);
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback failed. Context {0}, Exception: {1}", new object[]
					{
						this.TracingContext,
						ex
					});
					if (endpoint.EndpointState != 2 && endpoint.EndpointState != 1)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback. Ignoring exception after the connection is closed: Context {0}, Exception: {1}", new object[]
						{
							this.TracingContext,
							ex
						});
						this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SessionDisconnected, ex);
					}
					InstantMessagingError code = ex.Code;
					if (code != 18103)
					{
						switch (code)
						{
						case 18200:
							this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SignInFailure, ex);
							goto IL_593;
						case 18201:
							if (ex.SubCode == 504)
							{
								this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SipEndpointOperationTimeout, ex);
							}
							else
							{
								this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SipEndpointFailureResponse, ex);
							}
							if (InstantMessageProvider.ArePerfCountersEnabled)
							{
								bool isEarlierSignInSuccessful = this.IsEarlierSignInSuccessful;
							}
							this.IsEarlierSignInSuccessful = false;
							goto IL_593;
						case 18202:
						{
							this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SipEndpointRegister, ex);
							InstantMessagingErrorSubCode subCode = ex.SubCode;
							if (subCode == 484 || subCode == 504)
							{
								if (InstantMessageProvider.ArePerfCountersEnabled)
								{
									bool isEarlierSignInSuccessful2 = this.IsEarlierSignInSuccessful;
								}
								this.IsEarlierSignInSuccessful = false;
								goto IL_593;
							}
							goto IL_593;
						}
						case 18203:
							goto IL_593;
						case 18204:
							break;
						default:
							if (code != 18302)
							{
								goto IL_593;
							}
							break;
						}
						this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SipEndpointConnectionFailure, ex);
					}
					else
					{
						switch (ex.SubCode)
						{
						case 15:
							this.GenerateOperationFailurePayload("SignInCallback. Unsupported Legacy User.", new InstantMessagePayload(InstantMessagePayloadType.UnsupportedLegacyUser), ex);
							lock (this.ucEndpoint)
							{
								this.ucEndpoint = null;
							}
							endpoint = (result.AsyncState as IEndpoint);
							if (endpoint == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback. EndPoint is null.Context: {0}", new object[]
								{
									this.TracingContext
								});
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EndpointNull);
								return;
							}
							endpoint.BeginSignOut(new AsyncCallback(this.SignOutCallback), endpoint);
							break;
						case 16:
							InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Notifier, ex, "Unable to sign in due to privacy migration in progress.", InstantMessageServiceError.PrivacyMigrationInProgress, false);
							break;
						case 17:
							InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Notifier, ex, "Unable to sign in due to privacy migration needed.", InstantMessageServiceError.PrivacyMigrationNeeded, false);
							break;
						case 18:
							InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Notifier, ex, "Unable to sign in due to privacy policy changed.", InstantMessageServiceError.PrivacyPolicyChanged, false);
							break;
						default:
							this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SipEndpointOperationTimeout, ex);
							if (InstantMessageProvider.ArePerfCountersEnabled)
							{
								bool isEarlierSignInSuccessful3 = this.IsEarlierSignInSuccessful;
							}
							this.IsEarlierSignInSuccessful = false;
							break;
						}
					}
					IL_593:
					throw;
				}
				catch (Exception ex2)
				{
					this.RecordExceptionProperties(logger, ex2);
					if (InstantMessageProvider.ArePerfCountersEnabled)
					{
						bool isEarlierSignInSuccessful4 = this.IsEarlierSignInSuccessful;
					}
					this.IsEarlierSignInSuccessful = false;
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback failed. Context: {0}, Exception: {1}", new object[]
					{
						this.TracingContext,
						ex2
					});
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageServiceError.SignInFailure, ex2);
					throw;
				}
				finally
				{
					this.isSigningIn = false;
				}
			});
		}

		private void ConversationTerminateCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.ConversationTerminateCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ConversationTerminateCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "ConversationTerminateCallback"))
				{
					return;
				}
				IConversation conversation = result.AsyncState as IConversation;
				if (conversation == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ConversationTerminateCallback. Conversation is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
					return;
				}
				int cid = conversation.Cid;
				try
				{
					conversation.EndTerminate(result);
					if (this.messageQueueDictionary != null && this.messageQueueDictionary.ContainsKey(cid))
					{
						InstantMessageQueue instantMessageQueue = this.messageQueueDictionary[cid];
						this.messageQueueDictionary.Remove(cid);
						instantMessageQueue.Clear();
					}
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "ConversationTerminateCallback"))
					{
						throw;
					}
				}
			});
		}

		private void ParticipateCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.ParticipateCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "ParticipateCallback"))
				{
					return;
				}
				IConversation conversation = result.AsyncState as IConversation;
				if (conversation == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback. Conversation is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
					return;
				}
				try
				{
					conversation.EndParticipate(result);
					this.SendAndClearMessageList(conversation.Cid);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception ex)
				{
					InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.Notifier, "InstantMessageOCSProvider.ParticipateCallback", conversation.Cid, ex);
					if (this.EndpointDisconnected(logger, ex, endpoint, "ParticipateCallback"))
					{
						InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Notifier, "InstantMessageOCSProvider.ParticipateCallback", InstantMessageServiceError.SessionDisconnected, ex);
					}
					else if (conversation == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback: conversation is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
					}
					else
					{
						if (conversation.State == 5)
						{
							InstantMessagingException ex2 = ex as InstantMessagingException;
							if (ex2 != null)
							{
								InstantMessagingError code = ex2.Code;
								if (code == 0 || code == 18201 || code == 18206)
								{
									bool arePerfCountersEnabled = InstantMessageProvider.ArePerfCountersEnabled;
								}
							}
							throw;
						}
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback: conversation is not connected.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNotConnected);
					}
				}
			});
		}

		private void InstantMessagingParticipateCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.InstantMessagingParticipateCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.InstantMessagingParticipateCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IIMModality iimmodality = null;
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "ParticipateCallback"))
				{
					return;
				}
				try
				{
					iimmodality = (result.AsyncState as IIMModality);
					if (iimmodality == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.InstantMessagingParticipateCallback. Instant Messaging Modality is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNull);
					}
					else
					{
						iimmodality.EndParticipate(result);
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
					}
				}
				catch (Exception ex)
				{
					if (this.EndpointDisconnected(logger, ex, endpoint, "InstantMessagingParticipateCallback"))
					{
						InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Notifier, "InstantMessageOCSProvider.InstantMessagingParticipateCallback", InstantMessageServiceError.SessionDisconnected, ex);
					}
					else
					{
						if (iimmodality != null && iimmodality.IsConnected)
						{
							throw;
						}
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.InstantMessagingParticipateCallback. Ignoring exception because IM conversation is not connected : {0}.", new object[]
						{
							ex
						});
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNotConnected);
					}
				}
			});
		}

		private void NotifyComposingCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.NotifyComposingCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IIMModality iimmodality = null;
				IEndpoint endpoint = this.ucEndpoint;
				try
				{
					if (!this.EndpointNullOrNotConnected(logger, endpoint, "NotifyComposingCallback"))
					{
						iimmodality = (result.AsyncState as IIMModality);
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback. Instant Messaging Modality is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNull);
						}
						else
						{
							iimmodality.EndNotifyComposing(result);
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
						}
					}
				}
				catch (Exception ex)
				{
					if (this.EndpointDisconnected(logger, ex, endpoint, "NotifyComposingCallback"))
					{
						InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Notifier, "InstantMessageOCSProvider.NotifyComposingCallback", InstantMessageServiceError.SessionDisconnected, ex);
					}
					else
					{
						if (iimmodality != null && iimmodality.IsConnected)
						{
							throw;
						}
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback. Ignoring exception because IM conversation is not connected : {0}.", new object[]
						{
							ex
						});
					}
				}
			});
		}

		private void SendMessageCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.SendMessageCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessageCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IIMModality iimmodality = null;
				IEndpoint endpoint = this.ucEndpoint;
				try
				{
					if (!this.EndpointNullOrNotConnected(logger, endpoint, "SendMessageCallback"))
					{
						iimmodality = (result.AsyncState as IIMModality);
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessageCallback. Instant Messaging Modality is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNull);
						}
						else
						{
							iimmodality.EndSendMessage(result);
							bool arePerfCountersEnabled = InstantMessageProvider.ArePerfCountersEnabled;
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
						}
					}
				}
				catch (Exception ex)
				{
					InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.Notifier, "InstantMessageOCSProvider.SendMessageCallback", (iimmodality == null || iimmodality.Conversation == null) ? 0 : iimmodality.Conversation.Cid, ex);
					if (!this.EndpointDisconnected(logger, ex, endpoint, "SendMessageCallback"))
					{
						if (iimmodality != null && iimmodality.IsConnected)
						{
							InstantMessagingException ex2 = ex as InstantMessagingException;
							if (ex2 != null)
							{
								InstantMessagingError code = ex2.Code;
								if (code == 0 || code == 18102 || code == 18201)
								{
									bool arePerfCountersEnabled2 = InstantMessageProvider.ArePerfCountersEnabled;
								}
							}
							throw;
						}
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessageCallback. Ignoring exception because IM conversation is not connected : {0}.", new object[]
						{
							ex
						});
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNotConnected);
					}
				}
			});
		}

		private void CreateGroupCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.CreateGroupCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroupCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				string arg = result.AsyncState as string;
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "CreateGroupCallback"))
				{
					return;
				}
				Culture.InternalSetAsyncThreadCulture(this.UserContext.UserCulture);
				try
				{
					endpoint.EndAddGroup(result);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					string errMsg = string.Format(Strings.GetLocalizedString(-207298619), arg, CultureInfo.InvariantCulture);
					this.HandleFailedGroupEditResult(errMsg);
					if (!this.EndpointDisconnected(logger, exception, endpoint, "CreateGroupCallback"))
					{
						throw;
					}
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.CreateGroupCallback", InstantMessageServiceError.SessionDisconnected, exception);
				}
			});
		}

		private void GenerateInstantMessageUnavailablePayload(IEndpoint endPoint, string methodName, InstantMessageServiceError errorCode, Exception exception)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.GenerateInstantMessageUnavailablePayload. Context: {0}. endPoint: {1}, MethodName: {2}, InstantMessageServiceError: {3}, Exception: {4}", new object[]
			{
				this.TracingContext,
				(endPoint != null) ? endPoint.ToString() : "null",
				(methodName != null) ? methodName : "null",
				errorCode.ToString(),
				(exception != null) ? exception.ToString() : "null"
			});
			InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(base.Notifier, methodName, errorCode, exception);
			this.SignOutEndpoint(endPoint);
		}

		private void SignOutEndpoint(IEndpoint endPoint)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SignOutEndpoint. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (endPoint != null)
			{
				IEndpoint endpoint = Interlocked.CompareExchange<IEndpoint>(ref this.ucEndpoint, null, endPoint);
				if (this.ucEndpoint == null && endpoint != null)
				{
					if (InstantMessageProvider.ArePerfCountersEnabled)
					{
						bool flag = this.isEndpointRegistered;
					}
					this.TerminateEndpoint(endpoint);
					this.isEndpointRegistered = false;
					this.isSelfDataEstablished = false;
					this.isContactGroupEstablished = false;
					this.otheContactsGroupId = null;
					this.isUserInPrivateMode = false;
					this.userState = InstantMessagePresenceType.Offline;
				}
			}
		}

		private void TerminateEndpoint(IEndpoint endpoint)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateEndpoint. Context: {0}", new object[]
			{
				this.TracingContext
			});
			endpoint.ContactGroupChanged -= new IMEventHandler(this.OnContactGroupChanged);
			endpoint.UserPresenceChanged -= new IMEventHandler(this.OnUserPresenceChanged);
			endpoint.SelfPresenceChanged -= new IMEventHandler(this.OnSelfPresenceChanged);
			endpoint.ConversationReceived -= new IMEventHandler(this.OnConversationReceived);
			endpoint.QueryPresenceResultsReceived -= new IMEventHandler(this.OnQueryPresenceChanged);
			IEndpoint2 endpoint2 = endpoint as IEndpoint2;
			if (endpoint2 != null)
			{
				endpoint2.SelfPresenceSubscriptionStateUpdated -= new IMEventHandler(this.OnSelfPresenceSubscriptionStateUpdated);
				endpoint2.SubscriberChanged -= new IMEventHandler(this.OnSubscriberChanged);
			}
			endpoint.BeginSignOut(new AsyncCallback(this.SignOutCallback), endpoint);
		}

		private void GenerateOperationFailurePayload(string methodName, InstantMessagePayload payload, Exception exception)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.{0} failed. {1}. Context: {2}", new object[]
			{
				methodName,
				exception,
				this.TracingContext
			});
			InstantMessageNotifier notifier = base.Notifier;
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.GenerateOperationFailurePayload. Notifier is null.");
				return;
			}
			lock (notifier)
			{
				notifier.Add(payload);
			}
			notifier.PickupData();
		}

		private void PresenceChanged(object sender, IMEventArgs arguments, bool isQuery, RequestDetailsLogger logger)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. Context: {0}", new object[]
			{
				this.TracingContext
			});
			StringBuilder stringBuilder = new StringBuilder(6400);
			StringBuilder stringBuilder2 = new StringBuilder(6400);
			IUserPresenceEvent userPresenceEvent = (IUserPresenceEvent)arguments.Event;
			if (userPresenceEvent == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. User presence event is null.");
				return;
			}
			InstantMessageNotifier notifier = base.Notifier;
			if (notifier == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. Notifier is null.");
				return;
			}
			List<PresenceChange> list = new List<PresenceChange>();
			foreach (IPresenceNotificationData presenceNotificationData in userPresenceEvent.NotificationDataList)
			{
				INonRichPresenceData nonRichPresenceData = presenceNotificationData.NonRichPresenceData;
				if (nonRichPresenceData != null)
				{
					PresenceChange item = new PresenceChange
					{
						SipUri = nonRichPresenceData.TargetUri,
						Presence = nonRichPresenceData.AggregatePresenceState,
						UserName = nonRichPresenceData.UserName
					};
					list.Add(item);
				}
				else
				{
					try
					{
						IRichPresenceData richPresenceData = presenceNotificationData.RichPresenceData;
						if (richPresenceData != null)
						{
							IAggregateStateCategoryItem aggregateState = richPresenceData.AggregateState;
							long presence = 18000L;
							if (aggregateState != null)
							{
								presence = aggregateState.Availability;
							}
							string text = null;
							if (richPresenceData.ContactCard != null && richPresenceData.ContactCard.DisplayName != null)
							{
								text = richPresenceData.ContactCard.DisplayName;
								text = text.Trim();
							}
							PresenceChange item2 = new PresenceChange
							{
								SipUri = richPresenceData.Uri,
								Presence = this.MapToInstantMessagePresence(presence),
								UserName = text
							};
							list.Add(item2);
						}
					}
					catch (XmlException ex)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. Bad contact. {0}", new object[]
						{
							ex
						});
						return;
					}
					catch (ArgumentException ex2)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. ArgumentException. {0}", new object[]
						{
							ex2
						});
					}
				}
				if (presenceNotificationData.Categories.Count > 0)
				{
					stringBuilder = stringBuilder.Append(presenceNotificationData.Target);
					stringBuilder = stringBuilder.Append(",");
				}
				else
				{
					stringBuilder2 = stringBuilder2.Append(presenceNotificationData.Target);
					stringBuilder2 = stringBuilder2.Append(",");
				}
			}
			if (list.Count > 0)
			{
				lock (notifier)
				{
					InstantMessagePayloadType payloadType = isQuery ? InstantMessagePayloadType.QueryUserPresenceChange : InstantMessagePayloadType.UserPresenceChange;
					InstantMessagePayload instantMessagePayload = new InstantMessagePayload(payloadType);
					instantMessagePayload.UserPresenceChanges = list.ToArray();
					notifier.Add(new InstantMessagePayload(payloadType)
					{
						UserPresenceChanges = list.ToArray()
					});
				}
				notifier.PickupData();
				if (isQuery)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. Presence returned for {0} sips with Context: {1}", new object[]
					{
						list.Count,
						this.TracingContext
					});
					OwsLogRegistry.Register(logger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingQueryPresenceData), new Type[0]);
					logger.Set(InstantMessagingQueryPresenceData.LyncServer, this.serverName);
					logger.Set(InstantMessagingQueryPresenceData.UCSMode, this.isUserInUCSMode);
					logger.Set(InstantMessagingQueryPresenceData.PrivacyMode, this.isUserInPrivateMode);
					logger.Set(InstantMessagingQueryPresenceData.UserContext, base.UserContext.Key);
					logger.Set(InstantMessagingQueryPresenceData.SuccessfulSIPs, ExtensibleLogger.FormatPIIValue(stringBuilder.ToString()));
					logger.Set(InstantMessagingQueryPresenceData.FailedSIPs, ExtensibleLogger.FormatPIIValue(stringBuilder2.ToString()));
				}
			}
		}

		private void OnSelfPresenceSubscriptionStateUpdated(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnSelfPresenceSubscriptionStateUpdated", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceSubscriptionStateUpdated. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = sender as IEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnSelfPresenceSubscriptionStateUpdated"))
				{
					return;
				}
				ISelfPresenceSubscriptionStateUpdatedEvent selfPresenceSubscriptionStateUpdatedEvent = arguments.Event as ISelfPresenceSubscriptionStateUpdatedEvent;
				switch (selfPresenceSubscriptionStateUpdatedEvent.SubReason)
				{
				case 1:
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Notifier, null, "Unable to sign in due to privacy policy changed.", InstantMessageServiceError.PrivacyPolicyChanged, false);
					this.SignOutEndpoint(endpoint);
					return;
				case 2:
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Notifier, null, "Unable to sign in due to privacy migration in progress.", InstantMessageServiceError.PrivacyMigrationInProgress, false);
					this.SignOutEndpoint(endpoint);
					return;
				case 3:
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Notifier, null, "Unable to sign in due to privacy migration needed.", InstantMessageServiceError.PrivacyMigrationNeeded, false);
					this.SignOutEndpoint(endpoint);
					return;
				default:
					return;
				}
			});
		}

		private void OnSubscriberChanged(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.LogExceptionsOnly(base.UserContext, "OCS.OnSubscriberChanged", delegate
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSubscriberChanged. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint2 endpoint = sender as IEndpoint2;
				if (this.EndpointNullOrNotConnected(null, endpoint, "OnSubscriberChanged"))
				{
					return;
				}
				ISubscriberEvent subscriberEvent = (ISubscriberEvent)arguments.Event;
				if (subscriberEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSubscriberChanged. Subscriber event is null.");
					return;
				}
				if (!this.isUserInPrivateMode && endpoint.PrivacyModeState == 2)
				{
					this.isUserInPrivateMode = true;
				}
				if (this.isUserInPrivateMode)
				{
					if (subscriberEvent.Subscribers != null)
					{
						lock (this.pendingSubscribers)
						{
							foreach (ISubscriber item in subscriberEvent.Subscribers)
							{
								this.pendingSubscribers.Add(item);
							}
						}
					}
					if (endpoint.EndpointState == 2)
					{
						this.ProcessSubscribers();
					}
				}
			});
		}

		private void ProcessSubscribers()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribers. Context: {0}", new object[]
			{
				this.TracingContext
			});
			IEndpoint2 endpoint = this.ucEndpoint as IEndpoint2;
			if (this.EndpointNullOrNotConnected(null, endpoint, "ProcessSubscribers"))
			{
				return;
			}
			List<string> list = new List<string>();
			List<ISubscriber> list2 = new List<ISubscriber>();
			InstantMessagePayload instantMessagePayload = null;
			lock (this.pendingSubscribers)
			{
				bool flag2 = false;
				foreach (ISubscriber subscriber in this.pendingSubscribers)
				{
					if (!subscriber.IsAcknowledged)
					{
						flag2 = true;
						string text = InstantMessageUtilities.ToSipFormat(subscriber.Id);
						if (!this.isNotifyAdditionToContactList)
						{
							endpoint.BeginAckSubscriber(InstantMessageUtilities.FromSipFormat(text), new AsyncCallback(this.DeclineBuddyAckSubscriberCallback), text);
						}
						else
						{
							list.Add(text);
							list2.Add(subscriber);
						}
					}
				}
				if (!flag2)
				{
					instantMessagePayload = new InstantMessagePayload(InstantMessagePayloadType.EmptySubscribers);
				}
				this.pendingSubscribers.Clear();
			}
			if (instantMessagePayload != null)
			{
				InstantMessageNotifier notifier = base.Notifier;
				if (notifier == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback. Notifier is null.");
					return;
				}
				lock (notifier)
				{
					notifier.Add(instantMessagePayload);
				}
				notifier.PickupData();
			}
			if (list.Count != 0)
			{
				endpoint.BeginQueryPresence(list.ToArray(), 4, false, new AsyncCallback(this.ProcessSubscribersQueryPresenceCallback), list2);
			}
			list = null;
		}

		private void ProcessSubscribersQueryPresenceCallback(IAsyncResult result)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.SignOutCallback", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "ProcessSubscribersQueryPresenceCallback"))
				{
					return;
				}
				ICollection<IPresenceNotificationData> collection;
				try
				{
					collection = endpoint.EndQueryPresence(result);
				}
				catch (Exception exception)
				{
					if (this.EndpointDisconnected(logger, exception, endpoint, "ProcessSubscribersQueryPresenceCallback"))
					{
						return;
					}
					throw;
				}
				InstantMessageNotifier notifier = this.Notifier;
				if (notifier != null)
				{
					lock (notifier)
					{
						List<InstantMessageContact> list = new List<InstantMessageContact>();
						Dictionary<string, IRichPresenceData> dictionary = new Dictionary<string, IRichPresenceData>();
						foreach (IPresenceNotificationData presenceNotificationData in collection)
						{
							IRichPresenceData richPresenceData = presenceNotificationData.RichPresenceData;
							if (richPresenceData != null)
							{
								dictionary.Add(InstantMessageUtilities.FromSipFormat(richPresenceData.Uri), richPresenceData);
							}
						}
						List<ISubscriber> list2 = result.AsyncState as List<ISubscriber>;
						foreach (ISubscriber subscriber in list2)
						{
							InstantMessageContact instantMessageContact = new InstantMessageContact();
							instantMessageContact.SipUri = subscriber.Id;
							if (dictionary.ContainsKey(subscriber.Id))
							{
								IContactCardCategoryItem contactCard = dictionary[subscriber.Id].ContactCard;
								if (contactCard != null)
								{
									instantMessageContact.DisplayName = contactCard.DisplayName;
									instantMessageContact.Title = contactCard.Title;
									instantMessageContact.Company = contactCard.Company;
									instantMessageContact.Office = contactCard.Office;
								}
							}
							if (string.IsNullOrEmpty(instantMessageContact.DisplayName))
							{
								instantMessageContact.DisplayName = subscriber.DisplayName;
							}
							list.Add(instantMessageContact);
						}
						if (list.Count > 0)
						{
							notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.PendingContactList)
							{
								PendingContacts = list.ToArray()
							});
						}
					}
					notifier.PickupData();
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
					return;
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback. Notifier is null.");
			});
		}

		private void OnSelfPresenceChanged(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnSelfPresenceChanged", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceChanged. Context: {0}", new object[]
				{
					this.TracingContext
				});
				ISelfPresenceEvent selfPresenceEvent = (ISelfPresenceEvent)arguments.Event;
				if (selfPresenceEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceChanged. Self presence event is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					return;
				}
				foreach (IPresenceCategory presenceCategory in selfPresenceEvent.Categories)
				{
					IPresenceCategoryItem presenceCategoryItem = null;
					try
					{
						presenceCategoryItem = presenceCategory.CategoryData;
					}
					catch (ArgumentException ex)
					{
						if (!(ex.InnerException is XmlException))
						{
							throw;
						}
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceChanged. Unable to get CategoryData due to ArgumentException with inner XmlException. {0}.", new object[]
						{
							ex
						});
					}
					catch (XmlException ex2)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceChanged. Unable to get CategoryData due to XmlException. {0}.", new object[]
						{
							ex2
						});
					}
					if (presenceCategoryItem != null)
					{
						IAlertsSettingsCategoryItem alertsSettingsCategoryItem = presenceCategoryItem as IAlertsSettingsCategoryItem;
						if (alertsSettingsCategoryItem != null)
						{
							this.isNotifyAdditionToContactList = alertsSettingsCategoryItem.NotifyAdditionToContactList;
						}
						else if (presenceCategory.ContainerId == 2)
						{
							IAggregateStateCategoryItem aggregateStateCategoryItem = presenceCategoryItem as IAggregateStateCategoryItem;
							if (aggregateStateCategoryItem != null)
							{
								InstantMessagePresenceType instantMessagePresenceType = this.MapToInstantMessagePresence(aggregateStateCategoryItem.Availability);
								if (instantMessagePresenceType != this.userState)
								{
									this.userState = instantMessagePresenceType;
									InstantMessagePayloadUtilities.GenerateUpdatePresencePayload(this.Notifier, (int)this.userState);
								}
							}
						}
					}
				}
				this.isSelfDataEstablished = true;
				logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
			});
		}

		private void OnUserPresenceChanged(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.LogExceptionsOnly(base.UserContext, "OCS.OnUserPresenceChanged", delegate
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnUserPresenceChanged. Context: {0}", new object[]
				{
					this.TracingContext
				});
				this.PresenceChanged(sender, arguments, false, null);
			});
		}

		private void OnQueryPresenceChanged(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnQueryPresenceChanged", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnQueryPresenceChanged. Context: {0}", new object[]
				{
					this.TracingContext
				});
				this.PresenceChanged(sender, arguments, true, logger);
			});
		}

		private void OnContactGroupChanged(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnContactGroupChanged", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnContactGroupChanged. Context: {0}", new object[]
				{
					this.TracingContext
				});
				Culture.InternalSetAsyncThreadCulture(this.UserContext.UserCulture);
				IContactGroupEvent contactGroupEvent = (IContactGroupEvent)arguments.Event;
				if (contactGroupEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnContactGroupChanged. ContactGroupEvent is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					return;
				}
				List<InstantMessageGroup> list = this.ConvertGroups(contactGroupEvent.AddedGroups);
				foreach (InstantMessageGroup instantMessageGroup in list)
				{
					if (instantMessageGroup.GroupType == InstantMessageGroupType.OtherContacts)
					{
						this.otheContactsGroupId = instantMessageGroup.Id;
						break;
					}
				}
				list.Sort();
				Dictionary<string, InstantMessageGroup> dictionary = new Dictionary<string, InstantMessageGroup>();
				foreach (InstantMessageGroup instantMessageGroup2 in list)
				{
					dictionary.Add(instantMessageGroup2.Id, instantMessageGroup2);
				}
				List<InstantMessageBuddy> list2 = this.ConvertContacts(contactGroupEvent.AddedContacts, dictionary);
				List<InstantMessageBuddy> second = this.ConvertContacts(contactGroupEvent.UpdatedContacts, dictionary);
				list2.Sort();
				InstantMessagePayloadUtilities.GenerateBuddyListPayload(this.Notifier, list, list2.Concat(second));
				List<InstantMessageGroup> groups = this.ConvertGroups(contactGroupEvent.DeletedGroups);
				InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload(this.Notifier, groups);
				List<InstantMessageGroup> groups2 = this.ConvertGroups(contactGroupEvent.UpdatedGroups);
				InstantMessagePayloadUtilities.GenerateGroupRenamePayload(this.Notifier, groups2);
				List<InstantMessageBuddy> buddies = this.ConvertContacts(contactGroupEvent.DeletedContacts, dictionary);
				InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload(this.Notifier, buddies);
				this.ExpandedGroupIds = null;
				this.isContactGroupEstablished = true;
				logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
			});
		}

		private string GetContactDisplayName(IContact contact)
		{
			string result = string.Empty;
			if (contact == null)
			{
				throw new ArgumentNullException("contact");
			}
			if (!string.IsNullOrEmpty(contact.Name))
			{
				result = contact.Name;
			}
			else
			{
				result = InstantMessageUtilities.FromSipFormat(contact.Uri);
			}
			return result;
		}

		private void OnConversationReceived(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnConversationReceived", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnConversationReceived"))
				{
					return;
				}
				IConversationReceivedEvent conversationReceivedEvent = (IConversationReceivedEvent)arguments.Event;
				if (conversationReceivedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. ConversationReceivedEvent is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					return;
				}
				IConversation conversation = conversationReceivedEvent.Conversation;
				if (conversation == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Conversation is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
					return;
				}
				IIMModality iimmodality = null;
				foreach (IModality modality in conversationReceivedEvent.Modalities)
				{
					if (modality is IIMModality)
					{
						iimmodality = (IIMModality)modality;
						break;
					}
				}
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Unsupported modality.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNotSupported);
					return;
				}
				InstantMessageNotifier notifier = this.Notifier;
				if (notifier == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Notifier is null.");
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.NotifierNull);
					return;
				}
				try
				{
					lock (notifier)
					{
						if (conversationReceivedEvent.Inviter != null)
						{
							string uri = conversationReceivedEvent.Inviter.Uri;
							string text = conversationReceivedEvent.Inviter.Name;
							if (string.IsNullOrEmpty(text))
							{
								text = InstantMessageUtilities.FromSipFormat(uri);
							}
							if (!string.IsNullOrEmpty(text) && uri != null)
							{
								string toastMessage = null;
								if (!string.IsNullOrEmpty(conversation.Subject))
								{
									toastMessage = conversation.Subject;
								}
								else if (conversationReceivedEvent.ToastMessage != null && !string.IsNullOrEmpty(conversationReceivedEvent.ToastMessage.Content))
								{
									toastMessage = conversationReceivedEvent.ToastMessage.Content;
								}
								notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.NewChatMessageToast)
								{
									ChatSessionId = new int?(conversation.Cid),
									SipUri = uri,
									DisplayName = text,
									MessageSubject = conversation.Subject,
									ToastMessage = toastMessage,
									IsConference = conversationReceivedEvent.IsConference
								});
							}
						}
						notifier.PickupData();
					}
					conversation.ConversationStateChanged += new IMEventHandler(this.OnConversationStateChanged);
					iimmodality.MessageReceived += new IMEventHandler(this.OnMessageReceived);
					iimmodality.MessageSendFailed += new IMEventHandler(this.OnMessageSendFailed);
					iimmodality.ComposingStateChanged += new IMEventHandler(this.OnComposingStateChanged);
					iimmodality.ModalityParticipantUpdated += new IMEventHandler(this.OnModalityParticipantUpdated);
					iimmodality.ModalityParticipantRemoved += new IMEventHandler(this.OnModalityParticipantRemoved);
					conversation.ParticipantUpdated += new IMEventHandler(this.OnParticipantUpdated);
					conversation.ParticipantRemoved += new IMEventHandler(this.OnParticipantRemoved);
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "OnConversationReceived"))
					{
						throw;
					}
				}
			});
		}

		private void OnParticipantUpdated(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnParticipantUpdated", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnParticipantUpdated"))
				{
					return;
				}
				try
				{
					IParticipantUpdatedEvent participantUpdatedEvent = (IParticipantUpdatedEvent)arguments.Event;
					if (participantUpdatedEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. ParticipantUpdatedEvent is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					}
					else
					{
						IConversation conversation = participantUpdatedEvent.Conversation;
						if (conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. Conversation is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
						}
						else
						{
							IParticipantInfo participantInfo = participantUpdatedEvent.ParticipantInfo;
							if (participantInfo == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. ParticipantInfo is null.");
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ParticipantNull);
							}
							else if (!conversation.IsConference)
							{
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.IgnoredBecause1To1);
							}
							else if (string.Compare(this.UserContext.SipUri, participantInfo.Uri, StringComparison.OrdinalIgnoreCase) == 0)
							{
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.IgnoredBecauseSelf);
							}
							else
							{
								InstantMessagePayloadUtilities.GenerateParticipantJoinedPayload(this.Notifier, conversation.Cid, participantInfo.Uri, participantInfo.Name);
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
							}
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "OnParticipantUpdated"))
					{
						throw;
					}
				}
			});
		}

		private void OnParticipantRemoved(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnParticipantRemoved", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnParticipantRemoved"))
				{
					return;
				}
				try
				{
					IParticipantRemovedEvent participantRemovedEvent = (IParticipantRemovedEvent)arguments.Event;
					if (participantRemovedEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved. ParticipantRemovedEvent is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					}
					else
					{
						IConversation conversation = participantRemovedEvent.Conversation;
						if (conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved. Conversation is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
						}
						else if (!conversation.IsConference)
						{
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.IgnoredBecause1To1);
						}
						else
						{
							InstantMessagePayloadUtilities.GenerateParticipantLeftPayload(this.Notifier, conversation.Cid, participantRemovedEvent.ParticipantUri);
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "OnParticipantRemoved"))
					{
						throw;
					}
				}
			});
		}

		private void OnConversationStateChanged(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnConversationStateChanged", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnConversationStateChanged"))
				{
					return;
				}
				try
				{
					IConversationStateChangedEvent conversationStateChangedEvent = (IConversationStateChangedEvent)arguments.Event;
					if (conversationStateChangedEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. ConversationStateChangedEvent is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					}
					else
					{
						IConversation conversation = conversationStateChangedEvent.Conversation;
						if (conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. Conversation is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
						}
						else if (conversationStateChangedEvent.ConversationState == 6)
						{
							IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
							conversation.ParticipantUpdated -= new IMEventHandler(this.OnParticipantUpdated);
							conversation.ParticipantRemoved -= new IMEventHandler(this.OnParticipantRemoved);
							conversation.ConversationStateChanged -= new IMEventHandler(this.OnConversationStateChanged);
							if (iimmodality == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. Instant Messaging Modality is null.");
							}
							else
							{
								iimmodality.MessageReceived -= new IMEventHandler(this.OnMessageReceived);
								iimmodality.MessageSendFailed -= new IMEventHandler(this.OnMessageSendFailed);
								iimmodality.ComposingStateChanged -= new IMEventHandler(this.OnComposingStateChanged);
								iimmodality.ModalityParticipantUpdated -= new IMEventHandler(this.OnModalityParticipantUpdated);
								iimmodality.ModalityParticipantRemoved -= new IMEventHandler(this.OnModalityParticipantRemoved);
								InstantMessageNotifier notifier = this.Notifier;
								if (notifier == null)
								{
									ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. Notifier is null.");
								}
								else
								{
									lock (notifier)
									{
										notifier.Add(new InstantMessagePayload(InstantMessagePayloadType.EndChatSession)
										{
											ChatSessionId = new int?(conversation.Cid)
										});
									}
									notifier.PickupData();
									logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
								}
							}
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "OnConversationStateChanged"))
					{
						throw;
					}
				}
			});
		}

		private void SendAndClearMessageList(int chatId)
		{
			if (this.messageQueueDictionary != null && this.messageQueueDictionary.ContainsKey(chatId))
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendAndClearMessageList. Chat ID: {0}. Context: {1}", new object[]
				{
					chatId,
					this.TracingContext
				});
				InstantMessageQueue instantMessageQueue = this.messageQueueDictionary[chatId];
				instantMessageQueue.SendAndClearMessageList();
			}
		}

		private void OnMessageReceived(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnMessageReceived", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnMessageReceived"))
				{
					return;
				}
				try
				{
					IMessageReceivedEvent messageReceivedEvent = (IMessageReceivedEvent)arguments.Event;
					if (messageReceivedEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. MessageReceivedEvent is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					}
					else
					{
						IIMModality iimmodality = sender as IIMModality;
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Instant Messaging Modality is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNull);
						}
						else
						{
							IConversation conversation = iimmodality.Conversation;
							if (conversation == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Conversation is null.");
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
							}
							else
							{
								InstantMessageNotifier notifier = this.Notifier;
								if (notifier == null)
								{
									ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Notifier is null.");
									logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.NotifierNull);
								}
								else
								{
									lock (notifier)
									{
										InstantMessagePayload instantMessagePayload = new InstantMessagePayload(InstantMessagePayloadType.NewChatMessage)
										{
											IsNewSession = (conversation.State == 1),
											ChatSessionId = new int?((iimmodality.Conversation != null) ? iimmodality.Conversation.Cid : -1),
											SipUri = messageReceivedEvent.SenderUri,
											MessageContent = ((messageReceivedEvent.Message != null) ? messageReceivedEvent.Message.Content : null),
											MessageFormat = ((messageReceivedEvent.Message != null) ? messageReceivedEvent.Message.Format : null)
										};
										if (iimmodality.Conversation.Subject != null)
										{
											instantMessagePayload.MessageSubject = iimmodality.Conversation.Subject;
										}
										notifier.Add(instantMessagePayload);
										InstantMessageOCSNotifier instantMessageOCSNotifier = notifier as InstantMessageOCSNotifier;
										if (instantMessageOCSNotifier != null)
										{
											instantMessageOCSNotifier.RegisterDeliverySuccessNotification(this, iimmodality, messageReceivedEvent.MessageId, logger);
										}
										else
										{
											ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Payload is not an OCS payload.");
										}
									}
									notifier.PickupData();
									bool arePerfCountersEnabled = InstantMessageProvider.ArePerfCountersEnabled;
									logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (!this.EndpointDisconnected(logger, ex, endpoint, "OnMessageReceived"))
					{
						InstantMessagingException ex2 = ex as InstantMessagingException;
						if (ex2 != null && ex2.Code == 18201)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. OcsFailureResponse. {0}", new object[]
							{
								ex
							});
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.PartialSuccess);
						}
						throw;
					}
				}
			});
		}

		private void OnMessageSendFailed(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnMessageSendFailed", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnMessageSendFailed"))
				{
					return;
				}
				try
				{
					IMessageSendFailedEvent messageSendFailedEvent = (IMessageSendFailedEvent)arguments.Event;
					if (messageSendFailedEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. MessageSendFailedEvent is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					}
					else
					{
						IIMModality iimmodality = messageSendFailedEvent.Modality as IIMModality;
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. Instant Messaging Modality is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNull);
						}
						else
						{
							IConversation conversation = iimmodality.Conversation;
							if (conversation == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. Conversation is null.");
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
							}
							else
							{
								InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.Notifier, "InstantMessageOCSProvider.OnMessageSendFailed", conversation.Cid, null);
							}
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "OnMessageSendFailed"))
					{
						throw;
					}
				}
			});
		}

		private void OnComposingStateChanged(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.LogExceptionsOnly(base.UserContext, "OCS.OnComposingStateChanged", delegate
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnComposingStateChanged. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(null, endpoint, "OnComposingStateChanged"))
				{
					return;
				}
				try
				{
					IComposingStateEvent composingStateEvent = (IComposingStateEvent)arguments.Event;
					if (composingStateEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnComposingStateChanged. ComposingStateEvent is null.");
					}
					else
					{
						IIMModality iimmodality = composingStateEvent.Modality as IIMModality;
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnComposingStateChanged. Instant Messaging Modality is null.");
						}
						else if (iimmodality.Conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnComposingStateChanged. Conversation is null.");
						}
						else
						{
							UserActivityType alertType = UserActivityType.StoppedTyping;
							if (composingStateEvent.State == 1)
							{
								alertType = UserActivityType.Typing;
							}
							InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(this.Notifier, iimmodality.Conversation.Cid, alertType, composingStateEvent.ComposerUri);
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(null, exception, endpoint, "OnComposingStateChanged"))
					{
						throw;
					}
				}
			});
		}

		private void OnModalityParticipantUpdated(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnModalityParticipantUpdated", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnModalityParticipantUpdated"))
				{
					return;
				}
				try
				{
					IModalityParticipantUpdatedEvent modalityParticipantUpdatedEvent = (IModalityParticipantUpdatedEvent)arguments.Event;
					if (modalityParticipantUpdatedEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. ParticipantUpdatedEvent is null.");
						logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EventNull);
					}
					else
					{
						IIMModality iimmodality = modalityParticipantUpdatedEvent.Modality as IIMModality;
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Instant Messaging Modality is null.");
							logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ModalityNull);
						}
						else
						{
							IConversation conversation = iimmodality.Conversation;
							if (conversation == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Conversation is null.");
								logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ConversationNull);
							}
							else
							{
								IParticipantInfo participantInfo = modalityParticipantUpdatedEvent.ParticipantInfo;
								if (participantInfo == null)
								{
									ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. ParticipantInfo is null.");
									logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.ParticipantNull);
								}
								else if (!conversation.IsConference)
								{
									logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.IgnoredBecause1To1);
								}
								else if (string.Compare(this.UserContext.SipUri, participantInfo.Uri, StringComparison.OrdinalIgnoreCase) == 0)
								{
									logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.IgnoredBecauseSelf);
								}
								else
								{
									InstantMessagePayloadUtilities.GenerateParticipantJoinedPayload(this.Notifier, iimmodality.Conversation.Cid, participantInfo.Uri, participantInfo.Name);
									logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Success);
								}
							}
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "OnModalityParticipantUpdated"))
					{
						throw;
					}
				}
			});
		}

		private void OnModalityParticipantRemoved(object sender, IMEventArgs arguments)
		{
			SimulatedWebRequestContext.Execute(base.UserContext, "OCS.OnModalityParticipantRemoved", delegate(RequestDetailsLogger logger)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantRemoved. Context: {0}", new object[]
				{
					this.TracingContext
				});
				IEndpoint endpoint = this.ucEndpoint;
				if (this.EndpointNullOrNotConnected(logger, endpoint, "OnModalityParticipantRemoved"))
				{
					return;
				}
				try
				{
					IModalityParticipantRemovedEvent modalityParticipantRemovedEvent = (IModalityParticipantRemovedEvent)arguments.Event;
					if (modalityParticipantRemovedEvent == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantRemoved. ParticipantRemovedEvent is null.");
					}
					else
					{
						IIMModality iimmodality = modalityParticipantRemovedEvent.Modality as IIMModality;
						if (iimmodality == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Instant Messaging Modality is null.");
						}
						else if (iimmodality.Conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Conversation is null.");
						}
						else
						{
							InstantMessagePayloadUtilities.GenerateParticipantLeftPayload(this.Notifier, iimmodality.Conversation.Cid, modalityParticipantRemovedEvent.ParticipantUri);
						}
					}
				}
				catch (Exception exception)
				{
					if (!this.EndpointDisconnected(logger, exception, endpoint, "OnModalityParticipantRemoved"))
					{
						throw;
					}
				}
			});
		}

		private void QueueMessage(IConversation conversation, string messageFormat, string message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueueMessage. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (this.messageQueueDictionary == null)
			{
				this.messageQueueDictionary = new Dictionary<int, InstantMessageQueue>();
			}
			if (this.messageQueueDictionary.ContainsKey(conversation.Cid))
			{
				InstantMessageQueue instantMessageQueue = this.messageQueueDictionary[conversation.Cid];
				instantMessageQueue.AddMessage(messageFormat, message);
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueueMessage. Creating new message queue.");
			InstantMessageQueue instantMessageQueue2 = new InstantMessageQueue(base.UserContext, conversation, base.Notifier);
			instantMessageQueue2.AddMessage(messageFormat, message);
			this.messageQueueDictionary.Add(conversation.Cid, instantMessageQueue2);
		}

		private InstantMessagePresenceType MapToInstantMessagePresence(long presence)
		{
			if (presence >= 18000L)
			{
				return InstantMessagePresenceType.Offline;
			}
			if (presence >= 15000L)
			{
				return InstantMessagePresenceType.Away;
			}
			if (presence >= 12000L)
			{
				return InstantMessagePresenceType.BeRightBack;
			}
			if (presence >= 9000L)
			{
				return InstantMessagePresenceType.DoNotDisturb;
			}
			if (presence >= 7500L)
			{
				return InstantMessagePresenceType.IdleBusy;
			}
			if (presence >= 6000L)
			{
				return InstantMessagePresenceType.Busy;
			}
			if (presence >= 4500L)
			{
				return InstantMessagePresenceType.IdleOnline;
			}
			if (presence >= 3000L)
			{
				return InstantMessagePresenceType.Online;
			}
			return InstantMessagePresenceType.None;
		}

		private void CreateEndpointAndBeginSignIn()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateEndpointAndBeginSignIn");
			if (this.isSigningIn)
			{
				this.isRefreshNeeded = true;
				return;
			}
			this.isSigningIn = true;
			if (InstantMessageProvider.ArePerfCountersEnabled)
			{
				base.UserContext.SignIntoIMTime = InstantMessageProvider.GetElapsedMilliseconds();
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				try
				{
					endpoint = InstantMessageOCSProvider.EndpointManager.CreateEndpoint(base.UserContext.SipUri, this.serverName, 5, null, null);
				}
				catch (InstantMessagingException ex)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.EndpointManager.CreateEndpoint failed. {0}", new object[]
					{
						ex
					});
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCreateEndpointFailure, string.Empty, new object[]
					{
						(base.UserContext.MailboxSession != null) ? base.UserContext.MailboxSession.DisplayName : base.UserContext.SipUri,
						base.UserContext.SipUri,
						ex
					});
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(base.Notifier, ex, "Failed to create an endpoint.", InstantMessageServiceError.CreateEndpointFailure, false);
					this.RecordExceptionProperties(OwaApplication.GetRequestDetailsLogger, ex);
					if (!(ex.InnerException is ArgumentException))
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.CreateEndpointAndBeginSignIn", base.UserContext, ex);
					}
					this.isSigningIn = false;
					return;
				}
				endpoint.ContactGroupChanged += new IMEventHandler(this.OnContactGroupChanged);
				endpoint.UserPresenceChanged += new IMEventHandler(this.OnUserPresenceChanged);
				endpoint.SelfPresenceChanged += new IMEventHandler(this.OnSelfPresenceChanged);
				endpoint.ConversationReceived += new IMEventHandler(this.OnConversationReceived);
				endpoint.QueryPresenceResultsReceived += new IMEventHandler(this.OnQueryPresenceChanged);
				IEndpoint2 endpoint2 = endpoint as IEndpoint2;
				if (endpoint2 != null)
				{
					endpoint2.SelfPresenceSubscriptionStateUpdated += new IMEventHandler(this.OnSelfPresenceSubscriptionStateUpdated);
					endpoint2.SubscriberChanged += new IMEventHandler(this.OnSubscriberChanged);
				}
				endpoint.BeginSignIn(false, 0, new AsyncCallback(this.SignInCallback), endpoint);
				return;
			}
			if (endpoint.EndpointState == null || endpoint.EndpointState == 5)
			{
				endpoint.BeginSignIn(false, 0, new AsyncCallback(this.SignInCallback), endpoint);
			}
		}

		private void HandleFailedGroupEditResult(string errMsg)
		{
			this.NotifyClientOfError(InstantMessagePayload.ReportError(errMsg, InstantMessageOperationType.GroupEdit));
		}

		private void HandleFailedResult(string errMsg)
		{
			this.NotifyClientOfError(InstantMessagePayload.ReportError(errMsg, InstantMessageOperationType.Unspecified));
		}

		private void NotifyClientOfError(InstantMessagePayload errorToReport)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyClientOfError. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (!string.IsNullOrEmpty(errorToReport.ErrorMessage))
			{
				lock (base.Notifier)
				{
					base.Notifier.Add(errorToReport);
				}
				base.Notifier.PickupData();
			}
		}

		private List<InstantMessageGroup> ConvertGroups(ICollection<IGroup> groups)
		{
			List<InstantMessageGroup> list = new List<InstantMessageGroup>();
			foreach (IGroup group in groups)
			{
				if (group != null && !group.IsDG)
				{
					InstantMessageGroupType groupType = InstantMessageGroupType.Standard;
					if (group.GroupData != null && group.GroupData.Contains("groupType=\"pinnedGroup\""))
					{
						groupType = InstantMessageGroupType.Pinned;
					}
					InstantMessageGroup instantMessageGroup = InstantMessageGroup.Create(group.GroupId.ToString(), group.Name ?? string.Empty, groupType);
					instantMessageGroup.SetExpandedState(base.ExpandedGroupIds);
					list.Add(instantMessageGroup);
				}
			}
			return list;
		}

		private List<InstantMessageBuddy> ConvertContacts(ICollection<IContact> contacts, Dictionary<string, InstantMessageGroup> groups)
		{
			List<InstantMessageBuddy> list = new List<InstantMessageBuddy>();
			foreach (IContact contact in contacts)
			{
				if (contact != null)
				{
					InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create(string.Empty, contact.Uri, this.GetContactDisplayName(contact), null);
					if (contact.GroupIds != null)
					{
						foreach (int num in contact.GroupIds)
						{
							string text = num.ToString();
							InstantMessageGroup group;
							if (groups != null && groups.ContainsKey(text))
							{
								group = groups[text];
							}
							else
							{
								group = InstantMessageGroup.Create(text, string.Empty);
							}
							instantMessageBuddy.AddGroup(group);
						}
					}
					instantMessageBuddy.Tagged = contact.Tagged;
					list.Add(instantMessageBuddy);
				}
			}
			return list;
		}

		private void AddBuddyInUcsMode(IMailboxSession session, string firstName, string lastName, string imAddress, string displayName)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddyInUcsMode. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (imAddress == null)
			{
				throw new ArgumentNullException("imAddress");
			}
			try
			{
				StoreObjectId contactId = null;
				PersonId personId = null;
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddyInUcsMode. Retrieving contact imAddress {0}. Context: {1}", new object[]
				{
					imAddress,
					this.TracingContext
				});
				UnifiedContactStoreUtilities unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, new XSOFactory());
				unifiedContactStoreUtilities.RetrieveOrCreateContact(imAddress, null, displayName, firstName, lastName, out contactId, out personId);
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddyInUcsMode. Adding contact imAddress {0}.Context: {1}", new object[]
				{
					imAddress,
					this.TracingContext
				});
				unifiedContactStoreUtilities.AddContactToGroup(contactId, displayName, null);
				this.RecordBuddyMetadata(imAddress, null);
			}
			catch (Exception ex)
			{
				InstantMessageProvider.Log(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Exception);
				InstantMessageProvider.Log(ServiceCommonMetadata.ExceptionName, ex.Message);
				InstantMessageUtilities.SendWatsonReport("InstantMessageUCSProvider.AddBuddyInUcsMode", base.UserContext, ex);
			}
		}

		private void RemoveBuddyInUcsMode(IMailboxSession session, string imAddress, StoreId contactId)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyInUcsMode. Context: {0}", new object[]
			{
				this.TracingContext
			});
			if (imAddress == null)
			{
				throw new ArgumentNullException("imAddress");
			}
			if (contactId == null)
			{
				throw new ArgumentNullException("contactId");
			}
			try
			{
				UnifiedContactStoreUtilities unifiedContactStoreUtilities = new UnifiedContactStoreUtilities(session, InstantMessageProvider.XsoFactory);
				unifiedContactStoreUtilities.RemoveContactFromImList(contactId);
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyInUcsMode.im address:{0} successfully removed from im list.Context {1}", new object[]
				{
					imAddress,
					this.TracingContext
				});
				this.RecordBuddyMetadata(imAddress, contactId);
			}
			catch (Exception ex)
			{
				InstantMessageProvider.Log(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Exception);
				InstantMessageProvider.Log(ServiceCommonMetadata.ExceptionName, ex.Message);
				InstantMessageUtilities.SendWatsonReport("InstantMessageUCSProvider.RemoveBuddyInUcsMode", base.UserContext, ex);
			}
		}

		private bool EndpointDisconnected(RequestDetailsLogger logger, Exception exception, IEndpoint endpoint, string callbackName)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "{0} caught exception: {1}", new object[]
			{
				callbackName,
				exception.ToString()
			});
			if (logger != null)
			{
				this.RecordExceptionProperties(logger, exception);
			}
			if (endpoint == null || endpoint.EndpointState != 2)
			{
				this.GenerateInstantMessageUnavailablePayload(endpoint, callbackName, InstantMessageServiceError.SessionDisconnected, exception);
				return true;
			}
			return false;
		}

		private bool EndpointNullOrNotConnected(RequestDetailsLogger logger, IEndpoint endpoint, string callbackName)
		{
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "{0}: The endpoint is null.", new object[]
				{
					callbackName
				});
				if (logger != null)
				{
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EndpointNull);
				}
				return true;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "{0}: The endpoint is not connected.", new object[]
				{
					callbackName
				});
				if (logger != null)
				{
					logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.EndpointNotConnected);
				}
				return true;
			}
			return false;
		}

		private void RecordExceptionProperties(RequestDetailsLogger logger, Exception exception)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), exception.ToString());
			logger.Set(ServiceCommonMetadata.ErrorCode, InstantMessageOCSProvider.CallbackResult.Exception);
			OwsLogRegistry.Register(logger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessageOCSProvider.InstantMessagingCallbackData), new Type[0]);
			logger.Set(InstantMessageOCSProvider.InstantMessagingCallbackData.ExceptionType, exception.GetType().Name);
			logger.Set(InstantMessageOCSProvider.InstantMessagingCallbackData.ExceptionMessage, exception.Message);
			logger.Set(InstantMessageOCSProvider.InstantMessagingCallbackData.ExceptionDetails, exception.ToString());
			InstantMessagingException ex = exception as InstantMessagingException;
			if (ex != null)
			{
				logger.Set(InstantMessageOCSProvider.InstantMessagingCallbackData.ExceptionCode, ex.Code);
				logger.Set(InstantMessageOCSProvider.InstantMessagingCallbackData.ExceptionSubCode, ex.SubCode);
			}
			if (exception.Data != null && exception.Data.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in exception.Data)
				{
					stringBuilder.Append(obj);
					stringBuilder.Append("->");
					stringBuilder.Append(exception.Data[obj]);
					stringBuilder.Append("\r\n");
				}
				logger.Set(InstantMessageOCSProvider.InstantMessagingCallbackData.ExceptionData, stringBuilder);
			}
		}

		private void RecordBuddyMetadata(string buddySipUri, StoreId buddyContactId)
		{
			InstantMessageProvider.Log(InstantMessagingBuddyMetadata.LyncServer, this.serverName);
			InstantMessageProvider.Log(InstantMessagingBuddyMetadata.UserContext, base.UserContext.Key);
			InstantMessageProvider.Log(InstantMessagingBuddyMetadata.PrivacyMode, this.isUserInPrivateMode);
			InstantMessageProvider.Log(InstantMessagingBuddyMetadata.UCSMode, this.isUserInUCSMode);
			InstantMessageProvider.Log(InstantMessagingBuddyMetadata.SIP, ExtensibleLogger.FormatPIIValue(buddySipUri));
			if (buddyContactId != null)
			{
				InstantMessageProvider.Log(InstantMessagingBuddyMetadata.ContactId, buddyContactId);
			}
		}

		private void RecordSubscriptionMetadata(string subscriptionUris, string unSubcribeUri)
		{
			InstantMessageProvider.Log(InstantMessagingSubscriptionMetadata.LyncServer, this.serverName);
			InstantMessageProvider.Log(InstantMessagingSubscriptionMetadata.UserContext, base.UserContext.Key);
			InstantMessageProvider.Log(InstantMessagingSubscriptionMetadata.UCSMode, this.isUserInUCSMode);
			InstantMessageProvider.Log(InstantMessagingSubscriptionMetadata.PrivacyMode, this.isUserInPrivateMode);
			if (subscriptionUris != null)
			{
				InstantMessageProvider.Log(InstantMessagingSubscriptionMetadata.SubscribedSIPs, ExtensibleLogger.FormatPIIValue(subscriptionUris));
			}
			if (unSubcribeUri != null)
			{
				InstantMessageProvider.Log(InstantMessagingSubscriptionMetadata.UnSubscribedSIP, ExtensibleLogger.FormatPIIValue(unSubcribeUri));
			}
		}

		private const string UserAgentFormat = "OWA/{0}";

		private readonly string serverName;

		private static int[] mtlsPortNumbers = new int[]
		{
			5075,
			5076,
			5077
		};

		private static object initializationLock = new object();

		private static bool isInitialized;

		private InstantMessagePresenceType userState;

		private volatile bool isSigningIn;

		private volatile bool isRefreshNeeded;

		private IEndpoint ucEndpoint;

		private volatile bool isEndpointRegistered;

		private volatile bool isSelfDataEstablished;

		private volatile bool isContactGroupEstablished;

		private volatile bool isCurrentlyActivityBasedAway;

		private Dictionary<int, InstantMessageQueue> messageQueueDictionary;

		private volatile string otheContactsGroupId;

		private List<ISubscriber> pendingSubscribers = new List<ISubscriber>();

		private volatile bool isUserInPrivateMode;

		private volatile bool isUserInUCSMode;

		private volatile bool isNotifyAdditionToContactList = true;

		private ContentType defaultContentType = new ContentType("text/plain;charset=utf-8");

		private string tracingContext;

		private enum CallbackResult
		{
			Success,
			PartialSuccess,
			Exception,
			EndpointNull,
			EndpointNotConnected,
			EventNull,
			ConversationNull,
			ConversationNotConnected,
			ModalityNull,
			ModalityNotConnected,
			ModalityNotSupported,
			NotifierNull,
			ParticipantNull,
			AsyncStateNull,
			IgnoredBecause1To1,
			IgnoredBecauseSelf
		}

		private enum InstantMessagingCallbackData
		{
			[DisplayName("IM.D")]
			ExceptionDetails,
			[DisplayName("IM.EM")]
			ExceptionMessage,
			[DisplayName("IM.ET")]
			ExceptionType,
			[DisplayName("IM.EC")]
			ExceptionCode,
			[DisplayName("IM.ESC")]
			ExceptionSubCode,
			[DisplayName("IM.Unav")]
			ServiceUnavailable,
			[DisplayName("IM.ED")]
			ExceptionData
		}

		private class AddBuddyContext
		{
			public AddBuddyContext(InstantMessageBuddy buddy, bool ackSubscriber)
			{
				this.Buddy = buddy;
				this.AckSubscriber = ackSubscriber;
			}

			public InstantMessageBuddy Buddy { get; private set; }

			public bool AckSubscriber { get; private set; }
		}
	}
}

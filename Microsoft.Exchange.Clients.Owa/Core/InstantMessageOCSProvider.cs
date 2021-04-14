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
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InstantMessaging;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class InstantMessageOCSProvider : InstantMessageProvider
	{
		internal static IEndpointManager EndpointManager { get; private set; }

		internal static void InitializeEndpointManager(string certificateIssuer, byte[] certificateSerial, int mtlsPortNumber)
		{
			bool flag = false;
			string text = string.Empty;
			if (string.IsNullOrEmpty(OwaRegistryKeys.IMImplementationDLLPath))
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. No registry setting for IM Provider.");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderNoRegistrySetting, string.Empty, new object[]
				{
					OwaRegistryKeys.IMKeyPath,
					OwaRegistryKeys.IMImplementationDLLPathKey
				});
				return;
			}
			if (!File.Exists(OwaRegistryKeys.IMImplementationDLLPath))
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<string>(0L, "InstantMessageOCSProvider.InitializeEndpointManager. IM provider not found.  File: {0}", OwaRegistryKeys.IMImplementationDLLPath);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderFileDoesNotExist, string.Empty, new object[]
				{
					OwaRegistryKeys.IMImplementationDLLPath
				});
				return;
			}
			IEndpointManager endpointManager = null;
			Type[] types = new Type[]
			{
				typeof(string),
				typeof(byte[]),
				typeof(string)
			};
			try
			{
				Assembly assembly = Assembly.LoadFrom(OwaRegistryKeys.IMImplementationDLLPath);
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsClass && type.IsPublic && type.GetInterface("Microsoft.Exchange.InstantMessaging.IEndpointManager") != null)
					{
						if (endpointManager != null)
						{
							OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderMultipleClasses, string.Empty, new object[]
							{
								OwaRegistryKeys.IMImplementationDLLPath
							});
							return;
						}
						ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, types, null);
						if (constructor != null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Found IM provider with default signature.");
							endpointManager = (Activator.CreateInstance(type, new object[]
							{
								certificateIssuer,
								certificateSerial,
								InstantMessageOCSProvider.ApplicationUserAgent
							}) as IEndpointManager);
						}
						else
						{
							constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[0], null);
							if (constructor != null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Found IM provider with empty constructor");
								endpointManager = (Activator.CreateInstance(type) as IEndpointManager);
							}
						}
					}
				}
				if (endpointManager != null)
				{
					foreach (int num in InstantMessageOCSProvider.mtlsPortNumbers)
					{
						try
						{
							int num2 = (mtlsPortNumber == -1) ? num : mtlsPortNumber;
							ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Initializing Endpoint with port number : {0}", num2);
							endpointManager.Initialize(null, num2);
							flag = true;
							OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_IMEndpointManagerInitializedSuccessfully);
							break;
						}
						catch (InstantMessagingException ex)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Exception hit while initializing.  Exception: {0}", ex);
							text = ((ex.Message != null) ? ex.Message : string.Empty);
							if (ex.Code != 18105 || mtlsPortNumber != -1)
							{
								break;
							}
						}
					}
					if (flag)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug(0L, "InstantMessageOCSProvider.InitializeEndpointManager. EndpointManager initialized successfully");
						InstantMessageOCSProvider.EndpointManager = endpointManager;
						IEndpointManager2 endpointManager2 = endpointManager as IEndpointManager2;
						if (endpointManager2 != null)
						{
							endpointManager2.IsPrivacyModeSupported = true;
						}
					}
					else
					{
						OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderExceptionDuringLoad, string.Empty, new object[]
						{
							OwaRegistryKeys.IMImplementationDLLPath,
							text
						});
					}
				}
				else
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError(0L, "InstantMessageOCSProvider.InitializeEndpointManager. No constructor found.");
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderNoValidConstructor, string.Empty, new object[]
					{
						OwaRegistryKeys.IMImplementationDLLPath
					});
				}
			}
			catch (Exception innerException)
			{
				while (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				text = ((innerException != null && innerException.Message != null) ? innerException.Message : string.Empty);
				ExTraceGlobals.InstantMessagingTracer.TraceError<Exception>(0L, "InstantMessageOCSProvider.InitializeEndpointManager. Exception hit while initializing.  Exception: {0}", innerException);
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMProviderExceptionDuringLoad, string.Empty, new object[]
				{
					OwaRegistryKeys.IMImplementationDLLPath,
					text
				});
			}
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

		private InstantMessageOCSProvider(UserContext userContext, InstantMessagePayload payload)
		{
			if (payload == null)
			{
				throw new ArgumentNullException("payload");
			}
			this.Payload = payload;
			this.Payload.ChangeUserPresenceAfterInactivity += this.ChangeUserPresenceAfterInactivity;
			this.userContext = userContext;
			this.serverName = Globals.OCSServerName;
			this.userState = 18000;
		}

		internal static InstantMessageOCSProvider Create(UserContext userContext, InstantMessagePayload payload)
		{
			if (string.IsNullOrEmpty(userContext.SipUri))
			{
				return null;
			}
			return new InstantMessageOCSProvider(userContext, payload);
		}

		internal override bool IsSessionStarted
		{
			get
			{
				return this.isEndpointRegistered && this.isSelfDataEstablished && this.isContactGroupEstablished;
			}
		}

		internal override void ParticipateInConversation(int conversationId)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Chat Id: {0}", conversationId);
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.ParticipateInConversation", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			IConversation conversation = endpoint.GetConversation(conversationId);
			if (conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Conversation is null.");
				return;
			}
			if (conversation.IsConference)
			{
				conversation.BeginParticipate(new AsyncCallback(this.ParticipateCallback), conversation);
				return;
			}
			IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
			if (iimmodality == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateInConversation. Instant Messaging Modality is null.");
				return;
			}
			iimmodality.BeginParticipate(new AsyncCallback(this.InstantMessagingParticipateCallback), iimmodality);
		}

		internal override void PublishSelfPresence(int presence)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfPresence. Presence: {0}", presence);
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint != null)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfPresence. Sending UN response to the client");
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.PublishSelfPresence", InstantMessageFailure.SessionDisconnected, null);
					return;
				}
				endpoint.BeginPublishSelfState(this.ConvertToUserState(presence), new AsyncCallback(this.PublishSelfStateCallback), endpoint);
				this.userState = this.MapToPresenceAvailability((long)presence);
			}
		}

		internal override void PublishResetStatus()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishResetStatus");
			this.PublishSelfPresence(3000);
		}

		internal override int SendMessage(InstantMessageProvider.ProviderMessage message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessage. Chat Id: {0}", message.ChatSessionId);
			if (string.IsNullOrEmpty(message.Body))
			{
				return -1;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessage. EndPoint is null.");
				return -1;
			}
			IConversation conversation = endpoint.GetConversation(message.ChatSessionId);
			if (conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessage. Conversation is null.");
				return this.SendNewChatMessage(message);
			}
			IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
			if (iimmodality == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessage. Instant Messaging Modality is null.");
				return -1;
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

		internal override int SendNewChatMessage(InstantMessageProvider.ProviderMessage message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage.");
			if (string.IsNullOrEmpty(message.Body))
			{
				return -1;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. EndPoint is null.");
				return -1;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SendNewChatMessage", InstantMessageFailure.SessionDisconnected, null);
				return -1;
			}
			IConversation conversation = endpoint.CreateConversation(message.Recipients, null, this.defaultContentType, message.Body);
			if (conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. Conversation is null.");
				return -1;
			}
			IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
			if (iimmodality == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendNewChatMessage. Instant Messaging Modality is null.");
				return -1;
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

		internal override void MakeEndpointMostActive()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.MakeEndpointMostActive.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.MakeEndpointMostActive. EndPoint is null.");
				return;
			}
			if (endpoint.IsMostActive || !this.IsSessionStarted)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "InstantMessageOCSProvider.MakeEndpointMostActive.  Didn't publish.  IsMostActive: {0} SessionStarted: {1}", endpoint.IsMostActive, this.IsSessionStarted);
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.MakeEndpointMostActive. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.MakeEndpointMostActive", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			endpoint.BeginPublishMouseKeyboardActivity(new AsyncCallback(this.PublishMouseKeyboardActivityCallBack), endpoint);
		}

		internal override void QueryPresence(string[] sipUris)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence");
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
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.QueryPresence", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			try
			{
				string[] array;
				if (sipUris.Length > 128)
				{
					array = new string[128];
					for (int i = 0; i < 128; i++)
					{
						array[i] = sipUris[i];
					}
				}
				else
				{
					array = sipUris;
				}
				endpoint.BeginQueryPresence(array, 1, true, new AsyncCallback(this.QueryPresenceCallback), endpoint);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.QueryPresence", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresence. Exception message is {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.QueryPresence", this.userContext, ex);
					}
				}
			}
		}

		internal override void AddSubscription(string[] sipUris)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription");
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
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddSubscription", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			try
			{
				endpoint.BeginSubscribePresence(sipUris, new AsyncCallback(this.SubscribePresenceCallback), endpoint);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddSubscription", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AddSubscription. Exception message is {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AddSubscription", this.userContext, ex);
					}
				}
			}
		}

		internal override void RemoveSubscription(string sipUri)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveSubscription");
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
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveSubscription. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RemoveSubscription", this.userContext, ex);
				}
			}
		}

		internal override void AddBuddy(InstantMessageBuddy buddy, InstantMessageGroup group)
		{
			this.AddBuddy(buddy, group, false);
		}

		private void AddBuddy(InstantMessageBuddy buddy, InstantMessageGroup group, bool ackSubscriber)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy.");
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
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddBuddy", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<string>((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. Contact SIP Uri: {0}", buddy.SipUri);
			int num;
			if (!int.TryParse(group.Id, out num))
			{
				throw new OwaInvalidOperationException("The group id is not valid. GroupId:" + group.Id);
			}
			int[] array = new int[]
			{
				num
			};
			buddy.AddGroups(Array.ConvertAll<int, string>(array, (int i) => i.ToString()));
			InstantMessageOCSProvider.AddBuddyContext addBuddyContext = new InstantMessageOCSProvider.AddBuddyContext(buddy, ackSubscriber);
			try
			{
				if (this.isUserInPrivateMode)
				{
					endpoint.BeginQueryPresence(new string[]
					{
						buddy.SipUri
					}, 16, false, new AsyncCallback(this.AddContactQueryPresenceCallback), addBuddyContext);
				}
				else
				{
					endpoint.BeginAddContact(buddy.SipUri, string.Empty, true, false, array, new AsyncCallback(this.AddBuddyCallback), addBuddyContext);
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
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddy. Exception message is {0}.", ex);
				throw;
			}
		}

		internal override void RemoveBuddy(InstantMessageBuddy buddy)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy.");
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
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RemoveBuddy", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<string>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. Contact SIP Uri: {0}", buddy.SipUri);
			try
			{
				if (this.isUserInPrivateMode)
				{
					int[] array = new int[]
					{
						100,
						200,
						300,
						400
					};
					List<IContainerUpdateOperation> list = new List<IContainerUpdateOperation>();
					for (int i = 0; i < array.Length; i++)
					{
						IContainerUpdateOperation containerUpdateOperation = endpoint.CreateContainerUpdateOperation(array[i]);
						containerUpdateOperation.DeleteUri(buddy.SipUri);
						list.Add(containerUpdateOperation);
					}
					endpoint.BeginUpdateContainerMembership(list, new AsyncCallback(this.DeleteContactUpdateContainerCallback), buddy.SipUri);
				}
				else
				{
					endpoint.BeginDeleteContact(buddy.SipUri, new AsyncCallback(this.RemoveBuddyCallback), buddy.SipUri);
				}
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. InvalidOperationException.");
					throw new OwaInvalidOperationException("Remove buddy threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddy. Exception message is {0}.", ex);
				throw;
			}
		}

		internal override void AcceptBuddy(InstantMessageBuddy buddy, InstantMessageGroup group)
		{
			this.AddBuddy(buddy, group, true);
		}

		internal override void DeclineBuddy(InstantMessageBuddy buddy)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddy.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddy. EndPoint is null.");
				return;
			}
			try
			{
				endpoint.BeginAckSubscriber(InstantMessageUtilities.FromSipFormat(buddy.SipUri), new AsyncCallback(this.DeclineBuddyAckSubscriberCallback), buddy.SipUri);
			}
			catch (InstantMessagingException arg)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-1660189394), InstantMessageUtilities.FromSipFormat(buddy.SipUri), CultureInfo.InvariantCulture);
				this.HandleFailedResult(errMsg);
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddy. {0}.", arg);
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.DeclineBuddy", this.userContext, exception);
			}
		}

		private void DeclineBuddyAckSubscriberCallback(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback");
			string sipAddress = result.AsyncState as string;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback. Endpoint is null.");
				return;
			}
			try
			{
				endpoint.EndAckSubscriber(result);
			}
			catch (InstantMessagingException arg)
			{
				if (!this.isUserInPrivateMode)
				{
					string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-1660189394), InstantMessageUtilities.FromSipFormat(sipAddress), CultureInfo.InvariantCulture);
					this.HandleFailedResult(errMsg);
					ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback. {0}.", arg);
				}
				else
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback. Ignoring exception on auto acknowledge in privacy + notification mode.");
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.DeclineBuddyAckSubscriberCallback", this.userContext, exception);
			}
		}

		private void DeleteContactUpdateContainerCallback(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeleteContactUpdateContainerCallback");
			string text = result.AsyncState as string;
			IEndpoint2 endpoint = this.ucEndpoint as IEndpoint2;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.DeleteContactUpdateContainerCallback. Endpoint is null.");
				return;
			}
			try
			{
				endpoint.EndUpdateContainerMembership(result);
				endpoint.BeginDeleteContact(text, new AsyncCallback(this.RemoveBuddyCallback), text);
			}
			catch (InstantMessagingException ex)
			{
				this.GenerateOperationFailurePayload("DeleteContactUpdateContainerCallback", string.Format(CultureInfo.InvariantCulture, "RCF(\"{0}\");", new object[]
				{
					Utilities.JavascriptEncode(text)
				}), ex, true);
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.DeleteContactUpdateContainerCallback. {0}.", ex);
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.DeleteContactUpdateContainerCallback", this.userContext, exception);
			}
		}

		internal override void EndChatSession(int chatSessionId, bool disconnectSession)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageOCSProvider.EndChatSession. Chat Id: {0}", chatSessionId);
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

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<bool>((long)this.GetHashCode(), "InstantMessageOCSProvider.Dispose. Disposing: {0}", isDisposing);
			if (isDisposing)
			{
				if (this.Payload != null)
				{
					this.Payload.ChangeUserPresenceAfterInactivity -= this.ChangeUserPresenceAfterInactivity;
				}
				this.TerminateOCSSession();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.InternalGetDisposeTracker.");
			return DisposeTracker.Get<InstantMessageOCSProvider>(this);
		}

		internal override void EstablishSession()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.EstablishOCSSession");
			IEndpoint endpoint = this.ucEndpoint;
			this.ExpandedGroupIds = InstantMessageExpandPersistence.GetExpandedGroups(this.userContext);
			InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload(this.Payload, true);
			if (endpoint == null)
			{
				lock (this)
				{
					endpoint = this.ucEndpoint;
					if (endpoint == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.EstablishOCSSession. New Session.");
						if (!this.isSigningIn)
						{
							if (Globals.ArePerfCountersEnabled)
							{
								this.userContext.SignIntoIMTime = Globals.ApplicationTime;
							}
							this.CreateEndpoint();
						}
						else
						{
							this.isRefreshNeeded = true;
						}
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
			this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.EstablishSession", InstantMessageFailure.SessionDisconnected, null);
		}

		private void ChangeUserPresenceAfterInactivity(object sender, EventArgs e)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity");
			if (!this.isSelfDataEstablished)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. SelfDataSession not established.");
				return;
			}
			if (this.IsActivityBasedPresenceSet)
			{
				long num = Globals.ApplicationTime - this.userContext.LastUserRequestTime;
				if (this.isCurrentlyActivityBasedAway || num < (long)(Globals.ActivityBasedPresenceDuration * 2))
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity. No presence change needed for now.");
					return;
				}
			}
			if (this.userState == 3000 || this.userState == 4500 || this.userState == 6000 || this.userState == 7500 || this.userState == 9000)
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
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.ChangeUserPresenceAfterInactivity", InstantMessageFailure.SessionDisconnected, null);
					return;
				}
				if (!this.IsActivityBasedPresenceSet && (this.userState == 3000 || this.userState == 6000 || this.userState == 9000))
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

		internal override void ResetPresence()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence.");
			if (!this.isSelfDataEstablished)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence. SelfDataSession not established.");
				return;
			}
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence. Endpoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ResetPresence. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.ResetPresence", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			endpoint.BeginPublishMachineState(3700, new AsyncCallback(this.PublishMachineStateCallback), endpoint);
			this.IsActivityBasedPresenceSet = false;
			this.isCurrentlyActivityBasedAway = false;
		}

		internal void TerminateOCSSession()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateOCSSession");
			this.TerminateActiveConversations();
			this.SignOutEndpoint(this.ucEndpoint);
		}

		private UserStateEnum ConvertToUserState(int presence)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ConvertToUserState");
			if (presence <= 6000)
			{
				if (presence == 0)
				{
					return 0;
				}
				if (presence == 3000)
				{
					return 3500;
				}
				if (presence == 6000)
				{
					return 6500;
				}
			}
			else if (presence <= 12000)
			{
				if (presence == 9000)
				{
					return 9500;
				}
				if (presence == 12000)
				{
					return 12500;
				}
			}
			else
			{
				if (presence == 15000)
				{
					return 15500;
				}
				if (presence == 18000)
				{
					return 18500;
				}
			}
			return 0;
		}

		private void TerminateConversation(IConversation conversation)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateConversation. Conversation ID: {0}", conversation.Cid);
			conversation.BeginTerminate(new AsyncCallback(this.ConversationTerminateCallback), conversation);
		}

		private void TerminateActiveConversations()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.TerminateActiveConversations");
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

		private void GeneratePresenceChangePayload(string uri, PresenceAvailabilityEnum newPresence, string userName, StringBuilder payloadBuffer)
		{
			payloadBuffer.Append("['");
			if (uri != null)
			{
				payloadBuffer.Append(Utilities.JavascriptEncode(uri));
			}
			payloadBuffer.Append("',");
			payloadBuffer.Append(newPresence);
			payloadBuffer.Append(",'");
			if (userName != null)
			{
				payloadBuffer.Append(Utilities.JavascriptEncode(userName));
			}
			payloadBuffer.Append("']");
		}

		internal override void NotifyTyping(int chatSessionId, bool typingCanceled)
		{
			try
			{
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Endpoint is null.");
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
			catch (InstantMessagingException arg)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyTyping. Could not notify the typing state. {0}", arg);
			}
		}

		internal override void CreateGroup(string groupName)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.CreateGroup", InstantMessageFailure.SessionDisconnected, null);
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
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroup. Exception message is {0}.", ex);
				throw;
			}
		}

		internal override void RemoveGroup(InstantMessageGroup group)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroup.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroup. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.DeleteGroup. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.DeleteGroup", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			int num;
			if (!int.TryParse(group.Id, out num))
			{
				throw new OwaInvalidRequestException("The group id is not valid. GroupId:" + group.Id);
			}
			try
			{
				InstantMessageOCSProvider.GroupContext groupContext = new InstantMessageOCSProvider.GroupContext(group);
				endpoint.BeginDeleteGroup(num, new AsyncCallback(this.RemoveGroupCallback), groupContext);
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroup. InvalidOperationException.");
					throw new OwaInvalidOperationException("Delete group threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroup. Exception message is {0}.", ex);
				throw;
			}
		}

		internal override void RenameGroup(InstantMessageGroup group, string newGroupName)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroup.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroup. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroup. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RenameGroup", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			int num;
			if (!int.TryParse(group.Id, out num))
			{
				throw new OwaInvalidRequestException("The group id is not valid. GroupId:" + group.Id);
			}
			try
			{
				InstantMessageOCSProvider.GroupContext groupContext = new InstantMessageOCSProvider.GroupContext(group, newGroupName);
				endpoint.BeginUpdateGroup(num, newGroupName, new AsyncCallback(this.RenameGroupCallback), groupContext);
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroup. InvalidOperationException.");
					throw new OwaInvalidOperationException("Rename group threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroup. Exception message is {0}.", ex);
				throw;
			}
		}

		internal override void CopyBuddy(InstantMessageGroup group, InstantMessageBuddy buddy)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddy.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddy. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddy. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.CopyBuddy", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			int item;
			if (!int.TryParse(group.Id, out item))
			{
				throw new OwaInvalidRequestException("The group id is not valid. GroupId:" + group.Id);
			}
			try
			{
				InstantMessageOCSProvider.CopyMoveContext copyMoveContext = new InstantMessageOCSProvider.CopyMoveContext(group, buddy);
				List<int> list = new List<int>();
				foreach (string text in buddy.GroupIds)
				{
					int item2;
					if (!int.TryParse(text, out item2))
					{
						throw new OwaInvalidRequestException("The buddy group id is not valid. GroupId:" + text);
					}
					list.Add(item2);
				}
				if (!list.Contains(item))
				{
					list.Add(item);
				}
				int[] array = list.ToArray();
				endpoint.BeginUpdateContact(buddy.SipUri, buddy.DisplayName, true, buddy.Tagged, array, new AsyncCallback(this.CopyBuddyCallback), copyMoveContext);
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddy. InvalidOperationException.");
					throw new OwaInvalidOperationException("Copy buddy threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddy. Exception message is {0}.", ex);
				throw;
			}
		}

		internal override void MoveBuddy(InstantMessageGroup oldGroup, InstantMessageGroup newGroup, InstantMessageBuddy buddy)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddy.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddy. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddy. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.MoveBuddy", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			int item;
			if (!int.TryParse(oldGroup.Id, out item))
			{
				throw new OwaInvalidRequestException("The group id is not valid. GroupId:" + oldGroup.Id);
			}
			int item2;
			if (!int.TryParse(newGroup.Id, out item2))
			{
				throw new OwaInvalidRequestException("The group id is not valid. GroupId:" + newGroup.Id);
			}
			try
			{
				InstantMessageOCSProvider.CopyMoveContext copyMoveContext = new InstantMessageOCSProvider.CopyMoveContext(oldGroup, newGroup, buddy);
				List<int> list = new List<int>();
				foreach (string text in buddy.GroupIds)
				{
					int item3;
					if (!int.TryParse(text, out item3))
					{
						throw new OwaInvalidRequestException("The buddy group id is not valid. GroupId:" + text);
					}
					list.Add(item3);
				}
				if (list.Contains(item))
				{
					list.Remove(item);
				}
				if (!list.Contains(item2))
				{
					list.Add(item2);
				}
				int[] array = list.ToArray();
				endpoint.BeginUpdateContact(buddy.SipUri, buddy.DisplayName, true, buddy.Tagged, array, new AsyncCallback(this.MoveBuddyCallback), copyMoveContext);
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddy. InvalidOperationException.");
					throw new OwaInvalidOperationException("Move buddy threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddy. Exception message is {0}.", ex);
				throw;
			}
		}

		internal override void RemoveFromGroup(InstantMessageGroup group, InstantMessageBuddy buddy)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroup.");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroup. EndPoint is null.");
				return;
			}
			if (endpoint.EndpointState != 2)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroup. Sending UN response to the client");
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RemoveFromGroup", InstantMessageFailure.SessionDisconnected, null);
				return;
			}
			int item;
			if (!int.TryParse(group.Id, out item))
			{
				throw new OwaInvalidRequestException("The group id is not valid. GroupId:" + group.Id);
			}
			try
			{
				InstantMessageOCSProvider.RemoveFromGroupContext removeFromGroupContext = new InstantMessageOCSProvider.RemoveFromGroupContext(group, buddy);
				List<int> list = new List<int>();
				foreach (string text in buddy.GroupIds)
				{
					int item2;
					if (!int.TryParse(text, out item2))
					{
						throw new OwaInvalidRequestException("The buddy group id is not valid. GroupId:" + text);
					}
					list.Add(item2);
				}
				if (list.Contains(item))
				{
					list.Remove(item);
				}
				int[] array = list.ToArray();
				endpoint.BeginUpdateContact(buddy.SipUri, buddy.DisplayName, true, buddy.Tagged, array, new AsyncCallback(this.RemoveFromGroupCallback), removeFromGroupContext);
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagingError code = ex.Code;
				if (code == 18102)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroup. InvalidOperationException.");
					throw new OwaInvalidOperationException("Remove buddy from group threw an InvalidOperation exception.", ex, this);
				}
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroup. Exception message is {0}.", ex);
				throw;
			}
		}

		private void SignOutCallback(IAsyncResult result)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SignOutCallback.");
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				if (endpoint != null)
				{
					endpoint.EndSignOut(result);
				}
			}
			catch (Exception ex)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorSipEndpointTerminate, string.Empty, new object[]
				{
					this.userContext.SipUri,
					ex
				});
				InstantMessageUtilities.SendInstantMessageWatsonReport(this.userContext, ex);
			}
		}

		private void PublishMouseKeyboardActivityCallBack(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMouseKeyboardActivityCallBack.");
			IEndpoint endpoint = result.AsyncState as IEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMouseKeyboardActivityCallBack. EndPoint is null.");
				return;
			}
			try
			{
				endpoint.EndPublishMouseKeyboardActivity(result);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMouseKeyboardActivityCallback. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18204 || code == 18302)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMouseKeyboardActivityCallBack. Exception. {0}", ex);
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("MSCF(\"");
						stringBuilder.Append(Utilities.JavascriptEncode(ex.Message));
						stringBuilder.Append("\");");
						this.GenerateOperationFailurePayload("PublishMouseKeyboardActivityCallBack", stringBuilder.ToString(), ex, false);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.PublishMouseKeyboardActivityCallBack", this.userContext, exception);
			}
		}

		private void QueryPresenceCallback(IAsyncResult result)
		{
			IEndpoint endpoint = result.AsyncState as IEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback. Endpoint is null.");
				return;
			}
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback.");
				endpoint.EndQueryPresence(result);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.QueryPresenceCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 0 || code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.QueryPresenceCallback. {0}", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.QueryPresenceCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.QueryPresenceCallback", this.userContext, exception);
			}
		}

		private void PublishSelfStateCallback(IAsyncResult result)
		{
			IEndpoint endpoint = result.AsyncState as IEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfStateCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfStateCallback.");
			try
			{
				endpoint.EndPublishSelfState(result);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishSelfStateCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.PublishSelfStateCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					this.GenerateOperationFailurePayload("PublishSelfStateCallback", string.Format(CultureInfo.InvariantCulture, "PCF(\"{0}\");", new object[]
					{
						Utilities.JavascriptEncode(ex.Message)
					}), ex, false);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.PublishSelfStateCallback", this.userContext, exception);
			}
		}

		private void PublishMachineStateCallback(IAsyncResult result)
		{
			IEndpoint endpoint = result.AsyncState as IEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMachineStateCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMachineStateCallback.");
			try
			{
				endpoint.EndPublishMachineState(result);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.PublishMachineStateCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.PublishMachineStateCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("MSCF(\"");
					stringBuilder.Append(Utilities.JavascriptEncode(ex.Message));
					stringBuilder.Append("\");");
					this.GenerateOperationFailurePayload("PublishMachineStateCallback", stringBuilder.ToString(), ex, false);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.PublishMachineStateCallback", this.userContext, exception);
			}
		}

		private void AddBuddyCallback(IAsyncResult result)
		{
			InstantMessageOCSProvider.AddBuddyContext addBuddyContext = result.AsyncState as InstantMessageOCSProvider.AddBuddyContext;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddyCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddyCallback");
			try
			{
				endpoint.EndAddContact(result);
			}
			catch (InstantMessagingException ex)
			{
				this.GenerateOperationFailurePayload("AddBuddyCallback", string.Format(CultureInfo.InvariantCulture, "ACF(\"{0}\");", new object[]
				{
					Utilities.JavascriptEncode(addBuddyContext.Buddy.SipUri)
				}), ex, true);
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AddBuddyCallback. {0}.", ex);
				if (endpoint.EndpointState != 2)
				{
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.AddBuddyCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AddBuddyCallback", this.userContext, exception);
			}
		}

		private void AddContactQueryPresenceCallback(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddContactQueryPresenceCallback");
			InstantMessageOCSProvider.AddBuddyContext addBuddyContext = result.AsyncState as InstantMessageOCSProvider.AddBuddyContext;
			IEndpoint2 endpoint = this.ucEndpoint as IEndpoint2;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddContactQueryPresenceCallback. Endpoint is null.");
				return;
			}
			try
			{
				ICollection<IPresenceNotificationData> collection = endpoint.EndQueryPresence(result);
				if (collection.Count <= 0)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddContactQueryPresenceCallback. Notification result is empty.");
				}
				else
				{
					SourceNetworksEnum sourceNetworksEnum = collection.ElementAt(0).NetworkSource;
					if (sourceNetworksEnum == null)
					{
						sourceNetworksEnum = 1;
					}
					int privacyDefaultContainerId = endpoint.GetPrivacyDefaultContainerId(sourceNetworksEnum);
					IContainerUpdateOperation containerUpdateOperation = endpoint.CreateContainerUpdateOperation(privacyDefaultContainerId);
					containerUpdateOperation.AddUri(addBuddyContext.Buddy.SipUri);
					endpoint.BeginUpdateContainerMembership(new List<IContainerUpdateOperation>
					{
						containerUpdateOperation
					}, new AsyncCallback(this.AddContactUpdateContainerCallback), addBuddyContext);
				}
			}
			catch (InstantMessagingException ex)
			{
				this.GenerateOperationFailurePayload("AddContactQueryPresenceCallback", string.Format(CultureInfo.InvariantCulture, "ACF(\"{0}\");", new object[]
				{
					Utilities.JavascriptEncode(addBuddyContext.Buddy.SipUri)
				}), ex, true);
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AddContactQueryPresenceCallback. {0}.", ex);
				if (addBuddyContext.AckSubscriber && ex.Code != 18204 && ex.Code != 18201)
				{
					this.AcceptBuddyAckSubscriber(addBuddyContext.Buddy.SipUri);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AddContactQueryPresenceCallback", this.userContext, exception);
			}
		}

		private void AddContactUpdateContainerCallback(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AddContactUpdateContainerCallback");
			InstantMessageOCSProvider.AddBuddyContext addBuddyContext = result.AsyncState as InstantMessageOCSProvider.AddBuddyContext;
			IEndpoint2 endpoint = this.ucEndpoint as IEndpoint2;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AddContactUpdateContainerCallback. Endpoint is null.");
				return;
			}
			try
			{
				endpoint.EndUpdateContainerMembership(result);
				List<int> list = new List<int>();
				foreach (string text in addBuddyContext.Buddy.GroupIds)
				{
					int item;
					if (!int.TryParse(text, out item))
					{
						throw new OwaInvalidOperationException("The group id is not valid. GroupId:" + text);
					}
					list.Add(item);
				}
				endpoint.BeginAddContact(addBuddyContext.Buddy.SipUri, string.Empty, true, false, list.ToArray(), new AsyncCallback(this.AddBuddyCallback), addBuddyContext);
				if (addBuddyContext.AckSubscriber)
				{
					this.AcceptBuddyAckSubscriber(addBuddyContext.Buddy.SipUri);
				}
			}
			catch (InstantMessagingException ex)
			{
				this.GenerateOperationFailurePayload("AddContactUpdateContainerCallback", string.Format(CultureInfo.InvariantCulture, "ACF(\"{0}\");", new object[]
				{
					Utilities.JavascriptEncode(addBuddyContext.Buddy.SipUri)
				}), ex, true);
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AddContactUpdateContainerCallback. {0}.", ex);
				if (addBuddyContext.AckSubscriber && ex.Code != 18204 && ex.Code != 18201)
				{
					this.AcceptBuddyAckSubscriber(addBuddyContext.Buddy.SipUri);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AddContactUpdateContainerCallback", this.userContext, exception);
			}
		}

		internal void AcceptBuddyAckSubscriber(string sipUri)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriber.");
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
			catch (InstantMessagingException arg)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-536320848), InstantMessageUtilities.FromSipFormat(sipUri), CultureInfo.InvariantCulture);
				this.HandleFailedResult(errMsg);
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriber. {0}.", arg);
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AcceptBuddyAckSubscriber", this.userContext, exception);
			}
		}

		internal override void GetBuddyList()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.GetBuddyList");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.GetBuddyList. Endpoint is null.");
				return;
			}
			endpoint.BeginRefreshPresenceSession(new AsyncCallback(this.RefreshPresenceSessionCallback), endpoint);
		}

		private void AcceptBuddyAckSubscriberCallback(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriberCallback");
			string sipAddress = result.AsyncState as string;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriberCallback. Endpoint is null.");
				return;
			}
			try
			{
				endpoint.EndAckSubscriber(result);
			}
			catch (InstantMessagingException arg)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-536320848), InstantMessageUtilities.FromSipFormat(sipAddress), CultureInfo.InvariantCulture);
				this.HandleFailedResult(errMsg);
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.AcceptBuddyAckSubscriberCallback. {0}.", arg);
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.AcceptBuddyAckSubscriberCallback", this.userContext, exception);
			}
		}

		private void RemoveBuddyCallback(IAsyncResult result)
		{
			string s = null;
			IEndpoint endpoint = this.ucEndpoint;
			s = (result.AsyncState as string);
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyCallback");
			Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
			try
			{
				endpoint.EndDeleteContact(result);
				InstantMessagePayload payload = this.Payload;
				if (payload == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyCallback. Payload is null.");
				}
				else
				{
					int length;
					lock (payload)
					{
						length = payload.Length;
						payload.Append("RCS('" + Utilities.JavascriptEncode(s) + "');");
					}
					payload.PickupData(length);
				}
			}
			catch (InstantMessagingException ex)
			{
				this.GenerateOperationFailurePayload("RemoveBuddyCallback", string.Format(CultureInfo.InvariantCulture, "RCF(\"{0}\");", new object[]
				{
					Utilities.JavascriptEncode(s)
				}), ex, true);
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveBuddyCallback. {0}.", ex);
				if (endpoint.EndpointState != 2)
				{
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RemoveBuddyCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RemoveBuddyCallback", this.userContext, exception);
			}
		}

		private void SubscribePresenceCallback(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SubscribePresenceCallback");
			IEndpoint endpoint = result.AsyncState as IEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SubscribePresenceCallback. Endpoint is null.");
				return;
			}
			try
			{
				endpoint.EndSubscribePresence(result);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.SubscribePresenceSessionCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SubscribePresenceCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 0 || code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.SubscribePresenceCallback. {0}", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.SubscribePresenceCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.SubscribePresenceCallback", this.userContext, exception);
			}
		}

		private void UnsubscribePresenceCallback(IAsyncResult result)
		{
			IEndpoint endpoint = result.AsyncState as IEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.UnsubscribePresenceCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.UnsubscribePresenceCallback");
			try
			{
				endpoint.EndUnsubscribePresence(result);
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.UnsubscribePresenceCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.UnsubscribePresenceCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 0 || code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.UnsubscribePresenceCallback. {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.UnsubscribePresenceCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.UnsubscribePresenceCallback", this.userContext, exception);
			}
		}

		private void RefreshPresenceSessionCallback(IAsyncResult result)
		{
			IEndpoint endpoint = null;
			endpoint = (result.AsyncState as IEndpoint);
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RefreshPresenceSessionCallback. Endpoint is null.");
				return;
			}
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RefreshPresenceSessionCallback");
				endpoint.EndRefreshPresenceSession(result);
				InstantMessagePayloadUtilities.GenerateUpdatePresencePayload(this.Payload, this.userState);
				InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload(this.Payload, false);
			}
			catch (InstantMessagingException ex)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RefreshPresenceSessionCallback. {0}.", ex);
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RefreshPresenceSessionCallback", InstantMessageFailure.SessionDisconnected, ex);
				InstantMessagingError code = ex.Code;
				if (code != 0 && code != 18201 && code != 18204)
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RefreshPresenceSessionCallback", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RefreshPresenceSessionCallback", this.userContext, exception);
			}
		}

		private void SignInCallback(IAsyncResult result)
		{
			IEndpoint endpoint = null;
			try
			{
				endpoint = (result.AsyncState as IEndpoint);
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback. EndPoint is null.");
				}
				else
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback");
					endpoint.EndSignIn(result);
					if (base.IsDisposed)
					{
						this.TerminateEndpoint(endpoint);
					}
					else
					{
						InstantMessagePayloadUtilities.GenerateInstantMessageSignInPayload(this.Payload, false);
						this.isEarlierSignInSuccessful = true;
						if (Globals.ArePerfCountersEnabled)
						{
							PerformanceCounterManager.UpdateImSignOnTimePerformanceCounter(Globals.ApplicationTime - this.userContext.SignIntoIMTime);
							PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.IMCurrentUsers, 1L);
							OwaSingleCounters.IMTotalUsers.Increment();
							PerformanceCounterManager.AddInstantMessagingLogonResult(true);
						}
						IEndpoint endpoint2 = Interlocked.Exchange<IEndpoint>(ref this.ucEndpoint, endpoint);
						if (endpoint2 != null)
						{
							this.TerminateEndpoint(endpoint2);
							if (Globals.ArePerfCountersEnabled && this.isEndpointRegistered)
							{
								PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.IMCurrentUsers, -1L);
							}
						}
						this.isEndpointRegistered = true;
						this.MakeEndpointMostActive();
						if (!this.isOtherContactsGroupCreated)
						{
							this.CreateGroup("~");
						}
						IEndpoint2 endpoint3 = endpoint as IEndpoint2;
						if (endpoint3 != null && endpoint3.PrivacyModeState == 2)
						{
							this.ProcessSubscribers();
							this.isUserInPrivateMode = true;
						}
						lock (this)
						{
							if (this.isRefreshNeeded)
							{
								this.GetBuddyList();
								this.isRefreshNeeded = false;
							}
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback failed. {0}", ex);
				if (endpoint.EndpointState != 2 && endpoint.EndpointState != 1)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code != 18103)
					{
						switch (code)
						{
						case 18200:
							this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SignInFailure, ex);
							goto IL_413;
						case 18201:
							if (ex.SubCode == 504)
							{
								this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SipEndpointOperationTimeout, ex);
							}
							else
							{
								this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SipEndpointFailureResponse, ex);
							}
							if (Globals.ArePerfCountersEnabled && this.isEarlierSignInSuccessful)
							{
								OwaSingleCounters.IMTotalLogonFailures.Increment();
								PerformanceCounterManager.AddInstantMessagingLogonResult(false);
							}
							this.isEarlierSignInSuccessful = false;
							goto IL_413;
						case 18202:
						{
							this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SipEndpointRegister, ex);
							InstantMessagingErrorSubCode subCode = ex.SubCode;
							if (subCode == 484 || subCode == 504)
							{
								if (Globals.ArePerfCountersEnabled && this.isEarlierSignInSuccessful)
								{
									OwaSingleCounters.IMTotalLogonFailures.Increment();
									PerformanceCounterManager.AddInstantMessagingLogonResult(false);
								}
								this.isEarlierSignInSuccessful = false;
								goto IL_413;
							}
							goto IL_413;
						}
						case 18203:
							goto IL_406;
						case 18204:
							break;
						default:
							if (code != 18302)
							{
								goto IL_406;
							}
							break;
						}
						this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SipEndpointConnectionFailure, ex);
						goto IL_413;
						IL_406:
						InstantMessageUtilities.SendInstantMessageWatsonReport(this.userContext, ex);
					}
					else
					{
						switch (ex.SubCode)
						{
						case 15:
							this.GenerateOperationFailurePayload("SignInCallback. Unsupported Legacy User.", "UNC();", ex, false);
							lock (this.ucEndpoint)
							{
								this.ucEndpoint = null;
							}
							endpoint = (result.AsyncState as IEndpoint);
							if (endpoint == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback. EndPoint is null.");
							}
							else
							{
								endpoint.BeginSignOut(new AsyncCallback(this.SignOutCallback), endpoint);
							}
							break;
						case 16:
							InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Payload, ex, "Unable to sign in due to privacy migration in progress.", InstantMessageFailure.PrivacyMigrationInProgress, false);
							break;
						case 17:
							InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Payload, ex, "Unable to sign in due to privacy migration needed.", InstantMessageFailure.PrivacyMigrationNeeded, false);
							break;
						case 18:
							InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Payload, ex, "Unable to sign in due to privacy policy changed.", InstantMessageFailure.PrivacyPolicyChanged, false);
							break;
						default:
							this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SipEndpointOperationTimeout, ex);
							if (Globals.ArePerfCountersEnabled && this.isEarlierSignInSuccessful)
							{
								OwaSingleCounters.IMTotalLogonFailures.Increment();
								PerformanceCounterManager.AddInstantMessagingLogonResult(false);
							}
							this.isEarlierSignInSuccessful = false;
							break;
						}
					}
				}
				IL_413:;
			}
			catch (Exception ex2)
			{
				if (Globals.ArePerfCountersEnabled && this.isEarlierSignInSuccessful)
				{
					OwaSingleCounters.IMTotalLogonFailures.Increment();
					PerformanceCounterManager.AddInstantMessagingLogonResult(false);
				}
				this.isEarlierSignInSuccessful = false;
				ExTraceGlobals.InstantMessagingTracer.TraceError<Exception>((long)this.GetHashCode(), "InstantMessageOCSProvider.SignInCallback failed. {0}", ex2);
				this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.SignInCallback", InstantMessageFailure.SignInFailure, ex2);
				InstantMessageUtilities.SendInstantMessageWatsonReport(this.userContext, ex2);
			}
			finally
			{
				this.isSigningIn = false;
			}
		}

		private void ConversationTerminateCallback(IAsyncResult result)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ConversationTerminateCallback");
				IConversation conversation = result.AsyncState as IConversation;
				if (conversation == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ConversationTerminateCallback. Conversation is null.");
				}
				else
				{
					int cid = conversation.Cid;
					conversation.EndTerminate(result);
					if (this.messageQueueDictionary != null && this.messageQueueDictionary.ContainsKey(cid))
					{
						InstantMessageQueue instantMessageQueue = this.messageQueueDictionary[cid];
						this.messageQueueDictionary.Remove(cid);
						instantMessageQueue.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<Exception>((long)this.GetHashCode(), "InstantMessageOCSProvider.ConversationTerminateCallback failed. {0}", ex);
				InstantMessageUtilities.SendInstantMessageWatsonReport(this.userContext, ex);
			}
		}

		private void ParticipateCallback(IAsyncResult result)
		{
			IConversation conversation = null;
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback");
				conversation = (result.AsyncState as IConversation);
				if (conversation == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback. Conversation is null.");
				}
				else
				{
					conversation.EndParticipate(result);
					this.SendAndClearMessageList(conversation.Cid);
				}
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.Payload, "InstantMessageOCSProvider.ParticipateCallback", conversation.Cid, ex);
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null || endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback. Ignoring exception after the connection is closed : {0}.", ex);
					InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Payload, "InstantMessageOCSProvider.ParticipateCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else if (conversation == null || conversation.State != 5)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.ParticipateCallback. Ignoring exception if conversation is not connected : {0}.", ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code != 0 && code != 18201)
					{
						switch (code)
						{
						case 18204:
							goto IL_138;
						case 18206:
							goto IL_10D;
						}
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.ParticipateCallback", this.userContext, ex);
						goto IL_138;
					}
					IL_10D:
					if (Globals.ArePerfCountersEnabled)
					{
						OwaSingleCounters.IMTotalMessageDeliveryFailures.Increment();
						PerformanceCounterManager.AddSentInstantMessageResult(false);
					}
				}
				IL_138:;
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.ParticipateCallback", this.userContext, exception);
			}
		}

		private void InstantMessagingParticipateCallback(IAsyncResult result)
		{
			IIMModality iimmodality = null;
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.InstantMessagingParticipateCallback");
				iimmodality = (result.AsyncState as IIMModality);
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.InstantMessagingParticipateCallback. Instant Messaging Modality is null.");
				}
				else
				{
					iimmodality.EndParticipate(result);
				}
			}
			catch (InstantMessagingException ex)
			{
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null || endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.InstantMessagingParticipateCallback. Ignoring exception after the connection is closed : {0}.", ex);
					InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Payload, "InstantMessageOCSProvider.InstantMessagingParticipateCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else if (iimmodality == null || !iimmodality.IsConnected)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.InstantMessagingParticipateCallback. Ignoring exception because IM conversation is not connected : {0}.", ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code != 0)
					{
						switch (code)
						{
						case 18200:
						case 18201:
						case 18204:
						case 18206:
							break;
						default:
							InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.InstantMessagingParticipateCallback", this.userContext, ex);
							break;
						}
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.InstantMessagingParticipateCallback", this.userContext, exception);
			}
		}

		private void NotifyComposingCallback(IAsyncResult result)
		{
			IIMModality iimmodality = null;
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback");
				iimmodality = (result.AsyncState as IIMModality);
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback. Instant Messaging Modality is null.");
				}
				else
				{
					iimmodality.EndNotifyComposing(result);
				}
			}
			catch (InstantMessagingException ex)
			{
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null || endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback. Ignoring exception after the connection is closed : {0}.", ex);
					InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Payload, "InstantMessageOCSProvider.NotifyComposingCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else if (iimmodality == null || !iimmodality.IsConnected)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback. Ignoring exception because IM conversation is not connected : {0}.", ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code <= 18102)
					{
						if (code != 0)
						{
							if (code != 18102)
							{
								goto IL_121;
							}
							if (ex.SubCode != 9)
							{
								InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.NotifyComposingCallback", this.userContext, ex);
								goto IL_132;
							}
							goto IL_132;
						}
					}
					else if (code != 18201 && code != 18204)
					{
						goto IL_121;
					}
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.NotifyComposingCallback. {0}.", ex);
					goto IL_132;
					IL_121:
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.NotifyComposingCallback", this.userContext, ex);
				}
				IL_132:;
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.NotifyComposingCallback", this.userContext, exception);
			}
		}

		private void SendMessageCallback(IAsyncResult result)
		{
			IIMModality iimmodality = null;
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessageCallback");
				iimmodality = (result.AsyncState as IIMModality);
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessageCallback. Instant Messaging Modality is null.");
				}
				else
				{
					iimmodality.EndSendMessage(result);
					if (Globals.ArePerfCountersEnabled)
					{
						PerformanceCounterManager.AddSentInstantMessageResult(true);
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.Payload, "InstantMessageOCSProvider.SendMessageCallback", (iimmodality == null || iimmodality.Conversation == null) ? 0 : iimmodality.Conversation.Cid, ex);
				IEndpoint endpoint = this.ucEndpoint;
				if (endpoint == null || endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessageCallback. Ignoring exception after the connection is closed : {0}.", ex);
					InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Payload, "InstantMessageOCSProvider.SendMessageCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else if (iimmodality == null || !iimmodality.IsConnected)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.SendMessageCallback. Ignoring exception because IM conversation is not connected : {0}.", ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code <= 18102)
					{
						if (code != 0)
						{
							if (code != 18102)
							{
								goto IL_177;
							}
							if (ex.SubCode == 9)
							{
								goto IL_188;
							}
							InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.SendMessageCallback", this.userContext, ex);
							if (Globals.ArePerfCountersEnabled)
							{
								OwaSingleCounters.IMTotalMessageDeliveryFailures.Increment();
								PerformanceCounterManager.AddSentInstantMessageResult(false);
								goto IL_188;
							}
							goto IL_188;
						}
					}
					else if (code != 18201)
					{
						if (code != 18204)
						{
							goto IL_177;
						}
						goto IL_188;
					}
					if (Globals.ArePerfCountersEnabled)
					{
						OwaSingleCounters.IMTotalMessageDeliveryFailures.Increment();
						PerformanceCounterManager.AddSentInstantMessageResult(false);
						goto IL_188;
					}
					goto IL_188;
					IL_177:
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.SendMessageCallback", this.userContext, ex);
				}
				IL_188:;
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.SendMessageCallback", this.userContext, exception);
			}
		}

		private void CreateGroupCallback(IAsyncResult result)
		{
			string arg = result.AsyncState as string;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroupCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroupCallback");
			Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
			try
			{
				endpoint.EndAddGroup(result);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-207298619), arg, CultureInfo.InvariantCulture);
				this.HandleFailedGroupEditResult(errMsg);
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroupCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.CreateGroupCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateGroupCallback. {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.CreateGroupCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.CreateGroupCallback", this.userContext, exception);
			}
		}

		private void RemoveGroupCallback(IAsyncResult result)
		{
			InstantMessageOCSProvider.GroupContext groupContext = result.AsyncState as InstantMessageOCSProvider.GroupContext;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroupCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroupCallback");
			Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
			try
			{
				endpoint.EndDeleteGroup(result);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(1559251810), groupContext.Group.Name, CultureInfo.InvariantCulture);
				this.HandleFailedGroupEditResult(errMsg);
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroupCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RemoveGroupCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveGroupCallback. {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RemoveGroupCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RemoveGroupCallback", this.userContext, exception);
			}
		}

		private void RenameGroupCallback(IAsyncResult result)
		{
			InstantMessageOCSProvider.GroupContext groupContext = result.AsyncState as InstantMessageOCSProvider.GroupContext;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroupCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroupCallback");
			Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
			try
			{
				endpoint.EndUpdateGroup(result);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(1026299268), groupContext.Group.Name, groupContext.NewGroupName, CultureInfo.InvariantCulture);
				this.HandleFailedResult(errMsg, true);
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroupCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RenameGroupCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RenameGroupCallback. {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RenameGroupCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RenameGroupCallback", this.userContext, exception);
			}
		}

		private void CopyBuddyCallback(IAsyncResult result)
		{
			InstantMessageOCSProvider.CopyMoveContext copyMoveContext = result.AsyncState as InstantMessageOCSProvider.CopyMoveContext;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddyCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddyCallback");
			Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
			try
			{
				endpoint.EndUpdateContact(result);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-384888546), copyMoveContext.Buddy.SipUri, copyMoveContext.NewGroup.Name, CultureInfo.InvariantCulture);
				this.HandleFailedClientSideOperationResult(errMsg);
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddyCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.CopyBuddyCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.CopyBuddyCallback. {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.CopyBuddyCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.CopyBuddyCallback", this.userContext, exception);
			}
		}

		private void MoveBuddyCallback(IAsyncResult result)
		{
			InstantMessageOCSProvider.CopyMoveContext copyMoveContext = result.AsyncState as InstantMessageOCSProvider.CopyMoveContext;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddyCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddyCallback");
			Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
			try
			{
				endpoint.EndUpdateContact(result);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-1057312430), copyMoveContext.Buddy.SipUri, copyMoveContext.OldGroup.Name, CultureInfo.InvariantCulture);
				this.HandleFailedClientSideOperationResult(errMsg);
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddyCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.MoveBuddyCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.MoveBuddyCallback. {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.MoveBuddyCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.MoveBuddyCallback", this.userContext, exception);
			}
		}

		private void RemoveFromGroupCallback(IAsyncResult result)
		{
			InstantMessageOCSProvider.RemoveFromGroupContext removeFromGroupContext = result.AsyncState as InstantMessageOCSProvider.RemoveFromGroupContext;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroupCallback. Endpoint is null.");
				return;
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroupCallback");
			Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
			try
			{
				endpoint.EndUpdateContact(result);
			}
			catch (InstantMessagingException ex)
			{
				string errMsg = string.Format(LocalizedStrings.GetNonEncoded(-1057312430), removeFromGroupContext.Buddy.SipUri, removeFromGroupContext.Group.Name, CultureInfo.InvariantCulture);
				this.HandleFailedResult(errMsg, true);
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroupCallback. Ignoring exception after the connection is closed : {0}.", ex);
					this.GenerateInstantMessageUnavailablePayload(endpoint, "InstantMessageOCSProvider.RemoveFromGroupCallback", InstantMessageFailure.SessionDisconnected, ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201 || code == 18204)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.RemoveFromGroupCallback. {0}.", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RemoveFromGroupCallback", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.RemoveFromGroupCallback", this.userContext, exception);
			}
		}

		private void GenerateInstantMessageUnavailablePayload(IEndpoint endPoint, string methodName, InstantMessageFailure errorCode, Exception exception)
		{
			InstantMessagePayloadUtilities.GenerateInstantMessageUnavailablePayload(this.Payload, methodName, errorCode, exception);
			this.SignOutEndpoint(endPoint);
		}

		private void SignOutEndpoint(IEndpoint endPoint)
		{
			if (endPoint != null)
			{
				IEndpoint endpoint = Interlocked.CompareExchange<IEndpoint>(ref this.ucEndpoint, null, endPoint);
				if (this.ucEndpoint == null && endpoint != null)
				{
					if (Globals.ArePerfCountersEnabled && this.isEndpointRegistered)
					{
						PerformanceCounterManager.IncrementCurrentUsersCounterBy(OwaSingleCounters.IMCurrentUsers, -1L);
					}
					this.TerminateEndpoint(endpoint);
					this.isEndpointRegistered = false;
					this.isSelfDataEstablished = false;
					this.isContactGroupEstablished = false;
					this.isOtherContactsGroupCreated = false;
					this.isUserInPrivateMode = false;
					this.userState = 18000;
				}
			}
		}

		private void TerminateEndpoint(IEndpoint endpoint)
		{
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

		private void GenerateOperationFailurePayload(string methodName, string payloadString, Exception exception, bool contactListOperation)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError<string, Exception>((long)this.GetHashCode(), "InstantMessageOCSProvider.{0} failed. {1}", methodName, exception);
			InstantMessagePayload payload = this.Payload;
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.GenerateOperationFailurePayload. Payload is null.");
				return;
			}
			int length;
			lock (payload)
			{
				length = payload.Length;
				payload.Append(payloadString);
			}
			payload.PickupData(length);
		}

		private void PresenceChanged(object sender, IMEventArgs arguments, bool isQuery)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged");
			IUserPresenceEvent userPresenceEvent = (IUserPresenceEvent)arguments.Event;
			if (userPresenceEvent == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. User presence event is null.");
				return;
			}
			InstantMessagePayload payload = this.Payload;
			if (payload == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. Payload is null.");
				return;
			}
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IPresenceNotificationData presenceNotificationData in userPresenceEvent.NotificationDataList)
			{
				INonRichPresenceData nonRichPresenceData = presenceNotificationData.NonRichPresenceData;
				if (nonRichPresenceData != null)
				{
					if (flag)
					{
						stringBuilder.Append(isQuery ? "QPC([" : "PC([");
						flag = false;
					}
					else
					{
						stringBuilder.Append(",");
					}
					this.GeneratePresenceChangePayload(nonRichPresenceData.TargetUri, nonRichPresenceData.AggregatePresenceState, nonRichPresenceData.UserName, stringBuilder);
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
							if (flag)
							{
								stringBuilder.Append(isQuery ? "QPC([" : "PC([");
								flag = false;
							}
							else
							{
								stringBuilder.Append(",");
							}
							string text = null;
							if (richPresenceData.ContactCard != null && richPresenceData.ContactCard.DisplayName != null)
							{
								text = richPresenceData.ContactCard.DisplayName;
								text = text.Substring(1);
							}
							this.GeneratePresenceChangePayload(richPresenceData.Uri, this.MapToPresenceAvailability(presence), text, stringBuilder);
						}
					}
					catch (XmlException arg)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<XmlException>((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. Bad contact. {0}", arg);
						return;
					}
					catch (ArgumentException arg2)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError<ArgumentException>((long)this.GetHashCode(), "InstantMessageOCSProvider.PresenceChanged. ArgumentException. {0}", arg2);
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append("]);");
				lock (payload)
				{
					int length = payload.Length;
					payload.Append(stringBuilder);
					payload.PickupData(length);
				}
			}
		}

		private void OnSelfPresenceSubscriptionStateUpdated(object sender, IMEventArgs arguments)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceSubscriptionStateUpdated");
				IEndpoint endpoint = sender as IEndpoint;
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceSubscriptionStateUpdated. EndPoint is null.");
				}
				else
				{
					ISelfPresenceSubscriptionStateUpdatedEvent selfPresenceSubscriptionStateUpdatedEvent = arguments.Event as ISelfPresenceSubscriptionStateUpdatedEvent;
					switch (selfPresenceSubscriptionStateUpdatedEvent.SubReason)
					{
					case 1:
						InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Payload, null, "Unable to sign in due to privacy policy changed.", InstantMessageFailure.PrivacyPolicyChanged, false);
						this.SignOutEndpoint(endpoint);
						break;
					case 2:
						InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Payload, null, "Unable to sign in due to privacy migration in progress.", InstantMessageFailure.PrivacyMigrationInProgress, false);
						this.SignOutEndpoint(endpoint);
						break;
					case 3:
						InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Payload, null, "Unable to sign in due to privacy migration needed.", InstantMessageFailure.PrivacyMigrationNeeded, false);
						this.SignOutEndpoint(endpoint);
						break;
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnSelfPresenceSubscriptionStateUpdated", this.userContext, exception);
			}
		}

		private void OnSubscriberChanged(object sender, IMEventArgs arguments)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSubscriberChanged");
				ISubscriberEvent subscriberEvent = (ISubscriberEvent)arguments.Event;
				IEndpoint2 endpoint = sender as IEndpoint2;
				if (subscriberEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSubscriberChanged. Subscriber event is null.");
				}
				else if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSubscriberChanged. Endpoint is null.");
				}
				else
				{
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
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnSubscriberChanged", this.userContext, exception);
			}
		}

		private void ProcessSubscribers()
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribers");
				IEndpoint2 endpoint = this.ucEndpoint as IEndpoint2;
				if (endpoint == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribers. Endpoint is null.");
				}
				else
				{
					List<string> list = new List<string>();
					List<ISubscriber> list2 = new List<ISubscriber>();
					StringBuilder stringBuilder = new StringBuilder();
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
							stringBuilder.Append("CRL('');");
						}
						this.pendingSubscribers.Clear();
					}
					if (stringBuilder.Length != 0)
					{
						InstantMessagePayload payload = this.Payload;
						if (payload == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback. Payload is null.");
							return;
						}
						int length;
						lock (payload)
						{
							length = payload.Length;
							payload.Append(stringBuilder);
						}
						payload.PickupData(length);
					}
					if (list.Count != 0)
					{
						endpoint.BeginQueryPresence(list.ToArray(), 4, false, new AsyncCallback(this.ProcessSubscribersQueryPresenceCallback), list2);
					}
					list = null;
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.ProcessSubscribers", this.userContext, exception);
			}
		}

		private void ProcessSubscribersQueryPresenceCallback(IAsyncResult result)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback");
			List<ISubscriber> list = result.AsyncState as List<ISubscriber>;
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback. Endpoint is null.");
				return;
			}
			try
			{
				ICollection<IPresenceNotificationData> collection = endpoint.EndQueryPresence(result);
				InstantMessagePayload payload = this.Payload;
				if (payload == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback. Payload is null.");
				}
				else
				{
					int length;
					lock (payload)
					{
						bool flag2 = true;
						length = payload.Length;
						Dictionary<string, IRichPresenceData> dictionary = new Dictionary<string, IRichPresenceData>();
						payload.Append("PCL([");
						foreach (IPresenceNotificationData presenceNotificationData in collection)
						{
							IRichPresenceData richPresenceData = presenceNotificationData.RichPresenceData;
							if (richPresenceData != null)
							{
								dictionary.Add(InstantMessageUtilities.FromSipFormat(richPresenceData.Uri), richPresenceData);
							}
						}
						foreach (ISubscriber subscriber in list)
						{
							if (!flag2)
							{
								payload.Append(",");
							}
							string id = subscriber.Id;
							string text = string.Empty;
							string text2 = string.Empty;
							string text3 = string.Empty;
							string text4 = string.Empty;
							if (dictionary.ContainsKey(subscriber.Id))
							{
								IRichPresenceData richPresenceData2 = dictionary[subscriber.Id];
								IContactCardCategoryItem contactCard = richPresenceData2.ContactCard;
								if (contactCard != null)
								{
									if (contactCard.DisplayName != null)
									{
										text = Utilities.JavascriptEncode(Utilities.HtmlEncode(contactCard.DisplayName.Replace("\n", string.Empty)));
									}
									if (contactCard.Title != null)
									{
										text2 = Utilities.JavascriptEncode(Utilities.HtmlEncode(contactCard.Title));
									}
									if (contactCard.Company != null)
									{
										text3 = Utilities.JavascriptEncode(Utilities.HtmlEncode(contactCard.Company));
									}
									if (contactCard.Office != null)
									{
										text4 = Utilities.JavascriptEncode(Utilities.HtmlEncode(contactCard.Office));
									}
								}
							}
							if (string.IsNullOrEmpty(text) && subscriber.DisplayName != null)
							{
								text = Utilities.JavascriptEncode(Utilities.HtmlEncode(subscriber.DisplayName));
							}
							payload.Append(string.Format("['{0}','2','','{1}','{2}','{3}','{4}']", new object[]
							{
								id,
								text,
								text2,
								text3,
								text4
							}));
							flag2 = false;
						}
						payload.Append("]);");
					}
					payload.PickupData(length);
				}
			}
			catch (InstantMessagingException arg)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback. {0}.", arg);
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.ProcessSubscribersQueryPresenceCallback", this.userContext, exception);
			}
		}

		private void OnSelfPresenceChanged(object sender, IMEventArgs arguments)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceChanged");
				ISelfPresenceEvent selfPresenceEvent = (ISelfPresenceEvent)arguments.Event;
				if (selfPresenceEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceChanged. Self presence event is null.");
				}
				else
				{
					foreach (IPresenceCategory presenceCategory in selfPresenceEvent.Categories)
					{
						IPresenceCategoryItem presenceCategoryItem = null;
						try
						{
							presenceCategoryItem = presenceCategory.CategoryData;
						}
						catch (XmlException arg)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError<XmlException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnSelfPresenceChanged. Failed to get CategoryData. {0}.", arg);
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
									PresenceAvailabilityEnum presenceAvailabilityEnum = this.MapToPresenceAvailability(aggregateStateCategoryItem.Availability);
									if (presenceAvailabilityEnum != this.userState)
									{
										this.userState = presenceAvailabilityEnum;
										InstantMessagePayloadUtilities.GenerateUpdatePresencePayload(this.Payload, this.userState);
									}
								}
							}
						}
					}
					this.isSelfDataEstablished = true;
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnSelfPresenceChanged", this.userContext, exception);
			}
		}

		private void OnUserPresenceChanged(object sender, IMEventArgs arguments)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnUserPresenceChanged");
				this.PresenceChanged(sender, arguments, false);
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnUserPresenceChanged", this.userContext, exception);
			}
		}

		private void OnQueryPresenceChanged(object sender, IMEventArgs arguments)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnQueryPresenceChanged");
				this.PresenceChanged(sender, arguments, true);
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnQueryPresenceChanged", this.userContext, exception);
			}
		}

		private void OnContactGroupChanged(object sender, IMEventArgs arguments)
		{
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnContactGroupChanged");
				Culture.InternalSetAsyncThreadCulture(this.userContext.UserCulture);
				IContactGroupEvent contactGroupEvent = (IContactGroupEvent)arguments.Event;
				if (contactGroupEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnContactGroupChanged. ContactGroupEvent is null.");
				}
				else
				{
					List<InstantMessageGroup> list = this.ConvertGroups(contactGroupEvent.AddedGroups);
					if (list.Any((InstantMessageGroup g) => g.Type == InstantMessageGroupType.OtherContacts))
					{
						this.isOtherContactsGroupCreated = true;
					}
					list.Sort();
					Dictionary<string, InstantMessageGroup> dictionary = new Dictionary<string, InstantMessageGroup>();
					foreach (InstantMessageGroup instantMessageGroup in list)
					{
						dictionary.Add(instantMessageGroup.Id, instantMessageGroup);
					}
					List<InstantMessageBuddy> list2 = this.ConvertContacts(contactGroupEvent.AddedContacts, dictionary);
					List<InstantMessageBuddy> second = this.ConvertContacts(contactGroupEvent.UpdatedContacts, dictionary);
					list2.Sort();
					InstantMessagePayloadUtilities.GenerateBuddyListPayload(this.Payload, list, list2.Concat(second));
					List<InstantMessageGroup> groups = this.ConvertGroups(contactGroupEvent.DeletedGroups);
					InstantMessagePayloadUtilities.GenerateDeletedGroupsPayload(this.Payload, groups);
					List<InstantMessageGroup> list3 = this.ConvertGroups(contactGroupEvent.UpdatedGroups);
					foreach (InstantMessageGroup group in list3)
					{
						InstantMessagePayloadUtilities.GenerateGroupRenamePayload(this.Payload, group);
					}
					List<InstantMessageBuddy> buddies = this.ConvertContacts(contactGroupEvent.DeletedContacts, dictionary);
					InstantMessagePayloadUtilities.GenerateDeletedBuddiesPayload(this.Payload, buddies);
					this.ExpandedGroupIds = null;
					this.isContactGroupEstablished = true;
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnContactGroupChanged", this.userContext, exception);
			}
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
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. EndPoint is null.");
				return;
			}
			try
			{
				IConversationReceivedEvent conversationReceivedEvent = (IConversationReceivedEvent)arguments.Event;
				if (conversationReceivedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. ConversationReceivedEvent is null.");
				}
				else
				{
					IConversation conversation = conversationReceivedEvent.Conversation;
					if (conversation == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Conversation is null.");
					}
					else
					{
						InstantMessagePayload payload = this.Payload;
						if (payload == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Payload is null.");
						}
						else
						{
							lock (payload)
							{
								int length = payload.Length;
								if (conversationReceivedEvent.Inviter != null)
								{
									string text = conversationReceivedEvent.Inviter.Name;
									if (string.IsNullOrEmpty(text))
									{
										text = InstantMessageUtilities.FromSipFormat(conversationReceivedEvent.Inviter.Uri);
									}
									string uri = conversationReceivedEvent.Inviter.Uri;
									if (!string.IsNullOrEmpty(text) && uri != null)
									{
										payload.Append("TST(\"");
										payload.Append(conversation.Cid.ToString(CultureInfo.InvariantCulture));
										payload.Append("\",\"");
										payload.Append(Utilities.JavascriptEncode(uri));
										payload.Append("\",\"");
										payload.Append(Utilities.JavascriptEncode(Utilities.HtmlEncode(text)));
										payload.Append("\",\"");
										if (!string.IsNullOrEmpty(conversation.Subject))
										{
											payload.Append(Utilities.JavascriptEncode(Utilities.HtmlEncode(conversation.Subject)));
										}
										else if (conversationReceivedEvent.ToastMessage != null && !string.IsNullOrEmpty(conversationReceivedEvent.ToastMessage.Content))
										{
											payload.Append(Utilities.JavascriptEncode(Utilities.HtmlEncode(conversationReceivedEvent.ToastMessage.Content)));
										}
										payload.Append("\",");
										payload.Append(conversationReceivedEvent.IsConference ? 1 : 0);
										payload.Append(");");
									}
								}
								payload.PickupData(length);
							}
							IIMModality iimmodality = conversation.GetModality(1) as IIMModality;
							if (iimmodality == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Instant Messaging Modality is null.");
							}
							else
							{
								conversation.ConversationStateChanged += new IMEventHandler(this.OnConversationStateChanged);
								iimmodality.MessageReceived += new IMEventHandler(this.OnMessageReceived);
								iimmodality.MessageSendFailed += new IMEventHandler(this.OnMessageSendFailed);
								iimmodality.ComposingStateChanged += new IMEventHandler(this.OnComposingStateChanged);
								iimmodality.ModalityParticipantUpdated += new IMEventHandler(this.OnModalityParticipantUpdated);
								iimmodality.ModalityParticipantRemoved += new IMEventHandler(this.OnModalityParticipantRemoved);
								conversation.ParticipantUpdated += new IMEventHandler(this.OnParticipantUpdated);
								conversation.ParticipantRemoved += new IMEventHandler(this.OnParticipantRemoved);
							}
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationReceived. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnConversationReceived", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnConversationReceived", this.userContext, exception);
			}
		}

		private void OnParticipantUpdated(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. EndPoint is null.");
				return;
			}
			try
			{
				IParticipantUpdatedEvent participantUpdatedEvent = (IParticipantUpdatedEvent)arguments.Event;
				if (participantUpdatedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. ParticipantUpdatedEvent is null.");
				}
				else
				{
					IConversation conversation = participantUpdatedEvent.Conversation;
					if (conversation == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. Conversation is null.");
					}
					else
					{
						IParticipantInfo participantInfo = participantUpdatedEvent.ParticipantInfo;
						if (participantInfo == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. ParticipantInfo is null.");
						}
						else if (conversation.IsConference)
						{
							if (string.Compare(this.userContext.SipUri, participantInfo.Uri, StringComparison.OrdinalIgnoreCase) != 0)
							{
								InstantMessagePayloadUtilities.GenerateParticipantJoinedPayload(this.Payload, conversation.Cid.ToString(CultureInfo.InvariantCulture), participantInfo.Uri, participantInfo.Name);
							}
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantUpdated. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnParticipantUpdated", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnParticipantUpdated", this.userContext, exception);
			}
		}

		private void OnParticipantRemoved(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved. EndPoint is null.");
				return;
			}
			try
			{
				IParticipantRemovedEvent participantRemovedEvent = (IParticipantRemovedEvent)arguments.Event;
				if (participantRemovedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved. ParticipantRemovedEvent is null.");
				}
				else
				{
					IConversation conversation = participantRemovedEvent.Conversation;
					if (conversation == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved. Conversation is null.");
					}
					else if (conversation.IsConference)
					{
						InstantMessagePayloadUtilities.GenerateParticipantLeftPayload(this.Payload, conversation.Cid.ToString(CultureInfo.InvariantCulture), participantRemovedEvent.ParticipantUri);
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnParticipantRemoved. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnParticipantRemoved", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnParticipantRemoved", this.userContext, exception);
			}
		}

		private void OnConversationStateChanged(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. EndPoint is null.");
				return;
			}
			try
			{
				IConversationStateChangedEvent conversationStateChangedEvent = (IConversationStateChangedEvent)arguments.Event;
				if (conversationStateChangedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. ConversationStateChangedEvent is null.");
				}
				else
				{
					IConversation conversation = conversationStateChangedEvent.Conversation;
					if (conversation == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. Conversation is null.");
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
							InstantMessagePayload payload = this.Payload;
							if (payload == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. Payload is null.");
							}
							else
							{
								int length;
								lock (payload)
								{
									length = payload.Length;
									payload.Append("EIM(\"");
									payload.Append(conversation.Cid.ToString(CultureInfo.InvariantCulture));
									payload.Append("\");");
								}
								payload.PickupData(length);
							}
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnConversationStateChanged. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnConversationStateChanged", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnConversationStateChanged", this.userContext, exception);
			}
		}

		private void SendAndClearMessageList(int chatId)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageOCSProvider.SendAndClearMessageList. Chat ID: {0}", chatId);
			if (this.messageQueueDictionary != null && this.messageQueueDictionary.ContainsKey(chatId))
			{
				InstantMessageQueue instantMessageQueue = this.messageQueueDictionary[chatId];
				instantMessageQueue.SendAndClearMessageList();
			}
		}

		private void OnMessageReceived(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. EndPoint is null.");
				return;
			}
			try
			{
				IMessageReceivedEvent messageReceivedEvent = (IMessageReceivedEvent)arguments.Event;
				if (messageReceivedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. MessageReceivedEvent is null.");
				}
				else
				{
					IIMModality iimmodality = sender as IIMModality;
					if (iimmodality == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Instant Messaging Modality is null.");
					}
					else
					{
						IConversation conversation = iimmodality.Conversation;
						if (conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Conversation is null.");
						}
						else
						{
							InstantMessagePayload payload = this.Payload;
							if (payload == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Payload is null.");
							}
							else
							{
								int length;
								lock (payload)
								{
									length = payload.Length;
									if (conversation.State == 1)
									{
										payload.Append("NIM(\"");
									}
									else
									{
										payload.Append("IM(\"");
									}
									if (iimmodality.Conversation != null)
									{
										payload.Append(Utilities.JavascriptEncode(iimmodality.Conversation.Cid.ToString(CultureInfo.InvariantCulture)));
									}
									payload.Append("\",\"");
									payload.Append(Utilities.JavascriptEncode(messageReceivedEvent.SenderUri));
									payload.Append("\",\"");
									if (messageReceivedEvent.Message != null)
									{
										payload.Append(Utilities.JavascriptEncode(messageReceivedEvent.Message.Content));
									}
									payload.Append("\",\"");
									if (messageReceivedEvent.Message != null)
									{
										payload.Append(Utilities.JavascriptEncode(messageReceivedEvent.Message.Format));
									}
									if (iimmodality.Conversation.Subject != null && conversation.State == 1)
									{
										payload.Append("\",\"");
										payload.Append(iimmodality.Conversation.Subject);
									}
									payload.Append("\");");
									InstantMessageOCSPayload instantMessageOCSPayload = payload as InstantMessageOCSPayload;
									if (instantMessageOCSPayload != null)
									{
										instantMessageOCSPayload.RegisterDeliverySuccessNotification(iimmodality, messageReceivedEvent.MessageId);
									}
									else
									{
										ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Payload is not an OCS payload.");
									}
								}
								payload.PickupData(length);
								if (Globals.ArePerfCountersEnabled)
								{
									OwaSingleCounters.IMTotalInstantMessagesReceived.Increment();
								}
							}
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessagingError code = ex.Code;
					if (code == 18201)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageReceived. OcsFailureResponse. {0}", ex);
					}
					else
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnMessageReceived", this.userContext, ex);
					}
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnMessageReceived", this.userContext, exception);
			}
		}

		private void OnMessageSendFailed(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. EndPoint is null.");
				return;
			}
			try
			{
				IMessageSendFailedEvent messageSendFailedEvent = (IMessageSendFailedEvent)arguments.Event;
				if (messageSendFailedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. MessageSendFailedEvent is null.");
				}
				else
				{
					IIMModality iimmodality = messageSendFailedEvent.Modality as IIMModality;
					if (iimmodality == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. Instant Messaging Modality is null.");
					}
					else
					{
						IConversation conversation = iimmodality.Conversation;
						if (conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. Conversation is null.");
						}
						else
						{
							InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.Payload, "InstantMessageOCSProvider.OnMessageSendFailed", conversation.Cid, null);
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnMessageSendFailed. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnMessageSendFailed", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnMessageSendFailed", this.userContext, exception);
			}
		}

		private void OnComposingStateChanged(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnComposingStateChanged");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnComposingStateChanged. EndPoint is null.");
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
						InstantMessageAlert alertType = InstantMessageAlert.StoppedTyping;
						if (composingStateEvent.State == 1)
						{
							alertType = InstantMessageAlert.Typing;
						}
						InstantMessagePayloadUtilities.GenerateInstantMessageAlertPayload(this.Payload, iimmodality.Conversation.Cid.ToString(CultureInfo.InvariantCulture), alertType, composingStateEvent.ComposerUri);
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnComposingStateChanged. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnModalityParticipantUpdated", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnModalityParticipantUpdated", this.userContext, exception);
			}
		}

		private void OnModalityParticipantUpdated(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. EndPoint is null.");
				return;
			}
			try
			{
				IModalityParticipantUpdatedEvent modalityParticipantUpdatedEvent = (IModalityParticipantUpdatedEvent)arguments.Event;
				if (modalityParticipantUpdatedEvent == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. ParticipantUpdatedEvent is null.");
				}
				else
				{
					IIMModality iimmodality = modalityParticipantUpdatedEvent.Modality as IIMModality;
					if (iimmodality == null)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Instant Messaging Modality is null.");
					}
					else
					{
						IConversation conversation = iimmodality.Conversation;
						if (conversation == null)
						{
							ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Conversation is null.");
						}
						else
						{
							IParticipantInfo participantInfo = modalityParticipantUpdatedEvent.ParticipantInfo;
							if (participantInfo == null)
							{
								ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. ParticipantInfo is null.");
							}
							else if (conversation.IsConference)
							{
								if (string.Compare(this.userContext.SipUri, participantInfo.Uri, StringComparison.OrdinalIgnoreCase) != 0)
								{
									InstantMessagePayloadUtilities.GenerateParticipantJoinedPayload(this.Payload, iimmodality.Conversation.Cid.ToString(CultureInfo.InvariantCulture), participantInfo.Uri, participantInfo.Name);
								}
							}
						}
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantUpdated. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnModalityParticipantUpdated", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnModalityParticipantUpdated", this.userContext, exception);
			}
		}

		private void OnModalityParticipantRemoved(object sender, IMEventArgs arguments)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantRemoved");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantRemoved. EndPoint is null.");
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
						InstantMessagePayloadUtilities.GenerateParticipantLeftPayload(this.Payload, iimmodality.Conversation.Cid.ToString(CultureInfo.InvariantCulture), modalityParticipantRemovedEvent.ParticipantUri);
					}
				}
			}
			catch (InstantMessagingException ex)
			{
				if (endpoint.EndpointState != 2)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.OnModalityParticipantRemoved. Ignoring exception after the connection is closed : {0}.", ex);
				}
				else
				{
					InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnModalityParticipantRemoved", this.userContext, ex);
				}
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.OnModalityParticipantRemoved", this.userContext, exception);
			}
		}

		private void QueueMessage(IConversation conversation, string messageFormat, string message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.QueueMessage");
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
			InstantMessageQueue instantMessageQueue2 = new InstantMessageQueue(this.userContext, conversation, this.Payload);
			instantMessageQueue2.AddMessage(messageFormat, message);
			this.messageQueueDictionary.Add(conversation.Cid, instantMessageQueue2);
		}

		private PresenceAvailabilityEnum MapToPresenceAvailability(long presence)
		{
			if (presence >= 18000L)
			{
				return 18000;
			}
			if (presence >= 15000L)
			{
				return 15000;
			}
			if (presence >= 12000L)
			{
				return 12000;
			}
			if (presence >= 9000L)
			{
				return 9000;
			}
			if (presence >= 7500L)
			{
				return 7500;
			}
			if (presence >= 6000L)
			{
				return 6000;
			}
			if (presence >= 4500L)
			{
				return 4500;
			}
			if (presence >= 3000L)
			{
				return 3000;
			}
			return 0;
		}

		private void CreateEndpoint()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateEndpoint");
			IEndpoint endpoint = this.ucEndpoint;
			if (endpoint == null)
			{
				try
				{
					endpoint = InstantMessageOCSProvider.EndpointManager.CreateEndpoint(this.userContext.SipUri, this.serverName, 5, null, null);
				}
				catch (InstantMessagingException ex)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError<InstantMessagingException>((long)this.GetHashCode(), "InstantMessageOCSProvider.CreateEndpoint failed. {0}", ex);
					OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ErrorIMCreateEndpointFailure, string.Empty, new object[]
					{
						(this.userContext.MailboxSession != null) ? this.userContext.MailboxSession.DisplayName : this.userContext.SipUri,
						this.userContext.SipUri,
						ex
					});
					InstantMessagePayloadUtilities.GenerateUnavailablePayload(this.Payload, ex, "Failed to create an endpoint.", InstantMessageFailure.CreateEndpointFailure, false);
					if (!(ex.InnerException is ArgumentException))
					{
						InstantMessageUtilities.SendWatsonReport("InstantMessageOCSProvider.CreateEndpoint", this.userContext, ex);
					}
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
				this.isSigningIn = true;
				endpoint.BeginSignIn(false, 0, new AsyncCallback(this.SignInCallback), endpoint);
				return;
			}
			if (endpoint.EndpointState == null || endpoint.EndpointState == 5)
			{
				endpoint.BeginSignIn(false, 0, new AsyncCallback(this.SignInCallback), endpoint);
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

		private void HandleFailedGroupEditResult(string errMsg)
		{
			this.HandleFailedResult(errMsg, "REGE", true);
		}

		private void HandleFailedClientSideOperationResult(string errMsg)
		{
			this.HandleFailedResult(errMsg, "REMB", true);
		}

		private void HandleFailedResult(string errMsg)
		{
			this.HandleFailedResult(errMsg, "RE", false);
		}

		private void HandleFailedResult(string errMsg, bool contactListOperation)
		{
			this.HandleFailedResult(errMsg, "RE", contactListOperation);
		}

		private void HandleFailedResult(string errMsg, string clientPayloadMethod, bool contactListOperation)
		{
			this.ReportError(errMsg, clientPayloadMethod, contactListOperation);
		}

		private void ReportError(string msg, string payloadFunction, bool contactListOperation)
		{
			if (string.IsNullOrEmpty(msg))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length;
			lock (this.Payload)
			{
				length = this.Payload.Length;
				stringBuilder.Append(payloadFunction);
				stringBuilder.Append("(\"");
				stringBuilder.Append(Utilities.JavascriptEncode(msg));
				stringBuilder.Append("\");");
				if (stringBuilder.Length > 0)
				{
					this.Payload.Append(stringBuilder);
					stringBuilder.Remove(0, stringBuilder.Length);
				}
			}
			this.Payload.PickupData(length);
		}

		private List<InstantMessageGroup> ConvertGroups(ICollection<IGroup> groups)
		{
			List<InstantMessageGroup> list = new List<InstantMessageGroup>();
			foreach (IGroup group in groups)
			{
				if (group != null && !group.IsDG)
				{
					InstantMessageGroupType type = InstantMessageGroupType.Standard;
					if (group.GroupData != null && group.GroupData.Contains("groupType=\"pinnedGroup\""))
					{
						type = InstantMessageGroupType.Pinned;
					}
					InstantMessageGroup instantMessageGroup = InstantMessageGroup.Create(group.GroupId.ToString(), group.Name ?? string.Empty, type);
					instantMessageGroup.SetExpandedState(this.ExpandedGroupIds);
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
					InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create(string.Empty, contact.Uri, this.GetContactDisplayName(contact));
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

		private const string UserAgentFormat = "OWA/{0}";

		private static int[] mtlsPortNumbers = new int[]
		{
			5075,
			5076,
			5077
		};

		private PresenceAvailabilityEnum userState;

		private string serverName;

		private volatile bool isSigningIn;

		private volatile bool isRefreshNeeded;

		private IEndpoint ucEndpoint;

		private volatile bool isEndpointRegistered;

		private volatile bool isSelfDataEstablished;

		private volatile bool isContactGroupEstablished;

		private volatile bool isCurrentlyActivityBasedAway;

		private Dictionary<int, InstantMessageQueue> messageQueueDictionary;

		private volatile bool isOtherContactsGroupCreated;

		private List<ISubscriber> pendingSubscribers = new List<ISubscriber>();

		private volatile bool isUserInPrivateMode;

		private volatile bool isNotifyAdditionToContactList = true;

		private ContentType defaultContentType = new ContentType("text/plain;charset=utf-8");

		private class AddBuddyContext
		{
			public InstantMessageBuddy Buddy { get; private set; }

			public bool AckSubscriber { get; private set; }

			public AddBuddyContext(InstantMessageBuddy buddy, bool ackSubscriber)
			{
				this.Buddy = buddy;
				this.AckSubscriber = ackSubscriber;
			}
		}

		private class GroupContext
		{
			public InstantMessageGroup Group { get; private set; }

			public string NewGroupName { get; private set; }

			public GroupContext(InstantMessageGroup group)
			{
				this.Group = group;
			}

			public GroupContext(InstantMessageGroup group, string newGroupName)
			{
				this.Group = group;
				this.NewGroupName = newGroupName;
			}
		}

		private class CopyMoveContext
		{
			public InstantMessageGroup OldGroup { get; private set; }

			public InstantMessageGroup NewGroup { get; private set; }

			public InstantMessageBuddy Buddy { get; private set; }

			public CopyMoveContext(InstantMessageGroup newGroup, InstantMessageBuddy buddy)
			{
				this.NewGroup = newGroup;
				this.Buddy = buddy;
			}

			public CopyMoveContext(InstantMessageGroup oldGroup, InstantMessageGroup newGroup, InstantMessageBuddy buddy)
			{
				this.OldGroup = oldGroup;
				this.NewGroup = newGroup;
				this.Buddy = buddy;
			}
		}

		private class RemoveFromGroupContext
		{
			public InstantMessageGroup Group { get; private set; }

			public InstantMessageBuddy Buddy { get; private set; }

			public RemoveFromGroupContext(InstantMessageGroup group, InstantMessageBuddy buddy)
			{
				this.Group = group;
				this.Buddy = buddy;
			}
		}
	}
}

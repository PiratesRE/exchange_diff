using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	internal abstract class UserNotificationEvent : DisposableBase
	{
		internal UserNotificationEvent(string user, Guid recipientObjectGuid, Guid tenantGuid, XmlNode eventData)
		{
			this.callId = UserNotificationEvent.GetNodeValue(eventData, "CallId");
			this.subject = UserNotificationEvent.GetNodeValue(eventData, "Subject");
			string nodeValue = UserNotificationEvent.GetNodeValue(eventData, "Priority");
			if (!string.IsNullOrEmpty(nodeValue))
			{
				if (!UserNotificationEvent.importanceMap.ContainsKey(nodeValue))
				{
					throw new ArgumentException("Priority");
				}
				this.importance = UserNotificationEvent.importanceMap[nodeValue];
			}
			else
			{
				this.importance = Importance.Normal;
			}
			user = UserNotificationEvent.FormatTarget(user, "User");
			if (string.IsNullOrEmpty(user))
			{
				CallIdTracer.TraceError(ExTraceGlobals.OCSNotifEventsTracer, 0, "UserNotificationEvent(): user is null or empty", new object[0]);
				throw new ArgumentException("User");
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "UserNotificationEvent(): looking up recipient by recipient object GUID = {0}", new object[]
			{
				recipientObjectGuid
			});
			this.tenantGuid = tenantGuid;
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromTenantGuid(tenantGuid);
			ADRecipient adrecipient = iadrecipientLookup.LookupByObjectId(new ADObjectId(recipientObjectGuid));
			this.subscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(adrecipient);
			if (this.subscriber == null)
			{
				PIIMessage data = PIIMessage.Create(PIIType._User, user);
				CallIdTracer.TraceError(ExTraceGlobals.OCSNotifEventsTracer, 0, data, "UserNotificationEvent(): user _User is not UM enabled", new object[0]);
				throw new UserNotUmEnabledException(user);
			}
			string nodeValue2 = UserNotificationEvent.GetNodeValue(eventData, "From");
			string text = UserNotificationEvent.FormatTarget(nodeValue2, "From");
			if (string.IsNullOrEmpty(text))
			{
				this.callerId = PhoneNumber.Empty;
				return;
			}
			if (!PhoneNumber.TryParse(text, out this.callerId))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "FormatTarget({0}): Invalid target {1}", new object[]
				{
					"From",
					text
				});
				throw new ArgumentException("From");
			}
		}

		public Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		internal string CallId
		{
			get
			{
				return this.callId;
			}
		}

		internal ExDateTime Time
		{
			get
			{
				return this.time;
			}
		}

		internal PhoneNumber CallerId
		{
			get
			{
				return this.callerId;
			}
		}

		internal UMSubscriber Subscriber
		{
			get
			{
				return this.subscriber;
			}
		}

		internal string Subject
		{
			get
			{
				return this.subject;
			}
		}

		protected abstract string MessageClass { get; }

		internal static void Initialize()
		{
			UmServiceGlobals.VoipPlatform.OnMessageReceived += UserNotificationEvent.HandleServiceRequest;
		}

		internal static UserNotificationEvent Deserialize(string xmlData)
		{
			UserNotificationEvent result;
			try
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "UserNotificationEvent.Deserialize: {0}", new object[]
				{
					xmlData
				});
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xmlData);
				XmlNode xmlNode = UserNotificationEvent.SelectNode(xmlDocument, "UserNotification");
				string nodeValue = UserNotificationEvent.GetNodeValue(xmlNode, "User");
				string nodeValue2 = UserNotificationEvent.GetNodeValue(xmlNode, "Template");
				string nodeValue3 = UserNotificationEvent.GetNodeValue(xmlNode, "RecipientObjectGuid");
				if (nodeValue3 == null)
				{
					throw new ArgumentException("RecipientObjectGuid");
				}
				Guid recipientObjectGuid = new Guid(nodeValue3);
				string nodeValue4 = UserNotificationEvent.GetNodeValue(xmlNode, "TenantGuid");
				if (nodeValue4 == null)
				{
					throw new ArgumentException("TenantGuid");
				}
				Guid guid = new Guid(nodeValue4);
				string nodeValue5 = UserNotificationEvent.GetNodeValue(xmlNode, "Time");
				ExDateTime now;
				if (string.IsNullOrEmpty(nodeValue5) || !ExDateTime.TryParse(nodeValue5, out now))
				{
					now = ExDateTime.Now;
				}
				XmlNode xmlNode2 = UserNotificationEvent.SelectNode(xmlNode, "Event");
				XmlAttribute xmlAttribute = xmlNode2.Attributes["type"];
				if (xmlAttribute == null)
				{
					throw new ArgumentException("type");
				}
				UserNotificationEvent userNotificationEvent;
				switch ((UserNotificationEvent.UserNotificationEventType)Enum.Parse(typeof(UserNotificationEvent.UserNotificationEventType), xmlAttribute.Value, true))
				{
				case UserNotificationEvent.UserNotificationEventType.Missed:
					userNotificationEvent = new MissedUserNotificationEvent(nodeValue, recipientObjectGuid, guid, xmlNode2);
					break;
				case UserNotificationEvent.UserNotificationEventType.Answered:
					if ((UserNotificationEvent.RoutingGroupClass)Enum.Parse(typeof(UserNotificationEvent.RoutingGroupClass), UserNotificationEvent.GetNodeValue(xmlNode2, "TargetClass"), true) == UserNotificationEvent.RoutingGroupClass.Primary)
					{
						userNotificationEvent = new CallForwardedUserNotificationEvent(nodeValue, recipientObjectGuid, guid, nodeValue2, xmlNode2);
					}
					else
					{
						userNotificationEvent = new TeamPickUpUserNotificationEvent(nodeValue, recipientObjectGuid, guid, nodeValue2, xmlNode2);
					}
					break;
				case UserNotificationEvent.UserNotificationEventType.Forbidden:
					userNotificationEvent = new CallNotForwardedUserNotificationEvent(nodeValue, recipientObjectGuid, guid, nodeValue2, xmlNode2);
					break;
				default:
					throw new ArgumentException("type");
				}
				userNotificationEvent.time = now;
				result = userNotificationEvent;
			}
			catch (XmlException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.OCSNotifEventsTracer, 0, "UserNotificationEvent.Deserialize exception: {0}", new object[]
				{
					ex
				});
				throw new NotificationEventFormatException(ex);
			}
			catch (ArgumentException ex2)
			{
				CallIdTracer.TraceError(ExTraceGlobals.OCSNotifEventsTracer, 0, "UserNotificationEvent.Deserialize exception: {0}", new object[]
				{
					ex2
				});
				throw new NotificationEventFormatException(ex2);
			}
			catch (FormatException ex3)
			{
				CallIdTracer.TraceError(ExTraceGlobals.OCSNotifEventsTracer, 0, "UserNotificationEvent.Deserialize exception: {0}", new object[]
				{
					ex3
				});
				throw new NotificationEventFormatException(ex3);
			}
			return result;
		}

		internal static void ValidateAndSubmitMessage(string fromUri, OrganizationId organizationId, byte[] messageBody)
		{
			try
			{
				string text = Encoding.ASCII.GetString(messageBody);
				CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "ValidateAndSubmitMessage: {0}", new object[]
				{
					text
				});
				text = UserNotificationEvent.ValidateAndResolveTargetUser(fromUri, organizationId, text);
				CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "Processed xml: {0}", new object[]
				{
					text
				});
				UMMessageSubmission.SubmitOCSMessage(text);
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.OCSNotifEventsTracer, 0, "ValidateAndSubmitMessage: {0}", new object[]
				{
					ex
				});
				CallEndingReason endingReason = CallEndingReason.TransientError;
				if (ex is UserNotUmEnabledException)
				{
					endingReason = CallEndingReason.MailboxIsNotUMEnabled;
				}
				else if (ex is ArgumentException || ex is NotificationEventFormatException)
				{
					endingReason = CallEndingReason.InvalidRequest;
				}
				else if (ex is IOException || ex is UnauthorizedAccessException || ex is DataValidationException || ex is DataSourceOperationException || ex is DataSourceTransientException)
				{
					endingReason = CallEndingReason.TransientError;
				}
				else
				{
					if (!GrayException.IsGrayException(ex))
					{
						throw;
					}
					endingReason = CallEndingReason.TransientError;
					ExceptionHandling.SendWatsonWithoutDumpWithExtraData(ex);
				}
				throw CallRejectedException.Create(Strings.InvalidRequest, endingReason, null, new object[0]);
			}
		}

		internal static string ExtractEumProxyAddressFromXml(byte[] messageBody)
		{
			string text = null;
			try
			{
				string @string = Encoding.ASCII.GetString(messageBody);
				CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "ReadEumProxyAddressFromXml: {0}", new object[]
				{
					@string
				});
				using (Stream stream = new MemoryStream(messageBody))
				{
					using (XmlReader xmlReader = XmlReader.Create(stream))
					{
						while (xmlReader.Read())
						{
							if (string.Equals(xmlReader.Name, "EumProxyAddress"))
							{
								if (xmlReader.Read())
								{
									text = xmlReader.Value;
									break;
								}
								break;
							}
						}
					}
				}
			}
			catch (XmlException ex)
			{
				throw new ArgumentException(ex.Message, "messageBody", ex);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "EumProxyAddress: {0}", new object[]
			{
				text
			});
			ValidateArgument.NotNullOrEmpty(text, "EumProxyAddress");
			return text;
		}

		internal void RenderMessage(MessageItem message, ContactInfo callerInfo)
		{
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(this.subscriber.MessageSubmissionCulture);
			this.InternalRenderMessage(messageContentBuilder, message, callerInfo);
			message.Importance = this.importance;
			message[MessageItemSchema.UcSubject] = (this.Subject ?? string.Empty);
			using (MemoryStream memoryStream = messageContentBuilder.ToStream())
			{
				using (Stream stream = message.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.UTF8.Name)))
				{
					memoryStream.WriteTo(stream);
				}
			}
			if (!string.IsNullOrEmpty(this.MessageClass))
			{
				message.ClassName = this.MessageClass;
			}
		}

		private static void HandleServiceRequest(BaseUMVoipPlatform voipPlatform, InfoMessage.PlatformMessageReceivedEventArgs e)
		{
			e.ResponseCode = 403;
			SipRoutingHelper sipRoutingHelper = SipRoutingHelper.Create(e.CallInfo);
			PlatformSipUri uri = e.CallInfo.CallingParty.Uri;
			string text = UserNotificationEvent.FormatTarget(uri.ToString(), "From");
			SipRoutingHelper.Context routingContext = sipRoutingHelper.GetRoutingContext(text, e.CallInfo.RequestUri);
			if (routingContext == null || routingContext.Recipient == null)
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.CouldNotFindUserBySipUri, "User: {0}.", new object[]
				{
					text
				});
			}
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(routingContext.Recipient))
			{
				if (umsubscriber == null)
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.MailboxIsNotUMEnabled, "User: {0}.", new object[]
					{
						text
					});
				}
				PipelineSubmitStatus pipelineSubmitStatus = PipelineDispatcher.Instance.CanSubmitLowPriorityWorkItem(umsubscriber.ADUser.ServerLegacyDN, umsubscriber.ADUser.DistinguishedName, PipelineDispatcher.ThrottledWorkItemType.NonCDRWorkItem);
				if (pipelineSubmitStatus != PipelineSubmitStatus.Ok)
				{
					PIIMessage data = PIIMessage.Create(PIIType._User, umsubscriber);
					CallIdTracer.TraceWarning(ExTraceGlobals.OCSNotifEventsTracer, 0, data, "UserNotificationEvent: pipeline is full for user '_User'. Rejecting the message.", new object[0]);
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.PipelineFull, "User: {0}.", new object[]
					{
						text
					});
				}
				UserNotificationEvent.ValidateAndSubmitMessage(uri.ToString(), routingContext.Recipient.OrganizationId, e.Message.Body);
				e.ResponseCode = 200;
			}
		}

		protected static string GetNodeValue(XmlNode inputNode, string query)
		{
			XmlNode xmlNode = inputNode.SelectSingleNode(query);
			if (xmlNode == null)
			{
				return null;
			}
			return xmlNode.InnerText;
		}

		protected static void SetNodeValue(XmlNode parentNode, string childName, string textValue)
		{
			XmlNode xmlNode = parentNode.SelectSingleNode(childName);
			if (xmlNode == null)
			{
				xmlNode = parentNode.OwnerDocument.CreateElement(childName);
				parentNode.AppendChild(xmlNode);
			}
			xmlNode.InnerText = textValue;
		}

		protected static XmlNode SelectNode(XmlNode parent, string name)
		{
			XmlNode xmlNode = parent.SelectSingleNode(name);
			if (xmlNode == null)
			{
				throw new ArgumentException(name);
			}
			return xmlNode;
		}

		protected static bool DefaultTryParseTargetUser(string target, out string userPart, out string hostPart, out bool isPhoneNumber)
		{
			userPart = string.Empty;
			hostPart = string.Empty;
			isPhoneNumber = false;
			PlatformSipUri platformSipUri = null;
			if (Platform.Builder.TryCreateSipUri(target, out platformSipUri))
			{
				userPart = platformSipUri.User;
				hostPart = platformSipUri.Host;
				isPhoneNumber = (platformSipUri.UserParameter == UserParameter.Phone);
				return true;
			}
			return false;
		}

		protected static string FormatTarget(string target, string fieldName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "FormatTarget({0}): target {1}", new object[]
			{
				fieldName,
				target
			});
			if (string.IsNullOrEmpty(target))
			{
				return target;
			}
			string text;
			string text2;
			bool flag;
			if (!UserNotificationEvent.TryParseTargetUser(target, out text, out text2, out flag))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "FormatTarget({0}): Invalid target {1}", new object[]
				{
					fieldName,
					target
				});
				throw new ArgumentException(fieldName);
			}
			if (UtilityMethods.IsAnonymousNumber(text))
			{
				target = string.Empty;
			}
			else if (flag)
			{
				target = text.Split(new char[]
				{
					';'
				})[0];
			}
			else
			{
				target = string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
				{
					text,
					text2
				});
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.OCSNotifEventsTracer, 0, "FormatTarget({0}): processed target {1}", new object[]
			{
				fieldName,
				target
			});
			return target;
		}

		private static string ValidateAndResolveTargetUser(string fromUri, OrganizationId organizationId, string notificationXml)
		{
			ValidateArgument.NotNull(organizationId, "OrganizationId");
			ValidateArgument.NotNullOrEmpty(fromUri, "From");
			ValidateArgument.NotNullOrEmpty(notificationXml, "NotificationXml");
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(notificationXml);
			XmlNode xmlNode = UserNotificationEvent.SelectNode(xmlDocument, "UserNotification");
			fromUri = UserNotificationEvent.FormatTarget(fromUri, "From");
			string text = UserNotificationEvent.GetNodeValue(xmlNode, "User");
			text = UserNotificationEvent.FormatTarget(text, "User");
			ValidateArgument.NotNullOrEmpty(text, "User");
			string nodeValue = UserNotificationEvent.GetNodeValue(xmlNode, "EumProxyAddress");
			ADRecipient adrecipient = null;
			if (!CommonConstants.UseDataCenterCallRouting)
			{
				ValidateArgument.NotNullOrEmpty(nodeValue, "EumProxyAddress");
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromOrganizationId(OrganizationId.ForestWideOrgId, null);
				adrecipient = iadrecipientLookup.LookupByUmAddress(nodeValue);
			}
			else
			{
				if (!string.Equals(text, fromUri, StringComparison.InvariantCultureIgnoreCase))
				{
					throw new ArgumentException("FROM uri does not match User node in xml payload", "From");
				}
				if (!string.IsNullOrEmpty(nodeValue))
				{
					CallIdTracer.TraceError(ExTraceGlobals.OCSNotifEventsTracer, 0, "EumProxyAddress cannot be set for hosted exchange.", new object[0]);
					throw new ArgumentException("EumProxyAddress");
				}
				IADRecipientLookup iadrecipientLookup2 = ADRecipientLookupFactory.CreateFromOrganizationId(organizationId, null);
				adrecipient = iadrecipientLookup2.LookupByEumSipResourceIdentifierPrefix(text);
			}
			Guid guid;
			using (UMSubscriber umsubscriber = UMRecipient.Factory.FromADRecipient<UMSubscriber>(adrecipient))
			{
				if (umsubscriber == null)
				{
					throw new UserNotUmEnabledException(text);
				}
				guid = umsubscriber.TenantGuid;
			}
			UserNotificationEvent.SetNodeValue(xmlNode, "RecipientObjectGuid", adrecipient.Guid.ToString());
			UserNotificationEvent.SetNodeValue(xmlNode, "TenantGuid", guid.ToString());
			return xmlDocument.InnerXml;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "UserNotificationEvent.Dispose() called", new object[0]);
				if (this.subscriber != null)
				{
					this.subscriber.Dispose();
					this.subscriber = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UserNotificationEvent>(this);
		}

		protected string FindTarget(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return target;
			}
			target = UserNotificationEvent.FormatTarget(target, "Target");
			ContactInfo contactInfo = null;
			PhoneNumber phoneNumber = null;
			if (PhoneNumber.TryParse(target, out phoneNumber))
			{
				contactInfo = ContactInfo.FindContactByCallerId(this.Subscriber, phoneNumber);
			}
			if (contactInfo != null && !string.IsNullOrEmpty(contactInfo.DisplayName))
			{
				return contactInfo.DisplayName;
			}
			return target;
		}

		protected abstract void InternalRenderMessage(MessageContentBuilder content, MessageItem message, ContactInfo callerInfo);

		private static Dictionary<string, Importance> CreateImportanceMap()
		{
			Dictionary<string, Importance> dictionary = new Dictionary<string, Importance>(StringComparer.InvariantCultureIgnoreCase);
			dictionary["normal"] = Importance.Normal;
			dictionary["urgent"] = Importance.High;
			dictionary["emergency"] = Importance.High;
			dictionary["non-urgent"] = Importance.Low;
			return dictionary;
		}

		private readonly Guid tenantGuid;

		internal static TryParseTargetUserDelegate TryParseTargetUser = new TryParseTargetUserDelegate(UserNotificationEvent.DefaultTryParseTargetUser);

		private static Dictionary<string, Importance> importanceMap = UserNotificationEvent.CreateImportanceMap();

		private string callId;

		private string subject;

		private PhoneNumber callerId;

		private UMSubscriber subscriber;

		private ExDateTime time;

		private Importance importance = Importance.Normal;

		protected enum UserNotificationEventType
		{
			Missed,
			Answered,
			Forbidden
		}

		protected enum RoutingGroupClass
		{
			Primary,
			Secondary
		}
	}
}

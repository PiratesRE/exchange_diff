using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class PropertyCommand
	{
		internal PropertyCommand(CommandContext commandContext)
		{
			this.commandContext = commandContext;
			this.xmlLocalName = commandContext.PropertyInformation.LocalName;
			this.xmlNamespaceUri = commandContext.PropertyInformation.NamespaceUri;
		}

		public static bool InMemoryProcessOnly
		{
			get
			{
				return PropertyCommand.inMemoryProcessOnly;
			}
		}

		public virtual bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return false;
			}
		}

		public static void ToServiceObjectInMemoryOnly(Action process)
		{
			try
			{
				PropertyCommand.inMemoryProcessOnly = true;
				process();
			}
			finally
			{
				PropertyCommand.inMemoryProcessOnly = false;
			}
		}

		public static object GetPropertyValueFromStoreObject(StoreObject storeObject, PropertyDefinition propertyDefinition)
		{
			object obj = storeObject.TryGetProperty(propertyDefinition);
			PropertyError propertyError = obj as PropertyError;
			if (propertyError == null)
			{
				return obj;
			}
			StorePropertyDefinition storePropertyDefinition = (StorePropertyDefinition)propertyDefinition;
			if ((propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory || propertyError.PropertyErrorCode == PropertyErrorCode.RequireStreamed) && storePropertyDefinition.SpecifiedWith != PropertyTypeSpecifier.Calculated)
			{
				try
				{
					using (Stream stream = storeObject.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
					{
						byte[] array = new byte[stream.Length];
						stream.Read(array, 0, array.Length);
						return PropertyCommand.DeserializePropertyStream(propertyDefinition.Type, array);
					}
				}
				catch (NotSupportedException)
				{
					throw new DataSizeLimitException(SearchSchemaMap.GetPropertyPath(propertyDefinition));
				}
				catch (InvalidOperationException)
				{
					throw new DataSizeLimitException(SearchSchemaMap.GetPropertyPath(propertyDefinition));
				}
			}
			throw PropertyError.ToException(new PropertyError[]
			{
				propertyError
			});
		}

		public void Update()
		{
			UpdateCommandSettings commandSettings = this.GetCommandSettings<UpdateCommandSettings>();
			SetPropertyUpdate setPropertyUpdate = null;
			DeletePropertyUpdate deletePropertyUpdate = null;
			AppendPropertyUpdate appendPropertyUpdate = null;
			if (UpdatePropertyList.TryGetPropertyUpdate<AppendPropertyUpdate>(commandSettings.PropertyUpdate, out appendPropertyUpdate))
			{
				this.AppendUpdate(appendPropertyUpdate, commandSettings);
				return;
			}
			if (UpdatePropertyList.TryGetPropertyUpdate<SetPropertyUpdate>(commandSettings.PropertyUpdate, out setPropertyUpdate))
			{
				this.SetUpdate(setPropertyUpdate, commandSettings);
				return;
			}
			if (UpdatePropertyList.TryGetPropertyUpdate<DeletePropertyUpdate>(commandSettings.PropertyUpdate, out deletePropertyUpdate))
			{
				this.DeleteUpdate(deletePropertyUpdate, commandSettings);
			}
		}

		public virtual void AppendUpdate(AppendPropertyUpdate appendPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
		}

		public virtual void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
		}

		public virtual void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
		}

		public virtual void SetPhase2()
		{
		}

		public virtual void SetPhase3()
		{
		}

		public virtual void PostUpdate()
		{
		}

		public void SetPropertyValueOnStoreObject(StoreObject storeObject, PropertyDefinition propertyDefinition, object value)
		{
			try
			{
				this.SetStoreObjectProperty(storeObject, propertyDefinition, value);
			}
			catch (PropertyErrorException ex)
			{
				StorePropertyDefinition storePropertyDefinition = (StorePropertyDefinition)propertyDefinition;
				if (ex.PropertyErrors.Length == 1 && storePropertyDefinition.SpecifiedWith != PropertyTypeSpecifier.Calculated && (ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory || ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.RequireStreamed))
				{
					byte[] array = PropertyCommand.SerializePropertyStream(value);
					try
					{
						using (Stream stream = storeObject.OpenPropertyStream(propertyDefinition, PropertyOpenMode.Modify))
						{
							stream.Write(array, 0, array.Length);
						}
						goto IL_90;
					}
					catch (NotSupportedException)
					{
						throw new DataSizeLimitException(SearchSchemaMap.GetPropertyPath(propertyDefinition));
					}
					catch (InvalidOperationException)
					{
						throw new DataSizeLimitException(SearchSchemaMap.GetPropertyPath(propertyDefinition));
					}
					goto IL_8E;
					IL_90:
					return;
				}
				IL_8E:
				throw;
			}
		}

		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}", string.IsNullOrEmpty(this.xmlLocalName) ? "<NULL>" : this.xmlLocalName, base.GetType().FullName);
		}

		internal static bool TryGetValueFromPropertyBag<T>(IDictionary<PropertyDefinition, object> propertyBag, PropertyDefinition key, out T value)
		{
			object obj = null;
			value = default(T);
			if (propertyBag.TryGetValue(key, out obj) && obj is T)
			{
				value = (T)((object)obj);
				return true;
			}
			return false;
		}

		protected static bool StorePropertyExists(StoreObject storeObject, PropertyDefinition propertyDefinition)
		{
			PropertyError propertyError = storeObject.TryGetProperty(propertyDefinition) as PropertyError;
			if (propertyError != null)
			{
				PropertyErrorCode propertyErrorCode = propertyError.PropertyErrorCode;
				switch (propertyErrorCode)
				{
				case PropertyErrorCode.NotFound:
				case PropertyErrorCode.IncorrectValueType:
				case PropertyErrorCode.GetCalculatedPropertyError:
					return false;
				case PropertyErrorCode.NotEnoughMemory:
					break;
				case PropertyErrorCode.NullValue:
				case PropertyErrorCode.SetCalculatedPropertyError:
				case PropertyErrorCode.SetStoreComputedPropertyError:
					goto IL_4C;
				default:
					if (propertyErrorCode != PropertyErrorCode.RequireStreamed)
					{
						goto IL_4C;
					}
					break;
				}
				return true;
				IL_4C:
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<PropertyErrorCode, string>(0L, "Store error inside StorePropertyExists(), error code = {0} description = {1}", propertyError.PropertyErrorCode, propertyError.PropertyErrorDescription);
				throw new PropertyRequestFailedException((storeObject is Folder) ? ((CoreResources.IDs)2370747299U) : CoreResources.IDs.ErrorItemPropertyRequestFailed, SearchSchemaMap.GetPropertyPath(propertyDefinition));
			}
			return true;
		}

		protected static void PreventSentMessageUpdate(CommandContext commandContext)
		{
			UpdateCommandSettings updateCommandSettings = commandContext.CommandSettings as UpdateCommandSettings;
			if (updateCommandSettings != null)
			{
				MessageItem messageItem = updateCommandSettings.StoreObject as MessageItem;
				if (messageItem != null && !messageItem.IsDraft)
				{
					throw new InvalidPropertyUpdateSentMessageException(commandContext.PropertyInformation.PropertyPath);
				}
			}
		}

		protected T GetCommandSettings<T>() where T : CommandSettings
		{
			return this.commandContext.CommandSettings as T;
		}

		internal static SingleRecipientType CreateRecipientFromParticipant(ParticipantInformation participantInformation)
		{
			SingleRecipientType singleRecipientType = new SingleRecipientType();
			singleRecipientType.Mailbox = new EmailAddressWrapper();
			singleRecipientType.Mailbox.Name = participantInformation.DisplayName;
			singleRecipientType.Mailbox.EmailAddress = participantInformation.EmailAddress;
			singleRecipientType.Mailbox.RoutingType = participantInformation.RoutingType;
			singleRecipientType.Mailbox.SipUri = participantInformation.SipUri;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				singleRecipientType.Mailbox.MailboxType = participantInformation.MailboxType.ToString();
			}
			return singleRecipientType;
		}

		internal static SingleRecipientType CreateOneOffRecipientFromParticipant(IParticipant participant)
		{
			return new SingleRecipientType
			{
				Mailbox = new EmailAddressWrapper(),
				Mailbox = 
				{
					Name = participant.DisplayName,
					EmailAddress = participant.EmailAddress,
					RoutingType = participant.RoutingType,
					SipUri = participant.SipUri,
					MailboxType = MailboxHelper.MailboxTypeType.OneOff.ToString()
				}
			};
		}

		internal static SingleRecipientType CreateOneOffRecipientFromParticipantSmtpAddress(IParticipant participant)
		{
			return new SingleRecipientType
			{
				Mailbox = new EmailAddressWrapper(),
				Mailbox = 
				{
					Name = participant.DisplayName,
					EmailAddress = participant.SmtpEmailAddress,
					RoutingType = "SMTP",
					SipUri = participant.SipUri,
					MailboxType = MailboxHelper.MailboxTypeType.OneOff.ToString()
				}
			};
		}

		protected SingleRecipientType CreateRecipientFromParticipant(ParticipantInformation participantInformation, StoreObject storeObject)
		{
			SingleRecipientType singleRecipientType = PropertyCommand.CreateRecipientFromParticipant(participantInformation);
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010) || participantInformation.RoutingType == "MAPIPDL")
			{
				StoreParticipantOrigin storeParticipantOrigin = participantInformation.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null && storeParticipantOrigin.OriginItemId != null)
				{
					MailboxSession mailboxSession = storeObject.Session as MailboxSession;
					if (mailboxSession != null && !IdConverter.IsFromPublicStore(storeParticipantOrigin.OriginItemId))
					{
						MailboxId mailboxId = new MailboxId(mailboxSession);
						ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeParticipantOrigin.OriginItemId, mailboxId, null);
						singleRecipientType.Mailbox.ItemId = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
					}
				}
			}
			return singleRecipientType;
		}

		protected ItemId GetParticipantItemId(ParticipantInformation participantInformation, StoreObject storeObject)
		{
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010) || participantInformation.RoutingType == "MAPIPDL")
			{
				StoreParticipantOrigin storeParticipantOrigin = participantInformation.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null && storeParticipantOrigin.OriginItemId != null)
				{
					MailboxSession mailboxSession = storeObject.Session as MailboxSession;
					if (mailboxSession != null && !IdConverter.IsFromPublicStore(storeParticipantOrigin.OriginItemId))
					{
						MailboxId mailboxId = new MailboxId(mailboxSession);
						ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeParticipantOrigin.OriginItemId, mailboxId, null);
						return new ItemId
						{
							Id = concatenatedId.Id,
							ChangeKey = concatenatedId.ChangeKey
						};
					}
				}
			}
			return null;
		}

		protected virtual Participant GetParticipantFromAddress(Item item, EmailAddressWrapper address)
		{
			return MailboxHelper.GetParticipantFromAddress(address);
		}

		protected Participant GetParticipantOrDLFromAddress(EmailAddressWrapper address, StoreObject storeObject)
		{
			Participant result = null;
			if (address.ItemId == null)
			{
				result = MailboxHelper.GetParticipantFromAddress(address);
			}
			else
			{
				if (!(storeObject.Session is MailboxSession))
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3795663900U);
				}
				StoreObjectType storeObjectType = StoreObjectType.Unknown;
				IdAndSession idAndSession = null;
				try
				{
					idAndSession = this.commandContext.IdConverter.ConvertItemIdToIdAndSessionReadOnly(address.ItemId);
					storeObjectType = IdConverter.GetAsStoreObjectId(idAndSession.Id).ObjectType;
				}
				catch (ObjectNotFoundException)
				{
					if (address.MailboxType == MailboxHelper.MailboxTypeType.PrivateDL.ToString())
					{
						throw;
					}
					storeObjectType = StoreObjectType.Contact;
				}
				switch (storeObjectType)
				{
				case StoreObjectType.Contact:
					break;
				case StoreObjectType.DistributionList:
					using (DistributionList distributionList = (DistributionList)ServiceCommandBase.GetXsoItem(storeObject.Session, idAndSession.Id, new PropertyDefinition[0]))
					{
						return distributionList.GetAsParticipant();
					}
					break;
				default:
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, GetParticipantOrDLFromAddressMetadata.ObjectType, storeObjectType);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, GetParticipantOrDLFromAddressMetadata.EmailAddress, address.EmailAddress);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, GetParticipantOrDLFromAddressMetadata.Name, address.Name);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, GetParticipantOrDLFromAddressMetadata.OriginalDisplayName, address.OriginalDisplayName);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, GetParticipantOrDLFromAddressMetadata.MailboxType, address.MailboxType);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, GetParticipantOrDLFromAddressMetadata.ItemId, address.ItemId);
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotUsePersonalContactsAsRecipientsOrAttendees);
				}
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2013))
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotUsePersonalContactsAsRecipientsOrAttendees);
				}
				result = MailboxHelper.GetParticipantFromAddress(address);
			}
			return result;
		}

		protected virtual void SetStoreObjectProperty(StoreObject storeObject, PropertyDefinition propertyDefinition, object value)
		{
			storeObject[propertyDefinition] = value;
		}

		protected void ValidateDataSize(long dataSize)
		{
			if (dataSize > 2147483647L)
			{
				throw new DataSizeLimitException(this.commandContext.PropertyInformation.PropertyPath);
			}
		}

		protected void DeleteProperties(StoreObject storeObject, PropertyPath propertyPath, params PropertyDefinition[] propertyDefinitions)
		{
			try
			{
				storeObject.DeleteProperties(propertyDefinitions);
			}
			catch (NotSupportedException innerException)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "NotSupportedException for PropertyPath: " + propertyPath);
				throw new InvalidPropertyDeleteException(propertyPath, innerException);
			}
		}

		private static byte[] SerializePropertyStream(object valueToSerialize)
		{
			string text = valueToSerialize as string;
			string s = text;
			if (text != null)
			{
				if (CallContext.Current.EncodeStringProperties && ExchangeVersion.Current.Supports(ExchangeVersionType.Exchange2012))
				{
					s = Util.EncodeForAntiXSS(text);
				}
				return Encoding.Unicode.GetBytes(s);
			}
			byte[] array = valueToSerialize as byte[];
			if (array != null)
			{
				return array;
			}
			return null;
		}

		private static object DeserializePropertyStream(Type type, byte[] bytes)
		{
			if (type == typeof(string))
			{
				return Encoding.Unicode.GetString(bytes, 0, bytes.Length);
			}
			if (type == typeof(byte[]))
			{
				return bytes;
			}
			return null;
		}

		protected static void CreateXmlAttribute(XmlElement parentElement, string attributeName, string attributeValue)
		{
			ServiceXml.CreateAttribute(parentElement, attributeName, attributeValue);
		}

		protected XmlElement CreateXmlElement(XmlElement parentElement, string localName)
		{
			return ServiceXml.CreateElement(parentElement, localName, this.xmlNamespaceUri);
		}

		protected XmlElement CreateXmlTextElement(XmlElement parentElement, string localName, string textValue)
		{
			return ServiceXml.CreateTextElement(parentElement, localName, textValue, this.xmlNamespaceUri);
		}

		protected XmlElement CreateXmlTextElement(XmlElement parentElement, string localName, XmlText textNode)
		{
			return ServiceXml.CreateTextElement(parentElement, localName, textNode, this.xmlNamespaceUri);
		}

		protected XmlElement CreateXmlTextElementOptionally(XmlElement parentElement, string localName, string textValue)
		{
			if (string.IsNullOrEmpty(textValue))
			{
				return null;
			}
			return ServiceXml.CreateTextElement(parentElement, localName, textValue, this.xmlNamespaceUri);
		}

		protected XmlElement CreateParticipantXml(XmlElement parentElement, ParticipantInformation participantInformation)
		{
			XmlElement xmlElement = this.CreateXmlElement(parentElement, "Mailbox");
			this.CreateXmlTextElement(xmlElement, "Name", participantInformation.DisplayName);
			this.CreateXmlTextElementOptionally(xmlElement, "EmailAddress", participantInformation.EmailAddress);
			this.CreateXmlTextElementOptionally(xmlElement, "RoutingType", participantInformation.RoutingType);
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				MailboxHelper.MailboxTypeType mailboxType = participantInformation.MailboxType;
				this.CreateXmlTextElementOptionally(xmlElement, "MailboxType", mailboxType.ToString());
			}
			return xmlElement;
		}

		protected void CreateParticipantOrDLXml(XmlElement parentElement, ParticipantInformation participantInformation, StoreObject storeObject)
		{
			if (participantInformation == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "Participant is null. storeObject.ClassName={0};", storeObject.ClassName);
				return;
			}
			XmlElement idParentElement = this.CreateParticipantXml(parentElement, participantInformation);
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010) || participantInformation.RoutingType == "MAPIPDL")
			{
				StoreParticipantOrigin storeParticipantOrigin = participantInformation.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null && storeParticipantOrigin.OriginItemId != null)
				{
					MailboxSession mailboxSession = storeObject.Session as MailboxSession;
					if (mailboxSession != null && !IdConverter.IsFromPublicStore(storeParticipantOrigin.OriginItemId))
					{
						MailboxId mailboxId = new MailboxId(mailboxSession);
						IdConverter.CreateStoreIdXml(idParentElement, storeParticipantOrigin.OriginItemId, mailboxId, "ItemId");
					}
				}
			}
		}

		protected Participant GetParticipantFromXml(XmlElement parentElement)
		{
			XmlElement xmlElement = parentElement["Name", "http://schemas.microsoft.com/exchange/services/2006/types"];
			string displayName;
			if (xmlElement == null)
			{
				displayName = string.Empty;
			}
			else
			{
				displayName = ServiceXml.GetXmlTextNodeValue(xmlElement);
			}
			XmlElement xmlElement2 = parentElement["EmailAddress", "http://schemas.microsoft.com/exchange/services/2006/types"];
			if (xmlElement2 == null)
			{
				throw new MissingInformationEmailAddressException();
			}
			string xmlTextNodeValue = ServiceXml.GetXmlTextNodeValue(xmlElement2);
			XmlElement xmlElement3 = parentElement["RoutingType", "http://schemas.microsoft.com/exchange/services/2006/types"];
			string text;
			if (xmlElement3 == null)
			{
				text = string.Empty;
			}
			else
			{
				text = ServiceXml.GetXmlTextNodeValue(xmlElement3);
				try
				{
					new CustomProxyAddressPrefix(text);
				}
				catch (ArgumentException ex)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>((long)this.GetHashCode(), "Invalid routing type: '{0}'.  ArgumentException encountered: {1}", text, ex.Message);
					throw new MalformedRoutingTypeException(this.commandContext.PropertyInformation.PropertyPath, ex);
				}
			}
			Participant participant = new Participant(displayName, xmlTextNodeValue, text);
			Participant participant2 = MailboxHelper.TryConvertTo(participant, "EX");
			if (null != participant2)
			{
				return participant2;
			}
			return participant;
		}

		protected Participant GetParticipantOrDLFromXml(XmlElement parentElement, StoreObject storeObject)
		{
			XmlElement xmlElement = parentElement["ItemId", "http://schemas.microsoft.com/exchange/services/2006/types"];
			Participant result = null;
			if (xmlElement == null)
			{
				result = this.GetParticipantFromXml(parentElement);
			}
			else
			{
				if (!(storeObject.Session is MailboxSession))
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3795663900U);
				}
				StoreId id = IdConverter.ConvertXmlToStoreId(xmlElement, (MailboxSession)storeObject.Session, BasicTypes.Item);
				switch (IdConverter.GetAsStoreObjectId(id).ObjectType)
				{
				case StoreObjectType.Contact:
					break;
				case StoreObjectType.DistributionList:
					using (DistributionList distributionList = (DistributionList)ServiceCommandBase.GetXsoItem(storeObject.Session, id, new PropertyDefinition[0]))
					{
						return distributionList.GetAsParticipant();
					}
					break;
				default:
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotUsePersonalContactsAsRecipientsOrAttendees);
				}
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2013))
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotUsePersonalContactsAsRecipientsOrAttendees);
				}
				result = this.GetParticipantFromXml(parentElement);
			}
			return result;
		}

		private const int StreamReadSize = 1024;

		[ThreadStatic]
		private static bool inMemoryProcessOnly;

		protected string xmlLocalName;

		protected string xmlNamespaceUri;

		protected CommandContext commandContext;

		public delegate IPropertyCommand CreatePropertyCommand(CommandContext commandContext);
	}
}

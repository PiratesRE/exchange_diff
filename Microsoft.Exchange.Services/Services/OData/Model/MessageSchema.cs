using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class MessageSchema : ItemSchema
	{
		public new static MessageSchema SchemaInstance
		{
			get
			{
				return MessageSchema.MessageSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return Message.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return MessageSchema.DeclaredMessageProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return MessageSchema.AllMessageProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return MessageSchema.DefaultMessageProperties;
			}
		}

		public override void RegisterEdmModel(EdmModel model)
		{
			base.RegisterEdmModel(model);
			CustomActions.RegisterAction(model, Message.EdmEntityType, Message.EdmEntityType, "Copy", new Dictionary<string, IEdmTypeReference>
			{
				{
					"DestinationId",
					EdmCoreModel.Instance.GetString(true)
				}
			});
			CustomActions.RegisterAction(model, Message.EdmEntityType, Message.EdmEntityType, "Move", new Dictionary<string, IEdmTypeReference>
			{
				{
					"DestinationId",
					EdmCoreModel.Instance.GetString(true)
				}
			});
			CustomActions.RegisterAction(model, Message.EdmEntityType, Message.EdmEntityType, "CreateReply", null);
			CustomActions.RegisterAction(model, Message.EdmEntityType, Message.EdmEntityType, "CreateReplyAll", null);
			CustomActions.RegisterAction(model, Message.EdmEntityType, Message.EdmEntityType, "CreateForward", null);
			CustomActions.RegisterAction(model, Message.EdmEntityType, null, "Reply", new Dictionary<string, IEdmTypeReference>
			{
				{
					"Comment",
					EdmCoreModel.Instance.GetString(true)
				}
			});
			CustomActions.RegisterAction(model, Message.EdmEntityType, null, "ReplyAll", new Dictionary<string, IEdmTypeReference>
			{
				{
					"Comment",
					EdmCoreModel.Instance.GetString(true)
				}
			});
			CustomActions.RegisterAction(model, Message.EdmEntityType, null, "Forward", new Dictionary<string, IEdmTypeReference>
			{
				{
					"Comment",
					EdmCoreModel.Instance.GetString(true)
				},
				{
					"ToRecipients",
					new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(Recipient.EdmComplexType.Member, true)))
				}
			});
			CustomActions.RegisterAction(model, Message.EdmEntityType, null, "Send", null);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MessageSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("ParentFolderId", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider = new SimpleEwsPropertyProvider(ItemSchema.ParentFolderId);
			simpleEwsPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = EwsIdConverter.EwsIdToODataId((s[sp] as FolderId).Id);
			};
			propertyDefinition2.EwsPropertyProvider = simpleEwsPropertyProvider;
			MessageSchema.ParentFolderId = propertyDefinition;
			PropertyDefinition propertyDefinition3 = new PropertyDefinition("Sender", typeof(Recipient));
			propertyDefinition3.EdmType = new EdmComplexTypeReference(Recipient.EdmComplexType.Member, true);
			propertyDefinition3.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate);
			PropertyDefinition propertyDefinition4 = propertyDefinition3;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider2 = new SimpleEwsPropertyProvider(MessageSchema.Sender);
			simpleEwsPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = s.GetValueOrDefault<SingleRecipientType>(sp).ToRecipient();
			};
			simpleEwsPropertyProvider2.Setter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				s[sp] = (e[ep] as Recipient).ToSingleRecipientType();
			};
			propertyDefinition4.EwsPropertyProvider = simpleEwsPropertyProvider2;
			propertyDefinition3.ODataPropertyValueConverter = new RecipientODataConverter();
			MessageSchema.Sender = propertyDefinition3;
			PropertyDefinition propertyDefinition5 = new PropertyDefinition("From", typeof(Recipient));
			propertyDefinition5.EdmType = new EdmComplexTypeReference(Recipient.EdmComplexType.Member, true);
			propertyDefinition5.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition6 = propertyDefinition5;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider3 = new SimpleEwsPropertyProvider(MessageSchema.From);
			simpleEwsPropertyProvider3.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = s.GetValueOrDefault<SingleRecipientType>(sp).ToRecipient();
			};
			simpleEwsPropertyProvider3.Setter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				s[sp] = (e[ep] as Recipient).ToSingleRecipientType();
			};
			propertyDefinition6.EwsPropertyProvider = simpleEwsPropertyProvider3;
			propertyDefinition5.ODataPropertyValueConverter = new RecipientODataConverter();
			MessageSchema.From = propertyDefinition5;
			MessageSchema.ToRecipients = new PropertyDefinition("ToRecipients", typeof(Recipient[]))
			{
				EdmType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(Recipient.EdmComplexType.Member, true))),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new RecipientsPropertyProvider(MessageSchema.ToRecipients),
				ODataPropertyValueConverter = new RecipientsODataConverter()
			};
			MessageSchema.CcRecipients = new PropertyDefinition("CcRecipients", typeof(Recipient[]))
			{
				EdmType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(Recipient.EdmComplexType.Member, true))),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new RecipientsPropertyProvider(MessageSchema.CcRecipients),
				ODataPropertyValueConverter = new RecipientsODataConverter()
			};
			MessageSchema.BccRecipients = new PropertyDefinition("BccRecipients", typeof(Recipient[]))
			{
				EdmType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(Recipient.EdmComplexType.Member, true))),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new RecipientsPropertyProvider(MessageSchema.BccRecipients),
				ODataPropertyValueConverter = new RecipientsODataConverter()
			};
			MessageSchema.ReplyTo = new PropertyDefinition("ReplyTo", typeof(Recipient[]))
			{
				EdmType = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(Recipient.EdmComplexType.Member, true))),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new RecipientsPropertyProvider(MessageSchema.ReplyTo),
				ODataPropertyValueConverter = new RecipientsODataConverter()
			};
			PropertyDefinition propertyDefinition7 = new PropertyDefinition("ConversationId", typeof(string));
			propertyDefinition7.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition7.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition8 = propertyDefinition7;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider4 = new SimpleEwsPropertyProvider(ItemSchema.ConversationId);
			simpleEwsPropertyProvider4.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				string value = null;
				ItemId valueOrDefault = s.GetValueOrDefault<ItemId>(ItemSchema.ConversationId);
				if (valueOrDefault != null)
				{
					value = EwsIdConverter.EwsIdToODataId(valueOrDefault.Id);
				}
				e[ep] = value;
			};
			simpleEwsPropertyProvider4.QueryConstantBuilder = ((object o) => EwsIdConverter.ODataIdToEwsId(o as string));
			propertyDefinition8.EwsPropertyProvider = simpleEwsPropertyProvider4;
			MessageSchema.ConversationId = propertyDefinition7;
			MessageSchema.UniqueBody = new PropertyDefinition("UniqueBody", typeof(ItemBody))
			{
				EdmType = new EdmComplexTypeReference(ItemBody.EdmComplexType.Member, true),
				EwsPropertyProvider = new BodyPropertyProvider(ItemSchema.UniqueBody),
				ODataPropertyValueConverter = new ItemBodyODataConverter()
			};
			MessageSchema.IsDeliveryReceiptRequested = new PropertyDefinition("IsDeliveryReceiptRequested", typeof(bool))
			{
				EdmType = EdmCoreModel.Instance.GetBoolean(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(MessageSchema.IsDeliveryReceiptRequested)
			};
			MessageSchema.IsReadReceiptRequested = new PropertyDefinition("IsReadReceiptRequested", typeof(bool))
			{
				EdmType = EdmCoreModel.Instance.GetBoolean(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(MessageSchema.IsReadReceiptRequested)
			};
			MessageSchema.IsRead = new PropertyDefinition("IsRead", typeof(bool))
			{
				EdmType = EdmCoreModel.Instance.GetBoolean(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(MessageSchema.IsRead)
			};
			MessageSchema.IsDraft = new PropertyDefinition("IsDraft", typeof(bool))
			{
				EdmType = EdmCoreModel.Instance.GetBoolean(true),
				Flags = PropertyDefinitionFlags.CanFilter,
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ItemSchema.IsDraft)
			};
			MessageSchema.DateTimeReceived = new PropertyDefinition("DateTimeReceived", typeof(DateTimeOffset))
			{
				EdmType = EdmCoreModel.Instance.GetDateTimeOffset(true),
				Flags = PropertyDefinitionFlags.CanFilter,
				EwsPropertyProvider = new DateTimePropertyProvider(ItemSchema.DateTimeReceived)
			};
			MessageSchema.DateTimeSent = new PropertyDefinition("DateTimeSent", typeof(DateTimeOffset))
			{
				EdmType = EdmCoreModel.Instance.GetDateTimeOffset(true),
				Flags = PropertyDefinitionFlags.CanFilter,
				EwsPropertyProvider = new DateTimePropertyProvider(ItemSchema.DateTimeSent)
			};
			PropertyDefinition propertyDefinition9 = new PropertyDefinition("EventId", typeof(string));
			propertyDefinition9.EdmType = EdmCoreModel.Instance.GetString(true);
			PropertyDefinition propertyDefinition10 = propertyDefinition9;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider5 = new SimpleEwsPropertyProvider(MeetingMessageSchema.AssociatedCalendarItemId);
			simpleEwsPropertyProvider5.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				string value = null;
				ItemId valueOrDefault = s.GetValueOrDefault<ItemId>(MeetingMessageSchema.AssociatedCalendarItemId);
				if (valueOrDefault != null)
				{
					value = EwsIdConverter.EwsIdToODataId(valueOrDefault.Id);
				}
				e[ep] = value;
			};
			propertyDefinition10.EwsPropertyProvider = simpleEwsPropertyProvider5;
			MessageSchema.EventId = propertyDefinition9;
			MessageSchema.MeetingMessageType = new PropertyDefinition("MeetingMessageType", typeof(MeetingMessageType))
			{
				EdmType = new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(MeetingMessageType)), true),
				Flags = PropertyDefinitionFlags.CanFilter,
				EwsPropertyProvider = new MeetingMessageTypePropertyProvider(ItemSchema.ItemClass)
			};
			MessageSchema.DeclaredMessageProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				MessageSchema.ParentFolderId,
				MessageSchema.From,
				MessageSchema.Sender,
				MessageSchema.ToRecipients,
				MessageSchema.CcRecipients,
				MessageSchema.BccRecipients,
				MessageSchema.ReplyTo,
				MessageSchema.ConversationId,
				MessageSchema.UniqueBody,
				MessageSchema.DateTimeReceived,
				MessageSchema.DateTimeSent,
				MessageSchema.IsDeliveryReceiptRequested,
				MessageSchema.IsReadReceiptRequested,
				MessageSchema.IsDraft,
				MessageSchema.IsRead,
				MessageSchema.EventId,
				MessageSchema.MeetingMessageType,
				ItemSchema.DateTimeCreated,
				ItemSchema.LastModifiedTime
			});
			MessageSchema.AllMessageProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(ItemSchema.AllItemProperties.Union(MessageSchema.DeclaredMessageProperties)));
			MessageSchema.DefaultMessageProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(ItemSchema.DefaultItemProperties)
			{
				MessageSchema.ParentFolderId,
				MessageSchema.From,
				MessageSchema.Sender,
				MessageSchema.ToRecipients,
				MessageSchema.CcRecipients,
				MessageSchema.BccRecipients,
				MessageSchema.ReplyTo,
				MessageSchema.ConversationId,
				MessageSchema.DateTimeReceived,
				MessageSchema.DateTimeSent,
				MessageSchema.IsDeliveryReceiptRequested,
				MessageSchema.IsReadReceiptRequested,
				MessageSchema.IsDraft,
				MessageSchema.IsRead,
				MessageSchema.EventId,
				MessageSchema.MeetingMessageType,
				ItemSchema.DateTimeCreated,
				ItemSchema.LastModifiedTime
			});
			MessageSchema.MessageSchemaInstance = new LazyMember<MessageSchema>(() => new MessageSchema());
		}

		public static readonly PropertyDefinition ParentFolderId;

		public static readonly PropertyDefinition Sender;

		public static readonly PropertyDefinition From;

		public static readonly PropertyDefinition ToRecipients;

		public static readonly PropertyDefinition CcRecipients;

		public static readonly PropertyDefinition BccRecipients;

		public static readonly PropertyDefinition ReplyTo;

		public static readonly PropertyDefinition ConversationId;

		public static readonly PropertyDefinition UniqueBody;

		public static readonly PropertyDefinition IsDeliveryReceiptRequested;

		public static readonly PropertyDefinition IsReadReceiptRequested;

		public static readonly PropertyDefinition IsRead;

		public static readonly PropertyDefinition IsDraft;

		public static readonly PropertyDefinition DateTimeReceived;

		public static readonly PropertyDefinition DateTimeSent;

		public static readonly PropertyDefinition EventId;

		public static readonly PropertyDefinition MeetingMessageType;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredMessageProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllMessageProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultMessageProperties;

		private static readonly LazyMember<MessageSchema> MessageSchemaInstance;
	}
}

using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class RetentionTagPropertyBase : PropertyCommand, IToXmlCommand, IToServiceObjectCommand, IToXmlForPropertyBagCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public RetentionTagPropertyBase(CommandContext commandContext, Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType retentionAction) : base(commandContext)
		{
			this.retentionAction = retentionAction;
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			bool flag = false;
			Guid? retentionTag = this.GetRetentionTag(storeObject, out flag);
			if (retentionTag != null)
			{
				serviceObject.PropertyBag[propertyInformation] = new RetentionTagType
				{
					IsExplicit = flag,
					Value = retentionTag.ToString()
				};
				ExTraceGlobals.ELCTracer.TraceDebug<bool, Guid?>((long)this.GetHashCode(), "[RetentionTagPropertyBase::ToServiceObject] IsExplicit = {0}, Value = {1}", flag, retentionTag);
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			bool flag = false;
			Guid? retentionTagFromPropertyBag = this.GetRetentionTagFromPropertyBag(commandSettings.PropertyBag, out flag);
			if (retentionTagFromPropertyBag != null)
			{
				serviceObject.PropertyBag[propertyInformation] = new RetentionTagType
				{
					IsExplicit = flag,
					Value = retentionTagFromPropertyBag.ToString()
				};
				ExTraceGlobals.ELCTracer.TraceDebug<bool, Guid?>((long)this.GetHashCode(), "[RetentionTagPropertyBase::ToServiceObjectForPropertyBag] IsExplicit = {0}, Value = {1}", flag, retentionTagFromPropertyBag);
			}
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			this.SetProperty(storeObject, serviceObject, false);
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			this.SetProperty(storeObject, serviceObject, true);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			this.DeleteRetentionTag(updateCommandSettings.StoreObject);
		}

		internal abstract Guid? GetRetentionTag(StoreObject storeObject, out bool isExplicit);

		internal abstract Guid? GetRetentionTagFromPropertyBag(IDictionary<PropertyDefinition, object> propertyBag, out bool isExplicit);

		internal abstract void SetRetentionTag(StoreObject storeObject, PolicyTag policyTag);

		internal abstract void NewRetentionTag(StoreObject storeObject, PolicyTag policyTag);

		internal abstract void DeleteRetentionTag(StoreObject storeObject);

		internal Item OpenItemForRetentionTag(StoreSession storeSession, StoreObjectId itemId, PropertyDefinition[] properties)
		{
			Item item = Item.Bind(storeSession, itemId, properties);
			item.OpenAsReadWrite();
			return item;
		}

		internal Folder OpenFolderForRetentionTag(StoreSession storeSession, StoreObjectId folderId, PropertyDefinition[] properties)
		{
			return Folder.Bind(storeSession, folderId, properties);
		}

		internal void SetProperty(StoreObject storeObject, ServiceObject serviceObject, bool isNewObject)
		{
			RetentionTagType valueOrDefault = serviceObject.GetValueOrDefault<RetentionTagType>(this.commandContext.PropertyInformation);
			MailboxSession mailboxSession = (MailboxSession)storeObject.Session;
			if (!valueOrDefault.IsExplicit)
			{
				ExTraceGlobals.ELCTracer.TraceError<string, StoreObjectId>((long)this.GetHashCode(), "[RetentionTagPropertyBase::SetProperty] Tag {0} is not allowed to have IsExplicit set to false for {1}", valueOrDefault.Value, storeObject.StoreObjectId);
				throw new InvalidRetentionTagException((CoreResources.IDs)3769371271U);
			}
			Guid guid;
			if (!Guid.TryParse(valueOrDefault.Value, out guid) || !(guid != Guid.Empty))
			{
				return;
			}
			if (mailboxSession == null)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorInvalidRetentionTagNone);
			}
			PolicyTagList policyTagList = mailboxSession.GetPolicyTagList(this.retentionAction);
			if (policyTagList == null)
			{
				throw new InvalidRetentionTagException(CoreResources.IDs.ErrorInvalidRetentionTagNone);
			}
			PolicyTag policyTag;
			if (!policyTagList.TryGetValue(guid, out policyTag))
			{
				ExTraceGlobals.ELCTracer.TraceError<Guid, StoreObjectId>((long)this.GetHashCode(), "[RetentionTagPropertyBase::SetProperty] Tag {0} has incorrect intended action type for {1}", guid, storeObject.StoreObjectId);
				throw new InvalidRetentionTagException(CoreResources.IDs.ErrorInvalidRetentionTagTypeMismatch);
			}
			if (!policyTag.IsVisible)
			{
				ExTraceGlobals.ELCTracer.TraceError<Guid, StoreObjectId>((long)this.GetHashCode(), "[RetentionTagPropertyBase::SetProperty] Tag {0} is an invisible tag for {1}", guid, storeObject.StoreObjectId);
				throw new InvalidRetentionTagException((CoreResources.IDs)4105318492U);
			}
			if (isNewObject)
			{
				this.NewRetentionTag(storeObject, policyTag);
				return;
			}
			this.SetRetentionTag(storeObject, policyTag);
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceItem = commandSettings.ServiceItem;
			bool flag = false;
			Guid? retentionTag = this.GetRetentionTag(storeObject, out flag);
			if (retentionTag != null)
			{
				XmlElement xmlElement = base.CreateXmlTextElement(serviceItem, this.xmlLocalName, retentionTag.ToString());
				PropertyCommand.CreateXmlAttribute(xmlElement, "IsExplicit", flag.ToString());
				ExTraceGlobals.ELCTracer.TraceDebug<XmlElement>((long)this.GetHashCode(), "[RetentionTagPropertyBase::ToXml] {0}", xmlElement);
			}
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			bool flag = false;
			Guid? retentionTagFromPropertyBag = this.GetRetentionTagFromPropertyBag(commandSettings.PropertyBag, out flag);
			if (retentionTagFromPropertyBag != null)
			{
				XmlElement xmlElement = base.CreateXmlTextElement(commandSettings.ServiceItem, this.xmlLocalName, retentionTagFromPropertyBag.ToString());
				PropertyCommand.CreateXmlAttribute(xmlElement, "IsExplicit", flag.ToString());
				ExTraceGlobals.ELCTracer.TraceDebug<XmlElement>((long)this.GetHashCode(), "[RetentionTagPropertyBase::ToXmlForPropertyBag] {0}", xmlElement);
			}
		}

		private Microsoft.Exchange.Data.Directory.SystemConfiguration.RetentionActionType retentionAction;
	}
}

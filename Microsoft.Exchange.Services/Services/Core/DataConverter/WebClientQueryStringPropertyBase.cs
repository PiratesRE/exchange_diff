using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class WebClientQueryStringPropertyBase : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectForPropertyBagCommand, IToServiceObjectCommand, IPropertyCommand
	{
		public WebClientQueryStringPropertyBase(CommandContext commandContext) : base(commandContext)
		{
			this.itemIdProperty = this.propertyDefinitions[0];
			this.itemClassProperty = this.propertyDefinitions[1];
			this.isAssociatedProperty = this.propertyDefinitions[2];
		}

		public override bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			string className;
			if (this.ShouldCreateForServiceObjectOrXML(commandSettings.StoreObject, out className))
			{
				string itemId = this.GetItemId(commandSettings.StoreObject.Id, commandSettings.IdAndSession);
				commandSettings.ServiceObject.PropertyBag[this.commandContext.PropertyInformation] = this.CreateQueryString(className, itemId);
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			string className;
			StoreId storeId;
			if (this.ShouldCreateForPropertyBag(commandSettings.PropertyBag, out className, out storeId))
			{
				string itemId = this.GetItemId(storeId, commandSettings.IdAndSession);
				commandSettings.ServiceObject.PropertyBag[this.commandContext.PropertyInformation] = this.CreateQueryString(className, itemId);
			}
		}

		private string GetItemId(StoreId storeId, IdAndSession idAndSession)
		{
			return IdConverter.GetConcatenatedId(storeId, idAndSession, null).Id;
		}

		private bool ShouldCreateForServiceObjectOrXML(StoreObject storeObject, out string className)
		{
			className = null;
			bool flag = (bool)PropertyCommand.GetPropertyValueFromStoreObject(storeObject, this.isAssociatedProperty);
			if (!flag)
			{
				StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeObject.Id);
				className = storeObject.ClassName;
				bool isPublic = !IdConverter.IsMessageId(asStoreObjectId.ProviderLevelItemId) || IdConverter.IsFromPublicStore(asStoreObjectId);
				if (this.ValidateProperty(storeObject, className, isPublic))
				{
					return true;
				}
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[WebClientQueryStringPropertyBase::ShouldCreateForServiceObjectOrXML] Returning false (isAssociated: {0}} for storeObject.Id: {1}", flag.ToString(), storeObject.Id.ToString());
			return false;
		}

		private bool ShouldCreateForPropertyBag(IDictionary<PropertyDefinition, object> propertyBag, out string className, out StoreId storeId)
		{
			className = null;
			storeId = null;
			bool flag;
			if (PropertyCommand.TryGetValueFromPropertyBag<StoreId>(propertyBag, this.itemIdProperty, out storeId) && PropertyCommand.TryGetValueFromPropertyBag<string>(propertyBag, this.itemClassProperty, out className) && PropertyCommand.TryGetValueFromPropertyBag<bool>(propertyBag, this.isAssociatedProperty, out flag))
			{
				if (!flag)
				{
					StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId);
					bool isPublic = IdConverter.IsFromPublicStore(asStoreObjectId);
					if (this.ValidatePropertyFromPropertyBag(propertyBag, className, isPublic))
					{
						return true;
					}
				}
				else
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[WebClientQueryStringPropertyBase::ShouldCreateForPropertyBag] Returning false (isAssociated: {0}} for storeObject.Id: {1}", flag.ToString(), (storeId != null) ? storeId.ToString() : "<null>");
				}
			}
			else
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string>((long)this.GetHashCode(), "[WebClientQueryStringPropertyBase::ShouldCreateForPropertyBag] Returning false for storeObject.Id: {0}", (storeId != null) ? storeId.ToString() : "<null>");
			}
			return false;
		}

		protected virtual bool ValidateProperty(StoreObject storeObject, string className, bool isPublic)
		{
			return true;
		}

		protected virtual bool ValidatePropertyFromPropertyBag(IDictionary<PropertyDefinition, object> propertyBag, string className, bool isPublic)
		{
			return true;
		}

		private StringBuilder CreateQueryString(string className, string id)
		{
			StringBuilder stringBuilder = new StringBuilder("?");
			stringBuilder.Append("ItemID=" + WebUtility.UrlEncode(id));
			stringBuilder.Append("&exvsurl=1");
			stringBuilder.Append("&viewmodel=" + this.GetOwaViewModel(className));
			if (CallContext.Current.AccessingPrincipal.MailboxInfo == null)
			{
				return stringBuilder;
			}
			return stringBuilder;
		}

		protected virtual string GetOwaViewModel(string className)
		{
			if (className != null)
			{
				if (className == "IPM.Appointment")
				{
					return WebClientQueryStringPropertyBase.ReadCalendarViewModel;
				}
				if (className == "IPM.Contact")
				{
					return WebClientQueryStringPropertyBase.ReadContactViewModel;
				}
				if (className == "IPM.Task")
				{
					return WebClientQueryStringPropertyBase.ReadTaskViewModel;
				}
			}
			return WebClientQueryStringPropertyBase.ReadItemViewModel;
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			string className;
			if (this.ShouldCreateForServiceObjectOrXML(commandSettings.StoreObject, out className))
			{
				string itemId = this.GetItemId(commandSettings.StoreObject.Id, commandSettings.IdAndSession);
				this.CreateQueryStringXmlElement(commandSettings.ServiceItem, className, itemId);
			}
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			string className;
			StoreId storeId;
			if (this.ShouldCreateForPropertyBag(commandSettings.PropertyBag, out className, out storeId))
			{
				string itemId = this.GetItemId(storeId, commandSettings.IdAndSession);
				this.CreateQueryStringXmlElement(commandSettings.ServiceItem, className, itemId);
			}
		}

		protected void CreateQueryStringXmlElement(XmlElement serviceItem, string className, string id)
		{
			StringBuilder stringBuilder = this.CreateQueryString(className, id);
			base.CreateXmlTextElement(serviceItem, this.xmlLocalName, stringBuilder.ToString());
		}

		private const int ItemIdPropertyIndex = 0;

		private const int ItemClassPropertyIndex = 1;

		private const int IsAssociatedPropertyIndex = 2;

		private PropertyDefinition itemIdProperty;

		private PropertyDefinition itemClassProperty;

		private PropertyDefinition isAssociatedProperty;

		protected static readonly string ReadContactViewModel = "PersonaCardViewModelFactory";

		protected static readonly string ReadCalendarViewModel = "CalendarItemDetailsViewModelFactory";

		protected static readonly string ReadTaskViewModel = "TaskReadingPaneViewModelPopOutFactory";

		protected static readonly string ReadItemViewModel = "ReadMessageItem";
	}
}

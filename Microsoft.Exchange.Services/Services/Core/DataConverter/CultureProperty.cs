using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CultureProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public CultureProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static CultureProperty CreateCommand(CommandContext commandContext)
		{
			return new CultureProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			Item storeItem = (Item)commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			CultureInfo itemCultureInfo = CultureProperty.GetItemCultureInfo(storeItem);
			if (itemCultureInfo != null && !string.IsNullOrEmpty(itemCultureInfo.Name))
			{
				serviceObject[this.commandContext.PropertyInformation] = itemCultureInfo.Name;
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			CultureInfo itemCultureInfo = CultureProperty.GetItemCultureInfo(propertyBag, commandSettings.IdAndSession.Session);
			if (itemCultureInfo != null && !string.IsNullOrEmpty(itemCultureInfo.Name))
			{
				serviceObject[this.commandContext.PropertyInformation] = itemCultureInfo.Name;
			}
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			Item item = (Item)commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			this.SetProperty(serviceObject, item);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Item item = (Item)updateCommandSettings.StoreObject;
			this.SetProperty(setPropertyUpdate.ServiceObject, item);
		}

		private static CultureInfo GetItemCultureInfo(IDictionary<PropertyDefinition, object> propertyBag, StoreSession session)
		{
			CultureInfo result = null;
			int lcid;
			if (PropertyCommand.TryGetValueFromPropertyBag<int>(propertyBag, CultureProperty.MessageLocaleID, out lcid) && CultureProperty.TryGetCultureFromLcid(lcid, out result))
			{
				return result;
			}
			int codePage;
			if (PropertyCommand.TryGetValueFromPropertyBag<int>(propertyBag, CultureProperty.InternetCPID, out codePage) && CultureProperty.TryGetCultureInfoFromCodepage(codePage, out result))
			{
				return result;
			}
			if (PropertyCommand.TryGetValueFromPropertyBag<int>(propertyBag, ItemSchema.Codepage, out codePage) && CultureProperty.TryGetCultureInfoFromCodepage(codePage, out result))
			{
				return result;
			}
			if (!CultureProperty.TryGetPrimaryMailboxCulture(session as MailboxSession, out result))
			{
				return null;
			}
			return result;
		}

		private static CultureInfo GetItemCultureInfo(Item storeItem)
		{
			CultureInfo result = null;
			object obj = null;
			if (CultureProperty.TryGetProperty(storeItem, CultureProperty.MessageLocaleID, out obj) && CultureProperty.TryGetCultureFromLcid((int)obj, out result))
			{
				return result;
			}
			if (CultureProperty.TryGetProperty(storeItem, CultureProperty.InternetCPID, out obj) && CultureProperty.TryGetCultureInfoFromCodepage((int)obj, out result))
			{
				return result;
			}
			if (CultureProperty.TryGetProperty(storeItem, ItemSchema.Codepage, out obj) && CultureProperty.TryGetCultureInfoFromCodepage((int)obj, out result))
			{
				return result;
			}
			if (!CultureProperty.TryGetPrimaryMailboxCulture(storeItem.Session as MailboxSession, out result))
			{
				return null;
			}
			return result;
		}

		private static void SetItemCulture(Item storeItem, CultureInfo cultureInfo)
		{
			storeItem[CultureProperty.MessageLocaleID] = LocaleMap.GetLcidFromCulture(cultureInfo);
			int num = 0;
			if (CultureProperty.TryGetCodePageFromCultureInfo(cultureInfo, out num))
			{
				storeItem[ItemSchema.Codepage] = num;
			}
		}

		private static bool TryGetPrimaryMailboxCulture(MailboxSession session, out CultureInfo cultureInfo)
		{
			cultureInfo = null;
			if (session != null && !PropertyCommand.InMemoryProcessOnly && session.Capabilities.CanHaveCulture)
			{
				CultureInfo[] mailboxCultures = session.GetMailboxCultures();
				cultureInfo = ((mailboxCultures.Length > 0) ? mailboxCultures[0] : null);
			}
			return cultureInfo != null;
		}

		private static bool TryGetCultureFromLcid(int lcid, out CultureInfo culture)
		{
			bool result;
			try
			{
				culture = LocaleMap.GetCultureFromLcid(lcid);
				result = true;
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<int, string>(0L, "[CultureProperty::TryGetCultureInfoFromLcid] ArgumentException encountered when using lcid '{0}'.  Exception message: {1}", lcid, ex.Message);
				culture = null;
				result = false;
			}
			return result;
		}

		private static bool TryGetCultureInfoFromCodepage(int codePage, out CultureInfo cultureInfo)
		{
			bool result;
			try
			{
				Charset charset = Charset.GetCharset(codePage);
				Culture culture = charset.Culture;
				cultureInfo = new CultureInfo(culture.Name);
				result = true;
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<int, string>(0L, "[CultureProperty::TryGetCultureInfoFromCodepage] ArgumentException encountered when using codepage '{0}'.  Exception message: {1}", codePage, ex.Message);
				cultureInfo = null;
				result = false;
			}
			catch (InvalidCharsetException ex2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<int, string>(0L, "[CultureProperty::TryGetCultureInfoFromCodepage] InvalidCharsetException encountered when using codepage '{0}'.  Exception message: {1}", codePage, ex2.Message);
				cultureInfo = null;
				result = false;
			}
			return result;
		}

		private static bool TryGetCodePageFromCultureInfo(CultureInfo cultureInfo, out int codePage)
		{
			Culture culture = null;
			if (!Culture.TryGetCulture(cultureInfo.Name, out culture))
			{
				codePage = 0;
				return false;
			}
			codePage = culture.WindowsCharset.CodePage;
			return true;
		}

		private static bool TryGetProperty(StoreObject storeObject, PropertyDefinition propDef, out object value)
		{
			object obj = storeObject.TryGetProperty(propDef);
			if (obj is PropertyError)
			{
				value = null;
				return false;
			}
			value = obj;
			return true;
		}

		private void SetProperty(ServiceObject serviceObject, Item item)
		{
			string valueOrDefault = serviceObject.GetValueOrDefault<string>(this.commandContext.PropertyInformation);
			CultureInfo cultureInfo = null;
			try
			{
				cultureInfo = new CultureInfo(valueOrDefault);
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>(0L, "[CultureProperty::SetProperty] ArgumentException encountered whensetting property to value '{0}'.  Exception message: {1}", valueOrDefault, ex.Message);
				throw new UnsupportedCultureException(ex);
			}
			if (cultureInfo == CultureInfo.InvariantCulture)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "[CultureProperty::SetProperty] Can't set item culture to invariant culture.  Input culture: {0}", valueOrDefault);
				throw new UnsupportedCultureException();
			}
			CultureProperty.SetItemCulture(item, cultureInfo);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("CultureProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("CultureProperty.ToXmlForPropertyBag should not be called.");
		}

		public static PropertyTagPropertyDefinition InternetCPID = PropertyTagPropertyDefinition.CreateCustom("PR_INTERNET_CPID", 1071513603U);

		public static PropertyTagPropertyDefinition MessageLocaleID = PropertyTagPropertyDefinition.CreateCustom("PR_MESSAGE_LOCALE_ID", 1072758787U);
	}
}

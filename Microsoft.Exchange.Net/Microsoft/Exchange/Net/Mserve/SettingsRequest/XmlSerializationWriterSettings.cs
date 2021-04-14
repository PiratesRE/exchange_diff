using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	internal class XmlSerializationWriterSettings : XmlSerializationWriter
	{
		public void Write33_Settings(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("Settings", "HMSETTINGS:");
				return;
			}
			base.TopLevelElement();
			this.Write32_Settings("Settings", "HMSETTINGS:", (Settings)o, false, false);
		}

		private void Write32_Settings(string n, string ns, Settings o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(Settings)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write6_ServiceSettingsType("ServiceSettings", "HMSETTINGS:", o.ServiceSettings, false, false);
			this.Write31_AccountSettingsType("AccountSettings", "HMSETTINGS:", o.AccountSettings, false, false);
			base.WriteEndElement(o);
		}

		private void Write31_AccountSettingsType(string n, string ns, AccountSettingsType o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(AccountSettingsType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("AccountSettingsType", "HMSETTINGS:");
			}
			if (o.Item != null)
			{
				if (o.Item is AccountSettingsTypeGet)
				{
					this.Write30_AccountSettingsTypeGet("Get", "HMSETTINGS:", (AccountSettingsTypeGet)o.Item, false, false);
				}
				else
				{
					if (!(o.Item is AccountSettingsTypeSet))
					{
						throw base.CreateUnknownTypeException(o.Item);
					}
					this.Write29_AccountSettingsTypeSet("Set", "HMSETTINGS:", (AccountSettingsTypeSet)o.Item, false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write29_AccountSettingsTypeSet(string n, string ns, AccountSettingsTypeSet o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(AccountSettingsTypeSet)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			FiltersRequestTypeFilter[] filters = o.Filters;
			if (filters != null)
			{
				base.WriteStartElement("Filters", "HMSETTINGS:", null, false);
				for (int i = 0; i < filters.Length; i++)
				{
					this.Write15_FiltersRequestTypeFilter("Filter", "HMSETTINGS:", filters[i], false, false);
				}
				base.WriteEndElement();
			}
			ListsRequestTypeList[] lists = o.Lists;
			if (lists != null)
			{
				base.WriteStartElement("Lists", "HMSETTINGS:", null, false);
				for (int j = 0; j < lists.Length; j++)
				{
					this.Write18_ListsRequestTypeList("List", "HMSETTINGS:", lists[j], false, false);
				}
				base.WriteEndElement();
			}
			this.Write28_OptionsType("Options", "HMSETTINGS:", o.Options, false, false);
			this.Write10_StringWithCharSetType("UserSignature", "HMSETTINGS:", o.UserSignature, false, false);
			base.WriteEndElement(o);
		}

		private void Write10_StringWithCharSetType(string n, string ns, StringWithCharSetType o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(StringWithCharSetType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("StringWithCharSetType", "HMSETTINGS:");
			}
			base.WriteAttribute("charset", "", o.charset);
			if (o.Value != null)
			{
				base.WriteValue(o.Value);
			}
			base.WriteEndElement(o);
		}

		private void Write28_OptionsType(string n, string ns, OptionsType o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(OptionsType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("OptionsType", "HMSETTINGS:");
			}
			base.WriteElementStringRaw("ConfirmSent", "HMSETTINGS:", XmlConvert.ToString(o.ConfirmSent));
			base.WriteElementString("HeaderDisplay", "HMSETTINGS:", this.Write19_HeaderDisplayType(o.HeaderDisplay));
			base.WriteElementString("IncludeOriginalInReply", "HMSETTINGS:", this.Write20_IncludeOriginalInReplyType(o.IncludeOriginalInReply));
			base.WriteElementString("JunkLevel", "HMSETTINGS:", this.Write21_JunkLevelType(o.JunkLevel));
			base.WriteElementString("JunkMailDestination", "HMSETTINGS:", this.Write22_JunkMailDestinationType(o.JunkMailDestination));
			base.WriteElementString("ReplyIndicator", "HMSETTINGS:", this.Write23_ReplyIndicatorType(o.ReplyIndicator));
			base.WriteElementString("ReplyToAddress", "HMSETTINGS:", o.ReplyToAddress);
			base.WriteElementStringRaw("SaveSentMail", "HMSETTINGS:", XmlConvert.ToString(o.SaveSentMail));
			base.WriteElementStringRaw("UseReplyToAddress", "HMSETTINGS:", XmlConvert.ToString(o.UseReplyToAddress));
			this.Write25_OptionsTypeVacationResponse("VacationResponse", "HMSETTINGS:", o.VacationResponse, false, false);
			this.Write27_OptionsTypeMailForwarding("MailForwarding", "HMSETTINGS:", o.MailForwarding, false, false);
			base.WriteEndElement(o);
		}

		private void Write27_OptionsTypeMailForwarding(string n, string ns, OptionsTypeMailForwarding o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(OptionsTypeMailForwarding)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("Mode", "HMSETTINGS:", this.Write26_ForwardingMode(o.Mode));
			string[] addresses = o.Addresses;
			if (addresses != null)
			{
				base.WriteStartElement("Addresses", "HMSETTINGS:", null, false);
				for (int i = 0; i < addresses.Length; i++)
				{
					base.WriteElementString("Address", "HMSETTINGS:", addresses[i]);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private string Write26_ForwardingMode(ForwardingMode v)
		{
			string result;
			switch (v)
			{
			case ForwardingMode.NoForwarding:
				result = "NoForwarding";
				break;
			case ForwardingMode.ForwardOnly:
				result = "ForwardOnly";
				break;
			case ForwardingMode.StoreAndForward:
				result = "StoreAndForward";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.ForwardingMode");
			}
			return result;
		}

		private void Write25_OptionsTypeVacationResponse(string n, string ns, OptionsTypeVacationResponse o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(OptionsTypeVacationResponse)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("Mode", "HMSETTINGS:", this.Write24_VacationResponseMode(o.Mode));
			base.WriteElementString("StartTime", "HMSETTINGS:", o.StartTime);
			base.WriteElementString("EndTime", "HMSETTINGS:", o.EndTime);
			base.WriteElementString("Message", "HMSETTINGS:", o.Message);
			base.WriteEndElement(o);
		}

		private string Write24_VacationResponseMode(VacationResponseMode v)
		{
			string result;
			switch (v)
			{
			case VacationResponseMode.NoVacationResponse:
				result = "NoVacationResponse";
				break;
			case VacationResponseMode.OncePerSender:
				result = "OncePerSender";
				break;
			case VacationResponseMode.OncePerContact:
				result = "OncePerContact";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.VacationResponseMode");
			}
			return result;
		}

		private string Write23_ReplyIndicatorType(ReplyIndicatorType v)
		{
			string result;
			switch (v)
			{
			case ReplyIndicatorType.None:
				result = "None";
				break;
			case ReplyIndicatorType.Line:
				result = "Line";
				break;
			case ReplyIndicatorType.Arrow:
				result = "Arrow";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.ReplyIndicatorType");
			}
			return result;
		}

		private string Write22_JunkMailDestinationType(JunkMailDestinationType v)
		{
			string result;
			switch (v)
			{
			case JunkMailDestinationType.ImmediateDeletion:
				result = "Immediate Deletion";
				break;
			case JunkMailDestinationType.JunkMail:
				result = "Junk Mail";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.JunkMailDestinationType");
			}
			return result;
		}

		private string Write21_JunkLevelType(JunkLevelType v)
		{
			string result;
			switch (v)
			{
			case JunkLevelType.Off:
				result = "Off";
				break;
			case JunkLevelType.Low:
				result = "Low";
				break;
			case JunkLevelType.High:
				result = "High";
				break;
			case JunkLevelType.Exclusive:
				result = "Exclusive";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.JunkLevelType");
			}
			return result;
		}

		private string Write20_IncludeOriginalInReplyType(IncludeOriginalInReplyType v)
		{
			string result;
			switch (v)
			{
			case IncludeOriginalInReplyType.Auto:
				result = "Auto";
				break;
			case IncludeOriginalInReplyType.Manual:
				result = "Manual";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.IncludeOriginalInReplyType");
			}
			return result;
		}

		private string Write19_HeaderDisplayType(HeaderDisplayType v)
		{
			string result;
			switch (v)
			{
			case HeaderDisplayType.NoHeader:
				result = "No Header";
				break;
			case HeaderDisplayType.Basic:
				result = "Basic";
				break;
			case HeaderDisplayType.Full:
				result = "Full";
				break;
			case HeaderDisplayType.Advanced:
				result = "Advanced";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.HeaderDisplayType");
			}
			return result;
		}

		private void Write18_ListsRequestTypeList(string n, string ns, ListsRequestTypeList o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(ListsRequestTypeList)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteAttribute("name", "", o.name);
			AddressesAndDomainsType[] items = o.Items;
			if (items != null)
			{
				ItemsChoiceType[] itemsElementName = o.ItemsElementName;
				if (itemsElementName == null || itemsElementName.Length < items.Length)
				{
					throw base.CreateInvalidChoiceIdentifierValueException("Microsoft.Exchange.Net.Mserve.SettingsRequest.ItemsChoiceType", "ItemsElementName");
				}
				for (int i = 0; i < items.Length; i++)
				{
					AddressesAndDomainsType addressesAndDomainsType = items[i];
					ItemsChoiceType itemsChoiceType = itemsElementName[i];
					if (addressesAndDomainsType != null)
					{
						if (itemsChoiceType == ItemsChoiceType.Set)
						{
							if (addressesAndDomainsType != null && addressesAndDomainsType == null)
							{
								throw base.CreateMismatchChoiceException("Microsoft.Exchange.Net.Mserve.SettingsRequest.AddressesAndDomainsType", "ItemsElementName", "Microsoft.Exchange.Net.Mserve.SettingsRequest.ItemsChoiceType.@Set");
							}
							this.Write17_AddressesAndDomainsType("Set", "HMSETTINGS:", addressesAndDomainsType, false, false);
						}
						else if (itemsChoiceType == ItemsChoiceType.Delete)
						{
							if (addressesAndDomainsType != null && addressesAndDomainsType == null)
							{
								throw base.CreateMismatchChoiceException("Microsoft.Exchange.Net.Mserve.SettingsRequest.AddressesAndDomainsType", "ItemsElementName", "Microsoft.Exchange.Net.Mserve.SettingsRequest.ItemsChoiceType.@Delete");
							}
							this.Write17_AddressesAndDomainsType("Delete", "HMSETTINGS:", addressesAndDomainsType, false, false);
						}
						else
						{
							if (itemsChoiceType != ItemsChoiceType.Add)
							{
								throw base.CreateUnknownTypeException(addressesAndDomainsType);
							}
							if (addressesAndDomainsType != null && addressesAndDomainsType == null)
							{
								throw base.CreateMismatchChoiceException("Microsoft.Exchange.Net.Mserve.SettingsRequest.AddressesAndDomainsType", "ItemsElementName", "Microsoft.Exchange.Net.Mserve.SettingsRequest.ItemsChoiceType.@Add");
							}
							this.Write17_AddressesAndDomainsType("Add", "HMSETTINGS:", addressesAndDomainsType, false, false);
						}
					}
				}
			}
			base.WriteEndElement(o);
		}

		private void Write17_AddressesAndDomainsType(string n, string ns, AddressesAndDomainsType o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(AddressesAndDomainsType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("AddressesAndDomainsType", "HMSETTINGS:");
			}
			string[] addresses = o.Addresses;
			if (addresses != null)
			{
				base.WriteStartElement("Addresses", "HMSETTINGS:", null, false);
				for (int i = 0; i < addresses.Length; i++)
				{
					base.WriteElementString("Address", "HMSETTINGS:", addresses[i]);
				}
				base.WriteEndElement();
			}
			string[] domains = o.Domains;
			if (domains != null)
			{
				base.WriteStartElement("Domains", "HMSETTINGS:", null, false);
				for (int j = 0; j < domains.Length; j++)
				{
					base.WriteElementString("Domain", "HMSETTINGS:", domains[j]);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private void Write15_FiltersRequestTypeFilter(string n, string ns, FiltersRequestTypeFilter o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(FiltersRequestTypeFilter)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementStringRaw("ExecutionOrder", "HMSETTINGS:", XmlConvert.ToString(o.ExecutionOrder));
			base.WriteElementStringRaw("Enabled", "HMSETTINGS:", XmlConvert.ToString(o.Enabled));
			base.WriteElementString("RunWhen", "HMSETTINGS:", this.Write7_RunWhenType(o.RunWhen));
			this.Write12_Item("Condition", "HMSETTINGS:", o.Condition, false, false);
			this.Write14_Item("Actions", "HMSETTINGS:", o.Actions, false, false);
			base.WriteEndElement(o);
		}

		private void Write14_Item(string n, string ns, FiltersRequestTypeFilterActions o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(FiltersRequestTypeFilterActions)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write13_Item("MoveToFolder", "HMSETTINGS:", o.MoveToFolder, false, false);
			base.WriteEndElement(o);
		}

		private void Write13_Item(string n, string ns, FiltersRequestTypeFilterActionsMoveToFolder o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(FiltersRequestTypeFilterActionsMoveToFolder)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("FolderId", "HMSETTINGS:", o.FolderId);
			base.WriteEndElement(o);
		}

		private void Write12_Item(string n, string ns, FiltersRequestTypeFilterCondition o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(FiltersRequestTypeFilterCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write11_Item("Clause", "HMSETTINGS:", o.Clause, false, false);
			base.WriteEndElement(o);
		}

		private void Write11_Item(string n, string ns, FiltersRequestTypeFilterConditionClause o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(FiltersRequestTypeFilterConditionClause)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("Field", "HMSETTINGS:", this.Write8_FilterKeyType(o.Field));
			base.WriteElementString("Operator", "HMSETTINGS:", this.Write9_FilterOperatorType(o.Operator));
			this.Write10_StringWithCharSetType("Value", "HMSETTINGS:", o.Value, false, false);
			base.WriteEndElement(o);
		}

		private string Write9_FilterOperatorType(FilterOperatorType v)
		{
			string result;
			switch (v)
			{
			case FilterOperatorType.Contains:
				result = "Contains";
				break;
			case FilterOperatorType.Doesnotcontain:
				result = "Does not contain";
				break;
			case FilterOperatorType.Containsword:
				result = "Contains word";
				break;
			case FilterOperatorType.Startswith:
				result = "Starts with";
				break;
			case FilterOperatorType.Endswith:
				result = "Ends with";
				break;
			case FilterOperatorType.Equals:
				result = "Equals";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.FilterOperatorType");
			}
			return result;
		}

		private string Write8_FilterKeyType(FilterKeyType v)
		{
			string result;
			switch (v)
			{
			case FilterKeyType.Subject:
				result = "Subject";
				break;
			case FilterKeyType.FromName:
				result = "From Name";
				break;
			case FilterKeyType.FromAddress:
				result = "From Address";
				break;
			case FilterKeyType.ToorCCLine:
				result = "To or CC Line";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.FilterKeyType");
			}
			return result;
		}

		private string Write7_RunWhenType(RunWhenType v)
		{
			string result;
			switch (v)
			{
			case RunWhenType.MessageReceived:
				result = "MessageReceived";
				break;
			case RunWhenType.MessageSent:
				result = "MessageSent";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.RunWhenType");
			}
			return result;
		}

		private void Write30_AccountSettingsTypeGet(string n, string ns, AccountSettingsTypeGet o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(AccountSettingsTypeGet)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write1_Object("Filters", "HMSETTINGS:", o.Filters, false, false);
			this.Write1_Object("Lists", "HMSETTINGS:", o.Lists, false, false);
			this.Write1_Object("Options", "HMSETTINGS:", o.Options, false, false);
			this.Write1_Object("Properties", "HMSETTINGS:", o.Properties, false, false);
			this.Write1_Object("UserSignature", "HMSETTINGS:", o.UserSignature, false, false);
			base.WriteEndElement(o);
		}

		private void Write1_Object(string n, string ns, object o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(object)))
				{
					if (type == typeof(AccountSettingsType))
					{
						this.Write31_AccountSettingsType(n, ns, (AccountSettingsType)o, isNullable, true);
						return;
					}
					if (type == typeof(OptionsType))
					{
						this.Write28_OptionsType(n, ns, (OptionsType)o, isNullable, true);
						return;
					}
					if (type == typeof(AddressesAndDomainsType))
					{
						this.Write17_AddressesAndDomainsType(n, ns, (AddressesAndDomainsType)o, isNullable, true);
						return;
					}
					if (type == typeof(StringWithCharSetType))
					{
						this.Write10_StringWithCharSetType(n, ns, (StringWithCharSetType)o, isNullable, true);
						return;
					}
					if (type == typeof(ServiceSettingsType))
					{
						this.Write6_ServiceSettingsType(n, ns, (ServiceSettingsType)o, isNullable, true);
						return;
					}
					if (type == typeof(RunWhenType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("RunWhenType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write7_RunWhenType((RunWhenType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(FilterKeyType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("FilterKeyType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write8_FilterKeyType((FilterKeyType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(FilterOperatorType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("FilterOperatorType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write9_FilterOperatorType((FilterOperatorType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(FiltersRequestTypeFilter[]))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ArrayOfFiltersRequestTypeFilter", "HMSETTINGS:");
						FiltersRequestTypeFilter[] array = (FiltersRequestTypeFilter[])o;
						if (array != null)
						{
							for (int i = 0; i < array.Length; i++)
							{
								this.Write15_FiltersRequestTypeFilter("Filter", "HMSETTINGS:", array[i], false, false);
							}
						}
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(ItemsChoiceType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ItemsChoiceType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write16_ItemsChoiceType((ItemsChoiceType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(string[]))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ArrayOfString", "HMSETTINGS:");
						string[] array2 = (string[])o;
						if (array2 != null)
						{
							for (int j = 0; j < array2.Length; j++)
							{
								base.WriteElementString("Address", "HMSETTINGS:", array2[j]);
							}
						}
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(string[]))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ArrayOfString1", "HMSETTINGS:");
						string[] array3 = (string[])o;
						if (array3 != null)
						{
							for (int k = 0; k < array3.Length; k++)
							{
								base.WriteElementString("Domain", "HMSETTINGS:", array3[k]);
							}
						}
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(ListsRequestTypeList[]))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ArrayOfListsRequestTypeList", "HMSETTINGS:");
						ListsRequestTypeList[] array4 = (ListsRequestTypeList[])o;
						if (array4 != null)
						{
							for (int l = 0; l < array4.Length; l++)
							{
								this.Write18_ListsRequestTypeList("List", "HMSETTINGS:", array4[l], false, false);
							}
						}
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(HeaderDisplayType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("HeaderDisplayType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write19_HeaderDisplayType((HeaderDisplayType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(IncludeOriginalInReplyType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("IncludeOriginalInReplyType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write20_IncludeOriginalInReplyType((IncludeOriginalInReplyType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(JunkLevelType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("JunkLevelType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write21_JunkLevelType((JunkLevelType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(JunkMailDestinationType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("JunkMailDestinationType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write22_JunkMailDestinationType((JunkMailDestinationType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(ReplyIndicatorType))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ReplyIndicatorType", "HMSETTINGS:");
						base.Writer.WriteString(this.Write23_ReplyIndicatorType((ReplyIndicatorType)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(VacationResponseMode))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("VacationResponseMode", "HMSETTINGS:");
						base.Writer.WriteString(this.Write24_VacationResponseMode((VacationResponseMode)o));
						base.Writer.WriteEndElement();
						return;
					}
					if (type == typeof(ForwardingMode))
					{
						base.Writer.WriteStartElement(n, ns);
						base.WriteXsiType("ForwardingMode", "HMSETTINGS:");
						base.Writer.WriteString(this.Write26_ForwardingMode((ForwardingMode)o));
						base.Writer.WriteEndElement();
						return;
					}
					base.WriteTypedPrimitive(n, ns, o, true);
					return;
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			base.WriteEndElement(o);
		}

		private string Write16_ItemsChoiceType(ItemsChoiceType v)
		{
			string result;
			switch (v)
			{
			case ItemsChoiceType.Add:
				result = "Add";
				break;
			case ItemsChoiceType.Delete:
				result = "Delete";
				break;
			case ItemsChoiceType.Set:
				result = "Set";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsRequest.ItemsChoiceType");
			}
			return result;
		}

		private void Write6_ServiceSettingsType(string n, string ns, ServiceSettingsType o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(ServiceSettingsType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("ServiceSettingsType", "HMSETTINGS:");
			}
			base.WriteElementString("SafetySchemaVersion", "HMSETTINGS:", o.SafetySchemaVersion);
			this.Write2_Item("SafetyLevelRules", "HMSETTINGS:", o.SafetyLevelRules, false, false);
			this.Write3_Item("SafetyActions", "HMSETTINGS:", o.SafetyActions, false, false);
			this.Write4_ServiceSettingsTypeProperties("Properties", "HMSETTINGS:", o.Properties, false, false);
			this.Write5_ServiceSettingsTypeLists("Lists", "HMSETTINGS:", o.Lists, false, false);
			base.WriteEndElement(o);
		}

		private void Write5_ServiceSettingsTypeLists(string n, string ns, ServiceSettingsTypeLists o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(ServiceSettingsTypeLists)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write1_Object("Get", "HMSETTINGS:", o.Get, false, false);
			base.WriteEndElement(o);
		}

		private void Write4_ServiceSettingsTypeProperties(string n, string ns, ServiceSettingsTypeProperties o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(ServiceSettingsTypeProperties)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write1_Object("Get", "HMSETTINGS:", o.Get, false, false);
			base.WriteEndElement(o);
		}

		private void Write3_Item(string n, string ns, ServiceSettingsTypeSafetyActions o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(ServiceSettingsTypeSafetyActions)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write1_Object("GetVersion", "HMSETTINGS:", o.GetVersion, false, false);
			this.Write1_Object("Get", "HMSETTINGS:", o.Get, false, false);
			base.WriteEndElement(o);
		}

		private void Write2_Item(string n, string ns, ServiceSettingsTypeSafetyLevelRules o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(ServiceSettingsTypeSafetyLevelRules)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write1_Object("GetVersion", "HMSETTINGS:", o.GetVersion, false, false);
			this.Write1_Object("Get", "HMSETTINGS:", o.Get, false, false);
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}

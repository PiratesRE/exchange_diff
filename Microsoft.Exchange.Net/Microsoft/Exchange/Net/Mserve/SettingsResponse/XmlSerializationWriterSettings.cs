using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	internal class XmlSerializationWriterSettings : XmlSerializationWriter
	{
		public void Write43_Settings(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteEmptyTag("Settings", "HMSETTINGS:");
				return;
			}
			base.TopLevelElement();
			this.Write42_Settings("Settings", "HMSETTINGS:", (Settings)o, false, false);
		}

		private void Write42_Settings(string n, string ns, Settings o, bool isNullable, bool needType)
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
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			this.Write2_SettingsFault("Fault", "HMSETTINGS:", o.Fault, false, false);
			this.Write3_SettingsAuthPolicy("AuthPolicy", "HMSETTINGS:", o.AuthPolicy, false, false);
			this.Write10_SettingsServiceSettings("ServiceSettings", "HMSETTINGS:", o.ServiceSettings, false, false);
			this.Write41_SettingsAccountSettings("AccountSettings", "HMSETTINGS:", o.AccountSettings, false, false);
			base.WriteEndElement(o);
		}

		private void Write41_SettingsAccountSettings(string n, string ns, SettingsAccountSettings o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsAccountSettings)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			if (o.StatusSpecified)
			{
				base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			}
			if (o.Item != null)
			{
				if (o.Item is SettingsAccountSettingsSet)
				{
					this.Write40_SettingsAccountSettingsSet("Set", "HMSETTINGS:", (SettingsAccountSettingsSet)o.Item, false, false);
				}
				else
				{
					if (!(o.Item is SettingsAccountSettingsGet))
					{
						throw base.CreateUnknownTypeException(o.Item);
					}
					this.Write33_SettingsAccountSettingsGet("Get", "HMSETTINGS:", (SettingsAccountSettingsGet)o.Item, false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write33_SettingsAccountSettingsGet(string n, string ns, SettingsAccountSettingsGet o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsAccountSettingsGet)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			FiltersResponseTypeFilter[] filters = o.Filters;
			if (filters != null)
			{
				base.WriteStartElement("Filters", "HMSETTINGS:", null, false);
				for (int i = 0; i < filters.Length; i++)
				{
					this.Write19_FiltersResponseTypeFilter("Filter", "HMSETTINGS:", filters[i], false, false);
				}
				base.WriteEndElement();
			}
			ListsGetResponseTypeList[] lists = o.Lists;
			if (lists != null)
			{
				base.WriteStartElement("Lists", "HMSETTINGS:", null, false);
				for (int j = 0; j < lists.Length; j++)
				{
					this.Write7_ListsGetResponseTypeList("List", "HMSETTINGS:", lists[j], false, false);
				}
				base.WriteEndElement();
			}
			this.Write29_OptionsType("Options", "HMSETTINGS:", o.Options, false, false);
			this.Write32_PropertiesType("Properties", "HMSETTINGS:", o.Properties, false, false);
			this.Write14_StringWithVersionType("UserSignature", "HMSETTINGS:", o.UserSignature, false, false);
			base.WriteEndElement(o);
		}

		private void Write14_StringWithVersionType(string n, string ns, StringWithVersionType o, bool isNullable, bool needType)
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
				if (!(type == typeof(StringWithVersionType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("StringWithVersionType", "HMSETTINGS:");
			}
			base.WriteAttribute("version", "", XmlConvert.ToString(o.version));
			if (o.Value != null)
			{
				base.WriteValue(o.Value);
			}
			base.WriteEndElement(o);
		}

		private void Write32_PropertiesType(string n, string ns, PropertiesType o, bool isNullable, bool needType)
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
				if (!(type == typeof(PropertiesType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("PropertiesType", "HMSETTINGS:");
			}
			base.WriteElementString("AccountStatus", "HMSETTINGS:", this.Write30_AccountStatusType(o.AccountStatus));
			base.WriteElementString("ParentalControlStatus", "HMSETTINGS:", this.Write31_ParentalControlStatusType(o.ParentalControlStatus));
			base.WriteElementStringRaw("MailBoxSize", "HMSETTINGS:", XmlConvert.ToString(o.MailBoxSize));
			base.WriteElementStringRaw("MaxMailBoxSize", "HMSETTINGS:", XmlConvert.ToString(o.MaxMailBoxSize));
			base.WriteElementStringRaw("MaxAttachments", "HMSETTINGS:", XmlConvert.ToString(o.MaxAttachments));
			base.WriteElementStringRaw("MaxMessageSize", "HMSETTINGS:", XmlConvert.ToString(o.MaxMessageSize));
			base.WriteElementStringRaw("MaxUnencodedMessageSize", "HMSETTINGS:", XmlConvert.ToString(o.MaxUnencodedMessageSize));
			base.WriteElementStringRaw("MaxFilters", "HMSETTINGS:", XmlConvert.ToString(o.MaxFilters));
			base.WriteElementStringRaw("MaxFilterClauseValueLength", "HMSETTINGS:", XmlConvert.ToString(o.MaxFilterClauseValueLength));
			base.WriteElementStringRaw("MaxRecipients", "HMSETTINGS:", XmlConvert.ToString(o.MaxRecipients));
			base.WriteElementStringRaw("MaxUserSignatureLength", "HMSETTINGS:", XmlConvert.ToString(o.MaxUserSignatureLength));
			base.WriteElementStringRaw("BlockListAddressMax", "HMSETTINGS:", XmlConvert.ToString(o.BlockListAddressMax));
			base.WriteElementStringRaw("BlockListDomainMax", "HMSETTINGS:", XmlConvert.ToString(o.BlockListDomainMax));
			base.WriteElementStringRaw("WhiteListAddressMax", "HMSETTINGS:", XmlConvert.ToString(o.WhiteListAddressMax));
			base.WriteElementStringRaw("WhiteListDomainMax", "HMSETTINGS:", XmlConvert.ToString(o.WhiteListDomainMax));
			base.WriteElementStringRaw("WhiteToListMax", "HMSETTINGS:", XmlConvert.ToString(o.WhiteToListMax));
			base.WriteElementStringRaw("AlternateFromListMax", "HMSETTINGS:", XmlConvert.ToString(o.AlternateFromListMax));
			base.WriteElementStringRaw("MaxDailySendMessages", "HMSETTINGS:", XmlConvert.ToString(o.MaxDailySendMessages));
			base.WriteEndElement(o);
		}

		private string Write31_ParentalControlStatusType(ParentalControlStatusType v)
		{
			string result;
			switch (v)
			{
			case ParentalControlStatusType.None:
				result = "None";
				break;
			case ParentalControlStatusType.FullAccess:
				result = "FullAccess";
				break;
			case ParentalControlStatusType.RestrictedAccess:
				result = "RestrictedAccess";
				break;
			case ParentalControlStatusType.NoAccess:
				result = "NoAccess";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.ParentalControlStatusType");
			}
			return result;
		}

		private string Write30_AccountStatusType(AccountStatusType v)
		{
			string result;
			switch (v)
			{
			case AccountStatusType.OK:
				result = "OK";
				break;
			case AccountStatusType.Blocked:
				result = "Blocked";
				break;
			case AccountStatusType.RequiresHIP:
				result = "RequiresHIP";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.AccountStatusType");
			}
			return result;
		}

		private void Write29_OptionsType(string n, string ns, OptionsType o, bool isNullable, bool needType)
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
			base.WriteElementString("HeaderDisplay", "HMSETTINGS:", this.Write20_HeaderDisplayType(o.HeaderDisplay));
			base.WriteElementString("IncludeOriginalInReply", "HMSETTINGS:", this.Write21_IncludeOriginalInReplyType(o.IncludeOriginalInReply));
			base.WriteElementString("JunkLevel", "HMSETTINGS:", this.Write22_JunkLevelType(o.JunkLevel));
			base.WriteElementString("JunkMailDestination", "HMSETTINGS:", this.Write23_JunkMailDestinationType(o.JunkMailDestination));
			base.WriteElementString("ReplyIndicator", "HMSETTINGS:", this.Write24_ReplyIndicatorType(o.ReplyIndicator));
			base.WriteElementString("ReplyToAddress", "HMSETTINGS:", o.ReplyToAddress);
			base.WriteElementStringRaw("SaveSentMail", "HMSETTINGS:", XmlConvert.ToString(o.SaveSentMail));
			base.WriteElementStringRaw("UseReplyToAddress", "HMSETTINGS:", XmlConvert.ToString(o.UseReplyToAddress));
			this.Write26_OptionsTypeVacationResponse("VacationResponse", "HMSETTINGS:", o.VacationResponse, false, false);
			this.Write28_OptionsTypeMailForwarding("MailForwarding", "HMSETTINGS:", o.MailForwarding, false, false);
			base.WriteEndElement(o);
		}

		private void Write28_OptionsTypeMailForwarding(string n, string ns, OptionsTypeMailForwarding o, bool isNullable, bool needType)
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
			base.WriteElementString("Mode", "HMSETTINGS:", this.Write27_ForwardingMode(o.Mode));
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

		private string Write27_ForwardingMode(ForwardingMode v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.ForwardingMode");
			}
			return result;
		}

		private void Write26_OptionsTypeVacationResponse(string n, string ns, OptionsTypeVacationResponse o, bool isNullable, bool needType)
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
			base.WriteElementString("Mode", "HMSETTINGS:", this.Write25_VacationResponseMode(o.Mode));
			base.WriteElementString("StartTime", "HMSETTINGS:", o.StartTime);
			base.WriteElementString("EndTime", "HMSETTINGS:", o.EndTime);
			base.WriteElementString("Message", "HMSETTINGS:", o.Message);
			base.WriteEndElement(o);
		}

		private string Write25_VacationResponseMode(VacationResponseMode v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.VacationResponseMode");
			}
			return result;
		}

		private string Write24_ReplyIndicatorType(ReplyIndicatorType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.ReplyIndicatorType");
			}
			return result;
		}

		private string Write23_JunkMailDestinationType(JunkMailDestinationType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.JunkMailDestinationType");
			}
			return result;
		}

		private string Write22_JunkLevelType(JunkLevelType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.JunkLevelType");
			}
			return result;
		}

		private string Write21_IncludeOriginalInReplyType(IncludeOriginalInReplyType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.IncludeOriginalInReplyType");
			}
			return result;
		}

		private string Write20_HeaderDisplayType(HeaderDisplayType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.HeaderDisplayType");
			}
			return result;
		}

		private void Write7_ListsGetResponseTypeList(string n, string ns, ListsGetResponseTypeList o, bool isNullable, bool needType)
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
				if (!(type == typeof(ListsGetResponseTypeList)))
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
			string[] localParts = o.LocalParts;
			if (localParts != null)
			{
				base.WriteStartElement("LocalParts", "HMSETTINGS:", null, false);
				for (int k = 0; k < localParts.Length; k++)
				{
					base.WriteElementString("LocalPart", "HMSETTINGS:", localParts[k]);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private void Write19_FiltersResponseTypeFilter(string n, string ns, FiltersResponseTypeFilter o, bool isNullable, bool needType)
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
				if (!(type == typeof(FiltersResponseTypeFilter)))
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
			base.WriteElementString("RunWhen", "HMSETTINGS:", this.Write11_RunWhenType(o.RunWhen));
			this.Write16_Item("Condition", "HMSETTINGS:", o.Condition, false, false);
			this.Write18_Item("Actions", "HMSETTINGS:", o.Actions, false, false);
			base.WriteEndElement(o);
		}

		private void Write18_Item(string n, string ns, FiltersResponseTypeFilterActions o, bool isNullable, bool needType)
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
				if (!(type == typeof(FiltersResponseTypeFilterActions)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write17_Item("MoveToFolder", "HMSETTINGS:", o.MoveToFolder, false, false);
			base.WriteEndElement(o);
		}

		private void Write17_Item(string n, string ns, FiltersResponseTypeFilterActionsMoveToFolder o, bool isNullable, bool needType)
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
				if (!(type == typeof(FiltersResponseTypeFilterActionsMoveToFolder)))
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

		private void Write16_Item(string n, string ns, FiltersResponseTypeFilterCondition o, bool isNullable, bool needType)
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
				if (!(type == typeof(FiltersResponseTypeFilterCondition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write15_Item("Clause", "HMSETTINGS:", o.Clause, false, false);
			base.WriteEndElement(o);
		}

		private void Write15_Item(string n, string ns, FiltersResponseTypeFilterConditionClause o, bool isNullable, bool needType)
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
				if (!(type == typeof(FiltersResponseTypeFilterConditionClause)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("Field", "HMSETTINGS:", this.Write12_FilterKeyType(o.Field));
			base.WriteElementString("Operator", "HMSETTINGS:", this.Write13_FilterOperatorType(o.Operator));
			this.Write14_StringWithVersionType("Value", "HMSETTINGS:", o.Value, false, false);
			base.WriteEndElement(o);
		}

		private string Write13_FilterOperatorType(FilterOperatorType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.FilterOperatorType");
			}
			return result;
		}

		private string Write12_FilterKeyType(FilterKeyType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.FilterKeyType");
			}
			return result;
		}

		private string Write11_RunWhenType(RunWhenType v)
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
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Net.Mserve.SettingsResponse.RunWhenType");
			}
			return result;
		}

		private void Write40_SettingsAccountSettingsSet(string n, string ns, SettingsAccountSettingsSet o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsAccountSettingsSet)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			this.Write35_SettingsCategoryResponseType("Filters", "HMSETTINGS:", o.Filters, false, false);
			this.Write39_ListsSetResponseType("Lists", "HMSETTINGS:", o.Lists, false, false);
			this.Write35_SettingsCategoryResponseType("Options", "HMSETTINGS:", o.Options, false, false);
			this.Write35_SettingsCategoryResponseType("UserSignature", "HMSETTINGS:", o.UserSignature, false, false);
			base.WriteEndElement(o);
		}

		private void Write35_SettingsCategoryResponseType(string n, string ns, SettingsCategoryResponseType o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsCategoryResponseType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("SettingsCategoryResponseType", "HMSETTINGS:");
			}
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			this.Write34_Fault("Fault", "HMSETTINGS:", o.Fault, false, false);
			base.WriteEndElement(o);
		}

		private void Write34_Fault(string n, string ns, Fault o, bool isNullable, bool needType)
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
				if (!(type == typeof(Fault)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("Faultcode", "HMSETTINGS:", o.Faultcode);
			base.WriteElementString("Faultstring", "HMSETTINGS:", o.Faultstring);
			base.WriteElementString("Detail", "HMSETTINGS:", o.Detail);
			base.WriteEndElement(o);
		}

		private void Write39_ListsSetResponseType(string n, string ns, ListsSetResponseType o, bool isNullable, bool needType)
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
				if (!(type == typeof(ListsSetResponseType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("ListsSetResponseType", "HMSETTINGS:");
			}
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			ListsSetResponseTypeList[] list = o.List;
			if (list != null)
			{
				for (int i = 0; i < list.Length; i++)
				{
					this.Write38_ListsSetResponseTypeList("List", "HMSETTINGS:", list[i], false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write38_ListsSetResponseTypeList(string n, string ns, ListsSetResponseTypeList o, bool isNullable, bool needType)
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
				if (!(type == typeof(ListsSetResponseTypeList)))
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
			StatusType[] items = o.Items;
			if (items != null)
			{
				ItemsChoiceType[] itemsElementName = o.ItemsElementName;
				if (itemsElementName == null || itemsElementName.Length < items.Length)
				{
					throw base.CreateInvalidChoiceIdentifierValueException("Microsoft.Exchange.Net.Mserve.SettingsResponse.ItemsChoiceType", "ItemsElementName");
				}
				for (int i = 0; i < items.Length; i++)
				{
					StatusType statusType = items[i];
					ItemsChoiceType itemsChoiceType = itemsElementName[i];
					if (statusType != null)
					{
						if (itemsChoiceType == ItemsChoiceType.Add)
						{
							if (statusType != null && statusType == null)
							{
								throw base.CreateMismatchChoiceException("Microsoft.Exchange.Net.Mserve.SettingsResponse.StatusType", "ItemsElementName", "Microsoft.Exchange.Net.Mserve.SettingsResponse.ItemsChoiceType.@Add");
							}
							this.Write37_StatusType("Add", "HMSETTINGS:", statusType, false, false);
						}
						else if (itemsChoiceType == ItemsChoiceType.Set)
						{
							if (statusType != null && statusType == null)
							{
								throw base.CreateMismatchChoiceException("Microsoft.Exchange.Net.Mserve.SettingsResponse.StatusType", "ItemsElementName", "Microsoft.Exchange.Net.Mserve.SettingsResponse.ItemsChoiceType.@Set");
							}
							this.Write37_StatusType("Set", "HMSETTINGS:", statusType, false, false);
						}
						else
						{
							if (itemsChoiceType != ItemsChoiceType.Delete)
							{
								throw base.CreateUnknownTypeException(statusType);
							}
							if (statusType != null && statusType == null)
							{
								throw base.CreateMismatchChoiceException("Microsoft.Exchange.Net.Mserve.SettingsResponse.StatusType", "ItemsElementName", "Microsoft.Exchange.Net.Mserve.SettingsResponse.ItemsChoiceType.@Delete");
							}
							this.Write37_StatusType("Delete", "HMSETTINGS:", statusType, false, false);
						}
					}
				}
			}
			base.WriteEndElement(o);
		}

		private void Write37_StatusType(string n, string ns, StatusType o, bool isNullable, bool needType)
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
				if (!(type == typeof(StatusType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("StatusType", "HMSETTINGS:");
			}
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			this.Write34_Fault("Fault", "HMSETTINGS:", o.Fault, false, false);
			base.WriteEndElement(o);
		}

		private void Write10_SettingsServiceSettings(string n, string ns, SettingsServiceSettings o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsServiceSettings)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			this.Write4_RulesResponseType("SafetyLevelRules", "HMSETTINGS:", o.SafetyLevelRules, false, false);
			this.Write4_RulesResponseType("SafetyActions", "HMSETTINGS:", o.SafetyActions, false, false);
			this.Write6_Item("Properties", "HMSETTINGS:", o.Properties, false, false);
			this.Write9_SettingsServiceSettingsLists("Lists", "HMSETTINGS:", o.Lists, false, false);
			base.WriteEndElement(o);
		}

		private void Write9_SettingsServiceSettingsLists(string n, string ns, SettingsServiceSettingsLists o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsServiceSettingsLists)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			this.Write8_Item("Get", "HMSETTINGS:", o.Get, false, false);
			base.WriteEndElement(o);
		}

		private void Write8_Item(string n, string ns, SettingsServiceSettingsListsGet o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsServiceSettingsListsGet)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			ListsGetResponseTypeList[] lists = o.Lists;
			if (lists != null)
			{
				base.WriteStartElement("Lists", "HMSETTINGS:", null, false);
				for (int i = 0; i < lists.Length; i++)
				{
					this.Write7_ListsGetResponseTypeList("List", "HMSETTINGS:", lists[i], false, false);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private void Write6_Item(string n, string ns, SettingsServiceSettingsProperties o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsServiceSettingsProperties)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			this.Write5_ServiceSettingsPropertiesType("Get", "HMSETTINGS:", o.Get, false, false);
			base.WriteEndElement(o);
		}

		private void Write5_ServiceSettingsPropertiesType(string n, string ns, ServiceSettingsPropertiesType o, bool isNullable, bool needType)
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
				if (!(type == typeof(ServiceSettingsPropertiesType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("ServiceSettingsPropertiesType", "HMSETTINGS:");
			}
			base.WriteElementStringRaw("ServiceTimeOut", "HMSETTINGS:", XmlConvert.ToString(o.ServiceTimeOut));
			base.WriteElementStringRaw("MinSyncPollInterval", "HMSETTINGS:", XmlConvert.ToString(o.MinSyncPollInterval));
			base.WriteElementStringRaw("MinSettingsPollInterval", "HMSETTINGS:", XmlConvert.ToString(o.MinSettingsPollInterval));
			base.WriteElementStringRaw("SyncMultiplier", "HMSETTINGS:", XmlConvert.ToString(o.SyncMultiplier));
			base.WriteElementStringRaw("MaxObjectsInSync", "HMSETTINGS:", XmlConvert.ToString(o.MaxObjectsInSync));
			base.WriteElementStringRaw("MaxNumberOfEmailAdds", "HMSETTINGS:", XmlConvert.ToString(o.MaxNumberOfEmailAdds));
			base.WriteElementStringRaw("MaxNumberOfFolderAdds", "HMSETTINGS:", XmlConvert.ToString(o.MaxNumberOfFolderAdds));
			base.WriteElementStringRaw("MaxNumberOfStatelessObjects", "HMSETTINGS:", XmlConvert.ToString(o.MaxNumberOfStatelessObjects));
			base.WriteElementStringRaw("DefaultStatelessEmailWindowSize", "HMSETTINGS:", XmlConvert.ToString(o.DefaultStatelessEmailWindowSize));
			base.WriteElementStringRaw("MaxStatelessEmailWindowSize", "HMSETTINGS:", XmlConvert.ToString(o.MaxStatelessEmailWindowSize));
			base.WriteElementStringRaw("MaxTotalLengthOfForwardingAddresses", "HMSETTINGS:", XmlConvert.ToString(o.MaxTotalLengthOfForwardingAddresses));
			base.WriteElementStringRaw("MaxVacationResponseMessageLength", "HMSETTINGS:", XmlConvert.ToString(o.MaxVacationResponseMessageLength));
			base.WriteElementString("MinVacationResponseStartTime", "HMSETTINGS:", o.MinVacationResponseStartTime);
			base.WriteElementString("MaxVacationResponseEndTime", "HMSETTINGS:", o.MaxVacationResponseEndTime);
			base.WriteElementStringRaw("MaxNumberOfProvisionCommands", "HMSETTINGS:", XmlConvert.ToString(o.MaxNumberOfProvisionCommands));
			base.WriteEndElement(o);
		}

		private void Write4_RulesResponseType(string n, string ns, RulesResponseType o, bool isNullable, bool needType)
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
				if (!(type == typeof(RulesResponseType)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("RulesResponseType", "HMSETTINGS:");
			}
			base.WriteElementStringRaw("Status", "HMSETTINGS:", XmlConvert.ToString(o.Status));
			if (o.Get != null || o.Get == null)
			{
				base.WriteElementLiteral(o.Get, "Get", "HMSETTINGS:", false, false);
				base.WriteElementString("Version", "HMSETTINGS:", o.Version);
				base.WriteEndElement(o);
				return;
			}
			throw base.CreateInvalidAnyTypeException(o.Get);
		}

		private void Write3_SettingsAuthPolicy(string n, string ns, SettingsAuthPolicy o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsAuthPolicy)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("SAP", "HMSETTINGS:", o.SAP);
			base.WriteElementString("Version", "HMSETTINGS:", o.Version);
			base.WriteEndElement(o);
		}

		private void Write2_SettingsFault(string n, string ns, SettingsFault o, bool isNullable, bool needType)
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
				if (!(type == typeof(SettingsFault)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType(null, "HMSETTINGS:");
			}
			base.WriteElementString("Faultcode", "HMSETTINGS:", o.Faultcode);
			base.WriteElementString("Faultstring", "HMSETTINGS:", o.Faultstring);
			base.WriteElementString("Detail", "HMSETTINGS:", o.Detail);
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}

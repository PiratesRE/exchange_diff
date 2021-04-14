using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Ceres.SearchCore.Admin.Model;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FullTextIndex;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	internal class FullTextIndexSchema
	{
		private void PopulateLookupDictionary()
		{
			this.AddProperty(PropTag.Message.SearchAllIndexedProps, null, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchIsPartiallyIndexed, FastIndexSystemSchema.IsPartiallyProcessed, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.BodyUnicode, FastIndexSystemSchema.Body, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.RtfCompressed, FastIndexSystemSchema.Body, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.BodyHtml, FastIndexSystemSchema.Body, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchFullTextBody, FastIndexSystemSchema.Body, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Sharing.SharingRemoteComment, FastIndexSystemSchema.SharingInfo, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Sharing.SharingRemotePath, FastIndexSystemSchema.SharingInfo, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Sharing.SharingRemoteName, FastIndexSystemSchema.SharingInfo, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Sharing.SharingInitiatorName, FastIndexSystemSchema.SharingInfo, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Sharing.SharingLocalName, FastIndexSystemSchema.SharingInfo, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Sharing.SharingBrowseUrl, FastIndexSystemSchema.SharingInfo, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.MailboxGuid, FastIndexSystemSchema.MailboxGuid, FullTextIndexSchema.CriteriaFullTextFlavor.CanBeServiced);
			this.AddProperty(PropTag.Message.MessageClass, FastIndexSystemSchema.ItemClass, FullTextIndexSchema.CriteriaFullTextFlavor.CanBeServiced);
			this.AddProperty(PropTag.Message.NormalizedSubject, FastIndexSystemSchema.Subject, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.Subject, FastIndexSystemSchema.Subject, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchFullTextSubject, FastIndexSystemSchema.Subject, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.MessageSize, FastIndexSystemSchema.Size, FullTextIndexSchema.CriteriaFullTextFlavor.CanBeServiced);
			this.AddProperty(PropTag.Message.ClientSubmitTime, FastIndexSystemSchema.Sent, FullTextIndexSchema.CriteriaFullTextFlavor.CanBeServiced);
			this.AddProperty(PropTag.Message.MessageDeliveryTime, FastIndexSystemSchema.Received, FullTextIndexSchema.CriteriaFullTextFlavor.CanBeServiced);
			this.AddProperty(PropTag.Message.Importance, FastIndexSystemSchema.Importance, FullTextIndexSchema.CriteriaFullTextFlavor.CanBeServiced);
			this.AddProperty(PropTag.Message.SentRepresentingName, FastIndexSystemSchema.From, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SentRepresentingEmailAddress, FastIndexSystemSchema.From, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SenderName, FastIndexSystemSchema.From, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SenderEmailAddress, FastIndexSystemSchema.From, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchSender, FastIndexSystemSchema.From, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.DisplayBcc, FastIndexSystemSchema.Bcc, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchRecipientsBcc, FastIndexSystemSchema.Bcc, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.DisplayCc, FastIndexSystemSchema.Cc, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchRecipientsCc, FastIndexSystemSchema.Cc, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.DisplayTo, FastIndexSystemSchema.To, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchRecipientsTo, FastIndexSystemSchema.To, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchRecipients, FastIndexSystemSchema.Recipients, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.MessageRecipients, FastIndexSystemSchema.Recipients, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.DisplayName, FastIndexSystemSchema.DisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.DisplayNameFirstLast, FastIndexSystemSchema.DisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.DisplayNamePrefix, FastIndexSystemSchema.DisplayNamePrefix, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.EmailAddress, FastIndexSystemSchema.EmailAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.GivenName, FastIndexSystemSchema.FirstName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SurName, FastIndexSystemSchema.LastName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.DepartmentName, FastIndexSystemSchema.DepartmentName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Common.ToDoTitle, FastIndexSystemSchema.TaskTitle, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.PublicStrings.Keywords, FastIndexSystemSchema.Categories, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Appointment.Location, FastIndexSystemSchema.MeetingLocation, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.UnifiedMessaging.UMAudioNotes, FastIndexSystemSchema.UmAudioNotes, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.ConversationTopic, FastIndexSystemSchema.ConversationTopic, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.EmailDisplayName, FastIndexSystemSchema.EmailDisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.EmailEmailAddress, FastIndexSystemSchema.EmailAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.EmailOriginalDisplayName, FastIndexSystemSchema.EmailOriginalDisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.Email2DisplayName, FastIndexSystemSchema.EmailDisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.Email2EmailAddress, FastIndexSystemSchema.EmailAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.Email2OriginalDisplayName, FastIndexSystemSchema.EmailOriginalDisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.Email3DisplayName, FastIndexSystemSchema.EmailDisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.Email3EmailAddress, FastIndexSystemSchema.EmailAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.Email3OriginalDisplayName, FastIndexSystemSchema.EmailOriginalDisplayName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.FileUnder, FastIndexSystemSchema.FileAs, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.HomeAddress, FastIndexSystemSchema.HomeAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.InstMsg, FastIndexSystemSchema.IMAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.OtherAddress, FastIndexSystemSchema.OtherAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.WorkAddress, FastIndexSystemSchema.BusinessAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.WorkAddressCity, FastIndexSystemSchema.BusinessAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.WorkAddressCountry, FastIndexSystemSchema.BusinessAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.WorkAddressPostalCode, FastIndexSystemSchema.BusinessAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.WorkAddressState, FastIndexSystemSchema.BusinessAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.WorkAddressStreet, FastIndexSystemSchema.BusinessAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.YomiCompanyName, FastIndexSystemSchema.YomiCompanyName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.YomiFirstName, FastIndexSystemSchema.YomiFirstName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.YomiLastName, FastIndexSystemSchema.YomiLastName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.AirSync.ASIMAddress2, FastIndexSystemSchema.IMAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.AirSync.ASIMAddress3, FastIndexSystemSchema.IMAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.Account, FastIndexSystemSchema.Account, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.BusinessTelephoneNumber, FastIndexSystemSchema.BusinessPhoneNumber, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.Business2TelephoneNumber, FastIndexSystemSchema.BusinessPhoneNumber, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.CarTelephoneNumber, FastIndexSystemSchema.CarPhoneNumber, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.CompanyMainPhoneNumber, FastIndexSystemSchema.BusinessMainPhone, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.CompanyName, FastIndexSystemSchema.CompanyName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.HomeAddressCity, FastIndexSystemSchema.HomeAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.HomeAddressCountry, FastIndexSystemSchema.HomeAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.HomeAddressPostalCode, FastIndexSystemSchema.HomeAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.HomeAddressStateOrProvince, FastIndexSystemSchema.HomeAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.HomeAddressStreet, FastIndexSystemSchema.HomeAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.HomeTelephoneNumber, FastIndexSystemSchema.HomePhone, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.Home2TelephoneNumber, FastIndexSystemSchema.HomePhone, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.MiddleName, FastIndexSystemSchema.MiddleName, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.MobileTelephoneNumber, FastIndexSystemSchema.MobilePhoneNumber, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Address.MobileTelephoneNumber2, FastIndexSystemSchema.MobilePhoneNumber, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.NickName, FastIndexSystemSchema.Nickname, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.OfficeLocation, FastIndexSystemSchema.OfficeLocation, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.OtherAddressCity, FastIndexSystemSchema.OtherAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.OtherAddressCountry, FastIndexSystemSchema.OtherAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.OtherAddressPostalCode, FastIndexSystemSchema.OtherAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.OtherAddressStateOrProvince, FastIndexSystemSchema.OtherAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.OtherAddressStreet, FastIndexSystemSchema.OtherAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.PostOfficeBox, FastIndexSystemSchema.BusinessAddress, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.PrimaryTelephoneNumber, FastIndexSystemSchema.PrimaryTelephoneNumber, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.Title, FastIndexSystemSchema.Title, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchAttachments, FastIndexSystemSchema.Body, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.SearchAttachmentsOLK, FastIndexSystemSchema.Body, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.ContentId, FastIndexSystemSchema.AttachmentFileNames, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.MimeUrl, FastIndexSystemSchema.AttachmentFileNames, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.ToGroupExpansionRecipients, FastIndexSystemSchema.ToGroupExpansionRecipients, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.CcGroupExpansionRecipients, FastIndexSystemSchema.CcGroupExpansionRecipients, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(PropTag.Message.BccGroupExpansionRecipients, FastIndexSystemSchema.BccGroupExpansionRecipients, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
			this.AddProperty(NamedPropInfo.Compliance.GroupExpansionRecipients, FastIndexSystemSchema.GroupExpansionRecipients, FullTextIndexSchema.CriteriaFullTextFlavor.MustBeServiced);
		}

		private FullTextIndexSchema()
		{
			this.maxSupplementaryRestrictionsSupported = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "FastMaxSupplementaryTerms", 4);
			this.PopulateLookupDictionary();
			this.AddProperty("FolderId", FastIndexSystemSchema.FolderId, FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryOnly);
		}

		public static FullTextIndexSchema Current
		{
			get
			{
				return FullTextIndexSchema.HookableInstance.Value;
			}
		}

		public int MaxNumberOfSupplementaryRestrictions
		{
			get
			{
				return this.maxSupplementaryRestrictionsSupported;
			}
		}

		internal static Hookable<FullTextIndexSchema> Instance
		{
			get
			{
				return FullTextIndexSchema.HookableInstance;
			}
		}

		internal Dictionary<StorePropName, FullTextIndexSchema.FullTextIndexInfo> SchemaLookupDictionary
		{
			get
			{
				return this.schemaLookupDictionary;
			}
		}

		internal List<StorePropTag> PropTags
		{
			get
			{
				return this.propTags;
			}
		}

		internal List<StoreNamedPropInfo> PropInfos
		{
			get
			{
				return this.propInfos;
			}
		}

		public static void Initialize()
		{
			TimeSpan interval = TimeSpan.FromSeconds((double)RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "QueryVersionLookupSeconds", (int)DefaultSettings.Get.QueryVersionRefreshInterval.TotalSeconds));
			FullTextIndexSchema.queryVersionLookupTask = new RecurringTask<FullTextIndexSchema>(new Task<FullTextIndexSchema>.TaskCallback(FullTextIndexSchema.QueryVersionLookupTimer), FullTextIndexSchema.Current, interval);
			FullTextIndexSchema.queryVersionLookupTask.Start();
		}

		public static void Terminate()
		{
			if (FullTextIndexSchema.queryVersionLookupTask != null)
			{
				FullTextIndexSchema.queryVersionLookupTask.Dispose();
				FullTextIndexSchema.queryVersionLookupTask = null;
			}
		}

		public static string GetImsFlowName(Guid databaseGuid)
		{
			return FlowDescriptor.GetImsFlowDescriptor(SearchConfig.Instance, FastIndexVersion.GetIndexSystemName(databaseGuid)).DisplayName;
		}

		public bool IsColumnInFullTextIndex(Column column, Guid mdbGuid, out FullTextIndexSchema.FullTextIndexInfo fastPropertyInfo)
		{
			ExtendedPropertyColumn extendedPropertyColumn = column as ExtendedPropertyColumn;
			if (extendedPropertyColumn != null)
			{
				return this.IsPropertyInFullTextIndex(extendedPropertyColumn.StorePropTag.PropInfo.PropName, mdbGuid, out fastPropertyInfo);
			}
			if (string.IsNullOrEmpty(column.Name))
			{
				fastPropertyInfo = null;
				return false;
			}
			return this.schemaLookupDictionaryForColumns.TryGetValue(column.Name, out fastPropertyInfo) && fastPropertyInfo.FullTextFlavor >= FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryOnly && fastPropertyInfo.RequiredVersion.QueryVersion <= this.GetCurrentQueryVersion(mdbGuid);
		}

		public bool IsPropertyInFullTextIndex(StorePropName propName, Guid mdbGuid, out FullTextIndexSchema.FullTextIndexInfo fastPropertyInfo)
		{
			return this.schemaLookupDictionary.TryGetValue(propName, out fastPropertyInfo) && fastPropertyInfo.FullTextFlavor >= FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryOnly && fastPropertyInfo.RequiredVersion.QueryVersion <= this.GetCurrentQueryVersion(mdbGuid);
		}

		public FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteria value, Guid mdbGuid, out bool needsResidualFiltering)
		{
			return this.GetCriteriaFullTextFlavor(value, mdbGuid, false, out needsResidualFiltering);
		}

		public FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteria value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering)
		{
			bool flag;
			return this.GetCriteriaFullTextFlavor(value, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering, out flag);
		}

		private static bool IsPrefixSearch(SearchCriteriaText criteria)
		{
			if ((ushort)(criteria.FullnessFlags & SearchCriteriaText.SearchTextFullness.PhraseMatch) == 32)
			{
				return false;
			}
			ConstantColumn constantColumn = criteria.Rhs as ConstantColumn;
			string text = (constantColumn.Value as string) ?? string.Empty;
			return text.EndsWith("*", StringComparison.OrdinalIgnoreCase) || (ushort)(criteria.FullnessFlags & SearchCriteriaText.SearchTextFullness.Prefix) == 2 || (ushort)(criteria.FullnessFlags & SearchCriteriaText.SearchTextFullness.PrefixOnAnyWord) == 16 || (ushort)(criteria.FullnessFlags & SearchCriteriaText.SearchTextFullness.SubString) == 1;
		}

		private static bool IsMatchingOnDefaultMessageClass(FullTextIndexSchema.FullTextIndexInfo indexColumn, ConstantColumn constantColumn, bool isPrefixSearch)
		{
			bool result = false;
			if (indexColumn.Definition != null && indexColumn.FastPropertyName == FastIndexSystemSchema.ItemClass.Name)
			{
				string text = (constantColumn.Value as string) ?? string.Empty;
				if (isPrefixSearch)
				{
					text = text.TrimEnd(new char[]
					{
						'*'
					});
					if ("IPM.Note".StartsWith(text, StringComparison.OrdinalIgnoreCase))
					{
						result = true;
					}
				}
				else if (text.Equals("IPM.Note", StringComparison.OrdinalIgnoreCase))
				{
					result = true;
				}
			}
			return result;
		}

		private static void TraceCriteriaFullTextFlavor(FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor, bool needsResidualFiltering, bool isDueToBadFilterFactor, string criteriaType, string criteriaDesc, string reasonForFlavor)
		{
			ExTraceGlobals.CriteriaFullTextFlavorTracer.TraceDebug(0L, "GetCriteriaFullTextFlavor for {0} restriction: result=={1}, needsResidualFiltering=={2}, isDueToBadFilterFactor=={3}", new object[]
			{
				criteriaType,
				criteriaFullTextFlavor,
				needsResidualFiltering,
				isDueToBadFilterFactor
			});
			ExTraceGlobals.CriteriaFullTextFlavorTracer.TraceDebug<string>(0L, "    Reason: {0}", reasonForFlavor);
			ExTraceGlobals.CriteriaFullTextFlavorTracer.TraceDebug<string>(0L, "    Criteria: {0}", criteriaDesc);
		}

		private static void QueryVersionLookupTimer(TaskExecutionDiagnosticsProxy diagnosticsContext, FullTextIndexSchema state, Func<bool> shouldCallbackContinue)
		{
			if (!shouldCallbackContinue())
			{
				return;
			}
			IReadOnlyDictionary<Guid, IndexStatus> indexStatusDictionary = IndexStatusStore.Instance.GetIndexStatusDictionary();
			if (indexStatusDictionary.Count == 0)
			{
				state.currentQueryVersions.Clear();
				return;
			}
			foreach (KeyValuePair<Guid, IndexStatus> keyValuePair in indexStatusDictionary)
			{
				state.currentQueryVersions[keyValuePair.Key] = keyValuePair.Value.Version.QueryVersion;
			}
		}

		private int GetCurrentQueryVersion(Guid mdbGuid)
		{
			int result;
			if (mdbGuid != Guid.Empty && this.currentQueryVersions.TryGetValue(mdbGuid, out result))
			{
				return result;
			}
			VersionInfo latest = VersionInfo.Latest;
			return latest.QueryVersion;
		}

		private FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteria value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering, out bool isDueToBadFilterFactor)
		{
			isDueToBadFilterFactor = false;
			SearchCriteriaAnd searchCriteriaAnd = value as SearchCriteriaAnd;
			SearchCriteriaOr searchCriteriaOr = value as SearchCriteriaOr;
			SearchCriteriaNot searchCriteriaNot = value as SearchCriteriaNot;
			SearchCriteriaNear searchCriteriaNear = value as SearchCriteriaNear;
			SearchCriteriaCompare searchCriteriaCompare = value as SearchCriteriaCompare;
			SearchCriteriaText searchCriteriaText = value as SearchCriteriaText;
			if (searchCriteriaAnd != null)
			{
				return this.GetCriteriaFullTextFlavor(searchCriteriaAnd, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering);
			}
			if (searchCriteriaOr != null)
			{
				return this.GetCriteriaFullTextFlavor(searchCriteriaOr, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering);
			}
			if (searchCriteriaNot != null)
			{
				return this.GetCriteriaFullTextFlavor(searchCriteriaNot, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering);
			}
			if (searchCriteriaNear != null)
			{
				return this.GetCriteriaFullTextFlavor(searchCriteriaNear, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering);
			}
			if (searchCriteriaCompare != null)
			{
				return this.GetCriteriaFullTextFlavor(searchCriteriaCompare, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering, out isDueToBadFilterFactor);
			}
			if (searchCriteriaText != null)
			{
				return this.GetCriteriaFullTextFlavor(searchCriteriaText, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering, out isDueToBadFilterFactor);
			}
			needsResidualFiltering = true;
			if (ExTraceGlobals.CriteriaFullTextFlavorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				FullTextIndexSchema.TraceCriteriaFullTextFlavor(FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced, needsResidualFiltering, isDueToBadFilterFactor, "search", (value != null) ? value.ToString() : "<null>", "Unsupported search criteria.");
			}
			return FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
		}

		private void AddProperty(object property, FastIndexSystemField indexSystemField, FullTextIndexSchema.CriteriaFullTextFlavor queryUsageFlavor)
		{
			if (this.maxSupplementaryRestrictionsSupported == 0 && queryUsageFlavor == FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryOnly)
			{
				queryUsageFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			if (property.GetType() == typeof(StorePropTag))
			{
				StorePropTag item = (StorePropTag)property;
				this.schemaLookupDictionary.Add(item.PropName, new FullTextIndexSchema.FullTextIndexInfo(indexSystemField, queryUsageFlavor));
				this.propTags.Add(item);
				return;
			}
			StoreNamedPropInfo storeNamedPropInfo = (StoreNamedPropInfo)property;
			this.schemaLookupDictionary.Add(storeNamedPropInfo.PropName, new FullTextIndexSchema.FullTextIndexInfo(indexSystemField, queryUsageFlavor));
			this.propInfos.Add(storeNamedPropInfo);
		}

		private void AddProperty(string columnName, FastIndexSystemField indexSystemField, FullTextIndexSchema.CriteriaFullTextFlavor queryUsageFlavor)
		{
			if (this.maxSupplementaryRestrictionsSupported == 0 && queryUsageFlavor == FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryOnly)
			{
				queryUsageFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			this.schemaLookupDictionaryForColumns.Add(columnName, new FullTextIndexSchema.FullTextIndexInfo(indexSystemField, queryUsageFlavor));
		}

		private FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteriaAnd value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering)
		{
			string reasonForFlavor;
			FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor;
			if (value.NestedCriteria.Length == 0)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Missing nested criteria.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else
			{
				needsResidualFiltering = false;
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
				foreach (SearchCriteria value2 in value.NestedCriteria)
				{
					bool flag;
					FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor2 = this.GetCriteriaFullTextFlavor(value2, mdbGuid, onlyFullTextAvailable, out flag);
					criteriaFullTextFlavor = (FullTextIndexSchema.CriteriaFullTextFlavor)Math.Max((int)criteriaFullTextFlavor, (int)criteriaFullTextFlavor2);
					needsResidualFiltering = (needsResidualFiltering || flag);
				}
				reasonForFlavor = "Combined result of the individual clauses.";
			}
			if (ExTraceGlobals.CriteriaFullTextFlavorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				FullTextIndexSchema.TraceCriteriaFullTextFlavor(criteriaFullTextFlavor, needsResidualFiltering, false, "AND", value.ToString(), reasonForFlavor);
			}
			return criteriaFullTextFlavor;
		}

		private FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteriaOr value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering)
		{
			string reasonForFlavor;
			FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor;
			if (value.NestedCriteria.Length == 0)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Missing nested criteria.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else
			{
				needsResidualFiltering = false;
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
				int num = 0;
				foreach (SearchCriteria value2 in value.NestedCriteria)
				{
					bool flag;
					bool flag2;
					FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor2 = this.GetCriteriaFullTextFlavor(value2, mdbGuid, onlyFullTextAvailable, out flag, out flag2);
					if (criteriaFullTextFlavor2 == FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryOnly)
					{
						num++;
					}
					criteriaFullTextFlavor = (FullTextIndexSchema.CriteriaFullTextFlavor)Math.Max((int)criteriaFullTextFlavor, (int)criteriaFullTextFlavor2);
					needsResidualFiltering = (needsResidualFiltering || flag);
				}
				if (!onlyFullTextAvailable && num > this.maxSupplementaryRestrictionsSupported && criteriaFullTextFlavor == FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryOnly)
				{
					needsResidualFiltering = true;
					reasonForFlavor = "Exceeded maximum number of supplementary clauses.";
					criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.SupplementaryCannotBeServiced;
				}
				else
				{
					reasonForFlavor = "Combined result of the individual clauses.";
				}
			}
			if (ExTraceGlobals.CriteriaFullTextFlavorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				FullTextIndexSchema.TraceCriteriaFullTextFlavor(criteriaFullTextFlavor, needsResidualFiltering, false, "OR", value.ToString(), reasonForFlavor);
			}
			return criteriaFullTextFlavor;
		}

		private FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteriaNear value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering)
		{
			string reasonForFlavor;
			FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor;
			if (value.Criteria == null)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Missing nested criteria.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else
			{
				reasonForFlavor = "Result of nested criteria evaluation.";
				criteriaFullTextFlavor = this.GetCriteriaFullTextFlavor(value.Criteria, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering);
			}
			if (ExTraceGlobals.CriteriaFullTextFlavorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				FullTextIndexSchema.TraceCriteriaFullTextFlavor(criteriaFullTextFlavor, needsResidualFiltering, false, "NEAR", value.ToString(), reasonForFlavor);
			}
			return criteriaFullTextFlavor;
		}

		private FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteriaNot value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering)
		{
			string reasonForFlavor;
			FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor;
			if (value.Criteria == null)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Missing nested criteria.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else
			{
				bool flag;
				criteriaFullTextFlavor = this.GetCriteriaFullTextFlavor(value.Criteria, mdbGuid, onlyFullTextAvailable, out needsResidualFiltering, out flag);
				if (flag)
				{
					needsResidualFiltering = false;
					criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CanBeServiced;
				}
				reasonForFlavor = "Result of nested criteria evaluation.";
			}
			if (ExTraceGlobals.CriteriaFullTextFlavorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				FullTextIndexSchema.TraceCriteriaFullTextFlavor(criteriaFullTextFlavor, needsResidualFiltering, false, "NOT", value.ToString(), reasonForFlavor);
			}
			return criteriaFullTextFlavor;
		}

		private FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteriaCompare value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering, out bool isDueToBadFilterFactor)
		{
			isDueToBadFilterFactor = false;
			ConstantColumn constantColumn = value.Rhs as ConstantColumn;
			string reasonForFlavor;
			FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor;
			FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo;
			if (constantColumn == null)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Column value is not a ConstantColumn.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (constantColumn.Value == null || (constantColumn.Value is string && string.IsNullOrEmpty((string)constantColumn.Value)))
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Null or empty column value.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (!FqlQueryGenerator.IsValidFqlRangeType(constantColumn.Type) && value.RelOp != SearchCriteriaCompare.SearchRelOp.Equal && value.RelOp != SearchCriteriaCompare.SearchRelOp.NotEqual)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Full-text index engine only supports Equal and NotEqual operators for the column type.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (!this.IsColumnInFullTextIndex(value.Lhs, mdbGuid, out fullTextIndexInfo))
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Column is not indexed by the full-text index engine.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (fullTextIndexInfo.Definition == null || !fullTextIndexInfo.Definition.Queryable)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Column is not marked as queryable.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (fullTextIndexInfo.Definition.Type == 1 && !fullTextIndexInfo.Definition.NoWordBreaker)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Cannot perform Equal/NotEqual matching for columns that support word-breaking.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (!onlyFullTextAvailable && FullTextIndexSchema.IsMatchingOnDefaultMessageClass(fullTextIndexInfo, constantColumn, false))
			{
				isDueToBadFilterFactor = true;
				needsResidualFiltering = true;
				reasonForFlavor = "Cannot perform matching on default message class.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else
			{
				needsResidualFiltering = false;
				reasonForFlavor = "Per the definition for the column in the full-text index schema.";
				criteriaFullTextFlavor = fullTextIndexInfo.FullTextFlavor;
			}
			if (ExTraceGlobals.CriteriaFullTextFlavorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				FullTextIndexSchema.TraceCriteriaFullTextFlavor(criteriaFullTextFlavor, needsResidualFiltering, isDueToBadFilterFactor, "PROPERTY", (value != null) ? value.ToString() : "<null>", reasonForFlavor);
			}
			return criteriaFullTextFlavor;
		}

		private FullTextIndexSchema.CriteriaFullTextFlavor GetCriteriaFullTextFlavor(SearchCriteriaText value, Guid mdbGuid, bool onlyFullTextAvailable, out bool needsResidualFiltering, out bool isDueToBadFilterFactor)
		{
			isDueToBadFilterFactor = false;
			ConstantColumn constantColumn = value.Rhs as ConstantColumn;
			string reasonForFlavor;
			FullTextIndexSchema.CriteriaFullTextFlavor criteriaFullTextFlavor;
			FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo;
			if (constantColumn == null)
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Column value is not a ConstantColumn.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (constantColumn.Value == null || (constantColumn.Value is string && string.IsNullOrEmpty((string)constantColumn.Value)))
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Null or empty column value.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (!this.IsColumnInFullTextIndex(value.Lhs, mdbGuid, out fullTextIndexInfo))
			{
				needsResidualFiltering = true;
				reasonForFlavor = "Column is not indexed by the full-text index engine.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else if (!onlyFullTextAvailable && FullTextIndexSchema.IsMatchingOnDefaultMessageClass(fullTextIndexInfo, constantColumn, FullTextIndexSchema.IsPrefixSearch(value)))
			{
				isDueToBadFilterFactor = true;
				needsResidualFiltering = true;
				reasonForFlavor = "Cannot perform matching on default message class.";
				criteriaFullTextFlavor = FullTextIndexSchema.CriteriaFullTextFlavor.CannotBeServiced;
			}
			else
			{
				needsResidualFiltering = false;
				reasonForFlavor = "Per the definition for the column in the full-text index schema.";
				criteriaFullTextFlavor = fullTextIndexInfo.FullTextFlavor;
			}
			if (ExTraceGlobals.CriteriaFullTextFlavorTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				FullTextIndexSchema.TraceCriteriaFullTextFlavor(criteriaFullTextFlavor, needsResidualFiltering, isDueToBadFilterFactor, "CONTENT", (value != null) ? value.ToString() : "<null>", reasonForFlavor);
			}
			return criteriaFullTextFlavor;
		}

		[Conditional("DEBUG")]
		private void CheckAllFastPropertiesReferenced()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo in this.schemaLookupDictionary.Values)
			{
				if (fullTextIndexInfo.Definition != null)
				{
					hashSet.Add(fullTextIndexInfo.FastPropertyName);
				}
			}
			foreach (FullTextIndexSchema.FullTextIndexInfo fullTextIndexInfo2 in this.schemaLookupDictionaryForColumns.Values)
			{
				if (fullTextIndexInfo2.Definition != null)
				{
					hashSet.Add(fullTextIndexInfo2.FastPropertyName);
				}
			}
			FieldInfo[] fields = typeof(FastIndexSystemSchema).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			List<string> list = new List<string>();
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(null);
				IndexSystemField indexSystemField = value as IndexSystemField;
				if (indexSystemField != null && indexSystemField.Queryable && !hashSet.Contains(indexSystemField.Name) && !FullTextIndexSchema.KnownExceptions.Contains(indexSystemField.Name))
				{
					list.Add(indexSystemField.Name);
				}
			}
			if (list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value2 in list)
				{
					stringBuilder.AppendLine(value2);
				}
			}
		}

		private static readonly HashSet<string> KnownExceptions = new HashSet<string>
		{
			"itemid",
			"documentid",
			"errorcode",
			"lastattempttime"
		};

		private static readonly Hookable<FullTextIndexSchema> HookableInstance = Hookable<FullTextIndexSchema>.Create(true, new FullTextIndexSchema());

		private static RecurringTask<FullTextIndexSchema> queryVersionLookupTask;

		private readonly int maxSupplementaryRestrictionsSupported;

		private readonly Dictionary<StorePropName, FullTextIndexSchema.FullTextIndexInfo> schemaLookupDictionary = new Dictionary<StorePropName, FullTextIndexSchema.FullTextIndexInfo>();

		private readonly Dictionary<string, FullTextIndexSchema.FullTextIndexInfo> schemaLookupDictionaryForColumns = new Dictionary<string, FullTextIndexSchema.FullTextIndexInfo>();

		private readonly List<StorePropTag> propTags = new List<StorePropTag>();

		private readonly List<StoreNamedPropInfo> propInfos = new List<StoreNamedPropInfo>();

		private readonly ConcurrentDictionary<Guid, int> currentQueryVersions = new ConcurrentDictionary<Guid, int>();

		public enum CriteriaFullTextFlavor
		{
			SupplementaryCannotBeServiced,
			CannotBeServiced,
			SupplementaryOnly,
			CanBeServiced,
			MustBeServiced
		}

		public class FullTextIndexInfo
		{
			public FullTextIndexInfo(FastIndexSystemField fastIndexSystemField, FullTextIndexSchema.CriteriaFullTextFlavor queryUsageFlags)
			{
				this.fastIndexSystemField = fastIndexSystemField;
				this.FullTextFlavor = queryUsageFlags;
			}

			public FullTextIndexSchema.CriteriaFullTextFlavor FullTextFlavor { get; private set; }

			public string FastPropertyName
			{
				get
				{
					if (this.fastIndexSystemField != null)
					{
						return this.fastIndexSystemField.Name;
					}
					return string.Empty;
				}
			}

			public IndexSystemField Definition
			{
				get
				{
					if (this.fastIndexSystemField != null)
					{
						return this.fastIndexSystemField.Definition;
					}
					return null;
				}
			}

			public VersionInfo RequiredVersion
			{
				get
				{
					if (this.fastIndexSystemField != null)
					{
						return this.fastIndexSystemField.Version;
					}
					return VersionInfo.Unknown;
				}
			}

			private readonly FastIndexSystemField fastIndexSystemField;
		}
	}
}

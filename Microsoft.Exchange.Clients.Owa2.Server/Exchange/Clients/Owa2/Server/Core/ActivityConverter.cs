using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ActivityConverter
	{
		public ActivityConverter(UserContext userContext, IMailboxSession mailboxSession, string ipAddress, string userAgent, string clientVersion)
		{
			ArgumentValidator.ThrowIfNull("userContext", userContext);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNullOrEmpty("clientVersion", clientVersion);
			this.userContext = userContext;
			this.mailboxSession = mailboxSession;
			this.ipAddress = ipAddress;
			this.userAgent = userAgent;
			this.clientVersion = clientVersion;
			if (this.userContext.Key != null && UserContextUtilities.IsValidGuid(this.userContext.Key.UserContextId))
			{
				this.clientSessionId = new Guid(this.userContext.Key.UserContextId);
				return;
			}
			this.clientSessionId = Guid.NewGuid();
			ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.ctor] UserContext.Key.UserContextId is not valid: {0}.", (this.userContext.Key != null) ? this.userContext.Key.UserContextId : "UserContext.Key is null");
		}

		internal static Dictionary<string, string> ReportingLabelsForKeys
		{
			get
			{
				return ActivityConverter.reportingLabelsForKeys;
			}
		}

		public IList<Activity> GetActivities(ICollection<ClientLogEvent> events)
		{
			List<Activity> list = new List<Activity>(events.Count);
			foreach (ClientLogEvent clientLogEvent in events)
			{
				ActivityConverter.ConversionState conversionState = new ActivityConverter.ConversionState();
				ActivityId activityId;
				if (!ActivityConverter.TryGetActivityId(clientLogEvent.EventId, out activityId))
				{
					conversionState.Errors |= ActivityConverter.ConversionErrors.BadActivityId;
					ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.GetActivities] ActivityId cannot be resolved from client datapoint EventId: '{0}'.", clientLogEvent.EventId);
				}
				ExDateTime minValue;
				if (!ExDateTime.TryParse(ExTimeZone.UtcTimeZone, clientLogEvent.Time, out minValue))
				{
					conversionState.Errors |= ActivityConverter.ConversionErrors.BadTimestamp;
					minValue = ExDateTime.MinValue;
					ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.GetActivities] timestamp cannot be parsed: '{0}'", clientLogEvent.Time);
				}
				ActivityConverter.PropertySet requiredProperties;
				if (!ActivityConverter.propertiesForActivities.TryGetValue(activityId, out requiredProperties))
				{
					ActivityConverter.tracer.TraceDebug<string>(0L, "[ActivityConverter.GetActivities] activity id {0} does not have any required properties mapped. This may be expected.", activityId.ToString());
				}
				if (ActivityConverter.IsItemlessActivity(activityId))
				{
					this.AddItemlessActivity(list, clientLogEvent, activityId, minValue, requiredProperties, conversionState);
				}
				else if (ActivityConverter.IsMultipleItemActivity(activityId))
				{
					this.AddMultipleItemActivities(list, clientLogEvent, activityId, minValue, requiredProperties, conversionState);
				}
				else
				{
					this.AddSingleItemActivity(list, clientLogEvent, activityId, minValue, requiredProperties, conversionState);
				}
			}
			if (list.Count < events.Count)
			{
				string text = string.Format("[ActivityConverter.GetActivities] activity count {0} is less than datapoint count {1}. This indicates possible data loss due to conversion failures.", list.Count, events.Count);
				ActivityConverter.tracer.TraceError(0L, text);
				list.Add(this.CreateErrorActivity(text));
			}
			return list;
		}

		private static bool IsItemlessActivity(ActivityId activityId)
		{
			return ActivityConverter.itemlessActivities.Contains(activityId);
		}

		private static bool IsMultipleItemActivity(ActivityId activityId)
		{
			return ActivityConverter.multipleItemActivities.Contains(activityId);
		}

		private void AddSingleItemActivity(IList<Activity> results, ClientLogEvent logEvent, ActivityId activityId, ExDateTime time, ActivityConverter.PropertySet requiredProperties, ActivityConverter.ConversionState conversionState)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			StoreObjectId itemId = null;
			if (requiredProperties != ActivityConverter.PropertySet.GroupMailbox)
			{
				itemId = ActivityConverter.GetItemId(logEvent, dictionary, conversionState);
			}
			ActivityConverter.GetRequiredItemActivityProperties(logEvent, requiredProperties, dictionary, conversionState);
			Activity item = new Activity(activityId, ClientId.Web, time, this.clientSessionId, this.clientVersion, this.GetNextActivitySequenceNumber(), this.mailboxSession, itemId, null, dictionary);
			results.Add(item);
		}

		private static bool TryGetActivityId(string name, out ActivityId activityId)
		{
			if (string.IsNullOrEmpty(name))
			{
				ActivityConverter.tracer.TraceError(0L, "[ActivityConverter.TryGetActivityId] Activity name was null or empty.");
				activityId = ActivityId.Error;
				return false;
			}
			if (ActivityConverter.TryGetActivityIdFromSpecialCase(name, out activityId))
			{
				return true;
			}
			if (ActivityConverter.TryGetActivityIdFromEnum(name, out activityId))
			{
				return true;
			}
			ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.TryGetActivityId] Activity name not recognized: {0}.", name);
			activityId = ActivityId.Error;
			return false;
		}

		private static bool TryGetActivityIdFromSpecialCase(string name, out ActivityId activityId)
		{
			if (string.Compare(name, "SessionInfo") == 0)
			{
				activityId = ActivityId.Logon;
				return true;
			}
			activityId = ActivityId.Min;
			return false;
		}

		private static bool TryGetActivityIdFromEnum(string name, out ActivityId activityId)
		{
			int num = name.LastIndexOf('.');
			if (num == -1 || num + 1 >= name.Length)
			{
				ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.TryGetItemActivityIdFromEnum] Error converting {0} to ActivityId", name);
				activityId = ActivityId.Min;
				return false;
			}
			string value = name.Substring(num + 1);
			if (!Enum.TryParse<ActivityId>(value, out activityId))
			{
				ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.TryGetItemActivityIdFromEnum] Error converting {0} to ActivityId", name);
				activityId = ActivityId.Min;
				return false;
			}
			return true;
		}

		private static bool TryGetStoreId(string idString, BasicTypes basicType, out StoreObjectId storeId)
		{
			if (string.IsNullOrEmpty(idString))
			{
				ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.TryGetStoreId] Invalid id \"{0}.\"", (idString == null) ? "(null)" : "(empty)");
				storeId = null;
				return false;
			}
			try
			{
				ItemId itemId = new ItemId(idString, null);
				storeId = (StoreObjectId)IdConverter.ConvertItemIdToStoreId(itemId, basicType);
				return true;
			}
			catch (LocalizedException arg)
			{
				ActivityConverter.tracer.TraceError<LocalizedException, string>(0L, "[ActivityConverter.TryGetStoreId] Exception {0} occurred converting {1} to StoreId", arg, idString);
			}
			storeId = null;
			return false;
		}

		private static bool TryGetStoreConversationId(string idString, out ConversationId conversationId)
		{
			if (string.IsNullOrEmpty(idString))
			{
				ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.TryGetStoreConversationId] Invalid id \"{0}.\"", (idString == null) ? "(null)" : "(empty)");
				conversationId = null;
				return false;
			}
			try
			{
				conversationId = IdConverter.EwsIdToConversationId(idString);
				return true;
			}
			catch (LocalizedException arg)
			{
				ActivityConverter.tracer.TraceError<LocalizedException, string>(0L, "[ActivityConverter.TryGetStoreConversationId] Exception {0} occurred converting {1} to ConversationId", arg, idString);
			}
			conversationId = null;
			return false;
		}

		private static StoreObjectId GetItemId(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			StoreObjectId result = null;
			string text;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "id", true, conversionState, out text) && !ActivityConverter.TryGetStoreId(text, BasicTypes.Item, out result))
			{
				activityCustomProperties.Add("BadItemIds", text);
				conversionState.Errors |= ActivityConverter.ConversionErrors.BadItemIds;
			}
			return result;
		}

		private static StoreObjectId[] GetItemIds(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			StoreObjectId[] array = new StoreObjectId[1];
			StoreObjectId[] array2 = array;
			string text;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "ids", true, conversionState, out text))
			{
				string[] array3 = text.Split(new char[]
				{
					','
				});
				array2 = new StoreObjectId[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					if (!ActivityConverter.TryGetStoreId(array3[i], BasicTypes.Item, out array2[i]))
					{
						activityCustomProperties.Add("BadItemIds", array3[i]);
						conversionState.Errors |= ActivityConverter.ConversionErrors.BadItemIds;
						break;
					}
				}
			}
			return array2;
		}

		private static bool TryGetClientLogEventProperty(ClientLogEvent clientLogEvent, string key, bool isRequiredProperty, ActivityConverter.ConversionState conversionState, out string value)
		{
			value = null;
			if (!clientLogEvent.TryGetValue<string>(key, out value))
			{
				ActivityConverter.tracer.TraceDebug<string>(0L, "[ActivityConverter.TryGetClientLogEventProperty] {0} is missing.", key);
			}
			if (string.IsNullOrEmpty(value))
			{
				if (isRequiredProperty)
				{
					ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.TryGetClientLogEventProperty] {0} property is null or empty.", key);
					conversionState.AddMissingProperty(key);
				}
				return false;
			}
			return true;
		}

		private void AddMultipleItemActivities(IList<Activity> results, ClientLogEvent logEvent, ActivityId activityId, ExDateTime time, ActivityConverter.PropertySet requiredProperties, ActivityConverter.ConversionState conversionState)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			StoreObjectId[] itemIds = ActivityConverter.GetItemIds(logEvent, dictionary, conversionState);
			ActivityConverter.GetRequiredItemActivityProperties(logEvent, requiredProperties, dictionary, conversionState);
			foreach (StoreObjectId itemId in itemIds)
			{
				Dictionary<string, string> customProperties = new Dictionary<string, string>(dictionary);
				Activity item = new Activity(activityId, ClientId.Web, time, this.clientSessionId, this.clientVersion, this.GetNextActivitySequenceNumber(), this.mailboxSession, itemId, null, customProperties);
				results.Add(item);
			}
		}

		private void AddItemlessActivity(IList<Activity> results, ClientLogEvent logEvent, ActivityId activityId, ExDateTime time, ActivityConverter.PropertySet requiredProperties, ActivityConverter.ConversionState conversionState)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			this.GetRequiredItemlessActivityProperties(logEvent, requiredProperties, dictionary, conversionState);
			Activity item = new Activity(activityId, ClientId.Web, time, this.clientSessionId, this.clientVersion, this.GetNextActivitySequenceNumber(), this.mailboxSession, null, null, dictionary);
			results.Add(item);
		}

		private static void GetDeleteProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "dt", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["dt"], value);
			}
			int num = -1;
			if (ActivityConverter.TryGetIntegerProperty(logEvent, "dm", false, conversionState, out num))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["dm"], num.ToString());
			}
			int num2 = -1;
			if (ActivityConverter.TryGetIntegerProperty(logEvent, "cda", false, conversionState, out num2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["cda"], num2.ToString());
			}
		}

		private static void GetConversationProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			int num = -1;
			if (ActivityConverter.TryGetIntegerProperty(logEvent, "co", true, conversionState, out num))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["co"], num.ToString());
				if (num == 1)
				{
					ActivityConverter.GetConversationId(logEvent, true, activityCustomProperties, conversionState);
				}
			}
		}

		private static void GetConversationId(ClientLogEvent logEvent, bool isRequiredProperty, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string text;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "cid", isRequiredProperty, conversionState, out text))
			{
				ConversationId conversationId = null;
				if (ActivityConverter.TryGetStoreConversationId(text, out conversationId) && conversationId != null)
				{
					activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["cid"], conversationId.ToString());
					return;
				}
				activityCustomProperties.Add("BadConversationId", text);
				conversionState.Errors |= ActivityConverter.ConversionErrors.BadConversationId;
			}
		}

		private static bool TryGetIntegerProperty(ClientLogEvent logEvent, string keyName, bool isRequiredProperty, ActivityConverter.ConversionState conversionState, out int propertyValue)
		{
			propertyValue = 0;
			string text;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, keyName, isRequiredProperty, conversionState, out text) && int.TryParse(text, out propertyValue))
			{
				return true;
			}
			ActivityConverter.tracer.TraceError<string, string>(0L, "[ActivityConverter.TryGetIntegerProperty] failed to get integer property for '{0}'; value '{1}' cannot be converted to an integer", keyName, text);
			return false;
		}

		private void AddSessionStartProperties(ClientLogEvent clientLogEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(clientLogEvent, "l", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["l"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(clientLogEvent, "tz", true, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["tz"], value2);
			}
			activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["ip"], this.ipAddress);
			activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["brn"], this.userAgent);
			if (this.mailboxSession.Mailbox != null)
			{
				bool valueOrDefault = this.mailboxSession.Mailbox.GetValueOrDefault<bool>(MailboxSchema.InferenceUserUIReady, false);
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["uir"], valueOrDefault ? "1" : "0");
			}
			if (this.userContext.FeaturesManager != null)
			{
				HashSet<string> enabledFlightedFeatures = this.userContext.FeaturesManager.GetEnabledFlightedFeatures(FlightedFeatureScope.Any);
				string value3 = string.Join<string>(",", enabledFlightedFeatures);
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["fl"], value3);
			}
			int num;
			if (ActivityConverter.TryGetIntegerProperty(clientLogEvent, "uio", true, conversionState, out num))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["uio"], num.ToString());
			}
		}

		private static void GetDestinationFolderProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string idString;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "df", true, conversionState, out idString))
			{
				StoreObjectId storeObjectId = null;
				if (ActivityConverter.TryGetStoreId(idString, BasicTypes.Folder, out storeObjectId))
				{
					activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["df"], storeObjectId.ToString());
				}
			}
		}

		private static void GetSelectedFolderProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "sfn", false, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["sfn"], value);
			}
		}

		private static void GetGroupMailboxProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "gms", false, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["gms"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "gmt", false, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["gmt"], value2);
			}
		}

		private static void GetIsClutterProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			int num;
			if (ActivityConverter.TryGetIntegerProperty(logEvent, "cl", true, conversionState, out num))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["cl"], num.ToString());
			}
		}

		private static void AddClutterExpansionProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "ct", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["ct"], value);
			}
		}

		private static void AddFeatureSurveyProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "fvr", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["fvr"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "fvc", false, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["fvc"], value2);
			}
		}

		private static void GetSurveyResponseProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState, UserContext userContext)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "sc", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["sc"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "sfq", true, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["sfq"], value2);
			}
			string value3;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "ssq", true, conversionState, out value3))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["ssq"], value3);
			}
			string value4;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "sfm", true, conversionState, out value4))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["sfm"], value4);
			}
			string value5;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "ssd", true, conversionState, out value5))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["ssd"], value5);
			}
			string value6;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "ssz", true, conversionState, out value6))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["ssz"], value6);
			}
			string value7;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "sdm", true, conversionState, out value7))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["sdm"], value7);
			}
			string value8;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "sds", true, conversionState, out value8))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["sds"], value8);
			}
			string value9;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "scd", true, conversionState, out value9))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["scd"], value9);
			}
			string value10;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "Bld", true, conversionState, out value10))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["Bld"], value10);
			}
			if (userContext.FeaturesManager != null)
			{
				HashSet<string> enabledFlightedFeatures = userContext.FeaturesManager.GetEnabledFlightedFeatures(FlightedFeatureScope.Any);
				string value11 = string.Join<string>(",", enabledFlightedFeatures);
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["fl"], value11);
			}
		}

		private static void AddInferenceUiDisabledProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "uds", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["uds"], value);
			}
		}

		private static void AddPivotNavigationProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "mp", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["mp"], value);
			}
		}

		private static void AddConversionErrorProperties(Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string text = conversionState.Errors.ToString();
			activityCustomProperties.Add("ConversionErrors", text);
			ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.AddConversionStateProperties] There were errors during conversion: {0}", text);
			if ((conversionState.Errors & ActivityConverter.ConversionErrors.MissingRequiredProperties) == ActivityConverter.ConversionErrors.MissingRequiredProperties)
			{
				string text2 = string.Join(",", conversionState.MissingPropertyList.ToArray());
				activityCustomProperties.Add("MissingProperties", text2);
				ActivityConverter.tracer.TraceError<string>(0L, "[ActivityConverter.AddConversionStateProperties] Missing required properties: {0}", text2);
			}
		}

		private static void GetRequiredItemActivityProperties(ClientLogEvent logEvent, ActivityConverter.PropertySet requiredProperties, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			if ((requiredProperties & ActivityConverter.PropertySet.Delete) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.GetDeleteProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.DestinationFolder) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.GetDestinationFolderProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.IsClutter) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.GetIsClutterProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SelectedFolder) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.GetSelectedFolderProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.GroupMailbox) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.GetGroupMailboxProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.ConversationInfo) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.GetConversationProperties(logEvent, activityCustomProperties, conversionState);
			}
			else
			{
				ActivityConverter.GetConversationId(logEvent, false, activityCustomProperties, conversionState);
			}
			if (conversionState.Errors != ActivityConverter.ConversionErrors.None)
			{
				ActivityConverter.AddConversionErrorProperties(activityCustomProperties, conversionState);
			}
		}

		private static void AddSearchSessionCommonProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "issi", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["issi"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "isss", true, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["isss"], value2);
			}
		}

		private static void AddSearchRequestCommonProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "issi", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["issi"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "isrid", false, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["isrid"], value2);
			}
		}

		private static void AddSearchResultsProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "isrc", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["isrc"], value);
			}
		}

		private static void AddSearchRefinersReceivedProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "isrd", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["isrd"], value);
			}
		}

		private static void AddSearchRequestEndProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "issu", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["issu"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "isqs", true, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["isqs"], value2);
			}
		}

		private static void AddSearchSessionEndProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "issa", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["issa"], value);
			}
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "istsa", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["istsa"], value);
			}
		}

		private static void AddIntroductionPeekProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "ipks", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["ipks"], value);
			}
		}

		private static void AddHelpPanelProperties(ClientLogEvent logEvent, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			string value;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "hlpm", true, conversionState, out value))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["hlpm"], value);
			}
			string value2;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "hlpa", true, conversionState, out value2))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["hlpa"], value2);
			}
			string value3;
			if (ActivityConverter.TryGetClientLogEventProperty(logEvent, "hlpc", true, conversionState, out value3))
			{
				activityCustomProperties.Add(ActivityConverter.reportingLabelsForKeys["hlpc"], value3);
			}
		}

		private void GetRequiredItemlessActivityProperties(ClientLogEvent logEvent, ActivityConverter.PropertySet requiredProperties, Dictionary<string, string> activityCustomProperties, ActivityConverter.ConversionState conversionState)
		{
			if (requiredProperties == ActivityConverter.PropertySet.Logon)
			{
				this.AddSessionStartProperties(logEvent, activityCustomProperties, conversionState);
			}
			if (requiredProperties == ActivityConverter.PropertySet.ClutterExpansion)
			{
				ActivityConverter.AddClutterExpansionProperties(logEvent, activityCustomProperties, conversionState);
			}
			if (requiredProperties == ActivityConverter.PropertySet.FeatureSurvey)
			{
				ActivityConverter.AddFeatureSurveyProperties(logEvent, activityCustomProperties, conversionState);
			}
			if (requiredProperties == ActivityConverter.PropertySet.SurveyResponse)
			{
				ActivityConverter.GetSurveyResponseProperties(logEvent, activityCustomProperties, conversionState, this.userContext);
			}
			if (requiredProperties == ActivityConverter.PropertySet.InferenceUiDisabled)
			{
				ActivityConverter.AddInferenceUiDisabledProperties(logEvent, activityCustomProperties, conversionState);
			}
			if (requiredProperties == ActivityConverter.PropertySet.PivotNavigation)
			{
				ActivityConverter.AddPivotNavigationProperties(logEvent, activityCustomProperties, conversionState);
			}
			if (requiredProperties == ActivityConverter.PropertySet.IntroductionPeek)
			{
				ActivityConverter.AddIntroductionPeekProperties(logEvent, activityCustomProperties, conversionState);
			}
			if (requiredProperties == ActivityConverter.PropertySet.HelpPanel)
			{
				ActivityConverter.AddHelpPanelProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SearchSessionCommon) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.AddSearchSessionCommonProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SearchSessionEnd) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.AddSearchSessionEndProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SearchRequestCommon) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.AddSearchRequestCommonProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SearchResultsReceived) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.AddSearchResultsProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SearchRefinersReceived) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.AddSearchRefinersReceivedProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SearchRequestEnd) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.AddSearchRequestEndProperties(logEvent, activityCustomProperties, conversionState);
			}
			if ((requiredProperties & ActivityConverter.PropertySet.SelectedFolder) != ActivityConverter.PropertySet.None)
			{
				ActivityConverter.GetSelectedFolderProperties(logEvent, activityCustomProperties, conversionState);
			}
			if (conversionState.Errors != ActivityConverter.ConversionErrors.None)
			{
				ActivityConverter.AddConversionErrorProperties(activityCustomProperties, conversionState);
			}
		}

		private long GetNextActivitySequenceNumber()
		{
			return this.userContext.GetNextClientActivitySequenceNumber();
		}

		private Activity CreateErrorActivity(string errorMessage)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("ConversionErrors", errorMessage);
			return new Activity(ActivityId.Error, ClientId.Web, ExDateTime.UtcNow, this.clientSessionId, this.clientVersion, this.GetNextActivitySequenceNumber(), this.mailboxSession, null, null, dictionary);
		}

		internal const string SessionInfoDatapointName = "SessionInfo";

		internal const string MoveToDeletedItems = "MoveToDeletedItems";

		internal const string ConversionErrorsReportingLabel = "ConversionErrors";

		internal const string MissingPropertiesReportingLabel = "MissingProperties";

		internal const string BadItemIdsReportingLabel = "BadItemIds";

		internal const string BadConversationIdReportingLabel = "BadConversationId";

		internal const string ItemIdKeyName = "id";

		internal const string ItemIdArrayKeyName = "ids";

		internal const string DeleteTypeKeyName = "dt";

		public const string SelectedFolderNameKeyName = "sfn";

		public const string GroupMailboxSmtpAddressKeyName = "gms";

		public const string GroupMailboxTypeKeyName = "gmt";

		internal const string DestinationFolderIdKeyName = "df";

		internal const string SessionInfoIPAddressKeyName = "ip";

		internal const string IsClutterKeyName = "cl";

		internal const string IsConversationKeyName = "co";

		internal const string ConversationIdKeyName = "cid";

		internal const string UnreadCountKeyName = "ct";

		internal const string InferenceUiReadyKeyName = "uir";

		internal const string InferenceUiOnKeyName = "uio";

		internal const string FlightsListKeyName = "fl";

		internal const string DeletedByMultiSelectKeyName = "dm";

		internal const string DeletedByDeleteAllKeyName = "cda";

		internal const string FeatureValueResponseKeyName = "fvr";

		public const string FeatureValueCommentsKeyName = "fvc";

		public const string SurveyComponentKeyName = "sc";

		public const string SurveyFirstQuestionKeyName = "sfq";

		public const string SurveySecondQuestionKeyName = "ssq";

		public const string SurveyFeedbackMessageKeyName = "sfm";

		public const string SurveySendKeyName = "ssd";

		public const string SurveySnoozeKeyName = "ssz";

		public const string SurveyDismissKeyName = "sdm";

		public const string SurveyDontShowAgainKeyName = "sds";

		public const string SurveyCustomDataKeyName = "scd";

		public const string ServerVersionKeyName = "Bld";

		internal const string InferenceUiDisabledSourceKeyName = "uds";

		internal const string PivotNavigationDestinationKeyName = "mp";

		private const string SearchSessionId = "issi";

		private const string SearchSessionStatistics = "isss";

		private const string SearchSuccessActivity = "issa";

		private const string SearchTentativeSuccessActivity = "istsa";

		private const string SearchRequestId = "isrid";

		private const string SearchResultsCount = "isrc";

		private const string SearchTriggerType = "istt";

		private const string SearchRefinersData = "isrd";

		private const string IsSearchSuccessful = "issu";

		private const string InstantSearchQueryStatistics = "isqs";

		private const string IntroductionPeekKeyName = "ipks";

		private const string HelpPanelModuleKeyName = "hlpm";

		private const string HelpArticleIdsKeyName = "hlpa";

		private const string HelpArticleClickedKeyName = "hlpc";

		private readonly IMailboxSession mailboxSession;

		private readonly UserContext userContext;

		private readonly string ipAddress;

		private readonly string userAgent;

		private readonly string clientVersion;

		private readonly Guid clientSessionId;

		private static Trace tracer = ExTraceGlobals.ActivityConverterTracer;

		private static Dictionary<ActivityId, ActivityConverter.PropertySet> propertiesForActivities = new Dictionary<ActivityId, ActivityConverter.PropertySet>
		{
			{
				ActivityId.Categorize,
				ActivityConverter.PropertySet.Category
			},
			{
				ActivityId.ClutterGroupOpened,
				ActivityConverter.PropertySet.ClutterExpansion
			},
			{
				ActivityId.Delete,
				ActivityConverter.PropertySet.Delete | ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.FeatureValueResponse,
				ActivityConverter.PropertySet.FeatureSurvey
			},
			{
				ActivityId.Flag,
				ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.FlagCleared,
				ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.FlagComplete,
				ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.InspectorDisplayStart,
				ActivityConverter.PropertySet.IsClutter
			},
			{
				ActivityId.Logon,
				ActivityConverter.PropertySet.Logon
			},
			{
				ActivityId.Move,
				ActivityConverter.PropertySet.DestinationFolder | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.NewMessage,
				ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.Reply,
				ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.ReplyAll,
				ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.Forward,
				ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.ModernGroupsQuickCompose,
				ActivityConverter.PropertySet.GroupMailbox
			},
			{
				ActivityId.ModernGroupsQuickReply,
				ActivityConverter.PropertySet.GroupMailbox
			},
			{
				ActivityId.ModernGroupsConversationSelected,
				ActivityConverter.PropertySet.GroupMailbox
			},
			{
				ActivityId.PivotChange,
				ActivityConverter.PropertySet.PivotNavigation
			},
			{
				ActivityId.ReadingPaneDisplayStart,
				ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.IsClutter | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.ReadingPaneDisplayEnd,
				ActivityConverter.PropertySet.ConversationInfo
			},
			{
				ActivityId.TurnInferenceOff,
				ActivityConverter.PropertySet.InferenceUiDisabled
			},
			{
				ActivityId.MessageSelected,
				ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.MarkAsRead,
				ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.MarkAsUnread,
				ActivityConverter.PropertySet.ConversationInfo | ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.MarkAllItemsAsRead,
				ActivityConverter.PropertySet.SelectedFolder
			},
			{
				ActivityId.MarkMessageAsClutter,
				ActivityConverter.PropertySet.ConversationInfo
			},
			{
				ActivityId.MarkMessageAsNotClutter,
				ActivityConverter.PropertySet.ConversationInfo
			},
			{
				ActivityId.IgnoreConversation,
				ActivityConverter.PropertySet.ConversationInfo
			},
			{
				ActivityId.SearchSessionStart,
				ActivityConverter.PropertySet.SearchSessionCommon
			},
			{
				ActivityId.SearchSessionEnd,
				ActivityConverter.PropertySet.SearchSessionCommon | ActivityConverter.PropertySet.SearchSessionEnd
			},
			{
				ActivityId.SearchRequestStart,
				ActivityConverter.PropertySet.SearchRequestCommon | ActivityConverter.PropertySet.SearchRequestStart
			},
			{
				ActivityId.SearchResultsReceived,
				ActivityConverter.PropertySet.SearchRequestCommon | ActivityConverter.PropertySet.SearchResultsReceived
			},
			{
				ActivityId.SearchRefinersReceived,
				ActivityConverter.PropertySet.SearchRequestCommon | ActivityConverter.PropertySet.SearchRefinersReceived
			},
			{
				ActivityId.SearchRequestEnd,
				ActivityConverter.PropertySet.SearchRequestCommon | ActivityConverter.PropertySet.SearchRequestEnd
			},
			{
				ActivityId.IntroductionPeekControllerCreated,
				ActivityConverter.PropertySet.IntroductionPeek
			},
			{
				ActivityId.IntroductionPeekShown,
				ActivityConverter.PropertySet.IntroductionPeek
			},
			{
				ActivityId.IntroductionPeekDismissed,
				ActivityConverter.PropertySet.IntroductionPeek
			},
			{
				ActivityId.IntroductionLearnMoreClicked,
				ActivityConverter.PropertySet.IntroductionPeek
			},
			{
				ActivityId.IntroductionTryFeatureClicked,
				ActivityConverter.PropertySet.IntroductionPeek
			},
			{
				ActivityId.SurveyResponse,
				ActivityConverter.PropertySet.SurveyResponse
			},
			{
				ActivityId.HelpPanelShown,
				ActivityConverter.PropertySet.HelpPanel
			},
			{
				ActivityId.HelpPanelClosed,
				ActivityConverter.PropertySet.HelpPanel
			},
			{
				ActivityId.HelpArticleShown,
				ActivityConverter.PropertySet.HelpPanel
			},
			{
				ActivityId.HelpArticleLinkClicked,
				ActivityConverter.PropertySet.HelpPanel
			}
		};

		private static HashSet<ActivityId> multipleItemActivities = new HashSet<ActivityId>
		{
			ActivityId.Categorize,
			ActivityId.Delete,
			ActivityId.Flag,
			ActivityId.FlagCleared,
			ActivityId.FlagComplete,
			ActivityId.IgnoreConversation,
			ActivityId.MarkAsUnread,
			ActivityId.MarkAsRead,
			ActivityId.MarkMessageAsClutter,
			ActivityId.MarkMessageAsNotClutter,
			ActivityId.Move,
			ActivityId.ReadingPaneDisplayStart,
			ActivityId.ReadingPaneDisplayEnd,
			ActivityId.MessageSelected
		};

		private static HashSet<ActivityId> itemlessActivities = new HashSet<ActivityId>
		{
			ActivityId.ClutterGroupOpened,
			ActivityId.ClutterGroupClosed,
			ActivityId.FeatureValueResponse,
			ActivityId.Logon,
			ActivityId.NewMessage,
			ActivityId.PivotChange,
			ActivityId.TurnInferenceOff,
			ActivityId.TurnInferenceOn,
			ActivityId.SearchSessionStart,
			ActivityId.SearchSessionEnd,
			ActivityId.SearchRequestStart,
			ActivityId.SearchResultsReceived,
			ActivityId.SearchRefinersReceived,
			ActivityId.SearchRequestEnd,
			ActivityId.IntroductionPeekControllerCreated,
			ActivityId.IntroductionPeekShown,
			ActivityId.IntroductionPeekDismissed,
			ActivityId.IntroductionLearnMoreClicked,
			ActivityId.IntroductionTryFeatureClicked,
			ActivityId.SurveyResponse,
			ActivityId.MarkAllItemsAsRead,
			ActivityId.HelpCenterShown,
			ActivityId.HelpPanelCreated,
			ActivityId.HelpPanelShown,
			ActivityId.HelpPanelClosed,
			ActivityId.HelpArticleShown,
			ActivityId.HelpArticleLinkClicked,
			ActivityId.UserPhotoUploaded
		};

		private static Dictionary<string, string> reportingLabelsForKeys = new Dictionary<string, string>
		{
			{
				"cda",
				"DeletedByDeleteAll"
			},
			{
				"dm",
				"DeletedByMultiSelect"
			},
			{
				"dt",
				"DeleteType"
			},
			{
				"df",
				"DestinationFolder"
			},
			{
				"fvc",
				"FeatureComments"
			},
			{
				"fvr",
				"FeatureValue"
			},
			{
				"fl",
				"Flights"
			},
			{
				"uds",
				"InferenceUiDisabled"
			},
			{
				"uio",
				"InferenceUiOnKeyName"
			},
			{
				"uir",
				"InferenceUiReady"
			},
			{
				"cl",
				"IsClutter"
			},
			{
				"co",
				"IsConversation"
			},
			{
				"mp",
				"PivotDestination"
			},
			{
				"brn",
				"Browser"
			},
			{
				"ip",
				"IPAddress"
			},
			{
				"l",
				"Layout"
			},
			{
				"tz",
				"Timezone"
			},
			{
				"ct",
				"UnreadCount"
			},
			{
				"cid",
				"ConversationId"
			},
			{
				"sfn",
				"SelectedFolderName"
			},
			{
				"gms",
				"GroupMailboxSmtpAddress"
			},
			{
				"gmt",
				"GroupMailboxType"
			},
			{
				"issi",
				"SearchSessionId"
			},
			{
				"isrid",
				"SearchRequestId"
			},
			{
				"isrc",
				"SearchResultsCount"
			},
			{
				"istt",
				"SearchTriggerType"
			},
			{
				"isrd",
				"SearchRefinersData"
			},
			{
				"issu",
				"SearchSucessful"
			},
			{
				"isqs",
				"SearchQueryStatistics"
			},
			{
				"ipks",
				"IntroductionPeekSource"
			},
			{
				"sc",
				"SurveyComponent"
			},
			{
				"sfq",
				"FirstQuestion"
			},
			{
				"ssq",
				"SecondQuestion"
			},
			{
				"sfm",
				"FeedbackMessage"
			},
			{
				"ssd",
				"Send"
			},
			{
				"ssz",
				"Snooze"
			},
			{
				"sdm",
				"Dismiss"
			},
			{
				"sds",
				"DontShowAgain"
			},
			{
				"scd",
				"SurveyCustomData"
			},
			{
				"hlpm",
				"HelpPanelModule"
			},
			{
				"hlpa",
				"HelpArticleIds"
			},
			{
				"hlpc",
				"HelpArticleClicked"
			},
			{
				"Bld",
				"ServerVersion"
			},
			{
				"isss",
				"SearchSessionStatistics"
			},
			{
				"issa",
				"SearchSuccess"
			},
			{
				"istsa",
				"SearchTentativeSuccess"
			}
		};

		[Flags]
		private enum PropertySet
		{
			None = 0,
			Logon = 1,
			Delete = 2,
			ConversationInfo = 4,
			DestinationFolder = 8,
			Category = 16,
			IsClutter = 32,
			ClutterExpansion = 64,
			PivotNavigation = 128,
			InferenceUiDisabled = 256,
			FeatureSurvey = 512,
			SelectedFolder = 1024,
			SearchSessionCommon = 2048,
			SearchRequestCommon = 4096,
			SearchRequestStart = 8192,
			SearchResultsReceived = 16384,
			SearchRefinersReceived = 32768,
			SearchRequestEnd = 65536,
			IntroductionPeek = 131072,
			SurveyResponse = 262144,
			GroupMailbox = 524288,
			HelpPanel = 1048576,
			SearchSessionEnd = 2097152
		}

		[Flags]
		private enum ConversionErrors
		{
			None = 0,
			BadActivityId = 1,
			BadTimestamp = 2,
			BadItemIds = 4,
			MissingRequiredProperties = 8,
			BadConversationId = 16
		}

		private class ConversionState
		{
			public ActivityConverter.ConversionErrors Errors { get; set; }

			public List<string> MissingPropertyList { get; private set; }

			public void AddMissingProperty(string propertyName)
			{
				if (this.MissingPropertyList == null)
				{
					this.MissingPropertyList = new List<string>();
				}
				this.MissingPropertyList.Add(propertyName);
				this.Errors |= ActivityConverter.ConversionErrors.MissingRequiredProperties;
			}
		}
	}
}

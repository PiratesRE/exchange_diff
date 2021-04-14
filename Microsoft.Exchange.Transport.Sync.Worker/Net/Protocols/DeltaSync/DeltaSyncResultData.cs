using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder;
using Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SendResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessResponse;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncResultData
	{
		internal DeltaSyncResultData(Sync syncResponse) : this(syncResponse.Status, syncResponse.Fault.Faultcode, syncResponse.Fault.Faultstring, syncResponse.Fault.Detail)
		{
			this.syncResponse = syncResponse;
		}

		internal DeltaSyncResultData(Send sendResponse) : this(sendResponse.Status, sendResponse.Fault.Faultcode, sendResponse.Fault.Faultstring, sendResponse.Fault.Detail)
		{
			this.sendResponse = sendResponse;
		}

		internal DeltaSyncResultData(Settings settingsResponse) : this(settingsResponse.Status, settingsResponse.Fault.Faultcode, settingsResponse.Fault.Faultstring, settingsResponse.Fault.Detail)
		{
			this.settingsResponse = settingsResponse;
		}

		internal DeltaSyncResultData(ItemOperations itemOperationsResponse) : this(itemOperationsResponse.Status, itemOperationsResponse.Fault.Faultcode, itemOperationsResponse.Fault.Faultstring, itemOperationsResponse.Fault.Detail)
		{
			this.itemOperationsResponse = itemOperationsResponse;
		}

		internal DeltaSyncResultData(Stateless statelessResponse) : this(statelessResponse.Status, (statelessResponse.Fault != null) ? statelessResponse.Fault.Faultcode : string.Empty, (statelessResponse.Fault != null) ? statelessResponse.Fault.Faultstring : string.Empty, (statelessResponse.Fault != null) ? statelessResponse.Fault.Detail : string.Empty)
		{
			this.stateLessResponse = statelessResponse;
		}

		private DeltaSyncResultData(int topLevelStatusCode, string faultCode, string faultString, string faultDetail)
		{
			this.topLevelStatusCode = topLevelStatusCode;
			this.faultCode = faultCode;
			this.faultString = faultString;
			this.faultDetail = faultDetail;
		}

		internal bool IsTopLevelOperationSuccessful
		{
			get
			{
				return this.topLevelStatusCode == 1;
			}
		}

		internal bool IsAuthenticationError
		{
			get
			{
				return this.topLevelStatusCode == 3204;
			}
		}

		internal int TopLevelStatusCode
		{
			get
			{
				return this.topLevelStatusCode;
			}
		}

		internal Sync SyncResponse
		{
			get
			{
				return this.syncResponse;
			}
		}

		internal Send SendResponse
		{
			get
			{
				return this.sendResponse;
			}
		}

		internal Settings SettingsResponse
		{
			get
			{
				return this.settingsResponse;
			}
		}

		internal ItemOperations ItemOperationsResponse
		{
			get
			{
				return this.itemOperationsResponse;
			}
		}

		internal Stateless StatelessResponse
		{
			get
			{
				return this.stateLessResponse;
			}
		}

		internal string FaultCode
		{
			get
			{
				return this.faultCode;
			}
		}

		internal string FaultString
		{
			get
			{
				return this.faultString;
			}
		}

		internal string FaultDetail
		{
			get
			{
				return this.faultDetail;
			}
		}

		internal static bool TryGetFolderEmailCollections(Sync syncResponse, out Collection folderCollection, out Collection emailCollection, out Exception exception)
		{
			folderCollection = null;
			emailCollection = null;
			exception = null;
			foreach (object obj in syncResponse.Collections.CollectionCollection)
			{
				Collection collection = (Collection)obj;
				if (collection.Class != null)
				{
					if (collection.Class.Equals(DeltaSyncCommon.FolderCollectionName, StringComparison.OrdinalIgnoreCase))
					{
						if (collection.internalStatusSpecified)
						{
							exception = DeltaSyncResultData.GetStatusException(collection.Status);
							folderCollection = collection;
						}
					}
					else if (collection.Class.Equals(DeltaSyncCommon.EmailCollectionName, StringComparison.OrdinalIgnoreCase) && collection.internalStatusSpecified)
					{
						exception = DeltaSyncResultData.GetStatusException(collection.Status);
						emailCollection = collection;
					}
					if (exception != null)
					{
						return false;
					}
				}
			}
			if (folderCollection != null && folderCollection.SyncKey != null && emailCollection != null && emailCollection.SyncKey != null)
			{
				return true;
			}
			exception = new InvalidServerResponseException();
			return false;
		}

		internal static bool TryGetSettings(Settings settingsResponse, out DeltaSyncSettings deltaSyncSettings, out Exception exception)
		{
			deltaSyncSettings = null;
			exception = null;
			int status;
			if (settingsResponse.ServiceSettings.Properties.Status == 1)
			{
				if (settingsResponse.AccountSettings.Status == 1)
				{
					deltaSyncSettings = new DeltaSyncSettings(settingsResponse.ServiceSettings.Properties.Get, settingsResponse.AccountSettings.Get.Properties);
					return true;
				}
				status = settingsResponse.AccountSettings.Status;
			}
			else
			{
				status = settingsResponse.ServiceSettings.Properties.Status;
			}
			exception = DeltaSyncResultData.GetStatusException(status);
			return false;
		}

		internal static bool TryGetMessageStream(ItemOperations fetchResponse, out Stream messageStream, out Exception exception)
		{
			messageStream = null;
			exception = null;
			if (fetchResponse.Responses.Fetch.internalStatusSpecified)
			{
				if (fetchResponse.Responses.Fetch.Status != 1)
				{
					exception = DeltaSyncResultData.GetStatusException(fetchResponse.Responses.Fetch.Status);
					return false;
				}
				messageStream = fetchResponse.Responses.Fetch.Message.EmailMessage;
			}
			if (messageStream != null)
			{
				return true;
			}
			exception = new InvalidServerResponseException();
			return false;
		}

		internal static Exception GetStatusException(int statusCode)
		{
			if (statusCode == 1)
			{
				return null;
			}
			if (DeltaSyncResultData.ArgumentInRange(statusCode, 3100, 3199))
			{
				return new PartnerAuthenticationException(statusCode);
			}
			if (DeltaSyncResultData.ArgumentInRange(statusCode, 3200, 3299))
			{
				return new UserAccessException(statusCode);
			}
			if (DeltaSyncResultData.ArgumentInRange(statusCode, 4100, 4199))
			{
				return new RequestFormatException(statusCode);
			}
			if (DeltaSyncResultData.ArgumentInRange(statusCode, 4200, 4299))
			{
				return new RequestContentException(statusCode);
			}
			if (DeltaSyncResultData.ArgumentInRange(statusCode, 4300, 4399))
			{
				return new SettingsViolationException(statusCode);
			}
			if (DeltaSyncResultData.ArgumentInRange(statusCode, 4400, 4499))
			{
				return new DataOutOfSyncException(statusCode);
			}
			if (DeltaSyncResultData.ArgumentInRange(statusCode, 5000, 5999))
			{
				return new DeltaSyncServerException(statusCode);
			}
			return new UnknownDeltaSyncException(statusCode);
		}

		internal static string DecodeValue(Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder.DisplayName displayName)
		{
			if (displayName.encoding != null && displayName.encoding.Equals("2"))
			{
				byte[] bytes = Convert.FromBase64String(displayName.Value);
				return Encoding.UTF8.GetString(bytes);
			}
			return displayName.Value;
		}

		internal Exception GetStatusException()
		{
			return DeltaSyncResultData.GetStatusException(this.topLevelStatusCode);
		}

		private static bool ArgumentInRange(int arg, int lowerLimit, int upperLimit)
		{
			return arg >= lowerLimit && arg <= upperLimit;
		}

		private readonly int topLevelStatusCode;

		private readonly Sync syncResponse;

		private readonly Send sendResponse;

		private readonly Settings settingsResponse;

		private readonly ItemOperations itemOperationsResponse;

		private readonly Stateless stateLessResponse;

		private readonly string faultCode;

		private readonly string faultString;

		private readonly string faultDetail;
	}
}

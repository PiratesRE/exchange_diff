using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class GetSyncStateHandler : ExchangeDiagnosableWrapper<GetSyncStateResult>
	{
		protected override string UsageText
		{
			get
			{
				return "This diagnostics handler returns sync state metadata and blob data for a given mailbox. The handler supports \"EmailAddress\", \"DeviceID\", \"DeviceType\", \"SyncState\" and \"FidMapping\" arguments. Below are examples for using this diagnostics handler:\r\n\r\n";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: Return all metadata for a given mailbox\r\nGet-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetSyncState -Argument \"EmailAddress=jondoe@contoso.com\"\r\n\r\nExample 2: Return all metadata for a given device\r\nGet-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetSyncState -Argument \"EmailAddress=jondoe@contoso.com;DeviceId=WP123;DeviceType=WP8\"\r\n\r\nExample 3: Return a particular named SyncState for a given device\r\nGet-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetSyncState -Argument \"EmailAddress=jondoe@contoso.com;DeviceId=WP123;DeviceType=WP8;SyncState=5\"\r\n\r\nExample 4: Return all sync states for a given device\r\nGet-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetSyncState -Argument \"EmailAddress=jondoe@contoso.com;DeviceId=WP123;DeviceType=WP8\"\r\n\r\nExample 5: Return the fid mapping for a given device\r\nGet-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GetSyncState -Argument \"EmailAddress=jondoe@contoso.com;deviceId=WP123;DeviceType=WP8;FidMapping\"\r\n\r\nNOTE:\r\n1. EmailAddress is always required.\r\n2. DeviceId and DeviceType MUST be used together.\r\n3. If SyncState is supplied, DeviceId and DeviceType must also be supplied.\r\n4. The actual blob and size is ONLY returned when fetching a named sync state.\r\n5. FidMapping and SyncStateName should be mutually exclusive.  Both require DeviceId and DeviceType";
			}
		}

		public static GetSyncStateHandler GetInstance()
		{
			if (GetSyncStateHandler.instance == null)
			{
				lock (GetSyncStateHandler.lockObject)
				{
					if (GetSyncStateHandler.instance == null)
					{
						GetSyncStateHandler.instance = new GetSyncStateHandler();
					}
				}
			}
			return GetSyncStateHandler.instance;
		}

		private GetSyncStateHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "GetSyncState";
			}
		}

		internal override GetSyncStateResult GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			ParsedCallData parsedCallData = this.ParseCallData(arguments.Argument);
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromProxyAddress(ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(parsedCallData.Mailbox.Domain), parsedCallData.Mailbox.ToString(), RemotingOptions.AllowCrossSite);
			GetSyncStateResult result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.CurrentCulture, "Client=ActiveSync"))
			{
				GetSyncStateResult data = SyncStateDiagnostics.GetData(mailboxSession, parsedCallData);
				if (parsedCallData.FidMapping)
				{
					this.FillFidMapping(data, mailboxSession);
				}
				result = data;
			}
			return result;
		}

		private void FillFidMapping(GetSyncStateResult results, MailboxSession session)
		{
			foreach (DeviceData deviceData in results.Devices)
			{
				foreach (SyncStateFolderData syncStateFolderData in deviceData.SyncFolders)
				{
					if (syncStateFolderData != null && !string.IsNullOrEmpty(syncStateFolderData.SyncStateBlob))
					{
						syncStateFolderData.FolderMapping = new List<FolderMappingData>();
						using (PooledMemoryStream pooledMemoryStream = new PooledMemoryStream(102400))
						{
							byte[] array = Convert.FromBase64String(syncStateFolderData.SyncStateBlob);
							pooledMemoryStream.Write(array, 0, array.Length);
							pooledMemoryStream.Flush();
							pooledMemoryStream.Position = 0L;
							int num;
							int num2;
							long num3;
							long num4;
							Dictionary<string, bool> dictionary;
							GenericDictionaryData<ConstStringData, string, DerivedData<ICustomSerializableBuilder>> genericDictionaryData = SyncState.InternalDeserializeData(pooledMemoryStream, out num, out num2, out num3, out num4, out dictionary);
							FolderIdMapping folderIdMapping = genericDictionaryData.Data["IdMapping"].Data as FolderIdMapping;
							IDictionaryEnumerator syncIdIdEnumerator = folderIdMapping.SyncIdIdEnumerator;
							while (syncIdIdEnumerator.MoveNext())
							{
								string shortId = syncIdIdEnumerator.Key as string;
								ISyncItemId syncItemId = syncIdIdEnumerator.Value as ISyncItemId;
								StoreObjectId storeObjectId = syncItemId.NativeId as StoreObjectId;
								try
								{
									using (Folder folder = Folder.Bind(session, storeObjectId, new PropertyDefinition[]
									{
										FolderSchema.DisplayName
									}))
									{
										DefaultFolderType defaultFolderType = session.IsDefaultFolderType(folder.Id);
										syncStateFolderData.FolderMapping.Add(new FolderMappingData
										{
											ShortId = shortId,
											LongId = storeObjectId.ToString(),
											Name = folder.DisplayName,
											DefaultFolderType = defaultFolderType.ToString(),
											Exception = null
										});
									}
								}
								catch (Exception ex)
								{
									syncStateFolderData.FolderMapping.Add(new FolderMappingData
									{
										ShortId = shortId,
										LongId = "[Error]",
										Name = "[Error]",
										DefaultFolderType = "[Error]",
										Exception = ex.ToString()
									});
								}
							}
						}
					}
				}
			}
		}

		private ParsedCallData ParseCallData(string arguments)
		{
			ParsedCallData parsedCallData = new ParsedCallData();
			parsedCallData.Metadata = true;
			string text = arguments.ToLower().Trim();
			string[] array = text.Split(new char[]
			{
				';'
			});
			foreach (string text2 in array)
			{
				if (text2.StartsWith("emailaddress="))
				{
					parsedCallData.Mailbox = SmtpAddress.Parse(text2.Substring("emailaddress=".Length));
				}
				else if (text2.StartsWith("deviceid="))
				{
					parsedCallData.Metadata = false;
					parsedCallData.DeviceId = text2.Substring("deviceid=".Length);
				}
				else if (text2.StartsWith("devicetype="))
				{
					parsedCallData.Metadata = false;
					parsedCallData.DeviceType = text2.Substring("devicetype=".Length);
				}
				else if (text2.StartsWith("syncstate="))
				{
					parsedCallData.Metadata = false;
					parsedCallData.SyncStateName = text2.Substring("syncstate=".Length);
				}
				else if (text2.StartsWith("fidmapping"))
				{
					parsedCallData.Metadata = false;
					parsedCallData.FidMapping = true;
					parsedCallData.SyncStateName = "FolderIdMapping";
				}
			}
			if (parsedCallData.Mailbox == SmtpAddress.Empty)
			{
				throw new ArgumentException(string.Format("{0} argument MUST be specified and must be a valid Smtp address.", "emailaddress="));
			}
			bool flag = string.IsNullOrEmpty(parsedCallData.DeviceId);
			bool flag2 = string.IsNullOrEmpty(parsedCallData.DeviceType);
			if (flag != flag2)
			{
				throw new ArgumentException(string.Format("{0} and {1} arguments must be both present.", "deviceid=", "devicetype="));
			}
			if ((!string.IsNullOrEmpty(parsedCallData.SyncStateName) || parsedCallData.FidMapping) && flag)
			{
				throw new ArgumentException(string.Format("If {0} or {1} is specified, then {2} and {3} must also be specified", new object[]
				{
					"syncstate=",
					"fidmapping",
					"deviceid=",
					"devicetype="
				}));
			}
			return parsedCallData;
		}

		private const string DeviceIdArgument = "deviceid=";

		private const string DeviceTypeArgument = "devicetype=";

		private const string SyncStateNameArgument = "syncstate=";

		private const string FidMappingArgument = "fidmapping";

		private static GetSyncStateHandler instance;

		private static object lockObject = new object();
	}
}

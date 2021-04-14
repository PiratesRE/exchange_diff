using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Activity
	{
		internal Activity(MemoryPropertyBag propertyBag)
		{
			Util.ThrowOnNullArgument(propertyBag, "propertyBag");
			this.propertyBag = propertyBag;
			this.propertyBag.SetAllPropertiesLoaded();
		}

		public Activity(ActivityId id, ClientId clientId, ExDateTime timeStamp, Guid clientSessionId, string clientVersion, long sequenceNumber, IMailboxSession mailboxSession, StoreObjectId itemId = null, StoreObjectId previousItemId = null, IDictionary<string, string> customProperties = null) : this(new MemoryPropertyBag())
		{
			ArgumentValidator.ThrowIfNull("clientId", clientId);
			ArgumentValidator.ThrowIfNullOrEmpty("clientVersion", clientVersion);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			EnumValidator<ActivityId>.ThrowIfInvalid(id);
			if (id == ActivityId.Min)
			{
				throw new ArgumentException("The value supplied for ActivityId is invalid (ActivityId.Min)");
			}
			if (clientId == ClientId.Min)
			{
				throw new ArgumentException("The value is not a valid ClientId (ClientId.Min)");
			}
			if (!timeStamp.HasTimeZone)
			{
				throw new ArgumentException("Timestamp has unspecified timezone: " + Activity.BuildDiagnosticString(id, clientId, timeStamp, clientSessionId, clientVersion, sequenceNumber, mailboxSession, itemId, previousItemId, customProperties));
			}
			this.Id = id;
			this.ClientId = clientId;
			this.TimeStamp = timeStamp.ToUtc();
			this.ClientSessionId = clientSessionId;
			this.SequenceNumber = sequenceNumber;
			this.ClientVersion = clientVersion;
			this.MailboxGuid = mailboxSession.MailboxGuid;
			IUserPrincipal userPrincipal = mailboxSession.MailboxOwner as IUserPrincipal;
			this.NetId = ((userPrincipal != null) ? userPrincipal.NetId : null);
			if (mailboxSession.Capabilities != null && mailboxSession.Capabilities.CanHaveCulture && mailboxSession.PreferedCulture != null)
			{
				this.LocaleName = mailboxSession.PreferedCulture.Name;
				this.LocaleId = Activity.GetLcidFromMailboxSession(mailboxSession);
			}
			IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
			if (mailboxOwner != null)
			{
				if (mailboxOwner.MailboxInfo.OrganizationId != null && mailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit != null)
				{
					this.TenantName = mailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit.Name;
				}
				this.mailboxType = new long?((long)mailboxOwner.RecipientTypeDetails);
			}
			if (itemId != null)
			{
				this.ItemId = itemId;
			}
			if (previousItemId != null)
			{
				this.PreviousItemId = previousItemId;
			}
			this.CustomPropertiesDictionary = (customProperties ?? new Dictionary<string, string>());
			if (clientId.Equals(ClientId.Other, true))
			{
				this.customPropertiesDictionary["ClientString"] = mailboxSession.ClientInfoString;
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null && !string.IsNullOrEmpty(currentActivityScope.ClientInfo))
				{
					this.customPropertiesDictionary["ActivityScopeClientInfo"] = currentActivityScope.ClientInfo;
				}
			}
			this.activityCreationTime = ExDateTime.UtcNow;
		}

		public ActivityId Id
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<ActivityId>(ActivitySchema.ActivityId, ActivityId.Min);
			}
			private set
			{
				this.propertyBag.SetProperty(ActivitySchema.ActivityId, value);
			}
		}

		public ClientId ClientId
		{
			get
			{
				object obj = this.propertyBag.TryGetProperty(ActivitySchema.ClientId);
				if (PropertyError.IsPropertyError(obj))
				{
					return ClientId.Min;
				}
				ClientId clientId = ClientId.FromInt((int)obj);
				if (clientId == null)
				{
					return ClientId.Min;
				}
				return clientId;
			}
			private set
			{
				this.propertyBag.SetProperty(ActivitySchema.ClientId, value.ToInt());
			}
		}

		public ExDateTime TimeStamp
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<ExDateTime>(ActivitySchema.TimeStamp, ExDateTime.MinValue);
			}
			private set
			{
				this.propertyBag.SetProperty(ActivitySchema.TimeStamp, value);
			}
		}

		public StoreObjectId ItemId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<StoreObjectId>(ActivitySchema.ItemId, null);
			}
			internal set
			{
				this.propertyBag.SetProperty(ActivitySchema.ItemId, value);
			}
		}

		public StoreObjectId PreviousItemId
		{
			get
			{
				byte[] valueOrDefault = this.propertyBag.GetValueOrDefault<byte[]>(ActivitySchema.PreviousItemId, null);
				if (valueOrDefault != null)
				{
					return StoreObjectId.Deserialize(valueOrDefault);
				}
				return null;
			}
			private set
			{
				ArgumentValidator.ThrowIfNull("PreviousItemId", value);
				this.propertyBag.SetProperty(ActivitySchema.PreviousItemId, value.GetBytes());
			}
		}

		public int LocaleId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<int>(ActivitySchema.LocaleId, int.MinValue);
			}
			private set
			{
				this.propertyBag.SetProperty(ActivitySchema.LocaleId, value);
			}
		}

		public Guid ClientSessionId
		{
			get
			{
				return this.propertyBag.GetValueOrDefault<Guid>(ActivitySchema.SessionId, Guid.Empty);
			}
			private set
			{
				this.propertyBag.SetProperty(ActivitySchema.SessionId, value);
			}
		}

		public string ClientVersion { get; private set; }

		public string LocaleName { get; private set; }

		public long SequenceNumber { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public string TenantName { get; private set; }

		public NetID NetId { get; private set; }

		public ExDateTime ActivityCreationTime
		{
			get
			{
				return this.activityCreationTime;
			}
		}

		public long? MailboxType
		{
			get
			{
				return this.mailboxType;
			}
		}

		public string CustomPropertiesString
		{
			get
			{
				if (this.customPropertiesString == null)
				{
					this.customPropertiesString = Activity.SerializeDictionaryAsString(this.CustomPropertiesDictionary, this.isCustomPropertiesDictionaryTruncated);
				}
				return this.customPropertiesString;
			}
		}

		public override string ToString()
		{
			return Activity.BuildDiagnosticString(this.Id, this.ClientId, this.TimeStamp, this.ClientSessionId, this.ClientVersion, this.SequenceNumber, null, this.ItemId, this.PreviousItemId, this.CustomPropertiesDictionary);
		}

		private IDictionary<string, string> CustomPropertiesDictionary
		{
			get
			{
				if (this.customPropertiesDictionary == null)
				{
					this.customPropertiesDictionary = this.DeserializeCustomPropertiesDictionary(out this.isCustomPropertiesDictionaryTruncated);
				}
				return this.customPropertiesDictionary;
			}
			set
			{
				this.SerializeCustomPropertiesDictionary(value, out this.isCustomPropertiesDictionaryTruncated);
				this.customPropertiesDictionary = value;
			}
		}

		public bool TryGetCustomProperty(string propertyName, out string value)
		{
			return this.CustomPropertiesDictionary.TryGetValue(propertyName, out value);
		}

		internal bool TryGetSchemaProperty(PropertyDefinition propertyDefinition, out object value)
		{
			value = this.propertyBag.TryGetProperty(propertyDefinition);
			return !(value is PropertyError);
		}

		internal static IMessage CreateMessageAdapter(Action<Activity> saveAction)
		{
			Util.ThrowOnNullArgument(saveAction, "saveAction");
			return new ActivityMessageAdapter(delegate(MemoryPropertyBag propertyBag)
			{
				saveAction(new Activity(propertyBag));
			});
		}

		internal IMessage CreateMessageAdapter()
		{
			return new ActivityMessageAdapter(this.propertyBag);
		}

		internal static string SerializeDictionaryAsString(IDictionary<string, string> dictionary)
		{
			return Activity.SerializeDictionaryAsString(dictionary, false);
		}

		internal static string SerializeDictionaryAsString(IDictionary<string, string> dictionary, bool isDictionaryTruncated)
		{
			if (dictionary == null)
			{
				return string.Empty;
			}
			bool flag = true;
			bool flag2 = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder2.Append("&");
				}
				stringBuilder2.Append(Activity.EscapeString(keyValuePair.Key));
				stringBuilder2.Append("=");
				stringBuilder2.Append(Activity.EscapeString(keyValuePair.Value));
				if (stringBuilder.Length + stringBuilder2.Length > 4096)
				{
					flag2 = true;
					break;
				}
				stringBuilder.Append(stringBuilder2);
			}
			if (flag2 || isDictionaryTruncated)
			{
				stringBuilder.Append("&");
			}
			return stringBuilder.ToString();
		}

		internal static IDictionary<string, string> DeserializeDictionaryFromString(string serializedString)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(serializedString))
			{
				string[] array = serializedString.Split(new char[]
				{
					'&'
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						'='
					});
					if (array3.Length == 2)
					{
						dictionary.Add(Activity.UnescapeString(array3[0]), Activity.UnescapeString(array3[1]));
					}
				}
			}
			return dictionary;
		}

		internal static string EscapeString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in str)
			{
				char c2 = c;
				switch (c2)
				{
				case '%':
					stringBuilder.Append("%25");
					break;
				case '&':
					stringBuilder.Append("%26");
					break;
				default:
					if (c2 == '=')
					{
						stringBuilder.Append("%3d");
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				}
			}
			return stringBuilder.ToString();
		}

		internal static string UnescapeString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			return str.Replace("%3d", "=").Replace("%26", "&").Replace("%25", "%");
		}

		private void SerializeCustomPropertiesDictionary(IDictionary<string, string> data, out bool isTruncatedResult)
		{
			if (data == null || data.Count == 0)
			{
				isTruncatedResult = false;
				return;
			}
			AbstractCustomPropertySerializer serializer = CustomPropertySerializerFactory.GetSerializer();
			if (serializer == null)
			{
				isTruncatedResult = false;
				return;
			}
			byte[] array = serializer.Serialize(data, out isTruncatedResult);
			if (array != null)
			{
				this.propertyBag.SetProperty(ActivitySchema.CustomProperties, array);
			}
		}

		private Dictionary<string, string> DeserializeCustomPropertiesDictionary(out bool isTruncatedResult)
		{
			byte[] valueOrDefault = this.propertyBag.GetValueOrDefault<byte[]>(ActivitySchema.CustomProperties, null);
			if (valueOrDefault == null || valueOrDefault.Length == 0)
			{
				isTruncatedResult = false;
				return new Dictionary<string, string>();
			}
			AbstractCustomPropertySerializer deserializer = CustomPropertySerializerFactory.GetDeserializer(valueOrDefault);
			if (deserializer == null)
			{
				isTruncatedResult = false;
				return new Dictionary<string, string>();
			}
			return deserializer.Deserialize(valueOrDefault, out isTruncatedResult);
		}

		private static string BuildDiagnosticString(ActivityId id, ClientId clientId, ExDateTime timeStamp, Guid clientSessionId, string clientVersion, long sequenceNumber, IMailboxSession mailboxSession, StoreObjectId itemId = null, StoreObjectId previousItemId = null, IDictionary<string, string> customProperties = null)
		{
			string text = string.Empty;
			if (mailboxSession != null && mailboxSession.MailboxOwner != null)
			{
				text = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			}
			return string.Format("id:{0}, clientId:{1}, timeStamp:{2}, clientSessionId:{3}, clientVersion:{4}, sequenceNumber:{5}, owner:{6}, itemId:{7}, previousItemId:{8}, customProperties:{9}", new object[]
			{
				id,
				(clientId == null) ? string.Empty : clientId.ToString(),
				timeStamp.ToISOString(),
				clientSessionId,
				clientVersion,
				sequenceNumber,
				text,
				(itemId == null) ? string.Empty : itemId.ToBase64ProviderLevelItemId(),
				(previousItemId == null) ? string.Empty : previousItemId.ToBase64ProviderLevelItemId(),
				(customProperties == null) ? string.Empty : Activity.SerializeDictionaryAsString(customProperties)
			});
		}

		private static int GetLcidFromMailboxSession(IMailboxSession mailboxSession)
		{
			if (mailboxSession != null && mailboxSession.PreferedCulture != null)
			{
				return mailboxSession.PreferedCulture.LCID;
			}
			return 0;
		}

		internal const int MaxCustomPropertiesCharacterCount = 4096;

		private readonly MemoryPropertyBag propertyBag;

		private readonly ExDateTime activityCreationTime;

		private readonly long? mailboxType;

		private IDictionary<string, string> customPropertiesDictionary;

		private string customPropertiesString;

		private bool isCustomPropertiesDictionaryTruncated;
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class AuditEvent : IAuditEvent
	{
		public AuditEvent(MailboxSession session, MailboxAuditOperations operation, COWSettings settings, OperationResult result, LogonType logonType, bool externalAccess)
		{
			EnumValidator.ThrowIfInvalid<MailboxAuditOperations>(operation);
			EnumValidator.ThrowIfInvalid<OperationResult>(result, "result");
			EnumValidator.ThrowIfInvalid<LogonType>(logonType, "logonType");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(settings, "settings");
			this.MailboxSession = session;
			this.AuditOperation = operation;
			this.COWSettings = settings;
			this.OperationSucceeded = result;
			this.LogonType = logonType;
			this.ExternalAccess = externalAccess;
			this.CreationTime = DateTime.UtcNow;
			this.RecordId = CombGuidGenerator.NewGuid(this.CreationTime);
			this.OrganizationId = (string.IsNullOrEmpty(session.OrganizationId.ToString()) ? "First Org" : session.OrganizationId.ToString());
			this.MailboxGuid = session.MailboxGuid;
			this.OperationName = operation.ToString();
			this.LogonTypeName = logonType.ToString();
		}

		internal MailboxSession MailboxSession { get; private set; }

		internal MailboxAuditOperations AuditOperation { get; private set; }

		private protected COWSettings COWSettings { protected get; private set; }

		internal LogonType LogonType { get; private set; }

		public DateTime CreationTime { get; private set; }

		public Guid RecordId { get; private set; }

		public string OrganizationId { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public string OperationName { get; private set; }

		public string LogonTypeName { get; private set; }

		public OperationResult OperationSucceeded { get; private set; }

		public bool ExternalAccess { get; private set; }

		public IAuditLogRecord GetLogRecord()
		{
			return new AuditEvent.AuditLogRecord(this);
		}

		protected virtual IEnumerable<KeyValuePair<string, string>> InternalGetEventDetails()
		{
			yield return new KeyValuePair<string, string>("Operation", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				this.AuditOperation
			}));
			yield return new KeyValuePair<string, string>("OperationResult", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				this.OperationSucceeded
			}));
			yield return new KeyValuePair<string, string>("LogonType", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				this.AuditScopeFromLogonType(this.LogonType)
			}));
			yield return new KeyValuePair<string, string>("ExternalAccess", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				this.ExternalAccess
			}));
			yield return new KeyValuePair<string, string>("UtcTime", this.CreationTime.ToString("s"));
			yield return new KeyValuePair<string, string>("InternalLogonType", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				this.MailboxSession.LogonType
			}));
			yield return new KeyValuePair<string, string>("MailboxGuid", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				this.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid
			}));
			yield return new KeyValuePair<string, string>("MailboxOwnerUPN", this.MailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
			if (this.MailboxSession.MailboxOwner.Sid != null)
			{
				yield return new KeyValuePair<string, string>("MailboxOwnerSid", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					this.MailboxSession.MailboxOwner.Sid
				}));
				if (this.MailboxSession.MailboxOwner.MasterAccountSid != null)
				{
					yield return new KeyValuePair<string, string>("MailboxOwnerMasterAccountSid", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						this.MailboxSession.MailboxOwner.MasterAccountSid
					}));
				}
			}
			IdentityPair pair = this.GetUserIdentityPair();
			if (pair.LogonUserSid != null)
			{
				yield return new KeyValuePair<string, string>("LogonUserSid", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					pair.LogonUserSid
				}));
			}
			if (pair.LogonUserDisplayName != null)
			{
				yield return new KeyValuePair<string, string>("LogonUserDisplayName", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					pair.LogonUserDisplayName
				}));
			}
			string clientInfoString = (this.MailboxSession.RemoteClientSessionInfo == null) ? this.MailboxSession.ClientInfoString : this.MailboxSession.RemoteClientSessionInfo.ClientInfoString;
			if (!string.IsNullOrEmpty(clientInfoString))
			{
				yield return new KeyValuePair<string, string>("ClientInfoString", clientInfoString);
			}
			yield return new KeyValuePair<string, string>("ClientIPAddress", string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				this.MailboxSession.ClientIPAddress
			}));
			if (!string.IsNullOrEmpty(this.MailboxSession.ClientMachineName))
			{
				yield return new KeyValuePair<string, string>("ClientMachineName", this.MailboxSession.ClientMachineName);
			}
			if (!string.IsNullOrEmpty(this.MailboxSession.ClientProcessName))
			{
				yield return new KeyValuePair<string, string>("ClientProcessName", this.MailboxSession.ClientProcessName);
			}
			if (this.MailboxSession.ClientVersion != 0L)
			{
				yield return new KeyValuePair<string, string>("ClientVersion", AuditEvent.GetVersionString(this.MailboxSession.ClientVersion));
			}
			yield return new KeyValuePair<string, string>("OriginatingServer", string.Format(CultureInfo.InvariantCulture, "{0} ({1})\r\n", new object[]
			{
				AuditEvent.MachineName,
				"15.00.1497.015"
			}));
			yield break;
		}

		private IdentityPair GetUserIdentityPair()
		{
			IdentityPair result = default(IdentityPair);
			ClientSessionInfo remoteClientSessionInfo = this.MailboxSession.RemoteClientSessionInfo;
			if (remoteClientSessionInfo != null)
			{
				result.LogonUserSid = remoteClientSessionInfo.LogonUserSid;
				result.LogonUserDisplayName = remoteClientSessionInfo.LogonUserDisplayName;
			}
			else
			{
				result = IdentityHelper.GetIdentityPair(this.MailboxSession);
				if (result.LogonUserDisplayName == null && this.LogonType == LogonType.Owner && this.MailboxSession.MailboxOwner.MailboxInfo.IsArchive)
				{
					result.LogonUserDisplayName = AuditEvent.ResolveMailboxOwnerName(this.MailboxSession.MailboxOwner);
				}
			}
			return result;
		}

		private static string ResolveMailboxOwnerName(IExchangePrincipal owner)
		{
			string result;
			if (!string.IsNullOrEmpty(owner.MailboxInfo.DisplayName))
			{
				result = owner.MailboxInfo.DisplayName;
			}
			else if (!string.IsNullOrEmpty(owner.Alias))
			{
				result = owner.Alias;
			}
			else
			{
				result = ((owner.ObjectId == null) ? string.Empty : owner.ObjectId.ToString());
			}
			return result;
		}

		protected string GetCurrentFolderPathName()
		{
			string text = null;
			Folder currentFolder = this.COWSettings.GetCurrentFolder(this.MailboxSession);
			if (currentFolder != null)
			{
				text = (currentFolder.TryGetProperty(FolderSchema.FolderPathName) as string);
				if (text != null)
				{
					text = text.Replace(COWSettings.StoreIdSeparator, '\\');
				}
			}
			return text;
		}

		private static string GetVersionString(long versionNumber)
		{
			ushort num = (ushort)(versionNumber >> 48);
			ushort num2 = (ushort)((versionNumber & 281470681743360L) >> 32);
			ushort num3 = (ushort)((versionNumber & (long)((ulong)-65536)) >> 16);
			ushort num4 = (ushort)(versionNumber & 65535L);
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", new object[]
			{
				num,
				num2,
				num3,
				num4
			});
		}

		private AuditScopes AuditScopeFromLogonType(LogonType logonType)
		{
			switch (logonType)
			{
			case LogonType.Owner:
				return AuditScopes.Owner;
			case LogonType.Admin:
				return AuditScopes.Admin;
			case LogonType.Delegated:
				return AuditScopes.Delegate;
			default:
				throw new ArgumentOutOfRangeException("logonType");
			}
		}

		internal const int InitialCapacityEstimate = 1024;

		private static readonly string MachineName = Environment.MachineName;

		private class AuditLogRecord : IAuditLogRecord
		{
			public AuditLogRecord(AuditEvent eventData)
			{
				this.UserId = eventData.GetUserIdentityPair().LogonUserSid;
				this.CreationTime = eventData.CreationTime;
				this.Operation = eventData.AuditOperation.ToString();
				this.eventDetails = new List<KeyValuePair<string, string>>(eventData.InternalGetEventDetails());
			}

			public AuditLogRecordType RecordType
			{
				get
				{
					return AuditLogRecordType.MailboxAudit;
				}
			}

			public DateTime CreationTime { get; private set; }

			public string Operation { get; private set; }

			public string ObjectId
			{
				get
				{
					return null;
				}
			}

			public string UserId { get; private set; }

			public IEnumerable<KeyValuePair<string, string>> GetDetails()
			{
				return this.eventDetails;
			}

			private List<KeyValuePair<string, string>> eventDetails;
		}
	}
}

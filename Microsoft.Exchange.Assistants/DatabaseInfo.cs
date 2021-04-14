using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class DatabaseInfo : Base, IDatabaseInfo
	{
		internal DatabaseInfo(Guid guid, string databaseName, string databaseLegacyDN, bool isPublic, EventLogger eventLogger, bool zeroBox = true)
		{
			if (zeroBox)
			{
				this.guid = guid;
				this.systemMailboxName = "SystemMailbox{" + this.guid + "}";
				this.databaseName = databaseName;
				this.isPublic = isPublic;
				this.displayName = string.Concat(new object[]
				{
					this.databaseName,
					" (",
					this.guid,
					")"
				});
				if (!this.isPublic)
				{
					Guid empty = Guid.Empty;
					this.systemAttendantMailboxGuid = Guid.Empty;
					this.systemAttendantMailboxPresent = (this.guid == empty);
				}
			}
			else
			{
				this.Initialize(guid, databaseName, databaseLegacyDN, isPublic);
			}
			SingletonEventLogger.GetSingleton(eventLogger.ServiceName);
		}

		internal DatabaseInfo(Guid guid, string databaseName, string databaseLegacyDN, bool isPublic)
		{
			this.Initialize(guid, databaseName, databaseLegacyDN, isPublic);
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public Guid SystemMailboxGuid
		{
			get
			{
				if (this.systemMailboxPrincipal != null)
				{
					return this.systemMailboxPrincipal.MailboxInfo.MailboxGuid;
				}
				return Guid.Empty;
			}
		}

		public bool IsPublic
		{
			get
			{
				return this.isPublic;
			}
		}

		public List<MailboxInformation> GetMailboxTable(ClientType clientType, PropertyTagPropertyDefinition[] properties)
		{
			List<PropTag> list = new List<PropTag>(MailboxTableQuery.RequiredMailboxTableProperties.Length + properties.Length);
			list.AddRange(MailboxTableQuery.RequiredMailboxTableProperties);
			for (int i = 0; i < properties.Length; i++)
			{
				list.Add((PropTag)properties[i].PropertyTag);
			}
			PropValue[][] mailboxes = MailboxTableQuery.GetMailboxes((clientType == ClientType.EventBased) ? "Client=EBA" : "Client=TBA", this, list.ToArray());
			List<MailboxInformation> list2 = new List<MailboxInformation>(mailboxes.Length);
			foreach (PropValue[] mailboxPropValue in mailboxes)
			{
				MailboxInformation mailboxInformation = this.GetMailboxInformation(mailboxPropValue);
				if (mailboxInformation != null)
				{
					list2.Add(mailboxInformation);
				}
			}
			return list2;
		}

		private void Initialize(Guid guid, string databaseName, string databaseLegacyDN, bool isPublic)
		{
			this.guid = guid;
			this.systemMailboxName = "SystemMailbox{" + this.guid + "}";
			this.databaseName = databaseName;
			this.isPublic = isPublic;
			this.displayName = string.Concat(new object[]
			{
				this.databaseName,
				" (",
				this.guid,
				")"
			});
			Exception ex = null;
			if (!this.isPublic)
			{
				try
				{
					this.systemMailboxPrincipal = ExchangePrincipal.FromADSystemMailbox(ADSessionSettings.FromRootOrgScopeSet(), this.FindSystemMailbox(), LocalServer.GetServer());
				}
				catch (DataValidationException ex2)
				{
					ex = ex2;
				}
				catch (ObjectNotFoundException ex3)
				{
					ex = ex3;
				}
				catch (ADExternalException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					ExTraceGlobals.DatabaseInfoTracer.TraceError<DatabaseInfo, Exception>((long)this.GetHashCode(), "{0}: Unable to find valid system mailbox. Exception: {1}", this, ex);
					throw new MissingSystemMailboxException(this.DisplayName, ex, base.Logger);
				}
				Guid guid2 = Guid.Empty;
				try
				{
					ADSystemAttendantMailbox systemAttendant = this.GetSystemAttendant();
					if (systemAttendant != null && systemAttendant.Database != null)
					{
						guid2 = systemAttendant.Database.ObjectGuid;
						this.systemAttendantMailboxGuid = ((systemAttendant.ExchangeGuid == Guid.Empty) ? systemAttendant.Guid : systemAttendant.ExchangeGuid);
						ExTraceGlobals.DatabaseInfoTracer.TraceDebug<DatabaseInfo, Guid, Guid>((long)this.GetHashCode(), "{0}: System Attendant Mailbox: Database GUID: {1}, Mailbox GUID: {2}", this, guid2, this.systemAttendantMailboxGuid);
					}
				}
				catch (DataValidationException ex5)
				{
					ex = ex5;
				}
				catch (ObjectNotFoundException ex6)
				{
					ex = ex6;
				}
				if (ex != null)
				{
					base.TracePfd("PFD AIS {0} {1}: System Attendant Mailbox: Database GUID: {2}, Mailbox GUID: {3}", new object[]
					{
						30551,
						this,
						ex
					});
					throw new MissingSystemMailboxException(this.DisplayName, ex, base.Logger);
				}
				this.systemAttendantMailboxPresent = (this.guid == guid2);
				ExTraceGlobals.DatabaseInfoTracer.TraceDebug<DatabaseInfo>((long)this.GetHashCode(), "{0}: Created database info", this);
			}
			base.TracePfd("PFD AIS {0} {1}: Created database info Sucessfully", new object[]
			{
				19287,
				this
			});
		}

		private MailboxInformation GetMailboxInformation(PropValue[] mailboxPropValue)
		{
			PropValue mailboxProperty = MailboxTableQuery.GetMailboxProperty(mailboxPropValue, PropTag.UserGuid);
			PropValue mailboxProperty2 = MailboxTableQuery.GetMailboxProperty(mailboxPropValue, PropTag.DisplayName);
			PropValue mailboxProperty3 = MailboxTableQuery.GetMailboxProperty(mailboxPropValue, PropTag.DateDiscoveredAbsentInDS);
			DateTime lastLogonTime = MailboxInformation.GetLastLogonTime(mailboxPropValue);
			if (mailboxProperty.PropTag != PropTag.UserGuid || mailboxProperty2.PropTag != PropTag.DisplayName)
			{
				return null;
			}
			if (mailboxProperty3.PropTag == PropTag.DateDiscoveredAbsentInDS)
			{
				return null;
			}
			return MailboxInformation.Create(mailboxProperty.GetBytes(), this.Guid, mailboxProperty2.GetString(), null, mailboxPropValue, lastLogonTime, null);
		}

		private string DistinguishedName
		{
			get
			{
				if (this.distinguishedName == null)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 401, "DistinguishedName", "f:\\15.00.1497\\sources\\dev\\assistants\\src\\Assistants\\DatabaseInfo.cs");
					MailboxDatabase mailboxDatabase = tenantOrTopologyConfigurationSession.Read<MailboxDatabase>(new ADObjectId(this.Guid));
					this.distinguishedName = mailboxDatabase.DistinguishedName;
				}
				return this.distinguishedName;
			}
		}

		public override string ToString()
		{
			return this.displayName;
		}

		public MailboxSession GetSystemMailbox(ClientType clientType, string actionInfo)
		{
			MailboxSession mailbox;
			try
			{
				mailbox = this.GetMailbox(this.systemMailboxPrincipal, clientType, actionInfo);
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.DatabaseInfoTracer.TraceError<DatabaseInfo, string, ObjectNotFoundException>((long)this.GetHashCode(), "{0}: unable to open system mailbox named '{1}'. Exception: {2}", this, this.systemMailboxName, ex);
				throw new MissingSystemMailboxException(this.DisplayName, ex, base.Logger);
			}
			mailbox.ExTimeZone = ExTimeZone.CurrentTimeZone;
			ExTraceGlobals.DatabaseInfoTracer.TraceDebug<DatabaseInfo, string>((long)this.GetHashCode(), "{0}: Opened system mailbox named '{1}'", this, this.systemMailboxName);
			return mailbox;
		}

		public MailboxSession GetMailbox(ExchangePrincipal principal, ClientType clientType, string actionInfo)
		{
			string str = (clientType == ClientType.EventBased) ? "Client=EBA;Service=" : "Client=TBA;Service=";
			return MailboxSession.OpenAsAdmin(principal, CultureInfo.InvariantCulture, str + base.Logger.ServiceName + ";Action=" + actionInfo, true);
		}

		public bool IsUserMailbox(Guid mailboxGuid)
		{
			return this.isPublic || (mailboxGuid != this.SystemMailboxGuid && !this.IsSystemAttendantMailbox(mailboxGuid));
		}

		public bool IsSystemAttendantMailbox(Guid mailboxGuid)
		{
			return this.systemAttendantMailboxPresent && this.systemAttendantMailboxGuid == mailboxGuid;
		}

		IEnumerable<IMailboxInformation> IDatabaseInfo.GetMailboxTable(ClientType clientType, PropertyTagPropertyDefinition[] properties)
		{
			return this.GetMailboxTable(clientType, properties);
		}

		private ADSystemMailbox FindSystemMailbox()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 526, "FindSystemMailbox", "f:\\15.00.1497\\sources\\dev\\assistants\\src\\Assistants\\DatabaseInfo.cs");
			ADRecipient[] array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, this.systemMailboxName), null, 1);
			if (array.Length != 1 || !(array[0] is ADSystemMailbox))
			{
				ExTraceGlobals.DatabaseInfoTracer.TraceError<DatabaseInfo, int, string>((long)this.GetHashCode(), "{0}: Found {1} mailboxes named '{2}' in the AD", this, array.Length, this.systemMailboxName);
				throw new MissingSystemMailboxException(this.DisplayName, base.Logger);
			}
			return (ADSystemMailbox)array[0];
		}

		private ADSystemAttendantMailbox GetSystemAttendant()
		{
			string text = LocalServer.GetServer().ExchangeLegacyDN + "/cn=Microsoft System Attendant";
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 571, "GetSystemAttendant", "f:\\15.00.1497\\sources\\dev\\assistants\\src\\Assistants\\DatabaseInfo.cs");
			ADRecipient adrecipient = null;
			try
			{
				adrecipient = tenantOrRootOrgRecipientSession.FindByLegacyExchangeDN(text);
			}
			catch (DataValidationException arg)
			{
				ExTraceGlobals.DatabaseInfoTracer.TraceError<DataValidationException>((long)this.GetHashCode(), "{0}: Invalid system attendant mailbox: {1}", arg);
			}
			if (adrecipient == null || !(adrecipient is ADSystemAttendantMailbox))
			{
				ExTraceGlobals.DatabaseInfoTracer.TraceError<DatabaseInfo, string>((long)this.GetHashCode(), "{0}: Unable to find valid SA mailbox with legDN: {1}", this, text);
				return null;
			}
			return (ADSystemAttendantMailbox)adrecipient;
		}

		private const string SystemAttendantRelativeLegDN = "/cn=Microsoft System Attendant";

		private string distinguishedName;

		private Guid guid;

		private string systemMailboxName;

		private string databaseName;

		private ExchangePrincipal systemMailboxPrincipal;

		private bool systemAttendantMailboxPresent;

		private bool isPublic;

		private string displayName;

		private Guid systemAttendantMailboxGuid = Guid.Empty;
	}
}

using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class MailboxInformation : IMailboxInformation
	{
		private MailboxInformation(byte[] mailboxGuid, Guid databaseGuid, string displayName, bool active, DateTime lastLogonTime) : this(new Guid(mailboxGuid), databaseGuid, displayName, active, lastLogonTime, null)
		{
		}

		private MailboxInformation(Guid mailboxGuid, Guid databaseGuid, string displayName, bool active, DateTime lastLogonTime, TenantPartitionHint tenantPartitionHint)
		{
			this.MailboxData = new StoreMailboxDataExtended(mailboxGuid, databaseGuid, displayName, null, tenantPartitionHint, this.IsArchiveMailbox(), this.IsGroupMailbox(), this.IsTeamSiteMailbox(), this.IsSharedMailbox());
			this.Active = active;
			this.LastLogonTime = lastLogonTime;
		}

		internal StoreMailboxDataExtended MailboxData { get; private set; }

		public Guid MailboxGuid
		{
			get
			{
				return this.MailboxData.Guid;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.MailboxData.DisplayName;
			}
		}

		public DateTime LastProcessedDate { get; private set; }

		public bool Active { get; private set; }

		public DateTime LastLogonTime { get; private set; }

		public static DateTime GetLastLogonTime(PropValue[] propvalueArray)
		{
			PropValue mailboxProperty = MailboxTableQuery.GetMailboxProperty(propvalueArray, PropTag.LastLogonTime);
			if (mailboxProperty.PropType != PropType.SysTime)
			{
				return DateTime.MinValue;
			}
			return mailboxProperty.GetDateTime();
		}

		public bool IsArchiveMailbox()
		{
			return StoreSession.IsArchiveMailbox(this.GetIntValue(PropTag.MailboxMiscFlags));
		}

		public bool IsPublicFolderMailbox()
		{
			return StoreSession.IsPublicFolderMailbox(this.GetIntValue(PropTag.MailboxType));
		}

		public bool IsGroupMailbox()
		{
			return StoreSession.IsGroupMailbox(this.GetIntValue(PropTag.MailboxTypeDetail));
		}

		public bool IsUserMailbox()
		{
			return StoreSession.IsUserMailbox(this.GetIntValue(PropTag.MailboxTypeDetail));
		}

		public bool IsTeamSiteMailbox()
		{
			return StoreSession.IsTeamSiteMailbox(this.GetIntValue(PropTag.MailboxTypeDetail));
		}

		public bool IsSharedMailbox()
		{
			return StoreSession.IsSharedMailbox(this.GetIntValue(PropTag.MailboxTypeDetail));
		}

		public object GetMailboxProperty(PropertyTagPropertyDefinition property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			foreach (PropValue propValue in this.mailboxProperties)
			{
				uint num = (uint)(propValue.PropTag & (PropTag)4294901760U);
				uint num2 = property.PropertyTag & 4294901760U;
				if (num == num2)
				{
					object result;
					if (propValue.PropTag == (PropTag)property.PropertyTag)
					{
						result = propValue.Value;
					}
					else
					{
						result = null;
					}
					return result;
				}
			}
			throw new ArgumentException("property");
		}

		public override string ToString()
		{
			string text = (this.DisplayName != null) ? this.DisplayName : "<null>";
			return string.Concat(new string[]
			{
				"MailboxGuid=",
				this.MailboxGuid.ToString(),
				",DisplayName='",
				text,
				"'"
			});
		}

		internal static MailboxInformation Create(ExRpcAdmin rpcAdmin, Guid mailboxGuid, Guid databaseGuid)
		{
			string displayName = string.Empty;
			bool flag = false;
			DateTime lastLogonTime = DateTime.MinValue;
			try
			{
				PropValue[][] mailboxTableInfo = rpcAdmin.GetMailboxTableInfo(databaseGuid, mailboxGuid, new PropTag[]
				{
					PropTag.UserGuid,
					PropTag.DisplayName,
					PropTag.DateDiscoveredAbsentInDS,
					PropTag.LastLogonTime
				});
				foreach (PropValue[] array2 in mailboxTableInfo)
				{
					if (array2.Length != 4 || array2[0].PropTag != PropTag.UserGuid || array2[1].PropTag != PropTag.DisplayName || !new Guid(array2[0].GetBytes()).Equals(mailboxGuid))
					{
						MailboxInformation.tracer.TraceDebug(0L, "MailboxInformation: Row does not contain the expected data.");
					}
					else
					{
						displayName = (string)array2[1].RawValue;
						lastLogonTime = MailboxInformation.GetLastLogonTime(array2);
						if (array2[2].PropTag != PropTag.DateDiscoveredAbsentInDS)
						{
							MailboxInformation.tracer.TraceDebug<Guid>(0L, "MailboxInformation: Mailbox {1} is active.", mailboxGuid);
							flag = true;
							break;
						}
					}
				}
			}
			catch (MapiExceptionNotFound)
			{
			}
			MailboxInformation.tracer.TraceDebug<Guid, bool>(0L, "MailboxInformation: Mailbox {1} active state is {2}.", mailboxGuid, flag);
			return new MailboxInformation(mailboxGuid, databaseGuid, displayName, flag, lastLogonTime, null);
		}

		internal static MailboxInformation Create(byte[] mailboxGuid, Guid databaseGuid, string displayName, ControlData controlData, PropValue[] mailboxProperties, DateTime lastLogonTime, TenantPartitionHint tenantPartitionHint = null)
		{
			MailboxInformation mailboxInformation = new MailboxInformation(new Guid(mailboxGuid), databaseGuid, displayName, true, lastLogonTime, tenantPartitionHint);
			if (controlData != null)
			{
				mailboxInformation.LastProcessedDate = controlData.LastProcessedDate;
			}
			else
			{
				mailboxInformation.LastProcessedDate = DateTime.MinValue;
			}
			mailboxInformation.mailboxProperties = mailboxProperties;
			mailboxInformation.MailboxData.IsPublicFolderMailbox = mailboxInformation.IsPublicFolderMailbox();
			return mailboxInformation;
		}

		private int GetIntValue(PropTag tag)
		{
			if (this.mailboxProperties == null)
			{
				return 0;
			}
			int num = tag.Id();
			foreach (PropValue propValue in this.mailboxProperties)
			{
				if (propValue.PropTag.Id() == num)
				{
					int result;
					if (propValue.PropType == PropType.Int)
					{
						result = propValue.GetInt();
					}
					else
					{
						result = 0;
					}
					return result;
				}
			}
			return 0;
		}

		private static Trace tracer = ExTraceGlobals.EventAccessTracer;

		private PropValue[] mailboxProperties;
	}
}

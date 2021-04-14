using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class MailboxDiagnosticLogsDataProvider : XsoMailboxDataProviderBase
	{
		static MailboxDiagnosticLogsDataProvider()
		{
			PropertyDefinition[] array = new PropertyDefinition[]
			{
				StoreObjectSchema.EntryId,
				StoreObjectSchema.ChangeKey,
				StoreObjectSchema.ParentEntryId,
				StoreObjectSchema.ParentItemId,
				StoreObjectSchema.SearchKey,
				StoreObjectSchema.RecordKey
			};
			MailboxDiagnosticLogsDataProvider.mailboxExtendedProperties = new List<PropertyDefinition>(MailboxSchema.Instance.AllProperties);
			foreach (PropertyDefinition item in array)
			{
				MailboxDiagnosticLogsDataProvider.mailboxExtendedProperties.Remove(item);
			}
		}

		public MailboxDiagnosticLogsDataProvider(ExchangePrincipal exchangePrincipal, string action) : base(exchangePrincipal, action)
		{
		}

		public MailboxDiagnosticLogsDataProvider(string componentName, ExchangePrincipal exchangePrincipal, string action) : this(exchangePrincipal, action)
		{
			this.getProperties = false;
			this.componentName = componentName;
		}

		protected override IEnumerable<T> InternalFindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			if (filter != null && !(filter is FalseFilter))
			{
				throw new NotSupportedException("filter");
			}
			if (rootId != null && rootId is ADObjectId && !ADObjectId.Equals((ADObjectId)rootId, base.MailboxSession.MailboxOwner.ObjectId))
			{
				throw new NotSupportedException("rootId");
			}
			if (!typeof(MailboxDiagnosticLogs).IsAssignableFrom(typeof(T)))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			MailboxDiagnosticLogs mailboxDiagnosticLog = (MailboxDiagnosticLogs)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			if (this.getProperties)
			{
				mailboxDiagnosticLog.LogName = "ExtendedProperties";
				mailboxDiagnosticLog.MailboxLog = this.ReadMailboxTableProperties();
			}
			else
			{
				mailboxDiagnosticLog.LogName = this.componentName;
				mailboxDiagnosticLog.MailboxLog = this.ReadLogs();
			}
			if (mailboxDiagnosticLog.MailboxLog == null)
			{
				throw new ObjectNotFoundException(Strings.ExportMailboxDiagnosticLogsComponentNotFound(this.componentName ?? "$null", base.MailboxSession.MailboxOwner.MailboxInfo.DisplayName, this.GetAvailableLogNames()));
			}
			mailboxDiagnosticLog[SimpleProviderObjectSchema.Identity] = base.MailboxSession.MailboxOwner.ObjectId;
			yield return (T)((object)mailboxDiagnosticLog);
			yield break;
		}

		protected override void InternalSave(ConfigurableObject instance)
		{
			throw new NotSupportedException("SaveDiagnosticLog");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxDiagnosticLogsDataProvider>(this);
		}

		private string ReadMailboxTableProperties()
		{
			List<XElement> list = new List<XElement>();
			base.MailboxSession.Mailbox.Load(MailboxDiagnosticLogsDataProvider.mailboxExtendedProperties);
			object[] properties = base.MailboxSession.Mailbox.GetProperties(MailboxDiagnosticLogsDataProvider.mailboxExtendedProperties);
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in MailboxDiagnosticLogsDataProvider.mailboxExtendedProperties)
			{
				if (!(properties[num] is PropertyError))
				{
					string content;
					if (propertyDefinition.Type.Equals(typeof(byte[])))
					{
						byte[] array = (byte[])properties[num];
						StringBuilder stringBuilder = new StringBuilder((array.Length + 1) * 2);
						stringBuilder.Append("0x");
						foreach (byte b in array)
						{
							stringBuilder.Append(b.ToString("X2"));
						}
						content = stringBuilder.ToString();
					}
					else
					{
						content = properties[num].ToString();
					}
					list.Add(new XElement("Property", new object[]
					{
						new XElement("Name", propertyDefinition.Name),
						new XElement("Value", content)
					}));
				}
				num++;
			}
			XDocument xdocument = new XDocument(new object[]
			{
				new XElement("Properties", new XElement("MailboxTable", list.ToArray()))
			});
			return xdocument.ToString(SaveOptions.None);
		}

		private string ReadLogs()
		{
			SingleInstanceItemHandler singleInstanceItemHandler = new SingleInstanceItemHandler(string.Format("IPM.Microsoft.{0}.Log", this.componentName), DefaultFolderType.Configuration);
			return singleInstanceItemHandler.GetItemContent(base.MailboxSession);
		}

		private string GetAvailableLogNames()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (Folder folder = Folder.Bind(base.MailboxSession, DefaultFolderType.Configuration))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
				{
					StoreObjectSchema.ItemClass
				}))
				{
					object[][] rows = queryResult.GetRows(MailboxDiagnosticLogsDataProvider.maxComponentNameListLimit);
					if (rows != null)
					{
						foreach (object[] array2 in rows)
						{
							string itemClass = array2[0] as string;
							string mailboxLogComponentName = this.GetMailboxLogComponentName(itemClass);
							if (mailboxLogComponentName != null)
							{
								stringBuilder.Append((stringBuilder.Length > 0) ? (", " + mailboxLogComponentName) : mailboxLogComponentName);
							}
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		private string GetMailboxLogComponentName(string itemClass)
		{
			MatchCollection matchCollection = MailboxDiagnosticLogsDataProvider.messageClassRegex.Matches(itemClass);
			if (matchCollection.Count == 1)
			{
				return matchCollection[0].Groups["Component"].Value;
			}
			return null;
		}

		private const string MessageClass = "IPM.Microsoft.{0}.Log";

		private const int ItemClassIndex = 0;

		private static int maxComponentNameListLimit = 100;

		private static Regex messageClassRegex = new Regex("IPM\\.Microsoft\\.(?<Component>.[^\\.]+?)\\.Log", RegexOptions.IgnoreCase);

		private static List<PropertyDefinition> mailboxExtendedProperties = null;

		private readonly string componentName;

		private readonly bool getProperties = true;
	}
}

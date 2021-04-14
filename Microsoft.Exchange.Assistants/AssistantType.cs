using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class AssistantType : Base, IEventBasedAssistantType, IAssistantType
	{
		public AssistantType(IEventBasedAssistantType baseType, PerformanceCountersPerAssistantInstance performanceCounters)
		{
			this.baseType = baseType;
			this.PerformanceCounters = performanceCounters;
		}

		public PerformanceCountersPerAssistantInstance PerformanceCounters { get; private set; }

		public LocalizedString Name
		{
			get
			{
				return this.baseType.Name;
			}
		}

		public string NonLocalizedName
		{
			get
			{
				return this.baseType.NonLocalizedName;
			}
		}

		public IMailboxFilter MailboxFilter
		{
			get
			{
				return this.baseType as IMailboxFilter;
			}
		}

		public MapiEventTypeFlags EventMask
		{
			get
			{
				return this.baseType.EventMask;
			}
		}

		public bool NeedsMailboxSession
		{
			get
			{
				return this.baseType.NeedsMailboxSession;
			}
		}

		public PropertyDefinition[] PreloadItemProperties
		{
			get
			{
				return this.baseType.PreloadItemProperties;
			}
		}

		public bool ProcessesPublicDatabases
		{
			get
			{
				return this.baseType.ProcessesPublicDatabases;
			}
		}

		public Guid Identity
		{
			get
			{
				return this.baseType.Identity;
			}
		}

		public static AssistantType[] CreateArray(IEventBasedAssistantType[] eventBasedAssistantTypeArray, PerformanceCountersPerAssistantInstance performanceCountersPerAssistantsTotal)
		{
			AssistantType[] array = new AssistantType[eventBasedAssistantTypeArray.Length];
			int num = 0;
			foreach (IEventBasedAssistantType eventBasedAssistantType in eventBasedAssistantTypeArray)
			{
				string text = eventBasedAssistantType.GetType().Name;
				if (text.EndsWith("Type"))
				{
					text = text.Substring(0, text.Length - 4);
				}
				PerformanceCountersPerAssistantInstance performanceCounters = new PerformanceCountersPerAssistantInstance(text, performanceCountersPerAssistantsTotal);
				array[num++] = new AssistantType(eventBasedAssistantType, performanceCounters);
			}
			return array;
		}

		public IEventBasedAssistant CreateInstance(DatabaseInfo databaseInfo)
		{
			return this.baseType.CreateInstance(databaseInfo);
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableEventBasedAssistantType queryableEventBasedAssistantType = queryableObject as QueryableEventBasedAssistantType;
			if (queryableEventBasedAssistantType != null)
			{
				queryableEventBasedAssistantType.AssistantGuid = this.Identity;
				queryableEventBasedAssistantType.AssistantName = this.NonLocalizedName;
				if (this.MailboxFilter != null)
				{
					queryableEventBasedAssistantType.MailboxType = this.MailboxFilter.MailboxType.ToString();
				}
				queryableEventBasedAssistantType.MapiEventType = this.EventMask.ToString();
				queryableEventBasedAssistantType.NeedMailboxSession = this.NeedsMailboxSession;
			}
		}

		private IEventBasedAssistantType baseType;
	}
}

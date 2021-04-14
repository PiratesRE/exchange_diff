using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal class EmergencyKit
	{
		public EmergencyKit(MapiEvent mapiEvent)
		{
			this.mapiEvent = mapiEvent;
		}

		public EmergencyKit(Guid mailboxGuid)
		{
			this.mailboxGuid = mailboxGuid;
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public string MailboxDisplayName
		{
			get
			{
				if (this.eventDispatcher != null)
				{
					return this.eventDispatcher.MailboxDisplayName;
				}
				if (this.mailboxData != null)
				{
					return this.mailboxData.DisplayName;
				}
				return "<unknown>";
			}
		}

		public MapiEvent MapiEvent
		{
			get
			{
				return this.mapiEvent;
			}
		}

		public LocalizedString AssistantName
		{
			get
			{
				if (this.assistant != null)
				{
					return this.assistant.Name;
				}
				return Strings.driverName;
			}
		}

		public void SetContext(IAssistantBase assistant, EventDispatcher eventDispatcher)
		{
			this.assistant = assistant;
			this.eventDispatcher = eventDispatcher;
		}

		public void SetContext(IAssistantBase assistant, MailboxData mailboxData)
		{
			this.assistant = assistant;
			this.mailboxData = mailboxData;
		}

		public void SetContext(IAssistantBase assistant)
		{
			this.assistant = assistant;
		}

		public void UnsetContext()
		{
			this.assistant = null;
			this.eventDispatcher = null;
			this.mailboxData = null;
		}

		private Guid mailboxGuid;

		private MapiEvent mapiEvent;

		private IAssistantBase assistant;

		private EventDispatcher eventDispatcher;

		private MailboxData mailboxData;
	}
}

using System;
using System.Collections;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OwaEventNamespaceAttribute : Attribute
	{
		public OwaEventNamespaceAttribute(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.eventInfoTable = new Hashtable();
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Type HandlerType
		{
			get
			{
				return this.handlerType;
			}
			set
			{
				this.handlerType = value;
			}
		}

		internal Hashtable EventInfoTable
		{
			get
			{
				return this.eventInfoTable;
			}
		}

		internal void AddEventInfo(OwaEventAttribute eventInfo)
		{
			if (!this.eventInfoTable.ContainsKey(eventInfo.Name))
			{
				this.eventInfoTable[eventInfo.Name] = eventInfo;
				return;
			}
			if (!eventInfo.IsAsync)
			{
				throw new OwaException(string.Format("Event name already exists in the namespace. '{0}'", eventInfo.Name));
			}
			OwaEventAttribute owaEventAttribute = (OwaEventAttribute)this.eventInfoTable[eventInfo.Name];
			if (!owaEventAttribute.IsAsync)
			{
				throw new OwaException(string.Format("Event name already exists in the namespace. '{0}'", eventInfo.Name));
			}
			if (eventInfo.BeginMethodInfo == null && eventInfo.EndMethodInfo != null && owaEventAttribute.BeginMethodInfo != null && owaEventAttribute.EndMethodInfo == null)
			{
				owaEventAttribute.EndMethodInfo = eventInfo.EndMethodInfo;
				return;
			}
			if (eventInfo.BeginMethodInfo != null && eventInfo.EndMethodInfo == null && owaEventAttribute.BeginMethodInfo == null && owaEventAttribute.EndMethodInfo != null)
			{
				this.eventInfoTable[eventInfo.Name] = eventInfo;
				eventInfo.EndMethodInfo = owaEventAttribute.EndMethodInfo;
				return;
			}
			throw new OwaException("Error registering async event.");
		}

		internal OwaEventAttribute FindEventInfo(string name)
		{
			return (OwaEventAttribute)this.eventInfoTable[name];
		}

		private string name;

		private Hashtable eventInfoTable;

		private Type handlerType;
	}
}

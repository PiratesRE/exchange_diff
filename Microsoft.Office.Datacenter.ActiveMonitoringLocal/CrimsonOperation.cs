using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal abstract class CrimsonOperation<T> : IDisposable where T : IPersistence, new()
	{
		internal CrimsonOperation() : this(null, null)
		{
		}

		internal CrimsonOperation(EventBookmark bookmark) : this(bookmark, null)
		{
		}

		internal CrimsonOperation(EventBookmark bookmark, string channelName)
		{
			if (channelName == null)
			{
				channelName = CrimsonHelper.GetChannelName<T>();
			}
			this.ChannelName = channelName;
			this.BookMark = bookmark;
		}

		internal bool IsAccessPropertyDirectly { get; set; }

		internal string ExplicitXPathQuery { get; set; }

		internal DateTime? QueryStartTime { get; set; }

		internal DateTime? QueryEndTime { get; set; }

		internal string QueryUserPropertyCondition { get; set; }

		internal CrimsonConnectionInfo ConnectionInfo { get; set; }

		protected string ChannelName { get; set; }

		protected EventBookmark BookMark { get; set; }

		protected bool IsInitialized { get; set; }

		private protected EventLogQuery LogQuery { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal EventLogQuery GetQueryObject()
		{
			if (this.LogQuery != null)
			{
				return this.LogQuery;
			}
			string query;
			if (this.ExplicitXPathQuery != null)
			{
				query = this.ExplicitXPathQuery;
			}
			else
			{
				query = this.GetDefaultXPathQuery();
			}
			EventLogQuery eventLogQuery = new EventLogQuery(this.ChannelName, PathType.LogName, query);
			if (this.ConnectionInfo != null && this.ConnectionInfo.ComputerName != null)
			{
				if (this.ConnectionInfo.UserName != null)
				{
					eventLogQuery.Session = new EventLogSession(this.ConnectionInfo.ComputerName, this.ConnectionInfo.UserDomain, this.ConnectionInfo.UserName, this.ConnectionInfo.Password, SessionAuthentication.Default);
				}
				else
				{
					eventLogQuery.Session = new EventLogSession(this.ConnectionInfo.ComputerName);
				}
			}
			this.LogQuery = eventLogQuery;
			return this.LogQuery;
		}

		internal T EventToObject(EventRecord record)
		{
			T result = default(T);
			if (record != null)
			{
				try
				{
					result = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
					LocalDataAccessMetaData metaData = new LocalDataAccessMetaData(record);
					Dictionary<string, string> eventRecordProperties = this.GetEventRecordProperties(record);
					result.Initialize(eventRecordProperties, metaData);
				}
				catch (FormatException arg)
				{
					WTFDiagnostics.TraceError<FormatException>(WTFLog.DataAccess, this.traceContext, "[CrimsonOperation.EventToObject]: FormatException - Failed to initialize object properties: {0}", arg, null, "EventToObject", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\CrimsonOperation.cs", 214);
					result = default(T);
				}
				catch (EventLogException arg2)
				{
					WTFDiagnostics.TraceError<EventLogException>(WTFLog.DataAccess, this.traceContext, "[CrimsonOperation.EventToObject]: EventLogException - Failed to read Crimson event: {0}", arg2, null, "EventToObject", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\CrimsonOperation.cs", 221);
					result = default(T);
				}
				catch (ObjectDisposedException arg3)
				{
					WTFDiagnostics.TraceError<ObjectDisposedException>(WTFLog.DataAccess, this.traceContext, "[CrimsonOperation.EventToObject]:  ObjectDisposedException - Failed to read Crimson event: {0}", arg3, null, "EventToObject", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\CrimsonOperation.cs", 228);
					result = default(T);
				}
				catch (Exception arg4)
				{
					WTFDiagnostics.TraceError<Exception>(WTFLog.DataAccess, this.traceContext, "[CrimsonOperation.EventToObject]: Unknown Excption - Failed to read Crimson event: {0}", arg4, null, "EventToObject", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\LocalDataAccess\\CrimsonOperation.cs", 235);
					result = default(T);
				}
			}
			return result;
		}

		internal abstract void Cleanup();

		internal Dictionary<string, string> GetEventRecordProperties(EventRecord record)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(20);
			string xml = record.ToXml();
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(xml);
			using (XmlNodeList elementsByTagName = safeXmlDocument.GetElementsByTagName("EventXML"))
			{
				if (elementsByTagName != null && elementsByTagName.Count > 0)
				{
					XmlNode xmlNode = elementsByTagName.Item(0);
					using (XmlNodeList childNodes = xmlNode.ChildNodes)
					{
						foreach (object obj in childNodes)
						{
							XmlNode xmlNode2 = (XmlNode)obj;
							dictionary.Add(xmlNode2.Name, xmlNode2.InnerText);
						}
					}
				}
			}
			return dictionary;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.LogQuery != null && this.LogQuery.Session != null && this.LogQuery.Session != EventLogSession.GlobalSession)
					{
						this.LogQuery.Session.Dispose();
						this.LogQuery.Session = null;
					}
					this.Cleanup();
				}
				this.disposed = true;
			}
		}

		protected abstract string GetDefaultXPathQuery();

		private bool disposed;

		private TracingContext traceContext = TracingContext.Default;
	}
}

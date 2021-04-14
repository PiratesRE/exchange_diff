using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	[Table]
	public class StatusEntry
	{
		public StatusEntry()
		{
			this.MarkEntryAsPersisted();
		}

		protected internal StatusEntry(string key)
		{
			this.Key = key;
			this.state = StatusEntry.EntryState.ToBeWritten;
			this.CreatedTime = DateTime.UtcNow;
			this.UpdatedTime = this.CreatedTime;
			this.TableName = Settings.GetTableName(typeof(StatusEntry));
		}

		[Column]
		public DateTime CreatedTime { get; internal set; }

		[Column]
		public DateTime UpdatedTime { get; internal set; }

		[Column]
		public string XmlColumn1 { get; internal set; }

		[Column]
		public bool Remove { get; internal set; }

		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id
		{
			get
			{
				return this.id;
			}
			internal set
			{
				if (this.id == -1)
				{
					this.id = value;
				}
			}
		}

		[Column]
		public string Key { get; internal set; }

		[Column]
		public string TableName { get; internal set; }

		internal StatusEntry.EntryState State
		{
			get
			{
				return this.state;
			}
		}

		public string this[string propertyName]
		{
			get
			{
				return this.GetPropertyValue(propertyName);
			}
			set
			{
				this.SetPropertyValue(propertyName, value);
			}
		}

		internal bool EntryExistsInDatabase()
		{
			return this.state != StatusEntry.EntryState.ToBeWritten && this.state != StatusEntry.EntryState.Removed;
		}

		internal void PrepareForPersistance()
		{
			this.LoadPropertiesFromXml();
			XElement xelement = new XElement("Status", this.properties.Select(delegate(KeyValuePair<string, string> kv)
			{
				XElement xelement2 = new XElement("Prop", kv.Value);
				xelement2.SetAttributeValue("Name", kv.Key);
				return xelement2;
			}));
			this.XmlColumn1 = xelement.ToString();
		}

		internal void MarkEntryAsPersisted()
		{
			if (this.state != StatusEntry.EntryState.Removed)
			{
				this.state = StatusEntry.EntryState.Unchanged;
			}
		}

		internal void MarkEntryAsRemoved()
		{
			this.state = StatusEntry.EntryState.Removed;
		}

		private string GetPropertyValue(string propertyName)
		{
			string propertyValueFromXml;
			if (!this.properties.TryGetValue(propertyName, out propertyValueFromXml))
			{
				propertyValueFromXml = this.GetPropertyValueFromXml(propertyName);
			}
			return propertyValueFromXml;
		}

		private void SetPropertyValue(string propertyName, string value)
		{
			if (!this.properties.ContainsKey(propertyName))
			{
				this.properties.Add(propertyName, value);
				return;
			}
			throw new Exception(string.Format("Unable to set key {0} as it alreayd has a value.", propertyName));
		}

		private void LoadPropertiesFromXml()
		{
			if (!string.IsNullOrEmpty(this.XmlColumn1))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(this.XmlColumn1);
				XmlNodeList xmlNodeList = xmlDocument.SelectNodes(StatusEntry.XpathSearchExpression);
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string value = xmlNode.Attributes["Name"].Value;
					if (!this.properties.ContainsKey(value))
					{
						string innerText = xmlNode.InnerText;
						this.properties.Add(value, innerText);
					}
				}
			}
		}

		private string GetPropertyValueFromXml(string propertyName)
		{
			string empty = string.Empty;
			this.LoadPropertiesFromXml();
			this.properties.TryGetValue(propertyName, out empty);
			return empty;
		}

		private const int NonPersistedItemId = -1;

		private const string XmlRootNodeName = "Status";

		private const string PropertyXmlNodeName = "Prop";

		private static readonly string XpathSearchExpression = string.Format("{0}/{1}", "Status", "Prop");

		private StatusEntry.EntryState state = StatusEntry.EntryState.Unchanged;

		private int id = -1;

		private Dictionary<string, string> properties = new Dictionary<string, string>();

		internal enum EntryState
		{
			ToBeWritten,
			Removed,
			Unchanged
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarLogAnalysis : IComparable<CalendarLogAnalysis>, IComparer<CalendarLogAnalysis>
	{
		private CalendarLogAnalysis()
		{
		}

		internal CalendarLogAnalysis(CalendarLogId identity, Item item, IEnumerable<PropertyDefinition> loadedProperties)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (loadedProperties == null)
			{
				throw new ArgumentNullException("loadedProperties");
			}
			this.identity = identity;
			this.loadedProperties = loadedProperties;
			this.InitializeProperties(item);
		}

		internal ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		internal IEnumerable<AnalysisRule> Alerts
		{
			get
			{
				return this.alerts;
			}
		}

		internal string LocalLogTime
		{
			get
			{
				return this.OriginalLastModifiedTime.ToString("MMM dd, yyyy HH:mm:ss:ffff");
			}
		}

		internal string this[PropertyDefinition p]
		{
			get
			{
				if (this.InternalProperties.ContainsKey(p))
				{
					object obj = this.properties[p];
					if (obj != null)
					{
						return this.FormatPropertyValue(p, obj);
					}
				}
				return string.Empty;
			}
		}

		private string FormatPropertyValue(PropertyDefinition prop, object value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			if (prop.Type == typeof(ExDateTime))
			{
				if (value is ExDateTime)
				{
					return ((ExDateTime)value).ToString("yyyy MM dd HH mm ss ffff");
				}
				return ((DateTime)value).ToString("yyyy MM dd HH mm ss ffff");
			}
			else
			{
				if (prop.Type == typeof(byte[]))
				{
					byte[] bytes = (byte[])value;
					return bytes.To64BitString();
				}
				return value.ToString();
			}
		}

		internal bool HasAlerts
		{
			get
			{
				return this.Alerts.Count<AnalysisRule>() > 0;
			}
		}

		internal ExDateTime OriginalLastModifiedTime { get; private set; }

		internal int ItemVersion { get; private set; }

		internal Dictionary<PropertyDefinition, object> InternalProperties
		{
			get
			{
				return this.properties;
			}
		}

		internal CalendarLogId InternalIdentity
		{
			get
			{
				return this.identity;
			}
		}

		internal void AddAlert(AnalysisRule rule)
		{
			if (!this.alerts.Contains(rule))
			{
				this.alerts.Add(rule.Clone());
			}
		}

		private void InitializeProperties(Item item)
		{
			this.OriginalLastModifiedTime = item.GetProperty(CalendarItemBaseSchema.OriginalLastModifiedTime);
			this.ItemVersion = item.GetProperty(CalendarItemBaseSchema.ItemVersion);
			if (item.StoreObjectId == null)
			{
				this.properties.Add(ItemSchema.Id, item.GetProperty(ItemSchema.Id));
			}
			else
			{
				this.properties.Add(ItemSchema.Id, item.StoreObjectId.ToBase64String());
			}
			foreach (PropertyDefinition propertyDefinition in this.loadedProperties)
			{
				if (propertyDefinition != ItemSchema.Id)
				{
					object obj = item.TryGetProperty(propertyDefinition);
					if (!(obj is PropertyError))
					{
						this.properties.Add(propertyDefinition, obj);
					}
				}
			}
		}

		public static IEnumerable<PropertyDefinition> GetDisplayProperties(IEnumerable<CalendarLogAnalysis> logs)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			foreach (CalendarLogAnalysis calendarLogAnalysis in logs)
			{
				foreach (AnalysisRule analysisRule in calendarLogAnalysis.Alerts)
				{
					list.AddRange(analysisRule.RequiredProperties.Except(list));
				}
			}
			return list;
		}

		public static IComparer<CalendarLogAnalysis> GetComparer()
		{
			return new CalendarLogAnalysis();
		}

		public int CompareTo(CalendarLogAnalysis other)
		{
			return this.Compare(this, other);
		}

		public int Compare(CalendarLogAnalysis c0, CalendarLogAnalysis c1)
		{
			int num = c0.ItemVersion.CompareTo(c1.ItemVersion);
			if (num == 0)
			{
				num = c0.OriginalLastModifiedTime.CompareTo(c1.OriginalLastModifiedTime);
			}
			return num;
		}

		private const string SortableDateFormat = "yyyy MM dd HH mm ss ffff";

		private const string DisplayDateFormat = "MMM dd, yyyy HH:mm:ss:ffff";

		private CalendarLogId identity;

		private List<AnalysisRule> alerts = new List<AnalysisRule>();

		private Dictionary<PropertyDefinition, object> properties = new Dictionary<PropertyDefinition, object>();

		private IEnumerable<PropertyDefinition> loadedProperties;
	}
}

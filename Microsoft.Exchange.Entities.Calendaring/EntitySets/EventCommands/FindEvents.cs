using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FindEvents : FindStorageEntitiesCommand<Events, Event>
	{
		public override IDictionary<string, Microsoft.Exchange.Data.PropertyDefinition> PropertyMap
		{
			get
			{
				return FindEvents.EventPropertyMap;
			}
		}

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.FindEventsTracer;
			}
		}

		public static bool NeedsReread(IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> propertyDefinitions, ITracer tracer)
		{
			foreach (Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				if (!FindEvents.SupportsProperty(propertyDefinition))
				{
					tracer.TraceDebug<string>(0L, "Need re-read due to property: {0}", propertyDefinition.Name);
					return true;
				}
			}
			return false;
		}

		public static bool SupportsProperty(Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition propertyDefinition)
		{
			foreach (Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition propertyDefinition2 in FindEvents.HardCodedProperties)
			{
				if (propertyDefinition2.Name.Equals(propertyDefinition.Name))
				{
					return true;
				}
			}
			return false;
		}

		protected override IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> GetRequestedPropertyDependencies()
		{
			if (FindEvents.hardCodedStorageProperties == null)
			{
				FindEvents.hardCodedStorageProperties = this.Scope.EventDataProvider.MapProperties(FindEvents.HardCodedProperties).Concat(FindEvents.HardCodedPropertyDependencies);
			}
			return FindEvents.hardCodedStorageProperties;
		}

		protected override IEnumerable<Event> OnExecute()
		{
			EventDataProvider eventDataProvider = this.Scope.EventDataProvider;
			Microsoft.Exchange.Data.PropertyDefinition[] array = base.Properties.ToArray<Microsoft.Exchange.Data.PropertyDefinition>();
			bool flag = this.Context != null && this.Context.RequestedProperties != null && FindEvents.NeedsReread(this.Context.RequestedProperties, this.Trace);
			IEnumerable<Event> source = eventDataProvider.Find(base.QueryFilter, base.SortColumns, flag ? this.Scope.EventDataProvider.MapProperties(FindEvents.IdProperty).ToArray<Microsoft.Exchange.Data.PropertyDefinition>() : array);
			IEnumerable<Event> source2 = base.QueryOptions.ApplySkipTakeTo(source.AsQueryable<Event>());
			if (flag)
			{
				int count = (this.Context != null) ? this.Context.PageSizeOnReread : 20;
				return (from x in source2.Take(count)
				select this.Scope.Read(x.Id, this.Context)).ToList<Event>();
			}
			EventTimeAdjuster adjuster = this.Scope.TimeAdjuster;
			ExTimeZone sessionTimeZone = this.Scope.Session.ExTimeZone;
			return from e in source2
			select adjuster.AdjustTimeProperties(e, sessionTimeZone);
		}

		public static readonly Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[] HardCodedProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
		{
			SchematizedObject<EventSchema>.SchemaInstance.LastModifiedTimeProperty,
			SchematizedObject<EventSchema>.SchemaInstance.DateTimeCreatedProperty,
			SchematizedObject<EventSchema>.SchemaInstance.CategoriesProperty,
			SchematizedObject<EventSchema>.SchemaInstance.EndProperty,
			SchematizedObject<EventSchema>.SchemaInstance.HasAttachmentsProperty,
			SchematizedObject<EventSchema>.SchemaInstance.HasAttendeesProperty,
			SchematizedObject<EventSchema>.SchemaInstance.IdProperty,
			SchematizedObject<EventSchema>.SchemaInstance.IntendedEndTimeZoneIdProperty,
			SchematizedObject<EventSchema>.SchemaInstance.IntendedStartTimeZoneIdProperty,
			SchematizedObject<EventSchema>.SchemaInstance.IsAllDayProperty,
			SchematizedObject<EventSchema>.SchemaInstance.IsCancelledProperty,
			SchematizedObject<EventSchema>.SchemaInstance.IsOrganizerProperty,
			SchematizedObject<EventSchema>.SchemaInstance.ImportanceProperty,
			SchematizedObject<EventSchema>.SchemaInstance.LocationProperty,
			SchematizedObject<EventSchema>.SchemaInstance.OrganizerProperty,
			SchematizedObject<EventSchema>.SchemaInstance.ResponseStatusProperty,
			SchematizedObject<EventSchema>.SchemaInstance.ResponseRequestedProperty,
			SchematizedObject<EventSchema>.SchemaInstance.SensitivityProperty,
			SchematizedObject<EventSchema>.SchemaInstance.SeriesIdProperty,
			SchematizedObject<EventSchema>.SchemaInstance.SeriesMasterIdProperty,
			SchematizedObject<EventSchema>.SchemaInstance.ShowAsProperty,
			SchematizedObject<EventSchema>.SchemaInstance.StartProperty,
			SchematizedObject<EventSchema>.SchemaInstance.SubjectProperty,
			SchematizedObject<EventSchema>.SchemaInstance.TypeProperty
		};

		public static readonly Microsoft.Exchange.Data.PropertyDefinition[] HardCodedPropertyDependencies = new Microsoft.Exchange.Data.PropertyDefinition[]
		{
			CalendarItemInstanceSchema.PropertyChangeMetadataRaw
		};

		public static readonly Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[] IdProperty = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
		{
			SchematizedObject<EventSchema>.SchemaInstance.IdProperty
		};

		private static readonly Dictionary<string, Microsoft.Exchange.Data.PropertyDefinition> EventPropertyMap = new Dictionary<string, Microsoft.Exchange.Data.PropertyDefinition>
		{
			{
				"Subject",
				ItemSchema.Subject
			},
			{
				"Start",
				CalendarItemInstanceSchema.StartTime
			},
			{
				"End",
				CalendarItemInstanceSchema.EndTime
			},
			{
				"Importance",
				ItemSchema.Importance
			},
			{
				"IsAllDay",
				CalendarItemBaseSchema.IsAllDayEvent
			},
			{
				"ShowAs",
				InternalSchema.FreeBusyStatus
			},
			{
				"ResponseRequested",
				InternalSchema.IsResponseRequested
			}
		};

		private static IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> hardCodedStorageProperties;
	}
}

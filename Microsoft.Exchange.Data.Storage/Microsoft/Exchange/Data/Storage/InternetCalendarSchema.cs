using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class InternetCalendarSchema
	{
		internal static PropertyDefinition[] ExportFreeBusy
		{
			get
			{
				if (InternetCalendarSchema.exportFreeBusy == null)
				{
					lock (InternetCalendarSchema.exportFreeBusyLock)
					{
						if (InternetCalendarSchema.exportFreeBusy == null)
						{
							InternetCalendarSchema.exportFreeBusy = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
							{
								InternetCalendarSchema.ExportQueryOnly,
								InternetCalendarSchema.FreeBusyOnly
							});
						}
					}
				}
				return InternetCalendarSchema.exportFreeBusy;
			}
		}

		internal static PropertyDefinition[] ExportLimitedDetails
		{
			get
			{
				if (InternetCalendarSchema.exportLimitedDetails == null)
				{
					lock (InternetCalendarSchema.exportLimitedDetailsLock)
					{
						if (InternetCalendarSchema.exportLimitedDetails == null)
						{
							InternetCalendarSchema.exportLimitedDetails = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
							{
								InternetCalendarSchema.ExportFreeBusy,
								InternetCalendarSchema.LimitedDetailOnly
							});
						}
					}
				}
				return InternetCalendarSchema.exportLimitedDetails;
			}
		}

		internal static PropertyDefinition[] ExportFullDetails
		{
			get
			{
				if (InternetCalendarSchema.exportFullDetails == null)
				{
					lock (InternetCalendarSchema.exportFullDetailsLock)
					{
						if (InternetCalendarSchema.exportFullDetails == null)
						{
							InternetCalendarSchema.exportFullDetails = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
							{
								InternetCalendarSchema.ExportLimitedDetails,
								InternetCalendarSchema.FullDetailOnly
							});
						}
					}
				}
				return InternetCalendarSchema.exportFullDetails;
			}
		}

		internal static PropertyDefinition[] ImportCompare
		{
			get
			{
				if (InternetCalendarSchema.importCompare == null)
				{
					lock (InternetCalendarSchema.importCompareLock)
					{
						if (InternetCalendarSchema.importCompare == null)
						{
							InternetCalendarSchema.importCompare = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
							{
								InternetCalendarSchema.FreeBusyOnly,
								InternetCalendarSchema.LimitedDetailOnly,
								InternetCalendarSchema.ImportCompareOnly
							});
						}
					}
				}
				return InternetCalendarSchema.importCompare;
			}
		}

		internal static PropertyDefinition[] ImportUpdate
		{
			get
			{
				if (InternetCalendarSchema.importUpdate == null)
				{
					lock (InternetCalendarSchema.importUpdateLock)
					{
						if (InternetCalendarSchema.importUpdate == null)
						{
							InternetCalendarSchema.importUpdate = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
							{
								InternetCalendarSchema.ImportCompare,
								InternetCalendarSchema.ImportUpdateOnly
							});
						}
					}
				}
				return InternetCalendarSchema.importUpdate;
			}
		}

		internal static PropertyDefinition[] ImportQuery
		{
			get
			{
				if (InternetCalendarSchema.importQuery == null)
				{
					lock (InternetCalendarSchema.importQueryLock)
					{
						if (InternetCalendarSchema.importQuery == null)
						{
							InternetCalendarSchema.importQuery = Util.MergeArrays<PropertyDefinition>(new ICollection<PropertyDefinition>[]
							{
								InternetCalendarSchema.ImportCompare,
								InternetCalendarSchema.ImportQueryOnly
							});
						}
					}
				}
				return InternetCalendarSchema.importQuery;
			}
		}

		public static PropertyDefinition[] FromDetailLevel(DetailLevelEnumType detailLevel)
		{
			switch (detailLevel)
			{
			case DetailLevelEnumType.AvailabilityOnly:
				return InternetCalendarSchema.ExportFreeBusy;
			case DetailLevelEnumType.LimitedDetails:
				return InternetCalendarSchema.ExportLimitedDetails;
			case DetailLevelEnumType.FullDetails:
				return InternetCalendarSchema.ExportFullDetails;
			default:
				throw new ArgumentOutOfRangeException("detailLevel");
			}
		}

		private static readonly PropertyDefinition[] FreeBusyOnly = new PropertyDefinition[]
		{
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime,
			CalendarItemBaseSchema.MapiIsAllDayEvent,
			CalendarItemBaseSchema.FreeBusyStatus,
			CalendarItemBaseSchema.AppointmentSequenceNumber,
			CalendarItemBaseSchema.AppointmentRecurrenceBlob,
			CalendarItemBaseSchema.TimeZoneDefinitionRecurring,
			ItemSchema.TimeZoneDefinitionStart,
			ItemSchema.Subject
		};

		private static readonly PropertyDefinition[] LimitedDetailOnly = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.Location,
			ItemSchema.Sensitivity
		};

		private static readonly PropertyDefinition[] FullDetailOnly = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly PropertyDefinition[] ExportQueryOnly = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.TimeZone,
			CalendarItemBaseSchema.TimeZoneBlob,
			CalendarItemBaseSchema.GlobalObjectId,
			StoreObjectSchema.ItemClass,
			CalendarItemBaseSchema.IsRecurring,
			CalendarItemBaseSchema.AppointmentRecurring,
			CalendarItemBaseSchema.IsException
		};

		private static readonly PropertyDefinition[] ImportCompareOnly = new PropertyDefinition[]
		{
			ItemSchema.BodyTag
		};

		private static readonly PropertyDefinition[] ImportUpdateOnly = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.GlobalObjectId,
			CalendarItemBaseSchema.CleanGlobalObjectId,
			CalendarItemBaseSchema.TimeZone,
			CalendarItemBaseSchema.TimeZoneBlob,
			ItemSchema.TimeZoneDefinitionStart,
			CalendarItemBaseSchema.TimeZoneDefinitionEnd
		};

		private static readonly PropertyDefinition[] ImportQueryOnly = new PropertyDefinition[]
		{
			ItemSchema.Id,
			CalendarItemBaseSchema.GlobalObjectId,
			CalendarItemBaseSchema.AppointmentRecurring,
			CalendarItemBaseSchema.TimeZone,
			CalendarItemBaseSchema.TimeZoneBlob
		};

		private static PropertyDefinition[] exportFreeBusy;

		private static object exportFreeBusyLock = new object();

		private static PropertyDefinition[] exportLimitedDetails;

		private static object exportLimitedDetailsLock = new object();

		private static PropertyDefinition[] exportFullDetails;

		private static object exportFullDetailsLock = new object();

		private static PropertyDefinition[] importCompare;

		private static object importCompareLock = new object();

		private static PropertyDefinition[] importUpdate;

		private static object importUpdateLock = new object();

		private static PropertyDefinition[] importQuery;

		private static object importQueryLock = new object();
	}
}

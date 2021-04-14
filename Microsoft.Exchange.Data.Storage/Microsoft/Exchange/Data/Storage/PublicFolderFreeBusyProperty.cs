using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class PublicFolderFreeBusyProperty : SmartPropertyDefinition
	{
		public PublicFolderFreeBusyProperty() : base("PublicFolderFreeBusy", typeof(PublicFolderFreeBusy), PropertyFlags.None, PropertyDefinitionConstraint.None, PublicFolderFreeBusyProperty.dependantProps)
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			ExDateTime exDateTime = ExDateTime.MaxValue;
			ExDateTime t = ExDateTime.MinValue;
			List<PublicFolderFreeBusyAppointment> list = new List<PublicFolderFreeBusyAppointment>();
			foreach (PublicFolderFreeBusyProperty.FreeBusyPropertySet freeBusyPropertySet in PublicFolderFreeBusyProperty.FreeBusyPropertySets)
			{
				int[] array = propertyBag.GetValue(freeBusyPropertySet.PublishMonths) as int[];
				if (array != null && array.Length != 0)
				{
					byte[][] array2 = propertyBag.GetValue(freeBusyPropertySet.Appointments) as byte[][];
					object result;
					if (array2 == null || array2.Length == 0)
					{
						PublicFolderFreeBusyProperty.Tracer.TraceError((long)this.GetHashCode(), "PublicFolderFreeBusyProperty::InternalTryGetValue. No data.");
						result = PublicFolderFreeBusyProperty.calculatedPropertyError;
					}
					else
					{
						if (array2.Length == array.Length)
						{
							ExDateTime[] array3 = new ExDateTime[array.Length];
							for (int j = 0; j < array.Length; j++)
							{
								array3[j] = PublicFolderFreeBusyProperty.PublishMonthPropertyConverter.FromInt(array[j]);
							}
							foreach (ExDateTime exDateTime2 in array3)
							{
								if (exDateTime2 < exDateTime)
								{
									exDateTime = exDateTime2;
								}
								if (exDateTime2 > t)
								{
									t = exDateTime2;
								}
							}
							IEnumerable<PublicFolderFreeBusyAppointment> collection = PublicFolderFreeBusyProperty.AppointmentsPropertyConverter.FromBinary(array2, array3, freeBusyPropertySet.BusyType);
							list.AddRange(collection);
							goto IL_16A;
						}
						PublicFolderFreeBusyProperty.Tracer.TraceError<int, int>((long)this.GetHashCode(), "PublicFolderFreeBusyProperty::InternalTryGetValue. Appointments array length {0} does not match the publish month length {1}.", array2.Length, array.Length);
						result = PublicFolderFreeBusyProperty.calculatedPropertyError;
					}
					return result;
				}
				PublicFolderFreeBusyProperty.Tracer.TraceError<BusyType>((long)this.GetHashCode(), "PublicFolderFreeBusyProperty::InternalTryGetValue. Unable to retrieve the publish months information for Property Set {0}.", freeBusyPropertySet.BusyType);
				IL_16A:;
			}
			int numberOfMonths = exDateTime.Year * 12 + exDateTime.Month - (exDateTime.Year * 12 + exDateTime.Month) + 1;
			return new PublicFolderFreeBusy
			{
				StartDate = exDateTime,
				NumberOfMonths = numberOfMonths,
				Appointments = list
			};
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			PublicFolderFreeBusy publicFolderFreeBusy = value as PublicFolderFreeBusy;
			if (publicFolderFreeBusy == null)
			{
				throw new ArgumentException("value");
			}
			IEnumerable<PublicFolderFreeBusyAppointment> sortedAppointments = from appointment in publicFolderFreeBusy.Appointments
			orderby appointment.StartTime
			select appointment;
			ExDateTime startDate = publicFolderFreeBusy.StartDate;
			int numberOfMonths = publicFolderFreeBusy.NumberOfMonths;
			int[] array = new int[numberOfMonths];
			for (int i = 0; i < numberOfMonths; i++)
			{
				ExDateTime startMonth = startDate.AddMonths(i);
				array[i] = PublicFolderFreeBusyProperty.PublishMonthPropertyConverter.ToInt(startMonth);
			}
			foreach (PublicFolderFreeBusyProperty.FreeBusyPropertySet freeBusyPropertySet in PublicFolderFreeBusyProperty.FreeBusyPropertySets)
			{
				byte[][] propertyValue = PublicFolderFreeBusyProperty.AppointmentsPropertyConverter.ToBinary(sortedAppointments, freeBusyPropertySet.BusyType, startDate, numberOfMonths);
				propertyBag.SetValue(freeBusyPropertySet.Appointments, propertyValue);
				propertyBag.SetValue(freeBusyPropertySet.PublishMonths, array);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;

		private static readonly PropertyError calculatedPropertyError = new PropertyError(InternalSchema.PublicFolderFreeBusy, PropertyErrorCode.GetCalculatedPropertyError);

		private static readonly PropertyDependency[] dependantProps = new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ScheduleInfoFreeBusyBusy, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ScheduleInfoMonthsBusy, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ScheduleInfoFreeBusyTentative, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ScheduleInfoMonthsTentative, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ScheduleInfoFreeBusyOof, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ScheduleInfoMonthsOof, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ScheduleInfoFreeBusyMerged, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ScheduleInfoMonthsMerged, PropertyDependencyType.AllRead)
		};

		private static readonly PublicFolderFreeBusyProperty.FreeBusyPropertySet[] FreeBusyPropertySets = new PublicFolderFreeBusyProperty.FreeBusyPropertySet[]
		{
			new PublicFolderFreeBusyProperty.FreeBusyPropertySet
			{
				Appointments = InternalSchema.ScheduleInfoFreeBusyBusy,
				PublishMonths = InternalSchema.ScheduleInfoMonthsBusy,
				BusyType = BusyType.Busy
			},
			new PublicFolderFreeBusyProperty.FreeBusyPropertySet
			{
				Appointments = InternalSchema.ScheduleInfoFreeBusyTentative,
				PublishMonths = InternalSchema.ScheduleInfoMonthsTentative,
				BusyType = BusyType.Tentative
			},
			new PublicFolderFreeBusyProperty.FreeBusyPropertySet
			{
				Appointments = InternalSchema.ScheduleInfoFreeBusyOof,
				PublishMonths = InternalSchema.ScheduleInfoMonthsOof,
				BusyType = BusyType.OOF
			},
			new PublicFolderFreeBusyProperty.FreeBusyPropertySet
			{
				Appointments = InternalSchema.ScheduleInfoFreeBusyMerged,
				PublishMonths = InternalSchema.ScheduleInfoMonthsMerged,
				BusyType = BusyType.Unknown
			}
		};

		private static class AppointmentsPropertyConverter
		{
			public static byte[][] ToBinary(IEnumerable<PublicFolderFreeBusyAppointment> sortedAppointments, BusyType requestedFreeBusyType, ExDateTime startMonth, int numberOfMonths)
			{
				byte[][] array = new byte[numberOfMonths][];
				ExDateTime exDateTime = startMonth.AddMonths(1);
				bool flag = true;
				for (int i = 0; i < array.Length; i++)
				{
					double totalMinutes = (exDateTime - startMonth).TotalMinutes;
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
						{
							ushort num = 0;
							foreach (PublicFolderFreeBusyAppointment publicFolderFreeBusyAppointment in sortedAppointments)
							{
								if (!(publicFolderFreeBusyAppointment.EndTime < startMonth))
								{
									if (publicFolderFreeBusyAppointment.StartTime >= exDateTime)
									{
										break;
									}
									bool flag2 = publicFolderFreeBusyAppointment.BusyType == requestedFreeBusyType || (requestedFreeBusyType == BusyType.Unknown && publicFolderFreeBusyAppointment.BusyType != BusyType.Free && publicFolderFreeBusyAppointment.BusyType != BusyType.WorkingElseWhere);
									if (flag2)
									{
										double totalMinutes2 = (publicFolderFreeBusyAppointment.StartTime - startMonth).TotalMinutes;
										ushort num2;
										if (totalMinutes2 < 0.0)
										{
											num2 = 0;
										}
										else
										{
											num2 = (ushort)totalMinutes2;
										}
										double totalMinutes3 = (publicFolderFreeBusyAppointment.EndTime - startMonth).TotalMinutes;
										ushort val;
										if (totalMinutes3 > totalMinutes)
										{
											val = (ushort)totalMinutes;
										}
										else
										{
											val = (ushort)totalMinutes3;
										}
										if (flag)
										{
											binaryWriter.Write(num2);
											flag = false;
										}
										else if (num2 > num)
										{
											binaryWriter.Write(num);
											binaryWriter.Write(num2);
										}
										num = Math.Max(num, val);
									}
								}
							}
							if (num != 0)
							{
								binaryWriter.Write(num);
							}
						}
						array[i] = memoryStream.ToArray();
						startMonth = exDateTime;
						exDateTime = startMonth.AddMonths(1);
						flag = true;
					}
				}
				return array;
			}

			public static IEnumerable<PublicFolderFreeBusyAppointment> FromBinary(byte[][] binaryData, ExDateTime[] publishMonths, BusyType busyType)
			{
				List<PublicFolderFreeBusyAppointment> list = new List<PublicFolderFreeBusyAppointment>();
				for (int i = 0; i < binaryData.Length; i++)
				{
					ExDateTime exDateTime = publishMonths[i];
					using (MemoryStream memoryStream = new MemoryStream(binaryData[i]))
					{
						using (BinaryReader binaryReader = new BinaryReader(memoryStream))
						{
							while (memoryStream.Position + 4L <= memoryStream.Length)
							{
								ushort num = binaryReader.ReadUInt16();
								ushort num2 = binaryReader.ReadUInt16();
								list.Add(new PublicFolderFreeBusyAppointment(exDateTime.AddMinutes((double)num), exDateTime.AddMinutes((double)num2), busyType));
							}
						}
					}
				}
				return list;
			}
		}

		private static class PublishMonthPropertyConverter
		{
			public static int ToInt(ExDateTime startMonth)
			{
				return startMonth.Year * 16 + startMonth.Month;
			}

			public static ExDateTime FromInt(int publishMonth)
			{
				int year = publishMonth >> 4;
				int month = publishMonth & 15;
				return new ExDateTime(ExTimeZone.UtcTimeZone, year, month, 1);
			}
		}

		private sealed class FreeBusyPropertySet
		{
			public PropertyTagPropertyDefinition Appointments { get; set; }

			public PropertyTagPropertyDefinition PublishMonths { get; set; }

			public BusyType BusyType { get; set; }
		}
	}
}

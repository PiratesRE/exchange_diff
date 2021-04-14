using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyChangeMetadata
	{
		static PropertyChangeMetadata()
		{
			PropertyChangeMetadata.PropertyNameToPropertyAndIndex = new Dictionary<string, PropertyChangeMetadata.PropertyAndIndex>();
			for (int i = 0; i < PropertyChangeMetadata.ListOfTrackedPropertyGroups.Length; i++)
			{
				PropertyChangeMetadata.PropertyGroup propertyGroup = PropertyChangeMetadata.ListOfTrackedPropertyGroups[i];
				if (propertyGroup != null)
				{
					StorePropertyDefinition storeProperty = propertyGroup.StoreProperty;
					if (storeProperty != null && !(storeProperty is NativeStorePropertyDefinition))
					{
						PropertyChangeMetadata.CheckAndAddProperty(storeProperty.Name, storeProperty, propertyGroup, i);
					}
					else if (propertyGroup.IsBitField)
					{
						PropertyChangeMetadata.CheckAndAddProperty(propertyGroup.Name, null, propertyGroup, i);
					}
					foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in PropertyChangeMetadata.ListOfTrackedPropertyGroups[i])
					{
						PropertyChangeMetadata.CheckAndAddProperty(nativeStorePropertyDefinition.Name, nativeStorePropertyDefinition, propertyGroup, i);
					}
				}
			}
		}

		public PropertyChangeMetadata() : this(new BitArray(PropertyChangeMetadata.ListOfTrackedPropertyGroups.Length), 0)
		{
		}

		private PropertyChangeMetadata(BitArray metadataBitArray, int flags = 0)
		{
			this.masterPropertyOverrideGroupsBitArray = metadataBitArray;
			this.flags = flags;
		}

		internal bool AreAllPropertiesExceptions
		{
			get
			{
				return (this.flags & 1) == 1;
			}
		}

		public static PropertyChangeMetadata Parse(byte[] rawMetadata)
		{
			if (rawMetadata == null)
			{
				throw new ArgumentException("rawMetadata");
			}
			Exception innerException = null;
			try
			{
				if (rawMetadata.Length < 12)
				{
					ExTraceGlobals.CalendarSeriesTracer.TraceError(0L, "PropertyChangeMetadata::Parse. Byte stream is shorter than minimum raw metadata size.");
					throw new PropertyChangeMetadataFormatException(ServerStrings.PropertyChangeMetadataParseError);
				}
				using (MemoryStream memoryStream = new MemoryStream(rawMetadata))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.ReadInt32();
						int num = binaryReader.ReadInt32();
						int num2 = binaryReader.ReadInt32();
						byte[] array = binaryReader.ReadBytes(num2);
						if (array.Length < num2)
						{
							ExTraceGlobals.CalendarSeriesTracer.TraceError(0L, "PropertyChangeMetadata::Parse. Byte stream truncated. Not able to read serialized bits till the end.");
							throw new PropertyChangeMetadataFormatException(ServerStrings.PropertyChangeMetadataParseError);
						}
						byte[] array2 = array;
						if (num2 * 8 < PropertyChangeMetadata.ListOfTrackedPropertyGroups.Length)
						{
							array2 = new byte[PropertyChangeMetadata.TrackedPropertyGroupByteArraySize];
							Array.Copy(array, array2, array.Length);
						}
						return new PropertyChangeMetadata(new BitArray(array2), num);
					}
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.CalendarSeriesTracer.TraceError<ArgumentException>(0L, "PropertyChangeMetadata::Parse. Error parsing property change metadata. Ex: {0}", ex);
				innerException = ex;
			}
			catch (IOException ex2)
			{
				ExTraceGlobals.CalendarSeriesTracer.TraceError<IOException>(0L, "PropertyChangeMetadata::Parse. Error parsing property change metadata. Ex: {0}", ex2);
				innerException = ex2;
			}
			throw new PropertyChangeMetadataFormatException(ServerStrings.PropertyChangeMetadataParseError, innerException);
		}

		public static bool IsPropertyTracked(StorePropertyDefinition property)
		{
			return PropertyChangeMetadata.PropertyNameToPropertyAndIndex.ContainsKey(property.Name);
		}

		public static bool TryGetTrackedPropertyForName(string propertyName, out StorePropertyDefinition property)
		{
			property = null;
			PropertyChangeMetadata.PropertyAndIndex propertyAndIndex;
			if (PropertyChangeMetadata.PropertyNameToPropertyAndIndex.TryGetValue(propertyName, out propertyAndIndex))
			{
				property = propertyAndIndex.Property;
				return true;
			}
			return false;
		}

		public static PropertyChangeMetadata.PropertyGroup GetGroupForProperty(StorePropertyDefinition property)
		{
			return PropertyChangeMetadata.GetGroupForPropertyName(property.Name);
		}

		public static PropertyChangeMetadata.PropertyGroup GetGroupForPropertyName(string propertyName)
		{
			int num;
			if (!PropertyChangeMetadata.TryGetMetadataIndexForProperty(propertyName, out num))
			{
				return null;
			}
			return PropertyChangeMetadata.ListOfTrackedPropertyGroups[num];
		}

		public static PropertyChangeMetadata Merge(PropertyChangeMetadata metadata1, PropertyChangeMetadata metadata2)
		{
			if (metadata1 == null && metadata2 == null)
			{
				return null;
			}
			metadata1 = (metadata1 ?? new PropertyChangeMetadata());
			metadata2 = (metadata2 ?? new PropertyChangeMetadata());
			PropertyChangeMetadata propertyChangeMetadata = new PropertyChangeMetadata();
			BitArray bitArray = propertyChangeMetadata.masterPropertyOverrideGroupsBitArray;
			BitArray bitArray2 = metadata1.masterPropertyOverrideGroupsBitArray;
			BitArray bitArray3 = metadata2.masterPropertyOverrideGroupsBitArray;
			int length = Math.Max(Math.Max(((ICollection)bitArray2).Count, ((ICollection)bitArray3).Count), ((ICollection)bitArray).Count);
			bitArray2.Length = length;
			bitArray3.Length = length;
			bitArray.Length = length;
			bitArray.Or(bitArray2);
			bitArray.Or(bitArray3);
			propertyChangeMetadata.flags = (metadata1.flags | metadata2.flags);
			return propertyChangeMetadata;
		}

		public byte[] ToByteArray()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(1);
					binaryWriter.Write(this.flags);
					byte[] array = new byte[PropertyChangeMetadata.GetByteArraySize(this.masterPropertyOverrideGroupsBitArray.Length)];
					((ICollection)this.masterPropertyOverrideGroupsBitArray).CopyTo(array, 0);
					binaryWriter.Write(array.Length);
					binaryWriter.Write(array);
					binaryWriter.Flush();
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public IEnumerable<PropertyChangeMetadata.PropertyGroup> GetOverriddenGroups()
		{
			if (this.AreAllPropertiesExceptions)
			{
				return from propGroup in PropertyChangeMetadata.ListOfTrackedPropertyGroups
				where propGroup != null
				select propGroup;
			}
			return this.GetOverriddenGroupsFromBitMapOverride();
		}

		public IEnumerable<NativeStorePropertyDefinition> GetTrackedNonOverrideNativeStorePropertyDefinitions()
		{
			if (this.AreAllPropertiesExceptions)
			{
				return PropertyChangeMetadata.EmptyNativeStorePropertyList;
			}
			return from propertyAndIndex in PropertyChangeMetadata.PropertyNameToPropertyAndIndex
			let nativeProperty = propertyAndIndex.Value.Property as NativeStorePropertyDefinition
			where nativeProperty != null && !this.masterPropertyOverrideGroupsBitArray[propertyAndIndex.Value.Index]
			select nativeProperty;
		}

		public IEnumerable<string> GetTrackedNonOverrideStorePropertyNames()
		{
			if (this.AreAllPropertiesExceptions)
			{
				return PropertyChangeMetadata.EmptyStorePropertyNameList;
			}
			return from nameAndIndexPair in PropertyChangeMetadata.PropertyNameToPropertyAndIndex
			where nameAndIndexPair.Key != null && !this.masterPropertyOverrideGroupsBitArray[nameAndIndexPair.Value.Index]
			select nameAndIndexPair.Key;
		}

		public void MarkAllAsException()
		{
			this.flags |= 1;
		}

		public bool IsMasterPropertyOverride(string propertyName)
		{
			int index;
			return !PropertyChangeMetadata.TryGetMetadataIndexForProperty(propertyName, out index) || this.AreAllPropertiesExceptions || this.masterPropertyOverrideGroupsBitArray[index];
		}

		public void MarkAsMasterPropertyOverride(string propertyName)
		{
			int index;
			if (PropertyChangeMetadata.TryGetMetadataIndexForProperty(propertyName, out index))
			{
				this.masterPropertyOverrideGroupsBitArray[index] = true;
			}
		}

		private static void CheckAndAddProperty(string name, StorePropertyDefinition property, PropertyChangeMetadata.PropertyGroup propertyGroup, int index)
		{
			if (propertyGroup.IsBitField)
			{
				PropertyFlags propertyFlags = propertyGroup.ContainerStoreProperty.PropertyFlags;
			}
			PropertyChangeMetadata.PropertyNameToPropertyAndIndex.Add(name, new PropertyChangeMetadata.PropertyAndIndex
			{
				Property = property,
				Index = index
			});
		}

		private static int GetByteArraySize(int bitCount)
		{
			return (bitCount - 1) / 8 + 1;
		}

		private static bool TryGetMetadataIndexForProperty(string propertyName, out int index)
		{
			if (PropertyChangeMetadata.PropertyNameToPropertyAndIndex.ContainsKey(propertyName))
			{
				index = PropertyChangeMetadata.PropertyNameToPropertyAndIndex[propertyName].Index;
				return true;
			}
			index = -1;
			return false;
		}

		private IEnumerable<PropertyChangeMetadata.PropertyGroup> GetOverriddenGroupsFromBitMapOverride()
		{
			int bitmapLength = this.masterPropertyOverrideGroupsBitArray.Length;
			int referenceGroupsLength = PropertyChangeMetadata.ListOfTrackedPropertyGroups.Length;
			int groupCount;
			if (bitmapLength < referenceGroupsLength)
			{
				ExTraceGlobals.CalendarSeriesTracer.TraceError(0L, "PropertyChangeMetadata::GetOverridenGroups. Bitmap is shorter than minimum raw metadata size.");
				groupCount = bitmapLength;
			}
			else
			{
				groupCount = referenceGroupsLength;
				if (referenceGroupsLength < bitmapLength)
				{
					ExTraceGlobals.CalendarSeriesTracer.TraceError(0L, "PropertyChangeMetadata::GetOverridenGroups. Bitmap truncated. Not able to read serialized bits till the end.");
				}
			}
			for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
			{
				PropertyChangeMetadata.PropertyGroup group = PropertyChangeMetadata.ListOfTrackedPropertyGroups[groupIndex];
				if (group != null && this.masterPropertyOverrideGroupsBitArray[groupIndex])
				{
					yield return group;
				}
			}
			yield break;
		}

		private const int CurrentVersion = 1;

		private const int MinimumRawMetadataSize = 12;

		private const int BitsPerByte = 8;

		private const int AllPropertiesAreExceptionsFlag = 1;

		private static readonly Dictionary<string, PropertyChangeMetadata.PropertyAndIndex> PropertyNameToPropertyAndIndex;

		private static readonly NativeStorePropertyDefinition[] EmptyNativeStorePropertyList = Array<NativeStorePropertyDefinition>.Empty;

		private static readonly string[] EmptyStorePropertyNameList = Array<string>.Empty;

		private static readonly PropertyChangeMetadata.PropertyGroup[] ListOfTrackedPropertyGroups = new PropertyChangeMetadata.PropertyGroup[]
		{
			PropertyChangeMetadata.PropertyGroup.Subject,
			PropertyChangeMetadata.PropertyGroup.Body,
			PropertyChangeMetadata.PropertyGroup.Location,
			PropertyChangeMetadata.PropertyGroup.ReminderIsSet,
			PropertyChangeMetadata.PropertyGroup.ReminderTime,
			PropertyChangeMetadata.PropertyGroup.FreeBusy,
			PropertyChangeMetadata.PropertyGroup.Attachments,
			PropertyChangeMetadata.PropertyGroup.Color,
			PropertyChangeMetadata.PropertyGroup.Sensitivity,
			PropertyChangeMetadata.PropertyGroup.Importance,
			PropertyChangeMetadata.PropertyGroup.Categories,
			PropertyChangeMetadata.PropertyGroup.Response,
			null,
			PropertyChangeMetadata.PropertyGroup.DisallowNewTimeProposal,
			PropertyChangeMetadata.PropertyGroup.IsMeeting,
			PropertyChangeMetadata.PropertyGroup.IsCancelled,
			PropertyChangeMetadata.PropertyGroup.IsForward
		};

		private static readonly int TrackedPropertyGroupByteArraySize = PropertyChangeMetadata.GetByteArraySize(PropertyChangeMetadata.ListOfTrackedPropertyGroups.Length);

		private readonly BitArray masterPropertyOverrideGroupsBitArray;

		private int flags;

		internal struct PropertyAndIndex
		{
			internal StorePropertyDefinition Property;

			internal int Index;
		}

		public class PropertyGroup : IEnumerable<NativeStorePropertyDefinition>, IEnumerable
		{
			private PropertyGroup(string name, params StorePropertyDefinition[] properties)
			{
				this.AddProperties(properties);
				this.Name = name;
			}

			private PropertyGroup(StorePropertyDefinition property)
			{
				this.AddProperties(new StorePropertyDefinition[]
				{
					property
				});
				this.StoreProperty = property;
				this.Name = property.Name;
			}

			private PropertyGroup(string propertyName, StorePropertyDefinition property, StorePropertyDefinition containerProperty, int offset)
			{
				this.Name = propertyName;
				this.StoreProperty = property;
				this.ContainerStoreProperty = containerProperty;
				this.ContainerFlag = offset;
			}

			public StorePropertyDefinition StoreProperty { get; private set; }

			public string Name { get; private set; }

			public StorePropertyDefinition ContainerStoreProperty { get; private set; }

			public int ContainerFlag { get; private set; }

			public bool IsBitField
			{
				get
				{
					return this.ContainerStoreProperty != null;
				}
			}

			public IEnumerator<NativeStorePropertyDefinition> GetEnumerator()
			{
				return this.properties.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.properties.GetEnumerator();
			}

			private void AddProperties(params StorePropertyDefinition[] propertiesToAdd)
			{
				ICollection<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<StorePropertyDefinition>(PropertyDependencyType.NeedForRead, propertiesToAdd);
				this.properties.AddRange(nativePropertyDefinitions);
			}

			public static readonly PropertyChangeMetadata.PropertyGroup Attachments = new PropertyChangeMetadata.PropertyGroup(InternalSchema.MapiHasAttachment);

			public static readonly PropertyChangeMetadata.PropertyGroup Body = new PropertyChangeMetadata.PropertyGroup("Body", new StorePropertyDefinition[]
			{
				InternalSchema.TextBody,
				InternalSchema.HtmlBody,
				InternalSchema.RtfBody
			});

			public static readonly PropertyChangeMetadata.PropertyGroup Categories = new PropertyChangeMetadata.PropertyGroup(InternalSchema.Categories);

			public static readonly PropertyChangeMetadata.PropertyGroup Color = new PropertyChangeMetadata.PropertyGroup(InternalSchema.AppointmentColor);

			public static readonly PropertyChangeMetadata.PropertyGroup DisallowNewTimeProposal = new PropertyChangeMetadata.PropertyGroup(InternalSchema.DisallowNewTimeProposal);

			public static readonly PropertyChangeMetadata.PropertyGroup EndTime = new PropertyChangeMetadata.PropertyGroup("EndTime", new StorePropertyDefinition[]
			{
				CalendarItemBaseSchema.ClipEndTime,
				CalendarItemInstanceSchema.EndTime
			});

			public static readonly PropertyChangeMetadata.PropertyGroup FreeBusy = new PropertyChangeMetadata.PropertyGroup(InternalSchema.FreeBusyStatus);

			public static readonly PropertyChangeMetadata.PropertyGroup Importance = new PropertyChangeMetadata.PropertyGroup("Importance", new StorePropertyDefinition[]
			{
				InternalSchema.MapiImportance,
				InternalSchema.MapiPriority
			});

			public static readonly PropertyChangeMetadata.PropertyGroup Location = new PropertyChangeMetadata.PropertyGroup("Location", CalendarItemProperties.EnhancedLocationPropertyDefinitions.Concat(new NativeStorePropertyDefinition[]
			{
				InternalSchema.Location,
				InternalSchema.OldLocation,
				InternalSchema.LidWhere,
				InternalSchema.LocationAddressInternal
			}).ToArray<StorePropertyDefinition>());

			public static readonly PropertyChangeMetadata.PropertyGroup ReminderIsSet = new PropertyChangeMetadata.PropertyGroup("Reminder", new StorePropertyDefinition[]
			{
				InternalSchema.ReminderIsSetInternal
			});

			public static readonly PropertyChangeMetadata.PropertyGroup ReminderTime = new PropertyChangeMetadata.PropertyGroup("ReminderTime", new StorePropertyDefinition[]
			{
				InternalSchema.ReminderMinutesBeforeStartInternal
			});

			public static readonly PropertyChangeMetadata.PropertyGroup Response = new PropertyChangeMetadata.PropertyGroup("Response", new StorePropertyDefinition[]
			{
				CalendarItemBaseSchema.ResponseType
			});

			public static readonly PropertyChangeMetadata.PropertyGroup Sensitivity = new PropertyChangeMetadata.PropertyGroup("Sensitivity", new StorePropertyDefinition[]
			{
				InternalSchema.MapiSensitivity,
				InternalSchema.Privacy
			});

			public static readonly PropertyChangeMetadata.PropertyGroup StartTime = new PropertyChangeMetadata.PropertyGroup("StartTime", new StorePropertyDefinition[]
			{
				CalendarItemBaseSchema.ClipStartTime,
				CalendarItemInstanceSchema.StartTime
			});

			public static readonly PropertyChangeMetadata.PropertyGroup IsMeeting = new PropertyChangeMetadata.PropertyGroup(CalendarItemBaseSchema.IsMeeting.Name, CalendarItemBaseSchema.IsMeeting, InternalSchema.AppointmentStateInternal, 1);

			public static readonly PropertyChangeMetadata.PropertyGroup IsCancelled = new PropertyChangeMetadata.PropertyGroup("IsCancelled", null, InternalSchema.AppointmentStateInternal, 4);

			public static readonly PropertyChangeMetadata.PropertyGroup IsForward = new PropertyChangeMetadata.PropertyGroup("IsForward", null, InternalSchema.AppointmentStateInternal, 8);

			public static readonly PropertyChangeMetadata.PropertyGroup Subject = new PropertyChangeMetadata.PropertyGroup(InternalSchema.Subject);

			private readonly List<NativeStorePropertyDefinition> properties = new List<NativeStorePropertyDefinition>();
		}
	}
}

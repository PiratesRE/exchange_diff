using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal class PropertyState
	{
		public int UndoStackTop
		{
			get
			{
				return this.propertyUndoStackTop;
			}
		}

		public FlagProperties GetEffectiveFlags()
		{
			return this.flagProperties;
		}

		public FlagProperties GetDistinctFlags()
		{
			return this.distinctFlagProperties;
		}

		public PropertyValue GetEffectiveProperty(PropertyId id)
		{
			if (FlagProperties.IsFlagProperty(id))
			{
				return this.flagProperties.GetPropertyValue(id);
			}
			if (this.propertyMask.IsSet(id))
			{
				return this.properties[(int)(id - PropertyId.FontColor)];
			}
			return PropertyValue.Null;
		}

		public PropertyValue GetDistinctProperty(PropertyId id)
		{
			if (FlagProperties.IsFlagProperty(id))
			{
				return this.distinctFlagProperties.GetPropertyValue(id);
			}
			if (this.distinctPropertyMask.IsSet(id))
			{
				return this.properties[(int)(id - PropertyId.FontColor)];
			}
			return PropertyValue.Null;
		}

		public void SubtractDefaultFromDistinct(FlagProperties defaultFlags, Property[] defaultProperties)
		{
			FlagProperties y = defaultFlags ^ this.distinctFlagProperties;
			FlagProperties y2 = (this.distinctFlagProperties & y) | (this.distinctFlagProperties & ~defaultFlags);
			if (this.distinctFlagProperties != y2)
			{
				this.PushUndoEntry((PropertyId)74, this.distinctFlagProperties);
				this.distinctFlagProperties = y2;
			}
			if (defaultProperties != null)
			{
				bool flag = false;
				foreach (Property property in defaultProperties)
				{
					if (this.distinctPropertyMask.IsSet(property.Id) && this.properties[(int)(property.Id - PropertyId.FontColor)] == property.Value)
					{
						if (!flag)
						{
							this.PushUndoEntry(this.distinctPropertyMask);
							flag = true;
						}
						this.distinctPropertyMask.Clear(property.Id);
					}
				}
			}
		}

		public int ApplyProperties(FlagProperties flagProperties, Property[] propList, FlagProperties flagInheritanceMask, PropertyBitMask propertyInheritanceMask)
		{
			int result = this.propertyUndoStackTop;
			FlagProperties x = this.flagProperties & flagInheritanceMask;
			FlagProperties x2 = x | flagProperties;
			if (x2 != this.flagProperties)
			{
				this.PushUndoEntry(PropertyId.MaxValue, this.flagProperties);
				this.flagProperties = x2;
			}
			FlagProperties y = x ^ flagProperties;
			FlagProperties x3 = (flagProperties & y) | (flagProperties & ~x);
			if (x3 != this.distinctFlagProperties)
			{
				this.PushUndoEntry((PropertyId)74, this.distinctFlagProperties);
				this.distinctFlagProperties = x3;
			}
			PropertyBitMask propertyBitMask = this.propertyMask & ~propertyInheritanceMask;
			foreach (PropertyId propertyId in propertyBitMask)
			{
				this.PushUndoEntry(propertyId, this.properties[(int)(propertyId - PropertyId.FontColor)]);
			}
			PropertyBitMask allOff = PropertyBitMask.AllOff;
			this.propertyMask &= propertyInheritanceMask;
			if (propList != null)
			{
				foreach (Property property in propList)
				{
					if (this.propertyMask.IsSet(property.Id))
					{
						if (this.properties[(int)(property.Id - PropertyId.FontColor)] != property.Value)
						{
							this.PushUndoEntry(property.Id, this.properties[(int)(property.Id - PropertyId.FontColor)]);
							if (property.Value.IsNull)
							{
								this.propertyMask.Clear(property.Id);
							}
							else
							{
								this.properties[(int)(property.Id - PropertyId.FontColor)] = property.Value;
								allOff.Set(property.Id);
							}
						}
					}
					else if (!property.Value.IsNull)
					{
						if (!propertyBitMask.IsSet(property.Id))
						{
							this.PushUndoEntry(property.Id, PropertyValue.Null);
						}
						this.properties[(int)(property.Id - PropertyId.FontColor)] = property.Value;
						this.propertyMask.Set(property.Id);
						allOff.Set(property.Id);
					}
				}
			}
			if (allOff != this.distinctPropertyMask)
			{
				this.PushUndoEntry(this.distinctPropertyMask);
				this.distinctPropertyMask = allOff;
			}
			return result;
		}

		public void UndoProperties(int undoLevel)
		{
			for (int i = this.propertyUndoStackTop - 1; i >= undoLevel; i--)
			{
				if (this.propertyUndoStack[i].IsFlags)
				{
					this.flagProperties = this.propertyUndoStack[i].Flags.Flags;
				}
				else if (this.propertyUndoStack[i].IsDistinctFlags)
				{
					this.distinctFlagProperties = this.propertyUndoStack[i].Flags.Flags;
				}
				else if (this.propertyUndoStack[i].IsDistinctMask1)
				{
					this.distinctPropertyMask.Set1(this.propertyUndoStack[i].Bits.Bits);
				}
				else if (this.propertyUndoStack[i].IsDistinctMask2)
				{
					this.distinctPropertyMask.Set2(this.propertyUndoStack[i].Bits.Bits);
				}
				else if (this.propertyUndoStack[i].Property.Value.IsNull)
				{
					this.propertyMask.Clear(this.propertyUndoStack[i].Property.Id);
				}
				else
				{
					this.properties[(int)(this.propertyUndoStack[i].Property.Id - PropertyId.FontColor)] = this.propertyUndoStack[i].Property.Value;
					this.propertyMask.Set(this.propertyUndoStack[i].Property.Id);
				}
			}
			this.propertyUndoStackTop = undoLevel;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"flags: (",
				this.flagProperties.ToString(),
				"), props: (",
				this.propertyMask.ToString(),
				"), dflags: (",
				this.distinctFlagProperties.ToString(),
				"), dprops: (",
				this.distinctPropertyMask.ToString(),
				")"
			});
		}

		private void PushUndoEntry(PropertyId id, PropertyValue value)
		{
			if (this.propertyUndoStackTop == this.propertyUndoStack.Length)
			{
				if (this.propertyUndoStack.Length >= 8960)
				{
					throw new TextConvertersException("property undo stack is too large");
				}
				int num = Math.Min(this.propertyUndoStack.Length * 2, 8960);
				PropertyState.PropertyUndoEntry[] destinationArray = new PropertyState.PropertyUndoEntry[num];
				Array.Copy(this.propertyUndoStack, 0, destinationArray, 0, this.propertyUndoStackTop);
				this.propertyUndoStack = destinationArray;
			}
			this.propertyUndoStack[this.propertyUndoStackTop++].Set(id, value);
		}

		private void PushUndoEntry(PropertyId fakePropId, FlagProperties flagProperties)
		{
			if (this.propertyUndoStackTop == this.propertyUndoStack.Length)
			{
				if (this.propertyUndoStack.Length >= 8960)
				{
					throw new TextConvertersException("property undo stack is too large");
				}
				int num = Math.Min(this.propertyUndoStack.Length * 2, 8960);
				PropertyState.PropertyUndoEntry[] destinationArray = new PropertyState.PropertyUndoEntry[num];
				Array.Copy(this.propertyUndoStack, 0, destinationArray, 0, this.propertyUndoStackTop);
				this.propertyUndoStack = destinationArray;
			}
			this.propertyUndoStack[this.propertyUndoStackTop++].Set(fakePropId, flagProperties);
		}

		private void PushUndoEntry(PropertyBitMask propertyMask)
		{
			if (this.propertyUndoStackTop + 1 >= this.propertyUndoStack.Length)
			{
				if (this.propertyUndoStackTop + 2 >= 8960)
				{
					throw new TextConvertersException("property undo stack is too large");
				}
				int num = Math.Min(this.propertyUndoStack.Length * 2, 8960);
				PropertyState.PropertyUndoEntry[] destinationArray = new PropertyState.PropertyUndoEntry[num];
				Array.Copy(this.propertyUndoStack, 0, destinationArray, 0, this.propertyUndoStackTop);
				this.propertyUndoStack = destinationArray;
			}
			this.propertyUndoStack[this.propertyUndoStackTop++].Set((PropertyId)75, propertyMask.Bits1);
			this.propertyUndoStack[this.propertyUndoStackTop++].Set((PropertyId)76, propertyMask.Bits2);
		}

		private const int MaxStackSize = 8960;

		private FlagProperties flagProperties;

		private FlagProperties distinctFlagProperties;

		private PropertyBitMask propertyMask;

		private PropertyBitMask distinctPropertyMask;

		private PropertyValue[] properties = new PropertyValue[56];

		private PropertyState.PropertyUndoEntry[] propertyUndoStack = new PropertyState.PropertyUndoEntry[146];

		private int propertyUndoStackTop;

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		private struct FlagPropertiesUndo
		{
			public PropertyId FakeId;

			public FlagProperties Flags;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		private struct BitsUndo
		{
			public PropertyId FakeId;

			public uint Bits;
		}

		[StructLayout(LayoutKind.Explicit, Pack = 2)]
		private struct PropertyUndoEntry
		{
			public bool IsFlags
			{
				get
				{
					return this.Property.Id == PropertyId.MaxValue;
				}
			}

			public bool IsDistinctFlags
			{
				get
				{
					return this.Property.Id == (PropertyId)74;
				}
			}

			public bool IsDistinctMask1
			{
				get
				{
					return this.Property.Id == (PropertyId)75;
				}
			}

			public bool IsDistinctMask2
			{
				get
				{
					return this.Property.Id == (PropertyId)76;
				}
			}

			public void Set(PropertyId id, PropertyValue value)
			{
				this.Property.Set(id, value);
			}

			public void Set(PropertyId fakePropId, FlagProperties flagProperties)
			{
				this.Flags.FakeId = fakePropId;
				this.Flags.Flags = flagProperties;
			}

			public void Set(PropertyId fakePropId, uint bits)
			{
				this.Bits.FakeId = fakePropId;
				this.Bits.Bits = bits;
			}

			public const PropertyId FlagPropertiesFakeId = PropertyId.MaxValue;

			public const PropertyId DistinctFlagPropertiesFakeId = (PropertyId)74;

			public const PropertyId DistinctMask1FakeId = (PropertyId)75;

			public const PropertyId DistinctMask2FakeId = (PropertyId)76;

			[FieldOffset(0)]
			public Property Property;

			[FieldOffset(0)]
			public PropertyState.FlagPropertiesUndo Flags;

			[FieldOffset(0)]
			public PropertyState.BitsUndo Bits;
		}
	}
}

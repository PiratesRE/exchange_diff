using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class Field
	{
		public Field(object data)
		{
			ExTraceGlobals.FieldTracer.Information((long)this.GetHashCode(), "Field::Field - initializing Field with data {0}.", new object[]
			{
				(data == null) ? "null" : data
			});
			this.data = data;
			this.changeTypeFlags = Field.ChangeTypeFlags.Unchanged;
		}

		public object Data
		{
			get
			{
				ExTraceGlobals.FieldTracer.Information((long)this.GetHashCode(), "Field::Data - getting data {0}.", new object[]
				{
					(this.data == null) ? "null" : this.data
				});
				return this.data;
			}
			set
			{
				ExTraceGlobals.FieldTracer.Information((long)this.GetHashCode(), "Field::Data - setting data to {0}.", new object[]
				{
					(value == null) ? "null" : value
				});
				int num = (this.data == null) ? 0 : this.data.GetHashCode();
				int num2 = (value == null) ? 0 : value.GetHashCode();
				Type left = (this.data == null) ? null : this.data.GetType();
				Type right = (value == null) ? null : value.GetType();
				bool flag = num == num2 && left == right;
				this.changeTypeFlags |= Field.ChangeTypeFlags.Modified;
				if (!flag)
				{
					this.changeTypeFlags |= Field.ChangeTypeFlags.Changed;
				}
				this.data = value;
			}
		}

		public bool IsChanged
		{
			get
			{
				ExTraceGlobals.FieldTracer.Information((long)this.GetHashCode(), "Field::IsChanged - checking if field was touched.");
				return 0 != (byte)(Field.ChangeTypeFlags.Changed & this.changeTypeFlags);
			}
		}

		public bool IsModified
		{
			get
			{
				ExTraceGlobals.FieldTracer.Information((long)this.GetHashCode(), "Field::IsModified - checking if data was changed.");
				return 0 != (byte)(Field.ChangeTypeFlags.Modified & this.changeTypeFlags);
			}
		}

		public void ResetChangeTracking()
		{
			ExTraceGlobals.FieldTracer.Information((long)this.GetHashCode(), "Field::ResetChangeTracking - resetting change tracking.");
			this.changeTypeFlags = Field.ChangeTypeFlags.Unchanged;
		}

		private object data;

		private Field.ChangeTypeFlags changeTypeFlags;

		[Flags]
		private enum ChangeTypeFlags : byte
		{
			Unchanged = 0,
			Modified = 1,
			Changed = 2
		}
	}
}

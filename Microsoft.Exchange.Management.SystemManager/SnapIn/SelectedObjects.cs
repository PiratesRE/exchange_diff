using System;
using System.Collections;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal sealed class SelectedObjects : IComparable
	{
		public SelectedObjects(ICollection selectedObjects)
		{
			this.selectedObjects = selectedObjects;
		}

		public int Count
		{
			get
			{
				return this.selectedObjects.Count;
			}
		}

		private object[] GetSortedArray()
		{
			object[] array = this.selectedObjects as object[];
			if (!this.isSortedArray)
			{
				if (array == null)
				{
					array = new object[this.selectedObjects.Count];
					this.selectedObjects.CopyTo(array, 0);
				}
				ExTraceGlobals.ProgramFlowTracer.TracePerformance<int>(0L, "-->SelectedObjects.GetSortedArray: sorting {0} objects.", array.Length);
				Array.Sort(array, new SelectedObjects.ADObjectIdComparer());
				ExTraceGlobals.ProgramFlowTracer.TracePerformance<int>(0L, "<--SelectedObjects.GetSortedArray: sorting {0} objects.", array.Length);
				this.selectedObjects = array;
				this.isSortedArray = true;
			}
			return array;
		}

		public int CompareTo(object obj)
		{
			if (obj == this)
			{
				return 0;
			}
			SelectedObjects selectedObjects = obj as SelectedObjects;
			if (selectedObjects == null)
			{
				return -1;
			}
			if (this.Count != selectedObjects.Count)
			{
				return this.Count - selectedObjects.Count;
			}
			object[] sortedArray = this.GetSortedArray();
			object[] sortedArray2 = selectedObjects.GetSortedArray();
			for (int i = 0; i < sortedArray.Length; i++)
			{
				if (!object.Equals(sortedArray[i], sortedArray2[i]))
				{
					return -1;
				}
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			return this.CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				object[] sortedArray = this.GetSortedArray();
				this.hashCode = sortedArray.Length.GetHashCode();
				foreach (object obj in sortedArray)
				{
					this.hashCode = (this.hashCode << 13 | (int)((uint)this.hashCode >> 19));
					if (obj != null)
					{
						this.hashCode ^= obj.GetHashCode();
					}
				}
			}
			return this.hashCode;
		}

		private bool isSortedArray;

		private ICollection selectedObjects;

		private int hashCode;

		private class ADObjectIdComparer : IComparer
		{
			int IComparer.Compare(object x, object y)
			{
				int result = 0;
				ADObjectId adobjectId = x as ADObjectId;
				ADObjectId adobjectId2 = y as ADObjectId;
				if (adobjectId != null && adobjectId2 != null)
				{
					result = adobjectId.ObjectGuid.CompareTo((y as ADObjectId).ObjectGuid);
				}
				else if (x is IComparable && y is IComparable)
				{
					result = (x as IComparable).CompareTo(y);
				}
				return result;
			}
		}
	}
}

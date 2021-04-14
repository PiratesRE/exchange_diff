using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class CategorizedTableCollapseState
	{
		public CategorizedTableCollapseState(int categoryCount, int levelsInitiallyExpanded)
		{
			this.levelsInitiallyExpanded = levelsInitiallyExpanded;
			this.visibilityStates = new HashSet<long>[categoryCount];
			for (int i = 1; i < categoryCount; i++)
			{
				this.visibilityStates[i] = new HashSet<long>();
			}
			this.collapseStates = new HashSet<long>[categoryCount];
			for (int j = 0; j < categoryCount; j++)
			{
				this.collapseStates[j] = new HashSet<long>();
			}
		}

		public int LevelsInitiallyExpanded
		{
			get
			{
				return this.levelsInitiallyExpanded;
			}
		}

		public void SetHeaderCollapseState(int headerLevel, long categoryId, bool isExpanded)
		{
			bool flag;
			if (this.IsHeaderLevelExpandedByDefault(headerLevel))
			{
				flag = !isExpanded;
			}
			else
			{
				flag = isExpanded;
			}
			if (flag)
			{
				this.collapseStates[headerLevel].Add(categoryId);
				return;
			}
			this.collapseStates[headerLevel].Remove(categoryId);
		}

		public void SetHeaderVisibility(int headerLevel, long categoryId, bool isVisible)
		{
			if (headerLevel > 0)
			{
				bool flag;
				if (this.IsHeaderLevelVisibleByDefault(headerLevel))
				{
					flag = !isVisible;
				}
				else
				{
					flag = isVisible;
				}
				if (flag)
				{
					this.visibilityStates[headerLevel].Add(categoryId);
					return;
				}
				this.visibilityStates[headerLevel].Remove(categoryId);
			}
		}

		public bool IsHeaderExpanded(int headerLevel, long categoryId)
		{
			bool result;
			if (this.collapseStates[headerLevel].Contains(categoryId))
			{
				result = !this.IsHeaderLevelExpandedByDefault(headerLevel);
			}
			else
			{
				result = this.IsHeaderLevelExpandedByDefault(headerLevel);
			}
			return result;
		}

		public bool IsHeaderVisible(int headerLevel, long categoryId)
		{
			bool result = true;
			if (headerLevel > 0)
			{
				if (this.visibilityStates[headerLevel].Contains(categoryId))
				{
					result = !this.IsHeaderLevelVisibleByDefault(headerLevel);
				}
				else
				{
					result = this.IsHeaderLevelVisibleByDefault(headerLevel);
				}
			}
			return result;
		}

		public int Serialize(byte[] buffer, int offset)
		{
			int num = this.collapseStates.Length;
			int num2 = 8;
			for (int i = 1; i < num; i++)
			{
				num2 += 4 + this.visibilityStates[i].Count * 8;
			}
			for (int j = 0; j < num; j++)
			{
				num2 += 4 + this.collapseStates[j].Count * 8;
			}
			if (buffer != null)
			{
				ParseSerialize.SerializeInt32(num, buffer, offset);
				offset += 4;
				ParseSerialize.SerializeInt32(this.levelsInitiallyExpanded, buffer, offset);
				offset += 4;
				for (int k = 1; k < num; k++)
				{
					ParseSerialize.SerializeInt32(this.visibilityStates[k].Count, buffer, offset);
					offset += 4;
					foreach (long value in this.visibilityStates[k])
					{
						ParseSerialize.SerializeInt64(value, buffer, offset);
						offset += 8;
					}
				}
				for (int l = 0; l < num; l++)
				{
					ParseSerialize.SerializeInt32(this.collapseStates[l].Count, buffer, offset);
					offset += 4;
					foreach (long value2 in this.collapseStates[l])
					{
						ParseSerialize.SerializeInt64(value2, buffer, offset);
						offset += 8;
					}
				}
			}
			return num2;
		}

		public int Deserialize(byte[] buffer, int offset)
		{
			int num = offset;
			if (buffer.Length - offset < 8)
			{
				throw new StoreException((LID)44928U, ErrorCodeValue.InvalidCollapseState);
			}
			int num2 = ParseSerialize.ParseInt32(buffer, offset);
			offset += 4;
			if (num2 != this.collapseStates.Length)
			{
				throw new StoreException((LID)61312U, ErrorCodeValue.InvalidCollapseState);
			}
			int num3 = ParseSerialize.ParseInt32(buffer, offset);
			offset += 4;
			if (num3 != this.levelsInitiallyExpanded)
			{
				throw new StoreException((LID)36736U, ErrorCodeValue.InvalidCollapseState);
			}
			HashSet<long>[] array = new HashSet<long>[num2];
			for (int i = 1; i < num2; i++)
			{
				if (buffer.Length - offset < 4)
				{
					throw new StoreException((LID)53120U, ErrorCodeValue.InvalidCollapseState);
				}
				array[i] = new HashSet<long>();
				int num4 = ParseSerialize.ParseInt32(buffer, offset);
				offset += 4;
				if (num4 < 0 || num4 >= 268435455 || buffer.Length - offset < num4 * 8)
				{
					throw new StoreException((LID)46976U, ErrorCodeValue.InvalidCollapseState);
				}
				while (num4-- != 0)
				{
					long item = ParseSerialize.ParseInt64(buffer, offset);
					offset += 8;
					array[i].Add(item);
				}
			}
			HashSet<long>[] array2 = new HashSet<long>[this.collapseStates.Length];
			for (int j = 0; j < num2; j++)
			{
				if (buffer.Length - offset < 4)
				{
					throw new StoreException((LID)63360U, ErrorCodeValue.InvalidCollapseState);
				}
				array2[j] = new HashSet<long>();
				int num5 = ParseSerialize.ParseInt32(buffer, offset);
				offset += 4;
				if (num5 < 0 || num5 >= 268435455 || buffer.Length - offset < num5 * 8)
				{
					throw new StoreException((LID)38784U, ErrorCodeValue.InvalidCollapseState);
				}
				while (num5-- != 0)
				{
					long item2 = ParseSerialize.ParseInt64(buffer, offset);
					offset += 8;
					array2[j].Add(item2);
				}
			}
			this.visibilityStates = array;
			this.collapseStates = array2;
			return offset - num;
		}

		private bool IsHeaderLevelVisibleByDefault(int headerLevel)
		{
			return headerLevel <= this.levelsInitiallyExpanded;
		}

		private bool IsHeaderLevelExpandedByDefault(int headerLevel)
		{
			return headerLevel < this.levelsInitiallyExpanded;
		}

		private readonly int levelsInitiallyExpanded;

		private HashSet<long>[] visibilityStates;

		private HashSet<long>[] collapseStates;
	}
}

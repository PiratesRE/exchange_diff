using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class IdSet : IEquatable<IdSet>, IEnumerable<GuidGlobCountSet>, IEnumerable
	{
		internal IdSet(GuidGlobCountSet[] sets)
		{
			foreach (GuidGlobCountSet newGuidGlobCountSet in sets)
			{
				this.Insert(newGuidGlobCountSet);
			}
		}

		internal IdSet()
		{
		}

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
			set
			{
				this.isDirty = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
				{
					if (!guidGlobCountSet.IsEmpty)
					{
						return false;
					}
				}
				return true;
			}
		}

		public int CountGuids
		{
			get
			{
				return this.sets.Count;
			}
		}

		public int CountRanges
		{
			get
			{
				int num = 0;
				foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
				{
					num += guidGlobCountSet.CountRanges;
				}
				return num;
			}
		}

		public ulong CountIds
		{
			get
			{
				ulong num = 0UL;
				foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
				{
					num += guidGlobCountSet.CountIds;
				}
				return num;
			}
		}

		public static IdSet ParseWithReplGuids(Reader reader)
		{
			return IdSet.Parse(reader, (Reader r) => r.ReadGuid());
		}

		public static IdSet ParseWithReplIds(Reader reader, Func<ReplId, Guid> guidFromReplId)
		{
			return IdSet.Parse(reader, (Reader r) => guidFromReplId(ReplId.Parse(r)));
		}

		public static IdSet Union(IdSet first, IdSet second)
		{
			IdSet idSet = new IdSet();
			int num = 0;
			int num2 = 0;
			while (num < first.sets.Count || num2 < second.sets.Count)
			{
				int num3 = (num2 >= second.sets.Count) ? -1 : ((num >= first.sets.Count) ? 1 : first.sets[num].Guid.CompareTo(second.sets[num2].Guid));
				if (num3 < 0)
				{
					idSet.sets.Add(first.sets[num].Clone());
					num++;
				}
				else if (num3 > 0)
				{
					idSet.sets.Add(second.sets[num2].Clone());
					num2++;
				}
				else
				{
					GlobCountSet globCountSet = GlobCountSet.Union(first.sets[num].GlobCountSet, second.sets[num2].GlobCountSet);
					idSet.sets.Add(new GuidGlobCountSet(first.sets[num].Guid, globCountSet));
					num++;
					num2++;
				}
			}
			idSet.isDirty = false;
			return idSet;
		}

		public static IdSet Subtract(IdSet first, IdSet second)
		{
			IdSet idSet = new IdSet();
			int i = 0;
			int num = 0;
			while (i < first.sets.Count)
			{
				int num2 = (num >= second.sets.Count) ? -1 : first.sets[i].Guid.CompareTo(second.sets[num].Guid);
				if (num2 < 0)
				{
					idSet.sets.Add(first.sets[i].Clone());
					i++;
				}
				else if (num2 > 0)
				{
					num++;
				}
				else
				{
					GlobCountSet globCountSet = GlobCountSet.Subtract(first.sets[i].GlobCountSet, second.sets[num].GlobCountSet);
					if (globCountSet != null)
					{
						idSet.sets.Add(new GuidGlobCountSet(first.sets[i].Guid, globCountSet));
					}
					i++;
					num++;
				}
			}
			idSet.isDirty = false;
			return idSet;
		}

		public static IdSet Intersect(IdSet first, IdSet second)
		{
			IdSet idSet = new IdSet();
			int num = 0;
			int num2 = 0;
			while (num < first.sets.Count && num2 < second.sets.Count)
			{
				int num3 = first.sets[num].Guid.CompareTo(second.sets[num2].Guid);
				if (num3 < 0)
				{
					num++;
				}
				else if (num3 > 0)
				{
					num2++;
				}
				else
				{
					GlobCountSet globCountSet = GlobCountSet.Intersect(first.sets[num].GlobCountSet, second.sets[num2].GlobCountSet);
					if (globCountSet != null)
					{
						idSet.Insert(new GuidGlobCountSet(first.sets[num].Guid, globCountSet));
					}
					num++;
					num2++;
				}
			}
			idSet.isDirty = false;
			return idSet;
		}

		public bool Equals(IdSet other)
		{
			if (other == null || this.sets.Count != other.sets.Count)
			{
				return false;
			}
			for (int i = 0; i < this.sets.Count; i++)
			{
				if (!this.sets[i].Equals(other.sets[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			IdSet idSet = obj as IdSet;
			return idSet != null && this.Equals(idSet);
		}

		public override int GetHashCode()
		{
			int num = this.sets.Count;
			foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
			{
				num ^= guidGlobCountSet.GetHashCode();
			}
			return num;
		}

		public bool Insert(IdSet idSet)
		{
			bool flag = false;
			foreach (GuidGlobCountSet newGuidGlobCountSet in idSet.sets)
			{
				flag |= this.Insert(newGuidGlobCountSet);
			}
			return this.SetDirty(flag);
		}

		public bool Insert(GuidGlobCountSet newGuidGlobCountSet)
		{
			if (newGuidGlobCountSet.IsEmpty)
			{
				return false;
			}
			int num = this.FindSet(newGuidGlobCountSet.Guid);
			if (num > -1)
			{
				return this.SetDirty(this.sets[num].GlobCountSet.Insert(newGuidGlobCountSet.GlobCountSet));
			}
			this.sets.Insert(~num, newGuidGlobCountSet);
			return this.SetDirty(true);
		}

		public bool Insert(GuidGlobCount newGuidGlobCount)
		{
			return this.Insert(newGuidGlobCount.Guid, newGuidGlobCount.GlobCount);
		}

		public bool Insert(Guid guid, ulong globCount)
		{
			int num = this.FindSet(guid);
			if (num > -1)
			{
				return this.SetDirty(this.sets[num].GlobCountSet.Insert(globCount));
			}
			GlobCountSet globCountSet = new GlobCountSet();
			globCountSet.Insert(globCount);
			GuidGlobCountSet item = new GuidGlobCountSet(guid, globCountSet);
			this.sets.Insert(~num, item);
			return this.SetDirty(true);
		}

		public bool Insert(Guid guid, GlobCountRange range)
		{
			int num = this.FindSet(guid);
			if (num > -1)
			{
				return this.SetDirty(this.sets[num].GlobCountSet.Insert(range));
			}
			GlobCountSet globCountSet = new GlobCountSet();
			globCountSet.Insert(range);
			GuidGlobCountSet item = new GuidGlobCountSet(guid, globCountSet);
			this.sets.Insert(~num, item);
			return this.SetDirty(true);
		}

		public bool Remove(IdSet idSet)
		{
			bool flag = false;
			foreach (GuidGlobCountSet removedGuidGlobCountSet in idSet.sets)
			{
				flag |= this.Remove(removedGuidGlobCountSet);
			}
			return this.SetDirty(flag);
		}

		public bool Remove(GuidGlobCountSet removedGuidGlobCountSet)
		{
			int num = this.FindSet(removedGuidGlobCountSet.Guid);
			if (num > -1)
			{
				bool dirty = this.sets[num].GlobCountSet.Remove(removedGuidGlobCountSet.GlobCountSet);
				if (this.sets[num].GlobCountSet.IsEmpty)
				{
					this.sets.RemoveAt(num);
					this.lastGuidIndex = -1;
				}
				return this.SetDirty(dirty);
			}
			return false;
		}

		public bool Remove(GuidGlobCount removedGuidGlobCount)
		{
			int num = this.FindSet(removedGuidGlobCount.Guid);
			if (num > -1)
			{
				bool dirty = this.sets[num].GlobCountSet.Remove(removedGuidGlobCount.GlobCount);
				if (this.sets[num].GlobCountSet.IsEmpty)
				{
					this.sets.RemoveAt(num);
					this.lastGuidIndex = -1;
				}
				return this.SetDirty(dirty);
			}
			return false;
		}

		public bool Remove(Guid guid, GlobCountRange range)
		{
			int num = this.FindSet(guid);
			if (num > -1)
			{
				bool dirty = this.sets[num].GlobCountSet.Remove(range);
				if (this.sets[num].GlobCountSet.IsEmpty)
				{
					this.sets.RemoveAt(num);
					this.lastGuidIndex = -1;
				}
				return this.SetDirty(dirty);
			}
			return false;
		}

		public bool IdealPack()
		{
			bool flag = false;
			foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
			{
				flag |= guidGlobCountSet.GlobCountSet.IdealPack();
			}
			return this.SetDirty(flag);
		}

		public bool Contains(GuidGlobCount guidGlobCount)
		{
			int num = this.FindSet(guidGlobCount.Guid);
			return num > -1 && this.sets[num].GlobCountSet.Contains(guidGlobCount.GlobCount);
		}

		public void SerializeWithReplGuids(Writer writer)
		{
			foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
			{
				writer.WriteGuid(guidGlobCountSet.Guid);
				guidGlobCountSet.GlobCountSet.Serialize(writer);
			}
		}

		public byte[] SerializeWithReplGuids()
		{
			return BufferWriter.Serialize(new BufferWriter.SerializeDelegate(this.SerializeWithReplGuids));
		}

		public void SerializeWithReplIds(Writer writer, Func<Guid, ReplId> replIdFromGuid)
		{
			IOrderedEnumerable<GuidGlobCountSet> orderedEnumerable = from s in this.sets
			orderby replIdFromGuid(s.Guid).Value
			select s;
			foreach (GuidGlobCountSet guidGlobCountSet in orderedEnumerable)
			{
				replIdFromGuid(guidGlobCountSet.Guid).Serialize(writer);
				guidGlobCountSet.GlobCountSet.Serialize(writer);
			}
		}

		public byte[] SerializeWithReplIds(Func<Guid, ReplId> replIdFromGuid)
		{
			return BufferWriter.Serialize(delegate(Writer writer)
			{
				this.SerializeWithReplIds(writer, replIdFromGuid);
			});
		}

		public IEnumerator<GuidGlobCountSet> GetEnumerator()
		{
			return this.sets.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IdSet Clone()
		{
			IdSet idSet = new IdSet();
			foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
			{
				idSet.Insert(guidGlobCountSet.Clone());
			}
			idSet.isDirty = false;
			return idSet;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('{');
			bool flag = true;
			foreach (GuidGlobCountSet guidGlobCountSet in this.sets)
			{
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(guidGlobCountSet.ToString());
				flag = false;
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		private static IdSet Parse(Reader reader, Func<Reader, Guid> getReplGuid)
		{
			IdSet idSet = new IdSet();
			while (reader.Position < reader.Length)
			{
				Guid guid = getReplGuid(reader);
				GlobCountSet globCountSet = GlobCountSet.Parse(reader);
				idSet.Insert(new GuidGlobCountSet(guid, globCountSet));
			}
			idSet.isDirty = false;
			return idSet;
		}

		private int FindSet(Guid guidToFind)
		{
			if (this.lastGuidIndex < 0 || guidToFind != this.lastGuid)
			{
				GuidGlobCountSet item = new GuidGlobCountSet(guidToFind, null);
				this.lastGuidIndex = this.sets.BinarySearch(item, IdSet.GuidGlobCountSetComparer.Default);
				this.lastGuid = guidToFind;
			}
			return this.lastGuidIndex;
		}

		private bool SetDirty(bool changed)
		{
			this.isDirty = (this.isDirty || changed);
			return changed;
		}

		private readonly List<GuidGlobCountSet> sets = new List<GuidGlobCountSet>();

		private int lastGuidIndex = -1;

		private Guid lastGuid;

		private bool isDirty;

		private class GuidGlobCountSetComparer : IComparer<GuidGlobCountSet>
		{
			private GuidGlobCountSetComparer()
			{
			}

			int IComparer<GuidGlobCountSet>.Compare(GuidGlobCountSet x, GuidGlobCountSet y)
			{
				return x.Guid.CompareTo(y.Guid);
			}

			internal static IdSet.GuidGlobCountSetComparer Default = new IdSet.GuidGlobCountSetComparer();
		}
	}
}

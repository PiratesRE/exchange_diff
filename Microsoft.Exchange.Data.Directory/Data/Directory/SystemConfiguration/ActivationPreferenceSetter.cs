using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ActivationPreferenceSetter<T> where T : IActivationPreferenceSettable<T>, IConfigurable
	{
		public ActivationPreferenceSetter(T[] existingList, T entry, EntryAction entryAction) : this(existingList, entry, entryAction, 2147483647L, 134217727L)
		{
		}

		public ActivationPreferenceSetter(T[] existingList, T entry, EntryAction entryAction, long maxValue, long defaultMaxInterval)
		{
			if (entryAction == EntryAction.Insert && (existingList == null || existingList.Length == 0) && entry.DesiredValue != 1)
			{
				throw new ArgumentException("DesiredValue has to be 1, when inserting into an empty list.");
			}
			if (existingList == null)
			{
				this.m_existingList = new T[0];
			}
			else
			{
				this.m_existingList = existingList;
				Array.Sort<T>(this.m_existingList);
			}
			this.m_entry = entry;
			this.m_entryAction = entryAction;
			this.m_maxValue = maxValue;
			this.m_defaultMaxInterval = defaultMaxInterval;
		}

		public long MaxInterval
		{
			get
			{
				if (this.m_maxInterval == null)
				{
					this.m_maxInterval = new long?(this.ComputeNewInterval(this.m_defaultMaxInterval));
				}
				return this.m_maxInterval.Value;
			}
			set
			{
				this.m_maxInterval = new long?(value);
			}
		}

		public int MaxNumberOfIntervals
		{
			get
			{
				return this.ComputeMaxNumberOfIntervals(this.MaxInterval);
			}
		}

		public UpdateResult UpdateCachedValues()
		{
			if (this.m_entryAction == EntryAction.Insert)
			{
				return this.UpdateForInsertion();
			}
			return this.UpdateForModification();
		}

		public void SaveAllUpdatedValues(IConfigDataProvider session)
		{
			foreach (T t in this.m_existingList)
			{
				if (!t.Matches(this.m_entry))
				{
					t.InvalidHostServerAllowed = true;
					session.Save(t);
				}
			}
		}

		private long ComputeNewInterval(long startingInterval)
		{
			long num = startingInterval;
			int num2 = this.ComputeMaxNumberOfIntervals(num);
			while (this.m_existingList.Length > num2 && num > 0L)
			{
				num >>= 1;
				num2 = this.ComputeMaxNumberOfIntervals(num);
			}
			if (num == 0L)
			{
				throw new ActivationPreferenceException(DirectoryStrings.TooManyEntriesError);
			}
			return num;
		}

		private int ComputeMaxNumberOfIntervals(long interval)
		{
			return (int)(this.m_maxValue / interval - 1L);
		}

		private UpdateResult UpdateForModification()
		{
			int num = this.m_entry.DesiredValue - 1;
			bool flag = this.CheckIfForceReassignNeeded();
			if (!flag && this.m_existingList[num].Matches(this.m_entry))
			{
				this.m_entry.ActualValue = this.m_existingList[num].ActualValue;
				return UpdateResult.NoChange;
			}
			int num2 = -1;
			for (int i = 0; i < this.m_existingList.Length; i++)
			{
				if (this.m_existingList[i].Matches(this.m_entry))
				{
					num2 = i;
					break;
				}
			}
			int desiredIndex = num;
			if (num2 < num)
			{
				desiredIndex = num + 1;
			}
			bool flag2;
			long num3 = this.UpdateInternal(out flag2, desiredIndex, flag);
			if (flag2)
			{
				this.m_entry.ActualValue = (int)num3;
			}
			if (flag || !flag2)
			{
				return UpdateResult.AllChanged;
			}
			return UpdateResult.SingleChanged;
		}

		private bool CheckIfForceReassignNeeded()
		{
			bool result = false;
			try
			{
				new HashSet<int>(Array.ConvertAll<T, int>(this.m_existingList, (T input) => input.ActualValue));
			}
			catch (ArgumentException)
			{
				result = true;
			}
			return result;
		}

		private UpdateResult UpdateForInsertion()
		{
			bool flag = false;
			if (this.m_existingList == null || this.m_existingList.Length == 0)
			{
				flag = true;
			}
			bool flag2 = false;
			long num;
			bool flag3;
			if (flag)
			{
				flag3 = this.CalculatePossibleValue(this.m_maxValue, 0L, out num);
			}
			else
			{
				int desiredIndex = this.m_entry.DesiredValue - 1;
				flag2 = this.CheckIfForceReassignNeeded();
				num = this.UpdateInternal(out flag3, desiredIndex, flag2);
			}
			if (flag3)
			{
				this.m_entry.ActualValue = (int)num;
			}
			if (flag2 || !flag3)
			{
				return UpdateResult.AllChanged;
			}
			return UpdateResult.SingleChanged;
		}

		private long UpdateInternal(out bool calcReturnVal, int desiredIndex, bool forceReassignValues)
		{
			long result = 0L;
			calcReturnVal = false;
			if (!forceReassignValues)
			{
				if (desiredIndex == this.m_existingList.Length)
				{
					calcReturnVal = this.CalculatePossibleValue(this.m_maxValue, (long)this.m_existingList[desiredIndex - 1].ActualValue, out result);
				}
				else if (desiredIndex == 0)
				{
					calcReturnVal = this.CalculatePossibleValue((long)this.m_existingList[0].ActualValue, 0L, out result);
				}
				else
				{
					calcReturnVal = this.CalculatePossibleValue((long)this.m_existingList[desiredIndex].ActualValue, (long)this.m_existingList[desiredIndex - 1].ActualValue, out result);
				}
			}
			if (forceReassignValues || !calcReturnVal)
			{
				this.ReAssignAllActualValues(desiredIndex);
			}
			return result;
		}

		private void ReAssignAllActualValues(int desiredIndex)
		{
			if (this.m_entryAction == EntryAction.Insert && this.m_existingList.Length + 1 > this.MaxNumberOfIntervals)
			{
				this.MaxInterval >>= 1;
				if (this.MaxInterval == 0L)
				{
					throw new ActivationPreferenceException(DirectoryStrings.TooManyEntriesError);
				}
			}
			bool flag = false;
			int i = 0;
			int num = 1;
			while (i < this.m_existingList.Length)
			{
				if (desiredIndex == i && !flag)
				{
					this.m_entry.ActualValue = (int)((long)num * this.MaxInterval);
					flag = true;
					i--;
				}
				else if (this.m_existingList[i].Matches(this.m_entry))
				{
					num--;
				}
				else
				{
					this.m_existingList[i].ActualValue = (int)((long)num * this.MaxInterval);
				}
				i++;
				num++;
			}
			if (!flag && desiredIndex == i)
			{
				this.m_entry.ActualValue = (int)((long)num * this.MaxInterval);
			}
		}

		private bool CalculatePossibleValue(long next, long previous, out long possibleValue)
		{
			long num = next - previous >> 1;
			if (num > this.MaxInterval)
			{
				num = this.MaxInterval;
			}
			else if (num == 0L)
			{
				possibleValue = 0L;
				return false;
			}
			possibleValue = previous + num;
			return true;
		}

		private const long MinValue = 0L;

		private const long MaxValue = 2147483647L;

		private const long DefaultMaxInterval = 134217727L;

		private readonly long m_maxValue;

		private readonly long m_defaultMaxInterval;

		private readonly EntryAction m_entryAction;

		private T[] m_existingList;

		private T m_entry;

		private long? m_maxInterval;
	}
}

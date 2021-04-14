using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.People.Utilities
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ListDiffCalculator<T, K>
	{
		public ListDiffCalculator(IComparer<T> contactIdentityComparer, IContactDeltaCalculator<T, K> contactDeltaCalculator)
		{
			ArgumentValidator.ThrowIfNull("contactIdentityComparer", contactIdentityComparer);
			ArgumentValidator.ThrowIfNull("contactDeltaCalculator", contactDeltaCalculator);
			this.identityComparer = contactIdentityComparer;
			this.deltaCalculator = contactDeltaCalculator;
		}

		public DiffResult<T, K> DiffUnSortedLists(List<T> listUnSortedSource, List<T> listUnSortedTarget)
		{
			ArgumentValidator.ThrowIfNull("listSortedSource", listUnSortedSource);
			ArgumentValidator.ThrowIfNull("listSortedSource", listUnSortedTarget);
			this.SortList(listUnSortedSource);
			this.SortList(listUnSortedTarget);
			return this.DiffSortedLists(listUnSortedSource, listUnSortedTarget);
		}

		private void SortList(List<T> contacts)
		{
			contacts.Sort(this.identityComparer);
		}

		private DiffResult<T, K> DiffSortedLists(IEnumerable<T> listSortedSource, IEnumerable<T> listSortedTarget)
		{
			ArgumentValidator.ThrowIfNull("listSortedSource", listSortedSource);
			ArgumentValidator.ThrowIfNull("listSortedTarget", listSortedTarget);
			DiffResult<T, K> diffResult = new DiffResult<T, K>();
			bool flag = true;
			bool flag2 = false;
			using (IEnumerator<T> enumerator = listSortedTarget.GetEnumerator())
			{
				using (IEnumerator<T> enumerator2 = listSortedSource.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						T t = enumerator2.Current;
						if (t != null)
						{
							if (flag2)
							{
								diffResult.RemoveList.Add(t);
							}
							else
							{
								while (!flag2)
								{
									if (flag)
									{
										flag2 = !enumerator.MoveNext();
										if (flag2)
										{
											diffResult.RemoveList.Add(t);
											break;
										}
									}
									if (enumerator.Current != null)
									{
										int num = this.identityComparer.Compare(t, enumerator.Current);
										if (num == 0)
										{
											ICollection<Tuple<K, object>> collection = this.deltaCalculator.CalculateDelta(t, enumerator.Current);
											if (collection.Count != 0)
											{
												diffResult.UpdateList.Add(t, collection);
											}
											flag = true;
											break;
										}
										if (num < 0)
										{
											diffResult.RemoveList.Add(t);
											flag = false;
											break;
										}
										diffResult.AddList.Add(enumerator.Current);
										flag = true;
									}
								}
							}
						}
					}
					goto IL_134;
				}
				IL_123:
				diffResult.AddList.Add(enumerator.Current);
				IL_134:
				if (!flag2 && enumerator.MoveNext())
				{
					goto IL_123;
				}
			}
			return diffResult;
		}

		private readonly IComparer<T> identityComparer;

		private readonly IContactDeltaCalculator<T, K> deltaCalculator;
	}
}

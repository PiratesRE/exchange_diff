using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class WorkUnitCollection : BindingList<WorkUnit>
	{
		public WorkUnitCollection()
		{
			this.stopWatch = new Stopwatch();
		}

		public void AddRange(IEnumerable<WorkUnit> workUnits)
		{
			base.RaiseListChangedEvents = false;
			foreach (WorkUnit item in workUnits)
			{
				base.Add(item);
			}
			base.RaiseListChangedEvents = true;
			base.ResetBindings();
		}

		protected override void OnListChanged(ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor != null && e.PropertyDescriptor.Name == "Status")
			{
				WorkUnit workUnit = base[e.NewIndex];
				if (!this.stopWatch.IsRunning && workUnit.Status == WorkUnitStatus.InProgress)
				{
					this.stopWatch.Start();
				}
				else
				{
					bool flag = false;
					bool flag2 = true;
					for (int i = 0; i < base.Count; i++)
					{
						if (base[i].Status == WorkUnitStatus.InProgress)
						{
							flag = true;
						}
						if (base[i].Status != WorkUnitStatus.NotStarted)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						this.stopWatch.Reset();
					}
					if (flag)
					{
						if (!this.stopWatch.IsRunning)
						{
							this.stopWatch.Start();
						}
					}
					else
					{
						this.stopWatch.Stop();
					}
				}
			}
			base.OnListChanged(e);
		}

		public int ProgressValue
		{
			get
			{
				int num = 0;
				checked
				{
					for (int i = 0; i < base.Count; i++)
					{
						num += base[i].PercentComplete;
					}
					return num;
				}
			}
		}

		public int MaxProgressValue
		{
			get
			{
				return checked(Math.Max(base.Count, 1) * 100);
			}
		}

		public LocalizedString Description
		{
			get
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < base.Count; i++)
				{
					switch (base[i].Status)
					{
					case WorkUnitStatus.NotStarted:
						num++;
						break;
					case WorkUnitStatus.InProgress:
						num4++;
						break;
					case WorkUnitStatus.Completed:
						num2++;
						break;
					case WorkUnitStatus.Failed:
						num3++;
						break;
					}
				}
				if (num == base.Count)
				{
					return Strings.WorkUnitCollectionConfigurationSummary;
				}
				return Strings.WorkUnitCollectionStatus(base.Count, num2, num3);
			}
		}

		public bool AllCompleted
		{
			get
			{
				if (base.Count == 0)
				{
					return false;
				}
				for (int i = 0; i < base.Count; i++)
				{
					if (base[i].Status != WorkUnitStatus.Completed)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool HasFailures
		{
			get
			{
				for (int i = 0; i < base.Count; i++)
				{
					if (base[i].Status == WorkUnitStatus.Failed || base[i].Status == WorkUnitStatus.NotStarted)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool Cancelled
		{
			get
			{
				foreach (WorkUnit workUnit in this)
				{
					if (workUnit.Status == WorkUnitStatus.Failed && workUnit.Errors.Count > 0 && workUnit.Errors[0].Exception is PipelineStoppedException)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsDataChanged
		{
			get
			{
				return this.FindByStatus(WorkUnitStatus.Completed).Count > 0;
			}
		}

		public IList<WorkUnit> FindByStatus(WorkUnitStatus status)
		{
			List<WorkUnit> list = new List<WorkUnit>();
			for (int i = 0; i < base.Count; i++)
			{
				if (status == base[i].Status)
				{
					list.Add(base[i]);
				}
			}
			return list.ToArray();
		}

		public IList<WorkUnit> FindByErrorOrWarning()
		{
			List<WorkUnit> list = new List<WorkUnit>();
			for (int i = 0; i < base.Count; i++)
			{
				if (base[i].Errors.Count > 0 || base[i].Warnings.Count > 0)
				{
					list.Add(base[i]);
				}
			}
			return list.ToArray();
		}

		public string Summary
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(this.ElapsedTimeText);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(this.Description);
				for (int i = 0; i < base.Count; i++)
				{
					WorkUnit workUnit = base[i];
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(workUnit.Summary);
				}
				return stringBuilder.ToString();
			}
		}

		public TimeSpan ElapsedTime
		{
			get
			{
				return this.stopWatch.Elapsed;
			}
		}

		public string ElapsedTimeText
		{
			get
			{
				TimeSpan elapsedTime = this.ElapsedTime;
				return Strings.OverallElapsedTimeDescription(elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
			}
		}

		public WorkUnit[] ToArray()
		{
			WorkUnit[] array = new WorkUnit[base.Count];
			base.CopyTo(array, 0);
			return array;
		}

		private Stopwatch stopWatch;
	}
}

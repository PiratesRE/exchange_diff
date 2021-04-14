using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	[CLSCompliant(true)]
	public sealed class ExEventSourceInfo
	{
		public ExEventSourceInfo(string name)
		{
			this.Name = name;
			this.regWatcher = new RegistryWatcher("System\\CurrentControlSet\\Services\\" + this.Name + "\\Diagnostics\\", true);
		}

		[DefaultValue(900)]
		public int EventPeriodTime
		{
			get
			{
				return this.m_eventPeriodTime;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", "Value must be positive");
				}
				this.m_eventPeriodTime = ((value <= 604800) ? value : 604800);
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public ExEventCategory GetCategory(int index)
		{
			ExEventCategory exEventCategory = null;
			RegistryKey registryKey = null;
			ExEventCategory result;
			try
			{
				if (index < 1)
				{
					throw new ArgumentOutOfRangeException("index", "Value must be greater than zero. index = " + index);
				}
				if (this.regWatcher.IsChanged())
				{
					registryKey = Registry.LocalMachine.OpenSubKey(this.regWatcher.KeyName);
					if (registryKey == null)
					{
						return null;
					}
					try
					{
						this.categoriesLock.EnterWriteLock();
						foreach (ExEventCategory exEventCategory2 in this.m_eventCategories.Values)
						{
							if (exEventCategory2 != null)
							{
								string text = exEventCategory2.Number.ToString(NumberFormatInfo.InvariantInfo) + " " + exEventCategory2.Name;
								int? num = registryKey.GetValue(text) as int?;
								if (num == null)
								{
									ExTraceGlobals.EventLogTracer.TraceWarning<string, string>(55041, 0L, "Inappropriate registry key type {0} in {1}", text, this.regWatcher.KeyName);
									exEventCategory2.EventLevel = ExEventLog.EventLevel.Expert;
								}
								else
								{
									exEventCategory2.EventLevel = (ExEventLog.EventLevel)num.Value;
								}
							}
						}
					}
					finally
					{
						try
						{
							this.categoriesLock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
				try
				{
					this.categoriesLock.EnterReadLock();
					this.m_eventCategories.TryGetValue(index, out exEventCategory);
				}
				finally
				{
					try
					{
						this.categoriesLock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				if (exEventCategory != null)
				{
					result = exEventCategory;
				}
				else
				{
					if (registryKey == null)
					{
						registryKey = Registry.LocalMachine.OpenSubKey(this.regWatcher.KeyName);
					}
					if (registryKey != null)
					{
						string[] valueNames = registryKey.GetValueNames();
						if (valueNames.Length > 0)
						{
							string text2;
							if (index < valueNames.Length)
							{
								text2 = valueNames[index - 1];
							}
							else
							{
								text2 = valueNames[0];
							}
							string text3 = index.ToString(NumberFormatInfo.InvariantInfo) + " ";
							if (text2.StartsWith(text3))
							{
								int? num2 = registryKey.GetValue(text2) as int?;
								if (num2 == null)
								{
									ExTraceGlobals.EventLogTracer.TraceWarning<string, string>(63233, 0L, "Inappropriate registry key type {0} in {1}", text2, this.regWatcher.KeyName);
									return exEventCategory;
								}
								exEventCategory = new ExEventCategory(text2.Substring(text3.Length), index, (ExEventLog.EventLevel)num2.Value);
							}
							else
							{
								foreach (string text4 in valueNames)
								{
									if (text4.StartsWith(text3))
									{
										int? num3 = registryKey.GetValue(text4) as int?;
										if (num3 != null)
										{
											exEventCategory = new ExEventCategory(text4.Substring(text3.Length), index, (ExEventLog.EventLevel)num3.Value);
											break;
										}
										ExTraceGlobals.EventLogTracer.TraceWarning<string, string>(38657, 0L, "Inappropriate registry key type {0} in {1}", text4, this.regWatcher.KeyName);
									}
								}
							}
							if (exEventCategory != null)
							{
								try
								{
									this.categoriesLock.EnterWriteLock();
									this.m_eventCategories[index] = exEventCategory;
								}
								finally
								{
									try
									{
										this.categoriesLock.ExitWriteLock();
									}
									catch (SynchronizationLockException)
									{
									}
								}
							}
						}
					}
					result = exEventCategory;
				}
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
					registryKey = null;
				}
			}
			return result;
		}

		public const int DefaultEventPeriodTime = 900;

		private const int MaxEventPeriodTime = 604800;

		public string Name;

		private Dictionary<int, ExEventCategory> m_eventCategories = new Dictionary<int, ExEventCategory>();

		private ReaderWriterLockSlim categoriesLock = new ReaderWriterLockSlim();

		private int m_eventPeriodTime = 900;

		private RegistryWatcher regWatcher;
	}
}

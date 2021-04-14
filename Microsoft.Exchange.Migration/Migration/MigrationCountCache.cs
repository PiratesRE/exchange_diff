using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationCountCache : MigrationPersistableDictionary
	{
		internal bool IsEmpty
		{
			get
			{
				return base.PropertyBag.Count == 0;
			}
		}

		internal bool IsValid
		{
			get
			{
				foreach (object obj in ((IDictionary)base.PropertyBag).Keys)
				{
					string text = obj as string;
					if (string.IsNullOrEmpty(text))
					{
						return false;
					}
					object obj2 = base.Get<object>(text);
					if (obj2 is int && (int)obj2 < 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public static MigrationCountCache Deserialize(string serializedData)
		{
			MigrationCountCache migrationCountCache = new MigrationCountCache();
			if (!string.IsNullOrEmpty(serializedData))
			{
				migrationCountCache.DeserializeData(serializedData);
			}
			return migrationCountCache;
		}

		public MigrationCountCache Clone()
		{
			MigrationCountCache migrationCountCache = new MigrationCountCache();
			foreach (object obj in ((IDictionary)base.PropertyBag).Keys)
			{
				string text = obj as string;
				if (!string.IsNullOrEmpty(text))
				{
					migrationCountCache.Set<object>(text, base.Get<object>(text));
				}
			}
			return migrationCountCache;
		}

		public int GetCachedStatusCount(params MigrationUserStatus[] statuses)
		{
			if (statuses == null)
			{
				return 0;
			}
			return statuses.Sum(delegate(MigrationUserStatus status)
			{
				int? nullable = base.GetNullable<int>(MigrationCountCache.MapFromStatusToKey[status]);
				if (nullable == null)
				{
					return 0;
				}
				return nullable.GetValueOrDefault();
			});
		}

		public void SetCachedStatusCount(MigrationUserStatus status, int value)
		{
			base.Set<int>(MigrationCountCache.MapFromStatusToKey[status], value);
		}

		public int GetCachedOtherCount(string key)
		{
			int? nullable = base.GetNullable<int>(key);
			if (nullable == null)
			{
				return 0;
			}
			return nullable.GetValueOrDefault();
		}

		public void SetCachedOtherCount(string key, int value)
		{
			base.Set<int>(key, value);
		}

		public void IncrementCachedOtherCount(string key, int amount)
		{
			int num = base.GetNullable<int>(key) ?? 0;
			num += amount;
			base.Set<int>(key, num);
		}

		public ExDateTime? GetCachedTimestamp(string key)
		{
			return base.GetNullable<ExDateTime>(key);
		}

		public void SetCachedTimestamp(string key, ExDateTime? value)
		{
			base.SetNullable<ExDateTime>(key, value);
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("MigrationCountCache");
			foreach (object obj in ((IDictionary)base.PropertyBag).Keys)
			{
				string text = obj as string;
				if (!string.IsNullOrEmpty(text))
				{
					xelement.Add(new XElement(text, base.Get<object>(text)));
				}
			}
			return xelement;
		}

		public void ApplyStatusChange(MigrationCountCache.MigrationStatusChange change)
		{
			foreach (string key in change.Keys)
			{
				this.IncrementCachedOtherCount(key, change[key]);
			}
		}

		public const string LastSync = "LastSync";

		public const string RemovedItems = "Removed";

		public const string ProvisionedItems = "Provisioned";

		private const string StatusQueued = "StatusQueued";

		private const string StatusSyncing = "StatusSyncing";

		private const string StatusFailed = "StatusFailed";

		private const string StatusSynced = "StatusSynced";

		private const string StatusIncrementalFailed = "StatusIncrementalFailed";

		private const string StatusCompleting = "StatusCompleting";

		private const string StatusCompleted = "StatusCompleted";

		private const string StatusCompletionFailed = "StatusCompletionFailed";

		private const string StatusCompletedWithWarnings = "StatusCompletedWithWarnings";

		private const string StatusCorrupted = "StatusCorrupted";

		private const string StatusProvisioning = "StatusProvisioning";

		private const string StatusProvisionUpdating = "StatusProvisionUpdating";

		private const string StatusCompletionSynced = "StatusCompletionSynced";

		private const string StatusValidating = "StatusValidating";

		private const string StatusIncrementalSyncing = "StatusIncrementalSyncing";

		private const string StatusIncrementalSynced = "StatusIncrementalSynced";

		private const string StatusStopped = "StatusStopped";

		private const string StatusIncrementalStopped = "StatusIncrementalStopped";

		private const string StatusStarting = "StatusStarting";

		private const string StatusStopping = "StatusStopping";

		private const string StatusRemoving = "StatusRemoving";

		private static readonly Dictionary<MigrationUserStatus, string> MapFromStatusToKey = new Dictionary<MigrationUserStatus, string>
		{
			{
				MigrationUserStatus.Queued,
				"StatusQueued"
			},
			{
				MigrationUserStatus.Syncing,
				"StatusSyncing"
			},
			{
				MigrationUserStatus.Failed,
				"StatusFailed"
			},
			{
				MigrationUserStatus.Synced,
				"StatusSynced"
			},
			{
				MigrationUserStatus.IncrementalFailed,
				"StatusIncrementalFailed"
			},
			{
				MigrationUserStatus.Completing,
				"StatusCompleting"
			},
			{
				MigrationUserStatus.Completed,
				"StatusCompleted"
			},
			{
				MigrationUserStatus.CompletionFailed,
				"StatusCompletionFailed"
			},
			{
				MigrationUserStatus.CompletedWithWarnings,
				"StatusCompletedWithWarnings"
			},
			{
				MigrationUserStatus.Corrupted,
				"StatusCorrupted"
			},
			{
				MigrationUserStatus.Provisioning,
				"StatusProvisioning"
			},
			{
				MigrationUserStatus.ProvisionUpdating,
				"StatusProvisionUpdating"
			},
			{
				MigrationUserStatus.CompletionSynced,
				"StatusCompletionSynced"
			},
			{
				MigrationUserStatus.Validating,
				"StatusValidating"
			},
			{
				MigrationUserStatus.IncrementalSyncing,
				"StatusIncrementalSyncing"
			},
			{
				MigrationUserStatus.IncrementalSynced,
				"StatusIncrementalSynced"
			},
			{
				MigrationUserStatus.Stopped,
				"StatusStopped"
			},
			{
				MigrationUserStatus.IncrementalStopped,
				"StatusIncrementalStopped"
			},
			{
				MigrationUserStatus.Starting,
				"StatusStarting"
			},
			{
				MigrationUserStatus.Stopping,
				"StatusStopping"
			},
			{
				MigrationUserStatus.Removing,
				"StatusRemoving"
			}
		};

		internal class MigrationStatusChange : Dictionary<string, int>
		{
			private MigrationStatusChange()
			{
			}

			public static MigrationCountCache.MigrationStatusChange CreateStatusChange(MigrationUserStatus oldStatus, MigrationUserStatus newStatus)
			{
				if (oldStatus == newStatus)
				{
					return null;
				}
				return new MigrationCountCache.MigrationStatusChange
				{
					{
						MigrationCountCache.MapFromStatusToKey[oldStatus],
						-1
					},
					{
						MigrationCountCache.MapFromStatusToKey[newStatus],
						1
					}
				};
			}

			public static MigrationCountCache.MigrationStatusChange CreateInject(MigrationUserStatus status)
			{
				return new MigrationCountCache.MigrationStatusChange
				{
					{
						MigrationCountCache.MapFromStatusToKey[status],
						1
					}
				};
			}

			public static MigrationCountCache.MigrationStatusChange CreateRemoval(MigrationUserStatus status)
			{
				return new MigrationCountCache.MigrationStatusChange
				{
					{
						MigrationCountCache.MapFromStatusToKey[status],
						-1
					}
				};
			}

			public static MigrationCountCache.MigrationStatusChange operator +(MigrationCountCache.MigrationStatusChange statusChange1, MigrationCountCache.MigrationStatusChange statusChange2)
			{
				MigrationCountCache.MigrationStatusChange migrationStatusChange = new MigrationCountCache.MigrationStatusChange();
				foreach (string key in statusChange1.Keys.Union(statusChange2.Keys))
				{
					int num = 0;
					if (statusChange1.ContainsKey(key))
					{
						num += statusChange1[key];
					}
					if (statusChange2.ContainsKey(key))
					{
						num += statusChange2[key];
					}
					migrationStatusChange[key] = num;
				}
				return migrationStatusChange;
			}

			public static MigrationCountCache.MigrationStatusChange None = new MigrationCountCache.MigrationStatusChange();
		}
	}
}

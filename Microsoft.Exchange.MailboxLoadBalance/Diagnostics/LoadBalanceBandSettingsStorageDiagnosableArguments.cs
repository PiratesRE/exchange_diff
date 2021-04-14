using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceBandSettingsStorageDiagnosableArguments : LoadBalanceDiagnosableArgumentBase
	{
		public bool ShowPersistedBands
		{
			get
			{
				return base.HasArgument("persistedBands");
			}
		}

		public bool ShowActiveBands
		{
			get
			{
				return base.HasArgument("listBands");
			}
		}

		public bool ProcessAction
		{
			get
			{
				return base.HasArgument("action");
			}
		}

		public LoadBalanceBandSettingsStorageDiagnosableArguments.BandStorageActionType Action
		{
			get
			{
				return (LoadBalanceBandSettingsStorageDiagnosableArguments.BandStorageActionType)Enum.Parse(typeof(LoadBalanceBandSettingsStorageDiagnosableArguments.BandStorageActionType), base.GetArgument<string>("action"), true);
			}
		}

		public Band.BandProfile Profile
		{
			get
			{
				return (Band.BandProfile)Enum.Parse(typeof(Band.BandProfile), base.GetArgument<string>("profile"), true);
			}
		}

		public ByteQuantifiedSize MinSize
		{
			get
			{
				return ByteQuantifiedSize.Parse(base.GetArgument<string>("min"), ByteQuantifiedSize.Quantifier.MB);
			}
		}

		public ByteQuantifiedSize MaxSize
		{
			get
			{
				return ByteQuantifiedSize.Parse(base.GetArgument<string>("max"), ByteQuantifiedSize.Quantifier.MB);
			}
		}

		public bool Enabled
		{
			get
			{
				return base.HasArgument("enabled");
			}
		}

		public TimeSpan? MaxLogonAge
		{
			get
			{
				if (!base.HasArgument("maxLogonAge"))
				{
					return null;
				}
				return new TimeSpan?(base.GetArgument<TimeSpan>("maxLogonAge"));
			}
		}

		public TimeSpan? MinLogonAge
		{
			get
			{
				if (!base.HasArgument("minLogonAge"))
				{
					return null;
				}
				return new TimeSpan?(base.GetArgument<TimeSpan>("minLogonAge"));
			}
		}

		public bool IncludePhysicalOnly
		{
			get
			{
				return base.HasArgument("onlyPhysicalMailboxes");
			}
		}

		public double MailboxWeightFactor
		{
			get
			{
				return base.GetArgument<double>("mailboxSizeFactor");
			}
		}

		public Band CreateBand()
		{
			return new Band(this.Profile, this.MinSize, this.MaxSize, this.MailboxWeightFactor, this.IncludePhysicalOnly, this.MinLogonAge, this.MaxLogonAge);
		}

		protected override void ExtendSchema(Dictionary<string, Type> schema)
		{
			schema["persistedBands"] = typeof(bool);
			schema["listBands"] = typeof(bool);
			schema["action"] = typeof(string);
			schema["enabled"] = typeof(bool);
			schema["max"] = typeof(string);
			schema["min"] = typeof(string);
			schema["profile"] = typeof(string);
			schema["onlyPhysicalMailboxes"] = typeof(bool);
			schema["mailboxSizeFactor"] = typeof(double);
			schema["maxLogonAge"] = typeof(TimeSpan);
			schema["minLogonAge"] = typeof(TimeSpan);
		}

		private const string PersistedBands = "persistedBands";

		private const string StorageAction = "action";

		private const string BandProfile = "profile";

		private const string BandMinSize = "min";

		private const string BandMaxSize = "max";

		private const string EnabledFlag = "enabled";

		private const string ListBands = "listBands";

		private const string BandMaxLogonAge = "maxLogonAge";

		private const string BandMinLogonAge = "minLogonAge";

		private const string BandIncludePhysicalMailboxesOnly = "onlyPhysicalMailboxes";

		private const string BandMailboxSizeFactor = "mailboxSizeFactor";

		internal enum BandStorageActionType
		{
			Create,
			Remove,
			Enable,
			Disable
		}
	}
}

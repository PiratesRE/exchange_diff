using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class SyncBatchStatisticsBase
	{
		public TimeSpan ResponseTime { get; set; }

		public int ObjectCount { get; set; }

		public int LinkCount { get; set; }

		public SizeAndCountStatistics ObjectSize { get; set; }

		public double ObjectsPerSecond { get; set; }

		public double ObjectBytesPerSecond { get; set; }

		public SizeAndCountStatistics LinkSize { get; set; }

		public double LinksPerSecond { get; set; }

		public double LinkBytesPerSecond { get; set; }

		public Dictionary<string, int> ObjectTypes { get; set; }

		public static int SerializedSize(object obj)
		{
			StringWriter stringWriter = new StringWriter();
			XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
			xmlSerializer.Serialize(new XmlTextWriter(stringWriter), obj);
			return stringWriter.ToString().Length * 2;
		}

		public virtual void Calculate(IEnumerable<DirectoryObject> Objects, IEnumerable<DirectoryLink> Links)
		{
			this.ObjectCount = Objects.Count<DirectoryObject>();
			this.LinkCount = Links.Count<DirectoryLink>();
			if (SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site1 == null)
			{
				SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(SyncBatchStatisticsBase)));
			}
			this.ObjectSize = SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site1.Target(SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site1, SizeAndCountStatistics.Calculate(from o in Objects
			select SyncBatchStatisticsBase.SerializedSize(o)));
			this.ObjectsPerSecond = (double)this.ObjectCount / this.ResponseTime.TotalSeconds;
			this.ObjectBytesPerSecond = (double)this.ObjectSize.Sum / this.ResponseTime.TotalSeconds;
			if (SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site2 == null)
			{
				SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, SizeAndCountStatistics>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(SizeAndCountStatistics), typeof(SyncBatchStatisticsBase)));
			}
			this.LinkSize = SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site2.Target(SyncBatchStatisticsBase.<Calculate>o__SiteContainer0.<>p__Site2, SizeAndCountStatistics.Calculate(from o in Links
			select SyncBatchStatisticsBase.SerializedSize(o)));
			this.LinksPerSecond = (double)this.LinkCount / this.ResponseTime.TotalSeconds;
			this.LinkBytesPerSecond = (double)this.LinkSize.Sum / this.ResponseTime.TotalSeconds;
			this.ObjectTypes = (from o in Objects
			group o by o.GetType().Name).ToDictionary((IGrouping<string, DirectoryObject> g) => g.Key, (IGrouping<string, DirectoryObject> g) => g.Count<DirectoryObject>());
		}

		[CompilerGenerated]
		private static class <Calculate>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site1;

			public static CallSite<Func<CallSite, object, SizeAndCountStatistics>> <>p__Site2;
		}
	}
}

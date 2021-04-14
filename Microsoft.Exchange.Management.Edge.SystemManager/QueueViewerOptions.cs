using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	internal class QueueViewerOptions : ExchangeDataObject
	{
		public QueueViewerOptions(bool refresh, EnhancedTimeSpan interval, uint size)
		{
			this.AutoRefreshEnabled = refresh;
			this.RefreshInterval = interval;
			this.PageSize = size;
			base.ResetChangeTracking();
		}

		public bool AutoRefreshEnabled
		{
			get
			{
				return (bool)this[QueueViewerOptionsSchema.AutoRefreshEnabled];
			}
			set
			{
				this[QueueViewerOptionsSchema.AutoRefreshEnabled] = value;
			}
		}

		public EnhancedTimeSpan RefreshInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[QueueViewerOptionsSchema.RefreshInterval];
			}
			set
			{
				this[QueueViewerOptionsSchema.RefreshInterval] = value;
			}
		}

		public uint PageSize
		{
			get
			{
				return (uint)this[QueueViewerOptionsSchema.PageSize];
			}
			set
			{
				this[QueueViewerOptionsSchema.PageSize] = value;
			}
		}

		internal override ObjectSchema Schema
		{
			get
			{
				return QueueViewerOptions.schema;
			}
		}

		private static QueueViewerOptionsSchema schema = ObjectSchema.GetInstance<QueueViewerOptionsSchema>();
	}
}

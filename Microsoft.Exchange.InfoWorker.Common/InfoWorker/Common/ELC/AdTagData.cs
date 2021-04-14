using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class AdTagData
	{
		internal RetentionTag Tag
		{
			get
			{
				return this.policyTag;
			}
			set
			{
				this.policyTag = value;
			}
		}

		internal SortedDictionary<Guid, ContentSetting> ContentSettings
		{
			get
			{
				return this.contentSettings;
			}
			set
			{
				this.contentSettings = value;
			}
		}

		private RetentionTag policyTag;

		private SortedDictionary<Guid, ContentSetting> contentSettings;
	}
}

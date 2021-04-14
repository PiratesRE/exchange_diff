using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class StoreTagData : AdTagData
	{
		internal StoreTagData()
		{
			base.ContentSettings = new SortedDictionary<Guid, ContentSetting>();
		}

		internal StoreTagData(AdTagData adTagData)
		{
			base.Tag = adTagData.Tag;
			base.ContentSettings = adTagData.ContentSettings;
			this.optedInto = false;
			if (base.Tag.Type == ElcFolderType.Personal)
			{
				this.isVisible = true;
				return;
			}
			this.isVisible = false;
		}

		internal bool OptedInto
		{
			get
			{
				return this.optedInto;
			}
			set
			{
				this.optedInto = value;
			}
		}

		internal bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
			set
			{
				this.isVisible = value;
			}
		}

		private bool optedInto;

		private bool isVisible;
	}
}

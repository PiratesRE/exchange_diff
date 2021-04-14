using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Microsoft.Office.CsmSdk.Controls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[PersistChildren(false)]
	[ParseChildren(true)]
	[NonVisualControl]
	public class ResourceManagerProxy : Control
	{
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[MergableProperty(false)]
		public List<ResourceEntry> ResourceEntries
		{
			get
			{
				return this.resourceEntries;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ResourceManager.GetCurrent(this.Page).ResourceEntries.AddRange(this.ResourceEntries);
		}

		private List<ResourceEntry> resourceEntries = new List<ResourceEntry>();
	}
}

using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class BreadcrumbsTrail : Breadcrumbs<Breadcrumb>
	{
		public BreadcrumbsTrail(string name, TrailLength length) : base((int)length)
		{
			this.Name = name;
		}

		public string Name { get; private set; }

		public void Drop(string message)
		{
			base.Drop(new Breadcrumb(message));
		}
	}
}

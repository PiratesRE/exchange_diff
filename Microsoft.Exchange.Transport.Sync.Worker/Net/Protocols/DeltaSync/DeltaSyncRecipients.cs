using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncRecipients
	{
		internal IList<string> To
		{
			get
			{
				if (this.to == null)
				{
					this.to = DeltaSyncRecipients.ReadOnlyEmptyList;
				}
				return this.to;
			}
			set
			{
				this.to = value;
			}
		}

		internal IList<string> Cc
		{
			get
			{
				if (this.cc == null)
				{
					this.cc = DeltaSyncRecipients.ReadOnlyEmptyList;
				}
				return this.cc;
			}
			set
			{
				this.cc = value;
			}
		}

		internal IList<string> Bcc
		{
			get
			{
				if (this.bcc == null)
				{
					this.bcc = DeltaSyncRecipients.ReadOnlyEmptyList;
				}
				return this.bcc;
			}
			set
			{
				this.bcc = value;
			}
		}

		internal int Count
		{
			get
			{
				return this.To.Count + this.Cc.Count + this.Bcc.Count;
			}
		}

		private static readonly ReadOnlyCollection<string> ReadOnlyEmptyList = new List<string>(0).AsReadOnly();

		private IList<string> to;

		private IList<string> cc;

		private IList<string> bcc;
	}
}

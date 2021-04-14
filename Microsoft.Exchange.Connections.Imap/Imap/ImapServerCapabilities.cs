using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ImapServerCapabilities : ServerCapabilities
	{
		internal ImapServerCapabilities()
		{
		}

		internal ImapServerCapabilities(IEnumerable<string> capabilities) : base(capabilities)
		{
		}

		public bool SupportsImap4Rev1
		{
			get
			{
				return base.Capabilities.Contains("IMAP4REV1", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsIdle
		{
			get
			{
				return base.Capabilities.Contains("IDLE", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsQuota
		{
			get
			{
				return base.Capabilities.Contains("QUOTA", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsEnable
		{
			get
			{
				return base.Capabilities.Contains("ENABLE", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsUidPlus
		{
			get
			{
				return base.Capabilities.Contains("UIDPLUS", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsChildren
		{
			get
			{
				return base.Capabilities.Contains("CHILDREN", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsMove
		{
			get
			{
				return base.Capabilities.Contains("MOVE", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsCompressEqualsDeflate
		{
			get
			{
				return base.Capabilities.Contains("COMPRESS=DEFLATE", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsXlist
		{
			get
			{
				return base.Capabilities.Contains("XLIST", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsId
		{
			get
			{
				return base.Capabilities.Contains("ID", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsUnselect
		{
			get
			{
				return base.Capabilities.Contains("UNSELECT", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsNamespace
		{
			get
			{
				return base.Capabilities.Contains("UIDPLUS", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public bool SupportsXGmExt1
		{
			get
			{
				return base.Capabilities.Contains("X-GM-EXT-1", StringComparer.CurrentCultureIgnoreCase);
			}
		}

		internal const string Imap4Rev1Capability = "IMAP4REV1";

		internal const string IdleCapability = "IDLE";

		internal const string QuotaCapability = "QUOTA";

		internal const string EnableCapability = "ENABLE";

		internal const string UidPlusCapability = "UIDPLUS";

		internal const string ChildrenCapability = "CHILDREN";

		internal const string MoveCapability = "MOVE";

		internal const string CompressEqualsDeflateCapability = "COMPRESS=DEFLATE";

		internal const string XlistCapability = "XLIST";

		internal const string IdCapability = "ID";

		internal const string UnselectCapability = "UNSELECT";

		internal const string NamespaceCapability = "NAMESPACE";

		internal const string XGmExt1Capability = "X-GM-EXT-1";
	}
}

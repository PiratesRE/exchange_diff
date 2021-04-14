using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WellKnownPrincipalMapper
	{
		public WellKnownPrincipalMapper()
		{
			this.guidToSidMap = new Dictionary<Guid, SecurityIdentifier>();
			this.sidToGuidMap = new Dictionary<SecurityIdentifier, Guid>();
		}

		public Guid this[SecurityIdentifier sid]
		{
			get
			{
				Guid result;
				if (this.sidToGuidMap.TryGetValue(sid, out result))
				{
					return result;
				}
				return Guid.Empty;
			}
		}

		public SecurityIdentifier this[Guid guid]
		{
			get
			{
				SecurityIdentifier result;
				if (this.guidToSidMap.TryGetValue(guid, out result))
				{
					return result;
				}
				return null;
			}
		}

		public void Initialize(IRecipientSession session)
		{
			if (this.initialized)
			{
				return;
			}
			lock (this.locker)
			{
				if (!this.initialized)
				{
					try
					{
						this.AddMapping(WellKnownPrincipalMapper.ExchangeServers, session.GetWellKnownExchangeGroupSid(WellKnownPrincipalMapper.ExchangeServers));
					}
					catch (ADExternalException)
					{
					}
					this.initialized = true;
				}
			}
		}

		private void AddMapping(Guid guid, SecurityIdentifier sid)
		{
			if (sid != null && guid != Guid.Empty)
			{
				this.guidToSidMap[guid] = sid;
				this.sidToGuidMap[sid] = guid;
			}
		}

		public static readonly Guid ExchangeServers = new Guid("00fa592b-68a2-43ea-83ba-89b4971b6863");

		private Dictionary<Guid, SecurityIdentifier> guidToSidMap;

		private Dictionary<SecurityIdentifier, Guid> sidToGuidMap;

		private bool initialized;

		private object locker = new object();
	}
}

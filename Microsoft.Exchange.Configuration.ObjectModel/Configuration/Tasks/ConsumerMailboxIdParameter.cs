using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ConsumerMailboxIdParameter : ADIdParameter
	{
		public ConsumerMailboxIdParameter()
		{
		}

		public ConsumerMailboxIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		protected ConsumerMailboxIdParameter(string identity) : base(identity)
		{
		}

		public ConsumerMailboxIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static ConsumerMailboxIdParameter Parse(string identity)
		{
			return new ConsumerMailboxIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			notFoundReason = null;
			if (typeof(T) != typeof(ADUser))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			List<ADUser> list = new List<ADUser>();
			ADUser aduser = null;
			Guid exchangeGuid;
			WindowsLiveId windowsLiveId;
			if (Guid.TryParse(base.RawIdentity, out exchangeGuid))
			{
				aduser = ((IRecipientSession)session).FindByExchangeGuidIncludingAlternate<ADUser>(exchangeGuid);
			}
			else if (ConsumerMailboxIdParameter.TryParseWindowsLiveId(base.RawIdentity, out windowsLiveId))
			{
				if (windowsLiveId.NetId != null)
				{
					aduser = ((IRecipientSession)session).FindByExchangeGuidIncludingAlternate<ADUser>(ConsumerIdentityHelper.GetExchangeGuidFromPuid(windowsLiveId.NetId.ToUInt64()));
				}
				else
				{
					aduser = ((IRecipientSession)session).FindByProxyAddress<ADUser>(new SmtpProxyAddress(windowsLiveId.SmtpAddress.ToString(), true));
				}
			}
			else if (base.InternalADObjectId != null)
			{
				aduser = ((IRecipientSession)session).FindADUserByObjectId(base.InternalADObjectId);
			}
			if (aduser != null)
			{
				list.Add(aduser);
			}
			return list as IEnumerable<T>;
		}

		private static bool TryParseWindowsLiveId(string identity, out WindowsLiveId liveId)
		{
			liveId = null;
			if (!identity.Contains("\\"))
			{
				return WindowsLiveId.TryParse(identity, out liveId);
			}
			int num = identity.LastIndexOf('\\');
			return WindowsLiveId.TryParse(identity.Substring(num + 1), out liveId);
		}
	}
}

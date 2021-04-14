using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTSession : ISession
	{
		public PSTSession(PstMailbox pstMailbox)
		{
			this.pstMailbox = pstMailbox;
		}

		public PstMailbox PstMailbox
		{
			get
			{
				return this.pstMailbox;
			}
		}

		public bool TryResolveToNamedProperty(PropertyTag propertyTag, out NamedProperty namedProperty)
		{
			namedProperty = null;
			INamedProperty namedProperty2 = null;
			if (!propertyTag.IsNamedProperty || !this.pstMailbox.IPst.ReadNamedPropertyTable().TryGetValue((ushort)propertyTag.PropertyId, out namedProperty2))
			{
				return false;
			}
			try
			{
				namedProperty = (namedProperty2.IsString ? new NamedProperty(namedProperty2.Guid, namedProperty2.Name) : new NamedProperty(namedProperty2.Guid, namedProperty2.Id));
			}
			catch (ArgumentException ex)
			{
				throw new ExArgumentException(ex.Message, ex);
			}
			return true;
		}

		public bool TryResolveFromNamedProperty(NamedProperty namedProperty, ref PropertyTag propertyTag)
		{
			ushort num;
			try
			{
				if (namedProperty.Kind == NamedPropertyKind.String)
				{
					num = this.pstMailbox.IPst.ReadIdFromNamedProp(namedProperty.Name, 0U, namedProperty.Guid, true);
				}
				else
				{
					num = this.pstMailbox.IPst.ReadIdFromNamedProp(null, namedProperty.Id, namedProperty.Guid, true);
				}
			}
			catch (PSTExceptionBase innerException)
			{
				throw new MailboxReplicationPermanentException(new LocalizedString(this.pstMailbox.IPst.FileName), innerException);
			}
			if (num != 0)
			{
				propertyTag = new PropertyTag((PropertyId)num, propertyTag.PropertyType);
				return true;
			}
			propertyTag = PropertyTag.CreateError(propertyTag.PropertyId);
			return false;
		}

		public void Dispose()
		{
		}

		private PstMailbox pstMailbox;
	}
}

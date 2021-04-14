using System;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferContext : MapiBase, ISession
	{
		public FastTransferContext() : base(MapiObjectType.FastTransferContext)
		{
		}

		public ErrorCode Configure(MapiLogon logon)
		{
			base.ParentObject = logon;
			base.Logon = logon;
			base.IsValid = true;
			return ErrorCode.NoError;
		}

		public Microsoft.Exchange.Protocols.MAPI.Version OtherSideVersion
		{
			get
			{
				return this.otherSideVersion;
			}
			set
			{
				this.otherSideVersion = value;
			}
		}

		public bool TryResolveFromNamedProperty(NamedProperty namedProperty, ref PropertyTag propertyTag)
		{
			StorePropName propName;
			if (namedProperty.Kind == NamedPropertyKind.Id)
			{
				propName = new StorePropName(namedProperty.Guid, namedProperty.Id);
			}
			else
			{
				propName = new StorePropName(namedProperty.Guid, namedProperty.Name);
			}
			ushort numberFromName = base.Logon.MapiMailbox.GetNumberFromName(base.CurrentOperationContext, true, propName, base.Logon);
			propertyTag = new PropertyTag((PropertyId)numberFromName, propertyTag.PropertyType);
			return true;
		}

		public bool TryResolveToNamedProperty(PropertyTag propertyTag, out NamedProperty namedProperty)
		{
			StorePropName nameFromNumber = LegacyHelper.GetNameFromNumber(base.CurrentOperationContext, (ushort)propertyTag.PropertyId, base.Logon.MapiMailbox);
			if (nameFromNumber == StorePropName.Invalid)
			{
				namedProperty = new NamedProperty();
				return false;
			}
			if (nameFromNumber.Name != null)
			{
				namedProperty = new NamedProperty(nameFromNumber.Guid, nameFromNumber.Name);
			}
			else
			{
				namedProperty = new NamedProperty(nameFromNumber.Guid, nameFromNumber.DispId);
			}
			return true;
		}

		internal static readonly Microsoft.Exchange.Protocols.MAPI.Version OofHistorySupportMinVersion = new Microsoft.Exchange.Protocols.MAPI.Version(14, 0, 326, 0);

		internal static readonly Microsoft.Exchange.Protocols.MAPI.Version Dumpster2SupportMinVersion = new Microsoft.Exchange.Protocols.MAPI.Version(14, 0, 572, 0);

		private Microsoft.Exchange.Protocols.MAPI.Version otherSideVersion = Microsoft.Exchange.Protocols.MAPI.Version.Minimum;
	}
}

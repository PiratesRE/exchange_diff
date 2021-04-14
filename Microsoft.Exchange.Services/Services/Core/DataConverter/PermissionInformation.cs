using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PermissionInformation : PermissionInformationBase<PermissionType>
	{
		public PermissionInformation()
		{
		}

		public PermissionInformation(PermissionType permissionType) : base(permissionType)
		{
		}

		internal PermissionLevel PermissionLevel
		{
			get
			{
				return (PermissionLevel)base.PermissionElement.PermissionLevel;
			}
			set
			{
				base.PermissionElement.PermissionLevel = (PermissionLevelType)value;
			}
		}

		internal override bool DoAnyNonPermissionLevelFieldsHaveValue()
		{
			return base.DoAnyNonPermissionLevelFieldsHaveValue();
		}

		protected override PermissionLevel GetPermissionLevelToSet()
		{
			return this.PermissionLevel;
		}

		internal override bool IsNonCustomPermissionLevelSet()
		{
			return this.PermissionLevel != PermissionLevel.Custom;
		}

		protected override void SetByTypePermissionFieldsOntoPermission(Permission permission)
		{
		}

		protected override PermissionType CreateDefaultBasePermissionType()
		{
			return new PermissionType();
		}

		internal override bool? CanReadItems
		{
			get
			{
				base.EnsurePermissionElementIsNotNull();
				if (base.PermissionElement.ReadItems != null)
				{
					return new bool?(base.PermissionElement.ReadItems.Value == PermissionReadAccess.FullDetails);
				}
				return null;
			}
			set
			{
				base.EnsurePermissionElementIsNotNull();
				if (value == null)
				{
					base.PermissionElement.ReadItems = null;
					return;
				}
				if (value.Value)
				{
					base.PermissionElement.ReadItems = new PermissionReadAccess?(PermissionReadAccess.FullDetails);
					return;
				}
				base.PermissionElement.ReadItems = new PermissionReadAccess?(PermissionReadAccess.None);
			}
		}
	}
}

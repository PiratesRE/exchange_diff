using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class ADConfigurationObject : ADObject
	{
		internal IConfigurationSession Session
		{
			get
			{
				return (IConfigurationSession)this.m_Session;
			}
		}

		internal virtual ADObjectId ParentPath
		{
			get
			{
				return null;
			}
		}

		internal void SetId(IConfigurationSession session, ADObjectId parent, string cn)
		{
			if (string.IsNullOrEmpty(cn))
			{
				throw new ArgumentException(DirectoryStrings.ErrorEmptyString("cn"), "cn");
			}
			ADObjectId adobjectId = session.GetOrgContainerId();
			if (this.ParentPath != null && !string.IsNullOrEmpty(this.ParentPath.DistinguishedName))
			{
				adobjectId = adobjectId.GetDescendantId(this.ParentPath);
			}
			if (parent != null && !string.IsNullOrEmpty(parent.DistinguishedName))
			{
				adobjectId = adobjectId.GetDescendantId(parent);
			}
			base.SetId(adobjectId.GetChildId(cn));
		}

		internal void SetId(IConfigurationSession session, string cn)
		{
			this.SetId(session, null, cn);
		}

		public string AdminDisplayName
		{
			get
			{
				return (string)this[ADConfigurationObjectSchema.AdminDisplayName];
			}
			internal set
			{
				this[ADConfigurationObjectSchema.AdminDisplayName] = value;
			}
		}

		internal virtual SystemFlagsEnum SystemFlags
		{
			get
			{
				return (SystemFlagsEnum)this[ADConfigurationObjectSchema.SystemFlags];
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (base.IsChanged(ADObjectSchema.Id) || (base.DistinguishedName != null && base.OriginalId != null && !base.DistinguishedName.Equals(base.OriginalId.DistinguishedName)))
			{
				int systemFlags = (int)this.SystemFlags;
				bool flag = 0 != (systemFlags & 1073741824);
				bool flag2 = 0 != (systemFlags & 536870912);
				bool flag3 = 0 != (systemFlags & 268435456);
				bool flag4 = !base.Id.Parent.Equals(base.OriginalId.Parent);
				bool flag5 = !base.Id.Rdn.UnescapedName.Equals(base.OriginalId.Rdn.UnescapedName, StringComparison.OrdinalIgnoreCase);
				if (flag4 && !flag2 && !flag3)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.MoveNotAllowed, this.Identity, string.Empty));
					return;
				}
				if (flag5 && !flag)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.RenameNotAllowed, this.Identity, string.Empty));
					return;
				}
				if (flag4 && !flag2)
				{
					int depth = base.OriginalId.Depth;
					int depth2 = base.Id.Depth;
					if (depth != depth2 || depth < 2 || depth2 < 2)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.LimitedMoveOnlyAllowed, this.Identity, string.Empty));
					}
					if (!base.Id.Parent.Parent.Equals(base.OriginalId.Parent.Parent))
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.LimitedMoveOnlyAllowed, this.Identity, string.Empty));
					}
				}
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(ADConfigurationObjectSchema.SystemFlags))
			{
				this[ADConfigurationObjectSchema.SystemFlags] = SystemFlagsEnum.Renamable;
			}
			base.StampPersistableDefaultValues();
		}
	}
}

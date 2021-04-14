using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Constraints
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SpecialMailboxPlacementConstraint : IAllocationConstraint
	{
		public SpecialMailboxPlacementConstraint(IList<Guid> nonMovableOrganizations)
		{
			this.nonMovableOrganizations = nonMovableOrganizations;
		}

		public ConstraintValidationResult Accept(LoadEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			DirectoryMailbox directoryMailbox = entity.DirectoryObject as DirectoryMailbox;
			if (directoryMailbox == null)
			{
				return new ConstraintValidationResult(this, true);
			}
			if (!(directoryMailbox is NonConnectedMailbox))
			{
				if (!directoryMailbox.PhysicalMailboxes.Any((IPhysicalMailbox pm) => pm.IsQuarantined))
				{
					Guid organizationId = directoryMailbox.OrganizationId;
					if (this.nonMovableOrganizations.Contains(organizationId))
					{
						return new ConstraintValidationResult(this, false);
					}
					return new ConstraintValidationResult(this, true);
				}
			}
			return new ConstraintValidationResult(this, false);
		}

		public IAllocationConstraint CloneForContainer(LoadContainer container)
		{
			return new SpecialMailboxPlacementConstraint(this.nonMovableOrganizations);
		}

		public override string ToString()
		{
			return string.Format("IsSpecialMailbox", new object[0]);
		}

		public void ValidateAccepted(LoadEntity entity)
		{
			if (!this.Accept(entity))
			{
				Guid guid = Guid.Empty;
				Guid guid2 = entity.Guid;
				DirectoryMailbox directoryMailbox = entity.DirectoryObject as DirectoryMailbox;
				if (directoryMailbox != null)
				{
					guid = directoryMailbox.OrganizationId;
				}
				throw new EntityIsNonMovableException(guid.ToString(), guid2.ToString());
			}
		}

		[DataMember]
		private readonly IList<Guid> nonMovableOrganizations;
	}
}

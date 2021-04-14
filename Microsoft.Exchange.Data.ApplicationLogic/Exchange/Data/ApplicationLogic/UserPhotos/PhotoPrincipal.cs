using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoPrincipal
	{
		public ICollection<string> EmailAddresses { get; set; }

		public OrganizationId OrganizationId { get; set; }

		public IEnumerable<string> GetEmailAddressDomains()
		{
			if (this.EmailAddresses == null || this.EmailAddresses.Count == 0)
			{
				return Array<string>.Empty;
			}
			return from address in this.EmailAddresses
			where SmtpAddress.IsValidSmtpAddress(address)
			select SmtpAddress.Parse(address).Domain;
		}

		public bool IsSame(PhotoPrincipal other)
		{
			if (other == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (this.EmailAddresses == null || this.EmailAddresses.Count == 0)
			{
				return false;
			}
			if (other.EmailAddresses == null || other.EmailAddresses.Count == 0)
			{
				return false;
			}
			return (from s in this.EmailAddresses.Intersect(other.EmailAddresses, StringComparer.OrdinalIgnoreCase)
			where !string.IsNullOrEmpty(s)
			select s).Any<string>();
		}

		public bool InSameOrganization(PhotoPrincipal other)
		{
			return other != null && (object.ReferenceEquals(this, other) || (!(this.OrganizationId == null) && !(other.OrganizationId == null) && this.OrganizationId.Equals(other.OrganizationId)));
		}
	}
}

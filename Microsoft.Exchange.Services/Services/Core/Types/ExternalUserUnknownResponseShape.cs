using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExternalUserUnknownResponseShape : ExternalUserResponseShape
	{
		protected override List<PropertyPath> PropertiesAllowedForReadAccess
		{
			get
			{
				return null;
			}
		}

		protected override PropertyPath[] GetPropertiesForCustomPermissions(ItemResponseShape requestedResponseShape)
		{
			return null;
		}
	}
}

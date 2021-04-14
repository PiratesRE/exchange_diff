using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleNameMapping
	{
		public bool IsDeprecatedRole
		{
			get
			{
				return this.NewNames != null && !this.IsSplitting;
			}
		}

		public bool IsSplitting { get; private set; }

		public RoleNameMapping(string nameOld, string nameNew)
		{
			this.OldName = nameOld;
			this.NewName = nameNew;
			this.NewNames = null;
			this.IsSplitting = false;
		}

		public RoleNameMapping(string nameOld, params string[] newNames) : this(nameOld, false, newNames)
		{
		}

		public RoleNameMapping(string nameOld, bool splitting, params string[] newNames)
		{
			this.OldName = nameOld;
			this.NewName = null;
			this.IsSplitting = splitting;
			this.NewNames = newNames.ToList<string>();
		}

		public string OldName;

		public string NewName;

		public List<string> NewNames;
	}
}

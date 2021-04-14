using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal class CompositeTask : ConfigurableObject
	{
		public CompositeTask() : base(new SimplePropertyBag(ComplianceJobSchema.Identity, ComplianceJobSchema.ObjectState, ComplianceJobSchema.ExchangeVersion))
		{
		}

		public Guid TenantId { get; set; }

		public Guid JobRunId { get; set; }

		public int TaskId { get; set; }

		public ComplianceBindingType BindingType { get; set; }

		public string UserMaster { get; set; }

		public string UserList
		{
			get
			{
				return this.userList;
			}
			set
			{
				this.userList = value;
				this.users = (string.IsNullOrWhiteSpace(value) ? null : Utils.JsonStringToStringArray(value));
			}
		}

		public IEnumerable<string> Users
		{
			get
			{
				return this.users;
			}
			set
			{
				this.users = value;
				this.userList = ((value != null) ? Utils.StringArrayToJsonString(value) : null);
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return null;
			}
		}

		private IEnumerable<string> users;

		private string userList;
	}
}

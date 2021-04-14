using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Provisioning
{
	public abstract class ProvisioningHandler
	{
		public string TaskName
		{
			get
			{
				return this.taskName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("TaskName");
				}
				this.taskName = value;
			}
		}

		public LogMessageDelegate LogMessage
		{
			get
			{
				LogMessageDelegate result;
				if ((result = this.logMessage) == null)
				{
					result = delegate(string message)
					{
					};
				}
				return result;
			}
			set
			{
				this.logMessage = value;
			}
		}

		public WriteErrorDelegate WriteError
		{
			get
			{
				WriteErrorDelegate result;
				if ((result = this.writeError) == null)
				{
					result = delegate(LocalizedException ex, ExchangeErrorCategory category)
					{
					};
				}
				return result;
			}
			set
			{
				this.writeError = value;
			}
		}

		public UserScope UserScope
		{
			get
			{
				return this.userScope;
			}
			set
			{
				this.userScope = value;
			}
		}

		public PropertyBag UserSpecifiedParameters
		{
			get
			{
				return this.userSpecifiedParameters;
			}
			set
			{
				this.userSpecifiedParameters = value;
			}
		}

		public ProvisioningCache ProvisioningCache
		{
			get
			{
				return this.provisioningCache;
			}
			internal set
			{
				this.provisioningCache = value;
			}
		}

		internal string AgentName
		{
			get
			{
				return this.agentName;
			}
			set
			{
				this.agentName = value;
			}
		}

		public abstract IConfigurable ProvisionDefaultProperties(IConfigurable readOnlyIConfigurable);

		public abstract bool UpdateAffectedIConfigurable(IConfigurable writeableIConfigurable);

		public abstract bool PreInternalProcessRecord(IConfigurable writeableIConfigurable);

		public abstract ProvisioningValidationError[] Validate(IConfigurable readOnlyIConfigurable);

		public abstract ProvisioningValidationError[] ValidateUserScope();

		public abstract void OnComplete(bool succeeded, Exception e);

		protected List<ProvisioningValidationError> validationErrorsList;

		private string agentName;

		private string taskName;

		private UserScope userScope;

		private PropertyBag userSpecifiedParameters;

		private ProvisioningCache provisioningCache;

		private LogMessageDelegate logMessage;

		private WriteErrorDelegate writeError;
	}
}

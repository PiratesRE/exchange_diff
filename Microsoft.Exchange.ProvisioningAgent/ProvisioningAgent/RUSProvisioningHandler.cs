using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal abstract class RUSProvisioningHandler : ProvisioningHandlerBase
	{
		public RUSProvisioningHandler()
		{
		}

		protected PartitionId PartitionId
		{
			get
			{
				if (this.partitionId == null)
				{
					AccountPartitionIdParameter accountPartitionIdParameter = (AccountPartitionIdParameter)base.UserSpecifiedParameters["AccountPartition"];
					if (accountPartitionIdParameter != null)
					{
						this.partitionId = RecipientTaskHelper.ResolvePartitionId(accountPartitionIdParameter, new Task.TaskErrorLoggingDelegate(this.WriteErrorToTaskErrorHandler));
					}
				}
				return this.partitionId;
			}
		}

		private void WriteErrorToTaskErrorHandler(Exception exception, ErrorCategory category, object unused)
		{
			base.WriteError((LocalizedException)exception, (ExchangeErrorCategory)category);
		}

		public sealed override bool UpdateAffectedIConfigurable(IConfigurable writeableIConfigurable)
		{
			if (writeableIConfigurable == null)
			{
				throw new ArgumentNullException("writeableIConfigurable");
			}
			ExTraceGlobals.RusTracer.TraceDebug<string, string>((long)this.GetHashCode(), "RUSProvisioningHandler.UpdateAffectedIConfigurable: writeableIConfigurable={0}, TaskName={1}", writeableIConfigurable.Identity.ToString(), base.TaskName);
			ADObject adobject;
			if (writeableIConfigurable is ADPresentationObject)
			{
				adobject = ((ADPresentationObject)writeableIConfigurable).DataObject;
			}
			else
			{
				adobject = (ADObject)writeableIConfigurable;
			}
			ADRecipient adrecipient = adobject as ADRecipient;
			return adrecipient != null && !string.IsNullOrEmpty(adrecipient.Alias) && this.UpdateRecipient(adrecipient);
		}

		protected virtual bool UpdateRecipient(ADRecipient recipient)
		{
			return false;
		}

		public sealed override ProvisioningValidationError[] Validate(IConfigurable readOnlyADObject)
		{
			if (readOnlyADObject == null)
			{
				return null;
			}
			List<ProvisioningValidationError> list = new List<ProvisioningValidationError>();
			this.Validate(readOnlyADObject, list);
			return list.ToArray();
		}

		protected virtual void Validate(IConfigurable readOnlyADObject, List<ProvisioningValidationError> errors)
		{
		}

		private PartitionId partitionId;
	}
}

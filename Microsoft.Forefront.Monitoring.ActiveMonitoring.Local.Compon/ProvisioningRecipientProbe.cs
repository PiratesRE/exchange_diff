using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Hygiene.Provisioning;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class ProvisioningRecipientProbe : ProvisioningProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionStartTime = DateTime.UtcNow;
			base.TraceInformation("Starting {0} provisioning probe.", new object[]
			{
				base.Definition.Name
			});
			try
			{
				this.ProbeRecipientProvisioning(cancellationToken);
			}
			catch (Exception ex)
			{
				base.TraceError("Encountered an exception during '{0}' probe execution: {1}.", new object[]
				{
					base.Definition.Name,
					ex.Message
				});
				throw;
			}
			base.TraceInformation("Completed {0} provisioning probe.", new object[]
			{
				base.Definition.Name
			});
		}

		private void ProbeRecipientProvisioning(CancellationToken cancellationToken)
		{
			RecipientProvisioningDefinition recipientProvisioningDefinition = new RecipientProvisioningDefinition(base.Definition.ExtensionAttributes);
			base.TraceInformation("Creating a provisioning probe in organization {0}. Will connect to {1} with user {2} and password {3}.", new object[]
			{
				recipientProvisioningDefinition.ProbeOrganization.ProbeOrganizationId.ObjectGuid,
				recipientProvisioningDefinition.Endpoint,
				recipientProvisioningDefinition.RunAsUser,
				recipientProvisioningDefinition.RunAsUserPassword
			});
			string text = recipientProvisioningDefinition.NamePrefix;
			if (recipientProvisioningDefinition.GenerateUniqueUser)
			{
				text = string.Format("{0}{1}", recipientProvisioningDefinition.NamePrefix, DateTime.UtcNow.Ticks);
			}
			ProvisioningActions provisioningActions = new ProvisioningActions(recipientProvisioningDefinition.Endpoint, recipientProvisioningDefinition.RunAsUser, recipientProvisioningDefinition.RunAsUserPassword);
			base.TraceInformation("Creating a {0} with name {1}.", new object[]
			{
				recipientProvisioningDefinition.RecipientType,
				text
			});
			if (recipientProvisioningDefinition.RecipientType == "user")
			{
				this.PersistResults(recipientProvisioningDefinition, text, provisioningActions.ProvisionUser(text));
				if (recipientProvisioningDefinition.AddLicense)
				{
					base.TraceInformation("adding a license to the user.", new object[0]);
					provisioningActions.AddExchangeUserLicense(text);
				}
			}
			else
			{
				if (!(recipientProvisioningDefinition.RecipientType == "group"))
				{
					throw new ArgumentException(string.Format("Invalid recipient type: {0}.", recipientProvisioningDefinition.RecipientType));
				}
				this.PersistResults(recipientProvisioningDefinition, text, provisioningActions.ProvisionGroup(text));
			}
			base.TraceInformation("Query the result from last run to check recipient provisioning.", new object[0]);
			ProbeResult lastResult = this.GetLastResult(recipientProvisioningDefinition, cancellationToken);
			if (recipientProvisioningDefinition.CleanupRecipient && this.IsVerifiableResult(lastResult))
			{
				Guid guid = Guid.Parse(lastResult.StateAttribute3);
				string stateAttribute = lastResult.StateAttribute4;
				string stateAttribute2 = lastResult.StateAttribute5;
				base.TraceInformation("Probing {0} recipient with name {1}.", new object[]
				{
					stateAttribute,
					stateAttribute2
				});
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, stateAttribute);
				ADRecipient adrecipient = null;
				if (stateAttribute2 == "user")
				{
					adrecipient = recipientProvisioningDefinition.ProbeSession.Find<ADUser>(filter, null, false, null).Cast<ADUser>().FirstOrDefault<ADUser>();
					base.TraceInformation("Cleaning up the user {0} from the last run.", new object[]
					{
						stateAttribute
					});
					provisioningActions.DeprovisionUser(stateAttribute);
				}
				else if (stateAttribute2 == "group")
				{
					adrecipient = recipientProvisioningDefinition.ProbeSession.Find<ADGroup>(filter, null, false, null).Cast<ADGroup>().FirstOrDefault<ADGroup>();
					base.TraceInformation("Cleaning up the group {0} from the last run.", new object[]
					{
						stateAttribute
					});
					provisioningActions.DeprovisionGroup(guid);
				}
				if (adrecipient == null)
				{
					string message = string.Format("{0} {1} was not synced *at all*, thereby violating our SLA of {2}.", stateAttribute2, stateAttribute, recipientProvisioningDefinition.AllowableLatency);
					base.TraceError(message, new object[0]);
					throw new Exception(message);
				}
				DateTime dateTime = new DateTime((long)lastResult.StateAttribute6, DateTimeKind.Utc);
				TimeSpan timeSpan = adrecipient.WhenCreatedUTC.Value - dateTime;
				base.TraceInformation("The last probe started at {0} and {1} {2} was synced at {3} (latency of {4}).", new object[]
				{
					dateTime,
					stateAttribute2,
					stateAttribute,
					adrecipient.WhenCreatedUTC.Value,
					timeSpan
				});
				if (timeSpan > recipientProvisioningDefinition.AllowableLatency)
				{
					string message2 = string.Format("{0} was found in the directory, but with latency {1} exceeding SLA of {2}.", stateAttribute, timeSpan, recipientProvisioningDefinition.AllowableLatency);
					base.TraceError(message2, new object[0]);
					throw new Exception(message2);
				}
			}
			else
			{
				base.TraceInformation("Exiting {0} probe because not enough data from last run.", new object[]
				{
					recipientProvisioningDefinition.RecipientType
				});
			}
		}

		private void PersistResults(RecipientProvisioningDefinition definition, string recipientName, Guid recipientId)
		{
			base.Result.StateAttribute1 = definition.Endpoint;
			base.Result.StateAttribute2 = definition.RunAsUser.ToLower();
			base.Result.StateAttribute3 = recipientId.ToString();
			base.Result.StateAttribute4 = recipientName.ToLower();
			base.Result.StateAttribute5 = definition.RecipientType.ToLower();
			base.Result.StateAttribute6 = (double)DateTime.UtcNow.Ticks;
		}

		private ProbeResult GetLastResult(RecipientProvisioningDefinition definition, CancellationToken cancellationToken)
		{
			ProbeResult result = null;
			if (base.Broker != null)
			{
				IEnumerable<ProbeResult> query = from r in base.Broker.GetProbeResults(base.Definition, base.Result.ExecutionStartTime.AddSeconds(-1.5 * (double)base.Definition.RecurrenceIntervalSeconds))
				where r.StateAttribute1 == definition.Endpoint && r.StateAttribute5 == definition.RecipientType
				select r;
				base.Broker.AsDataAccessQuery<ProbeResult>(query).ExecuteAsync(delegate(ProbeResult r)
				{
					result = r;
				}, cancellationToken, base.TraceContext);
			}
			return result;
		}

		private bool IsVerifiableResult(ProbeResult result)
		{
			return result != null && result.StateAttribute1 != null && result.StateAttribute2 != null && result.StateAttribute3 != null && result.StateAttribute4 != null && result.StateAttribute5 != null;
		}

		private const string UserRecipientType = "user";

		private const string GroupRecipientType = "group";
	}
}

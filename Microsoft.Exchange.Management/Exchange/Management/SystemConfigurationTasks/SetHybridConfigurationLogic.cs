using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class SetHybridConfigurationLogic
	{
		public static void Validate(HybridConfiguration dataObject, bool hasErrors, Task.TaskErrorLoggingDelegate writeErrorFunc)
		{
			if (hasErrors)
			{
				return;
			}
			if (dataObject.MaximumSupportedExchangeObjectVersion.ExchangeBuild < dataObject.ExchangeVersion.ExchangeBuild)
			{
				writeErrorFunc(new InvalidObjectOperationException(HybridStrings.ErrorHybridConfigurationTooNew(dataObject.ExchangeVersion.ToString(), dataObject.MaximumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (dataObject.IsModified(ADObjectSchema.Name))
			{
				writeErrorFunc(new ArgumentException(Strings.ErrorCannotChangeName), ErrorCategory.InvalidArgument, null);
			}
		}

		public static IConfigurable PrepareDataObject(PropertyBag fields, HybridConfiguration dataObject, IConfigDataProvider dataSession, HybridConfigurationTaskUtility.GetUniqueObject getDataObjectFunc, Task.TaskErrorLoggingDelegate writeErrorFunc)
		{
			MultiValuedProperty<ADObjectId> clientAccessServers = dataObject.ClientAccessServers;
			if (fields.IsModified("ClientAccessServers"))
			{
				ADPropertyDefinition clientAccessServers2 = HybridConfigurationSchema.ClientAccessServers;
				HybridConfigurationTaskUtility.ServerCriterion[] array = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsClientAccessServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotCAS));
				array[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE14OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotE14CAS));
				dataObject.ClientAccessServers = HybridConfigurationTaskUtility.ValidateServers(clientAccessServers2, dataSession, fields, getDataObjectFunc, writeErrorFunc, array);
			}
			if (fields.IsModified("SendingTransportServers"))
			{
				ADPropertyDefinition sendingTransportServers = HybridConfigurationSchema.SendingTransportServers;
				HybridConfigurationTaskUtility.ServerCriterion[] array2 = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array2[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsHubTransportServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorSendingTransportServerNotHub));
				array2[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE15OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorSendingTransportServerNotE15Hub));
				dataObject.SendingTransportServers = HybridConfigurationTaskUtility.ValidateServers(sendingTransportServers, dataSession, fields, getDataObjectFunc, writeErrorFunc, array2);
			}
			if (fields.IsModified("ReceivingTransportServers"))
			{
				ADPropertyDefinition receivingTransportServers = HybridConfigurationSchema.ReceivingTransportServers;
				HybridConfigurationTaskUtility.ServerCriterion[] array3 = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array3[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsFrontendTransportServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorReceivingTransportServerNotFrontEnd));
				array3[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE15OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorReceivingTransportServerNotE15FrontEnd));
				dataObject.ReceivingTransportServers = HybridConfigurationTaskUtility.ValidateServers(receivingTransportServers, dataSession, fields, getDataObjectFunc, writeErrorFunc, array3);
			}
			if (fields.IsModified("EdgeTransportServers"))
			{
				ADPropertyDefinition edgeTransportServers = HybridConfigurationSchema.EdgeTransportServers;
				HybridConfigurationTaskUtility.ServerCriterion[] array4 = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array4[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsEdgeServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotEdge));
				array4[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE14Sp1OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotE14Edge));
				dataObject.EdgeTransportServers = HybridConfigurationTaskUtility.ValidateServers(edgeTransportServers, dataSession, fields, getDataObjectFunc, writeErrorFunc, array4);
			}
			if (fields.IsModified("TlsCertificateName"))
			{
				string text = fields["TlsCertificateName"] as string;
				dataObject.TlsCertificateName = ((text == null) ? null : SmtpX509Identifier.Parse(text));
			}
			if (fields.IsModified("OnPremisesSmartHost"))
			{
				dataObject.OnPremisesSmartHost = (SmtpDomain)fields["OnPremisesSmartHost"];
			}
			if (fields.IsModified("Domains"))
			{
				dataObject.Domains = (MultiValuedProperty<AutoDiscoverSmtpDomain>)fields["Domains"];
			}
			if (fields.IsModified("Features"))
			{
				dataObject.Features = (MultiValuedProperty<HybridFeature>)fields["Features"];
			}
			if (fields.IsModified("ExternalIPAddresses"))
			{
				dataObject.ExternalIPAddresses = HybridConfigurationTaskUtility.ValidateExternalIPAddresses((MultiValuedProperty<IPRange>)fields["ExternalIPAddresses"], writeErrorFunc);
			}
			if (fields.IsModified("ServiceInstance"))
			{
				dataObject.ServiceInstance = (int)fields["ServiceInstance"];
			}
			return dataObject;
		}
	}
}

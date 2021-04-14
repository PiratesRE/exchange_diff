using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public abstract class DomainSetupTaskBase : SetupTaskBase
	{
		[Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
		public string Domain
		{
			get
			{
				return (string)base.Fields["Domain"];
			}
			set
			{
				base.Fields["Domain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllDomains
		{
			get
			{
				return (bool)(base.Fields["AllDomains"] ?? false);
			}
			set
			{
				base.Fields["AllDomains"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			List<ADDomain> list = new List<ADDomain>();
			if (this.AllDomains && base.Fields.IsModified("Domain"))
			{
				base.WriteError(new CannotSpecifyBothAllDomainsAndDomainException(), ErrorCategory.InvalidArgument, null);
			}
			this.unreachableDomains = new List<DomainNotReachableException>();
			ADForest localForest = ADForest.GetLocalForest();
			if (this.AllDomains)
			{
				ADCrossRef[] domainPartitions = localForest.GetDomainPartitions();
				if (domainPartitions == null || domainPartitions.Length == 0)
				{
					base.WriteError(new DomainsNotFoundException(), ErrorCategory.InvalidData, null);
				}
				foreach (ADCrossRef adcrossRef in domainPartitions)
				{
					Exception ex = null;
					try
					{
						if (this.IsDomainSetupNeeded(adcrossRef.NCName))
						{
							this.domainConfigurationSession.DomainController = null;
							ADDomain addomain = this.domainConfigurationSession.Read<ADDomain>(adcrossRef.NCName);
							base.LogReadObject(addomain);
							list.Add(addomain);
						}
					}
					catch (ADExternalException ex2)
					{
						ex = ex2;
					}
					catch (ADTransientException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						this.unreachableDomains.Add(new DomainNotReachableException(adcrossRef.DnsRoot[0], this.TaskName, ex));
					}
				}
				this.domainConfigurationSession.DomainController = null;
			}
			else if (base.Fields.IsModified("Domain"))
			{
				string text = (string)base.Fields["Domain"];
				if (string.IsNullOrEmpty(text))
				{
					base.WriteError(new DomainNotFoundException("<null>"), ErrorCategory.InvalidArgument, null);
				}
				ADCrossRef adcrossRef2 = localForest.FindDomainPartitionByFqdn(text);
				if (adcrossRef2 == null)
				{
					base.WriteError(new DomainNotFoundException(text), ErrorCategory.InvalidArgument, null);
				}
				if (this.IsDomainSetupNeeded(adcrossRef2.NCName))
				{
					ADDomain addomain2 = localForest.FindDomainByFqdn(text);
					addomain2 = this.domainConfigurationSession.Read<ADDomain>(addomain2.Id);
					base.LogReadObject(addomain2);
					list.Add(addomain2);
				}
			}
			else
			{
				ADCrossRef localDomainPartition = localForest.GetLocalDomainPartition();
				if (localDomainPartition == null)
				{
					base.WriteError(new LocalDomainNotFoundException(), ErrorCategory.InvalidData, null);
				}
				if (this.IsDomainSetupNeeded(localDomainPartition.NCName))
				{
					ADDomain addomain3 = localForest.FindLocalDomain();
					addomain3 = this.domainConfigurationSession.Read<ADDomain>(addomain3.Id);
					base.LogReadObject(addomain3);
					list.Add(addomain3);
				}
			}
			this.domains = list.ToArray();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (this.UnreachableDomainErrorIsFatal())
			{
				using (List<DomainNotReachableException>.Enumerator enumerator = this.unreachableDomains.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Exception exception = enumerator.Current;
						this.WriteError(exception, ErrorCategory.ObjectNotFound, null, false);
					}
					return;
				}
			}
			foreach (DomainNotReachableException ex in this.unreachableDomains)
			{
				this.WriteWarning(Strings.LegacyPermissionsDomainNotReachableWarning(ex.Dom));
			}
		}

		protected virtual bool IsDomainSetupNeeded(ADObjectId domainId)
		{
			return true;
		}

		protected virtual bool UnreachableDomainErrorIsFatal()
		{
			return true;
		}

		protected abstract string TaskName { get; }

		private const string paramDomain = "Domain";

		private const string paramAllDomains = "AllDomains";

		protected ADDomain[] domains;

		private List<DomainNotReachableException> unreachableDomains;
	}
}

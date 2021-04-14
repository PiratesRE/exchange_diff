using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ClientAccessServer", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetClientAccessServer : SetTopologySystemConfigurationObjectTask<ClientAccessServerIdParameter, ClientAccessServer, Server>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetClientAccessServer(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public Uri AutoDiscoverServiceInternalUri
		{
			get
			{
				return (Uri)base.Fields["AutoDiscoverServiceInternalUri"];
			}
			set
			{
				base.Fields["AutoDiscoverServiceInternalUri"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public MultiValuedProperty<string> AutoDiscoverSiteScope
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["AutoDiscoverSiteScope"];
			}
			set
			{
				base.Fields["AutoDiscoverSiteScope"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false, ParameterSetName = "AlternateServiceAccount")]
		public PSCredential[] AlternateServiceAccountCredential
		{
			get
			{
				return (PSCredential[])base.Fields["AlternateServiceAccountCredential"];
			}
			set
			{
				base.Fields["AlternateServiceAccountCredential"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AlternateServiceAccount")]
		public SwitchParameter CleanUpInvalidAlternateServiceAccountCredentials
		{
			get
			{
				return (SwitchParameter)(base.Fields["CleanUpInvalidAlternateServiceAccountCredentials"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CleanUpInvalidAlternateServiceAccountCredentials"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AlternateServiceAccount")]
		public SwitchParameter RemoveAlternateServiceAccountCredentials
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveAlternateServiceAccountCredentials"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveAlternateServiceAccountCredentials"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ClientAccessArrayIdParameter Array
		{
			get
			{
				return (ClientAccessArrayIdParameter)base.Fields["ClientAccessArray"];
			}
			set
			{
				base.Fields["ClientAccessArray"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override ClientAccessServerIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		internal static void EnsureRunningOnTargetServer(Task task, Server targetServer)
		{
			if (!targetServer.Id.Equals(LocalServer.GetServer().Id))
			{
				task.WriteError(new InvalidOperationException(Strings.CannotManipulateAlternateServiceAccountsRemotely(LocalServer.GetServer().Fqdn, targetServer.Fqdn)), ErrorCategory.InvalidOperation, targetServer.Identity);
			}
		}

		protected override void InternalStateReset()
		{
			this.alternateServiceAccountConfiguration = null;
			this.alternateServiceAccountCredentialsToRemove.Clear();
			base.InternalStateReset();
		}

		protected override IConfigurable PrepareDataObject()
		{
			Server server = (Server)base.PrepareDataObject();
			if (base.ParameterSetName == "AlternateServiceAccount")
			{
				if (this.NeedAlternateServiceAccountPasswords)
				{
					SetClientAccessServer.EnsureRunningOnTargetServer(this, server);
					this.alternateServiceAccountConfiguration = AlternateServiceAccountConfiguration.LoadWithPasswordsFromRegistry();
				}
				else
				{
					this.alternateServiceAccountConfiguration = AlternateServiceAccountConfiguration.LoadFromRegistry(server.Fqdn);
				}
			}
			return server;
		}

		protected sealed override void InternalValidate()
		{
			if (this.Instance.IsModified(ADObjectSchema.Name))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServerNameModified), ErrorCategory.InvalidOperation, this.Identity);
			}
			base.InternalValidate();
			if (base.Fields.IsModified("AutoDiscoverServiceInternalUri") && this.AutoDiscoverServiceInternalUri != null && (!this.AutoDiscoverServiceInternalUri.IsWellFormedOriginalString() || !Uri.IsWellFormedUriString(this.AutoDiscoverServiceInternalUri.ToString(), UriKind.Absolute)))
			{
				base.WriteError(new ArgumentException(Strings.AutoDiscoverUrlIsBad, "AutoDiscoverServiceInternalUri"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				return;
			}
			if (base.ParameterSetName == "AlternateServiceAccount")
			{
				if (this.CleanUpInvalidAlternateServiceAccountCredentials.ToBool() && this.RemoveAlternateServiceAccountCredentials.ToBool())
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCleanUpAndRemoveAlternateServiceAccountsAreMutuallyExclusive), ErrorCategory.SyntaxError, this.DataObject.Identity);
				}
				if (this.CleanUpInvalidAlternateServiceAccountCredentials.ToBool())
				{
					AlternateServiceAccountCredential[] array = this.alternateServiceAccountConfiguration.AllCredentials.ToArray<AlternateServiceAccountCredential>();
					ICollection<string> collection = new HashSet<string>(Microsoft.Exchange.Data.Directory.Management.AlternateServiceAccountCredential.UserNameComparer);
					foreach (AlternateServiceAccountCredential alternateServiceAccountCredential in array)
					{
						if (base.Stopping)
						{
							break;
						}
						bool flag = !collection.Contains(alternateServiceAccountCredential.QualifiedUserName);
						if (flag)
						{
							SecurityStatus securityStatus = SecurityStatus.DecryptFailure;
							flag &= alternateServiceAccountCredential.IsValid;
							if (flag)
							{
								base.WriteVerbose(Strings.VerboseValidatingAlternateServiceAccountCredential(alternateServiceAccountCredential.QualifiedUserName, alternateServiceAccountCredential.WhenAdded.Value));
								flag &= alternateServiceAccountCredential.TryAuthenticate(out securityStatus);
							}
							if (!flag)
							{
								base.WriteVerbose(Strings.VerboseFoundInvalidAlternateServiceAccountCredential(alternateServiceAccountCredential.QualifiedUserName, alternateServiceAccountCredential.WhenAdded ?? DateTime.MinValue, securityStatus.ToString()));
							}
						}
						if (flag)
						{
							base.WriteVerbose(Strings.VerboseFoundValidAlternateServiceAccountCredential(alternateServiceAccountCredential.QualifiedUserName, alternateServiceAccountCredential.WhenAdded.Value));
							collection.Add(alternateServiceAccountCredential.QualifiedUserName);
						}
						else
						{
							this.alternateServiceAccountCredentialsToRemove.Add(alternateServiceAccountCredential);
						}
					}
					if (array.Length > 0 && array.Length == this.alternateServiceAccountCredentialsToRemove.Count)
					{
						this.WriteWarning(Strings.AllAlternateServiceAccountCredentialsAreInvalidOnCleanup(this.DataObject.Fqdn));
						this.alternateServiceAccountCredentialsToRemove.Clear();
					}
				}
			}
		}

		protected sealed override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			if (this.DataObject.MajorVersion != LocalServer.GetServer().MajorVersion)
			{
				base.WriteError(new CannotModifyCrossVersionObjectException(this.DataObject.Id.DistinguishedName), ErrorCategory.InvalidOperation, null);
				return;
			}
			ClientAccessServer clientAccessServer = new ClientAccessServer(this.DataObject);
			if (base.Fields.IsModified("ClientAccessArray"))
			{
				ClientAccessArray clientAccessArrayFromIdParameter = this.GetClientAccessArrayFromIdParameter();
				if (clientAccessArrayFromIdParameter == null)
				{
					clientAccessServer.ClientAccessArray = null;
				}
				else
				{
					if (clientAccessArrayFromIdParameter.IsPriorTo15ExchangeObjectVersion)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorCannotSetToOldClientAccessArray(clientAccessArrayFromIdParameter.ExchangeVersion.ToString(), ClientAccessArray.MinimumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidOperation, this.Identity);
						return;
					}
					clientAccessServer.ClientAccessArray = (ADObjectId)clientAccessArrayFromIdParameter.Identity;
				}
			}
			bool flag = false;
			ADServiceConnectionPoint adserviceConnectionPoint = null;
			ADObjectId childId = clientAccessServer.Id.GetChildId("Protocols").GetChildId("Autodiscover").GetChildId(clientAccessServer.Name);
			if (base.Fields.IsModified("AutoDiscoverServiceInternalUri") && this.AutoDiscoverServiceInternalUri == null && base.Fields.IsModified("AutoDiscoverSiteScope") && this.AutoDiscoverSiteScope == null)
			{
				adserviceConnectionPoint = new ADServiceConnectionPoint();
				adserviceConnectionPoint.SetId(childId);
				base.DataSession.Delete(adserviceConnectionPoint);
				ADObjectId parent = adserviceConnectionPoint.Id.Parent;
				ADContainer adcontainer = new ADContainer();
				adcontainer.SetId(parent);
				base.DataSession.Delete(adcontainer);
				flag = true;
			}
			else
			{
				adserviceConnectionPoint = ((IConfigurationSession)base.DataSession).Read<ADServiceConnectionPoint>(childId);
				if (adserviceConnectionPoint == null)
				{
					adserviceConnectionPoint = new ADServiceConnectionPoint();
					adserviceConnectionPoint.SetId(childId);
					if (!base.Fields.IsModified("AutoDiscoverServiceInternalUri"))
					{
						string text = ComputerInformation.DnsFullyQualifiedDomainName;
						if (string.IsNullOrEmpty(text))
						{
							text = ComputerInformation.DnsPhysicalHostName;
						}
						adserviceConnectionPoint.ServiceBindingInformation.Add("https://" + text + "/Autodiscover/Autodiscover.xml");
					}
					if (!base.Fields.IsModified("AutoDiscoverSiteScope"))
					{
						adserviceConnectionPoint.Keywords.Add("77378F46-2C66-4aa9-A6A6-3E7A48B19596");
						string siteName = NativeHelpers.GetSiteName(false);
						if (!string.IsNullOrEmpty(siteName))
						{
							adserviceConnectionPoint.Keywords.Add("Site=" + siteName);
						}
					}
					adserviceConnectionPoint.ServiceDnsName = ComputerInformation.DnsPhysicalHostName;
					adserviceConnectionPoint.ServiceClassName = "ms-Exchange-AutoDiscover-Service";
					flag = true;
				}
				if (base.Fields.IsModified("AutoDiscoverServiceInternalUri"))
				{
					adserviceConnectionPoint.ServiceBindingInformation.Clear();
					if (this.AutoDiscoverServiceInternalUri != null)
					{
						adserviceConnectionPoint.ServiceBindingInformation.Add(this.AutoDiscoverServiceInternalUri.ToString());
					}
					flag = true;
				}
				if (base.Fields.IsModified("AutoDiscoverSiteScope"))
				{
					adserviceConnectionPoint.Keywords.Clear();
					adserviceConnectionPoint.Keywords.Add("77378F46-2C66-4aa9-A6A6-3E7A48B19596");
					if (this.AutoDiscoverSiteScope != null)
					{
						foreach (string str in this.AutoDiscoverSiteScope)
						{
							adserviceConnectionPoint.Keywords.Add("Site=" + str);
						}
					}
					flag = true;
				}
				if (flag)
				{
					ADObjectId parent2 = adserviceConnectionPoint.Id.Parent;
					if (((IConfigurationSession)base.DataSession).Read<ADContainer>(parent2) == null)
					{
						ADContainer adcontainer2 = new ADContainer();
						adcontainer2.SetId(parent2);
						base.DataSession.Save(adcontainer2);
					}
					base.DataSession.Save(adserviceConnectionPoint);
				}
			}
			bool flag2 = false;
			if (this.CleanUpInvalidAlternateServiceAccountCredentials.ToBool() && this.alternateServiceAccountCredentialsToRemove.Count > 0)
			{
				foreach (AlternateServiceAccountCredential credential in this.alternateServiceAccountCredentialsToRemove)
				{
					this.alternateServiceAccountConfiguration.RemoveCredential(credential);
				}
				flag2 = true;
			}
			if (this.RemoveAlternateServiceAccountCredentials.ToBool())
			{
				flag2 = this.alternateServiceAccountConfiguration.RemoveAllCredentials();
				flag2 = true;
			}
			if (this.AlternateServiceAccountCredential != null)
			{
				for (int i = this.AlternateServiceAccountCredential.Length - 1; i >= 0; i--)
				{
					this.alternateServiceAccountConfiguration.AddCredential(this.AlternateServiceAccountCredential[i]);
					flag2 = true;
				}
			}
			if (this.DataObject.ObjectState != ObjectState.Unchanged)
			{
				base.InternalProcessRecord();
			}
			else if (!flag && !flag2)
			{
				this.WriteWarning(Strings.WarningForceMessage);
			}
			TaskLogger.LogExit();
		}

		private bool NeedAlternateServiceAccountPasswords
		{
			get
			{
				return this.CleanUpInvalidAlternateServiceAccountCredentials.ToBool() || this.AlternateServiceAccountCredential != null;
			}
		}

		private ClientAccessArray GetClientAccessArrayFromIdParameter()
		{
			if (this.Array == null)
			{
				return null;
			}
			IEnumerable<ClientAccessArray> objects = this.Array.GetObjects<ClientAccessArray>(null, base.DataSession);
			ClientAccessArray result;
			using (IEnumerator<ClientAccessArray> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorClientAccessArrayNotFound(this.Array.ToString()));
				}
				ClientAccessArray clientAccessArray = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorClientAccessArrayNotUnique(this.Array.ToString()));
				}
				result = clientAccessArray;
			}
			return result;
		}

		private const string AutoDiscoverServiceInternalUriTag = "AutoDiscoverServiceInternalUri";

		private const string AutoDiscoverSiteScopeTag = "AutoDiscoverSiteScope";

		private const string AlternateServiceAccountParameterSet = "AlternateServiceAccount";

		private const string AlternateServiceAccountCredentialTag = "AlternateServiceAccountCredential";

		private const string CleanUpInvalidAlternateServiceAccountCredentialsTag = "CleanUpInvalidAlternateServiceAccountCredentials";

		private const string ClientAccessArrayTag = "ClientAccessArray";

		private const string RemoveAlternateServiceAccountCredentialsTag = "RemoveAlternateServiceAccountCredentials";

		private AlternateServiceAccountConfiguration alternateServiceAccountConfiguration;

		private readonly List<AlternateServiceAccountCredential> alternateServiceAccountCredentialsToRemove = new List<AlternateServiceAccountCredential>();
	}
}

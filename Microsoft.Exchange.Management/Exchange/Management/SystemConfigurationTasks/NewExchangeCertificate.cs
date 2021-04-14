using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeCertificate;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ExchangeCertificate", SupportsShouldProcess = true, DefaultParameterSetName = "Certificate")]
	public class NewExchangeCertificate : DataAccessTask<Server>
	{
		[Parameter(Mandatory = false)]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Request")]
		public SwitchParameter GenerateRequest
		{
			get
			{
				return (SwitchParameter)(base.Fields["GenerateRequest"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["GenerateRequest"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Request")]
		public string RequestFile
		{
			get
			{
				return (string)base.Fields["RequestFile"];
			}
			set
			{
				base.Fields["RequestFile"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeAutoDiscover
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeAutoDiscover"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeAutoDiscover"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FriendlyName
		{
			get
			{
				return (string)base.Fields["FriendlyName"];
			}
			set
			{
				base.Fields["FriendlyName"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public X509Certificate2 Instance
		{
			get
			{
				return (X509Certificate2)base.Fields["Instance"];
			}
			set
			{
				base.Fields["Instance"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeAcceptedDomains
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeAcceptedDomains"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeAcceptedDomains"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeServerFQDN
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeServerFQDN"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeServerFQDN"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeServerNetBIOSName
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeServerNetBIOSName"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeServerNetBIOSName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public X500DistinguishedName SubjectName
		{
			get
			{
				return (X500DistinguishedName)base.Fields["SubjectName"];
			}
			set
			{
				base.Fields["SubjectName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpDomainWithSubdomains> DomainName
		{
			get
			{
				return (MultiValuedProperty<SmtpDomainWithSubdomains>)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Certificate")]
		public virtual AllowedServices Services
		{
			get
			{
				return (AllowedServices)(base.Fields["Services"] ?? AllowedServices.SMTP);
			}
			set
			{
				base.Fields["Services"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int KeySize
		{
			get
			{
				return (int)(base.Fields["KeySize"] ?? 0);
			}
			set
			{
				base.Fields["KeySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PrivateKeyExportable
		{
			get
			{
				return (bool)(base.Fields["PrivateKeyExportable"] ?? false);
			}
			set
			{
				base.Fields["PrivateKeyExportable"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SubjectKeyIdentifier
		{
			get
			{
				return (string)base.Fields["SubjectKeyIdentifier"];
			}
			set
			{
				base.Fields["SubjectKeyIdentifier"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Request")]
		public SwitchParameter BinaryEncoded
		{
			get
			{
				return (SwitchParameter)(base.Fields["BinaryEncoded"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BinaryEncoded"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		internal override IConfigurationSession ConfigurationSession
		{
			get
			{
				return (IConfigurationSession)base.DataSession;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return this.GetWhatIfMessage();
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.Server == null)
			{
				this.Server = new ServerIdParameter();
			}
			this.serverObject = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound((string)this.Server)), new LocalizedString?(Strings.ErrorServerNotUnique((string)this.Server)));
			if (!this.serverObject.IsE14OrLater)
			{
				base.WriteError(new ArgumentException(Strings.RemoteCertificateExchangeVersionNotSupported(this.serverObject.Name)), ErrorCategory.InvalidArgument, null);
			}
			base.VerifyIsWithinScopes(DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true), 325, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\NewExchangeCertificate.cs"), this.serverObject, true, new DataAccessTask<Server>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			this.ValidateParameters();
			this.inputParams = new ExchangeCertificateRpc();
			this.inputParams.CreateExportable = this.PrivateKeyExportable;
			this.inputParams.CreateIncAccepted = this.IncludeAcceptedDomains;
			this.inputParams.CreateIncFqdn = this.IncludeServerFQDN;
			this.inputParams.CreateIncNetBios = this.IncludeServerNetBIOSName;
			this.inputParams.CreateIncAutoDisc = this.IncludeAutoDiscover;
			this.inputParams.CreateBinary = this.BinaryEncoded;
			this.inputParams.CreateRequest = this.GenerateRequest;
			this.inputParams.CreateKeySize = this.KeySize;
			this.inputParams.CreateServices = this.Services;
			this.inputParams.CreateAllowConfirmation = !this.Force;
			if (this.FriendlyName != null)
			{
				this.inputParams.CreateFriendlyName = this.FriendlyName;
			}
			if (this.SubjectName != null)
			{
				this.inputParams.CreateSubjectName = this.SubjectName.Name;
			}
			if (this.SubjectKeyIdentifier != null)
			{
				this.inputParams.CreateSubjectKeyIdentifier = this.SubjectKeyIdentifier;
			}
			if (this.DomainName != null)
			{
				this.inputParams.CreateDomains = this.DomainName;
			}
			if (this.Instance != null)
			{
				this.inputParams.CreateCloneCert = this.Instance.Export(X509ContentType.SerializedCert);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (base.HasErrors)
			{
				return;
			}
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = this.inputParams.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(this.serverObject.Name);
				outputBlob = exchangeCertificateRpcClient.CreateCertificate2(0, inBlob);
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
			}
			catch (RpcException)
			{
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			}
			if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
			{
				try
				{
					byte[] inBlob2 = this.inputParams.SerializeInputParameters(ExchangeCertificateRpcVersion.Version1);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(this.serverObject.Name);
					outputBlob = exchangeCertificateRpcClient2.CreateCertificate(0, inBlob2);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.WriteRpcError(e, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			ExchangeCertificateRpc.OutputTaskMessages(this.serverObject, exchangeCertificateRpc, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.GenerateRequest)
			{
				this.ProcessRequestResults(exchangeCertificateRpc.ReturnCert, exchangeCertificateRpc.ReturnCertRequest);
				return;
			}
			if (exchangeCertificateRpc.ReturnConfirmationList != null)
			{
				foreach (KeyValuePair<AllowedServices, LocalizedString> keyValuePair in exchangeCertificateRpc.ReturnConfirmationList)
				{
					if (base.ShouldContinue(keyValuePair.Value))
					{
						ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc();
						exchangeCertificateRpc2.EnableAllowConfirmation = false;
						exchangeCertificateRpc2.EnableServices = keyValuePair.Key;
						AllowedServices key = keyValuePair.Key;
						if (key == AllowedServices.SMTP)
						{
							exchangeCertificateRpc2.EnableUpdateAD = true;
						}
						exchangeCertificateRpc2.EnableByThumbprint = exchangeCertificateRpc.ReturnCert.Thumbprint;
						try
						{
							byte[] inBlob3 = exchangeCertificateRpc2.SerializeInputParameters(exchangeCertificateRpcVersion);
							if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
							{
								ExchangeCertificateRpcClient exchangeCertificateRpcClient3 = new ExchangeCertificateRpcClient(this.serverObject.Name);
								outputBlob = exchangeCertificateRpcClient3.EnableCertificate(0, inBlob3);
							}
							else
							{
								ExchangeCertificateRpcClient2 exchangeCertificateRpcClient4 = new ExchangeCertificateRpcClient2(this.serverObject.Name);
								outputBlob = exchangeCertificateRpcClient4.EnableCertificate2(0, inBlob3);
							}
							exchangeCertificateRpc.ReturnCert.Services |= keyValuePair.Key;
						}
						catch (RpcException e2)
						{
							ManageExchangeCertificate.WriteRpcError(e2, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
						}
						ExchangeCertificateRpc outputValues = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
						ExchangeCertificateRpc.OutputTaskMessages(this.serverObject, outputValues, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
			}
			exchangeCertificateRpc.ReturnCert.Identity = this.serverObject.Fqdn + "\\" + exchangeCertificateRpc.ReturnCert.Thumbprint;
			base.WriteObject(exchangeCertificateRpc.ReturnCert);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(AddAccessRuleCryptographicException).IsInstanceOfType(exception) || typeof(AddAccessRuleArgumentException).IsInstanceOfType(exception) || typeof(AddAccessRuleUnauthorizedAccessException).IsInstanceOfType(exception) || typeof(AddAccessRuleCOMException).IsInstanceOfType(exception);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 527, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\NewExchangeCertificate.cs");
		}

		private void ValidateParameters()
		{
			if (!this.GenerateRequest && this.BinaryEncoded)
			{
				base.WriteError(new ArgumentException(Strings.CertificateRequestMissingForArgument(this.serverObject.Name), "BinaryEncoded"), ErrorCategory.InvalidArgument, null);
			}
			if (!this.GenerateRequest && !string.IsNullOrEmpty(this.RequestFile))
			{
				base.WriteError(new ArgumentException(Strings.CertificateRequestMissingGenerateRequest(this.serverObject.Name), "RequestFile"), ErrorCategory.InvalidArgument, null);
			}
			if (!string.IsNullOrEmpty(this.RequestFile) && !this.IsValidRequestFile(this.RequestFile))
			{
				base.WriteError(new ArgumentException(Strings.CertificateInvalidRequestFile(this.serverObject.Name), "RequestFile"), ErrorCategory.InvalidArgument, null);
			}
		}

		private void ProcessRequestResults(ExchangeCertificate certificate, string request)
		{
			if (this.BinaryEncoded)
			{
				BinaryFileDataObject binaryFileDataObject = new BinaryFileDataObject();
				binaryFileDataObject.FileData = Convert.FromBase64String(request);
				base.WriteObject(binaryFileDataObject);
				if (this.GenerateRequest && !string.IsNullOrEmpty(this.RequestFile))
				{
					this.WriteRequest(binaryFileDataObject.FileData, string.Empty);
					return;
				}
			}
			else
			{
				string text = ManageExchangeCertificate.WrapCertificateRequestWithPemTags(request);
				base.WriteObject(text);
				if (this.GenerateRequest && !string.IsNullOrEmpty(this.RequestFile))
				{
					this.WriteRequest(null, text);
				}
			}
		}

		private bool IsValidRequestFile(string fileName)
		{
			bool result;
			try
			{
				if (File.Exists(fileName))
				{
					result = false;
				}
				else
				{
					using (StreamWriter streamWriter = File.CreateText(fileName))
					{
						streamWriter.Write("H");
					}
					if (File.Exists(fileName))
					{
						File.Delete(fileName);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				result = false;
			}
			catch (DirectoryNotFoundException)
			{
				result = false;
			}
			catch (PathTooLongException)
			{
				result = false;
			}
			catch (ArgumentNullException)
			{
				result = false;
			}
			catch (NotSupportedException)
			{
				result = false;
			}
			catch (ArgumentException)
			{
				result = false;
			}
			catch (SecurityException)
			{
				result = false;
			}
			catch (IOException)
			{
				result = false;
			}
			return result;
		}

		private void WriteRequest(byte[] data, string text)
		{
			try
			{
				string text2 = this.RequestFile;
				string text3 = Path.GetExtension(text2).Replace(".", "").ToUpper();
				text3 = text3.Replace("-", "_");
				if (!Enum.IsDefined(typeof(AllowedCertificateTypes), text3))
				{
					text2 += ".req";
				}
				if (this.BinaryEncoded)
				{
					using (FileStream fileStream = File.Create(text2))
					{
						fileStream.Write(data, 0, data.Length);
						goto IL_96;
					}
				}
				using (StreamWriter streamWriter = File.CreateText(text2))
				{
					streamWriter.Write(text);
				}
				IL_96:;
			}
			catch (IOException ex)
			{
				base.WriteError(new InvalidOperationException(Strings.RequestCertificateFileInvalid(ex.Message)), ErrorCategory.InvalidOperation, null);
			}
		}

		private LocalizedString GetWhatIfMessage()
		{
			this.inputParams.CreateWhatIf = true;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = this.inputParams.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(this.serverObject.Name);
				outputBlob = exchangeCertificateRpcClient.CreateCertificate2(0, inBlob);
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
			}
			catch (RpcException)
			{
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			}
			if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
			{
				try
				{
					byte[] inBlob2 = this.inputParams.SerializeInputParameters(exchangeCertificateRpcVersion);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(this.serverObject.Name);
					outputBlob = exchangeCertificateRpcClient2.CreateCertificate(0, inBlob2);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.WriteRpcError(e, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			this.inputParams.CreateWhatIf = false;
			return exchangeCertificateRpc.ReturnConfirmation;
		}

		private const string BinaryEncodedSwitchName = "BinaryEncoded";

		private const string RequestFileParamName = "RequestFile";

		private const string ForceSwitchName = "Force";

		private const string DefaultFileExt = ".req";

		private Server serverObject;

		private ExchangeCertificateRpc inputParams;

		private static class ParameterSet
		{
			public const string Certificate = "Certificate";

			public const string Request = "Request";
		}
	}
}

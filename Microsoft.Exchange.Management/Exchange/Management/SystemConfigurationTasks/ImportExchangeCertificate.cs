using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeCertificate;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Import", "ExchangeCertificate", DefaultParameterSetName = "FileData", SupportsShouldProcess = true)]
	public class ImportExchangeCertificate : DataAccessTask<Server>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
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

		[Parameter(Mandatory = true, ParameterSetName = "Instance", ValueFromPipeline = true)]
		public string[] Instance
		{
			get
			{
				return (string[])base.Fields["Instance"];
			}
			set
			{
				base.Fields["Instance"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "FileData")]
		public byte[] FileData
		{
			internal get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "FileName")]
		public string FileName
		{
			internal get
			{
				return (string)base.Fields["FileName"];
			}
			set
			{
				base.Fields["FileName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SecureString Password
		{
			internal get
			{
				return (SecureString)base.Fields["Password"];
			}
			set
			{
				base.Fields["Password"] = value;
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
		public string FriendlyName
		{
			internal get
			{
				return (string)base.Fields["FriendlyName"];
			}
			set
			{
				base.Fields["FriendlyName"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 158, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\ImportExchangeCertificate.cs");
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmImportExchangeCertificateDirect;
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
			base.VerifyIsWithinScopes(DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true), 207, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\ImportExchangeCertificate.cs"), this.serverObject, true, new DataAccessTask<Server>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			if (this.Instance == null && this.FileData == null && string.IsNullOrEmpty(this.FileName))
			{
				base.WriteError(new ArgumentException(Strings.ImportCertificateDataIsNull(this.serverObject.Name)), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (base.HasErrors)
			{
				return;
			}
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc();
			string text;
			if (this.FileData != null)
			{
				text = ImportExchangeCertificate.RemoveBase64HeaderFooter(CertificateEnroller.ToBase64String(this.FileData));
			}
			else if (this.Instance != null)
			{
				text = ImportExchangeCertificate.RemoveBase64HeaderFooter(string.Join(null, this.Instance));
			}
			else
			{
				text = ImportExchangeCertificate.RemoveBase64HeaderFooter(CertificateEnroller.ToBase64String(this.GetFileData(this.FileName)));
			}
			if (text.Length == 0)
			{
				base.WriteError(new ImportCertificateDataInvalidException(), ErrorCategory.ReadError, 0);
			}
			exchangeCertificateRpc.ImportCert = text;
			exchangeCertificateRpc.ImportDescription = this.FriendlyName;
			exchangeCertificateRpc.ImportExportable = this.PrivateKeyExportable;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(this.serverObject.Name);
				outputBlob = exchangeCertificateRpcClient.ImportCertificate2(0, inBlob, this.Password);
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
					byte[] inBlob2 = exchangeCertificateRpc.SerializeInputParameters(exchangeCertificateRpcVersion);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(this.serverObject.Name);
					outputBlob = exchangeCertificateRpcClient2.ImportCertificate(0, inBlob2, this.Password);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.WriteRpcError(e, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			ExchangeCertificateRpc.OutputTaskMessages(this.serverObject, exchangeCertificateRpc2, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (exchangeCertificateRpc2.ReturnCert != null)
			{
				exchangeCertificateRpc2.ReturnCert.Identity = this.serverObject.Fqdn + "\\" + exchangeCertificateRpc2.ReturnCert.Thumbprint;
			}
			base.WriteObject(exchangeCertificateRpc2.ReturnCert);
		}

		private byte[] GetFileData(string fileName)
		{
			try
			{
				using (FileStream fileStream = File.OpenRead(fileName))
				{
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, (int)fileStream.Length);
					return array;
				}
			}
			catch (Exception ex)
			{
				if (!(ex is UnauthorizedAccessException) && !(ex is DirectoryNotFoundException) && !(ex is PathTooLongException) && !(ex is ArgumentNullException) && !(ex is NotSupportedException) && !(ex is ArgumentException) && !(ex is SecurityException) && !(ex is IOException))
				{
					throw;
				}
				base.WriteError(new InvalidOperationException(Strings.ImportCertificateFileInvalid(this.serverObject.Name, ex.Message)), ErrorCategory.InvalidOperation, null);
			}
			return null;
		}

		private static string RemoveBase64HeaderFooter(string b64Data)
		{
			b64Data = b64Data.Trim();
			if (b64Data.StartsWith("-----BEGIN CERTIFICATE-----", StringComparison.OrdinalIgnoreCase))
			{
				b64Data = b64Data.Substring("-----BEGIN CERTIFICATE-----".Length);
			}
			else if (b64Data.StartsWith("-----BEGIN PKCS7-----", StringComparison.OrdinalIgnoreCase))
			{
				b64Data = b64Data.Substring("-----BEGIN PKCS7-----".Length);
			}
			if (b64Data.EndsWith("-----END CERTIFICATE-----", StringComparison.OrdinalIgnoreCase))
			{
				b64Data = b64Data.Substring(0, b64Data.Length - "-----END CERTIFICATE-----".Length);
			}
			else if (b64Data.EndsWith("-----END PKCS7-----", StringComparison.OrdinalIgnoreCase))
			{
				b64Data = b64Data.Substring(0, b64Data.Length - "-----END PKCS7-----".Length);
			}
			return b64Data;
		}

		private const string OptionalCertHeader = "-----BEGIN CERTIFICATE-----";

		private const string OptionalCertTrailer = "-----END CERTIFICATE-----";

		private const string OptionalPkcs7Header = "-----BEGIN PKCS7-----";

		private const string OptionalPkcs7Trailer = "-----END PKCS7-----";

		private Server serverObject;
	}
}

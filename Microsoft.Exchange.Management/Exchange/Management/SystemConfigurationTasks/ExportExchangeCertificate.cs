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

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Export", "ExchangeCertificate", SupportsShouldProcess = true, DefaultParameterSetName = "Thumbprint")]
	public class ExportExchangeCertificate : DataAccessTask<Server>, IIdentityExchangeCertificateCmdlet
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0)]
		public ExchangeCertificateIdParameter Identity
		{
			get
			{
				return (ExchangeCertificateIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
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

		[Parameter(Mandatory = false, ParameterSetName = "Thumbprint")]
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

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "Thumbprint", Position = 0)]
		public string Thumbprint
		{
			get
			{
				return (string)base.Fields["Thumbprint"];
			}
			set
			{
				base.Fields["Thumbprint"] = value;
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
		public SwitchParameter BinaryEncoded
		{
			internal get
			{
				return (bool)(base.Fields["BinaryEncoded"] ?? false);
			}
			set
			{
				base.Fields["BinaryEncoded"] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 147, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\ExportExchangeCertificate.cs");
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmExportExchangeCertificate(this.Thumbprint);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			GetExchangeCertificate.PrepareParameters(this);
			this.serverObject = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound((string)this.Server)), new LocalizedString?(Strings.ErrorServerNotUnique((string)this.Server)));
			if (!this.serverObject.IsE14OrLater)
			{
				base.WriteError(new ArgumentException(Strings.RemoteCertificateExchangeVersionNotSupported(this.serverObject.Name)), ErrorCategory.InvalidArgument, null);
			}
			base.VerifyIsWithinScopes(DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true), 189, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\ExportExchangeCertificate.cs"), this.serverObject, false, new DataAccessTask<Server>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			if (string.IsNullOrEmpty(this.Thumbprint))
			{
				base.WriteError(new ArgumentException(Strings.ExceptionEmptyStringNotAllowed, "Thumbprint"), ErrorCategory.InvalidArgument, null);
			}
			this.Thumbprint = ManageExchangeCertificate.UnifyThumbprintFormat(this.Thumbprint);
			if (!string.IsNullOrEmpty(this.FileName) && (File.Exists(this.FileName) || File.Exists(this.FileName + ".pfx")))
			{
				base.WriteError(new ArgumentException(Strings.CertificateInvalidFileName(this.serverObject.Name), "FileName"), ErrorCategory.InvalidArgument, null);
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
			exchangeCertificateRpc.ExportByThumbprint = this.Thumbprint;
			exchangeCertificateRpc.ExportBinary = this.BinaryEncoded;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(this.serverObject.Name);
				outputBlob = exchangeCertificateRpcClient.ExportCertificate2(0, inBlob, this.Password);
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
					outputBlob = exchangeCertificateRpcClient2.ExportCertificate(0, inBlob2, this.Password);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.WriteRpcError(e, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			ExchangeCertificateRpc.OutputTaskMessages(this.serverObject, exchangeCertificateRpc2, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.BinaryEncoded)
			{
				base.WriteObject(new BinaryFileDataObject
				{
					FileData = exchangeCertificateRpc2.ReturnExportFileData
				});
			}
			else
			{
				base.WriteObject(exchangeCertificateRpc2.ReturnExportBase64);
			}
			if (!string.IsNullOrEmpty(this.FileName))
			{
				this.WriteCertiricate(exchangeCertificateRpc2);
			}
		}

		private bool HandleException(Exception e)
		{
			return e is UnauthorizedAccessException || e is DirectoryNotFoundException || e is PathTooLongException || e is ArgumentNullException || e is NotSupportedException || e is ArgumentException || e is SecurityException || e is IOException;
		}

		private void WriteCertiricate(ExchangeCertificateRpc outputValues)
		{
			try
			{
				string text = this.FileName;
				string text2 = Path.GetExtension(text).Replace(".", "").ToUpper();
				text2 = text2.Replace("-", "_");
				if (!Enum.IsDefined(typeof(AllowedCertificateTypes), text2))
				{
					text += ".pfx";
				}
				if (this.BinaryEncoded)
				{
					using (FileStream fileStream = File.Create(text))
					{
						fileStream.Write(outputValues.ReturnExportFileData, 0, outputValues.ReturnExportFileData.Length);
						goto IL_A5;
					}
				}
				using (StreamWriter streamWriter = File.CreateText(text))
				{
					streamWriter.Write(outputValues.ReturnExportBase64);
				}
				IL_A5:;
			}
			catch (Exception ex)
			{
				if (!this.HandleException(ex))
				{
					throw;
				}
				base.WriteError(new InvalidOperationException(Strings.ExportCertificateFileInvalid(ex.Message)), ErrorCategory.InvalidOperation, null);
			}
		}

		private const string ThumbprintParameterName = "Thumbprint";

		private const string BinaryEncodedSwitchName = "BinaryEncoded";

		private const string FileParamName = "FileName";

		private const string DefaultFileExt = ".pfx";

		private Server serverObject;
	}
}

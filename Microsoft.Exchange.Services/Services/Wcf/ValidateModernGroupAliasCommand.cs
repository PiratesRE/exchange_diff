using System;
using System.Diagnostics;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.FederatedDirectory;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class ValidateModernGroupAliasCommand : ServiceCommand<ValidateModernGroupAliasResponse>
	{
		public ValidateModernGroupAliasCommand(CallContext callContext, ValidateModernGroupAliasRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "ValidateModernGroupAliasRequest", "ValidateModernGroupAliasCommand::ValidateModernGroupAliasCommand");
			this.request = request;
			OwsLogRegistry.Register("ValidateModernGroupAlias", typeof(ValidateModernGroupAliasMetadata), new Type[0]);
			WarmupGroupManagementDependency.WarmUpAsyncIfRequired(base.CallContext.AccessingPrincipal);
		}

		protected override ValidateModernGroupAliasResponse InternalExecute()
		{
			IRecipientSession adrecipientSession = base.CallContext.ADRecipientSessionContext.GetADRecipientSession();
			if (!ValidateModernGroupInputHelper.IsSmtpAddressUnique(adrecipientSession, this.request.Alias, this.request.Domain))
			{
				return new ValidateModernGroupAliasResponse(this.request.Alias, false);
			}
			if (!ValidateModernGroupInputHelper.IsAliasValid(adrecipientSession, base.CallContext.ADRecipientSessionContext.OrganizationId, this.request.Alias, new Task.TaskVerboseLoggingDelegate(this.LogHandler), new Task.ErrorLoggerDelegate(this.WriteError), ExchangeErrorCategory.Client))
			{
				return new ValidateModernGroupAliasResponse(this.request.Alias, false);
			}
			if (!ValidateModernGroupInputHelper.IsNameUnique(adrecipientSession, base.CallContext.ADRecipientSessionContext.OrganizationId, this.request.Alias, new Task.TaskVerboseLoggingDelegate(this.LogHandler), new Task.ErrorLoggerDelegate(this.WriteError), ExchangeErrorCategory.Client))
			{
				return new ValidateModernGroupAliasResponse(this.request.Alias, false);
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			AADClient aadclient = AADClientFactory.Create(base.CallContext.ADRecipientSessionContext.OrganizationId, GraphProxyVersions.Version14);
			if (aadclient != null)
			{
				try
				{
					bool flag = aadclient.IsAliasUnique(this.request.Alias);
					base.CallContext.ProtocolLog.Set(ValidateModernGroupAliasMetadata.AADQueryTime, stopwatch.Elapsed);
					if (!flag)
					{
						return new ValidateModernGroupAliasResponse(this.request.Alias, false);
					}
				}
				catch (AADException ex)
				{
					base.CallContext.ProtocolLog.Set(ValidateModernGroupAliasMetadata.AADQueryTime, stopwatch.Elapsed);
					ExTraceGlobals.ModernGroupsTracer.TraceError<SmtpAddress, AADException>((long)this.GetHashCode(), "ValidateModernGroupAliasCommand: User: {0}. Exception: {1}. AADClient.IsAliasUnique failed", base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress, ex);
					base.CallContext.ProtocolLog.Set(ValidateModernGroupAliasMetadata.Exception, ex);
				}
			}
			return new ValidateModernGroupAliasResponse(this.request.Alias, true);
		}

		private void LogHandler(LocalizedString message)
		{
			ExTraceGlobals.ModernGroupsTracer.TraceDebug<SmtpAddress, LocalizedString>((long)this.GetHashCode(), "ValidateModernGroupAliasCommand: User: {0}. Message: {1}", base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress, message);
		}

		private void WriteError(LocalizedException exception, ExchangeErrorCategory category, object target)
		{
			ExTraceGlobals.ModernGroupsTracer.TraceError((long)this.GetHashCode(), "ValidateModernGroupAliasCommand: User: {0}. Exception: {1}. Category: {2}. Target: {3}", new object[]
			{
				base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress,
				exception,
				category,
				target
			});
			base.CallContext.ProtocolLog.Set(ValidateModernGroupAliasMetadata.Exception, exception);
		}

		private readonly ValidateModernGroupAliasRequest request;
	}
}

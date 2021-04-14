using System;
using System.Collections.Generic;
using Microsoft.Ceres.ContentEngine.Parsing.Component;
using Microsoft.Ceres.CoreServices.Tools.Management.Client;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Fast
{
	internal sealed class DocumentFormatManager : FastManagementClient
	{
		internal DocumentFormatManager(string serverName)
		{
			base.DiagnosticsSession.ComponentName = "DocumentFormatManager";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.IndexManagementTracer;
			base.ConnectManagementAgents(serverName);
		}

		protected override int ManagementPortOffset
		{
			get
			{
				return 3;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DocumentFormatManager>(this);
		}

		public void EnableParsing(string format, bool enable)
		{
			Util.ThrowOnNullOrEmptyArgument(format, "format");
			base.PerformFastOperation(delegate()
			{
				this.parsingManagementService.EnableParsing(format, enable);
			}, "EnableParsing");
		}

		public void RemoveFormat(string format)
		{
			Util.ThrowOnNullOrEmptyArgument(format, "format");
			base.PerformFastOperation(delegate()
			{
				this.parsingManagementService.RemoveFormat(format);
			}, "RemoveFormat");
		}

		public void AddFilterBasedFormat(string id, string name, string mime, string extension)
		{
			Util.ThrowOnNullOrEmptyArgument(id, "id");
			Util.ThrowOnNullOrEmptyArgument(name, "name");
			Util.ThrowOnNullOrEmptyArgument(mime, "mime");
			Util.ThrowOnNullOrEmptyArgument(extension, "extension");
			base.PerformFastOperation(delegate()
			{
				this.parsingManagementService.AddFilterBasedFormat(id, name, mime, extension);
			}, "AddFilterBasedFormat");
		}

		public IList<FileFormatInfo> ListSupportedFormats()
		{
			return this.PerformFastOperation<IList<FileFormatInfo>>(() => this.parsingManagementService.ListSupportedFormats(), "ListSupportedFormats");
		}

		public FileFormatInfo GetFormat(string formatId)
		{
			return this.PerformFastOperation<FileFormatInfo>(() => this.parsingManagementService.GetFormat(formatId), "GetFormat");
		}

		protected override void InternalConnectManagementAgents(WcfManagementClient client)
		{
			this.parsingManagementService = client.GetManagementAgent<IParsingManagementAgent>("Parsing/Admin");
		}

		private volatile IParsingManagementAgent parsingManagementService;
	}
}

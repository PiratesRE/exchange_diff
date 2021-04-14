using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal sealed class IdentityMapping
	{
		public IdentityMapping(IRecipientSession recipientSession)
		{
			this.recipientSession = recipientSession;
			this.smtpAddresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public void Prefetch(params string[] smtpAddresses)
		{
			this.smtpAddresses.UnionWith(smtpAddresses);
		}

		public Guid GetIdentityFromSmtpAddress(string smtpAddress)
		{
			this.InitializeIfNeeded();
			Guid result;
			if (this.smtpAddressToIdentity.TryGetValue(smtpAddress, out result))
			{
				return result;
			}
			LogWriter.TraceAndLog(new LogWriter.TraceMethod(IdentityMapping.Tracer.TraceWarning), 1, this.GetHashCode(), "IdentityMapping.GetIdentityFromSmtpAddress: unable to retrieve identity for object with SMTP address {0}", new object[]
			{
				smtpAddress
			});
			return Guid.Empty;
		}

		public void AddToRelation(string[] smtpAddresses, RelationSet relationSet)
		{
			this.InitializeIfNeeded();
			if (smtpAddresses != null && smtpAddresses.Length > 0)
			{
				foreach (string text in smtpAddresses)
				{
					Guid guid;
					if (this.smtpAddressToIdentity.TryGetValue(text, out guid))
					{
						if (!IdentityMapping.RelationSetContains(relationSet, guid))
						{
							relationSet.Add(guid, 1);
						}
					}
					else
					{
						LogWriter.TraceAndLog(new LogWriter.TraceMethod(IdentityMapping.Tracer.TraceWarning), 1, this.GetHashCode(), "IdentityMapping.AddToRelation: unable to retrieve identity for object with SMTP address {0}", new object[]
						{
							text
						});
					}
				}
			}
		}

		public static bool RelationSetContains(RelationSet relationSet, Guid identity)
		{
			foreach (Relation relation in relationSet)
			{
				if (relation.TargetObjectId.Equals(identity))
				{
					return true;
				}
			}
			return false;
		}

		public void RemoveFromRelation(string[] smtpAddresses, RelationSet relationSet)
		{
			this.InitializeIfNeeded();
			if (smtpAddresses != null && smtpAddresses.Length > 0)
			{
				foreach (string text in smtpAddresses)
				{
					Guid guid;
					if (this.smtpAddressToIdentity.TryGetValue(text, out guid))
					{
						relationSet.Remove(guid);
					}
					else
					{
						LogWriter.TraceAndLog(new LogWriter.TraceMethod(IdentityMapping.Tracer.TraceWarning), 1, this.GetHashCode(), "IdentityMapping.RemoveFromRelation: unable to retrieve identity for object with SMTP address {0}", new object[]
						{
							text
						});
					}
				}
			}
		}

		private void InitializeIfNeeded()
		{
			if (this.smtpAddressToIdentity != null)
			{
				return;
			}
			SmtpProxyAddress[] array = new SmtpProxyAddress[this.smtpAddresses.Count];
			int num = 0;
			foreach (string address in this.smtpAddresses)
			{
				array[num] = new SmtpProxyAddress(address, false);
				num++;
			}
			if (IdentityMapping.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				IdentityMapping.Tracer.TraceDebug<string>((long)this.GetHashCode(), "IdentityMapping.InitializeIfNeeded: performing lookup in AD for the following SMTP addresses: {0}", string.Join<SmtpProxyAddress>(",", array));
			}
			Result<ADRawEntry>[] array2 = this.recipientSession.FindByProxyAddresses(array, new PropertyDefinition[]
			{
				ADRecipientSchema.ExternalDirectoryObjectId
			});
			this.smtpAddressToIdentity = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < array2.Length; i++)
			{
				string smtpAddress = array[i].SmtpAddress;
				Result<ADRawEntry> result = array2[i];
				if (result.Data == null)
				{
					LogWriter.TraceAndLog(new LogWriter.TraceMethod(IdentityMapping.Tracer.TraceError), 1, this.GetHashCode(), "IdentityMapping.InitializeIfNeeded: lookup object AD object SMTP addresses '{0}' failed due error: {1}", new object[]
					{
						smtpAddress,
						result.Error
					});
				}
				else
				{
					string text = result.Data[ADRecipientSchema.ExternalDirectoryObjectId] as string;
					Guid guid;
					if (text != null && Guid.TryParse(text, out guid))
					{
						this.smtpAddressToIdentity.Add(smtpAddress, guid);
						LogWriter.TraceAndLog(new LogWriter.TraceMethod(IdentityMapping.Tracer.TraceDebug), 4, this.GetHashCode(), "IdentityMapping.InitializeIfNeeded: AD object with SMTP addresses '{0}' maps to ExternalDirectoryObjectId '{1}'", new object[]
						{
							smtpAddress,
							guid
						});
					}
					else
					{
						LogWriter.TraceAndLog(new LogWriter.TraceMethod(IdentityMapping.Tracer.TraceError), 1, this.GetHashCode(), "IdentityMapping.InitializeIfNeeded: ExternalDirectoryObjectId is either empty or not valid guid for AD object with SMTP address: '{0}'", new object[]
						{
							smtpAddress
						});
					}
				}
			}
			this.smtpAddresses.Clear();
		}

		private static readonly Trace Tracer = ExTraceGlobals.FederatedDirectoryTracer;

		private readonly IRecipientSession recipientSession;

		private readonly HashSet<string> smtpAddresses;

		private Dictionary<string, Guid> smtpAddressToIdentity;
	}
}

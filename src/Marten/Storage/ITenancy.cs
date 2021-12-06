using System;
using Marten.Schema;

namespace Marten.Storage
{
    public interface ITenancy
    {
        ITenant GetTenant(string tenantId);
        ITenant Default { get; }

        [Obsolete("Don't hang that here")]
        IDocumentCleaner Cleaner { get; }

        [Obsolete("Don't hang that here")]
        IDocumentSchema Schema { get; }
    }
}

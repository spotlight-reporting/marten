using Marten.Events;

namespace Marten.Storage
{
    internal static class TenantExtensions
    {
        internal static IEventStorage EventStorage(this ITenant tenant)
        {
            return (IEventStorage)tenant.StorageFor<IEvent>();
        }
    }
}

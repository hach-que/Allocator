using System.Threading.Tasks;

namespace AllocatorLib
{
    public interface IAllocator
    {
        bool CanLease(Specification spec);
        Task<ILease> Lease(Specification spec);
        ILease WaitForLease(Specification spec);
    }
}

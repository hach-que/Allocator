using System.Threading.Tasks;

namespace AllocatorLib
{
    public interface IResource
    {
        bool CanLease(IAllocator allocator, Specification spec);
        Task<ILease> Lease(IAllocator allocator, Specification spec);
        void Deallocate();
    }
}

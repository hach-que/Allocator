using System.Threading.Tasks;

namespace AllocatorLib
{
    public interface IBlueprint
    {
        bool CanAllocate(IAllocator allocator, Specification spec);
        Task<IResource> Allocate(IAllocator allocator, Specification spec);
    }
}

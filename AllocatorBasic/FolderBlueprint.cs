using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AllocatorLib;

namespace AllocatorBasic
{
    public class FolderBlueprint : IBlueprint
    {
        public bool CanAllocate(IAllocator allocator, Specification spec)
        {
            return spec.ResourceType == "Folder" &&
                Directory.Exists("C:\\Workspace");
        }

        private string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        public async Task<IResource> Allocate(IAllocator allocator, Specification spec)
        {
            var directory = new DirectoryInfo("C:\\Workspace");
            return new FolderResource(directory.CreateSubdirectory("FolderAllocation_" + RandomString()));
        }
    }

    public class FolderResource : IResource
    {
        private FolderLease _folderLease;

        public DirectoryInfo Directory { get; private set; }
        public bool LeasedOut { get; private set; }

        public FolderResource(DirectoryInfo directoryInfo)
        {
            Directory = directoryInfo;
            LeasedOut = false;
        }

        public bool CanLease(IAllocator allocator, Specification spec)
        {
            return spec.ResourceType == "Folder" && !LeasedOut;
        }

        public async Task<ILease> Lease(IAllocator allocator, Specification spec)
        {
            LeasedOut = true;
            _folderLease = new FolderLease(this, Directory);
            return _folderLease;
        }

        public void Deallocate()
        {
            if (_folderLease == null) return;
            var lease = _folderLease;
            _folderLease = null;
            lease.Release();
            LeasedOut = false;
        }
    }

    public class FolderLease : ILease
    {
        private FolderResource _folderResource;
        public DirectoryInfo Directory { get; private set; }
        public int ID { get; private set; }
        public DateTime? Expiry { get; private set; }

        public FolderLease(FolderResource resource, DirectoryInfo directoryInfo)
        {
            _folderResource = resource;
            Directory = directoryInfo;
            ID = 0;
            Expiry = null;
        }

        public void Release()
        {
            if (!System.IO.Directory.Exists(Directory.FullName)) return;
            Directory.Delete();
            _folderResource.Deallocate();
        }
    }
}

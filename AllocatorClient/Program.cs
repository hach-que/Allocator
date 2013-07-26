using System;
using System.Diagnostics;
using AllocatorBasic;
using AllocatorLib;

namespace AllocatorClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // force reference for reflection.
            var folderBlueprint = new FolderBlueprint();

            var spec = new Specification("Folder", new object());

            var allocator = new DefaultAllocator();
            if (!allocator.CanLease(spec))
            {
                Console.WriteLine("Can't provide leased folder.");
            }
            var folderLease = allocator.Lease(spec).Result as FolderLease;
            Debug.Assert(folderLease != null, "folderLease != null");
            Console.WriteLine("Got folder lease at " + folderLease.Directory.FullName);
            folderLease.Release();
        }
    }
}

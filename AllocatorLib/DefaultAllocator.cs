using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllocatorLib
{
    public class DefaultAllocator : IAllocator
    {
        private readonly List<IResource> _activeResources = new List<IResource>();
        private readonly List<IBlueprint> _blueprints = new List<IBlueprint>();

        public DefaultAllocator()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes().Where(type => typeof (IBlueprint).IsAssignableFrom(type)))
                .Where(type => !type.IsInterface))
            {
                _blueprints.Add(Activator.CreateInstance(type) as IBlueprint);
            }
        }

        ~DefaultAllocator()
        {
            foreach (var resource in _activeResources)
                resource.Deallocate();
        }

        public bool CanLease(Specification spec)
        {
            if (_activeResources.Any(resource => resource.CanLease(this, spec)))
                return true;
            return _blueprints.Any(blueprint => blueprint.CanAllocate(this, spec));
        }

        public async Task<ILease> Lease(Specification spec)
        {
            // Check to see if any resources can give us a lease.
            foreach (var resource in _activeResources.Where(resource => resource.CanLease(this, spec)))
                return await resource.Lease(this, spec);

            // Nope, see if any blueprints can allocate a resource that
            // will give us a lease.
            foreach (var blueprint in _blueprints.Where(blueprint => blueprint.CanAllocate(this, spec)))
            {
                var resource = await blueprint.Allocate(this, spec);
                if (resource.CanLease(this, spec))
                    return await resource.Lease(this, spec);
                resource.Deallocate();
                throw new Exception("Blueprint stated that it could allocate a resource matching the spec, but the new resource could not lease.");
            }

            // Otherwise throw an exception.
            throw new Exception("Lease can't be provided.");
        }

        public ILease WaitForLease(Specification spec)
        {
            return Lease(spec).Result;
        }
    }
}

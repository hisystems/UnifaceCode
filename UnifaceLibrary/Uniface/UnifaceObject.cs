using System;

namespace UnifaceLibrary
{
    public class UnifaceObject
    {
        public UnifaceObjectId Id { get; }

        public UnifaceObject(UnifaceObjectId id)
        {
            Id = id;
            
        }
    }
}

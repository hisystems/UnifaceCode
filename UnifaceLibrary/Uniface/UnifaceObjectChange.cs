namespace UnifaceLibrary
{
    public class UnifaceObjectChange
    {
        public UnifaceObject Object { get; }
        public ChangeOperation ChangeOperation { get; }

        public UnifaceObjectChange(UnifaceObject unifaceObject, ChangeOperation changeOperation)
        {
            Object = unifaceObject;
            ChangeOperation = changeOperation;
        }
    }
}
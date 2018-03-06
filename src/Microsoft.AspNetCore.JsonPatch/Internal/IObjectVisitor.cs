namespace Microsoft.AspNetCore.JsonPatch.Internal
{
    public interface IObjectVisitor
    {
        bool TryVisit(ref object target, out IAdapter adapter, out string errorMessage);
    }
}
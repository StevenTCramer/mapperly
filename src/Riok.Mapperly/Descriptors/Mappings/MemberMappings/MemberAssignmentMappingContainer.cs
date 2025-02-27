using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Riok.Mapperly.Descriptors.Mappings.MemberMappings;

/// <summary>
/// A default implementation for <see cref="IMemberAssignmentMappingContainer"/>.
/// </summary>
public abstract class MemberAssignmentMappingContainer : IMemberAssignmentMappingContainer
{
    private readonly HashSet<IMemberAssignmentMapping> _delegateMappings = new();
    private readonly HashSet<IMemberAssignmentMappingContainer> _childContainers = new();
    private readonly IMemberAssignmentMappingContainer? _parent;

    protected MemberAssignmentMappingContainer(IMemberAssignmentMappingContainer? parent = null)
    {
        _parent = parent;
    }

    public virtual IEnumerable<StatementSyntax> Build(TypeMappingBuildContext ctx, ExpressionSyntax targetAccess)
    {
        var childContainerStatements = _childContainers.SelectMany(x => x.Build(ctx, targetAccess));
        var mappings = _delegateMappings.SelectMany(m => m.Build(ctx, targetAccess));
        return childContainerStatements.Concat(mappings);
    }

    public void AddMemberMappingContainer(IMemberAssignmentMappingContainer container)
    {
        if (!HasMemberMappingContainer(container))
        {
            _childContainers.Add(container);
        }
    }

    public bool HasMemberMappingContainer(IMemberAssignmentMappingContainer container) =>
        _childContainers.Contains(container) || _parent?.HasMemberMappingContainer(container) == true;

    public void AddMemberMapping(IMemberAssignmentMapping mapping)
    {
        if (!HasMemberMapping(mapping))
        {
            _delegateMappings.Add(mapping);
        }
    }

    public bool HasMemberMapping(IMemberAssignmentMapping mapping) =>
        _delegateMappings.Contains(mapping) || _parent?.HasMemberMapping(mapping) == true;
}

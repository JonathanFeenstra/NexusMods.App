using System.Reactive;
using NexusMods.Abstractions.GameLocators;
using NexusMods.Abstractions.Games.Trees;
using NexusMods.Paths;
using NexusMods.Paths.Trees;
using NexusMods.Paths.Trees.Traits;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NexusMods.App.UI.Controls.Trees.Files;

public class FileTreeNodeViewModel<TValue> : AViewModel<IFileTreeNodeViewModel>, IFileTreeNodeViewModel
{
    private KeyedBox<RelativePath, GamePathNode<TValue>> _item;
    
    [Reactive]
    public FileTreeNodeIconType Icon { get; private set; }
    public string Name => _item.Item.Segment;
    public long FileSize => 999999; // TEMP
    public GamePath FullPath => _item.GamePath();
    public GamePath ParentPath => _item.Parent()!.GamePath();

    public ReactiveCommand<Unit, Unit> ViewCommand { get; } = ReactiveCommand.Create(() => {});

    public FileTreeNodeViewModel(KeyedBox<RelativePath, GamePathNode<TValue>> item)
    {
        _item = item;
        Icon = _item.Item.IsFile ? FileTreeNodeIconType.OpenFolder : FileTreeNodeIconType.ClosedFolder;
    }
}

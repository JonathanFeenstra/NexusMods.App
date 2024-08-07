using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusMods.Abstractions.FileStore.Trees;
using NexusMods.Abstractions.GameLocators;
using NexusMods.Abstractions.Installers;
using NexusMods.Abstractions.Library.Installers;
using NexusMods.Abstractions.Library.Models;
using NexusMods.Abstractions.Loadouts;
using NexusMods.MnemonicDB.Abstractions;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;
using NexusMods.Paths.Trees.Traits;

namespace NexusMods.StandardGameLocators.TestHelpers.StubbedGames;

public class StubbedGameInstaller : ALibraryArchiveInstaller, IModInstaller
{
    private readonly RelativePath _preferencesPrefix = "preferences".ToRelativePath();
    private readonly RelativePath _savesPrefix = "saves".ToRelativePath();
    public StubbedGameInstaller(IServiceProvider serviceProvider) : base(serviceProvider, serviceProvider.GetRequiredService<ILogger<StubbedGameInstaller>>())
    {
    }

    public ValueTask<IEnumerable<ModInstallerResult>> GetModsAsync(
        ModInstallerInfo info,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(GetMods(info));
    }

    private IEnumerable<ModInstallerResult> GetMods(ModInstallerInfo info)
    {
        var modFiles = info.ArchiveFiles.GetFiles()
            .Select(kv =>
            {
                var path = kv.Path();
                if (path.Path.StartsWith(_preferencesPrefix))
                    return kv.ToStoredFile(new GamePath(LocationId.Preferences, path));

                if (path.Path.StartsWith(_savesPrefix))
                    return kv.ToStoredFile(new GamePath(LocationId.Saves, path));

                return kv.ToStoredFile(new GamePath(LocationId.Game, path));
            });

        yield return new ModInstallerResult
        {
            Id = info.BaseModId,
            Files = modFiles.AsEnumerable()
        };
    }
    
    public override ValueTask<InstallerResult> ExecuteAsync(
        LibraryArchive.ReadOnly libraryArchive,
        LoadoutItemGroup.New loadoutGroup,
        ITransaction tx,
        Loadout.ReadOnly loadout,
        CancellationToken cancellationToken)
    {
        var modFiles = libraryArchive.GetTree().GetFiles()
            .Select(kv =>
            {
                var path = kv.Item.Path;
                if (path.Path.StartsWith(_preferencesPrefix))
                    return kv.ToLoadoutFile(loadout, loadoutGroup, tx, new GamePath(LocationId.Preferences, path));

                if (path.Path.StartsWith(_savesPrefix))
                    return kv.ToLoadoutFile(loadout, loadoutGroup, tx, new GamePath(LocationId.Saves, path));

                return kv.ToLoadoutFile(loadout, loadoutGroup, tx, new GamePath(LocationId.Game, path));
            })
            .ToArray();

        return modFiles.Length == 0
            ? ValueTask.FromResult<InstallerResult>(new NotSupported())
            : ValueTask.FromResult<InstallerResult>(new Success());
    }
}

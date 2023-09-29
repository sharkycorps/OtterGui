using System;
using System.Collections.Generic;
using Dalamud.Data;
using Dalamud.Interface.Internal;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ImGuiScene;
using Lumina.Data.Files;

namespace OtterGui.Classes;

public class IconStorage : IDisposable
{
    private readonly DalamudPluginInterface        _pi;
    private readonly IDataManager                   _gameData;
    private readonly ITextureProvider               _textureProvider;
    private readonly Dictionary<uint, IDalamudTextureWrap> _icons;

    public IconStorage(DalamudPluginInterface pi, IDataManager gameData, ITextureProvider textureProvider, int size = 0)
    {
        _pi       = pi;
        _gameData = gameData;
        _textureProvider = textureProvider;
        _icons    = new Dictionary<uint, IDalamudTextureWrap>(size);
    }

    private static string HqPath(uint id)
        => $"ui/icon/{id / 1000 * 1000:000000}/{id:000000}_hr1.tex";

    private static string NormalPath(uint id)
        => $"ui/icon/{id / 1000 * 1000:000000}/{id:000000}.tex";

    public bool IconExists(uint id)
        => _gameData.FileExists(HqPath(id)) || _gameData.FileExists(NormalPath(id));

    public IDalamudTextureWrap this[int id]
        => LoadIcon(id);

    private TexFile? LoadIconHq(uint id)
        => _gameData.GetFile<TexFile>(HqPath(id));

    public IDalamudTextureWrap LoadIcon(int id)
        => LoadIcon((uint)id);  

    public IDalamudTextureWrap LoadIcon(uint id)
    {
        if (_icons.TryGetValue(id, out var ret))
            return ret;

        ret = _textureProvider.GetIcon(id);

        _icons[id] = ret;
        return ret;
    }

    public void Dispose()
    {
        foreach (var icon in _icons.Values)
            icon.Dispose();
        _icons.Clear();
    }

    ~IconStorage()
        => Dispose();
}

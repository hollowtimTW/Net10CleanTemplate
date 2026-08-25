using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace YourApp.FileStorage;

public sealed record StoredFile(
    string Id,                // GUID or content hash
    string FileName,
    string ContentType,
    long SizeBytes,
    string Path,              // backend-specific URI/path
    DateTime UploadedAtUtc);

public interface IFileStorage
{
    ValueTask<StoredFile> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    ValueTask<Stream> OpenReadAsync(string id, CancellationToken ct = default);
    ValueTask DeleteAsync(string id, CancellationToken ct = default);
}

public sealed class LocalFileStorage(string rootPath) : IFileStorage
{
    private readonly string _root = Path.GetFullPath(rootPath);
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, StoredFile> _index = new();

    public async ValueTask<StoredFile> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        Directory.CreateDirectory(_root);
        var id = Guid.NewGuid().ToString("N");
        var fullPath = Path.Combine(_root, id);
        await using (var fs = File.Create(fullPath))
        {
            await stream.CopyToAsync(fs, ct);
        }
        var info = new FileInfo(fullPath);
        var stored = new StoredFile(id, fileName, contentType, info.Length, fullPath, DateTime.UtcNow);
        _index[id] = stored;
        return stored;
    }

    public ValueTask<Stream> OpenReadAsync(string id, CancellationToken ct = default)
    {
        if (!_index.TryGetValue(id, out var stored))
            throw new FileNotFoundException(id);
        Stream stream = File.OpenRead(stored.Path);
        return ValueTask.FromResult(stream);
    }

    public ValueTask DeleteAsync(string id, CancellationToken ct = default)
    {
        if (_index.TryRemove(id, out var stored))
            File.Delete(stored.Path);
        return ValueTask.CompletedTask;
    }
}

public static class FileStorageExtensions
{
    public static IServiceCollection AddLocalFileStorage(this IServiceCollection services, string rootPath)
    {
        services.AddSingleton<IFileStorage>(_ => new LocalFileStorage(rootPath));
        return services;
    }
}
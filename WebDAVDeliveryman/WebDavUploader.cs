using System;

namespace WebDAVDeliveryman;

using WebDav;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

internal class WebDavUploader
{
    private readonly IWebDavClient _client;
    private readonly Config _config;

    public WebDavUploader(IWebDavClient client, Config config)
    {
        _client = client;
        _config = config;
    }

    public async Task UploadDirectoryAsync(string root)
    {
        await UploadDirectoryRecursiveAsync(root, root);
    }

    private async Task UploadDirectoryRecursiveAsync(string root, string current)
    {
        foreach (var dir in Directory.GetDirectories(current))
        {
            var dirName = Path.GetFileName(dir);

            if (_config.IgnoreFolders.Contains(dirName, StringComparer.OrdinalIgnoreCase))
                continue;

            var remoteDir = GetRemotePath(root, dir);
            await _client.Mkcol(remoteDir);

            await UploadDirectoryRecursiveAsync(root, dir);
        }

        foreach (var file in Directory.GetFiles(current))
        {
            var fileName = Path.GetFileName(file);

            if (_config.IgnoreFiles.Any(pattern => MatchesPattern(fileName, pattern)))
                continue;

            Console.WriteLine($"- Uploading: {file}");

            using var stream = File.OpenRead(file);
            var remoteFile = GetRemotePath(root, file);

            var result = await _client.PutFile(remoteFile, stream);
            if (!result.IsSuccessful)
                Console.WriteLine($"Error uploading {file}: {result.Description}");
        }
    }

    private static bool MatchesPattern(string fileName, string pattern) =>
        System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(pattern, fileName);

    private static string GetRemotePath(string root, string path)
    {
        var relative = path.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar);
        return "/" + relative.Replace(Path.DirectorySeparatorChar, '/');
    }
}


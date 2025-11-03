using WebDav;
using System.Text.Json;
using System.Net;
using WebDAVDeliveryman;


if (args.Length == 0)
{
    Console.WriteLine("Usage: wddm <folder>");
}

var folderPath = args[0];
if (!Directory.Exists(folderPath))
{
    Console.WriteLine($"Folder not found: {folderPath}");
    return;
}


var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
if (!File.Exists(configPath))
{
    Console.WriteLine("Config file not found: config.json");
    return;
}
var configRaw = File.ReadAllText(configPath);
var config = JsonSerializer.Deserialize<Config>(configRaw, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

var client = new WebDavClient(new WebDavClientParams
{
    BaseAddress = new Uri(config.Url),
    Credentials = new NetworkCredential(config.Username, config.Password)
});


var uploader = new WebDavUploader(client, config);
await uploader.UploadDirectoryAsync(folderPath);

Console.WriteLine("Done!");




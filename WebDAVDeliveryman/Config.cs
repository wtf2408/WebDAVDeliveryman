using System;

namespace WebDAVDeliveryman;

internal record Config(
    string Url,
    string Username,
    string Password,
    string[] IgnoreFolders,
    string[] IgnoreFiles
);

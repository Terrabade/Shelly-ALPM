using System.Collections.Generic;

namespace PackageManager.Alpm.Package;

public record AlpmPackageFileDto(string Name)
{
    public List<AlpmPackageFileDto> Files { get; } = [];
}
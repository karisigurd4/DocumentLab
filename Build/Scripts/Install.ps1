param($installPath, $toolsPath, $package, $project)

foreach ($folder in $project.ProjectItems.Item("Data").ProjectItems)
{
    foreach ($file in $folder.ProjectItems)
    {
        $file.Properties.Item("CopyToOutputDirectory").Value = 1
    }
}

foreach ($file in $project.ProjectItems.Item("Data").ProjectItems.Item("Context").ProjectItems.Item("swe").ProjectItems)
{
  $file.Properties.Item("CopyToOutputDirectory").Value = 1
}

$includeFolders = "tessdata", "x86", "x64"

foreach ($include in $includeFolders) 
{
    foreach ($file in $project.ProjectItems.Item($include).ProjectItems)
    {
        $file.Properties.Item("CopyToOutputDirectory").Value = 1
    }
}
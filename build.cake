var target = Argument("target", "default");
var configuration = Argument("configuration", "Release");
var solutionFile = File("./devmon_agent.sln");
var buildDir = Directory("./bin");
var buildConfigDirectory = buildDir + Directory(configuration);
var packagesDirectory = Directory("./packages");

Task("clean")
	.Does(() =>
{
	CleanDirectory(buildDir);
});

Task("build-anycpu")
	.Does(() =>
{
	NuGetRestore(solutionFile);
	MSBuild(solutionFile, settings => settings
		.SetConfiguration(configuration)
		.SetPlatformTarget(PlatformTarget.MSIL)
		.WithTarget("Rebuild")
		.SetVerbosity(Verbosity.Minimal));
});

Task("release")
	.Does(() =>
{
	CopyFile("./LICENSE", buildConfigDirectory.ToString() + "/LICENSE.txt");
	
	var licensesDir = buildConfigDirectory + Directory("licenses");
	CreateDirectory(licensesDir);
	CopyFiles("./licenses/*", licensesDir);
	
	var ignoredExt = new string[] { ".xml", ".pdb" };
	var files = GetFiles(buildConfigDirectory.ToString() + "/**/*")
		.Where(f => !ignoredExt.Contains(f.GetExtension().ToLowerInvariant()));
		
	Zip(buildConfigDirectory, buildDir.ToString() + "/devmon_agent.zip", files);
});

Task("build")
	.IsDependentOn("clean")
	.IsDependentOn("build-anycpu")
	.IsDependentOn("release")
	.Does(() =>
{
});

Task("Default")
    .IsDependentOn("build")
	.Does(()=> 
{ 
});
	
RunTarget(target);

Framework('4.5.1')
$erroractionpreference = "Stop"

properties {
    $location = (get-location);
    $outdir = (join-path $location "Build");
    $artifactsdir = (join-path $outdir "Artifacts");
    $bindir = (join-path $outdir "Bin");
}

task default -depends Help
task ci -depends rebuild,test
task appvayor -depends rebuild,rebuildtests         # appvayor can discover the tests


task Help {
  write-host "FolgendeTargets: ci, rebuild, test" -ForegroundColor Yellow
}

task Check {
}


task Clean -depends NugetRestore {
	[void](rmdir -force -recurse $outdir -ea SilentlyContinue)
}

task NugetRestore {
  exec { .nuget\nuget.exe restore }
}

task Rebuild -depends Clean {
  exec { .nuget\nuget.exe restore }

	$solution = get-location;

  exec { msbuild /nologo /v:minimal /t:rebuild /p:"Configuration=Release;OutputPath=$bindir/Lucene.Zealous/;SolutionDir=$solution/" "Source/Lucene.Zealous/Lucene.Zealous.csproj" }
}

task rebuildtests -depends Clean {
  exec { .nuget\nuget.exe restore }

	$solution = get-location;

  exec { msbuild /nologo /v:minimal /t:rebuild /p:"Configuration=Release;OutputPath=$bindir/Lucene.Zealous.Tests/;SolutionDir=$solution/" "Source/Lucene.Zealous.Tests/Lucene.Zealous.Tests.csproj" }
}

task Test -depends Clean {
  [void](mkdir $artifactsdir)

  exec { .nuget\nuget.exe restore }

  exec {.nuget\nuget install Machine.Specifications.Runner.Console -OutputDirectory Packages}
	$mspecdir = (resolve-path ".\Packages\Machine.Specifications.Runner.Console.0.*\")
	$mspec = @("$mspecdir\tools\mspec-x86-clr4.exe", "--xml", "$artifactsdir\mspec-results.xml", "--html", "$artifactsdir\mspec-results.html");

	foreach($testProj in (dir -Filter ".\Source\*.Tests")){
		exec { msbuild /nologo /v:m /t:rebuild /p:"Configuration=Release;OutputPath=$bindir/$testProj" "Source/$testProj/$testProj.csproj" }
		$mspec += "$bindir\$testProj\$testProj.dll";
	}

  write-host $mspec
	try {exec { &([scriptblock]::create($mspec -join ' ')) }} catch {}

	$xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
  $xslt.Load("Packages\MSpecToJUnit\MSpecToJUnit.xlt");
  $xslt.Transform("$artifactsdir\mspec-results.xml", "$artifactsdir\junit-results.xml");
}

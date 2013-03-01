properties { 
  $base_dir  = resolve-path ..\
  $build_base_dir = "$base_dir\build"
  $build_dir = "$build_base_dir\build"
  $packageinfo_dir = "$base_dir\GoodlyFere.Import"
  $debug_build_dir = "$build_dir\bin\debug"
  $release_build_dir = "$build_dir\bin\release"
  $release_dir = "$build_base_dir\Release"
  $sln_file = "$base_dir\GoodlyFere.Import.sln"
  $tools_dir = "$build_base_dir\Tools"
  $run_tests = $true
  $xunit_console = "$tools_dir\xunit.console.clr4.exe"
}

Framework "4.0"

task default -depends Package

task Clean {
  if (Test-Path $build_dir) { remove-item -force -recurse $build_dir }
  if (Test-Path $release_dir) { remove-item -force -recurse $release_dir }
}

task Init -depends Clean {
	mkdir @($release_dir, $build_dir) | out-null
	
    #UpdateVersion
}

task Compile -depends Init {
  Exec { msbuild $sln_file /p:"OutDir=$debug_build_dir\;Configuration=Debug;TargetFrameworkVersion=v4.5" } "msbuild (debug) failed."
  Exec { msbuild $sln_file /p:"OutDir=$release_build_dir\;Configuration=Release;TargetFrameworkVersion=v4.5" } "msbuild (release) failed."
}

task Test -depends Compile -precondition { return $run_tests }{
  cd $debug_build_dir
  Exec { & $xunit_console "GoodlyFere.Import.Tests.dll" } "xunit failed."
}

task Package -depends Compile, Test {
  $spec_files = @(Get-ChildItem $packageinfo_dir "*.nuspec" -Recurse)

  foreach ($spec in @($spec_files))
  {
	$dir =  $($spec.Directory)
	cd $dir
    Exec { nuget pack -o $release_dir -Properties Configuration=Release`;OutDir=$release_build_dir\ -Symbols } "nuget pack failed."
  }
}

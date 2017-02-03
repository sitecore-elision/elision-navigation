param($installPath, $toolsPath, $package, $project)

function RemoveProjectFromSolution($deploySource, $rootDir, $name) {   
    
    $deployTarget = $rootDir + "\" + $name + "\"
		
    $proj = $dte.Solution.Projects | ? { $_.ProjectName -eq $name } | % { $dte.Solution.Remove($_) };	
	
    # remove deploy target directory if it exists
    
    if ((test-path $deployTarget)) {
        Write-Host "Removing source files located at $deployTarget..."
    	Remove-Item -Recurse -Force $deployTarget
    }
    else{
        Write-Host "Could not find directory where $name is installed.  Checked at $deployTarget.  Manual removal of directory will be necessary."
        return
    }
}


$deploySource = join-path $installPath 'lib/Elision'
$rootDir = (Get-Item $installPath).parent.parent.fullname

ls $deploySource | % {
    Write-Host "Removing contents of $($_.Name) from solution..."
    RemoveProjectFromSolution ($deploySource + "\" + $_.Name) $rootDir $_.Name
} > $null
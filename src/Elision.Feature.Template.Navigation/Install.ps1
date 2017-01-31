param($installPath, $toolsPath, $package, $project)

# find out where to put the files, we're going to create a deploy directory
# at the same level as the solution.

function AddProjectToSolution($deploySource, $rootDir, $name) {   
    
    $deployTarget = $rootDir + "\" + $name + "\"
    # create our deploy target directory if it doesn't exist yet   
    
    if (!(test-path $deployTarget)) {
    	mkdir $deployTarget
    }
    else{
        Write-Host "$deployTarget is already in place, will not add to solution."
        return
    }
    
    # copy everything in there
    Copy-Item "$deploySource/*" $deployTarget -Recurse -Force
    
    # get the active solution
    $solution = Get-Interface $dte.Solution ([EnvDTE80.Solution2])
    
    # add all our project files to the directory we created    
    $projPath = $deployTarget + $name + ".csproj"
    $solution.AddFromFile($projPath, $false)
}


$deploySource = join-path $installPath 'lib/Elision'
$rootDir = (Get-Item $installPath).parent.parent.fullname

ls $deploySource | foreach-object {
    Write-Host "Adding contents of $_.Name to solution..."
    AddProjectToSolution ($deploySource + "\" + $_.Name) $rootDir $_.Name
} > $null
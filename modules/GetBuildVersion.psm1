Function GetBuildVersion {
    Param (
        [string]$VersionString
    )

    # Initialize default values
    $Major = 0
    $Minor = 0
    $Patch = 0
    $PreReleaseTag = ""
    $BuildRevision = 0

    # Process through regex
    if ($VersionString -match "^(?<major>\d+)(\.(?<minor>\d+))?(\.(?<patch>\d+))?(\-(?<pre>[0-9A-Za-z\-\.]+))?(\+(?<build>\d+))?$") {
        if ($matches['major']) { $Major = [uint64]$matches['major'] }
        if ($matches['minor']) { $Minor = [uint64]$matches['minor'] }
        if ($matches['patch']) { $Patch = [uint64]$matches['patch'] }
        if ($matches['pre']) { $PreReleaseTag = [string]$matches['pre'] }
        if ($matches['build']) { $BuildRevision = [uint64]$matches['build'] }
    } else {
        return "1.0.0-build"
    }

    # Construct the version string
    $Version = [string]$Major + '.' + [string]$Minor + '.' + [string]$Patch
    if ($PreReleaseTag -ne [string]::Empty) {
        $Version = $Version + '-' + $PreReleaseTag
    }

    if ($BuildRevision -ne 0) {
        $Version = $Version + '+' + [string]$BuildRevision
    }

    return $Version
}
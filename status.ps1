gci -r -Depth 2 .git -force | %{
	$_.Parent.Fullname -replace [regex]::Escape($pwd),"."
	cd $_.Parent; 
	git st; 
	popd
}

# !/bin/bash

compile='false'
run='false'
debug='false'
verbose='false'

while getopts 'cdvrh' opt; do
	case $opt in
		'c')
			compile='true'
			;;
		'r')
			run='true'
			;;
		'd')
			debug='true'
			;;
		'v')
			verbose='true'
			;;
		'h')
			#echo " **** MONO COMPILER FOR DND.NET ****"
			#echo "This script uses Mono to build and/or"
			#echo "run the dnd.NET software."
			#echo ""
			#echo "Use the -c option to compile, and the"
			#echo "-r option to run."
			#echo "When compiling, the output file is"
			#echo "./bin/dnd.NET.exe, which can be run "
			#echo "with the mono command, or with Wine."
			#echo " **** MONO COMPILER FOR DND.NET ****"
			#;;
			echo "Not implemented yet..."
			;;
		*)
			echo "Unknown option $opt"
			;;
	esac
done

if [[ $compile == 'true' ]]; then
	if [[ $verbose == 'true' ]]; then
		echo 'Verbose enabled: compiling verbose logging version to bin/vts-verbose.exe'
		mcs -define:VERBOSE -r:System.Data -r:System.Drawing -r:System.Windows.Forms -out:bin/vts-verbose.exe -main:Jay.VTS.Program *.cs */*.cs
	fi
	if [[ $debug == 'true' ]]; then
		echo 'Debug enabled: compiling debug version to bin/vts-debug.exe'
		mcs -define:VERBOSE -r:System.Data -r:System.Drawing -r:System.Windows.Forms -out:bin/vts-debug.exe -main:Jay.VTS.Program -debug *.cs */*.cs
	fi
	echo 'Compile enabled: compiling release version to bin/vts-parse.exe'
	mcs -r:System.Data -r:System.Drawing -r:System.Windows.Forms -out:bin/vts-parse.exe -main:Jay.VTS.Program *.cs */*.cs
	if [[ $? != 0 ]]; then
		>&2 echo "Failed to compile." 
		exit -1
	fi
fi
if [[ $run == 'true' ]]; then
	mono bin/vts-parse.exe &
fi

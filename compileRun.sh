# !/bin/bash

compile='false'
run='false'

while getopts 'crh' opt; do
	case $opt in
		'c')
			compile='true'
			;;
		'r')
			run='true'
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
	mcs -r:System.Data -r:System.Drawing -r:System.Windows.Forms -out:bin/vts-parse.exe -main:Jay.VTS.Program *.cs */*.cs
	if [[ $? != 0 ]]; then
		>&2 echo "Failed to compile." 
		exit -1
	fi
fi
if [[ $run == 'true' ]]; then
	mono bin/vts-parse.exe &
fi

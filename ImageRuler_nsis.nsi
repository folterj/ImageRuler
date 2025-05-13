# define the name of the installer
Outfile "ImageRuler.exe"
Icon "ImageRuler\Resources\RulersIcon.ico"

InstallDir $TEMP\ImageRulerSetup

AutoCloseWindow true

# default section
Section

HideWindow

SetOutPath $INSTDIR

File /r ImageRulerSetup\Release\*.*
ExecWait "$INSTDIR\setup.exe"

RMDir /r "$INSTDIR"

SectionEnd

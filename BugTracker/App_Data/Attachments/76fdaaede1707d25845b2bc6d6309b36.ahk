#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
; #Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.


msgbox hit enter once you are ready.

loop, Read, Makes.txt
{
	RegExMatch(A_LoopReadLine, "(\w+),\s+(\d+)", CarVar)

	Send % CarVar1
	Send -a{tab}
	Send % CarVar2
	Send {enter}{Left}
	Sleep, 50

	Send % CarVar1
	Send -b{tab}
	Send % CarVar2
	Send {enter}{Left}
	Sleep, 50

	Send % CarVar1
	Send -c{tab}
	Send % CarVar2
	Send {enter}{Left}
	Sleep, 50

}

Pause::Pause
ESC::ExitApp

VaccAppointment

Anleitung:
Installieren:
1) Projekt https://github.com/VinuelMancent/VaccAppointment an einen beliebigen Ort Klonen
2) Im Ordner VaccAppointment sind die 3 gefordeten Diagramme
3) Im Order ../VaccAppointment/VaccAppointment/bin/Debug/net5.0/win-x64 liegt eine VaccAppointment.exe, dies ist die auszuführende Datei
4) Sollte es Probleme damit geben, benötigt man die dotnet5 sdk (https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.302-windows-x64-installer)
5) Dann öffnet man den Ordner ../VaccAppointment/VaccAppointment, dort die Konsole und gibt dann "dotnet run" ein, dies öffnet das Programm direkt in der Konsole
6) Um den Admin sinnvoll nutzen zu können benötigt man Username und Passwort, diese lauten "vincent" und "hatne1verdient"
7) Um den Test selbst durchzuführen benötigt man ebenfalls die dotnet 5 sdk, geht in den ordner VaccAppointment/Testing, öffnet dort die Konsole und schreibt "dotnet test"

Das wars. Interessant ist vielleicht noch, dass der ConsoleManager direkt von mir (und auch in der Solution integriert ist), einzig die Library für den JSON Input/Output habe ich von meiner Firma benutzt. Die zugehörige .dll (Kubernative.Toolchain.dll) liegt unter ../VaccAppointment/VaccAppointment/bin/Debug/net5.0/

Danke für das Semester!

@echo off
FOR %%p IN (0.9 1.0) DO CALL :gen_all_for_protocol %%p

rem cp -f python/mavlink_ardupilotmega_v0.9.py ../mavlink.py
rem cp -f python/mavlink_ardupilotmega_v1.0.py ../mavlinkv10.py

goto :eof

:gen_all_for_protocol
rem  for xml in message_definitions/v$protocol/*.xml; do
FOR %%x IN (message_definitions/v%1/*.xml) DO CALL :gen_for_xml %%x %1
goto :eof
 
:gen_for_xml
set base=%~n1
.\mavgen.py --lang=C --wire-protocol=%2 --output=C\include_v%2 message_definitions\v%2\%1 || goto :exit_fail 1
.\mavgen.py --lang=python --wire-protocol=%2 --output=python\mavlink_%base%_v%2.py message_definitions\v%2\%1 || goto :exit_fail 1
goto :eof
 
:exit_fail
echo exit %1
pause
exit 
 
:eof
pause
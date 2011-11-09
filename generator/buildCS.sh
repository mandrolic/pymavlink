./mavgen.py --lang=CS --output=CS/ardupilotmega_v0.9/mavlink_messages message_definitions/v0.9/ardupilotmega.xml
cp CS/ardupilotmega_v0.9/*.cs CS/include/

pushd CS/include/
xbuild Mavlink_Net3_5.csproj
popd

cd CS/Mavlink_Monitor_Console/
xbuild Mavlink_Monitor_Console.csproj


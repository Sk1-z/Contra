out='Contra'
alias cmd='dotnet build --configuration=Release'

rm windows.zip linux.tar mac.tar

cmd --os=win -o=$out/Windows
cmd --os=linux -o=$out/Linux
cmd --os=osx -o=$out/Mac

zip windows.zip $out/Windows/
tar -cf linux.tar $out/Linux/
tar -cf mac.tar $out/Mac/


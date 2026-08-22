#!/bin/bash

echo "Building BPE5 build..."
dotnet build /p:BPE5=1
cp bin/Debug/netstandard2.1/com.rhythmdr.bpe5rtwitchtrapintegration.dll com.rhythmdr.bpe5rtwitchtrapintegration.dll
echo "Building BPE6 build..."
dotnet build
cp bin/Debug/netstandard2.1/com.rhythmdr.rtwitchtrapintegration.dll com.rhythmdr.rtwitchtrapintegration.dll
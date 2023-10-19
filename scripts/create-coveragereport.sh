#!/bin/bash

testsuccess=1;

dotnet test --collect:"XPlat Code Coverage" || testsuccess=0;
if [ "$testsuccess" == 1 ]
then
    dotnet tool run reportgenerator -reports:"ChatAppTest/TestResults/**/coverage.cobertura.xml" -targetdir:coveragereport;
    rm -R ChatAppTest/TestResults/*;
fi

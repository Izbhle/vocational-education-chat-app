# vocational-education-chat-app
Demonstration of a network Chat application build in C# with an avalonia Gui


## Test Coverage

--collect:"XPlat Code Coverage"


generate test coverage report with with
```sh
dotnet tool run reportgenerator -reports:"ChatAppTest/TestResults/${guid}/coverage.cobertura.xml" -targetdir:"coveragereport"
```
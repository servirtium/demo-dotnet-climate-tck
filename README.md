Note - The World Bank took down their climate WebAPI. Darn it. We now depend on a docker version of the same until we work out what to do long term. Docker build and deploy this locally - https://github.com/servirtium/worldbank-climate-recordings - see README

TL;DR:

```
docker build git@github.com:servirtium/worldbank-climate-recordings.git#main -t worldbank-weather-api-for-servirtium-development
docker run -d -p 4567:4567 worldbank-weather-api-for-servirtium-development
```

The build for this demo project needs that docker container running

# Servirtium demo for .NET

Demonstration project for Servirtium .NET implementation (https://github.com/servirtium/servirtium-dotnet)

This is roughly the sme as the Java example - https://github.com/servirtium/demo-java-climate-tck - but following idioms for .NET

The climate API tested uses a simple programmatic wrapper for World Bank's climate-data service. It can respond to requests with XML or 
JSON payloads, and the `Servirtium.AspNetCore` module can record and payback either. This is a standard showcase for Servirtium.

## Notable source files:

Climate API demo class: [Servirtium.Demo/ClimateApi.cs](https://github.com/servirtium/sdemo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApi.cs). 

Servirtium in use:

* Playback of a Servirtium recording: [Servirtium.Demo/ClimateApiPlaybackTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApiPlaybackTests.cs) (reuses ClimateApiTests.cs - see below)
* Making a Servirtium recording: [Servirtium.Demo/ClimateApiRecordingTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApiRecordingTests.cs) (reuses ClimateApiTests.cs - see below)
* For contrast, direct tests against the climate service (no Servirtium): [Servirtium.Demo/ClimateApiTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApiTests.cs) 

For your own use of Servirtium, you'd do something like the record and playback tests.

## Requirements

1. .NET Core 6 or above
2. Aspnetcore-runtime 3.1.14 or above.

### Mac OS

MacOS requires the mono GDI plus implemtation installed to run the PlanetAPI tests, this can be installed via brew:

`brew install mono-libgdiplus`

## Building and running tests

```
dotnet restore
dotnet build
dotnet test
```

Note - The World Bank took down their climate WebAPI. Darn it. We now depend on a docker version of the same until we work out what to do long term. Docker build and deploy this locally - https://github.com/servirtium/worldbank-climate-recordings - see README

TL;DR:

```
docker build git@github.com:servirtium/worldbank-climate-recordings.git#main -t worldbank-weather-api-for-servirtium-development
docker run -d -p 4567:4567 worldbank-weather-api-for-servirtium-development
```

The build for this demo project needs that docker container running

# Servirtium demo for .NET

Demonstration project for Servirtium .NET implementation (https://github.com/servirtium/servirtium-dotnet)

This repo was build following the step-by-step guide at [https://servirtium.dev/new](https://servirtium.dev/new)

- status: pretty much complete

As well as making a Servirtium library for a language, this step by step guild leaves you with a **contrived** example library that serves as an example of how to use Servirtium.

Someone wanting to see an example of how to use Servirtium for a .NET project would look at ths repo's source. Someone wanting to learn Servirtium by tutorial or extensive reference documentation needs to look elsewhere - sorry!

# Climate API library test harness

A reusable library for .NET usage that gives you average rainfall for a country, is what was made to serve as a test harness for this demo. The test harness in turn uses The world bank's REST Web-APIs - `/climateweb/rest/v1/country/annualavg/pr/{fromCCYY}/{toCCYY}/{countryISO}.xml` for that. See note at top of README.

The demo comes has unit tests and recordings of service interactions for each test.  The recordings are in the [Servirtium.Climate.Demo/test_playbacks](Servirtium.Climate.Demo/test_playbacks) folder.

The library comes with a means to re-record those service interactions, using Servirtium in "record" mode.

Teams evaluating the Servirtium library (but not developing it) would:

* ignore the world back climate API aspect of this (just for the sake of the demo)
* focus on a HTTP service their application uses (but could easily be outside the dev team in question)
* write their own tests (using their preferred test runner - SpecFlow, NBehave are fine choices).
* make servirtium optionally do recordings as a mode of operation (commit those recording to Git)
* enjoy their own builds being fast and always green (versus slow and flaky).
* have a non-CI build (daily/weekly) that attempts to re-record and alert that recordings have changed
* remember to keep secrets out of source control (passwords, API tokens and more).

Service tests facilitated by Servirtium (part of the "integration test" class) are one thing, but there should always be a much smaller number of them than **pure unit tests** (no I/O, less than 10ms each). Teams using this library in a larger application would use traditional in-process mocking (say via [Moq](https://github.com/moq/moq4)) for the pure unit tests. Reliance on "integration tests" for development on localhost (or far worse a named environment like "dev" or "qa") is a fools game.

Another dev team could use the recordings, as is, to make a new implementation of the library for some reason (say they did not like the license). And they need not even have access to localhost:4567. Some companies happily shipping Servirtium service recordings for specific test scenarios may attach a license agreement that forbids reverse engineering (of the closed-source backend, or the shipped library).

## Notable source files:

Climate API demo class: [Servirtium.Climate.Demo/ClimateApi.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Climate.Demo/ClimateApi.cs). 

Servirtium in use:

* Playback of a Servirtium recording: [Servirtium.Climate.Demo/ClimateApiPlaybackTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Climate.Demo/ClimateApiPlaybackTests.cs) (reuses ClimateApiTests.cs - see below)
* Making a Servirtium recording: [Servirtium.Climate.Demo/ClimateApiRecordingTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Climate.Demo/ClimateApiRecordingTests.cs) (reuses ClimateApiTests.cs - see below)
* For contrast, direct tests against the climate service (no Servirtium): [Servirtium.Climate.Demo/ClimateApiTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Climate.Demo/ClimateApiTests.cs) 

For your own use of Servirtium, you'd do something like the record and playback tests.

## Requirements

1. .NET Core 6 or above
2. Aspnetcore-runtime 3.1.14 or above.

### Mac OS instructions

MacOS requires the mono GDI plus implementation installed to run the PlanetAPI tests, this can be installed via brew:

`brew install mono-libgdiplus`

## Building and running tests

```
$ dotnet restore
$ dotnet build
$ dotnet test
```

There are 18 NUnit tests in this technology compatibility kit (TCK) project that serves as a demo.

* 6 tests that don't use Servirtium and directly invoke services on WorldBank.com's climate endpoint.
* 6 tests that do the above, but also record the interactions via Servirtium
* 6 tests that don't at all use WorldBank (or need to be online), but instead use the recordings in the above via Servirtium

## Psuedocode for the 6 tests:

```
test_averageRainfallForGreatBritainFrom1980to1999Exists()
    assert climateApi.getAveAnnualRainfall(1980, 1999, "gbr") == 988.8454972331015

test_averageRainfallForFranceFrom1980to1999Exists()
    assert climateApi.getAveAnnualRainfall(1980, 1999, "fra") == 913.7986955122727

test_averageRainfallForEgyptFrom1980to1999Exists()
    assert climateApi.getAveAnnualRainfall(1980, 1999, "egy") == 54.58587712129825

test_averageRainfallForGreatBritainFrom1985to1995DoesNotExist()
    climateApi.getAveAnnualRainfall(1985, 1995, "gbr")
    ... causes "date range not supported" 

test_averageRainfallForMiddleEarthFrom1980to1999DoesNotExist()
    climateApi.getAveAnnualRainfall(1980, 1999, "mde")
    ... causes "bad country code"

test_averageRainfallForGreatBritainAndFranceFrom1980to1999CanBeCalculatedFromTwoRequests()
    assert climateApi.getAveAnnualRainfall(1980, 1999, "gbr", "fra") == 951.3220963726872
```

### Climate API tests - direct (no Servirtium)

```
$ cd Servirtium.Climate.Demo
$ dotnet test --filter ClimateApiDirectTests
Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7, Duration: 35 ms
```

This mode of operation is important as you want to prove a bug is or is not in Servirtium itself.

### Climate API tests - record mode

```
$ cd Servirtium.Climate.Demo
$ dotnet test --filter ClimateApiRecordingTests
Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7, Duration: 1 s
```

You'd have a non-CI build (Jenkins, etc) that would attempt to re-record the interactions and fail if there were differences. Humans would investigate at the next opportunity (as a priority) to see why recording would differ. Specifically, on their own machine, not in the cloud-based build farm.

### Climate API tests - playback mode

```
$ cd Servirtium.Climate.Demo
$ dotnet test --filter ClimateApiRecordingTests
Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7, Duration: 590 ms
```

Note the playback mode is quickest. Your day to dey development of you main applications functionality would rely on this mode of operation. It would ideally have a "--servirtium-playback-mode-for-out-of-team-services" but different test frameworks have different ways of doing that from command-line settings